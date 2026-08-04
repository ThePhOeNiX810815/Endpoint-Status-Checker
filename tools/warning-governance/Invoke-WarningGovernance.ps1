param(
    [Parameter(Mandatory = $false)]
    [ValidateSet('capture', 'check')]
    [string]$Mode = 'check',

    [Parameter(Mandatory = $false)]
    [string]$SolutionPath = 'src/EndpointChecker.sln',

    [Parameter(Mandatory = $false)]
    [string]$BaselinePath = 'docs/refactoring/warning-baseline.unique.json'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Invoke-Dotnet {
    param(
        [string[]]$CommandArgs,
        [string]$LogPath
    )

    Write-Host ("Running: dotnet " + ($CommandArgs -join ' '))
    $output = & dotnet @CommandArgs 2>&1
    $exitCode = $LASTEXITCODE

    $output | Set-Content -Path $LogPath -Encoding UTF8

    return [pscustomobject]@{
        ExitCode = $exitCode
        Output = $output
        LogPath = $LogPath
    }
}

function Get-WarningCodeFamily {
    param([string]$Code)

    if ([string]::IsNullOrWhiteSpace($Code)) {
        return 'UNKNOWN'
    }

    $match = [regex]::Match($Code, '^[A-Za-z]+')
    if ($match.Success) {
        return $match.Value.ToUpperInvariant()
    }

    return 'UNKNOWN'
}

function Parse-WarningsFromLog {
    param(
        [string]$LogPath,
        [string]$Phase
    )

    $lineRegexWithFile = '^(?<file>.+?)\((?<line>\d+),(?<column>\d+)\):\s+warning\s+(?<code>[A-Za-z]+\d+):\s+(?<message>.+?)\s+\[(?<project>.+?)\]\s*$'
    $lineRegexWithoutFile = '^warning\s+(?<code>[A-Za-z]+\d+):\s+(?<message>.+?)(\s+\[(?<project>.+?)\])?\s*$'

    $warnings = New-Object System.Collections.Generic.List[object]

    foreach ($line in Get-Content -Path $LogPath) {
        $withFile = [regex]::Match($line, $lineRegexWithFile)
        if ($withFile.Success) {
            $warnings.Add([pscustomobject]@{
                Phase = $Phase
                File = $withFile.Groups['file'].Value.Trim()
                Line = [int]$withFile.Groups['line'].Value
                Column = [int]$withFile.Groups['column'].Value
                Code = $withFile.Groups['code'].Value.Trim().ToUpperInvariant()
                Message = $withFile.Groups['message'].Value.Trim()
                Project = $withFile.Groups['project'].Value.Trim()
            })
            continue
        }

        $withoutFile = [regex]::Match($line.Trim(), $lineRegexWithoutFile)
        if ($withoutFile.Success) {
            $project = ''
            if ($withoutFile.Groups['project'].Success) {
                $project = $withoutFile.Groups['project'].Value.Trim()
            }

            $warnings.Add([pscustomobject]@{
                Phase = $Phase
                File = ''
                Line = 0
                Column = 0
                Code = $withoutFile.Groups['code'].Value.Trim().ToUpperInvariant()
                Message = $withoutFile.Groups['message'].Value.Trim()
                Project = $project
            })
        }
    }

    return @($warnings.ToArray())
}

function Get-UniqueWarningInstances {
    param([object[]]$Warnings)

    $seen = New-Object 'System.Collections.Generic.HashSet[string]'
    $unique = New-Object System.Collections.Generic.List[object]

    foreach ($warning in $Warnings) {
        $key = ('{0}|{1}|{2}|{3}|{4}|{5}' -f [string]$warning.Code, [string]$warning.Project, [string]$warning.File, [string]$warning.Line, [string]$warning.Column, [string]$warning.Message)
        if ($seen.Add($key)) {
            $unique.Add($warning)
        }
    }

    return @($unique.ToArray())
}

function Group-Counts {
    param(
        [object[]]$Warnings,
        [scriptblock]$Selector
    )

    $map = @{}
    foreach ($warning in $Warnings) {
        $key = & $Selector $warning
        if (-not $map.ContainsKey($key)) {
            $map[$key] = 0
        }

        $map[$key]++
    }

    return $map
}

function Convert-HashtableToOrderedObject {
    param([hashtable]$Table)

    $ordered = [ordered]@{}
    foreach ($key in ($Table.Keys | Sort-Object)) {
        $ordered[$key] = $Table[$key]
    }

    return [pscustomobject]$ordered
}

function New-Snapshot {
    param(
        [string]$Solution,
        [string]$RestoreLogPath,
        [string]$BuildLogPath
    )

    $restoreWarnings = @(Parse-WarningsFromLog -LogPath $RestoreLogPath -Phase 'restore')
    $buildWarnings = @(Parse-WarningsFromLog -LogPath $BuildLogPath -Phase 'build')

    $restoreUnique = @(Get-UniqueWarningInstances -Warnings $restoreWarnings)
    $buildUnique = @(Get-UniqueWarningInstances -Warnings $buildWarnings)

    $buildKeySet = New-Object 'System.Collections.Generic.HashSet[string]'
    foreach ($warning in $buildUnique) {
        $key = ($warning.Code + '|' + $warning.Project + '|' + $warning.File + '|' + $warning.Line + '|' + $warning.Column + '|' + $warning.Message)
        $null = $buildKeySet.Add($key)
    }

    $restoreOnly = New-Object System.Collections.Generic.List[object]
    foreach ($warning in $restoreUnique) {
        $key = ($warning.Code + '|' + $warning.Project + '|' + $warning.File + '|' + $warning.Line + '|' + $warning.Column + '|' + $warning.Message)
        if (-not $buildKeySet.Contains($key)) {
            $restoreOnly.Add($warning)
        }
    }

    $byCode = Group-Counts -Warnings $buildUnique -Selector { param($w) $w.Code }
    $byFamily = Group-Counts -Warnings $buildUnique -Selector { param($w) Get-WarningCodeFamily -Code $w.Code }
    $byProject = Group-Counts -Warnings $buildUnique -Selector { param($w) if ([string]::IsNullOrWhiteSpace($w.Project)) { 'UNKNOWN' } else { $w.Project } }

    $generatedCodeWarnings = New-Object System.Collections.Generic.List[object]
    foreach ($warning in $buildUnique) {
        if ($warning.File -match '(\\obj\\|\.Designer\.cs$|\.g\.cs$|\.g\.i\.cs$)') {
            $generatedCodeWarnings.Add($warning)
        }
    }

    $generatedByCode = Group-Counts -Warnings $generatedCodeWarnings -Selector { param($w) $w.Code }

    $snapshot = [ordered]@{
        convention = 'unique-build-warning-instances'
        generated_at_utc = [DateTime]::UtcNow.ToString('o')
        dotnet_sdk = (& dotnet --version)
        solution = $Solution
        commands = [ordered]@{
            restore = "dotnet restore $Solution"
            analyzer_build = "dotnet build $Solution --no-restore -p:RunAnalyzers=true -v:minimal"
        }
        counts = [ordered]@{
            restore_raw_warning_lines = $restoreWarnings.Count
            restore_unique_warning_instances = $restoreUnique.Count
            build_raw_warning_lines = $buildWarnings.Count
            build_unique_warning_instances = $buildUnique.Count
            repeated_build_warning_lines = ($buildWarnings.Count - $buildUnique.Count)
            restore_only_unique_warning_instances = $restoreOnly.Count
        }
        by_code = Convert-HashtableToOrderedObject -Table $byCode
        by_family = Convert-HashtableToOrderedObject -Table $byFamily
        by_project = Convert-HashtableToOrderedObject -Table $byProject
        generated_code = [ordered]@{
            unique_warning_instances = $generatedCodeWarnings.Count
            by_code = Convert-HashtableToOrderedObject -Table $generatedByCode
        }
        platform_compatibility = [ordered]@{
            CA1416 = if ($byCode.ContainsKey('CA1416')) { $byCode['CA1416'] } else { 0 }
        }
        restore_only_warnings = $restoreOnly
    }

    return [pscustomobject]$snapshot
}

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..')
Set-Location $repoRoot

$restoreLog = Join-Path $env:TEMP 'endpointchecker-warning-restore.log'
$buildLog = Join-Path $env:TEMP 'endpointchecker-warning-build.log'

$restoreResult = Invoke-Dotnet -CommandArgs @('restore', $SolutionPath) -LogPath $restoreLog
if ($restoreResult.ExitCode -ne 0) {
    Write-Error "Restore failed. See $restoreLog"
}

$buildResult = Invoke-Dotnet -CommandArgs @('build', $SolutionPath, '--no-restore', '-p:RunAnalyzers=true', '-v:minimal') -LogPath $buildLog
if ($buildResult.ExitCode -ne 0) {
    Write-Error "Analyzer build failed. See $buildLog"
}

$current = New-Snapshot -Solution $SolutionPath -RestoreLogPath $restoreLog -BuildLogPath $buildLog

Write-Host "Build unique warning instances: $($current.counts.build_unique_warning_instances)"
Write-Host "Build repeated warning lines: $($current.counts.repeated_build_warning_lines)"
Write-Host "Restore-only unique warnings: $($current.counts.restore_only_unique_warning_instances)"

if ($Mode -eq 'capture') {
    $baselineFile = Resolve-Path -Path (Join-Path $repoRoot $BaselinePath) -ErrorAction SilentlyContinue
    if ($null -eq $baselineFile) {
        $baselineDirectory = Split-Path -Path (Join-Path $repoRoot $BaselinePath) -Parent
        if (-not (Test-Path $baselineDirectory)) {
            New-Item -ItemType Directory -Path $baselineDirectory | Out-Null
        }

        $current | ConvertTo-Json -Depth 8 | Set-Content -Path (Join-Path $repoRoot $BaselinePath) -Encoding UTF8
    }
    else {
        $current | ConvertTo-Json -Depth 8 | Set-Content -Path $baselineFile -Encoding UTF8
    }

    Write-Host "Captured baseline at $BaselinePath"
    exit 0
}

$baselineFullPath = Join-Path $repoRoot $BaselinePath
if (-not (Test-Path $baselineFullPath)) {
    Write-Error "Baseline file not found: $BaselinePath"
}

$baseline = Get-Content -Path $baselineFullPath -Raw | ConvertFrom-Json

$regressions = New-Object System.Collections.Generic.List[string]

$baselineCodes = @{}
$baseline.by_code.PSObject.Properties | ForEach-Object { $baselineCodes[$_.Name] = [int]$_.Value }

$currentCodes = @{}
$current.by_code.PSObject.Properties | ForEach-Object { $currentCodes[$_.Name] = [int]$_.Value }

foreach ($code in $currentCodes.Keys) {
    if (-not $baselineCodes.ContainsKey($code)) {
        $regressions.Add("New warning code detected: $code ($($currentCodes[$code]))")
        continue
    }

    if ($currentCodes[$code] -gt $baselineCodes[$code]) {
        $regressions.Add("Warning code $code increased: baseline=$($baselineCodes[$code]), current=$($currentCodes[$code])")
    }
}

if ([int]$current.counts.build_unique_warning_instances -gt [int]$baseline.counts.build_unique_warning_instances) {
    $regressions.Add("Total unique build warnings increased: baseline=$($baseline.counts.build_unique_warning_instances), current=$($current.counts.build_unique_warning_instances)")
}

if ($regressions.Count -gt 0) {
    Write-Host "Warning governance regression detected:" -ForegroundColor Red
    foreach ($message in $regressions) {
        Write-Host " - $message" -ForegroundColor Red
    }

    exit 1
}

Write-Host "Warning governance check passed."
exit 0
