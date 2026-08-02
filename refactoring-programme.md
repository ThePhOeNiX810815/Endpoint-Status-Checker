# Endpoint Status Checker v3 Refactoring Programme

## Ticket 1: Endpoint loading and parsing

Status: In progress on `refactor/v3-endpoint-definition-parser`

Base branch: `v3-main`

Objective:

Extract endpoint-definition parsing and default `EndpointDefinition` creation from `CheckerMainForm.LoadEndpointReferences` into a narrow internal parser while preserving the current v3 loading behavior.

Scope:

- Parse raw endpoint-definition lines without WinForms controls, message boxes, settings, file I/O, global mutable state, network calls, or application state mutation.
- Preserve blank/comment/lone-pipe ignored line handling.
- Preserve first-pipe splitting for `Name|URL`.
- Preserve duplicate-name detection semantics.
- Preserve embedded credential extraction, credential removal from final address, and current endpoint-name modification.
- Preserve URL escaping and protocol validation categories.
- Preserve current default `EndpointDefinition` field initialization.
- Keep file reading, progress labels, maximum endpoint-count handling, message boxes, last-seen restoration, disabled-state restoration, list refresh, and startup scan orchestration in `CheckerMainForm.LoadEndpointReferences`.

Non-goals:

- No HTTP, FTP, SSL, redirect, retry, Cloudflare, cancellation, export, or UI behavior changes.
- No public API changes.
- No dependency additions.
- No work on Ticket 2 or later.

Compatibility notes:

- Maximum endpoint count remains based on source file line number, not valid endpoint count.
- Duplicate detection remains ordinal and case-sensitive after the legacy trim/name normalization behavior.
- A line can still contribute to both duplicate-name and invalid-URL user-visible error categories.
- Unsupported protocol is reported only after the legacy `://` delimiter check succeeds.
- `Uri.EscapeUriString` remains intentionally used to preserve byte-for-byte URL normalization behavior from the original loader.

Tests:

- `tests/EndpointDefinitionParser.Tests` is a dependency-free console characterization suite for the internal parser behavior.

Verification:

- `dotnet run --project tests/EndpointDefinitionParser.Tests/EndpointDefinitionParser.Tests.csproj`
- `dotnet restore src/EndpointChecker.csproj -p:EnableWindowsTargeting=true`
- `dotnet build src/EndpointChecker.csproj --no-restore -p:EnableWindowsTargeting=true`
