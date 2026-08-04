# Endpoint Status Checker v3 Warning Governance Baseline

Date: 2026-08-04  
Base branch: Main-Dev-V3

## Priority 6 status under current permissions

- COMPLETE LOCALLY
- CI ENFORCEMENT BLOCKED BY PERMISSIONS
- OWNER/AUTHORIZED MAINTAINER ACTION REQUIRED

This repository currently has no .github/workflows directory. CI enforcement cannot be implemented by this agent due to explicit repository permission constraints.

## Authoritative counting convention

Governance is implemented by:

- tools/warning-governance/Invoke-WarningGovernance.ps1
- docs/refactoring/warning-baseline.unique.json

Authoritative metric:

- unique build warning instances from analyzer build output

Uniqueness key:

- warning code + project + file + line + column + message

Deterministic build command used by the script:

- dotnet build src/EndpointChecker.sln --no-restore --no-incremental -p:RunAnalyzers=true -v:minimal

Why non-incremental is required:

- incremental builds can hide warning lines when projects are up-to-date
- non-incremental ensures stable warning inventory on every run

Supporting metrics produced:

- restore_raw_warning_lines
- restore_unique_warning_instances
- build_raw_warning_lines
- build_unique_warning_instances
- repeated_build_warning_lines
- restore_only_unique_warning_instances
- by_code
- by_family
- by_project
- generated_code
- platform_compatibility

Regression detection behavior in check mode:

- fail on any new warning code
- fail on any existing warning code count increase
- fail on any new warning family
- fail on any existing warning family count increase
- fail when total unique build warning instances increase

Exit behavior:

- 0 = pass
- 1 = regression detected

## Current baseline snapshot

Baseline file:

- docs/refactoring/warning-baseline.unique.json

Current baseline values (captured from clean Main-Dev-V3):

- restore_raw_warning_lines: 0
- restore_unique_warning_instances: 0
- build_raw_warning_lines: 7602
- build_unique_warning_instances: 3799
- repeated_build_warning_lines: 3803
- restore_only_unique_warning_instances: 0

Current warning families:

- CA: 3781
- SYSLIB: 18

Top warning codes:

- CA1416: 3781
- SYSLIB0014: 16
- SYSLIB0013: 1
- SYSLIB0057: 1

## Local reproduction commands

Capture baseline:

```powershell
powershell -ExecutionPolicy Bypass -File tools/warning-governance/Invoke-WarningGovernance.ps1 -Mode capture
```

Check baseline:

```powershell
powershell -ExecutionPolicy Bypass -File tools/warning-governance/Invoke-WarningGovernance.ps1 -Mode check
```

## Local validation evidence (pass/fail/pass)

1. Baseline success (clean state)
- Command: powershell -ExecutionPolicy Bypass -File tools/warning-governance/Invoke-WarningGovernance.ps1 -Mode check
- Result: Warning governance check passed.
- Exit code: 0
- Output summary:
  - Build unique warning instances: 3799
  - Warning families: CA=3781, SYSLIB=18
  - No new warning codes reported

2. Intentional regression failure
- Temporary probe added: src/WarningGovernanceRegressionProbe.cs containing #warning Priority6RegressionProbe
- Command: powershell -ExecutionPolicy Bypass -File tools/warning-governance/Invoke-WarningGovernance.ps1 -Mode check
- Result: Warning governance regression detected.
- Exit code: 1
- Output summary:
  - Build unique warning instances: 3800
  - New warning code detected: CS1030 (1)
  - New warning family detected: CS (1)
  - Total unique build warnings increased: baseline=3799, current=3800

3. Restored success after full revert
- Temporary probe file removed completely
- Command: powershell -ExecutionPolicy Bypass -File tools/warning-governance/Invoke-WarningGovernance.ps1 -Mode check
- Result: Warning governance check passed.
- Exit code: 0
- Output summary:
  - Build unique warning instances: 3799
  - No regression remains

## Authorized maintainer CI integration handoff

CI wiring is not implemented in this branch because workflow file changes are permission-blocked.

OWNER/AUTHORIZED MAINTAINER ACTION REQUIRED

1. Workflow target
- If a PR/build workflow already exists for Main-Dev-V3, add a warning-governance step there.
- If no suitable workflow exists, create one under .github/workflows (maintainer only).

2. Required trigger filter
- pull_request targeting Main-Dev-V3
- optionally push targeting Main-Dev-V3

3. Required runner
- windows-latest recommended

4. Required SDK
- .NET 10 SDK (10.0.x)

5. Required governance command

```powershell
powershell -ExecutionPolicy Bypass -File tools/warning-governance/Invoke-WarningGovernance.ps1 -Mode check
```

6. Expected behavior
- success: exit code 0
- regression: exit code 1

7. Expected artifacts consumed
- tools/warning-governance/Invoke-WarningGovernance.ps1
- docs/refactoring/warning-baseline.unique.json

8. Expected output examples
- Green run includes: Warning governance check passed.
- Failing run includes: Warning governance regression detected: ...

9. Workflow permissions
- default repository read permissions are sufficient for this check step
- no special deployment or write scopes required

## Guardrails retained

- No global warning suppression introduced
- No analyzer severity reduction introduced
- No blanket warnings-as-errors flip introduced
- No dependency additions introduced
- No GitHub Actions file edits in this ticket
