# Roadmap

## Current State (2026-08-11)

- Active integration branch: `Main-Dev-V3`
- `feature/v3.1.1-rc2-prep` merged (PR #87); tag `v3.1.1-rc2` published
- Current public build channel: `v3.1.1-rc2` GitHub prerelease (`v3.1.1-rc1` marked superseded)
- Stable v3 release: pending

## Recently Completed

- v3 UI modernization and layout stabilization.
- SpeedTest reliability and readiness-flow fixes.
- TLS/public-IP fallback hardening for mixed endpoint environments.
- Refactoring programme priorities 1-6 closure and reconciliation artifacts.
- RC packaging + signing workflow for test distribution.
- RC2 prep: About screen, VirusTotal local-address handling, scan-progress flicker fix,
  HTTP/FTP endpoint reliability fixes, manual-updater hardening, and an enabled
  per-major-version-feed in-app updater with RC-confirmation and major-version safeguards.

## In Progress

- RC2 validation cycle with real endpoint datasets on the published prerelease.
- Documentation and repository cleanup for post-refactoring maintainability.

## Planned Next

- Bump `Main-Dev-V3/version.txt` to `3.1.1.2` once RC2 is validated, so existing 3.0+
  installs are offered the update through the now-enabled in-app updater.
- Promote v3.1.1 from RC to stable after validation sign-off.
- Continue targeted decomposition of large WinForms orchestration surfaces listed in residual findings.

## References

- Refactoring programme: `refactoring-programme.md`
- Residual findings: `refactoring-residual-findings.md`
- Reconciliation matrix: `docs/refactoring/reconciliation-2026-08-04.md`