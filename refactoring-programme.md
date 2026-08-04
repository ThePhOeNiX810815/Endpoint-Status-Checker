# Endpoint Status Checker v3 Refactoring Programme

## Ticket 1: Endpoint loading and parsing

Status: Merged to v3 development line via PR #44

Current v3 integration branch: `Main-Dev-V3`

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

Status: Merged to v3 development line via PR #45

Current v3 integration branch: `Main-Dev-V3`

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

Status: Merged to v3 development line via PR #46

Current v3 integration branch: `Main-Dev-V3`

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

## Ticket 4: Export snapshots, file paths, and structured export generation

Status: Merged to v3 development line via PR #47, PR #48, PR #49, and PR #50

Current v3 integration branch: `Main-Dev-V3`

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
- Extracted `EndpointExportFileSet` for legacy export file names and combined paths.
- Replaced repeated export path construction in export generation and export-folder validation with the file-set snapshot.
- Added dependency-free characterization tests for legacy export file names and path combination.
- Extracted JSON and XML export document creation into `EndpointStructuredExportGenerator`.
- Preserved indented JSON output and the existing `Encoding+` to `Encoding_` XML workaround.
- Added characterization tests for structured export output shape.

Non-goals:

- No JSON, XML, HTML, or XLSX output format changes.
- No change to file names, file-lock behavior, worksheet shape, column order, hidden columns, colors, formatting, or export error handling.
- No configuration load/save behavior changes.
- No UI redesign.

Tests:

- `tests/ExportOptions.Tests` covers the export-options snapshot.
- `tests/ExportRunSummary.Tests` covers the export run-summary snapshot.
- `tests/ExportFileSet.Tests` covers legacy export file names and path combination.
- `tests/StructuredExport.Tests` covers JSON and XML export generation.

Verification:

- `dotnet run --project tests/StructuredExport.Tests/StructuredExport.Tests.csproj`
- `dotnet run --project tests/ExportFileSet.Tests/ExportFileSet.Tests.csproj`
- `dotnet run --project tests/ExportRunSummary.Tests/ExportRunSummary.Tests.csproj`
- `dotnet run --project tests/ExportOptions.Tests/ExportOptions.Tests.csproj`
- `dotnet run --project tests/HttpCompatibility.Tests/HttpCompatibility.Tests.csproj`
- `dotnet run --project tests/EndpointCheckingCore.Tests/EndpointCheckingCore.Tests.csproj`
- `dotnet run --project tests/EndpointDefinitionParser.Tests/EndpointDefinitionParser.Tests.csproj`
- `dotnet build src/EndpointChecker.csproj --no-restore -p:EnableWindowsTargeting=true -v:q`

## Ticket 5: Cleanup, documentation, and residual review

Status: Ongoing (reconciled follow-up active)

Current v3 integration branch: `Main-Dev-V3`

Objective:

Perform a repository-wide clean-code review after Tickets 1-4, apply only low-risk cleanups that preserve observable behavior, and document residual issues that should not be changed without further characterization or separate migration work.

Scope completed so far:

- Removed obsolete `v3-main` branch usage from the active workflow after `Main-Dev-V3` became the v3 integration branch.
- Added XML documentation to important extracted internal boundaries from the refactoring programme.
- Added a residual-findings table for large remaining responsibilities, duplication, comments, direct UI coupling, and compatibility-sensitive areas.
- Merged PR #57: extracted HTML export post-processing transformations into `EndpointHtmlExportTransformer` with characterization tests.
- Merged PR #58: consolidated duplicated UI thread helper wrappers into `UiThreadHelpers` with dialog/form wrapper delegation preserved.
- Merged PR #59: extracted report-mail HTML table composition into `ReportMailTableBuilder`.

Non-goals:

- No behavior changes.
- No public API removals.
- No UI redesign.
- No branch, tag, or release changes outside retiring `v3-main`.
- No broad formatting pass.

Review artifacts:

- `refactoring-residual-findings.md`
- `docs/refactoring/reconciliation-2026-08-04.md`

## Ticket 6: Warning baseline and future warning governance

Status: Baseline documented; regression gate not yet implemented

Current v3 integration branch: `Main-Dev-V3`

