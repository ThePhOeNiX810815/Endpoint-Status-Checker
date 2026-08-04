# Endpoint Status Checker v3 Reconciliation (2026-08-04)

Base branch: `Main-Dev-V3`

This document reconciles what has actually merged versus the three priority tracks previously called out as high risk:

- Scan workflow characterization
- XLSX/HTML export compatibility baseline
- Thread-helper characterization and consolidation

## Merged evidence reviewed

- PR #57: HTML export transformer extraction (`EndpointHtmlExportTransformer` + tests)
- PR #58: UI thread-helper consolidation (`UiThreadHelpers` + tests)
- PR #59: report-mail table builder extraction (`ReportMailTableBuilder` + tests)

## Completion matrix rules

Status values:

- `Covered`: deterministic characterization exists in repository tests for the listed behavior.
- `Partial`: some related behavior is characterized, but not the full requirement.
- `Missing`: no deterministic characterization for the listed behavior.

A priority is complete only when all required rows are `Covered` (or explicitly owner-blocked).

## Priority 1: Scan workflow characterization

Overall status: `Completed against current deterministic criteria`

| Required behavior | Status | Current evidence |
| --- | --- | --- |
| HTTP routing | Covered | Deterministic protocol-route selection plus redirect-resolution seam coverage verify HTTP branch routing decisions without network dependency. |
| FTP routing | Covered | Deterministic protocol-route selection and FTP status-mapping seam coverage verify FTP branch routing/mapping decisions. |
| ping handling | Covered | Ping gating rules and deterministic ping timeout/success handling are characterized without external network dependency. |
| redirect behavior | Covered | Manual redirect gating, relative/absolute redirect resolution, and redirect annotation behavior are characterized by deterministic seams/tests. |
| timeout retry behavior | Covered | HTTP retry and ping timeout retry behavior are both characterized through deterministic executors. |
| non-timeout exception handling | Covered | HTTP non-timeout retry short-circuit and deterministic FTP transport-message mapping are covered. |
| response/status mapping | Covered | HTTP handled/transport/generic mappings, FTP handled/transport mappings, and terminal finalization mapping are covered by deterministic tests. |
| Cloudflare classification | Covered | `EndpointHttpResponseClassifier` tests cover CF-RAY and `Server` detection logic. |
| cancellation semantics | Covered | Rule-level cancellation checks plus terminal-finalizer cancellation overrides and progress completion are deterministically covered. |
| terminal endpoint completion | Covered | Pending/handled/unhandled/terminated endpoint terminal outcomes and progress completion semantics are covered by deterministic seams/tests. |
| progress completion | Covered | `EndpointCheckProgressTests` cover completion increments and total bounds. |
| active-worker drain | Covered | `EndpointCheckProgressTests` cover active worker drain to zero. |
| mixed fast/slow checks | Covered | `EndpointCheckProgressTests.MixedFastAndSlowEndpointCompletionKeepsActiveWorkerCountAccurate`. |
| handled terminal failures | Covered | HTTP handled failures and FTP handled failure mapping paths are characterized through deterministic mappers. |
| unhandled terminal failures | Covered | `EndpointCheckResultFactoryTests.UnhandledExceptionResultPreservesCurrentErrorMapping`. |

## Priority 2: XLSX/HTML export compatibility baseline

Overall status: `Completed against current deterministic criteria`

| Required behavior | Status | Current evidence |
| --- | --- | --- |
| HTML document structure | Covered | Summary-page placeholder replacement and auto-refresh insertion are covered by deterministic transformer tests. |
| HTML row/link transformations | Covered | `CreateEndpointUrlHyperLinks` behavior characterized by row click injection test. |
| auto-refresh behavior | Covered | `AddAutoRefreshMetaTag` characterization test exists. |
| refresh-button styling | Covered | `AddRefreshCssButton` characterization test exists. |
| escaping and encoding behavior | Covered | JSON/XML encoding workaround and legacy HTML hyperlink ampersand escaping are characterized in deterministic tests. |
| XLSX workbook existence and readable structure | Covered | Deterministic tests build, save, and reload the workbook to validate readable structure. |
| required worksheets | Covered | Deterministic workbook tests assert expected worksheet presence and legacy deletion behavior paths. |
| worksheet names | Covered | Worksheet names (`Summary`, `HTTP Endpoints`, `FTP Endpoints`) are asserted directly in deterministic tests. |
| required column order | Covered | Header-order assertions cover HTTP and FTP worksheet column order compatibility. |
| hidden-column behavior | Covered | Legacy hide-if-only-`N/A` behavior is asserted for representative hidden/visible columns. |
| representative cell values | Covered | Deterministic tests assert representative summary and endpoint worksheet cell values and hyperlinks. |
| important formatting where reliably testable | Covered | Deterministic tests assert key formatting (freeze panes, number format, and key fill colors). |
| file/path naming compatibility | Covered | Summary hyperlink placeholder replacement is characterized with legacy export file names across JSON/XML/XLSX/HTTP/FTP links. |

