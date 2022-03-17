# Endpoint Status Checker
### Homepage:
https://endpoint-status-checker.webnode.com

![image](https://raw.githubusercontent.com/ThePhOeNiX810815/Endpoint-Status-Checker/main/EndpointStatusCheckerImage.jpg)

Checks pre-defined list of network EndPoints on various conditions:

- checking endpoint availability based on 'Protocol Scan [HTTP/FTP]' or simple 'Ping'
- various external APIs used [as SpeedTest, GEO IP Loaction, TraceRoute, VirusTotal scan and more...]
- exporting scan result report [XML, JSON, HTML or XLS formats]
- automatic periodical or continuous scan options
- tray status icon and notifications

You can easily decompile the app, it's not obfuscated.

This piece of software is safe to use on your host, ignore any false positives.

Application have inteligent AutoUpdate method, using GitHub packages.

# Requirements
- .NET Framework 4.5 or later
- Application needs to be run As Administrator

# Known Issues
App may hang or stop responding, possibly throw unhandled exception from code.

All unhandled exceptions got to be automatically reported back to author. 

# To-Do List

Nothing by now, if you have something to suggest you can use 'Feature Request' button
on application main window and send your Feature Request or Improvement, or create GitHub Issue.

# Compiling From Source Code

Just open the solution in Visual Studio and click the (re)build button.

Application source is Visual Studio 2022 solution.

# Public API Keys Used

Application is using following public APIs:
- Google Maps (https://developers.google.com/maps/documentation/javascript/overview) (free key provided)
- VirusTotal (https://developers.virustotal.com/reference) (custom key must be created)

All keys are FREE to use, but mind that all these free APIs works with limits.

For each of these services you can set your own key (edit config file by 'CONFIG' button on main application window).

# NuGet Packages Used

Application is using following libraries:
- AGauge (2.0.1)
- ClosedXML (0.95.4)
- DocumentFormat.OpenXml (2.15.0)
- ExcelNumberFormat (1.1.0)
- FastMember (1.3.0) (a newer version available, but not for target framework version)
- Flurl (3.0.0-pre4) (a newer version available, but not for target framework version)
- FreeSpire.PDF (8.2.0)
- FreeSpire.XLS (12.2.0)
- HtmlAgilityPack (1.11.42)
- IPAddressRange (4.2.0)
- Nager.PublicSuffix (2.2.2) (a newer version available, but not for target framework version)
- Newtonsoft.Json (13.0.1)
- NSpeedTest (1.0.0) (a newer version available, but not for target framework version)
- System.IO.Compression (4.3.0)
- System.Net.Http (4.3.4)
- WhoisClient.NET (1.0.2) (a newer version available, but not for target framework version)
- WindowsAPICodePack-Core (1.1.2)
- WindowsAPICodePack-Shell (1.1.1)

# 3rd Party Applications Used

Application is using following external programs:
- VNC Viewer (vncviever.exe) - https://www.uvnc.com
- Putty (putty.exe) - https://www.putty.org