Objective:

Document current warning inventory and create a non-destructive path toward warning-regression governance.

Scope completed in this ticket:

- Captured warning baseline and warning inventory in `docs/refactoring/warning-baseline.md`.
- Preserved current behavior: no global warning suppressions, no warning-level reduction, no blanket warnings-as-errors.

Remaining work in this ticket:

- Implement CI warning-regression gate that compares unique warning counts against the documented baseline.
- Ensure the gate detects regressions without failing on existing baseline warnings.

## Reconciliation Priority Ledger (as of 2026-08-04)

This ledger is intentionally strict: a priority is not considered complete unless all listed completion criteria are covered by deterministic tests or blocked by an explicit owner decision.

1. Scan workflow characterization harness: Complete (deterministic criteria covered in `docs/refactoring/reconciliation-2026-08-04.md`)
2. XLSX and HTML export compatibility baseline: Complete (deterministic criteria covered in `docs/refactoring/reconciliation-2026-08-04.md`)
3. Protocol decomposition enabled by harness: Complete (strict closure matrix in `docs/refactoring/reconciliation-2026-08-04.md`)
4. `EndpointDetailsDialog` blocking `.Result` paths: In progress
5. Unsafe `Application.DoEvents` review/reduction: Not started
6. CI warning-regression governance: Not started (baseline exists)
7. Remaining low-risk residual cleanup: Partially performed (mail table builder extraction completed)

## Ticket 7: Scan-workflow characterization completion (phase 1)

Status: Completed (foundational seams and tests)

Current v3 integration branch: `Main-Dev-V3`

Objective:

Close high-priority characterization gaps in scan workflow behavior before deeper protocol decomposition.

Scope completed in this phase:

- Extracted deterministic ping timeout-retry seam into `EndpointPingRetryExecutor`.
- Extracted deterministic FTP response/exception status mapping seam into `EndpointFtpStatusMapper`.
- Wired `CheckerMainForm.GetPingTime` and `CheckerMainForm.FTPWebResponseStatusMessage` through these seams while preserving existing output semantics.
- Added characterization suites covering ping retry terminal outcomes and FTP status/exception mapping behavior.
- Corrected `tests/EndpointCheckingCore.Tests` execution model by converting prior xUnit-style files to the repository's executable custom runner pattern.

Remaining work in this ticket:

- Complete remaining partial scan-matrix items in `docs/refactoring/reconciliation-2026-08-04.md`, including deeper end-to-end terminal mapping and cancellation-sequencing coverage.

## Ticket 8: Scan-workflow characterization completion (phase 2)

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Close the remaining scan harness criteria by extracting deterministic seams for redirect resolution and terminal result finalization.

Scope completed in this phase:

- Extracted deterministic redirect resolution into `EndpointHttpRedirectResolver` and wired the manual redirect path through it.
- Extracted deterministic terminal result finalization into `EndpointScanTerminalFinalizer` and wired address/response-time/cancellation/last-seen updates through it.
- Added characterization coverage for redirect resolution and terminal finalization behavior without public internet dependency.
- Reconciled completion matrix to mark scan-workflow characterization criteria covered.

## Ticket 9: XLSX/HTML export compatibility completion

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Close Priority 2 by completing deterministic XLSX/HTML compatibility characterization and extracting a stable XLSX workbook-construction seam.

Scope completed in this ticket:

- Extracted deterministic workbook-construction seam into `EndpointXlsxExportWorkbookBuilder` and routed `EndpointsStatusExport` XLSX workbook assembly through it while preserving output semantics.
- Added deterministic `StructuredExport` compatibility coverage for workbook readability, worksheet presence/names, HTTP/FTP column order, representative values, hidden-column behavior, key formatting assertions, and legacy worksheet deletion rules.
- Added deterministic HTML summary-link replacement seam in `EndpointHtmlExportTransformer.ReplaceSummaryHyperLinkPlaceholders` and routed summary placeholder replacement through it.
- Expanded HTML compatibility coverage to include summary-page structure/link replacement and legacy hyperlink ampersand-escaping behavior.
- Reconciled completion matrix to mark export compatibility criteria covered.

