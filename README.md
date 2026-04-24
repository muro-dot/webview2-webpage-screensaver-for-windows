# WebView2 Web Page Screensaver

A Fork of the old archived project [ZenProjects/Chromium-Web-Page-Screensaver](https://github.com/ZenProjects/Chromium-Web-Page-Screensaver) that uses **Microsoft Edge WebView2 (Chromium)** in place of the [CefSharp WinForms](https://github.com/cefsharp/CefSharp) to display web pages as your screensaver.

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

# WebView2 웹 페이지 화면 보호기 (Web Page Screensaver)

이 프로젝트는 오래된 [ZenProjects/Chromium-Web-Page-Screensaver](https://github.com/ZenProjects/Chromium-Web-Page-Screensaver) 프로젝트를 포크하여 개선한 버전입니다. 기존의 [CefSharp WinForms](https://github.com/cefsharp/CefSharp) 대신 **Microsoft Edge WebView2 (Chromium)** 엔진을 사용하여 웹 페이지를 화면 보호기로 부드럽고 선명하게 출력합니다.

## 주요 개선 사항

- **고해상도(High DPI) 완벽 지원:** 4K 및 QHD 모니터에서 화면이 흐릿하게 보이던 문제를 해결하고, 1:1 픽셀 매칭으로 선명한 화질을 제공합니다.
- **최신 브라우저 엔진:** 최신 크로미움(Chromium) 기반의 Microsoft Edge WebView2 엔진을 탑재했습니다.
- **입력 감지 로직 강화:** 브라우저가 포커스를 가진 상태에서도 키보드 입력을 정확히 감지하여 화면 보호기가 정상적으로 종료되도록 수정했습니다.
- **성능 최적화:** 기존 CefSharp 빌드 대비 메모리 점유율을 대폭 낮추어 시스템 부담을 줄였습니다.

## 요구 사항

- [.NET Framework v4.6.2 이상](https://www.microsoft.com/en-us/download/details.aspx?id=53344 ".NET Framework")
- [Microsoft Edge WebView2 런타임](https://developer.microsoft.com/en-us/microsoft-edge/webview2/ "WebView2 Runtime")
- Windows 10 이상 (권장)

## 다운로드 및 설치 방법

- ***[최신 WebView2 화면 보호기 실행 파일 다운로드](https://github.com/muro-dot/Webview2_WebPage_Screensaver/releases/latest)***
- 다운로드한 압축 파일을 원하는 폴더(예: `C:\Program Files\WebScreensaver`)에 풉니다.
- 폴더 내의 `Webview2_WebPage_Screensaver.scr` 파일을 마우스 우클릭합니다.
- **'설치'**를 선택하여 시스템에 등록하거나, **'테스트'**를 눌러 즉시 실행해 볼 수 있습니다.
- 설치 후 나타나는 '화면 보호기 설정' 창에서 **'설정...'** 버튼을 눌러 출력할 웹 페이지 주소를 변경할 수 있습니다.

## 빌드 방법 (개발자용)

- 소스 코드를 클론(Clone)합니다.
- Visual Studio로 `.sln` 솔루션 파일을 엽니다 (VS 2022 권장).
- **NuGet 패키지 복원**을 통해 `Microsoft.Web.WebView2` 의존성을 다운로드합니다.
- 빌드 구성을 `Release`, 플랫폼을 `Any CPU` 또는 `x64`로 설정하여 빌드합니다.
- `bin/Release` 폴더에 생성된 `Webview2_WebPage_Screensaver.exe` 파일의 확장자를 `.scr`로 변경합니다.
- 생성된 `.scr` 파일을 우클릭하여 설치 및 사용합니다.

<img width="692" height="561" alt="설정 화면 미리보기" src="https://github.com/user-attachments/assets/2c0d02f8-5adc-45b5-b589-fe8e8fecbf45" />

