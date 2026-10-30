"""Rounded selection control with a themed popup and standard keyboard navigation."""
import tkinter as tk
from tkinter import font as tkfont

from .theme import rounded


class Select(tk.Canvas):
    def __init__(self, master, values, palette, width=220):
        self.scale = getattr(master.winfo_toplevel(), 'ui_scale', 1.0)
        self.s = lambda value: max(1, round(value * self.scale))
        super().__init__(master, width=self.s(width), height=self.s(46), bg=master.cget('bg'),
                         highlightthickness=0, borderwidth=0, takefocus=1, cursor='hand2')
        self.values, self.palette = list(values), palette
        self.owner_root = master.winfo_toplevel()
        self.index, self.active = 0, 0
        self.popup = None
        self.hover = False
        self.font = tkfont.Font(self, family='Malgun Gothic', size=-self.s(16))
        self.bind('<Configure>', lambda e: self.draw())
        self.bind('<Button-1>', lambda e: self.toggle_menu())
        self.bind('<space>', lambda e: self.open_menu())
        self.bind('<Return>', lambda e: self.open_menu())
        self.bind('<Alt-Down>', lambda e: self.open_menu())
        self.bind('<Down>', lambda e: self.step(1))
        self.bind('<Up>', lambda e: self.step(-1))
        self.bind('<Escape>', lambda e: self.close_menu())
        self.bind('<Enter>', lambda e: self.highlight(True))
        self.bind('<Leave>', lambda e: self.highlight(False))
        self.bind('<FocusIn>', lambda e: self.draw())
        self.bind('<FocusOut>', lambda e: self.draw())
        self.bind('<Destroy>', lambda e: self.close_menu(False) if e.widget is self else None)
        self.draw()

    def current(self, index=None):
        if index is not None:
            if not 0 <= index < len(self.values):
                raise IndexError(index)
            self.index = index
            self.draw()
        return self.index

    def get(self):
        return self.values[self.index]

    def highlight(self, active):
        self.hover = active
        self.draw()

    def draw(self):
        if not self.winfo_exists():
            return
        p = self.palette
        width = self.winfo_width() if self.winfo_width() > 1 else int(self.cget('width'))
        height = self.s(46)
        focused = self.focus_get() is self
        self.delete('all')
        self.background = rounded(self, width, height, self.s(12), p['hover'] if self.hover else p['inset'],
                                  p['accent'] if focused or self.popup else p['border'])
        self.create_image(0, 0, image=self.background, anchor='nw')
        self.create_text(self.s(16), height / 2, text=self.get(), font=self.font, fill=p['text'], anchor='w')
        x, y = width - self.s(22), height / 2
        direction = -1 if self.popup else 1
        self.create_line(x - self.s(4), y - direction * self.s(2), x, y + direction * self.s(2),
                         x + self.s(4), y - direction * self.s(2), fill=p['muted'], width=self.s(1.5), capstyle='round', joinstyle='round')

    def step(self, direction):
        self.commit(max(0, min(len(self.values) - 1, self.index + direction)))
        return 'break'

    def toggle_menu(self):
        self.close_menu() if self.popup else self.open_menu()

    def open_menu(self):
        if self.popup or not self.values:
            return 'break'
        self.focus_set()
        root = self.winfo_toplevel()
        # Only one selector owns a popup at a time, even when the previous
        # control belongs to a settings page that is about to be redrawn.
        previous = getattr(root, 'active_select', None)
        if previous is not None and previous is not self:
            previous.close_menu(False)
        root.active_select = self
        self.active = self.index
        self.popup = tk.Toplevel(root)
        popup = self.popup
        popup.overrideredirect(True)
        popup.transient(root)
        popup.attributes('-topmost', True)
        popup.configure(bg='#ff00ff')
        popup.attributes('-transparentcolor', '#ff00ff')
        width = max(self.winfo_width(), max(self.font.measure(value) for value in self.values) + self.s(76))
        row_height, padding = self.s(44), self.s(8)
        height = min(len(self.values), 8) * row_height + 2 * padding
        x, y = self.winfo_rootx(), self.winfo_rooty() + self.winfo_height() + self.s(6)
        from ..windows import monitors
        bounds = next((r for r in monitors() if r[0] <= x < r[0] + r[2] and r[1] <= self.winfo_rooty() < r[1] + r[3]), monitors()[0])
        x = max(bounds[0] + padding, min(x, bounds[0] + bounds[2] - width - padding))
        if y + height > bounds[1] + bounds[3] - padding:
            y = self.winfo_rooty() - height - self.s(6)
        popup.geometry(f'{width}x{height}{x:+d}{y:+d}')
        surface = tk.Canvas(popup, bg='#ff00ff', highlightthickness=0, borderwidth=0)
        surface.pack(fill='both', expand=True)
        self.popup_background = rounded(surface, width, height, self.s(14), self.palette['card'], self.palette['border'], hard_edge=True)
        surface.create_image(0, 0, image=self.popup_background, anchor='nw')
        self.menu = tk.Canvas(surface, bg=self.palette['card'], highlightthickness=0, borderwidth=0,
                              yscrollincrement=row_height, cursor='hand2', takefocus=1)
        self.menu.place(x=padding, y=padding, width=width - 2 * padding, height=height - 2 * padding)
        self.menu_width = width - 2 * padding
        self.menu_height = height - 2 * padding
        self.menu.configure(scrollregion=(0, 0, self.menu_width, len(self.values) * row_height))
        self.menu.bind('<Motion>', self.pointer)
        self.menu.bind('<Button-1>', self.clicked)
        popup.bind('<Button-1>', self.outside_click)
        popup.bind('<Down>', lambda e: self.move_active(1))
        popup.bind('<Up>', lambda e: self.move_active(-1))
        popup.bind('<Home>', lambda e: self.set_active(0))
        popup.bind('<End>', lambda e: self.set_active(len(self.values) - 1))
        popup.bind('<Return>', lambda e: self.commit(self.active))
        popup.bind('<space>', lambda e: self.commit(self.active))
        popup.bind('<Escape>', lambda e: self.close_menu())
        popup.bind('<Tab>', lambda e: self.tab_out(False))
        popup.bind('<Shift-Tab>', lambda e: self.tab_out(True))
        popup.bind('<MouseWheel>', self.wheel)
        popup.bind('<FocusOut>', lambda e: popup.after_idle(self.dismiss_unfocused) if self.popup else None)
        self.draw_menu()
        popup.update_idletasks()
        popup.deiconify()
        popup.lift()
        popup.grab_set()
        self.menu.focus_force()
        self.draw()
        return 'break'

    def draw_menu(self):
        if not self.popup:
            return
        canvas, p, row_height = self.menu, self.palette, self.s(44)
        canvas.delete('all')
        self.row_images = []
        for index, value in enumerate(self.values):
            top = index * row_height
            if index == self.active:
                image = rounded(canvas, self.menu_width, row_height, self.s(8), p['hover'])
                self.row_images.append(image)
                canvas.create_image(0, top, image=image, anchor='nw')
            canvas.create_text(self.s(12), top + row_height / 2, text=value, anchor='w', font=self.font,
                               fill=p['accent'] if index == self.index else p['text'])
            if index == self.index:
                x, y = self.menu_width - self.s(23), top + row_height / 2
                canvas.create_line(x - self.s(5), y, x - self.s(1), y + self.s(4), x + self.s(6), y - self.s(5),
                                   fill=p['accent'], width=self.s(2), capstyle='round', joinstyle='round')

    def set_active(self, index):
        self.active = index
        self.draw_menu()
        # Keep keyboard navigation visible for setups with many display profiles.
        top = self.menu.canvasy(0)
        start = index * self.s(44)
        if start < top:
            self.menu.yview_moveto(start / (len(self.values) * self.s(44)))
        elif start + self.s(44) > top + self.menu_height:
            self.menu.yview_moveto((start + self.s(44) - self.menu_height) / (len(self.values) * self.s(44)))
        return 'break'

    def move_active(self, direction):
        return self.set_active(max(0, min(len(self.values) - 1, self.active + direction)))

    def pointer(self, event):
        index = int(self.menu.canvasy(event.y) // self.s(44))
        if 0 <= index < len(self.values) and index != self.active:
            self.active = index
            self.draw_menu()

    def clicked(self, event):
        index = int(self.menu.canvasy(event.y) // self.s(44))
        if 0 <= index < len(self.values):
            return self.commit(index)

    def wheel(self, event):
        self.menu.yview_scroll(-1 if event.delta > 0 else 1, 'units')
        return 'break'

    def outside_click(self, event):
        if self.popup and not (self.popup.winfo_rootx() <= event.x_root < self.popup.winfo_rootx() + self.popup.winfo_width()
                               and self.popup.winfo_rooty() <= event.y_root < self.popup.winfo_rooty() + self.popup.winfo_height()):
            self.close_menu()
        return 'break'

    def dismiss_unfocused(self):
        if self.popup and self.popup.focus_get() is not self.menu:
            self.close_menu(False)

    def tab_out(self, backwards):
        target = self.tk_focusPrev() if backwards else self.tk_focusNext()
        self.close_menu(False)
        target.focus_set()
        return 'break'

    def commit(self, index):
        changed = self.index != index
        self.close_menu()
        self.current(index)
        if changed:
            self.event_generate('<<ComboboxSelected>>')
        return 'break'

    def close_menu(self, restore_focus=True):
        if self.popup:
            popup, self.popup = self.popup, None
            if getattr(self.owner_root, 'active_select', None) is self:
                self.owner_root.active_select = None
            popup.grab_release()
            popup.destroy()
            if restore_focus and self.winfo_exists():
                self.focus_set()
                self.draw()
        return 'break'