## Ticket 10: Protocol decomposition continuation (FTP request seam)

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Continue Priority 3 decomposition by extracting FTP request and credential-initialization setup from `bw_GetStatus_DoWork` into a deterministic seam while preserving runtime behavior.

Scope completed in this ticket:

- Extracted FTP request setup and credential fallback logic into `EndpointFtpRequestFactory`.
- Routed the FTP branch of `CheckerMainForm.bw_GetStatus_DoWork` through the extracted factory and preserved endpoint login-name/login-password mutation behavior.
- Added deterministic `EndpointCheckingCore` tests for FTP request setup fields and credential fallback semantics.
- Preserved legacy `FtpWebRequest` flow, method, timeout usage, and credential compatibility behavior.

## Ticket 11: Protocol decomposition continuation (network identity seam)

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Continue Priority 3 decomposition by extracting deterministic host/IP/DNS/MAC identity shaping rules from `bw_GetStatus_DoWork` while preserving runtime network-lookup behavior.

Scope completed in this ticket:

- Extracted seed host classification, MAC inclusion filtering, and resolved-identity finalization into `EndpointNetworkIdentityResolver`.
- Routed `CheckerMainForm.bw_GetStatus_DoWork` host-seeding and final identity assignment through the extracted seam.
- Preserved existing lookup orchestration (`Dns.GetHostAddresses`, `Dns.GetHostEntry`, `WindowsLookupService.Lookup`) and only moved deterministic shaping rules.
- Added deterministic `EndpointCheckingCore` coverage for IPv4-vs-host seed behavior, MAC gateway filtering rules, and resolved/fallback identity assignment behavior.

## Ticket 12: Protocol decomposition continuation (HTTP response interpretation seam)

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Continue Priority 3 decomposition by extracting deterministic HTTP response interpretation (success metadata and handled-error mapping) from `bw_GetStatus_DoWork` while preserving legacy compatibility outputs.

Scope completed in this ticket:

- Extracted deterministic HTTP success/handled-error interpretation into `EndpointHttpResponseInterpreter`.
- Routed HTTP success-path metadata assignment (status/message/redirect annotation/server ID/content metadata/content length normalization) through the extracted seam.
- Routed handled HTTP error message construction (including Cloudflare protection annotation text) through the extracted seam while preserving existing bypass invocation flow.
- Delegated content-length display formatting to the new seam and preserved legacy formatting semantics.
- Added deterministic `HttpCompatibility` tests covering success interpretation, handled-error mapping, Cloudflare-note construction, and content-length formatting behavior.

## Ticket 13: Protocol decomposition continuation (Cloudflare bypass result mapping seam)

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Continue Priority 3 decomposition by extracting deterministic Cloudflare bypass result/error interpretation from `bw_GetStatus_DoWork` while preserving existing bypass invocation and response semantics.

Scope completed in this ticket:

- Extracted deterministic bypass outcome interpretation into `EndpointCloudflareBypassInterpreter`.
- Routed bypass success override behavior (`ResponseCode` and `ResponseMessage`) through the extracted seam.
- Routed bypass failure and bypass-exception message suffix behavior through the extracted seam.
- Preserved existing bypass invocation, method selection, and FlareSolverr URL flow in `CheckerMainForm` orchestration.
- Added deterministic `HttpCompatibility` tests covering success override, failure append, and bypass-error append mapping behavior.

## Ticket 14: Protocol decomposition continuation (SSL certificate property mapping seam)

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Continue Priority 3 decomposition by extracting deterministic SSL certificate property mapping from `GetSSLCertificateInfo` while preserving existing request/service-point acquisition behavior.

Scope completed in this ticket:

- Extracted deterministic certificate-to-property mapping into `EndpointSslCertificatePropertyMapper`.
- Routed `GetSSLCertificateInfo` property population through the extracted seam while preserving legacy null-check and swallow-on-error behavior.
- Preserved existing `HttpWebRequest.ServicePoint.Certificate` acquisition and `X509Certificate2` conversion flow in form orchestration.
- Added deterministic `EndpointCheckingCore` coverage for null-certificate handling and representative mapped property/value behavior.

