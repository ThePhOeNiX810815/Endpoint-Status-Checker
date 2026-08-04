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

| Boundary | Status | Current evidence (files/symbols/tests) |
| --- | --- | --- |
| HTTP request creation | COMPLETE | `src/EndpointHttpRequestFactory.cs` wired via `PrepareHTTPWebRequest`; covered by `tests/HttpCompatibility.Tests/EndpointHttpRequestFactoryTests.cs` (request fields, credentials, cookies). |
| HTTP retry execution | COMPLETE | `src/EndpointHttpRetryExecutor.cs` wired via `GetHTTPWebResponse`; covered by timeout/non-timeout retry tests in `tests/HttpCompatibility.Tests/EndpointHttpRequestFactoryTests.cs`. |
| redirect resolution | COMPLETE | `src/EndpointHttpRedirectResolver.cs` in HTTP WebException redirect branch; covered by `tests/EndpointCheckingCore.Tests/EndpointScanOrchestrationSeamsTests.cs`. |
| HTTP response classification | COMPLETE | `src/EndpointHttpResponseClassifier.cs` used by `IsCloudflareProtected`; covered by Cloudflare classifier tests in `tests/HttpCompatibility.Tests/EndpointHttpRequestFactoryTests.cs`. |
| HTTP response metadata extraction | PARTIALLY COMPLETE | Success/error interpretation seam extracted in `src/EndpointHttpResponseInterpreter.cs` and used in `bw_GetStatus_DoWork`; deterministic tests added in `tests/HttpCompatibility.Tests/EndpointHttpRequestFactoryTests.cs`. Remaining stream/header-to-property collection and HTML metadata branches remain in form-owned orchestration. |
| HTTP terminal status/result mapping | COMPLETE | Status builders in `src/EndpointHttpStatusMapper.cs`, interpreted in `src/EndpointHttpResponseInterpreter.cs`, Cloudflare bypass override/append interpretation in `src/EndpointCloudflareBypassInterpreter.cs`, and terminal consolidation in `src/EndpointScanTerminalFinalizer.cs`; covered by deterministic `HttpCompatibility` and `EndpointCheckingCore` characterization tests. |
| FTP request creation | COMPLETE | `src/EndpointFtpRequestFactory.cs` wired in FTP path; covered by `tests/EndpointCheckingCore.Tests/EndpointFtpRequestFactoryTests.cs`. |
| FTP response/status mapping | COMPLETE | `src/EndpointFtpStatusMapper.cs` wired via `FTPWebResponseStatusMessage`; covered by `tests/EndpointCheckingCore.Tests/EndpointScanProtocolCompatibilityTests.cs`. |
| ping execution/result mapping | COMPLETE | `src/EndpointPingRetryExecutor.cs` wired via `GetPingTime`; result propagation to terminal flow covered by scan workflow/finalizer tests in `tests/EndpointCheckingCore.Tests`. |
| DNS/IP/MAC identity shaping | COMPLETE | `src/EndpointNetworkIdentityResolver.cs` wired for seed/filter/finalization; covered by `tests/EndpointCheckingCore.Tests/EndpointNetworkIdentityResolverTests.cs`. |
| share lookup/result shaping | PARTIALLY COMPLETE | Deterministic share-result shaping extracted into `src/EndpointNetworkShareResolver.cs` and covered by `tests/EndpointCheckingCore.Tests/EndpointNetworkShareResolverTests.cs`; runtime network share acquisition (`GetNetShares`) and orchestration remain in form. |
| SSL certificate extraction | PARTIALLY COMPLETE | Deterministic certificate-to-property mapping extracted into `src/EndpointSslCertificatePropertyMapper.cs` and characterized in `tests/EndpointCheckingCore.Tests/EndpointSslCertificatePropertyMapperTests.cs`; request/service-point certificate acquisition and flow control remain form-owned in `GetSSLCertificateInfo`. |
| Cloudflare classification and bypass result mapping | COMPLETE | Classification seam (`EndpointHttpResponseClassifier`), Cloudflare protection annotation (`EndpointHttpResponseInterpreter`), and bypass override/append/error interpretation (`EndpointCloudflareBypassInterpreter`) are deterministicly characterized in `tests/HttpCompatibility.Tests/EndpointHttpRequestFactoryTests.cs`; runtime bypass invocation remains orchestrated in form without behavior change. |
| terminal endpoint finalization | COMPLETE | `src/EndpointScanTerminalFinalizer.cs` wired at terminal stage; covered by `tests/EndpointCheckingCore.Tests/EndpointScanOrchestrationSeamsTests.cs`. |
| scan progress accounting | COMPLETE | `src/EndpointCheckProgress.cs` used in worker-loop progress path; covered by `tests/EndpointCheckingCore.Tests/EndpointCheckProgressTests.cs`. |
| cancellation handling | COMPLETE | Rule-level cancellation decisions in `src/EndpointScanWorkflowRules.cs` and terminal overrides in `src/EndpointScanTerminalFinalizer.cs`; covered by workflow/finalizer tests in `tests/EndpointCheckingCore.Tests`. |
| orchestration versus protocol implementation | PARTIALLY COMPLETE | `bw_GetStatus_DoWork` now orchestrates multiple seams (HTTP request/retry/redirect/interpretation, Cloudflare bypass interpretation, FTP request/status mapping, ping retry, identity shaping, share-result shaping, terminal finalization, SSL property mapping), but still contains substantial protocol-specific side-effect blocks (response stream persistence/meta extraction, share acquisition, SSL certificate acquisition, bypass invocation). |

Priority 3 is therefore still `in progress` under strict completion criteria.
