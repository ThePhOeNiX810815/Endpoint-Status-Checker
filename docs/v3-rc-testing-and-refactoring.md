# v3.1.1 RC1 Testing and Refactoring Guide

## Scope

This document defines how v3.1.1 RC1 is packaged, validated, and documented on the `Main-Dev-V3` line.

## Release Candidate Identity

- Version: `3.1.1.1` (RC build metadata)
- Tag: `v3.1.1-rc1`
- Distribution: GitHub prerelease asset only
- Asset name: `EndpointChecker-v3.1.1-rc1-test.zip`

## Update-Safety Rules

The RC must not affect v2.15 update behavior.

- Do not modify `Main-Dev-Branch/version.txt`.
- Do not modify `Main-Dev-Branch/package.txt`.
- Keep v3 update checks disabled in source (`app_UpdateChecksEnabled = false`).
- Publish as prerelease on `Main-Dev-V3`, not as a stable release.

## Build and Signing

### Publish command (self-contained x86)

```powershell
dotnet publish src/EndpointChecker.csproj -c Release -r win-x86 -o C:\TEMP\Checker\RC1-<timestamp>
```

### Code-signing expectation

- Sign `EndpointChecker.exe` with certificate subject `CN=David Smidke` from the local certificate store.
- Verify signature status before zipping.

## Test Strategy

## What runs during build

- `dotnet build` compiles sources but does not execute test suites automatically.
- Tests in this repository run via explicit `dotnet run` commands.

### Core test suites

```powershell
dotnet run --project tests/EndpointDefinitionParser.Tests/EndpointDefinitionParser.Tests.csproj
dotnet run --project tests/EndpointCheckingCore.Tests/EndpointCheckingCore.Tests.csproj
dotnet run --project tests/HttpCompatibility.Tests/HttpCompatibility.Tests.csproj
dotnet run --project tests/ExportOptions.Tests/ExportOptions.Tests.csproj
dotnet run --project tests/ExportRunSummary.Tests/ExportRunSummary.Tests.csproj
dotnet run --project tests/ExportFileSet.Tests/ExportFileSet.Tests.csproj
dotnet run --project tests/StructuredExport.Tests/StructuredExport.Tests.csproj
```

### Recommended pre-publish build check

```powershell
dotnet build src/EndpointChecker.sln -c Release
```

## Refactoring References

- Full programme and ticket progression: [../refactoring-programme.md](../refactoring-programme.md)
- Residual risk and deferred cleanups: [../refactoring-residual-findings.md](../refactoring-residual-findings.md)
- Reconciliation and closure matrix: [refactoring/reconciliation-2026-08-04.md](refactoring/reconciliation-2026-08-04.md)

## RC Publication Checklist

1. Confirm branch is `Main-Dev-V3`.
2. Build Release and run all explicit test suites.
3. Publish self-contained x86 output.
4. Sign `EndpointChecker.exe` using the `David Smidke` certificate.
5. Zip the published folder as `EndpointChecker-v3.1.1-rc1-test.zip`.
6. Create prerelease tag `v3.1.1-rc1` targeted to `Main-Dev-V3`.
7. Upload the ZIP as the prerelease asset.
8. Keep v2.15 updater source files unchanged.