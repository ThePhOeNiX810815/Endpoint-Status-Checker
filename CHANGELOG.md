# Endpoint Status Checker — Change Log

---

## v3.1.1-rc2 — 2026-08-11 (ticket branch `feature/v3.1.1-rc2-prep`, pending approval)

### Supersedes RC1

RC2 is intended to be published **instead of** `v3.1.1-rc1`, not alongside it, once approved
after local testing.

### New

- **About screen** — new "About" menu button with developer, version/build date, homepage
  link, a GitHub v3 page link, and a release-channel badge ("RELEASE CANDIDATE 2") that
  disappears on its own once a build's version has a zero 4th component (stable release).
  Plays one of two background tracks automatically; a single "keep playing after closing
  this screen" checkbox controls whether it survives dialog close — never plays two tracks
  at once, and only stops exactly when the dialog closes with the box unchecked.
- **VirusTotal — skip local addresses** — loopback/private-range/`.local`/bare-hostname
  addresses now skip the VirusTotal tab automatically (VirusTotal can only reach public
  hosts). Endpoints the classifier can't determine can be flagged manually via a new
  right-click "Flag as Local Address (Skip VirusTotal)" option on the endpoint list.
- **VirusTotal — no more unhandled-exception risk** — a scan failure now always shows a
  clear "scan failed — click Refresh to retry" status instead of either silently vanishing
  or (in a worst case) crashing the app via an unobserved background-thread exception.

### Fixed

- **Scan progress bar color flicker** — an ambient UI "pulse" animation and the real scan
  status update were both overwriting `lbl_ProgressCount`/`pb_RefreshProcess` colors roughly
  10 times a second during a scan. The pulse now backs off while a scan is running.
- **`httpstat.us` "response ended prematurely" failures** — disabled `KeepAlive` on check
  requests; each check is one-shot, so there's no benefit to it, and some servers reset
  pooled connections under concurrent requests to the same host.
- **Stale `ftp.debian.org` sample entry** — Debian retired anonymous FTP service years ago
  (confirmed: connection times out). Replaced with `test.rebex.net`, a dedicated always-on
  public FTP test server (confirmed reachable).
- **Manual updater tool** — a failed download/extract left the UI frozen with no message and
  no way to close or retry; now caught, logged, and reported, with controls re-enabled for a
  retry. Also: handles a release zip that wraps everything in one top-level folder, and
  retries a locked-file copy briefly (the just-stopped exe can stay locked for a moment).
- **Startup delay** — enabling in-app update checks (see below) could block the very first
  window from appearing for up to ~30s on a slow/unreliable network, with no feedback at all.
  The update-check `WebClient` now uses a 3s timeout specifically for this call site.

### Changed — in-app updater

- The updater now selects its feed by the running build's major version: v2 reads
  `Main-Dev-Branch/version.txt`, v3 reads `Main-Dev-V3/version.txt`. Previously every build
  read the v2 feed regardless of its own version.
- `app_UpdateChecksEnabled` is now `true` (`Main-Dev-V3/version.txt` is a live, already-used
  feed — verified against real feed data before flipping this). Two guardrails apply
  regardless: a v2 install can never be offered a v3 package (major-version ceiling), and a
  release-candidate build is never silently auto-installed even with "auto-update in future"
  enabled — the user is always asked to confirm first. Same confirmation gating was added to
  the standalone `EndpointChecker-Updater` tool.

---

## v3.1.1-rc1 — 2026-08-04

### Release Candidate (Test Channel)

- Published GitHub prerelease asset `EndpointChecker-v3.1.1-rc1-test.zip` targeted to `Main-Dev-V3`.
- Fixed signed-build detection so `CN=David Smidke` signed executables are recognized correctly and no longer show `CUSTOM UNSIGNED BUILD` on splash.
- Updated splash behavior and speedtest/TLS/public-IP fixes carried forward from the v3.1.0 line.
- Cleaned package payload by removing duplicate `bin/Debug` dependency copies and excluding debug symbol artifacts from the distributed RC ZIP.

### Repository Cleanup

