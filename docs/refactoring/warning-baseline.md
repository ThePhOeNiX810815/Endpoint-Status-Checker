# Endpoint Status Checker v3 Warning Baseline

## Priority 6 update (2026-08-04)

Warning governance now uses a reproducible machine-readable convention captured by:

- `tools/warning-governance/Invoke-WarningGovernance.ps1`
- `docs/refactoring/warning-baseline.unique.json`

Counting convention used by Priority 6:

- authoritative metric: **unique build warning instances** from analyzer build output
- uniqueness key: `warning code + project + file + line + column + message`
- supporting metrics:
	- raw warning lines in restore/build logs
	- repeated build warning lines (raw minus unique)
	- restore-only warning instances (present during restore, absent during build)
	- warning families (for example `CA`, `SYSLIB`, `NU`)
	- project-specific distribution
	- generated-code warning distribution
	- platform compatibility warning slice (`CA1416`)

Current clean analyzer rebuild snapshot (`warning-baseline.unique.json`):

- restore raw warning lines: `0`
- restore unique warning instances: `0`
- build raw warning lines: `0`
- build unique warning instances: `0`
- repeated build warning lines: `0`
- restore-only unique warning instances: `0`

CI regression policy now checks this baseline and fails when:

- a new warning code appears
- an existing warning code count increases
- total unique build warning instances increase

CI integration:

- `.github/workflows/warning-governance.yml` runs on `push` and `pull_request` targeting `Main-Dev-V3`.

Ticket: 6 - compiler and static-analysis warning baseline

Base branch: `Main-Dev-V3`

Feature branch: `cleanup/v3-warning-baseline`

Starting commit: `4a6ed9a092a7365a36893f9805c27895d8ec6d5d`

## Configuration inspected

The repository contains one application solution, `src/EndpointChecker.sln`, with the main project at `src/EndpointChecker.csproj`. Additional standalone test projects are under `tests/`, and the updater project is under `tools/EndpointChecker-Updater/`.

No repository-level `.editorconfig`, `Directory.Build.props`, `Directory.Build.targets`, or GitHub workflow files were present when this baseline was captured.

The main application project targets `net10.0-windows`, enables WinForms with `UseWindowsForms=true`, sets `PlatformTarget=x86`, disables nullable annotations with `Nullable=disable`, disables implicit usings with `ImplicitUsings=disable`, and does not treat warnings as errors. The only configured warning exclusion is `WFO1000`. No explicit analyzer package references are present, so the warnings below come from the .NET SDK/compiler analyzers.

Captured toolchain:

- .NET SDK: `10.0.110`
- MSBuild: `18.0.11`
- Host runtime: `10.0.10`
- `global.json`: not present

## Commands used

Baseline before fixes:

```bash
dotnet restore src/EndpointChecker.csproj -p:EnableWindowsTargeting=true
dotnet clean src/EndpointChecker.csproj -p:EnableWindowsTargeting=true
dotnet build src/EndpointChecker.csproj --no-restore -p:EnableWindowsTargeting=true -v:minimal
```

Verification after fixes:

```bash
dotnet build src/EndpointChecker.csproj --no-restore -p:EnableWindowsTargeting=true -p:RunAnalyzers=true -v:minimal
```

## Warning inventory

The build log prints each warning once at the call site and again in the build summary. Counts below intentionally reflect the captured build output.

| Warning | Before | After | Classification | Status |
| --- | ---: | ---: | --- | --- |
| `CA1416` | 7504 | 7504 | Category C/D | Retained |
| `SYSLIB0003` | 36 | 0 | Category B | Fixed |
| `SYSLIB0014` | 34 | 34 | Category C | Retained |
| `CA2200` | 12 | 0 | Category B | Fixed |
| `SYSLIB0057` | 2 | 2 | Category C | Retained |
| `SYSLIB0013` | 2 | 2 | Category C | Retained |
| `NU1900` | 2 | 2 | Category D | Retained |
| Total | 7592 | 7544 |  | 48 warnings reduced |

## Remaining warning inventory by file