## Priority 3: Thread-helper characterization and consolidation

Overall status: `Consolidation done; characterization incomplete`

| Required behavior | Status | Current evidence |
| --- | --- | --- |
| Shared helper extraction with wrapper preservation | Covered | PR #58 introduced `UiThreadHelpers` and preserved per-form wrapper call signatures and invocation style. |
| deterministic helper behavior characterization | Partial | `UiThreadHelpersTests` cover before-callback invocation, background thread flag, and exception swallowing. |
| `Application.DoEvents` safety and sequencing characterization | Missing | No deterministic harness currently characterizes reentrancy/timing impact in real form call paths. |

## Reconciliation summary

- Priority 1 and Priority 2 are now complete under strict deterministic criteria.
- Priority order remains:
  1. Continue protocol decomposition only where enabled by completed scan/export characterization.
  2. Characterize `Application.DoEvents` safety/sequencing in real helper call paths.
  3. Continue remaining high-risk residual reductions after the above are covered.

## Priority 3 protocol decomposition acceptance matrix (current)

Scope evaluated: `src/CheckerMainForm.cs` / `bw_GetStatus_DoWork` and all deterministic compatibility harnesses in `tests/HttpCompatibility.Tests` and `tests/EndpointCheckingCore.Tests`.

Status values used in this matrix:

- `COMPLETE`
- `PARTIALLY COMPLETE`
- `NOT STARTED`
- `BLOCKED`
- `INTENTIONALLY FORM-OWNED`

