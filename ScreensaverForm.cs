using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

namespace Web_Page_Screensaver
{
    public partial class ScreensaverForm : Form
    {
        private Timer timer;
        private Timer mouseMonitorTimer;
        private Point initialMousePos;
        private int currentSiteIndex = -1;
        private bool shuffleOrder;
        private List<string> urls;

        private PreferencesManager prefsManager = new PreferencesManager();
        private int screenNum;

        private WebView2 webView;
        private string currentLoadedUrl = string.Empty;
        private Timer recoveryTimer = null;

        // 폴백 HTML(NavigateToString) 표시 중인지 여부를 추적하는 플래그
        // NavigateToString 역시 NavigationCompleted(IsSuccess=true)를 발생시키므로,
        // 복구 타이머가 즉시 꺼지거나 시계 HUD가 폴백 위에 중복 주입되는 것을 방지합니다.
        private bool isShowingFallback = false;

        // 브라우저 프로세스 연속 크래시 카운터 (무한 재초기화 루프 및 CPU 폭증 방지)
        private int consecutiveCrashCount = 0;
        private const int MAX_CONSECUTIVE_CRASHES = 3;

        [ThreadStatic]
        private static Random random;

        public ScreensaverForm(int? screenNumber = null)
        {
            if (screenNumber == null) screenNum = prefsManager.EffectiveScreensList.FindIndex(s => s.IsPrimary);
            else screenNum = (int)screenNumber;

            InitializeComponent();
            try
            {
                Icon = ModernAppIcon.CreateAppIcon(32);
            }
            catch { }

            // 마우스 커서 숨김
            Cursor.Hide();

            // 마우스 이동 감지 타이머 초기화 (전역 유예시간 매니저 사용)
            initialMousePos = Cursor.Position;
            mouseMonitorTimer = new Timer();
            mouseMonitorTimer.Interval = 50;
            mouseMonitorTimer.Tick += MouseMonitorTimer_Tick;
            mouseMonitorTimer.Start();

            // 폼 종료 시 타이머 해제
            this.FormClosed += (s, e) =>
            {
                StopRecoveryTimer();
                if (mouseMonitorTimer != null)
                {
                    mouseMonitorTimer.Stop();
                    mouseMonitorTimer.Dispose();
                }
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                }
            };

            InitializeWebViewAsync();
        }

        private void MouseMonitorTimer_Tick(object sender, EventArgs e)
        {
            // 프로그램 시작 후 1.5초 유예시간 이내에는 마우스 미세 진동으로 인한 종료를 원천 방지
            if (GracePeriodManager.IsInGracePeriod())
            {
                initialMousePos = Cursor.Position;
                return;
            }

            Point currentPos = Cursor.Position;

            // X 또는 Y 좌표가 3픽셀 이상 의미 있게 이동했을 때에만 사용자 활동으로 인정
            if (Math.Abs(initialMousePos.X - currentPos.X) > 3 ||
                Math.Abs(initialMousePos.Y - currentPos.Y) > 3)
            {
                mouseMonitorTimer.Stop();
                HandleUserActivity();
            }
        }

        /// <summary>
        /// WebView2 컨트롤을 비동기로 초기화하고 이벤트와 보안 설정을 바인딩합니다.
        /// </summary>
        private async void InitializeWebViewAsync()
        {
            await SetupWebViewControlAsync();
            StartScreensaverLogic();
        }