## Ticket 15: Protocol decomposition continuation (network share result shaping seam)

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Continue Priority 3 decomposition by extracting deterministic network-share result shaping from `bw_GetStatus_DoWork` while preserving runtime share acquisition behavior.

Scope completed in this ticket:

- Extracted deterministic network-share list shaping/sorting into `EndpointNetworkShareResolver`.
- Routed network share assignment in `bw_GetStatus_DoWork` through the extracted seam while preserving null/empty behavior.
- Preserved existing runtime share acquisition via `GetNetShares(responseURI.Host)` and existing catch-swallow orchestration behavior.
- Added deterministic `EndpointCheckingCore` tests for null input, empty input, and legacy sorting/array-shaping behavior.

## Ticket 16: Protocol decomposition continuation (HTTP response body stream/meta boundary)

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Continue Priority 3 decomposition by extracting deterministic HTTP response-body stream/meta decision logic from `bw_GetStatus_DoWork` while preserving current response-save and metadata behavior.

Scope completed in this ticket:

- Extracted deterministic response-body trigger, bounded-byte read, HTML metadata gating, and encoding-fallback decision logic into `EndpointHttpResponseBodyProcessor`.
- Routed response-body read loop and HTML meta fallback branching in `bw_GetStatus_DoWork` through the extracted seam.
- Preserved existing bounded-read behavior, content-length update path, response-save behavior, and `ResolvePageMetaInfo` invocation order.
- Added deterministic `HttpCompatibility` tests for read-trigger conditions, max-byte-cap behavior, HTML-gating behavior, and encoding fallback/default assignment gating.

## Ticket 17: Protocol decomposition continuation (HTML metadata parsing seam)

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Continue Priority 3 decomposition by extracting deterministic HTML metadata parsing and side-effect shaping from `ResolvePageMetaInfo` while preserving runtime endpoint metadata outcomes.

Scope completed in this ticket:

- Extracted deterministic HTML metadata parsing and shaping into `EndpointHttpHtmlMetadataResolver`.
- Routed `ResolvePageMetaInfo` through the extracted resolver seam while preserving assignment semantics for title/description/author/language/theme/default encoding/meta list/link list.
- Preserved existing encoding parser behavior by delegating charset parsing through `CheckerMainForm.GetEncoding`.
- Added deterministic `HttpCompatibility` coverage for title/meta extraction, link deduplication/exclusion behavior, language resolution (`mul`), theme-color parsing fallback, and HTML-encoding preservation behavior.

## Ticket 18: Protocol decomposition continuation (HTTP header collection seam)

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Continue Priority 3 decomposition by extracting deterministic request/response header collection from `CheckerMainForm.GetHTTPWebHeaders` while preserving runtime header mapping behavior.

Scope completed in this ticket:

- Extracted deterministic header-collection mapping into `EndpointHttpHeaderCollector`.
- Routed `CheckerMainForm.GetHTTPWebHeaders` through the extracted seam while preserving caller-side list mutation behavior.
- Preserved existing header name/value mapping semantics and null/empty collection handling.
- Added deterministic `HttpCompatibility` coverage for null/empty handling and representative request header name/value mapping.

## Ticket 19: Protocol decomposition continuation (network share acquisition seam)

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Continue Priority 3 decomposition by extracting deterministic network-share acquisition decision and assignment flow from `bw_GetStatus_DoWork` while preserving host-enumeration behavior.

Scope completed in this ticket:

- Extracted network-share acquisition gating/exception-assignment semantics into `EndpointNetworkShareAcquisition`.
- Routed the `ResolveNetworkShares` branch in `bw_GetStatus_DoWork` through the extracted seam while preserving assignment-on-success and no-assignment-on-exception behavior.
- Preserved existing host share enumeration implementation by delegating acquisition through existing `GetNetShares` callback.
- Added deterministic `EndpointCheckingCore` coverage for disabled-gating behavior, success-path sorted assignment behavior, and exception-path no-assignment behavior.

## Ticket 20: Protocol decomposition continuation (SSL certificate acquisition seam)

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Continue Priority 3 decomposition by extracting deterministic SSL certificate acquisition/error-flow handling from `GetSSLCertificateInfo` while preserving mapped-property outcomes.

Scope completed in this ticket:

