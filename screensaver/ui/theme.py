"""OnSet-inspired palettes and antialiased, keyboard-accessible Canvas controls."""
import tkinter as tk
from PIL import Image, ImageDraw, ImageTk

DARK = dict(bg='#11131a', card='#1b1e28', inset='#141821', border='#303648', text='#f2f4f8', muted='#a3aec3', accent='#ffc046', on_accent='#292008', hover='#292f40', blue='#80b4ff')
LIGHT = dict(bg='#f2f4f9', card='#ffffff', inset='#f5f7fb', border='#dce2ed', text='#171e30', muted='#58667d', accent='#2563eb', on_accent='#ffffff', hover='#e7edf8', blue='#2563eb')


def rounded(master, width, height, radius, fill, border=None, hard_edge=False):
    scale = 3
    image = Image.new('RGBA', (width * scale, height * scale))
    ImageDraw.Draw(image).rounded_rectangle((1, 1, width * scale - 2, height * scale - 2), radius * scale,
                                           fill=fill, outline=border, width=scale if border else 1)
    image = image.resize((width, height), Image.Resampling.LANCZOS)
    if hard_edge:
        # Color-key Windows popups cannot blend fractional alpha at their edge;
        # threshold only the outer silhouette to prevent a colored fringe.
        image.putalpha(image.getchannel('A').point(lambda alpha: 255 if alpha >= 128 else 0))
    return ImageTk.PhotoImage(image, master=master)


class Button(tk.Canvas):
    def __init__(self, master, text, command, palette, width=140, primary=False, height=48):
        self.scale = getattr(master.winfo_toplevel(), 'ui_scale', 1.0)
        width, height = round(width * self.scale), round(height * self.scale)
        super().__init__(master, width=width, height=height, bg=master.cget('bg'), highlightthickness=0, bd=0, takefocus=1, cursor='hand2')
        self.label, self.command, self.palette = text, command, palette
        self.primary, self.w, self.h = primary, width, height
        self.bind('<Button-1>', lambda e: self.invoke())
        self.bind('<Return>', lambda e: self.invoke())
        self.bind('<space>', lambda e: self.invoke())
        for event in ('<Enter>', '<FocusIn>'):
            self.bind(event, lambda e: self.draw(True))
        for event in ('<Leave>', '<FocusOut>'):
            self.bind(event, lambda e: self.draw(False))
        self.draw()

    def invoke(self):
        self.focus_set()
        self.command()

    def draw(self, hover=False):
        p = self.palette
        self.delete('all')
        self.image = rounded(self, self.w, self.h, round(12 * self.scale), p['accent'] if self.primary else p['hover'] if hover else p['card'], p['accent'] if hover else p['border'])
        self.create_image(0, 0, image=self.image, anchor='nw')
        self.create_text(self.w / 2, self.h / 2, text=self.label, fill=p['on_accent'] if self.primary else p['text'], font=('Malgun Gothic', -round(16 * self.scale), 'bold'))


class Toggle(tk.Frame):
    def __init__(self, master, label, variable, palette):
        super().__init__(master, bg=palette['card'])
        self.variable, self.palette = variable, palette
        self.scale = getattr(master.winfo_toplevel(), 'ui_scale', 1.0)
        self.s = lambda value: round(value * self.scale)
        tk.Label(self, text=label, bg=palette['card'], fg=palette['text'], font=('Malgun Gothic', -self.s(16))).pack(side='left')
        self.canvas = tk.Canvas(self, width=self.s(48), height=self.s(28), highlightthickness=0, bg=palette['card'], takefocus=1, cursor='hand2')
        self.canvas.pack(side='right', padx=(self.s(16), 0))
        self.canvas.bind('<Button-1>', lambda e: self.toggle())
        self.canvas.bind('<space>', lambda e: self.toggle())
        self.canvas.bind('<Return>', lambda e: self.toggle())
        self.canvas.bind('<FocusIn>', lambda e: self.draw(True))
        self.canvas.bind('<FocusOut>', lambda e: self.draw())
        self.draw()

    def toggle(self):
        self.canvas.focus_set()
        self.variable.set(not self.variable.get())
        self.draw()

    def draw(self, focused=False):
        c, p = self.canvas, self.palette
        c.delete('all')
        self.image = rounded(c, self.s(48), self.s(28), self.s(14), p['accent'] if self.variable.get() else p['border'], p['blue'] if focused else None)
        c.create_image(0, 0, image=self.image, anchor='nw')
        x = 34 if self.variable.get() else 14
        c.create_oval(self.s(x - 9), self.s(5), self.s(x + 9), self.s(23), fill=p['on_accent'] if self.variable.get() else p['muted'], outline='')