        private async Task SetupWebViewControlAsync()
        {
            try
            {
                if (webView == null)
                {
                    webView = new WebView2();
                    webView.Dock = DockStyle.Fill;
                    this.Controls.Add(webView);

                    if (this.Controls.ContainsKey("closeButton"))
                    {
                        this.Controls["closeButton"].BringToFront();
                    }

                    // 키보드 입력 감지 (전역 유예시간 적용으로 조기 종료 방지)
                    webView.KeyDown += (sender, e) =>
                    {
                        if (!GracePeriodManager.IsInGracePeriod())
                        {
                            Application.Exit();
                        }
                    };

                    webView.PreviewKeyDown += (sender, e) =>
                    {
                        if (!GracePeriodManager.IsInGracePeriod())
                        {
                            Application.Exit();
                        }
                    };
                }

                // InPrivate 격리 세션 매니저를 통해 다중 모니터 간 단일 임시 디렉토리를 안전하게 공유
                string userDataFolder = InPrivateSessionManager.GetUserDataFolder(prefsManager.InPrivate);
                CoreWebView2EnvironmentOptions envOptions = InPrivateSessionManager.GetEnvironmentOptions(prefsManager.InPrivate);

                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder, envOptions);
                await webView.EnsureCoreWebView2Async(env);

                if (webView.CoreWebView2 != null)
                {
                    // 1. 오디오 자동 음소거 적용
                    webView.CoreWebView2.IsMuted = prefsManager.MuteAudio;

                    // 2. 무인 화면보호기 보안 및 방어 설정 (팝업, 권한, 다운로드, 우클릭 차단)
                    ConfigureWebViewSecurity(webView.CoreWebView2);

                    // 3. 프로세스 충돌(ProcessFailed) 핸들러 등록
                    webView.CoreWebView2.ProcessFailed += OnWebViewProcessFailed;

                    // 4. 웹페이지 로드 완료 및 에러 처리 핸들러 등록
                    webView.CoreWebView2.NavigationCompleted += OnWebViewNavigationCompleted;
                }
            }
            catch
            {
                // WebView2 런타임 미설치나 초기화 실패 시 블랙아웃을 방지하기 위해 오프라인 폴백 화면 표시
                ShowFallbackClock();
            }
        }

        /// <summary>
        /// 브라우저 렌더러 또는 메인 브라우저 프로세스 충돌 시 안정적인 자동 복구 로직
        /// </summary>
        private async void OnWebViewProcessFailed(object sender, CoreWebView2ProcessFailedEventArgs e)
        {
            try
            {
                consecutiveCrashCount++;
                if (consecutiveCrashCount > MAX_CONSECUTIVE_CRASHES)
                {
                    // 3회 이상 연속 크래시 발생 시 무한 루프 및 CPU 과점유를 방지하고 오프라인 폴백 시계로 안전 안착
                    ShowFallbackClock();
                    return;
                }

                if (e.ProcessFailedKind == CoreWebView2ProcessFailedKind.BrowserProcessExited)
                {
                    // 브라우저 메인 프로세스가 종료되면 기존 CoreWebView2 인스턴스는 영구 무효화되므로
                    // 컨트롤을 완전히 정리(Dispose)하고 새로 생성하여 재초기화합니다.
                    await RecreateWebViewAsync();
                }
                else
                {
                    // 단순 렌더러 프로세스 비정상 종료 시에는 리로드 시도
                    if (webView != null && webView.CoreWebView2 != null)
                    {
                        webView.Reload();
                    }
                }
            }
            catch
            {
                ShowFallbackClock();
            }
        }

        /// <summary>
        /// 브라우저 메인 프로세스 종료 시 컨트롤을 완전히 폐기하고 재생성하는 안전 복구 메서드
        /// </summary>
        private async Task RecreateWebViewAsync()
        {
            try
            {
                if (webView != null)
                {
                    this.Controls.Remove(webView);
                    webView.Dispose();
                    webView = null;
                }

                await SetupWebViewControlAsync();

                if (!string.IsNullOrEmpty(currentLoadedUrl))
                {
                    BrowseTo(currentLoadedUrl);
                }
            }
            catch
            {
                ShowFallbackClock();
            }
        }

        /// <summary>
        /// 웹페이지 로드 완료 이벤트 핸들러 (폴백 플래그 방어 및 HUD 주입)
        /// </summary>
        private void OnWebViewNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // 1. 폴백 HTML(NavigateToString) 로드 완료인 경우
            // NavigateToString 역시 IsSuccess=true를 발생시키므로 복구 타이머를 끄거나 HUD를 주입하지 않고 무시합니다.
            if (isShowingFallback)
            {
                return;
            }

            // 2. 실제 대상 웹페이지 로드 실패 시
            if (!e.IsSuccess && e.WebErrorStatus != CoreWebView2WebErrorStatus.OperationCanceled)
            {
                ShowFallbackClock();
                // 일시적 네트워크 장애에 대비해 30초마다 원래 URL로 재연결을 시도하는 복구 타이머 가동
                StartRecoveryTimer();
            }
            // 3. 실제 대상 웹페이지 로드 성공 시
            else if (e.IsSuccess)
            {
                // 성공했으므로 크래시 카운터 리셋 및 복구 타이머 정지
                consecutiveCrashCount = 0;
                StopRecoveryTimer();

                // 화면 확대/축소 배율(Zoom Factor) 적용
                int zoomPercent = prefsManager.GetZoomFactorByScreen(screenNum);
                webView.ZoomFactor = (zoomPercent > 0 ? zoomPercent : 100) / 100.0;

                // 디지털 시계/날짜 HUD 오버레이 주입 (설정된 모서리 위치 반영)
                if (prefsManager.ShowClockOverlay)
                {
                    webView.ExecuteScriptAsync(FallbackHtmlProvider.GetClockOverlayScript(prefsManager.ClockPositionPref));
                }
            }
        }

        /// <summary>
        /// 네트워크 단절이나 크래시 발생 시 오프라인 디지털 시계 화면을 안전하게 렌더링합니다.
        /// </summary>
        private void ShowFallbackClock()
        {
            try
            {
                isShowingFallback = true;
                string fallbackHtml = FallbackHtmlProvider.GetFallbackClockHtml(currentLoadedUrl, prefsManager.Language);
                if (webView != null && webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.NavigateToString(fallbackHtml);
                }
            }
            catch { }
        }

        /// <summary>
        /// 무인 화면보호기 보안 설정 (팝업, 권한 요구, 다운로드, 스크립트 대화상자 차단)
        /// </summary>
        private void ConfigureWebViewSecurity(CoreWebView2 core)
        {
            core.Settings.AreDefaultContextMenusEnabled = false;
            core.Settings.AreDevToolsEnabled = false;
            core.Settings.AreBrowserAcceleratorKeysEnabled = false;

            // 새 창/팝업 차단
            core.NewWindowRequested += (s, e) =>
            {
                e.Handled = true;
            };

            // 카메라, 마이크, 알림 등 모든 디바이스 권한 자동 거부
            core.PermissionRequested += (s, e) =>
            {
                e.State = CoreWebView2PermissionState.Deny;
            };

            // 무인 상태 파일 다운로드 차단
            core.DownloadStarting += (s, e) =>
            {
                e.Cancel = true;
            };

            // alert/confirm 등 모달 대화상자 자동 억제
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
                        // 재시도 시에는 실제 웹페이지 로딩을 시도하므로 폴백 플래그를 해제하고 탐색
                        isShowingFallback = false;
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

                    // 회전 타이머 Interval 최소 1초 안전 보장
                    int intervalSec = Math.Max(1, prefsManager.GetRotationIntervalByScreen(screenNum));
                    timer = new Timer();
                    timer.Interval = intervalSec * 1000;
                    timer.Tick += (s, ee) => RotateSite();
                    timer.Start();
                }

                RotateSite();
            }
            else
            {
                if (webView != null) webView.Visible = false;
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
                if (webView != null) webView.Visible = false;
            }
            else
            {
                if (webView != null) webView.Visible = true;
                currentLoadedUrl = url;
                // 새로운 실제 URL 탐색이 시작되므로 폴백 플래그 해제
                isShowingFallback = false;
                try
                {
                    if (webView != null && webView.CoreWebView2 != null)
                        webView.CoreWebView2.Navigate(url);
                }
                catch { }
            }
        }

        private void RotateSite()
        {
            // 다음 사이트로 회전할 때 이전 사이트의 재연결 타이머는 깨끗이 정지
            StopRecoveryTimer();

            currentSiteIndex++;
            if (currentSiteIndex >= Urls.Count) currentSiteIndex = 0;

            string rawUrl = Urls[currentSiteIndex];
            var parsed = ScreensaverUrlItem.Parse(rawUrl);

            // URL별 가변 인터벌(초) 타이머 동적 갱신 (최소 1초 안전 보장)
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