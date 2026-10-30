"""WebView2 playback runs in its own process so Tk never shares a GUI event loop."""
from __future__ import annotations

import json
import logging
import random
import shutil
import tempfile
import threading
import time
from pathlib import Path

from .clock import clock_html, overlay_script
from .config import Display, Settings, parse_url
from .windows import activity, cover_monitor, monitors, parent_window, window_bounds


class Playback:
    def __init__(self, settings: Settings, preview=False, parent=None, seconds=0, report=None):
        self.settings, self.preview, self.parent = settings, preview, parent
        self.seconds, self.report = seconds, report
        self.windows, self.states = [], []
        self.closed = threading.Event()
        self.errors = []
        self.start_time = time.monotonic()

    def close(self):
        if self.closed.is_set():
            return
        self.closed.set()
        for window in self.windows[:]:
            try:
                window.destroy()
            except Exception:
                logging.exception('Closing browser window')

    def initialize_native(self, window, display, bounds=None):
        from System import Action
        from System.Drawing import Size
        from System.Windows.Forms import AutoScaleMode, Button, Cursor, Timer, FormWindowState
        from Microsoft.Web.WebView2.Core import CoreWebView2PermissionState
        form = window.native
        control = form.webview
        state = {'ready': False, 'fallback': False, 'index': -1, 'next': float('inf'), 'retry': float('inf'), 'crashes': 0, 'last_url': '', 'history': [], 'loaded': 0}
        self.states.append(state)
        if bounds is not None and not self.preview and not self.parent:
            handle = int(form.Handle.ToInt64())
            state['initial_bounds'] = window_bounds(handle)
            state['target_bounds'] = bounds
            # pywebview scales constructor dimensions from logical pixels. The
            # monitor rectangles are already physical, so size the HWND directly.
            form.AutoScaleMode = getattr(AutoScaleMode, 'None')
            form.MinimumSize = Size(0, 0)
            form.MaximumSize = Size(0, 0)
            form.WindowState = FormWindowState.Normal
            def fit_screen(sender=None, args=None):
                cover_monitor(handle, bounds)
                state['actual_bounds'] = window_bounds(handle)
            fit_screen()
            form.Shown += fit_screen
        urls = list(display.urls)
        if display.randomize:
            random.Random(0 if self.settings.mode == 'Mirror' else None).shuffle(urls)

        def fallback():
            state['fallback'] = True
            state['retry'] = time.monotonic() + 30
            control.CoreWebView2.NavigateToString(clock_html(self.settings.language))

        def navigate():
            if not urls:
                fallback()
                return
            state['index'] = (state['index'] + 1) % len(urls)
            address, duration = parse_url(urls[state['index']])
            state.update(last_url=address, fallback=False, retry=float('inf'))
            state['history'].append(address)
            state['next'] = time.monotonic() + (duration or display.interval) if len(urls) > 1 else float('inf')
            control.CoreWebView2.Navigate(address)

        def completed(sender, args):
            if state['fallback'] or not state['last_url']:
                return
            if not args.IsSuccess:
                if str(args.WebErrorStatus) == 'OperationCanceled':
                    return
                self.errors.append(str(args.WebErrorStatus))
                fallback()
            else:
                state['loaded'] += 1
                state['crashes'] = 0
                state['retry'] = float('inf')
                control.ZoomFactor = display.zoom / 100
                state['actual'] = {'mute': bool(control.CoreWebView2.IsMuted), 'zoom': float(control.ZoomFactor),
                    'context_menus': bool(control.CoreWebView2.Settings.AreDefaultContextMenusEnabled),
                    'devtools': bool(control.CoreWebView2.Settings.AreDevToolsEnabled)}
                if self.settings.clock:
                    control.CoreWebView2.ExecuteScriptAsync(overlay_script(self.settings.clock_position))

        def failed(sender, args):
            state['crashes'] += 1
            self.errors.append(str(args.ProcessFailedKind))
            # A dead browser process cannot be recovered by Navigate; close cleanly
            # rather than keeping a permanently invalid COM controller alive.
            if 'BrowserProcessExited' in str(args.ProcessFailedKind):
                self.close()
            elif state['crashes'] <= 3:
                control.CoreWebView2.Reload()
            else:
                fallback()

        def popup(sender, args):
            args.Handled = True

        def permission(sender, args):
            args.State = CoreWebView2PermissionState.Deny

        def download(sender, args):
            args.Cancel = True

        def dialog(sender, args):
            args.Accept()

        def ready(sender=None, args=None):
            if state['ready']:
                return
            core = control.CoreWebView2
            if core is None:
                return
            state['ready'] = True
            core.IsMuted = self.settings.mute
            core.Settings.AreDefaultContextMenusEnabled = False
            core.Settings.AreDevToolsEnabled = False
            core.Settings.AreBrowserAcceleratorKeysEnabled = False
            core.Settings.AreDefaultScriptDialogsEnabled = False
            core.NewWindowRequested += popup
            core.PermissionRequested += permission
            core.DownloadStarting += download
            core.ScriptDialogOpening += dialog
            core.NavigationCompleted += completed
            core.ProcessFailed += failed
            # Let pywebview finish its initial NavigateToString before the first URL.
            # Otherwise cancelling that initial navigation can trigger a false fallback.
            state['next'] = time.monotonic() + .5

        control.CoreWebView2InitializationCompleted += ready
        ready()
        close_button = Button()
        close_button.Text = '닫기 / Esc' if self.settings.language == 'ko' else 'Close / Esc'
        close_button.Width, close_button.Height = 130, 40
        close_button.Left, close_button.Top = 20, 20
        close_button.Visible = self.preview or not self.settings.close_on_activity
        close_button.Click += lambda sender, args: self.close()
        form.Controls.Add(close_button)
        if close_button.Visible:
            close_button.BringToFront()
        elif not self.parent:
            Cursor.Hide()
            window.events.closed += lambda: Cursor.Show()
        if self.parent:
            parent_window(int(form.Handle.ToInt64()), self.parent)

        self.last_activity = activity()
        timer = Timer()
        timer.Interval = 100

        def tick(sender, args):
            now = time.monotonic()
            if 'target_bounds' in state:
                actual_bounds = window_bounds(handle)
                if actual_bounds != bounds:
                    fit_screen()
                else:
                    state['actual_bounds'] = actual_bounds
                state['client_size'] = [int(form.ClientSize.Width), int(form.ClientSize.Height)]
            if self.seconds and now - self.start_time >= self.seconds:
                self.close()
                return
            current = activity()
            # Each window tracks input separately; the grace period absorbs launch clicks.
            if now - self.start_time < 1.5:
                self.last_activity = current
            elif current != self.last_activity and not self.parent:
                self.last_activity = current
                if not self.preview and self.settings.close_on_activity:
                    self.close()
                    return
                import ctypes
                if ctypes.windll.user32.GetAsyncKeyState(0x1B) & 0x8000:
                    self.close()
                    return
            if not state['ready']:
                return
            try:
                if now >= state['next']:
                    navigate()
                elif now >= state['retry'] and state['last_url']:
                    state.update(fallback=False, retry=float('inf'))
                    control.CoreWebView2.Navigate(state['last_url'])
            except Exception:
                logging.exception('Browser playback')
                self.close()

        timer.Tick += tick
        timer.Start()
        # Keep managed timers/delegates alive until the native Form is disposed.
        state['timer'] = timer
        window.events.closed += lambda: timer.Stop()

    def run(self):
        import webview
        webview.settings['ALLOW_DOWNLOADS'] = False
        webview.settings['OPEN_EXTERNAL_LINKS_IN_BROWSER'] = False
        webview.settings['OPEN_DEVTOOLS_IN_DEBUG'] = False
        rectangles = monitors()
        if self.settings.mode == 'Span':
            left, top = min(r[0] for r in rectangles), min(r[1] for r in rectangles)
            right, bottom = max(r[0] + r[2] for r in rectangles), max(r[1] + r[3] for r in rectangles)
            rectangles = [(left, top, right - left, bottom - top)]
        if self.preview or self.parent:
            rectangles = [(100, 100, 960, 600)]
        for index, rectangle in enumerate(rectangles):
            profile_index = 0 if self.settings.mode in ('Mirror', 'Span') else index
            profile = self.settings.displays[profile_index] if profile_index < len(self.settings.displays) else Display(urls=[])
            window = webview.create_window('Web Screensaver — Preview' if self.preview else 'Web Screensaver',
                html=clock_html(self.settings.language), x=rectangle[0], y=rectangle[1], width=rectangle[2], height=rectangle[3],
                frameless=not self.preview, on_top=not self.preview, resizable=self.preview,
                background_color='#0c101a', text_select=False, easy_drag=False, shadow=False)
            window.events.before_show += lambda w=window, p=profile, r=rectangle: self.initialize_native(w, p, r)
            window.events.closed += self.close
            self.windows.append(window)
        temporary = tempfile.mkdtemp(prefix='webscreensaver-private-') if self.settings.private else None
        from .config import CONFIG_PATH
        storage = temporary or str(CONFIG_PATH.parent / 'browser')
        try:
            webview.start(gui='edgechromium', private_mode=self.settings.private, storage_path=storage, debug=False)
        finally:
            if temporary:
                for attempt in range(10):
                    try:
                        shutil.rmtree(temporary)
                        break
                    except FileNotFoundError:
                        break  # pywebview already removed the private profile on close.
                    except OSError:
                        time.sleep(.2)
                else:
                    logging.warning('Temporary browser directory is still locked: %s', temporary)
            if self.report:
                Path(self.report).write_text(json.dumps({'windows': len(self.windows), 'private_profile_removed': not Path(temporary).exists() if temporary else None, 'ready': sum(s['ready'] for s in self.states),
                    'errors': self.errors, 'profiles': [{'url': s['last_url'], 'fallback': s['fallback'], 'loaded': s['loaded'], 'history': s['history'], 'actual': s.get('actual'),
                        'initial_bounds': s.get('initial_bounds'), 'target_bounds': s.get('target_bounds'), 'actual_bounds': s.get('actual_bounds'), 'client_size': s.get('client_size')} for s in self.states]}, indent=2), encoding='utf-8')


def preview_thumbnail(parent, language='ko', seconds=0):
    """The Windows thumbnail is local-only, avoiding a full browser in a tiny panel."""
    import tkinter as tk
    import datetime
    root = tk.Tk()
    root.overrideredirect(True)
    root.configure(bg='#101621')
    root.geometry('320x200')
    root.update_idletasks()
    parent_window(root.winfo_id(), parent)
    label = tk.Label(root, bg='#101621', fg='#edf2fc', font=('Segoe UI', 24))
    label.pack(expand=True)
    def tick():
        label.configure(text=datetime.datetime.now().strftime('%H:%M:%S'))
        root.after(1000, tick)
    tick()
    if seconds:
        root.after(int(seconds * 1000), root.destroy)
    root.mainloop()
