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
        private string inPrivateTempFolder = null;
        private Timer recoveryTimer = null;

        private async void InitializeWebViewAsync()
        {
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            this.Controls.Add(webView);

            if (this.Controls.ContainsKey("closeButton"))
            {
                this.Controls["closeButton"].BringToFront();
            }

            // 폼 종료 시 InPrivate 임시 데이터 정리 및 타이머 해제
            this.FormClosed += (s, e) =>
            {
                StopRecoveryTimer();
                CleanupInPrivateData();
            };

            string userDataFolder;
            CoreWebView2EnvironmentOptions envOptions = null;

            if (prefsManager.InPrivate)
            {
                // InPrivate 모드: 고유한 임시 디렉토리를 생성하여 캐시/쿠키가 남지 않는 완전 격리 세션 보장
                inPrivateTempFolder = Path.Combine(Path.GetTempPath(), "Screensaver_InPrivate_" + Guid.NewGuid().ToString("N"));
                userDataFolder = inPrivateTempFolder;
                envOptions = new CoreWebView2EnvironmentOptions
                {
                    AdditionalBrowserArguments = "--inprivate"
                };
            }
            else
            {
                userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LibraryScreensaver_Data");
            }

            try
            {
                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder, envOptions);
                await webView.EnsureCoreWebView2Async(env);
            }
            catch
            {
                // 환경 초기화 실패 시 무시하고 진행
                return;
            }

            if (webView.CoreWebView2 != null)
            {
                // 1. 오디오 자동 음소거 적용
                webView.CoreWebView2.IsMuted = prefsManager.MuteAudio;

                // 2. 무인 화면보호기 보안 및 방어 설정 (팝업, 권한, 다운로드, 우클릭 차단)
                ConfigureWebViewSecurity(webView.CoreWebView2);

                // 3. 브라우저/렌더러 프로세스 충돌(ProcessFailed) 시 자동 복구
                webView.CoreWebView2.ProcessFailed += (s, e) =>
                {
                    try
                    {
                        if (e.ProcessFailedKind == CoreWebView2ProcessFailedKind.BrowserProcessExited)
                        {
                            if (!string.IsNullOrEmpty(currentLoadedUrl))
                            {
                                BrowseTo(currentLoadedUrl);
                            }
                        }
                        else
                        {
                            webView.Reload();
                        }
                    }
                    catch { }
                };

                // 4. 웹페이지 로드 완료 및 오류 시 이벤트 처리
                webView.CoreWebView2.NavigationCompleted += (s, e) =>
                {
                    if (!e.IsSuccess && e.WebErrorStatus != CoreWebView2WebErrorStatus.OperationCanceled)
                    {
                        // 네트워크 오류/서버 다운 시 우아한 모던 디지털 시계 Fallback 화면 렌더링
                        string fallbackHtml = FallbackHtmlProvider.GetFallbackClockHtml(currentLoadedUrl, prefsManager.Language);
                        webView.CoreWebView2.NavigateToString(fallbackHtml);

                        // 단일 URL이거나 네트워크 일시 단절 시 30초마다 자동 재연결을 시도하는 복구 타이머 가동
                        StartRecoveryTimer();
                    }
                    else if (e.IsSuccess)
                    {
                        // 로드 성공 시 복구 타이머 정지
                        StopRecoveryTimer();

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

            // 키보드 입력 감지 (특수키 포함)
            webView.KeyDown += (sender, e) =>
            {
                Application.Exit();
            };

            webView.PreviewKeyDown += (sender, e) =>
            {
                Application.Exit();
            };

            StartScreensaverLogic();
        }

        private void ConfigureWebViewSecurity(CoreWebView2 core)
        {
            // 우클릭 컨텍스트 메뉴, 개발자 도구, 액셀러레이터 키 비활성화
            core.Settings.AreDefaultContextMenusEnabled = false;
            core.Settings.AreDevToolsEnabled = false;
            core.Settings.AreBrowserAcceleratorKeysEnabled = false;

            // 새 창/팝업 차단 (외부 브라우저나 새 창이 화면보호기 위/아래로 뜨는 것 방지)
            core.NewWindowRequested += (s, e) =>
            {
                e.Handled = true;
            };

            // 카메라, 마이크, 위치, 알림 등 모든 디바이스/웹 권한 요청 자동 거부
            core.PermissionRequested += (s, e) =>
            {
                e.State = CoreWebView2PermissionState.Deny;
            };

            // 무인 상태에서의 불필요/악성 파일 다운로드 차단
            core.DownloadStarting += (s, e) =>
            {
                e.Cancel = true;
            };

            // alert/confirm/prompt 자바스크립트 대화상자가 화면보호기를 정지시키는 현상 차단 (대화상자 UI 억제)
            core.ScriptDialogOpening += (s, e) =>
            {
                e.Accept();
            };
        }

        private void StartRecoveryTimer()
        {
            if (recoveryTimer == null)
            {
                recoveryTimer = new Timer { Interval = 30000 }; // 30초 주기
                recoveryTimer.Tick += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(currentLoadedUrl))
                    {
                        BrowseTo(currentLoadedUrl);
                    }
                };
            }
            recoveryTimer.Stop();
            recoveryTimer.Start();
        }

        private void StopRecoveryTimer()
        {
            if (recoveryTimer != null)
            {
                recoveryTimer.Stop();
            }
        }

        private void CleanupInPrivateData()
        {
            try
            {
                if (!string.IsNullOrEmpty(inPrivateTempFolder) && Directory.Exists(inPrivateTempFolder))
                {
                    Directory.Delete(inPrivateTempFolder, true);
                }
            }
            catch { }
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