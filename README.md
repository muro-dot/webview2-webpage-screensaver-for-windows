# WebView2 Web Page Screensaver

A Fork of the old archived project [ZenProjects/Chromium-Web-Page-Screensaver](https://github.com/ZenProjects/Chromium-Web-Page-Screensaver) that uses **Microsoft Edge WebView2 (Chromium)** in place of the [CefSharp WinForms](https://github.com/cefsharp/CefSharp) to display web pages as your screensaver.

## Key Improvements

- **Audio Mute & InPrivate Browsing (v1.0.5):** Silent screensaver with automatic sound muting and privacy protection mode.
- **Graceful Fallback Clock (v1.0.5):** Elegant built-in neon digital clock when offline or page fails to load.
- **Live Mini Web Preview (v1.0.5):** Instant in-dialog preview of selected URLs with active zoom and mute settings.
- **Display Zoom Factor & Clock HUD (v1.0.5):** Scalable rendering (75%~200%) for 4K/QHD and sleek glassmorphism clock HUD overlay.
- **Config Backup & Restore (v1.0.5):** Single-click JSON export and import for seamless cross-PC setup.
- **System Theme Auto Switching (v1.0.4):** Real-time Light/Dark mode switching with DWM title bar sync.
- **Inline Editing & Vertical Toolbar (v1.0.4):** In-place double-click/F2 URL editing with compact right-side toolbar.
- **High DPI Support:** Sharp 1:1 pixel rendering on 4K/QHD monitors.
- **Modern WebView2 Engine:** Chromium-based Microsoft Edge WebView2 for low memory usage.
- **Multi-language Support (v1.0.3):** Instant switch between English and Korean (ENG/KOR).
- **Auto Update Checker (v1.0.5):** Background notification badge when newer GitHub releases are published.

## Dependencies

- [.NET Framework v4.8+](https://dotnet.microsoft.com/ko-kr/download/dotnet-framework/net48)
- [Microsoft Edge WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/)
- Windows 10 & up

## Download and Install

- Download the ***[Latest WebView2 Web Screensaver binary](https://github.com/muro-dot/Webview2_WebPage_Screensaver/releases/latest)*** ![Downloads](https://img.shields.io/github/downloads/muro-dot/Webview2_WebPage_Screensaver/total) 
- Unzip it to a permanent directory
- Find `Webview2_WebPage_Screensaver.scr` in the unziped directory, right click it
- Select `Install` to install, or `Test` to test it out without installing it
- If installing it, the windows `Screen Saver Settings` dialog will pop up with the correct screen saver already selected
- Use the `Settings...` button in the same dialog to change the web page(s) list displayed by the screen saver

## Build 

- Clone the source repository
- Open the `.sln` project file with Visual Studio (Tested with VS 2026).
- Restore NuGet packages to download the `Microsoft.Web.WebView2` dependency.
- Build in `Release` with `Any CPU` or `x64` mode
- Find `Webview2_WebPage_Screensaver.scr` in `bin/Release`
- Right click the `.scr` file, select `Install` to install, or `Test` to test it out
- Use the `Settings...` button to configure your custom URLs.

# WebView2 웹 페이지 화면 보호기 (Web Page Screensaver)

이 프로젝트는 오래된 [ZenProjects/Chromium-Web-Page-Screensaver](https://github.com/ZenProjects/Chromium-Web-Page-Screensaver) 프로젝트를 포크하여 개선한 버전입니다. 기존의 [CefSharp WinForms](https://github.com/cefsharp/CefSharp) 대신 **Microsoft Edge WebView2 (Chromium)** 엔진을 사용하여 웹 페이지를 화면 보호기로 부드럽고 선명하게 출력합니다.

## 주요 개선 사항

- **오디오 자동 음소거 및 시크릿 모드 (v1.0.5):** 웹페이지 소음 차단(기본 Mute) 및 쿠키/기록을 남기지 않는 안전한 브라우징.
- **오프라인/오류 시 우아한 모던 시계 폴백 (v1.0.5):** 인터넷 연결이 끊겨도 에러창 대신 세련된 네온 디지털 시계 자동 전환.
- **실시간 미니 웹 미리보기 (v1.0.5):** 화면보호기를 직접 실행하지 않고도 설정창에서 즉시 렌더링 상태 확인.
- **화면 배율 조절 & 시계 HUD 오버레이 (v1.0.5):** 4K/QHD 해상도 맞춤 배율(75%~200%) 및 글래스모피즘 디지털 시계 HUD.
- **설정 원클릭 백업/복원 (v1.0.5):** 전체 설정을 JSON 파일로 손쉽게 내보내고 타 PC에 불러오기 지원.
- **시스템 테마 실시간 자동 전환 (v1.0.4):** Windows 라이트/다크 모드 변경 시 재시작 없이 즉시 전환 및 타이틀바 일체화.
- **목록 내 직접 인라인 편집 (v1.0.4):** 별도 입력창 없이 목록에서 바로 더블클릭/F2로 수정 및 새 행 추가.
- **우측 일체형 버티컬 툴바 (v1.0.4):** 조작 버튼을 목록 우측에 밀착 배치하고 목록 뷰 높이 대폭 확장.
- **고해상도(High DPI) 완벽 지원:** 4K 및 QHD 모니터에서 1:1 픽셀 매칭으로 흐림 없는 선명한 화질 제공.
- **최신 WebView2 엔진 탑재:** 크로미움 기반 Edge WebView2 적용으로 메모리 점유율 및 성능 최적화.
- **다국어 지원 (v1.0.3):** 설정 창에서 한국어(KOR)와 영어(ENG) 실시간 전환 지원.
- **GitHub 최신 버전 자동 감지 (v1.0.5):** 새 릴리즈 출시 시 설정창 상단에 알림 뱃지 자동 표시.

## 요구 사항

- [.NET Framework v4.8+](https://dotnet.microsoft.com/ko-kr/download/dotnet-framework/net48)
- [Microsoft Edge WebView2 런타임](https://developer.microsoft.com/en-us/microsoft-edge/webview2/ "WebView2 Runtime")
- Windows 10 이상 (권장)

## 다운로드 및 설치 방법

- ***[최신 WebView2 화면 보호기 실행 파일 다운로드](https://github.com/muro-dot/Webview2_WebPage_Screensaver/releases/latest)*** ![Downloads](https://img.shields.io/github/downloads/muro-dot/Webview2_WebPage_Screensaver/total) 
- 다운로드한 압축 파일을 원하는 폴더(예: `C:\Program Files\WebScreensaver`)에 풉니다.
- 폴더 내의 `Webview2_WebPage_Screensaver.scr` 파일을 마우스 우클릭합니다.
- **'설치'**를 선택하여 시스템에 등록하거나, **'테스트'**를 눌러 즉시 실행해 볼 수 있습니다.
- 설치 후 나타나는 '화면 보호기 설정' 창에서 **'설정...'** 버튼을 눌러 출력할 웹 페이지 주소를 변경할 수 있습니다.

## 빌드 방법 (개발자용)

- 소스 코드를 클론(Clone)합니다.
- Visual Studio로 `.sln` 솔루션 파일을 엽니다 (VS 2026 권장).
- **NuGet 패키지 복원**을 통해 `Microsoft.Web.WebView2` 의존성을 다운로드합니다.
- 빌드 구성을 `Release`, 플랫폼을 `Any CPU` 또는 `x64`로 설정하여 빌드합니다.
- `bin/Release` 폴더에 생성된 `Webview2_WebPage_Screensaver.scr` 파일을 우클릭하여 설치 및 사용합니다.

### 테마 미리보기 (Dark Mode / Light Mode)

| 다크 모드 (Dark Mode) | 라이트 모드 (Light Mode) |
| :---: | :---: |
| <img width="420" alt="Dark Mode" src="assets/screenshot_dark.png" /> | <img width="420" alt="Light Mode" src="assets/screenshot_light.png" /> |

