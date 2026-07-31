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

![image](https://raw.githubusercontent.com/ThePhOeNiX810815/Endpoint-Status-Checker/main/EndpointStatusCheckerImage.jpg)

Checks a pre-defined list of network endpoints on various conditions:

- Endpoint availability via **Protocol Scan (HTTP/HTTPS/FTP)** or simple **Ping**
- **Cloudflare / bot-protection bypass** via FlareSolverr or Playwright CDP
- External APIs: SpeedTest, GeoIP location, TraceRoute, VirusTotal scan, and more
- Export scan results to **XML, JSON, HTML, or XLSX**
- Automatic periodic or continuous scan with configurable interval
- System tray icon with status summary and notifications
- **Premium dark theme** UI

You can freely decompile the app — it is not obfuscated.

This software is safe to use on your host; ignore any false positives from AV scanners.

The application has an intelligent auto-update mechanism using GitHub packages.

---

## Version 3.0 (Beta) — What's New

- **Migrated to .NET 10** — ships as a self-contained executable; no .NET runtime install required
- **Cloudflare bypass** — FlareSolverr (recommended) and Playwright CDP methods
- **Full dark theme** — navy palette across all controls, readable row colours, dark context menus
- **Premium animated progress bar** — real scan progress with shimmer animation
- **Crash fixes** — NotifyIcon reflection fix for .NET 10, cross-thread UI reads, inner exception chain
- **Startup robustness** — endpoint list resolved relative to exe via `AppContext.BaseDirectory`

See [CHANGELOG.md](CHANGELOG.md) for the full list of changes.

---

## Requirements

- Windows 10 / Server 2019 or later (Windows 7 / Server 2008 R2 minimum)
- **No .NET installation required** — the application bundles the .NET 10 runtime (self-contained build)
- Run as Administrator (required for ARP/MAC lookups and some network operations)

> **Note:** Versions up to 2.15 required .NET Framework 4.5. Version 3.0 targets .NET 10 and
> drops the Framework dependency entirely.

---

## Cloudflare / Bot-Protection Bypass

Click the **CF BYPASS** toolbar button to configure the bypass method:

### FlareSolverr (recommended)

1. Download and run FlareSolverr from <https://github.com/FlareSolverr/FlareSolverr>
   - Docker: `docker run -p 8191:8191 ghcr.io/flaresolverr/flaresolverr`
   - Native Windows: download the latest release binary from the releases page
2. In the CF Bypass settings dialog, select **FlareSolverr** and enter the URL
   (default: `http://localhost:8191`)
3. FlareSolverr must be running before starting a scan

### Playwright CDP (optional)

- Select **Playwright** in the bypass settings dialog
- Requires the Playwright browser binaries to be installed on the host machine
- The app detects Playwright at startup and reports its status in the dialog

---

## Known Issues / Limitations

- SSL/TLS failures on some sites (e.g. Discord, Shopify) on corporate networks may be caused
  by the corporate proxy aborting connections — this is not a bug in the application
- FlareSolverr must be running before the scan starts; the app does not launch it automatically
- Playwright bypass requires manual Playwright installation

---

## Compiling from Source

The project is a Visual Studio 2022 solution targeting **.NET 10**.

**Prerequisites:**

- Visual Studio 2022 (17.x or later) with the **.NET desktop development** workload
- .NET 10 SDK (`win-x86` runtime)

**Build:**

```shell
dotnet build src/EndpointChecker.csproj
```

or open `EndpointChecker.sln` in Visual Studio and press **F5** / **Build**.

The output is placed in `src/bin/x86/Debug/net10.0-windows/win-x86/`.

---

## Public API Keys

The application uses the following public APIs (free keys bundled):

- **Google Maps** — <https://developers.google.com/maps/documentation/javascript/overview>
- **VirusTotal** — <https://developers.virustotal.com/reference>

All free keys operate under usage limits. You can supply your own keys via the **CONFIG**
button on the main window.

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
| Nager.PublicSuffix | 2.2.2 |
| Newtonsoft.Json | 13.0.3 |
| System.DirectoryServices.AccountManagement | 8.0.0 |
| System.Management | 8.0.0 |
| WhoisClient.NET | 1.0.2.0 |

---

## 3rd Party Tools

- **VNC Viewer** (`vncviewer.exe`) — <https://www.uvnc.com>
- **PuTTY** (`putty.exe`) — <https://www.putty.org>

Embedded project sources (adjusted):

- NSpeedTest — <https://github.com/Kwull/NSpeedTest>
- ArpLookup — <https://github.com/georg-jung/ArpLookup>

---

```text
 ____      _              __  __            _            _
|  _ \ ___| |_ ___ _ __  |  \/  | __ _  ___| |__   __ _ (_)
| |_) / _ \ __/ _ \ '__| | |\/| |/ _` |/ __| '_ \ / _` || |
|  __/  __/ ||  __/ |    | |  | | (_| | (__| | | | (_| || |
|_|   \___|\__\___|_|    |_|  |_|\__,_|\___|_| |_|\__,_|/ |
                                                      |__/
```