- Removed obsolete handoff notes file `CODEX_HANDOFF_2026-08-01.md`.
- Continued using GitHub Releases as the distribution channel for packaged binaries.

---

## v3.1.0 — 2026-08-01

### Premium UI Refresh

- Reworked the main Endpoint List screen into a compact, resize-resistant layout with
  top-row action, endpoint selection, status export, output folder, column chooser, and
  scan progress sections.
- Rebalanced the lower settings area into dedicated **List Options**, **Common Options**,
  and **HTTP Options** groups with two-column layouts where needed so labels, inputs,
  and checkboxes remain visible at normal desktop sizes.
- Softened the endpoint grid styling: muted grid lines, readable pre-scan text, darker
  headers, and preserved visual row separation without the previous high-contrast grid.
- Added a persistent endpoint-list column chooser. Hidden columns are saved in user
  settings and restored on startup.
- Replaced boxed command icons with integrated icon+text buttons for a cleaner toolbar
  feel while preserving the existing actions and selection commands.
- Moved scan progress and last status update into a dedicated compact top-row section
  with a gradient progress bar, keeping the footer clear and avoiding bottom clipping.
- Polished the SpeedTest dialog layout so gauge labels and throughput text fit cleanly
  inside their panels.

---

## v3.0.0 — 2026-08-01 *(updated)*

### Patch — 2026-08-01

- **Fix: build date displayed as 29.12.1926** — `RetrieveLinkerTimestamp` read the PE
  header linker timestamp field, which .NET 10 deterministic builds populate with a
  hash-derived value rather than the real link time. Replaced with
  `File.GetLastWriteTime(Process.GetCurrentProcess().MainModule.FileName)`, which returns
  the actual publish date.
- **Fix: auto-updater rewrite for .NET 10 multi-file self-contained build** — The previous
  approach (copy the exe to `%TEMP%`, re-launch it with `/AutoUpdate`) breaks because the
  single bootstrapper `.exe` cannot run without its 200+ sibling DLLs. The updater is now
  fully in-process: it downloads and extracts the package, then writes a `.cmd` script to
  `%TEMP%` and launches it after `Environment.Exit(0)` releases all file locks. The script
  uses `xcopy /s /y /e` to replace every file in the install directory, restores user data
  files, and then relaunches the application.
