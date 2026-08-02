# Endpoint Status Checker v3 Refactoring Programme

## Ticket 1: Endpoint loading and parsing

Status: Merged to `v3-main` via PR #44

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

## Ticket 2: Endpoint checking core extraction

Status: In progress on `refactor/v3-endpoint-checking-core`

Base branch: `v3-main`

Objective:

Reduce the responsibility of `CheckerMainForm.bw_GetStatus_DoWork` by extracting behavior-preserving internal check-core boundaries that are deterministic and testable without changing network or UI behavior.

Scope completed in this ticket:

- Extracted an `EndpointCheckOptions` snapshot from WinForms control values.
- Preserved UI timeout unit conversion from seconds to milliseconds.
- Preserved thread-count adjustment based on enabled endpoint count.
- Extracted per-endpoint pending check-result initialization.
- Extracted top-level unhandled endpoint exception result mapping.
- Added dependency-free characterization tests for these extracted boundaries.

Non-goals:

- No replacement of `HttpWebRequest`.
- No change to HTTP request headers, cookies, authentication, redirects, retries, SSL, Cloudflare, timeout, cancellation, FTP, DNS, MAC, ping, export, or UI behavior.
- No move of `BackgroundWorker` orchestration.

Compatibility notes:

- The result factory intentionally preserves the current `N/A`, `ERROR`, and `Not Checked Yet` strings.
- The unhandled exception mapping intentionally preserves `ExceptionType -> message` response text.
- The extracted options object preserves existing timeout multiplication and thread-count reduction behavior.

Tests:

- `tests/EndpointCheckingCore.Tests` covers the new options snapshot and check-result factory boundaries.

Verification:

- `dotnet run --project tests/EndpointCheckingCore.Tests/EndpointCheckingCore.Tests.csproj`
- `dotnet run --project tests/EndpointDefinitionParser.Tests/EndpointDefinitionParser.Tests.csproj`
- `dotnet build src/EndpointChecker.csproj --no-restore -p:EnableWindowsTargeting=true`
