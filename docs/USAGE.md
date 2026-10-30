# 실행 / Usage

Windows 11 x64, .NET Framework 4.8 and Microsoft Edge WebView2 Evergreen Runtime are required. Python is bundled in the executable. The shared WebView2 Runtime is not bundled.

- `dist/WebScreensaver.exe`: open settings. Save changes before closing.
- `dist/WebScreensaver.scr`: right-click **Install** or **Test**; Windows invokes `/c` for configuration and `/s` for playback.
- `WebScreensaver.exe /s`: run the screensaver directly.
- `WebScreensaver.exe --preview`: open a resizable browser preview.
- `/p HWND` and `/p:HWND`: show a local clock inside the Windows screensaver thumbnail.
- Escape closes browser preview; full-screen playback exits on activity after a 1.5-second launch grace period. With exit-on-activity disabled, use the visible Close button or Escape.
- Enter one URL per line. `https://example.com|60` overrides its duration to 60 seconds. Empty playlists display a local clock.
- Mirror/Span use the first display profile. Separate uses each display's own profile; unconfigured displays show the clock.
- Settings are saved to `%LOCALAPPDATA%/WebScreensaverPython/settings.json`. On first use, existing `HKCU/Software/Web-Page-Screensaver` preferences are read without changing the original application. Existing JSON backups can be imported.
- The Windows thumbnail is a local clock; use **Preview** for actual web content.
- Release checking opens the original project's releases. The Python branch is not automatically installed over a C# release.

# 개발 / Development

Use Python 3.14 x64 and PowerShell:

```powershell
python -m venv .venv
.\.venv\Scripts\python.exe -m pip install -r requirements.txt
.\.venv\Scripts\python.exe main.py
.\build.ps1
```

`build.ps1` runs configuration tests, captures four actual UI screenshots, checks layout and builds a single executable plus an identical `.scr` copy. `assets/screenshot.png` is always English dark mode. The build does not change Windows screensaver registration.

```powershell
.\.venv\Scripts\python.exe tests/integration.py
.\.venv\Scripts\python.exe tests/integration.py --frozen
```

Integration tests require an unlocked interactive desktop. They open temporary browser previews, validate local page rotation, missing-file clock fallback and private mode, then close automatically. Test reports are in `assets/integration-*.json`. Actual external page behavior depends on the network and the site.

`main.py` handles Windows arguments; `screensaver/config.py` validates and migrates settings; `screensaver/ui/` implements the Tkinter/Pillow interface; `screensaver/player.py` manages native WebView2 playback. Tk settings and browser playback use separate processes. Qt and CEF are excluded from the frozen build.

The executable extracts its bundled Python dependencies to a temporary directory while running. Its on-disk EXE size is therefore different from the temporary extracted footprint. Fonts use installed Windows fonts; no third-party font bundle is included.

Multi-monitor hardware and Windows screensaver installation should be checked on the target machine. Automated tests do not install the screensaver or change system settings.