| Boundary | Status | Current evidence (files/symbols/tests) |
| --- | --- | --- |
| HTTP request creation | COMPLETE | `src/EndpointHttpRequestFactory.cs` wired via `PrepareHTTPWebRequest`; covered by `tests/HttpCompatibility.Tests/EndpointHttpRequestFactoryTests.cs` (request fields, credentials, cookies). |
| HTTP retry execution | COMPLETE | `src/EndpointHttpRetryExecutor.cs` wired via `GetHTTPWebResponse`; covered by timeout/non-timeout retry tests in `tests/HttpCompatibility.Tests/EndpointHttpRequestFactoryTests.cs`. |
| redirect resolution | COMPLETE | `src/EndpointHttpRedirectResolver.cs` in HTTP WebException redirect branch; covered by `tests/EndpointCheckingCore.Tests/EndpointScanOrchestrationSeamsTests.cs`. |
| HTTP response classification | COMPLETE | `src/EndpointHttpResponseClassifier.cs` used by `IsCloudflareProtected`; covered by Cloudflare classifier tests in `tests/HttpCompatibility.Tests/EndpointHttpRequestFactoryTests.cs`. |
| HTTP response metadata extraction | COMPLETE | Success/error interpretation seam extracted in `src/EndpointHttpResponseInterpreter.cs`; response body stream/meta gating extracted into `src/EndpointHttpResponseBodyProcessor.cs`; HTML metadata parsing side effects extracted into `src/EndpointHttpHtmlMetadataResolver.cs`; request/response header collection extracted into `src/EndpointHttpHeaderCollector.cs`; deterministic coverage exists in `tests/HttpCompatibility.Tests/EndpointHttpRequestFactoryTests.cs`. |
| HTTP terminal status/result mapping | COMPLETE | Status builders in `src/EndpointHttpStatusMapper.cs`, interpreted in `src/EndpointHttpResponseInterpreter.cs`, Cloudflare bypass override/append interpretation in `src/EndpointCloudflareBypassInterpreter.cs`, and terminal consolidation in `src/EndpointScanTerminalFinalizer.cs`; covered by deterministic `HttpCompatibility` and `EndpointCheckingCore` characterization tests. |
| FTP request creation | COMPLETE | `src/EndpointFtpRequestFactory.cs` wired in FTP path; covered by `tests/EndpointCheckingCore.Tests/EndpointFtpRequestFactoryTests.cs`. |
| FTP response/status mapping | COMPLETE | `src/EndpointFtpStatusMapper.cs` wired via `FTPWebResponseStatusMessage`; covered by `tests/EndpointCheckingCore.Tests/EndpointScanProtocolCompatibilityTests.cs`. |
| ping execution/result mapping | COMPLETE | `src/EndpointPingRetryExecutor.cs` wired via `GetPingTime`; result propagation to terminal flow covered by scan workflow/finalizer tests in `tests/EndpointCheckingCore.Tests`. |
| DNS/IP/MAC identity shaping | COMPLETE | `src/EndpointNetworkIdentityResolver.cs` wired for seed/filter/finalization; covered by `tests/EndpointCheckingCore.Tests/EndpointNetworkIdentityResolverTests.cs`. |
| share lookup/result shaping | COMPLETE | Deterministic share-result shaping extracted into `src/EndpointNetworkShareResolver.cs`; acquisition decision/exception-assignment flow extracted into `src/EndpointNetworkShareAcquisition.cs`; host-share enumeration and Win32 mapping extracted into `src/EndpointNetworkShareEnumerator.cs`; covered by `tests/EndpointCheckingCore.Tests/EndpointNetworkShareResolverTests.cs`, `tests/EndpointCheckingCore.Tests/EndpointNetworkShareAcquisitionTests.cs`, and `tests/EndpointCheckingCore.Tests/EndpointNetworkShareEnumeratorTests.cs`. |
| SSL certificate extraction | COMPLETE | Deterministic certificate-to-property mapping extracted into `src/EndpointSslCertificatePropertyMapper.cs`; certificate acquisition and exception handling flow extracted into `src/EndpointSslCertificateAcquisition.cs`; both are characterized in `tests/EndpointCheckingCore.Tests` (`EndpointSslCertificatePropertyMapperTests`, `EndpointSslCertificateAcquisitionTests`) and wired through `GetSSLCertificateInfo`. |
| Cloudflare classification and bypass result mapping | COMPLETE | Classification seam (`EndpointHttpResponseClassifier`), Cloudflare protection annotation (`EndpointHttpResponseInterpreter`), bypass invocation/error-flow seam (`EndpointCloudflareBypassExecutor`), and bypass override/append/error interpretation (`EndpointCloudflareBypassInterpreter`) are deterministically characterized in `tests/HttpCompatibility.Tests/EndpointHttpRequestFactoryTests.cs`. |
| terminal endpoint finalization | COMPLETE | `src/EndpointScanTerminalFinalizer.cs` wired at terminal stage; covered by `tests/EndpointCheckingCore.Tests/EndpointScanOrchestrationSeamsTests.cs`. |
| scan progress accounting | COMPLETE | `src/EndpointCheckProgress.cs` used in worker-loop progress path; covered by `tests/EndpointCheckingCore.Tests/EndpointCheckProgressTests.cs`. |
| cancellation handling | COMPLETE | Rule-level cancellation decisions in `src/EndpointScanWorkflowRules.cs` and terminal overrides in `src/EndpointScanTerminalFinalizer.cs`; covered by workflow/finalizer tests in `tests/EndpointCheckingCore.Tests`. |
| orchestration versus protocol implementation | INTENTIONALLY FORM-OWNED | `bw_GetStatus_DoWork` remains a compatibility-sensitive orchestration boundary that coordinates deterministic seams. Remaining control-flow density is intentional for now to preserve observable scan workflow and WinForms/background-worker sequencing; decomposition target for Priority 4+ is async-safety and UI isolation, not forced de-orchestration. |

Priority 3 protocol decomposition acceptance is therefore `complete` under strict current criteria: every non-orchestration protocol behavior row is `COMPLETE`, and the retained orchestration boundary is explicitly marked `INTENTIONALLY FORM-OWNED`.

## Priority 4: EndpointDetailsDialog async-safety evidence matrix

Scope evaluated: `src/EndpointDetailsDialog.cs`, `src/EndpointVirusTotalScanRetryExecutor.cs`, and deterministic core harness updates.

Status values used in this matrix:

- `COMPLETE`
- `PARTIALLY COMPLETE`
- `NOT STARTED`
- `BLOCKED`
- `INTENTIONALLY SYNCHRONOUS`

