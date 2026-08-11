# v3.1.1 RC2 Testing and Refactoring Guide

## Scope

This document defines how v3.1.1 RC2 is packaged, validated, and documented on the
`Main-Dev-V3` line. RC2 supersedes RC1 — it is intended to be published **instead of** RC1
once local testing is approved, not as an additional release alongside it.

Work for RC2 happens on ticket branch `feature/v3.1.1-rc2-prep`, PR'd into `Main-Dev-V3`
only after approval (see "RC2 Publication Checklist" below).

## Branch Policy

- v3 branch: `Main-Dev-V3`
- v3 rule: no merging with v2 is allowed, and no v3 change may affect v2 behavior, updater flow, or release delivery.
- v2 branch and default mainline: `Main-Dev-V2`
- v2 work must be kept separate and ported intentionally when needed.

## Release Candidate Identity

- Version: `3.1.1.2` (RC build metadata — the 4th component identifies the RC number)
- Tag: `v3.1.1-rc2`
- Distribution: GitHub prerelease asset only
- Asset name: `EndpointChecker-v3.1.1-rc2-test.zip`

## Update-Safety Rules (revised for RC2)

RC1 kept `app_UpdateChecksEnabled = false` because no v3 update feed existed yet. That's no
longer the case — `Main-Dev-V3/version.txt` is live and already serving real data (confirmed
by directly querying it). RC2 changes the safety model from "checks are off" to "checks are
on, with code-level guardrails":

- `CheckForUpdate()` selects its feed by `app_Version.Major` — a v2 build only ever reads
  `Main-Dev-Branch/version.txt`; a v3 build only ever reads `Main-Dev-V3/version.txt`. Neither
  line can be compared against the other's feed.
- A v2 install can never be offered a v3 package regardless of feed content (major-version
  ceiling enforced in code, not just by feed separation) — v2.15 → v3 stays manual-updater-only.
- A release-candidate build (`Version.Revision > 0`) is never silently auto-installed, even
  with "auto-update in future" enabled — the user is always shown a "test build" confirmation.
- The update-check `WebClient` uses a short (3s) timeout specifically at this call site, so a
  slow or blocked network can't turn into a long, silent hang before the app's first window
  appears.
- `Main-Dev-Branch/version.txt` / `package.txt` are still not modified by this RC.

## Build and Signing

### Publish command (self-contained x86)

```powershell
dotnet publish src/EndpointChecker.csproj -c Release -r win-x86 -o C:\TEMP\Checker\RC2-<timestamp>
```

### Code-signing expectation

- Sign `EndpointChecker.exe` with a trusted self-signed certificate from the local certificate store.
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

## RC2 Publication Checklist

0. Local sign-off on `feature/v3.1.1-rc2-prep` test build, then open a PR into `Main-Dev-V3`
   and merge it. Everything below happens on `Main-Dev-V3` after that merge.
1. Confirm branch is `Main-Dev-V3`.
2. Build Release and run all explicit test suites.
3. Publish self-contained x86 output.
4. Sign `EndpointChecker.exe` using the expected self-signed certificate for the local release workflow.
5. Zip the published folder as `EndpointChecker-v3.1.1-rc2-test.zip`.
6. Create prerelease tag `v3.1.1-rc2` targeted to `Main-Dev-V3`.
7. Upload the ZIP as the prerelease asset.
8. Mark the existing `v3.1.1-rc1` prerelease as superseded (RC2 replaces it, not alongside it).
9. Once ready, bump `Main-Dev-V3/version.txt` to `3.1.1.2` so existing 3.0+ installs are
   offered the update (see the enabled, per-major-version-feed in-app updater above).
10. Keep v2.15 updater source files unchanged.