- Extracted SSL certificate acquisition and swallow-on-error behavior into `EndpointSslCertificateAcquisition`.
- Routed `GetSSLCertificateInfo` through the extracted seam while preserving add-only assignment of mapped certificate properties.
- Preserved existing certificate property mapping via `EndpointSslCertificatePropertyMapper`.
- Added deterministic `EndpointCheckingCore` coverage for null getter, null certificate, exception fallback, and representative mapped-property outcomes.

## Ticket 21: Protocol decomposition continuation (Cloudflare bypass invocation seam)

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Continue Priority 3 decomposition by extracting Cloudflare bypass invocation/exception flow from the HTTP handled-error path while preserving bypass outcome mapping semantics.

Scope completed in this ticket:

- Extracted Cloudflare bypass invocation and exception-flow handling into `EndpointCloudflareBypassExecutor`.
- Routed the handled HTTP Cloudflare branch through the new seam while preserving no-attempt behavior, success override behavior, and exception append behavior.
- Preserved existing bypass outcome message semantics by delegating to `EndpointCloudflareBypassInterpreter`.
- Added deterministic `HttpCompatibility` coverage for no-attempt gating, bypass success mapping, and bypass exception behavior.

## Ticket 22: Protocol decomposition closure (network share host enumeration seam)

Status: Completed

Current v3 integration branch: `Main-Dev-V3`

Objective:

Close the remaining Priority 3 share-lookup gap by extracting Win32 host-share enumeration/mapping from `CheckerMainForm.GetNetShares` into a deterministic seam while preserving existing output formatting and error message compatibility.

Scope completed in this ticket:

- Extracted `NetShareEnum`/`NetApiBufferFree` interop and `SHARE_INFO_1` iteration into `EndpointNetworkShareEnumerator`.
- Preserved current share item formatting (`[Type] Name (Remark)`), known type-code mapping, and fallback unknown type/error-code formatting.
- Routed `CheckerMainForm.GetNetShares` through `EndpointNetworkShareEnumerator.Enumerate(...)`.
- Added deterministic characterization coverage in `tests/EndpointCheckingCore.Tests/EndpointNetworkShareEnumeratorTests.cs`.

Verification:

- `dotnet run --project tests/EndpointCheckingCore.Tests/EndpointCheckingCore.Tests.csproj`
- `dotnet run --project tests/HttpCompatibility.Tests/HttpCompatibility.Tests.csproj`
- `dotnet build src/EndpointChecker.sln`

## Ticket 23: EndpointDetailsDialog async-safety kickoff (VirusTotal task blocking bridge)

Status: In progress

Current v3 integration branch: `Main-Dev-V3`

Objective:

Begin Priority 4 by removing direct task `.Result` call sites from `EndpointDetailsDialog` VirusTotal workflows while preserving current user-visible status and error behavior.

Scope completed in this phase:

- Added `EndpointTaskSyncBridge` to centralize synchronous task completion via `ConfigureAwait(false).GetAwaiter().GetResult()`.
- Replaced direct `.Result` access in `GetVirusTotalScanReport` and `BW_VirusTotal_Report_DoWork` with `EndpointTaskSyncBridge.AwaitResult(...)`.
- Added deterministic characterization tests for bridge success path, exception unwrapping semantics, and null-argument guard.
- Replaced recursive retry + `Thread.Sleep` in `GetVirusTotalScanReport` with iterative `EndpointVirusTotalScanRetryExecutor` orchestration.
- Added cancellation-aware retry boundary checks for form-closing/disposal conditions in dialog VirusTotal scan enqueue flow.
- Added deterministic non-network tests for immediate success, retry/success, exhaustion, cancellation-before-attempt, cancellation-during-delay, legacy retry-message mapping, exact-attempt semantics, and status transition ordering.

Remaining work in this ticket:

- Isolate network I/O from UI thread invocation paths in `EndpointDetailsDialog` (notably `GetIPGeoInfo`).

Verification:

- `dotnet run --project tests/EndpointCheckingCore.Tests/EndpointCheckingCore.Tests.csproj`
- `dotnet run --project tests/HttpCompatibility.Tests/HttpCompatibility.Tests.csproj`
- `dotnet build src/EndpointChecker.sln`
