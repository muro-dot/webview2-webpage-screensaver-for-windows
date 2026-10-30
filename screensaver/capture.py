"""Capture the actual Tk window for every language/theme pair, never a UI mockup."""
import json
from pathlib import Path
import shutil
import tkinter as tk
import tkinter.font as tkfont
from PIL import ImageGrab

from .config import Settings
from .ui.app import SettingsApp


def capture_assets(directory):
    directory = Path(directory)
    directory.mkdir(parents=True, exist_ok=True)
    results = []
    for language in ('en', 'ko'):
        for theme in ('dark', 'light'):
            settings = Settings(language=language, theme=theme)
            settings.displays[0].urls = ['https://example.com/screensaver|60', 'https://www.wikipedia.org|30']
            app = SettingsApp(settings, capture=True)
            root = app.root
            root.update_idletasks()
            root.geometry(f'{root.winfo_width()}x{root.winfo_height()}+40+40')
            root.update()
            root.lift()
            def snapshot():
                root.update_idletasks()
                issues = []
                def inspect(widget):
                    if isinstance(widget, tk.Canvas):
                        for item in widget.find_all():
                            if widget.type(item) == 'text':
                                box = widget.bbox(item)
                                if box and (box[0] < 0 or box[2] > widget.winfo_width()):
                                    issues.append({'canvas_text': widget.itemcget(item, 'text')})
                    if isinstance(widget, tk.Label) and not widget.cget('wraplength'):
                        if widget.winfo_reqwidth() > widget.winfo_width():
                            issues.append({'label': widget.cget('text'), 'requested': widget.winfo_reqwidth(), 'width': widget.winfo_width()})
                    if widget.winfo_ismapped():
                        x, y = widget.winfo_rootx(), widget.winfo_rooty()
                        parent = widget.master
                        if parent is not None and (x + widget.winfo_width() > parent.winfo_rootx() + parent.winfo_width() + 2 or y + widget.winfo_height() > parent.winfo_rooty() + parent.winfo_height() + 2):
                            issues.append({'outside_parent': str(widget)})
                        if x + widget.winfo_width() > root.winfo_rootx() + root.winfo_width() + 2 or y + widget.winfo_height() > root.winfo_rooty() + root.winfo_height() + 2:
                            issues.append({'outside_window': str(widget)})
                    for child in widget.winfo_children():
                        inspect(child)
                inspect(root)
                name = f'screenshot_{"ko_" if language == "ko" else ""}{theme}.png'
                x, y = root.winfo_rootx(), root.winfo_rooty()
                ImageGrab.grab(bbox=(x, y, x + root.winfo_width(), y + root.winfo_height()), include_layered_windows=True).save(directory / name)
                results.append({'language': language, 'theme': theme, 'scale': app.ui_scale, 'issues': issues})
                app.mode_combo.open_menu()
                def popup_snapshot():
                    root.update_idletasks()
                    popup = app.mode_combo.popup
                    if popup is None:
                        issues.append({'popup': 'Menu closed before screenshot.'})
                    else:
                        if popup.winfo_rooty() + popup.winfo_height() > root.winfo_rooty() + root.winfo_height():
                            issues.append({'popup': 'Menu outside capture bounds.'})
                        ImageGrab.grab(bbox=(x, y, x + root.winfo_width(), y + root.winfo_height()), include_layered_windows=True).save(directory / f'dropdown_{language}_{theme}.png')
                    app.mode_combo.close_menu()
                    app.close()
                root.after(250, popup_snapshot)
            root.after(600, snapshot)
            app.run()
    shutil.copyfile(directory / 'screenshot_dark.png', directory / 'screenshot.png')
    (directory / 'layout-check.json').write_text(json.dumps(results, ensure_ascii=False, indent=2), encoding='utf-8')
    if any(item['issues'] for item in results):
        raise ValueError('Layout verification found clipped text; inspect assets/layout-check.json.')
