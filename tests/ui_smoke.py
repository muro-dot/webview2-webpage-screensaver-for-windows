"""Exercise settings callbacks and capture a small viewport without touching user data."""
from pathlib import Path
import sys
import tempfile
import tkinter as tk
from PIL import ImageGrab

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))
from screensaver.config import Settings, load
from screensaver.ui.app import SettingsApp
from screensaver.ui.theme import Button
from screensaver.windows import dpi_aware


def main():
    dpi_aware()
    with tempfile.TemporaryDirectory() as directory:
        path = Path(directory) / 'settings.json'
        app = SettingsApp(Settings(language='ko', theme='dark'), capture=True, path=path)
        app.root.update()
        app.url_text.delete('1.0', 'end')
        app.url_text.insert('1.0', 'https://example.com|45\nhttps://www.wikipedia.org')
        app.interval.set('60')
        app.zoom.set('150')
        app.randomize.set(True)
        app.variables['mute'].set(False)
        def widgets(widget):
            for child in widget.winfo_children():
                yield child
                yield from widgets(child)
        language_button = next(w for w in widgets(app.root) if isinstance(w, Button) and w.label == '한국어  /  EN')
        language_button.invoke()
        assert app.settings.language == 'en'
        app.theme_combo.open_menu()
        app.root.update()
        app.theme_combo.menu.event_generate('<Down>')
        app.root.update()
        assert app.theme_combo.active == 2
        app.theme_combo.menu.event_generate('<Return>')
        app.root.update()
        assert app.settings.theme == 'light'
        app.mode_combo.open_menu()
        app.root.update()
        app.mode_combo.menu.event_generate('<Down>')
        app.root.update()
        app.mode_combo.menu.event_generate('<Escape>')
        app.root.update()
        assert app.mode_combo.popup is None and app.mode_combo.current() == 0, (app.mode_combo.popup, app.mode_combo.current())
        app.position_combo.open_menu()
        app.root.update()
        app.position_combo.menu.event_generate('<End>')
        app.position_combo.menu.event_generate('<Return>')
        app.root.update()
        assert app.position_combo.current() == 3
        app.root.update()
        save_button = next(w for w in widgets(app.root) if isinstance(w, Button) and w.label == 'Save settings')
        save_button.invoke()
        saved = load(path)
        assert saved.displays[0].interval == 60 and saved.displays[0].zoom == 150
        assert len(saved.displays[0].urls) == 2 and saved.displays[0].randomize
        assert not saved.mute and saved.language == 'en' and saved.theme == 'light'
        app.root.geometry(f'{app.s(1080)}x{app.s(720)}+40+40')
        app.root.update()
        def snapshot():
            app.root.update_idletasks()
            x, y = app.root.winfo_rootx(), app.root.winfo_rooty()
            ImageGrab.grab(bbox=(x, y, x + app.root.winfo_width(), y + app.root.winfo_height())).save(ROOT / 'assets' / 'screenshot_small.png')
            for button in [w for w in widgets(app.root) if isinstance(w, Button) and w.label in ('Save settings', 'Preview')]:
                assert button.winfo_rooty() + button.winfo_height() < y + app.root.winfo_height()
            app.close()
        app.root.after(500, snapshot)
        app.run()
    print('PASS: popup keyboard navigation, Enter/Escape, language/theme preserves edits, Save, small viewport.')


if __name__ == '__main__':
    main()
