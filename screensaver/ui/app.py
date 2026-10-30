"""Bilingual settings editor. All form state is collected before destructive redraws."""
from __future__ import annotations

import copy
import json
import os
from pathlib import Path
import subprocess
import sys
import tempfile
import tkinter as tk
from tkinter import filedialog, messagebox, ttk

from ..config import CONFIG_PATH, Display, Settings, load
from ..windows import monitors, system_dark
from .theme import Button, DARK, LIGHT, Toggle


TEXT = {
    'ko': dict(title='나만의 웹, 나만의 스크린세이버', subtitle='좋아하는 웹페이지로 잠든 화면에 새로운 풍경을 만드세요.',
        settings='화면보호기 설정', displays='디스플레이', profile='디스플레이 프로필', screen='모니터', mode='다중 모니터 모드',
        modes=['개별 화면', '동일한 화면', '전체 화면 연결'], theme='테마', themes=['시스템', '다크', '라이트'],
        playlist='웹페이지 플레이리스트', playlist_sub='주소를 한 줄에 하나씩 입력하세요. 순서대로 화면에 표시됩니다.',
        hint='개별 표시 시간: https://example.com|60  ·  빈 목록은 시계를 표시합니다.',
        playback='재생 설정', interval='기본 표시 시간 (초)', zoom='화면 배율 (%)', random='무작위 순서',
        behavior='화면 동작', mute='소리 끄기', private='비공개 세션', clock='시계 오버레이', close='입력 시 종료',
        position='시계 위치', positions=['오른쪽 아래', '왼쪽 아래', '오른쪽 위', '왼쪽 위'],
        note='웹페이지는 Microsoft Edge\nWebView2로 재생됩니다.\nPython 설치 없이 실행하세요.',
        import_='가져오기', export='내보내기', preview='미리보기', save='저장', saved='설정을 저장했습니다.',
        ready='READY', engine='시스템 WebView2 · 가벼운 Python UI', error='설정을 확인하세요',
        imported='설정을 불러왔습니다. 저장하면 적용됩니다.', unsaved='변경한 설정을 저장하지 않고 닫을까요?',
        update='릴리스 확인', checking='릴리스 정보를 확인하고 있습니다…', no_release='연결할 수 없습니다. 잠시 후 다시 시도하세요.',
        latest='최신 릴리스', launch='화면보호기 실행', urls='개의 웹페이지', reset='기본값', reset_confirm='설정창을 기본값으로 되돌릴까요? 저장 전까지 파일은 유지됩니다.'),
    'en': dict(title='Your web. A calmer screen.', subtitle='Turn your favorite pages into a beautifully simple screensaver.',
        settings='Screensaver settings', displays='DISPLAYS', profile='Display profile', screen='Display', mode='Multi-monitor mode',
        modes=['Separate', 'Mirror', 'Span'], theme='Theme', themes=['System', 'Dark', 'Light'],
        playlist='Web page playlist', playlist_sub='Add one address per line. Pages play in the order shown.',
        hint='Custom duration: https://example.com|60  ·  An empty playlist shows a clock.',
        playback='Playback', interval='Default duration (sec)', zoom='Page zoom (%)', random='Shuffle pages',
        behavior='Screen behavior', mute='Mute audio', private='Private session', clock='Clock overlay', close='Exit on activity',
        position='Clock position', positions=['Bottom right', 'Bottom left', 'Top right', 'Top left'],
        note='Pages run on Microsoft Edge\nWebView2. No Python\ninstallation required.',
        import_='Import', export='Export', preview='Preview', save='Save settings', saved='Settings saved.',
        ready='READY', engine='System WebView2 · Lightweight Python UI', error='Check your settings',
        imported='Settings imported. Save to apply.', unsaved='Close without saving your changes?',
        update='Check releases', checking='Checking releases…', no_release='Unable to connect. Please try again later.',
        latest='Latest release', launch='Run screensaver', urls='web pages', reset='Reset', reset_confirm='Reset the editor to defaults? The saved file remains unchanged until you save.')
}


def command(*arguments):
    if getattr(sys, 'frozen', False):
        return [sys.executable, *arguments]
    return [sys.executable, str(Path(__file__).resolve().parents[2] / 'main.py'), *arguments]