| Warning | File/project | Count | Reason retained | Recommended future action |
| --- | --- | ---: | --- | --- |
| `CA1416` | `src/CheckerMainForm.cs` | 3832 | WinForms and Windows-only APIs are used throughout a Windows desktop application. Changing the platform annotations or project settings should be handled as a dedicated compatibility decision. | Add an explicit Windows platform support strategy after confirming packaging, analyzer, and CI expectations. |
| `CA1416` | `src/SpeedTestDialog.cs` | 1234 | Same WinForms platform analyzer baseline. | Same as above. |
| `CA1416` | `src/EndpointDetailsDialog.cs` | 1088 | Same WinForms platform analyzer baseline, plus Windows-only management/network inspection APIs. | Same as above, with characterization around endpoint details behavior. |
| `CA1416` | `src/ConfigDialog.cs` | 502 | Same WinForms platform analyzer baseline. | Same as above. |
| `CA1416` | `src/EndpointManagementDialog.cs` | 326 | Same WinForms platform analyzer baseline. | Same as above. |
| `CA1416` | `src/FeatureRequestDialog.cs` | 128 | Same WinForms platform analyzer baseline. | Same as above. |
| `CA1416` | `src/ExceptionDialog.cs` | 112 | Same WinForms platform analyzer baseline. | Same as above. |
| `CA1416` | `src/CloudflareBypassSettingsDialog.cs` | 68 | Same WinForms platform analyzer baseline. | Same as above. |
| `CA1416` | `src/SplashScreen.cs` | 58 | Same WinForms platform analyzer baseline. | Same as above. |
| `CA1416` | `src/AutoUpdaterDialog.cs` | 56 | Same WinForms platform analyzer baseline. | Same as above. |
| `CA1416` | `src/Program.cs` | 54 | Windows-only application bootstrap and dialog usage. | Same as above. |
| `CA1416` | `src/NewVersionDialog.cs` | 34 | Same WinForms platform analyzer baseline. | Same as above. |
| `CA1416` | `src/FontPublisher.cs` | 12 | Windows font installation API usage. | Add targeted platform annotations only after validating installer/update behavior. |
| `SYSLIB0014` | `src/EndpointDetailsDialog.cs` | 14 | `WebRequest`/`HttpWebRequest` behavior is compatibility-sensitive for status codes, redirects, SSL handling, authentication, and Cloudflare responses. | Replace only behind characterization tests for redirect, authentication, SSL failure, timeout, and Cloudflare scenarios. |
| `SYSLIB0014` | `src/Program.cs` | 10 | Auto-update and connectivity paths rely on legacy networking behavior. | Characterize updater download/proxy/TLS behavior before migration. |
| `SYSLIB0014` | `src/CheckerMainForm.cs` | 6 | Main endpoint checking still uses legacy networking compatibility paths. | Continue extracting and testing HTTP behavior before replacing APIs. |
| `SYSLIB0014` | `src/NSpeedTest/SpeedTestWebClient.cs` | 2 | Speed-test library code uses `WebClient`; changing it risks speed-test behavior. | Isolate speed-test HTTP behavior before replacing. |
| `SYSLIB0014` | `src/EndpointHttpRequestFactory.cs` | 2 | Central request factory intentionally preserves `HttpWebRequest` semantics. | Treat as the eventual migration boundary once compatibility tests are complete. |
| `SYSLIB0013` | `src/EndpointDefinitionParser.cs` | 2 | URL parsing and normalization are compatibility-sensitive; `EscapeDataString` is not a drop-in replacement for full URI strings. | Add parser characterization cases before changing URI escaping. |
| `SYSLIB0057` | `src/Program.cs` | 2 | Signed-file certificate loading affects updater/trust behavior. | Migrate to `X509CertificateLoader` only with signed/unsigned update package tests. |
| `NU1900` | `src/EndpointChecker.csproj` | 2 | NuGet vulnerability data could not be loaded from `https://api.nuget.org/v3/index.json` in the current network-restricted environment. | Re-run restore/build in CI or an environment with NuGet vulnerability feed access. |

## Warnings fixed

`SYSLIB0003` was fixed by removing `SecurityPermissionAttribute` constructor demands from WinForms classes. Code Access Security is not supported or honored by the current runtime, so retaining these attributes only produced obsolete API warnings.

Affected files:

- `src/AutoUpdaterDialog.cs`
- `src/CheckerMainForm.cs`
- `src/EndpointDetailsDialog.cs`
- `src/NewVersionDialog.cs`
- `src/SpeedTestDialog.cs`
- `src/SplashScreen.cs`

`CA2200` was fixed by replacing `throw ex;`, `throw wEX;`, and `throw eX;` with `throw;` inside the original catch blocks. This preserves the thrown exception while retaining the original stack information.

Affected files:

- `src/AutoUpdaterDialog.cs`
- `src/CheckerMainForm.cs`
- `src/SpeedTestDialog.cs`

## Future baseline policy

This ticket does not add global suppressions, disable analyzers, lower warning levels, add blanket nullable directives, or introduce a new CI baseline mechanism. The repository did not already contain warning-baseline infrastructure or workflows to extend.

Future work should make warnings visible in CI without hiding the current inventory. A practical next step is a CI job that captures the warning counts by code and fails when counts increase, while allowing dedicated tickets to reduce specific groups.

## Verification results

Restore, clean, build, and analyzer execution completed successfully for `src/EndpointChecker.csproj` with `EnableWindowsTargeting=true`. The final analyzer-enabled build completed with `7544` warnings and `0` errors.

All standalone test projects passed:

- `tests/StructuredExport.Tests/StructuredExport.Tests.csproj`
- `tests/ExportFileSet.Tests/ExportFileSet.Tests.csproj`
- `tests/ExportRunSummary.Tests/ExportRunSummary.Tests.csproj`
- `tests/ExportOptions.Tests/ExportOptions.Tests.csproj`
- `tests/HttpCompatibility.Tests/HttpCompatibility.Tests.csproj`
- `tests/EndpointCheckingCore.Tests/EndpointCheckingCore.Tests.csproj`
- `tests/EndpointDefinitionParser.Tests/EndpointDefinitionParser.Tests.csproj`

`dotnet format src/EndpointChecker.sln --verify-no-changes --no-restore` was also run. The first sandboxed attempt failed because Roslyn/MSBuild named-pipe access was denied. The command was re-run outside the sandbox and reported pre-existing whitespace formatting issues across broad source areas, including `AutoUpdaterDialog.cs` and `CheckerMainForm.cs`. Those formatting findings were not corrected in this ticket because applying repository-wide formatting would create unrelated churn and make the warning-baseline changes harder to review.

## Remaining technical debt

The largest remaining group is `CA1416`. It is mostly a project/platform annotation issue for a WinForms application built on a non-Windows host with `EnableWindowsTargeting=true`. Fixing it safely should be treated as a build-configuration and packaging compatibility ticket, not a local source cleanup.

The remaining `SYSLIB0014`, `SYSLIB0013`, and `SYSLIB0057` warnings are all in compatibility-sensitive code paths. They should be addressed only after tests cover redirects, SSL/certificate handling, authentication, proxy behavior, timeouts, cancellation, updater downloads, URI normalization, and Cloudflare-related responses.
