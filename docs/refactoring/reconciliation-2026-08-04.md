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

Overall status: `In progress (not complete)`

| Required behavior | Status | Current evidence |
| --- | --- | --- |
| HTTP routing | Partial | `EndpointScanWorkflowRulesTests.HttpProtocolRoutingRespectsProtocolValidationAndCancellation` and route resolver tests cover selection logic only. |
| FTP routing | Partial | `EndpointScanWorkflowRulesTests.FtpProtocolRoutingRespectsProtocolValidationAndCancellation` covers selection logic only. |
| ping handling | Partial | `ShouldRunPing`/`ShouldMarkPingCheckMessage` rules are covered; ping retry/outcome mapping path is not characterized. |
| redirect behavior | Partial | Manual redirect rule and redirect annotation are covered; end-to-end redirect handling chain is not fully characterized. |
| timeout retry behavior | Partial | HTTP timeout retry executor is covered; no equivalent deterministic characterization for ping timeout recursion path. |
| non-timeout exception handling | Partial | HTTP non-timeout retry short-circuit and HTTP exception message mapping covered; FTP exception handling mapping not covered. |
| response/status mapping | Partial | HTTP status/transport/generic mappings are covered; FTP response mapping and full terminal mapping matrix are not. |
| Cloudflare classification | Covered | `EndpointHttpResponseClassifier` tests cover CF-RAY and `Server` detection logic. |
| cancellation semantics | Partial | rule-level cancellation checks and progress cancellation completion exist; no deterministic full-scan cancellation sequencing harness. |
| terminal endpoint completion | Partial | `EndpointCheckResultFactory` and progress completion provide partial terminal behavior coverage. |
| progress completion | Covered | `EndpointCheckProgressTests` cover completion increments and total bounds. |
| active-worker drain | Covered | `EndpointCheckProgressTests` cover active worker drain to zero. |
| mixed fast/slow checks | Covered | `EndpointCheckProgressTests.MixedFastAndSlowEndpointCompletionKeepsActiveWorkerCountAccurate`. |
| handled terminal failures | Partial | HTTP handled failures covered; FTP handled failure mapping not characterized. |
| unhandled terminal failures | Covered | `EndpointCheckResultFactoryTests.UnhandledExceptionResultPreservesCurrentErrorMapping`. |

## Priority 2: XLSX/HTML export compatibility baseline

Overall status: `In progress (not complete)`

| Required behavior | Status | Current evidence |
| --- | --- | --- |
| HTML document structure | Partial | HTML transformation tests assert inserted fragments, not broader generated page structure. |
| HTML row/link transformations | Covered | `CreateEndpointUrlHyperLinks` behavior characterized by row click injection test. |
| auto-refresh behavior | Covered | `AddAutoRefreshMetaTag` characterization test exists. |
| refresh-button styling | Covered | `AddRefreshCssButton` characterization test exists. |
| escaping and encoding behavior | Partial | Structured JSON/XML encoding workaround covered; HTML escaping/encoding behavior not fully characterized. |
| XLSX workbook existence and readable structure | Missing | No deterministic workbook-load assertions currently in tests. |
| required worksheets | Missing | No tests validating Summary/HTTP/FTP worksheet presence rules. |
| worksheet names | Missing | No tests asserting worksheet naming compatibility. |
| required column order | Missing | No tests asserting column order for HTTP/FTP sheets. |
| hidden-column behavior | Missing | No tests asserting legacy hide-if-only-`N/A` behavior. |
| representative cell values | Missing | No tests asserting representative worksheet cell values. |
| important formatting where reliably testable | Missing | No tests asserting key formatting/freeze/filter/backgrounds. |
| file/path naming compatibility | Partial | `EndpointExportFileSet` tests cover naming/path composition, but not workbook/HTML linkage behavior. |

## Priority 3: Thread-helper characterization and consolidation

Overall status: `Consolidation done; characterization incomplete`

| Required behavior | Status | Current evidence |
| --- | --- | --- |
| Shared helper extraction with wrapper preservation | Covered | PR #58 introduced `UiThreadHelpers` and preserved per-form wrapper call signatures and invocation style. |
| deterministic helper behavior characterization | Partial | `UiThreadHelpersTests` cover before-callback invocation, background thread flag, and exception swallowing. |
| `Application.DoEvents` safety and sequencing characterization | Missing | No deterministic harness currently characterizes reentrancy/timing impact in real form call paths. |

## Reconciliation summary

- None of the three high-priority tracks are fully complete under strict criteria.
- Priority order remains:
  1. Finish scan workflow characterization harness.
  2. Finish XLSX/HTML export compatibility coverage.
  3. Continue protocol decomposition only where enabled by completed characterization.
