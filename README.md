# Endpoint Status Checker

```text
                                  .::!!!!!!!:.
  .!!!!!:.                        .:!!!!!!!!!!!!
  ~~~~!!!!!!.                 .:!!!!!!!!!UWWW$$$
      :$$NWX!!:           .:!!!!!!XUWW$$$$$$$$$P
      $$$$$##WX!:      .<!!!!UW$$$$"  $$$$$$$$#
      $$$$$  $$$UX   :!!UW$$$$$$$$$   4$$$$$*
      ^$$$B  $$$$\     $$$$$$$$$$$$   d$$R"
        "*$bd$$$$      '*$$$$$$$$$$$o+#"
             """"          """""""

 _____           _             _       _
| ____|_ __   __| |_ __   ___ (_)_ __ | |_
|  _| | '_ \ / _` | '_ \ / _ \| | '_ \| __|
| |___| | | | (_| | |_) | (_) | | | | | |_
|_____|_| |_|\__,_| .__/ \___/|_|_| |_|\__|
                  |_|

 ____  _        _                ____ _               _
/ ___|| |_ __ _| |_ _   _ ___   / ___| |__   ___  ___| | _____ _ __
\___ \| __/ _` | __| | | / __| | |   | '_ \ / _ \/ __| |/ / _ \ '__|
 ___) | || (_| | |_| |_| \__ \ | |___| | | |  __/ (__|   <  __/ |
|____/ \__\__,_|\__|\__,_|___/  \____|_| |_|\___|\___|_|\_\___|_|
```
<img width="643" height="360" alt="image" src="https://github.com/user-attachments/assets/74319e84-0e38-4bcc-bc9d-759e3ac6f8c4" />

Lazy Development

A Windows desktop tool that monitors a user-defined list of network endpoints and reports
their availability, response times, SSL state, DNS/IP/MAC resolution, and more — all in
a single scan with configurable automation.

---

## Features

| Category | Details |
| --- | --- |
| **Protocol support** | HTTP · HTTPS · FTP · FTPS · TCP port · ICMP ping |
| **Cloudflare bypass** | FlareSolverr (external proxy) or Playwright CDP (headless Chromium) |
| **Resolution** | DNS name · IP address · MAC address · Network shares · SSL certificate info |
| **Page analysis** | HTML title · meta tags · Open Graph · page links · content type & length · ETag |
| **External APIs** | SpeedTest · GeoIP + Google Maps · TraceRoute · VirusTotal · WHOIS |
| **Automation** | Scheduled or continuous scan · configurable interval · scan on startup |
| **Export** | XLSX · JSON · XML · HTML (per-scan report files) |
| **Notifications** | System tray icon with status summary and balloon error alerts |
| **UI** | Premium dark navy theme · animated progress bar · column reordering & width persistence |
| **Tools** | Launch VNC Viewer or PuTTY directly from a selected endpoint row |

---

## Current v3 Development Status

Endpoint Status Checker v3 is currently developed on the `Main-Dev-V3` branch.

## Branch Policy

- v3 branch: `Main-Dev-V3`
- v3 rule: no merging with v2 is allowed, and no v3 changes may affect v2 behavior or delivery.
- v2 branch and default mainline: `Main-Dev-V2`
- v2 rule: changes intended for v2 must stay isolated from the v3 development line unless explicitly ported separately.

Current public distribution status:

- Test channel: GitHub prerelease `v3.1.1-rc2` (supersedes and replaces `v3.1.1-rc1`)
- Official stable v3 channel: not published yet

## v3.1.1 RC2 Test Build

Fixes several issues found while validating RC1: About screen, VirusTotal local-address
handling, a scan-progress color flicker, a couple of endpoint-list/HTTP flakiness fixes, and
manual-updater robustness. See [CHANGELOG.md](CHANGELOG.md) for the full list.

