using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

namespace Web_Page_Screensaver
{
    public partial class ScreensaverForm : Form
    {
        private DateTime StartTime;
        private Timer timer;
        private Timer mouseMonitorTimer;
        private Point initialMousePos;
        private int currentSiteIndex = -1;
        private bool shuffleOrder;
        private List<string> urls;

        private PreferencesManager prefsManager = new PreferencesManager();
        private int screenNum;

        private WebView2 webView;

        [ThreadStatic]
        private static Random random;

        public ScreensaverForm(int? screenNumber = null)
        {
            // [해결 1] 폼이 생성되자마자 시작 시간을 기록하여 마우스 타이머의 오작동(조기 종료) 방지
            StartTime = DateTime.Now;

            if (screenNumber == null) screenNum = prefsManager.EffectiveScreensList.FindIndex(s => s.IsPrimary);
            else screenNum = (int)screenNumber;

            InitializeComponent();
            InitializeWebViewAsync();

            Cursor.Hide();

            initialMousePos = Cursor.Position;
            mouseMonitorTimer = new Timer();
            mouseMonitorTimer.Interval = 50;
            mouseMonitorTimer.Tick += MouseMonitorTimer_Tick;
            mouseMonitorTimer.Start();
        }

        private void MouseMonitorTimer_Tick(object sender, EventArgs e)
        {
            // 프로그램 실행 후 1.5초 동안은 마우스가 흔들려도 종료되지 않도록 유예기간 부여
            if (StartTime.AddSeconds(1.5) > DateTime.Now)
            {
                initialMousePos = Cursor.Position;
                return;
            }

            Point currentPos = Cursor.Position;

            // X나 Y 좌표가 3픽셀 이상 움직였을 때만 반응
            if (Math.Abs(initialMousePos.X - currentPos.X) > 3 ||
                Math.Abs(initialMousePos.Y - currentPos.Y) > 3)
            {
                mouseMonitorTimer.Stop();
                HandleUserActivity();
            }
        }

        private async void InitializeWebViewAsync()
        {
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            this.Controls.Add(webView);

            if (this.Controls.ContainsKey("closeButton"))
            {
                this.Controls["closeButton"].BringToFront();
            }

            // [해결 2] 권한 문제가 없는 안전한 폴더(LocalAppData)에 WebView2 임시 데이터를 저장
            string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LibraryScreensaver_Data");
            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);

            await webView.EnsureCoreWebView2Async(env);
// ---------------------------------------------------------
// [수정된 부분] CoreWebView2가 아닌 WebView2 컨트롤 자체의 이벤트를 사용합니다.

// 1. 일반적인 문자, 숫자 키 감지
webView.KeyDown += (sender, e) =>
{
    Application.Exit();
};

            // 2. 방향키, Tab, Esc 등 일반 KeyDown에서 놓칠 수 있는 특수키 감지 (안전장치)
            webView.PreviewKeyDown += (sender, e) =>
            {
                Application.Exit();
            };
            // ---------------------------------------------------------
            StartScreensaverLogic();
        }

        public List<string> Urls
        {
            get
            {
                if (urls == null)
                {
                    urls = prefsManager.GetUrlsByScreen(screenNum);
                }
                return urls;
            }
        }

        private void ScreensaverForm_Load(object sender, EventArgs e)
        {
            // StartTime은 생성자에서 처리하므로 여기는 비워둡니다.
        }

        private void StartScreensaverLogic()
        {
            if (Urls.Any())
            {
                if (Urls.Count > 1)
                {
                    shuffleOrder = prefsManager.GetRandomizeFlagByScreen(screenNum);
                    if (shuffleOrder)
                    {
                        random = new Random();
                        int n = urls.Count;
                        while (n > 1)
                        {
                            n--;
                            int k = random.Next(n + 1);
                            var value = urls[k];
                            urls[k] = urls[n];
                            urls[n] = value;
                        }
                    }

                    timer = new Timer();
                    timer.Interval = prefsManager.GetRotationIntervalByScreen(screenNum) * 1000;
                    timer.Tick += (s, ee) => RotateSite();
                    timer.Start();
                }

                RotateSite();
            }
            else
            {
                webView.Visible = false;
                if (this.Controls.ContainsKey("closeButton"))
                {
                    this.Controls["closeButton"].Visible = false;
                }
            }
        }

        private void BrowseTo(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                webView.Visible = false;
            }
            else
            {
                webView.Visible = true;
                try
                {
                    if (webView.CoreWebView2 != null)
                        webView.CoreWebView2.Navigate(url);
                }
                catch { }
            }
        }

        private void RotateSite()
        {
            currentSiteIndex++;
            if (currentSiteIndex >= Urls.Count) currentSiteIndex = 0;
            BrowseTo(Urls[currentSiteIndex]);
        }

        private void HandleUserActivity()
        {
            if (prefsManager.CloseOnActivity)
            {
                Close();
            }
            else
            {
                if (this.Controls.ContainsKey("closeButton"))
                {
                    this.Controls["closeButton"].Visible = true;
                }
                Cursor.Show();
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}