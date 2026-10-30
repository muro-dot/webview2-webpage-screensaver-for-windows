# Only the EdgeChromium backend is shipped; Qt/CEF engines are intentionally excluded.
from pathlib import Path
import webview

webview_root = Path(webview.__file__).parent
browser_files = [(str(webview_root / 'lib'), 'webview/lib'), (str(webview_root / 'js'), 'webview/js')]
a = Analysis(['main.py'], pathex=[], binaries=[], datas=[('assets/app.ico', 'assets')] + browser_files,
    hiddenimports=['webview.platforms.winforms', 'webview.platforms.edgechromium', 'clr_loader'],
    hookspath=[], hooksconfig={}, runtime_hooks=[], excludes=['PyQt5', 'PyQt6', 'PySide2', 'PySide6', 'cefpython3', 'webview.platforms.cef', 'webview.platforms.qt', 'webview.platforms.gtk'],
    noarchive=False, optimize=1)
pyz = PYZ(a.pure)
exe = EXE(pyz, a.scripts, a.binaries, a.datas, [], name='WebScreensaver', debug=False,
    bootloader_ignore_signals=False, strip=False, upx=False, console=False, icon='assets/app.ico',
    manifest='app.manifest')