| Workflow marker | Status | Current evidence (files/symbols/tests) |
| --- | --- | --- |
| `.Result` usage in `EndpointDetailsDialog` | COMPLETE | No direct `.Result` remains; VirusTotal scan/report task completion routes through `EndpointTaskSyncBridge.AwaitResult` in `GetVirusTotalScanReport` and `BW_VirusTotal_Report_DoWork`. |
| `.Wait()` usage in `EndpointDetailsDialog` | COMPLETE | No `.Wait()` call sites in `src/EndpointDetailsDialog.cs`. |
| `Thread.Sleep` usage in VirusTotal retry path | COMPLETE | Removed from `GetVirusTotalScanReport`; delay is now handled by iterative `EndpointVirusTotalScanRetryExecutor` boundary with cancellable delay delegate. |
| Recursive retry in VirusTotal scan enqueue path | COMPLETE | Removed: `GetVirusTotalScanReport` now executes one iterative retry loop via `EndpointVirusTotalScanRetryExecutor.Execute(...)`. |
| `Task.Run` usage in dialog workflows | COMPLETE | No `Task.Run` call sites in `src/EndpointDetailsDialog.cs`; async work remains on existing background abstractions. |
| `BackgroundWorker` workflows (`BW_PortCheck`, `BW_VirusTotal_Report`) | INTENTIONALLY SYNCHRONOUS | Existing WinForms `BackgroundWorker` orchestration is retained for compatibility; no deadlock-sensitive `.Result` remains directly on UI thread in those handlers. |
| `NewBackgroundThread` workflow usage | PARTIALLY COMPLETE | Still used across dialog for network jobs; VirusTotal scan enqueue path now centralized and safer, but broader usage remains. |
| `ThreadSafeInvoke` workflow usage | INTENTIONALLY SYNCHRONOUS | Still required for WinForms control mutation on UI thread; now constrained in VirusTotal scan retry path to status/tab updates only. |
| `Application.DoEvents` call sites | PARTIALLY COMPLETE | Still present via `NewBackgroundThread`/`ThreadSafeInvoke` wrappers and explicit per-link call in `ValidatePageLinks`; no blind removal performed in Priority 4. |
| Network I/O inside UI invocation block | PARTIALLY COMPLETE | `GetIPGeoInfo` now performs fetch/deserialization/map retrieval on background thread and uses `ThreadSafeInvoke` only for UI apply. Additional dialog workflows still require review to confirm no other UI-invoked I/O remains. |
| Form closing/disposal safety for retrying workflows | PARTIALLY COMPLETE | `EndpointDetailsDialog_FormClosing` now sets `virusTotalScanCancelled = true`; VirusTotal scan retry checks cancellation before attempts and during delay. No join/wait-on-worker strategy exists yet. |
| VirusTotal status-label update sequencing | PARTIALLY COMPLETE | Legacy retry-status message formatting remains preserved via `BuildLegacyRetryStatusMessage`; report polling now uses deterministic transition shaping (`EndpointVirusTotalReportWorkflow`) for present/waiting/no-scan states, while full end-to-end UI sequencing remains uncharacterized. |
| Cancellation flags and checks | PARTIALLY COMPLETE | VirusTotal scan enqueue and report polling now share a deterministic cancellation gate (`EndpointVirusTotalCancellationGate`) used before scheduling and during polling work; broader dialog workflows still do not share a unified cancellation contract. |
| Timeout handling in dialog network workflows | PARTIALLY COMPLETE | Existing timeout values remain preserved (`5000`/`10000`) across request paths; no consolidated timeout policy seam yet. |
| VirusTotal call workflows overall | PARTIALLY COMPLETE | Scan enqueue path is iterative/cancellable and report polling now applies explicit cancellation + deterministic transition shaping (`tests/EndpointCheckingCore.Tests/EndpointVirusTotalReportWorkflowTests.cs`), while polling remains synchronous bridge-based and dialog-level lifecycle orchestration is still not fully characterized. |
| IP geolocation workflow (`GetIPGeoInfo`) | PARTIALLY COMPLETE | Network work moved out of UI invocation, with deterministic result-shaping/tab-transition seam coverage in `tests/EndpointCheckingCore.Tests/EndpointGeoLocationPresentationBuilderTests.cs`. Remaining work is broader lifecycle/cancellation characterization in dialog-level integration. |
| WHOIS workflow (`GetWhoIsInfo`) | NOT STARTED | Still monolithic background network flow with UI updates; no deterministic shaping/cancellation seam yet. |

Priority 4 remains `in progress`; VirusTotal recursive retry/blocking-delay safety work is complete, while UI-thread network isolation and broader workflow cancellation/transition characterization are still pending.
