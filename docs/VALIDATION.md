# Validation · 2026-10-30

Environment: Windows 11 x64, Python 3.14.6, PyInstaller 6.22.3, pywebview 6.2.1, pythonnet 3.2.1, Pillow 13.0.0, system WebView2 Runtime.

- Configuration tests: 5 passed. Covers original C# backup import, atomic save/reload, invalid-data rejection and URL/duration validation.
- Settings callbacks: language/theme redraw preserved unsaved URLs, duration, shuffle, mute and zoom; Save button round trip passed using a temporary settings file.
- Layout: actual English/Korean dark/light screenshots inspected; automatic bounds checks found no clipped labels. A 1000×700 viewport retains footer controls and scrolls the settings area.
- Source and frozen EXE: actual WebView2 local page rotation, missing-page fallback and private-session playback passed. Runtime values confirmed mute=true, zoom=125%, context menus disabled and developer tools disabled. Private browser data directory deletion confirmed after exit.
- External HTTPS: example.com loaded successfully in the packaged EXE.
- SCR: default no-argument playback with an explicit temporary configuration initialized WebView2, loaded the page and exited on its test timer.
- `.exe` and `.scr`: identical executable payload, 24,636,923 bytes each (24.64 MB / 23.50 MiB). Shared .NET Framework and WebView2 Runtime are excluded from this figure.

Windows screensaver registration was not changed. Physical multi-monitor layouts, mixed-monitor DPI and the embedded Windows thumbnail need target-hardware validation. Renderer crashes retry up to three times; a terminated WebView2 browser process closes the saver instead of attempting to reuse an invalid native controller. Release checking is read-only and does not replace this Python build with a C# release.

Detailed generated reports: `assets/layout-check.json`, `assets/integration-source.json`, `assets/integration-frozen.json`, `assets/web-network-check.json` and `assets/screensaver-check.json`.
