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

## Homepage

<https://endpoint-status-checker.webnode.page>

![screenshot](https://raw.githubusercontent.com/ThePhOeNiX810815/Endpoint-Status-Checker/Main-Dev-Branch/EndpointStatusCheckerImage.png)

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

> Versions up to 2.15 required .NET Framework 4.5. Version 3.0 targets .NET 10 and ships
> the runtime bundled in the executable — no separate installation is needed.

---

## Installation

1. Download [**EndpointChecker-v3.0.0-win-x86.zip**](https://github.com/ThePhOeNiX810815/Endpoint-Status-Checker/releases/download/v3.0.0/EndpointChecker-v3.0.0-win-x86.zip) from the [latest release](https://github.com/ThePhOeNiX810815/Endpoint-Status-Checker/releases/latest) (~102 MB — the .NET 10 runtime is bundled).
2. Extract the archive to any folder. It will create its data files (`EndpointChecker_EndpointsList.txt`, `EndpointChecker_LastSeenOnline.json`) alongside `EndpointChecker.exe` on first run.
3. Run `EndpointChecker.exe` as Administrator.

No installer, no registry keys, no separate runtime — the self-contained build bundles everything.

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
