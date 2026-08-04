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

Scope evaluated from live code: `src/EndpointDetailsDialog.cs` and extracted seam files currently wired into it (`EndpointTaskSyncBridge`, `EndpointVirusTotalScanRetryExecutor`, `EndpointVirusTotalReportWorkflow`, `EndpointVirusTotalCancellationGate`, `EndpointGeoLocationPresentationBuilder`, `EndpointWhoIsPresentationBuilder`, `UiThreadHelpers`).

Status values used in this matrix:

- `COMPLETE`
- `PARTIALLY COMPLETE`
- `NOT STARTED`
- `BLOCKED`
- `INTENTIONALLY SYNCHRONOUS`

| Workflow | File | Symbol | Status | Current behavior | Extracted seam | Tests | Remaining risk | Rationale |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Task blocking markers | `src/EndpointDetailsDialog.cs` | `GetVirusTotalScanReport`, `BW_VirusTotal_Report_DoWork` | COMPLETE | No direct `.Result`/`.Wait()` remains in the dialog; VirusTotal task completion is bridged via awaiter-based sync completion. | `EndpointTaskSyncBridge` | `tests/EndpointCheckingCore.Tests/EndpointTaskSyncBridgeTests.cs` | Bridge still blocks worker thread by design. | UI-thread deadlock-sensitive task blocking was removed from dialog call sites while preserving synchronous worker orchestration. |
| Recursive retry markers | `src/EndpointDetailsDialog.cs` | `GetWebsiteFavicon` | COMPLETE | Favicon fallback now uses deterministic non-recursive request planning and sequential attempts. | `EndpointFaviconLookupPlanBuilder` | `tests/EndpointCheckingCore.Tests/EndpointDetailsDialogWorkflowSeamsTests.cs` | No explicit retry-count policy beyond two planned URLs. | Recursive self-call was removed while preserving direct-first then fallback attempt behavior. |
| Cancellation-aware delay markers | `src/EndpointVirusTotalScanRetryExecutor.cs` | `DefaultDelay` | INTENTIONALLY SYNCHRONOUS | Delay still uses `Thread.Sleep` in 100ms chunks with cancellation checks before each chunk and after loop. | `EndpointVirusTotalScanRetryExecutor.Delay` delegate seam | `tests/EndpointCheckingCore.Tests/EndpointVirusTotalScanRetryExecutorTests.cs` | Sleep-based waiting remains if default delay is used. | Blocking delay is isolated to worker path, cancellation-aware, and test-injectable; explicit compatibility retention pending broader async redesign. |
| UI invoke network isolation | `src/EndpointDetailsDialog.cs` | `GetIPGeoInfo`, `GetWhoIsInfo`, `GetMACVendor`, `GetWebsiteFavicon`, `ValidatePageLinks` | COMPLETE | All reviewed workflows perform network/compute work before invoke, and invoke blocks now apply prepared results only. | `EndpointGeoLocationPresentationBuilder`, `EndpointWhoIsPresentationBuilder`, `EndpointMacVendorPresentationBuilder`, `EndpointFaviconLookupPlanBuilder`, `EndpointPageLinkValidationWorkflow` | `EndpointGeoLocationPresentationBuilderTests`, `EndpointWhoIsPresentationBuilderTests`, `EndpointDetailsDialogWorkflowSeamsTests`, `EndpointPageLinkValidationWorkflowTests` | Legacy exception swallowing remains by compatibility choice. | Priority 4 async-safety objective for UI invoke boundaries is met without introducing network calls inside invoke blocks. |
| VirusTotal scan submission | `src/EndpointDetailsDialog.cs` | `GetVirusTotalScanReport` | COMPLETE | Iterative retry, cancellation checks, and status update sequencing are deterministic at retry boundary level. | `EndpointVirusTotalScanRetryExecutor`, `EndpointVirusTotalCancellationGate` | `EndpointVirusTotalScanRetryExecutorTests`, `EndpointVirusTotalReportWorkflowTests` | Worker orchestration remains synchronous by design. | Retry recursion and blocking-delay hazards were removed from dialog logic while preserving status behavior. |
| VirusTotal report polling | `src/EndpointDetailsDialog.cs` | `TIMER_VirusTotalResult_Tick`, `BW_VirusTotal_Report_DoWork` | COMPLETE | Timer scheduling is cancellation-gated; polling transition mapping is deterministic for present/waiting/no-scan states. | `EndpointVirusTotalReportWorkflow`, `EndpointVirusTotalCancellationGate`, `EndpointTaskSyncBridge` | `EndpointVirusTotalReportWorkflowTests`, `EndpointTaskSyncBridgeTests` | Exceptions remain swallowed for compatibility. | Priority 4 polling transition/cancellation seams are in place and tested without external calls. |
| MAC vendor lookup | `src/EndpointDetailsDialog.cs` | `GetMACVendor`, `GetImageFromURL` | COMPLETE | Multi-call lookup now separates result shaping from UI apply and is disposal/cancellation-gated before final mutation. | `EndpointMacVendorPresentationBuilder`, `EndpointDetailsDialogCancellationGate` | `EndpointDetailsDialogWorkflowSeamsTests` | Broad catch behavior retained for compatibility. | High-risk nested network/UI workflow now has deterministic shaping seam and explicit cancellation checks. |
| Favicon/image retrieval | `src/EndpointDetailsDialog.cs` | `GetWebsiteFavicon`, `GetImageFromURL`, `GetImageFromGoogleMapsAPI`, `GetCountryFlagByCode` | COMPLETE | Favicon retrieval now uses non-recursive planned attempts with cancellation gate checks; UI image assignment is invoke-applied only. | `EndpointFaviconLookupPlanBuilder`, `EndpointDetailsDialogCancellationGate`, `EndpointGeoLocationPresentationBuilder` | `EndpointDetailsDialogWorkflowSeamsTests`, `EndpointGeoLocationPresentationBuilderTests` | Image acquisition exceptions remain swallowed by compatibility policy. | Recursive retry and background-thread direct UI mutation were eliminated from this workflow family. |
| Link-validation loop | `src/EndpointDetailsDialog.cs` | `ValidatePageLinks` | COMPLETE | Loop remains background-threaded, no per-iteration `Application.DoEvents`, explicit cancellation/closing guard, deterministic status/summary mapping. | `EndpointPageLinkValidationWorkflow`, `EndpointDetailsDialogCancellationGate` | `EndpointPageLinkValidationWorkflowTests` | Long-running network loop still depends on legacy request timeout semantics. | High-risk loop now has deterministic transition seam and bounded close-path behavior. |
| Ping live refresh timer | `src/EndpointDetailsDialog.cs` | `TIMER_PingRefresh_Tick`, `DoPing`, `SetPingResponse` | COMPLETE | Timer scheduling/apply path now checks disposal/closing gate before enqueue and before UI mutation; timer is stopped at close. | `EndpointDetailsDialogCancellationGate` | `EndpointDetailsDialogWorkflowSeamsTests` (gate) + existing core scan/ping tests | UI helper swallow behavior still underlies invoke failures. | Priority 4 timer-outliving-form risk is explicitly bounded for ping refresh. |
| BackgroundWorker port scan | `src/EndpointDetailsDialog.cs` | `BW_PortCheck_DoWork`, `BW_PortCheck_RunWorkerCompleted` | INTENTIONALLY SYNCHRONOUS | BackgroundWorker + socket operations are retained; UI list updates happen through invoke wrapper. | None | None | No explicit cancellation contract for long port scan iterations. | Compatibility-sensitive WinForms workflow intentionally retained; no current deadlock marker. |
| WMI workflow | `src/EndpointDetailsDialog.cs` | `btn_WMIInfo_Refresh_Click`, `GetComputerInformationByWMI` | INTENTIONALLY SYNCHRONOUS | WMI connect/query runs on background thread and applies node updates via invoke; exception text is applied on UI thread. | None | None | Heavy workflow still monolithic with swallowed exceptions. | Kept for compatibility; not currently violating UI-thread network-I/O criterion. |
| Wrapper-level `DoEvents` usage | `src/EndpointDetailsDialog.cs`, `src/UiThreadHelpers.cs` | `NewBackgroundThread`, `ThreadSafeInvoke`, `UiThreadHelpers.StartBackgroundThread`, `UiThreadHelpers.SafeInvoke` | INTENTIONALLY SYNCHRONOUS | Dialog wrappers continue to pass `Application.DoEvents` into helper callbacks for compatibility with existing WinForms repaint/event behavior. | `UiThreadHelpers` | `UiThreadHelpersTests` | Reentrancy risk remains and is tracked as the next priority. | This is now explicitly carried into Priority 5 (repository-wide `DoEvents` risk reduction), not a remaining Priority 4 async-boundary gap. |
| Form closing/disposal lifecycle | `src/EndpointDetailsDialog.cs` | `EndpointDetailsDialog_FormClosing` | COMPLETE | Closing now sets cancellation markers and explicitly stops ping/VirusTotal timers before disposing form-owned collaborators. | `EndpointDetailsDialogCancellationGate`, `EndpointVirusTotalCancellationGate` | `EndpointDetailsDialogWorkflowSeamsTests`, `EndpointVirusTotalReportWorkflowTests` | Background operations still rely on existing catch behavior. | Priority 4 close-path safety criteria for timer callbacks and cancellation markers are now explicitly implemented. |

