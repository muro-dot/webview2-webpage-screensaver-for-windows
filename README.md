# WebView2 Web Page Screensaver

A Fork of the old archived project [cwc/web-page-screensaver](https://github.com/cwc/web-page-screensaver) that uses **Microsoft Edge WebView2 (Chromium)** in place of the [CefSharp WinForms](https://github.com/cefsharp/CefSharp) to display web pages as your screensaver.

## Key Improvements

- **High DPI Support:** Perfect 1:1 pixel rendering on 4K/QHD monitors (No more blurry screens).
- **Modern Engine:** Powered by the latest Chromium-based Microsoft Edge WebView2.
- **Enhanced Input Detection:** Fixed issues where the screensaver wouldn't exit on keyboard input when the browser had focus.
- **Improved Performance:** Lower memory footprint compared to legacy CefSharp builds.

## Dependencies

- [.NET Framework v4.6.2+](https://www.microsoft.com/en-us/download/details.aspx?id=53344 ".NET Framework")
- [Microsoft Edge WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/ "WebView2 Runtime")
- Windows 10 & up

## Download and Install

- Download the ***[Latest WebView2 Web Screensaver binary](https://github.com/muro-dot/Webview2_WebPage_Screensaver/releases/latest)***
- Unzip it to a permanent directory
- Find `Webview2_WebPage_Screensaver.scr` in the unziped directory, right click it
- Select `Install` to install, or `Test` to test it out without installing it
- If installing it, the windows `Screen Saver Settings` dialog will pop up with the correct screen saver already selected
- Use the `Settings...` button in the same dialog to change the web page(s) list displayed by the screen saver

## Build 

- Clone the source repository
- Open the `.sln` project file with Visual Studio (Tested with VS 2022).
- Restore NuGet packages to download the `Microsoft.Web.WebView2` dependency.
- Build in `Release` with `Any CPU` or `x64` mode
- Find `Webview2_WebPage_Screensaver.exe` in `bin/Release`, and rename it to `Webview2_WebPage_Screensaver.scr`
- Right click the `.scr` file, select `Install` to install, or `Test` to test it out
- Use the `Settings...` button to configure your custom URLs.
