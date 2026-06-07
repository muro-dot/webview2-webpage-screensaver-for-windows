using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics; // Added to get the process module for the global hook

namespace Web_Page_Screensaver
{
    static class Program
    {
        public static readonly string KEY = "Software\\Web-Page-Screensaver";

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main(string[] args)
        {
            // 1. Ignore high DPI scaling (1:1 mapping)
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 2. Screensaver preview mode (/p)
            if (args.Length > 0 && args[0].ToLower().Contains("/p"))
            {
                return;
            }

            // 3. Screensaver configuration mode (/c)
            if (args.Length > 0 && args[0].ToLower().Contains("/c"))
            {
                Application.Run(new PreferencesForm());
            }
            // 4. Screensaver execution mode (/s or no arguments)
            else
            {
                var formsList = new List<Form>();
                var screens = (new PreferencesManager()).EffectiveScreensList;

                foreach (var screen in screens)
                {
                    var screensaverForm = new ScreensaverForm(screen.ScreenNum)
                    {
                        Location = new Point(screen.Bounds.Left, screen.Bounds.Top),
                        Size = new Size(screen.Bounds.Width, screen.Bounds.Height)
                    };

                    formsList.Add(screensaverForm);
                }

                // [Mouse Detection] Register application-wide message filter (detects mouse clicks and movements)
                Application.AddMessageFilter(new ScreensaverInputFilter());

                // [Keyboard Detection] Register global keyboard hook (prevents WebView2 from intercepting keys)
                using (var keyboardHook = new GlobalKeyboardHook())
                {
                    keyboardHook.KeyPressed += (s, e) =>
                    {
                        Application.Exit();
                    };

                    // Run screensaver forms
                    Application.Run(new MultiFormContext(formsList));
                } // The hook is automatically released when exiting the using block (Dispose is called)
            }
        }
    }

    // [Modified Class] Intercept mouse input messages only (keyboard is handled by the global hook)
    public class ScreensaverInputFilter : IMessageFilter
    {
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_MBUTTONDOWN = 0x0207;

        private Point _originalMouseLocation = new Point(int.MaxValue, int.MaxValue);

        public bool PreFilterMessage(ref Message m)
        {
            // Exit unconditionally upon mouse click
            if (m.Msg == WM_LBUTTONDOWN || m.Msg == WM_RBUTTONDOWN || m.Msg == WM_MBUTTONDOWN)
            {
                Application.Exit();
                return true;
            }

            // If the mouse is moved
            if (m.Msg == WM_MOUSEMOVE)
            {
                if (_originalMouseLocation.X == int.MaxValue)
                {
                    // Save the initial mouse location
                    _originalMouseLocation = Cursor.Position;
                }
                else
                {
                    // Ignore slight mouse movements due to desk vibrations, etc. (Tolerance margin of 10 pixels)
                    if (Math.Abs(Cursor.Position.X - _originalMouseLocation.X) > 10 ||
                        Math.Abs(Cursor.Position.Y - _originalMouseLocation.Y) > 10)
                    {
                        Application.Exit();
                        return true;
                    }
                }
            }

            return false;
        }
    }

    // [Added Class] Global hook to intercept keyboard inputs at the OS level
    public class GlobalKeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public event EventHandler KeyPressed;

        public GlobalKeyboardHook()
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // When a keyboard press event is detected (Standard or System key)
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                KeyPressed?.Invoke(this, EventArgs.Empty);
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            // The hook must be released when the program exits
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }

        // --- Win32 API Imports ---
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }

    public class MultiFormContext : ApplicationContext
    {
        public MultiFormContext(List<Form> forms)
        {
            foreach (var form in forms)
            {
                form.FormClosed += (s, args) =>
                {
                    ExitThread();
                };

                form.Show();
            }
        }
    }
}