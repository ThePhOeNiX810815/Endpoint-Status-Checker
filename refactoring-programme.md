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

Status: Merged to `v3-main` via PR #45

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

## Ticket 3: HTTP compatibility boundaries

Status: Merged to `v3-main` via PR #46

Base branch: `v3-main`

Objective:

Reduce the HTTP-checking responsibility inside `CheckerMainForm` by extracting behavior-preserving internal helpers for request construction and Cloudflare response classification.

Scope completed in this ticket:

- Extracted legacy `HttpWebRequest` construction into `EndpointHttpRequestFactory`.
- Preserved request method, timeout, read/write timeout, redirect setting, keep-alive, cache policy, decompression, HTTP protocol version, maximum automatic redirects, user agent, browser-like headers, credentials, and legacy GDPR cookie injection.
- Preserved the current non-mutating `removeURLParameters` behavior.
- Extracted Cloudflare detection into `EndpointHttpResponseClassifier`.
- Added dependency-free characterization tests for the extracted HTTP compatibility boundaries.

Non-goals:

- No replacement of `HttpWebRequest` with `HttpClient`.
- No change to redirect-following behavior, retry behavior, SSL certificate handling, response-code mapping, cancellation, disposal, FTP, DNS, MAC, ping, export, persistence, or UI behavior.
- No public API changes.
- No dependency additions.

Compatibility notes:

- `WebRequest.Create(endpointURI.AbsoluteUri)` remains intentionally used because endpoint checking behavior is compatibility-sensitive.
- `AutomaticDecompression` and omitted invalid headers remain preserved from the v3 implementation.
- `removeURLParameters` intentionally remains non-mutating because the original code called `endpointURI.RemoveQuery()` without assigning the returned URL.
- Cloudflare classification still treats either `CF-RAY` or a `Server` value containing `cloudflare` as protected.

Tests:

- `tests/HttpCompatibility.Tests` covers request construction and Cloudflare classification.

Verification:

- `dotnet run --project tests/HttpCompatibility.Tests/HttpCompatibility.Tests.csproj`
- `dotnet run --project tests/EndpointCheckingCore.Tests/EndpointCheckingCore.Tests.csproj`
- `dotnet run --project tests/EndpointDefinitionParser.Tests/EndpointDefinitionParser.Tests.csproj`
- `dotnet build src/EndpointChecker.csproj --no-restore -p:EnableWindowsTargeting=true -v:q`

## Ticket 4: Export options and run-summary snapshots

Status: In progress on `refactor/v3-export-run-summary`

Base branch: `v3-main`

Objective:

Reduce direct WinForms control coupling inside endpoint-status export generation by extracting a small immutable snapshot of selected export formats.

Scope completed in this ticket:

- Extracted `EndpointExportOptions` from the four export checkbox values.
- Snapshotted export choices once at the `EndpointsStatusExport` boundary.
- Replaced repeated direct checkbox reads inside export generation with the snapshot.
- Added dependency-free characterization tests for the export-options snapshot.
- Extracted `EndpointExportRunSummary` for the check/run metadata written to the export summary worksheet.
- Kept the existing public `EndpointsStatusExport(...)` signature as a wrapper and moved the internal writer path to the summary snapshot.
- Added dependency-free characterization tests for the export run-summary snapshot.

Non-goals:

- No JSON, XML, HTML, or XLSX output format changes.
- No change to file names, file-lock behavior, worksheet shape, column order, hidden columns, colors, formatting, or export error handling.
- No configuration load/save behavior changes.
- No UI redesign.

Tests:

- `tests/ExportOptions.Tests` covers the export-options snapshot.
- `tests/ExportRunSummary.Tests` covers the export run-summary snapshot.

Verification:

- `dotnet run --project tests/ExportRunSummary.Tests/ExportRunSummary.Tests.csproj`
- `dotnet run --project tests/ExportOptions.Tests/ExportOptions.Tests.csproj`
- `dotnet run --project tests/HttpCompatibility.Tests/HttpCompatibility.Tests.csproj`
- `dotnet run --project tests/EndpointCheckingCore.Tests/EndpointCheckingCore.Tests.csproj`
- `dotnet run --project tests/EndpointDefinitionParser.Tests/EndpointDefinitionParser.Tests.csproj`
- `dotnet build src/EndpointChecker.csproj --no-restore -p:EnableWindowsTargeting=true -v:q`