Priority 4 is `complete` under the async-safety criteria used in this reconciliation matrix. Remaining `Application.DoEvents` risk is explicitly re-scoped into Priority 5.

## Priority 5: Application.DoEvents inventory and reduction plan

Scope evaluated from live code on `Main-Dev-V3`: repository-wide `Application.DoEvents` usage in forms and wrapper call paths.

Status values used in this matrix:

- `COMPLETE`
- `PARTIALLY COMPLETE`
- `NOT STARTED`
- `BLOCKED`
- `INTENTIONALLY SYNCHRONOUS`

| Workflow family | File | Symbol(s) | Status | Current behavior | Extracted seam | Tests | Remaining risk | Rationale |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Inventory completeness | `src/CheckerMainForm.cs`, `src/EndpointDetailsDialog.cs`, `src/FeatureRequestDialog.cs`, `src/ExceptionDialog.cs`, `src/SpeedTestDialog.cs`, `src/AutoUpdaterDialog.cs` | direct `Application.DoEvents` calls and `UiThreadHelpers` wrapper call sites | COMPLETE | Repository scan identifies direct call clusters plus wrapper-path injections using `UiThreadHelpers.StartBackgroundThread(..., Application.DoEvents)` and `UiThreadHelpers.SafeInvoke(..., Application.DoEvents)`. | `UiThreadHelpers` (existing wrapper seam) | `UiThreadHelpersTests` | Inventory alone does not reduce reentrancy. | Priority 5 starts with confirmed live-code inventory before behavioral reductions. |
| Checker main scan/export orchestration | `src/CheckerMainForm.cs` | direct `Application.DoEvents` calls in long-running scan/export loops (20 call sites) | PARTIALLY COMPLETE | Direct `DoEvents` remains inside compatibility-sensitive loops and progress/update branches. | protocol seams are complete; no dedicated `DoEvents` sequencing seam yet | existing scan/export deterministic suites (indirect) | Highest reentrancy/timing risk concentration remains here. | This is the first reduction target after inventory closure. |
| Dialog wrapper call paths | `src/EndpointDetailsDialog.cs`, `src/FeatureRequestDialog.cs`, `src/ExceptionDialog.cs`, `src/SpeedTestDialog.cs`, `src/AutoUpdaterDialog.cs` | `NewBackgroundThread`, `ThreadSafeInvoke`, wrapper delegates into `UiThreadHelpers` | INTENTIONALLY SYNCHRONOUS | Wrappers still pass `Application.DoEvents` callback for repaint/event processing compatibility during invoke/background transitions. | `UiThreadHelpers` | `UiThreadHelpersTests` | Reentrancy and ordering side effects are possible under nested message pumping. | Retained intentionally pending deterministic sequencing harness work. |
| Speed test workflow loops | `src/SpeedTestDialog.cs` | direct `Application.DoEvents` calls in test/progress phases (10 call sites) | NOT STARTED | Direct UI message pumping remains in long-running speed-test orchestration. | None | None specific to sequencing | External network timing plus `DoEvents` increases non-deterministic ordering risk. | Requires dedicated characterization before any safe reduction. |
| Feature/exception report flows | `src/FeatureRequestDialog.cs`, `src/ExceptionDialog.cs` | direct `Application.DoEvents` in attachment/report flows | NOT STARTED | Direct `DoEvents` remains in user-reporting flows alongside wrapper-based invoke patterns. | Shared mail table seam only (`ReportMailTableBuilder`) | `ReportMailTableBuilderTests` (content only) | Low-to-medium reentrancy risk with user-interaction timing sensitivity. | Reduction deferred until higher-risk main scan/export path is characterized. |

Priority 5 is `in progress`: inventory is complete, reduction/characterization work is not yet complete.
