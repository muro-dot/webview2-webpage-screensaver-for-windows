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
            // [Fix 1] Record the start time as soon as the form is created to prevent mouse timer malfunction (premature termination)
            StartTime = DateTime.Now;

            if (screenNumber == null) screenNum = prefsManager.EffectiveScreensList.FindIndex(s => s.IsPrimary);
            else screenNum = (int)screenNumber;

            InitializeComponent();
            try
            {
                Icon = ModernAppIcon.CreateAppIcon(32);
            }
            catch { }
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
            // Grant a grace period of 1.5 seconds after program execution so it doesn't terminate even if the mouse jitters
            if (StartTime.AddSeconds(1.5) > DateTime.Now)
            {
                initialMousePos = Cursor.Position;
                return;
            }

            Point currentPos = Cursor.Position;

            // React only when the X or Y coordinates move by more than 3 pixels
            if (Math.Abs(initialMousePos.X - currentPos.X) > 3 ||
                Math.Abs(initialMousePos.Y - currentPos.Y) > 3)
            {
                mouseMonitorTimer.Stop();
                HandleUserActivity();
            }
        }

        private string currentLoadedUrl = string.Empty;

        private async void InitializeWebViewAsync()
        {
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            this.Controls.Add(webView);

            if (this.Controls.ContainsKey("closeButton"))
            {
                this.Controls["closeButton"].BringToFront();
            }

            // [Fix 2] Save WebView2 temporary data in a safe folder (LocalAppData) with no permission issues
            string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LibraryScreensaver_Data");
            
            // InPrivate 모드 및 보안 옵션 설정
            CoreWebView2EnvironmentOptions envOptions = null;
            if (prefsManager.InPrivate)
            {
                envOptions = new CoreWebView2EnvironmentOptions
                {
                    AdditionalBrowserArguments = "--inprivate"
                };
            }

            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder, envOptions);
            await webView.EnsureCoreWebView2Async(env);

            // 오디오 자동 음소거 적용
            if (webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.IsMuted = prefsManager.MuteAudio;

                // 웹페이지 로드 완료 및 오류 시 이벤트 처리
                webView.CoreWebView2.NavigationCompleted += (s, e) =>
                {
                    if (!e.IsSuccess && e.WebErrorStatus != CoreWebView2WebErrorStatus.OperationCanceled)
                    {
                        // 네트워크 오류/서버 다운 시 우아한 모던 디지털 시계 Fallback 화면 렌더링
                        string fallbackHtml = FallbackHtmlProvider.GetFallbackClockHtml(currentLoadedUrl, prefsManager.Language);
                        webView.CoreWebView2.NavigateToString(fallbackHtml);
                    }
                    else if (e.IsSuccess)
                    {
                        // 1. 화면 확대/축소 배율(Zoom Factor) 적용
                        int zoomPercent = prefsManager.GetZoomFactorByScreen(screenNum);
                        webView.ZoomFactor = (zoomPercent > 0 ? zoomPercent : 100) / 100.0;

                        // 2. 글래스모피즘 디지털 시계/날짜 HUD 오버레이 주입 (사용자 설정 모서리 반영)
                        if (prefsManager.ShowClockOverlay)
                        {
                            webView.ExecuteScriptAsync(FallbackHtmlProvider.GetClockOverlayScript(prefsManager.ClockPositionPref));
                        }
                    }
                };
            }

            // ---------------------------------------------------------
            // [Modified part] Use events of the WebView2 control itself, not CoreWebView2.
            // 1. Detect general character and number keys
            webView.KeyDown += (sender, e) =>
            {
                Application.Exit();
            };

            // 2. Detect special keys like arrow keys, Tab, Esc, etc., that might be missed by general KeyDown (safety measure)
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
            // StartTime is handled in the constructor, so leave this empty.
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
                currentLoadedUrl = url;
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

            string rawUrl = Urls[currentSiteIndex];
            var parsed = ScreensaverUrlItem.Parse(rawUrl);

            // URL별 가변 인터벌(초) 타이머 동적 갱신
            if (timer != null)
            {
                int intervalSec = parsed.CustomInterval.HasValue 
                    ? parsed.CustomInterval.Value 
                    : prefsManager.GetRotationIntervalByScreen(screenNum);
                timer.Interval = Math.Max(1, intervalSec) * 1000;
            }

            BrowseTo(parsed.Url);
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