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

            // 3. 테마 미리보기 스크린샷 자동 생성 모드 (/capture-assets 또는 /screenshot)
            // 빌드 시 실행되어 assets 폴더에 항상 예시 URL(https://example.com/screensaver)이 적용된 최신 스크린샷을 생성합니다.
            if (args.Length > 0 && (args[0].ToLower().Contains("/capture-assets") || args[0].ToLower().Contains("/screenshot")))
            {
                string outputDir = args.Length > 1 ? args[1] : null;
                AssetScreenshotGenerator.GenerateAssets(outputDir);
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
                // 이전 비정상 종료 등으로 남은 오래된 InPrivate 임시 디렉토리를 백그라운드에서 정리
                InPrivateSessionManager.CleanupOrphanedSessions();

                // 애플리케이션 종료 시 현재 세션의 InPrivate 임시 데이터 정리
                Application.ApplicationExit += (s, e) =>
                {
                    InPrivateSessionManager.CleanupCurrentSession();
                };

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
            // 1.5초 유예시간 이내에는 탐색기 우클릭/더블클릭 등의 잔여 클릭으로 인한 조기 종료를 철저히 방지
            if (m.Msg == WM_LBUTTONDOWN || m.Msg == WM_RBUTTONDOWN || m.Msg == WM_MBUTTONDOWN)
            {
                if (!GracePeriodManager.IsInGracePeriod())
                {
                    Application.Exit();
                    return true;
                }
                return false;
            }

            // 마우스 이동 감지
            if (m.Msg == WM_MOUSEMOVE)
            {
                if (_originalMouseLocation.X == int.MaxValue)
                {
                    // 마우스 최초 위치 기록
                    _originalMouseLocation = Cursor.Position;
                }
                else
                {
                    // 유예시간 중에는 마우스 위치 기준점을 계속 갱신하여 유예시간 직후 오동작 방지
                    if (GracePeriodManager.IsInGracePeriod())
                    {
                        _originalMouseLocation = Cursor.Position;
                        return false;
                    }

                    // 책상 진동 등을 감안한 10픽셀 이상 유의미한 이동 감지 시에만 종료
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
            if (nCode >= 0)
            {
                int msg = wParam.ToInt32();
                // 키를 누르는 이벤트(WM_KEYDOWN 또는 WM_SYSKEYDOWN)에만 반응 (키를 뗄 때의 WM_KEYUP 무시)
                if (msg == WM_KEYDOWN || msg == WM_SYSKEYDOWN)
                {
                    // 전역 유예시간(1.5초)이 지난 후에만 키 입력 종료 이벤트 발생
                    if (!GracePeriodManager.IsInGracePeriod())
                    {
                        var handler = KeyPressed;
                        if (handler != null)
                        {
                            handler(this, EventArgs.Empty);
                        }
                    }
                }
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