class SettingsApp:
    def __init__(self, settings=None, capture=False, path=CONFIG_PATH):
        self.root = tk.Tk()
        self.root.title('Web Screensaver')
        self.root.geometry('1080x900')
        self.root.minsize(1000, 640)
        self.path, self.capture = Path(path), capture
        self.settings = copy.deepcopy(settings or load(self.path))
        self.original = copy.deepcopy(self.settings)
        self.selected = 0
        self.children = []
        self.preview_files = []
        self.release_queue = None
        self.alive = True
        self.p = None
        self.root.protocol('WM_DELETE_WINDOW', self.close)
        self.root.bind('<Control-s>', lambda e: self.save())
        self.root.bind('<Escape>', lambda e: self.close())
        icon = Path(getattr(sys, '_MEIPASS', Path(__file__).resolve().parents[2])) / 'assets' / 'app.ico'
        if icon.exists():
            self.root.iconbitmap(str(icon))
        self.render()
        self.theme_timer = self.root.after(1500, self.poll_theme)
        self.release_timer = self.root.after(2500, lambda: self.check_release(automatic=True)) if not capture else None

    def t(self, key):
        return TEXT[self.settings.language][key]

    def label(self, master, text, size=13, muted=False, bold=False, **kwargs):
        return tk.Label(master, text=text, bg=master.cget('bg'), fg=self.p['muted'] if muted else self.p['text'],
                        font=('Malgun Gothic', -size, 'bold' if bold else 'normal'), anchor='w', **kwargs)

    def card(self, master):
        return tk.Frame(master, bg=self.p['card'], highlightbackground=self.p['border'], highlightthickness=1, padx=22, pady=20)

    def combo(self, master, values, index, callback=None, width=22):
        result = ttk.Combobox(master, values=values, state='readonly', width=width, font=('Malgun Gothic', -13))
        result.current(index)
        if callback:
            result.bind('<<ComboboxSelected>>', callback)
        return result

    def render(self):
        for child in self.root.winfo_children():
            child.destroy()
        dark = self.settings.theme == 'dark' or (self.settings.theme == 'system' and system_dark())
        self.p = DARK if dark else LIGHT
        p = self.p
        self.root.configure(bg=p['bg'])
        style = ttk.Style(self.root)
        style.theme_use('clam')
        style.configure('TCombobox', fieldbackground=p['inset'], background=p['card'], foreground=p['text'], bordercolor=p['border'], arrowcolor=p['muted'], padding=7)
        style.map('TCombobox', fieldbackground=[('readonly', p['inset'])], foreground=[('readonly', p['text'])], selectbackground=[('readonly', p['inset'])], selectforeground=[('readonly', p['text'])])
        for orientation in ('Vertical', 'Horizontal'):
            style.configure(f'{orientation}.TScrollbar', background=p['border'], troughcolor=p['inset'], bordercolor=p['inset'],
                lightcolor=p['border'], darkcolor=p['border'], arrowcolor=p['muted'], arrowsize=12, width=12)
            style.map(f'{orientation}.TScrollbar', background=[('active', p['muted'])])
        self.root.option_add('*TCombobox*Listbox.background', p['card'])
        self.root.option_add('*TCombobox*Listbox.foreground', p['text'])

        header = tk.Frame(self.root, bg=p['bg'])
        header.pack(fill='x', padx=32, pady=(24, 0))
        brand = tk.Frame(header, bg=p['bg'])
        brand.pack(fill='x')
        self.label(brand, '◈  WEB SCREENSAVER', 14, bold=True).pack(side='left')
        Button(brand, '한국어  /  EN', self.switch_language, p, width=130, height=34).pack(side='right')
        self.label(header, self.t('title'), 28, bold=True).pack(anchor='w', pady=(16, 4))
        self.label(header, self.t('subtitle'), 13, muted=True).pack(anchor='w')

        toolbar = tk.Frame(self.root, bg=p['bg'])
        toolbar.pack(fill='x', padx=32, pady=(20, 15))
        self.label(toolbar, self.t('settings'), 13, bold=True).pack(side='left')
        self.theme_combo = self.combo(toolbar, self.t('themes'), ['system', 'dark', 'light'].index(self.settings.theme), self.switch_theme, width=10)
        self.theme_combo.pack(side='right')
        self.label(toolbar, self.t('theme'), muted=True).pack(side='right', padx=16)

        footer = tk.Frame(self.root, bg=p['bg'])
        footer.pack(side='bottom', fill='x', padx=32, pady=(14, 22))
        buttons = tk.Frame(footer, bg=p['bg'])
        buttons.pack(fill='x')
        Button(buttons, self.t('import_'), self.import_settings, p, width=88).pack(side='left')
        Button(buttons, self.t('export'), self.export_settings, p, width=88).pack(side='left', padx=16)
        Button(buttons, self.t('reset'), self.reset, p, width=80).pack(side='left')
        Button(buttons, self.t('save'), self.save, p, width=142, primary=True).pack(side='right')
        Button(buttons, self.t('preview'), self.preview, p, width=120).pack(side='right', padx=16)
        Button(buttons, self.t('launch'), lambda: self.preview(full=True), p, width=145).pack(side='right')
        status = tk.Frame(footer, bg=p['bg'])
        status.pack(fill='x', pady=(12, 0))
        self.status = self.label(status, self.t('engine'), 11, muted=True)
        self.status.pack(side='left')
        self.label(status, 'PYTHON EDITION  /  2.0', 10, muted=True).pack(side='right')

        viewport_frame = tk.Frame(self.root, bg=p['bg'])
        viewport_frame.pack(fill='both', expand=True, padx=32)
        viewport = tk.Canvas(viewport_frame, bg=p['bg'], highlightthickness=0)
        vertical = ttk.Scrollbar(viewport_frame, orient='vertical', command=viewport.yview)
        viewport.configure(yscrollcommand=vertical.set)
        vertical.pack(side='right', fill='y')
        viewport.pack(side='left', fill='both', expand=True)
        body = tk.Frame(viewport, bg=p['bg'])
        body_window = viewport.create_window(0, 0, window=body, anchor='nw')
        def fit_body(event):
            if event.height < 576:
                vertical.pack(side='right', fill='y', before=viewport)
            else:
                vertical.pack_forget()
            viewport.itemconfigure(body_window, width=event.width, height=max(576, event.height))
            viewport.configure(scrollregion=viewport.bbox('all'))
        viewport.bind('<Configure>', fit_body)
        def wheel(event):
            if event.widget is not self.url_text:
                viewport.yview_scroll(int(-event.delta / 120), 'units')
        self.root.bind_all('<MouseWheel>', wheel)
        body.columnconfigure(1, weight=1)
        body.rowconfigure(0, weight=1)
        sidebar = tk.Frame(body, bg=p['bg'], width=264)
        sidebar.grid(row=0, column=0, sticky='nsew', padx=(0, 20))
        sidebar.grid_propagate(False)
        display_card = self.card(sidebar)
        display_card.pack(fill='x')
        self.label(display_card, self.t('displays'), 11, muted=True, bold=True).pack(anchor='w')
        self.label(display_card, self.t('profile'), 18, bold=True).pack(anchor='w', pady=(8, 14))
        count = max(len(monitors()), len(self.settings.displays))
        values = [f'{self.t("screen")} {i + 1:02d}' for i in range(count)]
        self.screen_combo = self.combo(display_card, values, self.selected, self.switch_display, width=18)
        self.screen_combo.pack(fill='x')
        self.label(display_card, self.t('mode'), muted=True).pack(anchor='w', pady=(18, 7))
        self.mode_combo = self.combo(display_card, self.t('modes'), ['Separate', 'Mirror', 'Span'].index(self.settings.mode), width=18)
        self.mode_combo.pack(fill='x')
        self.label(display_card, self.t('note'), 12, muted=True, justify='left').pack(anchor='w', pady=(18, 0))
        release = tk.Frame(sidebar, bg=p['bg'])
        release.pack(fill='x', pady=15)
        Button(release, self.t('update'), self.check_release, p, width=264, height=36).pack()

        right = tk.Frame(body, bg=p['bg'])
        right.grid(row=0, column=1, sticky='nsew')
        editor = self.card(right)
        editor.pack(fill='both', expand=True)
        self.label(editor, self.t('playlist'), 19, bold=True).pack(anchor='w')
        self.label(editor, self.t('playlist_sub'), 12, muted=True).pack(anchor='w', pady=(4, 14))
        text_frame = tk.Frame(editor, bg=p['border'], padx=1, pady=1)
        text_frame.pack(fill='both', expand=True)
        scrollbar = ttk.Scrollbar(text_frame)
        scrollbar.pack(side='right', fill='y')
        self.url_text = tk.Text(text_frame, height=5, wrap='none', bg=p['inset'], fg=p['text'], insertbackground=p['accent'],
            selectbackground=p['accent'], selectforeground=p['on_accent'], font=('Consolas', -14), relief='flat', padx=14, pady=12, undo=True,
            yscrollcommand=scrollbar.set, highlightthickness=0)
        self.url_text.pack(fill='both', expand=True)
        scrollbar.configure(command=self.url_text.yview)
        profile = self.settings.displays[self.selected] if self.selected < len(self.settings.displays) else Display(urls=[])
        self.url_text.insert('1.0', '\n'.join(profile.urls))
        horizontal = ttk.Scrollbar(editor, orient='horizontal', command=self.url_text.xview)
        horizontal.pack(fill='x')
        self.url_text.configure(xscrollcommand=horizontal.set)
        self.label(editor, self.t('hint'), 11, muted=True, wraplength=610, justify='left').pack(anchor='w', pady=(10, 0))

        lower = tk.Frame(right, bg=p['bg'])
        lower.pack(fill='x', pady=(16, 0))
        lower.columnconfigure(0, weight=1)
        lower.columnconfigure(1, weight=1)
        playback = self.card(lower)
        playback.grid(row=0, column=0, sticky='nsew', padx=(0, 8))
        behavior = self.card(lower)
        behavior.grid(row=0, column=1, sticky='nsew', padx=(8, 0))
        self.label(playback, self.t('playback'), 16, bold=True).pack(anchor='w', pady=(0, 12))
        self.interval = tk.StringVar(value=str(profile.interval))
        self.zoom = tk.StringVar(value=str(profile.zoom))
        for key, variable in [('interval', self.interval), ('zoom', self.zoom)]:
            row = tk.Frame(playback, bg=p['card'])
            row.pack(fill='x', pady=(0, 12))
            self.label(row, self.t(key), 12).pack(side='left')
            tk.Entry(row, textvariable=variable, width=5, justify='center', bg=p['inset'], fg=p['text'], insertbackground=p['text'],
                relief='flat', font=('Segoe UI', -14), highlightbackground=p['border'], highlightthickness=1).pack(side='right', ipady=4)
        self.variables = {name: tk.BooleanVar(value=getattr(self.settings, name)) for name in ('mute', 'private', 'clock', 'close_on_activity')}
        self.randomize = tk.BooleanVar(value=profile.randomize)
        Toggle(playback, self.t('random'), self.randomize, p).pack(fill='x', pady=(0, 12))
        self.label(playback, self.t('position'), 12, muted=True).pack(anchor='w', pady=(0, 6))
        self.position_combo = self.combo(playback, self.t('positions'), ['BottomRight', 'BottomLeft', 'TopRight', 'TopLeft'].index(self.settings.clock_position), width=15)
        self.position_combo.pack(fill='x')
        self.label(behavior, self.t('behavior'), 16, bold=True).pack(anchor='w', pady=(0, 8))
        for attr, key in [('mute', 'mute'), ('private', 'private'), ('clock', 'clock'), ('close_on_activity', 'close')]:
            Toggle(behavior, self.t(key), self.variables[attr], p).pack(fill='x', pady=9)

    def collect(self):
        candidate = copy.deepcopy(self.settings)
        while self.selected >= len(candidate.displays):
            candidate.displays.append(Display(urls=[]))
        candidate.displays[self.selected] = Display([line.strip() for line in self.url_text.get('1.0', 'end').splitlines() if line.strip()],
                                                  int(self.interval.get()), self.randomize.get(), int(self.zoom.get()))
        candidate.mode = ['Separate', 'Mirror', 'Span'][self.mode_combo.current()]
        candidate.clock_position = ['BottomRight', 'BottomLeft', 'TopRight', 'TopLeft'][self.position_combo.current()]
        for attr, variable in self.variables.items():
            setattr(candidate, attr, variable.get())
        candidate.validate()
        self.settings = candidate

    def attempt(self, operation):
        try:
            self.collect()
            operation()
        except (ValueError, OSError, TypeError, json.JSONDecodeError) as error:
            messagebox.showerror(self.t('error'), str(error), parent=self.root)

    def switch_display(self, event=None):
        selected = self.screen_combo.current()
        def apply():
            self.selected = selected
            while selected >= len(self.settings.displays):
                self.settings.displays.append(Display(urls=[]))
            self.render()
        self.attempt(apply)
        if self.screen_combo.winfo_exists():
            self.screen_combo.current(self.selected)

    def switch_language(self):
        def apply():
            self.settings.language = 'en' if self.settings.language == 'ko' else 'ko'
            self.render()
        self.attempt(apply)

    def switch_theme(self, event=None):
        theme = ['system', 'dark', 'light'][self.theme_combo.current()]
        def apply():
            self.settings.theme = theme
            self.render()
        self.attempt(apply)

    def poll_theme(self):
        if not self.alive:
            return
        if self.settings.theme == 'system' and (self.p is DARK) != system_dark():
            try:
                self.collect()
                self.render()
            except ValueError:
                pass  # Keep invalid in-progress edits intact during system theme changes.
        for path, child in self.children[:]:
            if child.poll() is not None:
                path.unlink(missing_ok=True)
                self.children.remove((path, child))
        self.theme_timer = self.root.after(1500, self.poll_theme)

    def save(self):
        def apply():
            self.settings.save(self.path)
            self.original = copy.deepcopy(self.settings)
            self.status.configure(text=self.t('saved'))
        self.attempt(apply)

    def export_settings(self):
        def apply():
            name = filedialog.asksaveasfilename(parent=self.root, defaultextension='.json', initialfile='WebScreensaver_Backup.json', filetypes=[('JSON', '*.json')])
            if name:
                self.settings.save(Path(name))
        self.attempt(apply)

    def import_settings(self):
        name = filedialog.askopenfilename(parent=self.root, filetypes=[('JSON', '*.json')])
        if not name:
            return
        try:
            imported = load(Path(name))
            self.settings, self.selected = imported, 0
            self.render()
            self.status.configure(text=self.t('imported'))
        except (ValueError, TypeError, OSError) as error:
            messagebox.showerror(self.t('error'), str(error), parent=self.root)

    def reset(self):
        if messagebox.askyesno('Web Screensaver', self.t('reset_confirm'), parent=self.root):
            self.settings = Settings(language=self.settings.language, theme=self.settings.theme)
            self.selected = 0
            self.render()

    def preview(self, full=False):
        def apply():
            descriptor, name = tempfile.mkstemp(prefix='webscreensaver-preview-', suffix='.json')
            os.close(descriptor)
            path = Path(name)
            self.settings.save(path)
            try:
                child = subprocess.Popen(command('/s' if full else '--preview', '--config', str(path)), creationflags=subprocess.CREATE_NO_WINDOW)
                self.children.append((path, child))
            except OSError:
                path.unlink(missing_ok=True)
                raise
        self.attempt(apply)

    def check_release(self, automatic=False):
        if self.release_queue is not None:
            return
        import queue
        import threading
        import urllib.request
        import webbrowser
        self.release_queue = queue.Queue()
        if not automatic:
            self.status.configure(text=self.t('checking'))
        def fetch():
            try:
                request = urllib.request.Request('https://api.github.com/repos/muro-dot/Webview2_WebPage_Screensaver_for_Windows/releases/latest', headers={'User-Agent': 'WebScreensaver-Python'})
                with urllib.request.urlopen(request, timeout=5) as response:
                    release = json.load(response)
                self.release_queue.put(release.get('tag_name', '?'))
            except Exception:
                self.release_queue.put(None)
        def poll():
            if not self.alive:
                return
            try:
                version = self.release_queue.get_nowait()
            except queue.Empty:
                self.root.after(100, poll)
                return
            self.release_queue = None
            if not automatic:
                self.status.configure(text=f'{self.t("latest")}: {version}' if version else self.t('no_release'))
            elif version:
                import re
                numbers = tuple(int(part) for part in re.findall(r'\d+', version)[:3])
                if numbers > (2, 0, 0):
                    self.status.configure(text=f'{self.t("latest")}: {version}  ·  GitHub ↗')
            if not automatic and version and messagebox.askyesno('Web Screensaver', f'{self.t("latest")}: {version}\nGitHub ↗', parent=self.root):
                webbrowser.open('https://github.com/muro-dot/Webview2_WebPage_Screensaver_for_Windows/releases/latest')
        threading.Thread(target=fetch, daemon=True).start()
        self.root.after(100, poll)

    def close(self):
        if not self.capture:
            try:
                self.collect()
                changed = self.settings != self.original
            except ValueError:
                changed = True
            if changed and not messagebox.askyesno('Web Screensaver', self.t('unsaved'), parent=self.root):
                return
        self.alive = False
        self.root.after_cancel(self.theme_timer)
        if self.release_timer:
            self.root.after_cancel(self.release_timer)
        # Child processes keep their own config file until they exit; don't kill a running saver.
        self.root.destroy()

    def run(self):
        self.root.mainloop()