- **Fix: `UnzipUpdatePackage` no longer relies on zip internal structure** — Extraction
  target is now the fixed path `%TEMP%\EndpointChecker_Update\` rather than a path
  derived from `Entries.First().FullName`, making the updater independent of zip format.
- **Fix: `CleanTempPackageArchive` double-path bug** —
  `Path.Combine(app_TempDir, Path.Combine(app_TempDir, …))` corrected to
  `Path.Combine(app_TempDir, …)`.
- **New: standalone updater for v2.15 and earlier** —
  `EndpointChecker-Updater-v3.0.0-win-x86.exe` (~46 MB, self-contained, no runtime
  required) upgrades any previous version to v3.0.0. Auto-detects the install directory
  from running processes and common paths, preserves `EndpointChecker_EndpointsList.txt`
  and `EndpointChecker_LastSeenOnline.json`, and relaunches the application when done.
  Source at `tools/EndpointChecker-Updater/`.
- **New: legacy zip for v2.15 in-app auto-updater** —
  `EndpointChecker-v3.0.0-legacy-update.zip` is structured so the old v2.15 updater's
  extraction logic works: the first ZIP entry is an explicit directory entry
  (`EndpointChecker-v3.0.0/`) so `Entries.First().FullName` returns the folder name as
  expected. `package.txt` points to this zip, allowing v2.15 installations to self-update
  to v3.0.0 via the built-in update check.
- **Repo cleanup** — `src/bin/` and `src/obj/` added to `.gitignore`; negation rules keep
  `src/bin/Debug/VirusTotal.NET.dll` and `src/bin/Debug/tracert.dll` tracked (referenced
  assemblies required to compile, not build output).

### Patch — 2026-07-31

- **Fix: animated progress bar crash on scan start** — `ArgumentException: Parameter is not valid`
  was thrown by GDI+ inside `PremiumProgressBar.OnPaint` when the animation timer fired a
  repaint tick during a transient control layout phase (zero or near-zero control size).
  Fixed by:
  - Adding a top-level dimension guard (`Width ≤ 2 || Height ≤ 2 → return`) so paint is
    skipped entirely until the control is properly sized.
  - Wrapping each `LinearGradientBrush` construction in `try/catch (ArgumentException)` so
    isolated GDI+ failures are silently swallowed rather than propagating to the scan thread.

---

### Platform

- **Migrated to .NET 10** from .NET Framework 4.5. The project file was rewritten
  from the old `ToolsVersion="15.0"` MSBuild format to the SDK-style format
  (`Microsoft.NET.Sdk`).
- **Self-contained x86 build** — the .NET 10 runtime is bundled in the executable.
  No .NET installation is required on the target machine.
- `RuntimeIdentifier` set to `win-x86`; `PlatformTarget` kept at `x86` to preserve
  `SendARP` P/Invoke and WinForms COM interop compatibility.
- All `packages.config` + legacy `<Reference>` entries converted to `<PackageReference>`.

**Package updates:**

| Package | From | To | Notes |
| --- | --- | --- | --- |
| ClosedXML | 0.96.0 | 0.102.3 | API change — see Bug Fixes |
| Flurl | 3.0.0-pre4 | 3.0.6 | Stable release |
| FastMember | 1.3.0 | 1.5.0 | .NET Standard 2.1 |
| WindowsAPICodePack | 1.1.2 (FW-only) | Microsoft-WindowsAPICodePack 1.1.5 | Community .NET Standard fork |
| System.Management | — | 8.0.0 | Now required on .NET 8+ |
| System.DirectoryServices.AccountManagement | — | 8.0.0 | Now required on .NET 8+ |
| Microsoft.Playwright | — | 1.50.0 | New — Cloudflare bypass |

**Packages removed (now in-box on .NET 10):**
`System.IO.Compression`, `System.Net.Http`, `System.ValueTuple`,
`System.Runtime.InteropServices.RuntimeInformation`

---

### New Features

#### Cloudflare / Bot-Protection Bypass

Endpoints protected by Cloudflare are detected via the `CF-RAY` response header.
When detected, the status annotation includes the CF-RAY token. Three bypass modes
are available via **Settings → CF Bypass**:

| Mode | Description |
| --- | --- |
| **Disabled** | No bypass — status shown with `[Cloudflare Bot Protection]` annotation |
| **FlareSolverr** | Posts to an external FlareSolverr JSON API proxy (default: `http://localhost:8191`). Recommended for automated / production scans. |
| **Playwright** | Launches a local headless Chromium instance via CDP. Chromium binaries (~120 MB) are installed once on first use or via the in-app button. |

A `SemaphoreSlim(2, 2)` limits simultaneous Playwright Chromium instances to prevent
RAM exhaustion during parallel scans.

#### Configuration Dialog (issue #8)

Replaced the "open user.config in Notepad" menu shortcut with a proper five-tab
settings form accessible from **Settings → Configuration**:

- **Refresh & Notifications** — auto-refresh, continuous mode, interval, scan on startup, tray alerts
- **Scan** — validation method, timeouts, parallel threads, SSL/redirect toggles
- **Resolution** — DNS · IP · MAC · network shares · page meta · link resolution · save response
- **Export** — XLSX / JSON / XML / HTML toggles, output directory picker
- **Tools & API Keys** — VNC Viewer and PuTTY executable paths; VirusTotal and Google Maps API keys

Settings are read from `Settings.Default` on open and written back (with
`LoadConfiguration()` sync) on OK.

#### Endpoint Management Dialog (issue #9)

Replaced "open endpoint .txt in Notepad" with an in-app CRUD grid accessible from
**File → Endpoint List**:

- `DataGridView` with Name and URL columns — inline editing
- Add, Delete, Move Up, Move Down toolbar
- Comment lines and blank lines in the file are preserved on save
- The main form reloads the endpoint list automatically when the file is changed

