# Endpoint Status Checker — Change Log

---

## v3.0.0 — 2026-07-31 *(updated)*

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
