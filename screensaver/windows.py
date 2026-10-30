"""Pointer-safe Win32 helpers shared by Tk and the browser process."""
import ctypes
from ctypes import wintypes


def dpi_aware():
    try:
        ctypes.windll.shcore.SetProcessDpiAwareness(2)
    except (OSError, AttributeError):
        ctypes.windll.user32.SetProcessDPIAware()


def monitors():
    rectangles = []
    callback_type = ctypes.WINFUNCTYPE(wintypes.BOOL, wintypes.HANDLE, wintypes.HDC, ctypes.POINTER(wintypes.RECT), wintypes.LPARAM)
    @callback_type
    def collect(handle, dc, rectangle, data):
        r = rectangle.contents
        rectangles.append((r.left, r.top, r.right - r.left, r.bottom - r.top))
        return True
    ctypes.windll.user32.EnumDisplayMonitors(None, None, collect, 0)
    return rectangles or [(0, 0, 1280, 720)]


def system_dark():
    import winreg
    try:
        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, r'Software\Microsoft\Windows\CurrentVersion\Themes\Personalize') as key:
            return winreg.QueryValueEx(key, 'AppsUseLightTheme')[0] == 0
    except OSError:
        return True


def activity():
    # LASTINPUTINFO covers input consumed by Chromium without installing global hooks.
    class LastInput(ctypes.Structure):
        _fields_ = [('cbSize', wintypes.UINT), ('dwTime', wintypes.DWORD)]
    info = LastInput()
    info.cbSize = ctypes.sizeof(info)
    if not ctypes.windll.user32.GetLastInputInfo(ctypes.byref(info)):
        raise ctypes.WinError()
    return info.dwTime


def parent_window(child: int, parent: int):
    user = ctypes.windll.user32
    user.SetParent.argtypes = [wintypes.HWND, wintypes.HWND]
    user.SetParent.restype = wintypes.HWND
    user.GetWindowLongW.argtypes = [wintypes.HWND, ctypes.c_int]
    user.SetWindowLongW.argtypes = [wintypes.HWND, ctypes.c_int, ctypes.c_long]
    user.SetWindowPos.argtypes = [wintypes.HWND, wintypes.HWND, ctypes.c_int, ctypes.c_int, ctypes.c_int, ctypes.c_int, wintypes.UINT]
    user.SetParent(child, parent)
    style = user.GetWindowLongW(child, -16)
    user.SetWindowLongW(child, -16, (style | 0x40000000) & ~0x80000000)
    rectangle = wintypes.RECT()
    user.GetClientRect.argtypes = [wintypes.HWND, ctypes.POINTER(wintypes.RECT)]
    user.GetClientRect(parent, ctypes.byref(rectangle))
    user.SetWindowPos(child, None, 0, 0, rectangle.right, rectangle.bottom, 0x24)
