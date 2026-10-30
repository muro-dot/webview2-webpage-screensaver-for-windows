# Validation · 2026-10-30

Environment: Windows 11 x64, Python 3.14.6, PyInstaller 6.22.3, pywebview 6.2.1, pythonnet 3.2.1, Pillow 13.0.0, system WebView2 Runtime.

- Configuration tests: 5 passed. Covers original C# backup import, atomic save/reload, invalid-data rejection and URL/duration validation.
- Settings callbacks: language/theme redraw preserved unsaved URLs, duration, shuffle, mute and zoom; Save button round trip passed using a temporary settings file.
- Layout: actual English/Korean dark/light screenshots inspected; automatic bounds checks found no clipped labels. A 1000×700 viewport retains footer controls and scrolls the settings area.
- Source and frozen EXE: actual WebView2 local page rotation, missing-page fallback and private-session playback passed. Runtime values confirmed mute=true, zoom=125%, context menus disabled and developer tools disabled. Private browser data directory deletion confirmed after exit.
- External HTTPS: example.com loaded successfully in the packaged EXE.
- SCR: default no-argument playback with an explicit temporary configuration initialized WebView2, loaded the page and exited on its test timer.
- `.exe` and `.scr`: identical executable payload, 24,649,243 bytes each (24.65 MB / 23.51 MiB). Shared .NET Framework and WebView2 Runtime are excluded from this figure.

Windows screensaver registration was not changed. Physical multi-monitor layouts, mixed-monitor DPI and the embedded Windows thumbnail need target-hardware validation. Renderer crashes retry up to three times; a terminated WebView2 browser process closes the saver instead of attempting to reuse an invalid native controller. Release checking is read-only and does not replace this Python build with a C# release.

Detailed generated reports: `assets/layout-check.json`, `assets/integration-source.json`, `assets/integration-frozen.json`, `assets/web-network-check.json` and `assets/screensaver-check.json`.

## Fullscreen regression fix

- Reproduced on 3440×1440 at 125% Windows scaling: the initial pywebview Form was 3444×1415, leaving 25 pixels uncovered vertically.
- Borderless playback now sets the native HWND to physical monitor bounds before and after showing. DPI autoscaling and rounded edges are disabled for the saver window; windowed preview retains its normal behavior.
- Source and frozen SCR reports confirm both the window and its browser client area are exactly 3440×1440. Full desktop screenshots were visually inspected; the taskbar and screen edges are covered.
- Reports: `assets/fullscreen-source.json`, `assets/fullscreen-frozen.json`; regression command: `python tests/fullscreen.py --frozen`.

## Settings readability and selectors

- Body and button text now use 16-pixel design sizes with Windows DPI scaling; at 125%, the text renders at 20 physical pixels. Form dimensions, spacing and controls scale with it.
- Replaced native comboboxes with rounded themed selectors, checkmarks and hover/focus feedback. Arrow keys, Enter, Escape, Home/End and Tab are handled by the popup; selection notifications still use the existing settings path.
- UI smoke test passed: keyboard selection/cancellation, language/theme changes preserving edits, Save round trip and footer visibility in a smaller scrollable viewport.
- Actual base and open-menu screenshots inspected for Korean/English in dark/light mode. Source and packaged EXE layout checks passed for all four combinations.
