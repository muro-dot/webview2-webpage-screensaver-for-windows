"""Windows .scr command line and settings entry point."""
import argparse
import logging
from pathlib import Path
import sys

from screensaver.config import CONFIG_PATH, load
from screensaver.windows import dpi_aware


def main():
    dpi_aware()
    arguments = sys.argv[1:]
    default_mode = '/s' if getattr(sys, 'frozen', False) and Path(sys.executable).suffix.lower() == '.scr' else '/c'
    mode = arguments.pop(0).lower() if arguments and arguments[0].startswith('/') else default_mode
    parser = argparse.ArgumentParser(description='Python WebView2 Screensaver')
    parser.add_argument('--preview', action='store_true')
    parser.add_argument('--config', type=Path, default=CONFIG_PATH)
    parser.add_argument('--seconds', type=float, default=0)
    parser.add_argument('--report', type=Path)
    parser.add_argument('--capture-assets', type=Path)
    parser.add_argument('--language', choices=['ko', 'en'])
    parser.add_argument('--theme', choices=['dark', 'light', 'system'])
    parent = None
    if mode.startswith('/p'):
        handle = mode.partition(':')[2] or (arguments.pop(0) if arguments else '0')
        parent = int(handle)
        if not parent:
            return
    elif mode.split(':')[0] not in ('/s', '/c'):
        raise ValueError(f'Unsupported screensaver mode: {mode}')
    options = parser.parse_args(arguments)
    CONFIG_PATH.parent.mkdir(parents=True, exist_ok=True)
    logging.basicConfig(filename=CONFIG_PATH.parent / 'screensaver.log', level=logging.INFO,
                        format='%(asctime)s %(levelname)s %(message)s', encoding='utf-8')
    settings = load(options.config)
    if options.language:
        settings.language = options.language
    if options.theme:
        settings.theme = options.theme
    if options.capture_assets:
        from screensaver.capture import capture_assets
        capture_assets(options.capture_assets)
    elif parent:
        from screensaver.player import preview_thumbnail
        preview_thumbnail(parent, settings.language, options.seconds)
    elif mode == '/s' or options.preview:
        from screensaver.player import Playback
        Playback(settings, preview=options.preview, seconds=options.seconds, report=options.report).run()
    else:
        from screensaver.ui.app import SettingsApp
        app = SettingsApp(settings, path=options.config)
        if options.seconds:
            app.capture = True
            app.root.after(int(options.seconds * 1000), app.close)
        app.run()


if __name__ == '__main__':
    try:
        main()
    except Exception as error:
        logging.exception('Application failed')
        import ctypes
        if not any(flag in sys.argv for flag in ('--capture-assets', '--report')):
            ctypes.windll.user32.MessageBoxW(None, f'Web Screensaver\n\n{error}\n\nLog: {CONFIG_PATH.parent / "screensaver.log"}', 'Web Screensaver', 0x10)
        raise SystemExit(1)