#### Dark Theme

Applied a consistent navy palette (`#161319` body, `#1E2239` surface, `#263155` input)
across all controls, including both new dialogs. A `public static ApplyDarkTheme(Control root)`
helper was extracted from the main form so any new dialog can inherit the theme without
duplicating code.

#### Animated Progress Bar

Replaced the static GIF `PictureBox` with a custom `PremiumProgressBar` panel control:

- Renders real scan progress (filled portion tracks `current / total` endpoints)
- Gradient fill with a moving shimmer sweep at ~60 fps via a 16 ms `Timer`
- Implemented as a `public sealed class PremiumProgressBar : Panel` with
  `ControlStyles.UserPaint | OptimizedDoubleBuffer | AllPaintingInWmPaint | ResizeRedraw`

---

### Bug Fixes

#### Issue #36 — SendARP causes machine restart / BSOD

`NativeMethods.SendARP` is not thread-safe. Parallel scan threads calling it
simultaneously could trigger a NIC driver fault causing a BSOD on some machines.

**Fix:** A `SemaphoreSlim(1, 1)` in `ArpLookup/WindowsLookupService.cs` serialises
all `SendARP` P/Invoke calls.

#### Issue #35 — Special characters in endpoint URLs break parsing

Two root causes:

1. `line.Split('|')` without a count limit would re-split on a `|` inside the URL.
   **Fix:** `line.Split(new char[] { '|' }, 2)` — splits on the first pipe only.
2. URLs with spaces or RFC-3986 special characters would fail `Uri.IsWellFormedUriString`
   and be skipped.
   **Fix:** `Uri.EscapeUriString(address)` applied as a fallback before the validity check.

#### ClosedXML 0.102.x — `SetDataType()` removed

`IXLCells.SetDataType(XLDataType.Text)` no longer exists in ClosedXML 0.102.
**Fix:** Replaced with `.Style.NumberFormat.Format = "@"` (Excel text-format code —
identical functional result).

#### .NET 10 crash fixes

- **`NotifyIcon` field access** — reflection-based access to a private `Icon` field
  was broken by .NET 10 internal layout changes. Replaced with the public API.
- **Cross-thread UI reads** — several UI control reads on worker threads were
  marshalled via `ThreadSafeInvoke()`.
- **Inner exception chain** — `ExceptionDialog` and `FeatureRequestDialog` used
  `System.Web.UI.HtmlControls` types (ASP.NET WebForms, not available on .NET 10).
  Replaced with `StringBuilder`-based HTML generation using `WebUtility.HtmlEncode`.
- **WPF keyboard dependency** — `System.Windows.Input.Keyboard.IsKeyDown()` required
  `PresentationCore.dll`. Replaced with WinForms
  `(Control.ModifierKeys & Keys.Control) == Keys.Control`.
- **Endpoint list path** — path resolution changed to `AppContext.BaseDirectory` so
  the list file is always found next to the executable regardless of the working
  directory.

#### Chrome-accurate HTTP headers

The following `Sec-Fetch-*` headers were corrected:

| Header | Old value | New value |
| --- | --- | --- |
| `Sec-Fetch-Mode` | `Sec-Fetch-Node` (typo, invalid) | `navigate` |
| `Sec-Fetch-Site` | `same-origin` | `none` |
| `Sec-Fetch-Dest` | `empty` | `document` |
| `Sec-Fetch-User` | *(missing)* | `?1` |

`Accept` updated to the full Chrome-style string including `image/avif,image/webp`.

---

### Repository

- `.gitignore` updated — `CHANGELOG.md` no longer excluded from tracking
- `AssemblyVersion` and `AssemblyFileVersion` bumped to `3.0.0.0`
- `version.txt` updated to `3.0.0.0`

---

## v2.15.0 — 2025 (previous release)

See the [v2.15.0 release tag](https://github.com/ThePhOeNiX810815/Endpoint-Status-Checker/releases/tag/v2.15.0)
for the change history prior to the .NET 10 migration.