- Release channel: **GitHub prerelease only**
- Tag: `v3.1.1-rc2`
- Asset: `EndpointChecker-v3.1.1-rc2-test.zip`
- Download: [v3.1.1 RC2 test build](https://github.com/ThePhOeNiX810815/Endpoint-Status-Checker/releases/tag/v3.1.1-rc2)
- `v3.1.1-rc1` is marked superseded — use RC2 for testing going forward.

Update-checker behavior (changed in RC2):

- The in-app updater now checks a **per-major-version feed**: v2 builds check
  `Main-Dev-Branch/version.txt`, v3 builds check `Main-Dev-V3/version.txt` — each line only
  ever compares against its own channel.
- `app_UpdateChecksEnabled` is now **on** for both lines (`Main-Dev-V3/version.txt` is a live,
  already-in-use feed). This was verified against the real feed data before enabling it.
- Two safeguards remain regardless of that setting:
  - A v2 install is never offered a v3 package (major-version ceiling), so upgrading
    from v2.15 to v3 is still manual-updater-only.
  - A release-candidate build (non-zero 4th version number) is **never** silently
    auto-installed — the user is always asked to confirm, even with "auto-update in future" on.
- Startup no longer blocks on this check for more than a few seconds even on a slow/blocked
  network — the update-check `WebClient` timeout was shortened specifically for this call.

Test builds are signed with a trusted self-signed certificate (`CN=David Smidke`) from the
local certificate store before packaging.

Repository note:

- Large packaged binaries are not stored in this branch.
- Release files are published as GitHub Release assets only.

## What's New in v3.1.1 RC2

| Area | Change |
| --- | --- |
| **About screen** | New "About" menu button — developer, homepage, GitHub v3 page, version/build date, and an RC/stable release-channel badge that self-clears on a stable build. Also plays one of two background tracks, with an opt-in "keep playing after close" checkbox that never overlaps two tracks. |
| **VirusTotal** | Scans are now skipped automatically for local/private/loopback addresses (VirusTotal can't reach them anyway); endpoints the classifier can't determine can be flagged manually via right-click. Scan failures now show a clear "failed, click Refresh to retry" state instead of ever risking an unhandled exception. |
| **Scan progress bar** | Fixed a color flicker between the idle pulse animation and the active scan-status color — they were overwriting each other every ~95ms during a scan. |
| **HTTP checks** | Disabled `KeepAlive` on check requests — fixes "response ended prematurely" failures against servers (e.g. httpstat.us) that reset pooled connections under concurrent requests. |
| **Sample endpoint list** | Replaced the retired `ftp.debian.org` anonymous-FTP entry (Debian dropped FTP service years ago) with `test.rebex.net`, a dedicated always-on public FTP test server. |
| **Manual updater** | Fixed a bug where a failed download/extract left the tool frozen with no message and no way to close; now shows the error and re-enables the UI. Also handles zips that wrap a single top-level folder, and retries file copies briefly if the just-stopped exe is still momentarily locked. |
| **In-app updater** | See "Update-checker behavior" above — per-major-version feed routing, enabled checks, and a bounded startup timeout. |

## What's New in v3 Development

| Area | Change |
| --- | --- |
| **Premium UI** | Refined Endpoint List and SpeedTest screens with compact top controls, cleaner icon buttons, balanced spacing, and resize-resistant panels |
| **Endpoint grid** | Softer grey grid lines, darker headers, readable pre-scan text, and preserved row separation |
| **Column chooser** | New persistent column visibility chooser for the endpoint list |
| **Scan progress** | Compact Scan Progress section with last update text and a gradient progress bar |
| **Settings layout** | List, Common, HTTP, Status Export, and Output Folder controls reorganized to avoid clipping and wasted space |

---

## What's New in v3.0

| Area | Change |
| --- | --- |
| **Platform** | Migrated from .NET Framework 4.5 → **.NET 10**; ships as a **self-contained x86 executable** — no runtime install required |
| **Cloudflare bypass** | New three-mode bypass: Disabled / FlareSolverr / Playwright CDP (issue #32) |
| **Config dialog** | Full settings form with five tabs replacing the "open config in Notepad" shortcut (issue #8) |
| **Endpoint Management** | CRUD grid dialog for editing the endpoint list in-app, with Add / Delete / Move Up / Move Down (issue #9) |
| **Dark theme** | Navy palette across all controls, context menus, and both new dialogs |
| **Progress bar** | Animated shimmer bar showing real scan progress replacing the static GIF |
| **Crash fixes** | `NotifyIcon` reflection fix for .NET 10, cross-thread UI reads, inner exception chain reporting |
| **ARP stability** | `SemaphoreSlim` serialises `SendARP` P/Invoke calls — eliminates BSOD on parallel MAC lookups (issue #36) |
| **URL parsing** | Pipe-safe endpoint line parser (`Split(…, 2)`) + `Uri.EscapeUriString` fallback (issue #35) |
| **Headers** | Chrome-accurate HTTP headers — `Sec-Fetch-*`, corrected `Accept`, proper TLS fingerprint |

See [CHANGELOG.md](CHANGELOG.md) for the full technical change log.

---

## Requirements

- **OS:** Windows 10 / Server 2019 or later (Windows 7 / Server 2008 R2 minimum)
- **Runtime:** None — the application bundles the .NET 10 runtime (self-contained build)
- **Privileges:** Run as Administrator for ARP/MAC lookups and some network operations

> Versions up to 2.15 required .NET Framework 4.5. Versions 3.0 and newer target .NET 10 and ship
> the runtime bundled in the executable — no separate installation is needed.

---

## Installation

### Fresh install

Use the v3.1.1 RC prerelease ZIP from GitHub Releases for testing.

For development or test builds, publish the application from the `Main-Dev-V3` branch and extract the generated self-contained output to any folder. The app will create its data files (`EndpointChecker_EndpointsList.txt`, `EndpointChecker_LastSeenOnline.json`) alongside `EndpointChecker.exe` on first run.

Run `EndpointChecker.exe` as Administrator.

No installer, no registry keys, no separate runtime — the self-contained build bundles everything.

### Upgrading from v2.15 or earlier

Automatic upgrading to v3 is currently unavailable because this build is test-only and distributed as a prerelease ZIP.

When an official v3 release is prepared, the updater documentation will be updated to reference the release ZIP. Until then, keep v2.15 installations separate from manually supplied v3 test builds.

> **Note:** v3 development builds require Windows 10 (build 1607) or later. Machines running Windows 7 / Server 2008 R2 can continue using v2.15.

---

## Testing

### What runs during build

- `dotnet build` compiles the application and test projects but does **not** execute test suites automatically.
- Test execution in this repository is explicit and command-driven.

### Run all test suites

```powershell
dotnet run --project tests/EndpointDefinitionParser.Tests/EndpointDefinitionParser.Tests.csproj
dotnet run --project tests/EndpointCheckingCore.Tests/EndpointCheckingCore.Tests.csproj
dotnet run --project tests/HttpCompatibility.Tests/HttpCompatibility.Tests.csproj
dotnet run --project tests/ExportOptions.Tests/ExportOptions.Tests.csproj
dotnet run --project tests/ExportRunSummary.Tests/ExportRunSummary.Tests.csproj
dotnet run --project tests/ExportFileSet.Tests/ExportFileSet.Tests.csproj
dotnet run --project tests/StructuredExport.Tests/StructuredExport.Tests.csproj
```

### Build verification command

```powershell
dotnet build src/EndpointChecker.sln -c Release
```

---

## Refactoring Status

- Programme tracker: [refactoring-programme.md](refactoring-programme.md)
- Residual findings: [refactoring-residual-findings.md](refactoring-residual-findings.md)
- Reconciliation matrix: [docs/refactoring/reconciliation-2026-08-04.md](docs/refactoring/reconciliation-2026-08-04.md)

For a consolidated testing + RC + refactoring guide, see:

- [docs/v3-rc-testing-and-refactoring.md](docs/v3-rc-testing-and-refactoring.md)

---

## Quick Start

1. Launch `EndpointChecker.exe` as Administrator.
2. The default endpoint list loads automatically. Edit it via **File → Endpoint List**.
3. Press **Start** (or enable **Scan on startup** in **Settings → Configuration**) to run a scan.
4. Click any row to see full details in the endpoint details panel.

---

## Configuration

Open **Settings → Configuration** to access the full settings form:

| Tab | Settings |
| --- | --- |
| **Refresh & Notifications** | Auto-refresh toggle, continuous mode, interval (minutes), scan on startup, tray balloon alerts |
| **Scan** | Validation method (Protocol / Ping), ping/HTTP/FTP timeouts, parallel threads, SSL validation, auto-redirect |
| **Resolution** | DNS · IP · MAC · network shares · page meta · URL parameter stripping · link resolution · save response |
| **Export** | XLSX / JSON / XML / HTML export toggles and output directory |
| **Tools & API Keys** | VNC Viewer and PuTTY executable paths; VirusTotal and Google Maps API keys |

Settings are persisted in the user's application data profile and survive updates.

---

## Endpoint List Format

Endpoints are stored in `EndpointChecker_EndpointsList.txt` next to the executable.
Edit the file directly or use **File → Endpoint List** for an in-app grid editor.

```text
# Lines starting with # are comments — ignored by the parser
# Blank lines are also ignored

# Format:  Name|URL
Google|https://www.google.com
GitHub|https://github.com
Internal API|http://192.168.1.50:8080/health

# Name is optional — bare URLs are accepted
https://example.com
```

**Rules:**

- Separator is the **first** `|` on the line — pipe characters in the URL are safe.
- URLs with spaces or special characters are auto-escaped on load.
- Maximum 10 000 entries by default (configurable in Settings).

---

## Cloudflare / Bot-Protection Bypass

Endpoints protected by Cloudflare return HTTP 403/429/503 with a browser-challenge page.
`EndpointChecker` detects this via the `CF-RAY` response header and can attempt a bypass.

Open **Settings → CF Bypass** to configure the bypass method.

### Why HTTP header spoofing is not enough

`HttpWebRequest` sends a TLS `ClientHello` with a fixed cipher-suite list. Cloudflare's
edge compares this against known JA4 browser fingerprints. A .NET HTTP client's
fingerprint is instantly recognised as non-browser traffic — **header spoofing cannot fix
this**. Only a real Chromium instance (FlareSolverr or Playwright) presents the correct
JA4 fingerprint.

---

### Method 1 — FlareSolverr (Recommended)

[FlareSolverr](https://github.com/FlareSolverr/FlareSolverr) is a standalone proxy
server that uses an undetected Chromium driver to solve Cloudflare challenges and return
the page content to the caller over a local JSON API.

#### Option A — Docker (easiest)

**Prerequisites:** [Docker Desktop](https://www.docker.com/products/docker-desktop/)
installed and running.

```shell
docker run -d \
  --name flaresolverr \
  --restart unless-stopped \
  -p 8191:8191 \
  -e LOG_LEVEL=info \
  ghcr.io/flaresolverr/flaresolverr:latest
```

Verify it is running:

```shell
curl http://localhost:8191/health
# → {"status":"ok"}
```

#### Option B — Native Windows binary

1. Go to <https://github.com/FlareSolverr/FlareSolverr/releases/latest>.
2. Download `flaresolverr_windows_x64.zip`.
3. Extract the archive to any folder.
4. Run `flaresolverr.exe` — a console window opens and listens on port 8191 by default.

> FlareSolverr must be running **before** starting a scan. The app does not launch it
> automatically.

#### Configure in EndpointChecker

1. Open **Settings → CF Bypass**.
2. Select **FlareSolverr**.
3. Enter the URL (default: `http://localhost:8191`).
4. Click **OK**.

---

### Method 2 — Playwright CDP (Advanced)

[Playwright](https://playwright.dev) drives a local Chromium browser over the Chrome
DevTools Protocol (CDP). No external proxy is required, but Chromium binaries
(~120 MB) must be installed once on the machine.

#### Install Chromium for Playwright

##### Option A — via the in-app button

Open **Settings → CF Bypass**, select **Playwright**, and click
**Install / Update Chromium**. The app runs `playwright install chromium` in the
background.

##### Option B — via the .NET CLI

```shell
dotnet tool install --global Microsoft.Playwright.CLI
playwright install chromium
```

##### Option C — via PowerShell (after building from source)

```powershell
& "src\bin\x86\Debug\net10.0-windows\win-x86\EndpointChecker.exe" install chromium
```

#### Verify installation

Playwright stores Chromium in `%LOCALAPPDATA%\ms-playwright`. The CF Bypass dialog
reports the detected status on open.

#### Concurrency note

To prevent RAM exhaustion, EndpointChecker limits simultaneous Chromium instances to 2
when scanning many CF-protected endpoints in parallel.

---

### Bypass mode comparison

| | Disabled | FlareSolverr | Playwright |
| --- | --- | --- | --- |
| External dependency | None | FlareSolverr process | Chromium install (~120 MB) |
| Setup effort | — | Low (Docker one-liner) | Medium |
| RAM usage | Minimal | Low (runs in its own process) | ~150–300 MB per instance |
| Bypass strength | None | High | High |
| Recommended for | Quick checks | Production / automated scans | Local / dev use |

---

## API Keys

The application ships with public demo keys for the following APIs:

| API | Usage |
| --- | --- |
| [Google Maps](https://developers.google.com/maps/documentation/javascript/overview) | GeoIP map view |
| [VirusTotal](https://developers.virustotal.com/reference) | URL reputation scan |

Demo keys operate under usage limits. To use your own, open **Settings → Configuration → Tools & API Keys**.

---

## Building from Source

**Prerequisites:**

- Visual Studio 2022 (17.x+) with the **.NET desktop development** workload, **or** the .NET 10 SDK
- .NET 10 SDK — download from <https://dotnet.microsoft.com/download/dotnet/10.0>

**Clone and build:**

```shell
git clone https://github.com/ThePhOeNiX810815/Endpoint-Status-Checker.git
cd Endpoint-Status-Checker
dotnet build src/EndpointChecker.csproj -c Debug -p:Platform=x86
```

Output: `src\bin\x86\Debug\net10.0-windows\win-x86\EndpointChecker.exe`

**Publish self-contained release build:**

```shell
dotnet publish src/EndpointChecker.csproj -c Release -p:Platform=x86
```

Output: `src\bin\x86\Release\net10.0-windows\win-x86\publish\EndpointChecker.exe`

---

## Project Structure

```text
Endpoint-Status-Checker/
├── src/
│   ├── EndpointChecker.csproj        Main project file (SDK-style, net10.0-windows)
│   ├── CheckerMainForm.cs            Core application logic (~6 000 lines)
│   ├── CheckerMainForm.Designer.cs   WinForms layout
│   ├── ConfigDialog.cs               Settings dialog (5 tabs)
│   ├── EndpointManagementDialog.cs   Endpoint list CRUD grid
│   ├── CloudflareBypassChecker.cs    FlareSolverr + Playwright bypass engine
│   ├── CloudflareBypassSettingsDialog.*  CF bypass settings UI
│   ├── Program.cs                    Entry point, global state, startup
│   ├── ArpLookup/                    Windows ARP P/Invoke wrapper
│   └── Properties/                   Settings, AssemblyInfo, Resources
├── CHANGELOG.md
├── README.md
└── version.txt
```

---

## NuGet Packages

| Package | Version |
| --- | --- |
| AGauge | 2.0.1 |
| ClosedXML | 0.102.3 |
| DocumentFormat.OpenXml | 2.20.0 |
| ExcelNumberFormat | 1.1.0 |
| FastMember | 1.5.0 |
| Flurl | 3.0.6 |
| FreeSpire.XLS | 12.7.0 |
| HtmlAgilityPack | 1.11.72 |
| IPAddressRange | 4.2.0 |
| Microsoft-WindowsAPICodePack-Core | 1.1.5 |
| Microsoft-WindowsAPICodePack-Shell | 1.1.5 |
| Microsoft.Playwright | 1.50.0 |
| Nager.PublicSuffix | 2.2.2 |
| Newtonsoft.Json | 13.0.3 |
| System.DirectoryServices.AccountManagement | 8.0.0 |
| System.Management | 8.0.0 |
| WhoisClient.NET | 1.0.2.0 |

---

## Third-Party Tools

Optional external tools launched by the application:

- **VNC Viewer** (`vncviewer.exe`) — configure path in Settings → Tools & API Keys — <https://www.uvnc.com>
- **PuTTY** (`putty.exe`) — configure path in Settings → Tools & API Keys — <https://www.putty.org>

Embedded open-source components:

- **NSpeedTest** — <https://github.com/Kwull/NSpeedTest>
- **ArpLookup** — <https://github.com/georg-jung/ArpLookup>

---

## Known Limitations

- **SSL failures on corporate networks** — SSL/TLS errors on some sites (Discord, Shopify, etc.) may be caused by a corporate proxy aborting the connection at the TLS layer. This is not a bug in the application.
- **FlareSolverr must be started manually** — the application does not launch or restart it automatically.
- **Playwright requires one-time Chromium install** — see the Playwright setup section above.
- **x86 only** — the application is built as a 32-bit process to preserve `SendARP` P/Invoke compatibility.

---

## Security Notice

The application is not obfuscated — you are welcome to decompile and inspect it.

Bundled API keys in the default build are public demo keys shared across all default
installations. They carry usage quotas. Replace them with your own keys for production use.

---

```text
 ____      _              __  __            _            _
|  _ \ ___| |_ ___ _ __  |  \/  | __ _  ___| |__   __ _ (_)
| |_) / _ \ __/ _ \ '__| | |\/| |/ _` |/ __| '_ \ / _` || |
|  __/  __/ ||  __/ |    | |  | | (_| | (__| | | | (_| || |
|_|   \___|\__\___|_|    |_|  |_|\__,_|\___|_| |_|\__,_|/ |
                                                      |__/
```
