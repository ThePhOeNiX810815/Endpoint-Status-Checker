```
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

### Homepage:
https://endpoint-status-checker.webnode.page

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
- Windows 7 / Server 2008 R2 or later
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

Application is using following public APIs (free keys provided):
- Google Maps (https://developers.google.com/maps/documentation/javascript/overview)
- VirusTotal (https://developers.virustotal.com/reference)

All keys are FREE to use, but mind that all these free APIs works with limits.

For each of these services you can set your own key (edit config file by 'CONFIG' button on main application window).

# NuGet Packages Used

Application is using following libraries:
- AGauge (2.0.1)
- ClosedXML (0.96.0)
- DocumentFormat.OpenXml (2.20.0)
- ExcelNumberFormat (1.1.0)
- FastMember (1.3.0)
- Flurl (3.0.0-pre4)
- FreeSpire.XLS (12.7.0)
- HtmlAgilityPack (1.11.51)
- IPAddressRange (4.2.0)
- Nager.PublicSuffix (2.2.2)
- Newtonsoft.Json (13.0.3)
- System.IO.Compression (4.3.0)
- System.Net.Http (4.3.4)
- System.Runtime.InteropServices.RuntimeInformation (4.3.0)
- System.ValueTuple (4.5.0)
- WhoisClient.NET (1.0.2.0)
- WindowsAPICodePack-Core (1.1.2)
- WindowsAPICodePack-Shell (1.1.1)

# 3rd Party Applications Used

Application is using following external programs:
- VNC Viewer (vncviever.exe) - https://www.uvnc.com
- Putty (putty.exe) - https://www.putty.org


NSpeedTest project sources (adjusted) added to solution:
- https://github.com/Kwull/NSpeedTest

ArpLookup project sources (adjusted) added to solution:
- https://github.com/georg-jung/ArpLookup


```
 ____      _              __  __            _            _
|  _ \ ___| |_ ___ _ __  |  \/  | __ _  ___| |__   __ _ (_)
| |_) / _ \ __/ _ \ '__| | |\/| |/ _` |/ __| '_ \ / _` || |
|  __/  __/ ||  __/ |    | |  | | (_| | (__| | | | (_| || |
|_|   \___|\__\___|_|    |_|  |_|\__,_|\___|_| |_|\__,_|/ |
                                                      |__/
```
