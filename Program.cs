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
            // 1. 고해상도 스케일링 무시 (1:1 매칭)
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 2. 화면보호기 미리보기 모드 (/p)
            if (args.Length > 0 && args[0].ToLower().Contains("/p"))
            {
                return;
            }

            // 3. 화면보호기 설정 모드 (/c)
            if (args.Length > 0 && args[0].ToLower().Contains("/c"))
            {
                Application.Run(new PreferencesForm());
            }
            // 4. 화면보호기 실행 모드 (/s 또는 인자 없음)
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

                // [핵심 추가] 애플리케이션 전역에서 키보드/마우스 입력을 감지하는 필터 등록
                Application.AddMessageFilter(new ScreensaverInputFilter());

                Application.Run(new MultiFormContext(formsList));
            }
        }
    }

    // [추가된 클래스] 윈도우 입력 메시지 가로채기
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
            // 키보드 키를 누르거나 마우스 클릭을 한 경우 무조건 종료
            if (m.Msg == WM_KEYDOWN || m.Msg == WM_LBUTTONDOWN || m.Msg == WM_RBUTTONDOWN || m.Msg == WM_MBUTTONDOWN)
            {
                Application.Exit();
                return true;
            }

            // 마우스를 움직인 경우
            if (m.Msg == WM_MOUSEMOVE)
            {
                if (_originalMouseLocation.X == int.MaxValue)
                {
                    // 최초 마우스 위치 저장
                    _originalMouseLocation = Cursor.Position;
                }
                else
                {
                    // 책상 진동 등으로 인한 미세한 마우스 움직임은 무시 (오차 범위 10픽셀)
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