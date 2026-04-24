using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

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

                // [Core addition] Register a message filter to detect keyboard/mouse inputs globally across the application
                Application.AddMessageFilter(new ScreensaverInputFilter());

                Application.Run(new MultiFormContext(formsList));
            }
        }
    }

    // [Added class] Intercept Windows input messages
    public class ScreensaverInputFilter : IMessageFilter
    {
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_MBUTTONDOWN = 0x0207;

        private Point _originalMouseLocation = new Point(int.MaxValue, int.MaxValue);

        public bool PreFilterMessage(ref Message m)
        {
            // Exit unconditionally upon keyboard press or mouse click
            if (m.Msg == WM_KEYDOWN || m.Msg == WM_LBUTTONDOWN || m.Msg == WM_RBUTTONDOWN || m.Msg == WM_MBUTTONDOWN)
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