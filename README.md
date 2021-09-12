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

# Requirements
- .NET Framework 4.5 or later
- Application needs to be run As Administrator

# Known issues
App may hang or stop responding, possibly throw unhandled exception from code.
All unhandled exceptions got to be automatically reported back to author. 

# To-do list

Nothing by now, if you have something to suggest you can use 'Feature Request' button
on application main window and send your Feature Request or Improvement.

# Compiling from source code

Just open the solution in Visual Studio and click the (re)build button.

# Public Keys Used

Application is using following public APIs:
- Google Maps (https://developers.google.com/maps/documentation/javascript/overview)
- VirusTotal (https://developers.virustotal.com/reference)

All keys are FREE to use, but mind that all these free APIs works with limits.
For each of these services you can set your own key (edit config file by 'CONFIG' button on main application window).

# NuGet Packages Used

Application is using following libraries:
- ClosedXML (0.95.4)
- DocumentFormat.OpenXml (2.13.1)
- ExcelNumberFormat (1.1.0)
- FastMember (1.3.0) (newer version available, but not for target framework version)
- FreeSpire.PDF (7.8.9)
- FreeSpire.XLS (11.8.6)
- HtmlAgilityPack (1.11.36)
- IPAddressRange (4.2.0)
- Nager.PublicSuffix (2.2.2)
- Newtonsoft.Json (13.0.1)
- NSpeedTest (1.0.0) (newer version available, but not for target framework version)
- System.Net.Http (4.3.4)
- WhoisClient.NET (1.0.1) (newer version available, but not for target framework version)
- WindowsAPICodePack-Core (1.1.2)
- WindowsAPICodePack-Shell (1.1.1)
