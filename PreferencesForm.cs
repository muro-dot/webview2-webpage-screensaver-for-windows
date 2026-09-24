using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Web_Page_Screensaver
{
    public partial class PreferencesForm : Form 
    {
        private PreferencesManager prefsManager = new PreferencesManager();
        private List<PrefsByScreenUserControl> screenUserControls;
        private string currentLanguage = "ko";
        private bool isThemeEventRegistered = false;

        public PreferencesForm()
        {
            InitializeComponent();
            try
            {
                Icon = ModernAppIcon.CreateAppIcon(32);
            }
            catch { }
            RemoveExtraTabPages();
            screenTabControl.TabPages[0].Text = "Main Display";
            screenUserControls = new List<PrefsByScreenUserControl>() { prefsByScreenUserControl1 };
            LoadValuesForTab(0);

            // 시계 HUD 오버레이 체크 여부에 따라 위치 콤보박스 활성화/비활성화 연동
            cbClockOverlay.CheckedChanged += (s, e) =>
            {
                cmbClockPosition.Enabled = cbClockOverlay.Checked;
            };
        }

        private void PreferencesForm_Load(object sender, EventArgs e)
        {
            cbCloseOnActivity.Checked = prefsManager.CloseOnActivity;
            cbMuteAudio.Checked = prefsManager.MuteAudio;
            cbInPrivate.Checked = prefsManager.InPrivate;
            cbClockOverlay.Checked = prefsManager.ShowClockOverlay;
            cmbClockPosition.Enabled = cbClockOverlay.Checked;

            // 1. 윈도우 시스템 테마(다크/라이트) 감지 및 실시간 변경 이벤트 등록
            RegisterThemeEvents();
            bool isLight = ThemeManager.CheckWindowsLightTheme();
            ThemeManager.IsLightTheme = isLight;
            ApplyTheme(isLight);

            // 2. 저장된 언어 설정 로드 (기본 ko)
            currentLanguage = !string.IsNullOrEmpty(prefsManager.Language) ? prefsManager.Language : "ko";

            if (Screen.AllScreens.Length <= 1)
            {
                spanScreensButton.Enabled = false;
                mirrorScreensButton.Enabled = false;
                separateScreensButton.Enabled = false;
            }
            else
            {
                SetMultiScreenButtonFromMode();
                ArrangeScreenTabs();
            }

            ApplyLanguage(currentLanguage);

            // 3. 백그라운드에서 GitHub 최신 릴리즈 비동기 확인
            CheckForUpdatesAsync();
        }

        private async void CheckForUpdatesAsync()
        {
            try
            {
                var release = await UpdateChecker.CheckForUpdateAsync();
                if (release != null && release.HasUpdate && !IsDisposed && IsHandleCreated)
                {
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        btnUpdateNotice.Text = $"🚀 New: v{release.LatestVersion}";
                        btnUpdateNotice.Tag = release.ReleaseUrl;
                        btnUpdateNotice.Visible = true;
                    }));
                }
            }
            catch { }
        }

        private void RegisterThemeEvents()
        {
            if (!isThemeEventRegistered)
            {
                SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
                isThemeEventRegistered = true;
            }
        }

        public void CleanupThemeEvents()
        {
            if (isThemeEventRegistered)
            {
                SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
                isThemeEventRegistered = false;
            }
        }

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            // Windows 테마(라이트/다크) 변경 감지 시 UI 스레드에서 즉시 전환
            if (e.Category == UserPreferenceCategory.General || e.Category == UserPreferenceCategory.Color)
            {
                if (IsHandleCreated && !IsDisposed)
                {
                    try
                    {
                        BeginInvoke((MethodInvoker)(() =>
                        {
                            bool isLight = ThemeManager.CheckWindowsLightTheme();
                            ThemeManager.IsLightTheme = isLight;
                            ApplyTheme(isLight);
                        }));
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 다크 모드 / 라이트 모드 테마 적용
        /// </summary>
        public void ApplyTheme(bool isLight)
        {
            var colors = ThemeManager.Colors;

            // 1. 폼 및 DWM 윈도우 타이틀바 테마 적용
            BackColor = colors.Background;
            ForeColor = colors.TextPrimary;
            ThemeManager.SetFormTitleBarTheme(Handle, isLight);

            // 2. 헤더 라벨
            lblTitle.ForeColor = colors.TextPrimary;
            lblSubtitle.ForeColor = colors.TextSecondary;

            // 3. 다중 모니터 카드
            multiScreenCard.BackColor = colors.CardBackground;
            multiScreenCard.BorderColor = colors.CardBorder;

            bool isSingleScreen = Screen.AllScreens.Length <= 1;
            Color multiScreenTextColor = isSingleScreen ? colors.TextMuted : colors.TextPrimary;
            lblMultiScreen.ForeColor = multiScreenTextColor;
            spanScreensButton.ForeColor = multiScreenTextColor;
            mirrorScreensButton.ForeColor = multiScreenTextColor;
            separateScreensButton.ForeColor = multiScreenTextColor;

            // 4. 탭 컨트롤 및 각 탭 페이지
            screenTabControl.BackColor = colors.Background;
            foreach (TabPage tab in screenTabControl.TabPages)
            {
                tab.BackColor = colors.CardBackground;
            }

            // 5. 하단 패널 및 컨트롤
            cbCloseOnActivity.ForeColor = colors.TextPrimary;
            cbMuteAudio.ForeColor = colors.TextPrimary;
            cbInPrivate.ForeColor = colors.TextPrimary;
            cbClockOverlay.ForeColor = colors.TextPrimary;
            cmbClockPosition.BackColor = colors.InputBackground;
            cmbClockPosition.ForeColor = colors.TextPrimary;

            btnExport.Invalidate();
            btnImport.Invalidate();
            btnUpdateNotice.Invalidate();

            // 6. 자식 화면 컨트롤들에 테마 전파
            if (screenUserControls != null)
            {
                foreach (var ctrl in screenUserControls)
                {
                    ctrl.ApplyTheme(isLight);
                }
            }

            // 전체 다시 그리기
            Invalidate(true);
        }

        public void ApplyLanguage(string lang)
        {
            currentLanguage = lang;
            prefsManager.Language = lang;
            bool isKo = (lang == "ko");

            btnLangKor.IsSelected = isKo;
            btnLangEng.IsSelected = !isKo;

            Text = isKo ? "웹 화면보호기 설정 v1.0.6" : "WebView2 Web Page Screensaver Settings v1.0.6";
            lblTitle.Text = isKo ? "WebView2 웹 화면보호기 v1.0.6" : "WebView2 Web Screensaver v1.0.6";
            lblSubtitle.Text = isKo ? "웹사이트 및 대시보드를 고해상도 화면보호기로 출력합니다" : "Display websites and live dashboards with Microsoft WebView2";
            lblMultiScreen.Text = isKo ? "다중 모니터 모드:" : "Multi-Monitor Mode:";

            btnExport.Text = isKo ? "백업" : "Export";
            btnImport.Text = isKo ? "복원" : "Import";

            spanScreensButton.Text = isKo ? "확장 (전체 통합)" : "Span (All)";
            mirrorScreensButton.Text = isKo ? "복제 (동일 출력)" : "Mirror (Clone)";
            separateScreensButton.Text = isKo ? "개별 (모니터별 독립)" : "Separate (Each its own list)";

            screenModeTooltip.SetToolTip(spanScreensButton, isKo ? "모든 모니터를 하나의 큰 화면으로 합쳐서 표시합니다" : "Spread a single screen across all monitors");
            screenModeTooltip.SetToolTip(mirrorScreensButton, isKo ? "모든 모니터에 똑같은 웹사이트를 동시에 복제 출력합니다" : "Same websites shown on all monitors");
            screenModeTooltip.SetToolTip(separateScreensButton, isKo ? "각 모니터마다 서로 다른 웹사이트 목록을 설정합니다" : "Configure individual URL list for each screen");

            cbCloseOnActivity.Text = isKo ? "마우스 움직임 시 종료" : "Exit on mouse move";
            cbMuteAudio.Text = isKo ? "오디오 음소거" : "Mute Audio";
            cbInPrivate.Text = isKo ? "시크릿 모드 (InPrivate)" : "InPrivate Browsing";
            cbClockOverlay.Text = isKo ? "시계 HUD 오버레이" : "Clock HUD Overlay";

            // 시계 HUD 모서리 위치 선택 콤보박스 항목 갱신 (선택 상태 보존)
            int selectedClockIndex = cmbClockPosition.SelectedIndex >= 0
                ? cmbClockPosition.SelectedIndex
                : (int)prefsManager.ClockPositionPref;

            cmbClockPosition.Items.Clear();
            if (isKo)
            {
                cmbClockPosition.Items.AddRange(new object[] {
                    "우측 하단 (기본)",
                    "좌측 하단",
                    "우측 상단",
                    "좌측 상단"
                });
            }
            else
            {
                cmbClockPosition.Items.AddRange(new object[] {
                    "Bottom-Right (Default)",
                    "Bottom-Left",
                    "Top-Right",
                    "Top-Left"
                });
            }
            cmbClockPosition.SelectedIndex = (selectedClockIndex >= 0 && selectedClockIndex < cmbClockPosition.Items.Count)
                ? selectedClockIndex
                : 0;

            cancelButton.Text = isKo ? "취소" : "Cancel";
            okButton.Text = isKo ? "저장 및 적용" : "Save & Apply";

            // 언어별 텍스트 길이 변화에 따른 상단/하단 컨트롤 동적 정렬 (간섭 및 겹침 방지)
            AdjustResponsiveLayout();

            // 탭 이름 갱신
            UpdateTabTitles();

            // 자식 화면 컨트롤들에 언어 적용
            if (screenUserControls != null)
            {
                foreach (var ctrl in screenUserControls)
                {
                    ctrl.ApplyLanguage(lang);
                }
            }
        }

        /// <summary>
        /// 언어 변경 및 텍스트 폭 변화에 따라 멀티 모니터 라디오 버튼과 하단 옵션 컨트롤들을
        /// 동적으로 정렬하여 문구 간섭 및 글자 잘림을 방지합니다.
        /// </summary>
        public void AdjustResponsiveLayout()
        {
            // 1. 다중 모니터 라벨 및 라디오 버튼 동적 정렬 (영문 오버랩 해결)
            lblMultiScreen.AutoSize = true;
            spanScreensButton.AutoSize = true;
            mirrorScreensButton.AutoSize = true;
            separateScreensButton.AutoSize = true;

            int multiStartX = lblMultiScreen.Right + 14;
            spanScreensButton.Location = new Point(multiStartX, 12);
            mirrorScreensButton.Location = new Point(spanScreensButton.Right + 14, 12);
            separateScreensButton.Location = new Point(mirrorScreensButton.Right + 14, 12);

            // 2. 하단 패널 컨트롤 동적 정렬 (1열과 2열 및 버튼 간격 최적화)
            cbCloseOnActivity.AutoSize = true;
            cbInPrivate.AutoSize = true;
            cbMuteAudio.AutoSize = true;
            cbClockOverlay.AutoSize = true;

            cbCloseOnActivity.Location = new Point(4, 10);
            cbInPrivate.Location = new Point(4, 42);

            int col2Left = Math.Max(cbCloseOnActivity.Right, cbInPrivate.Right) + 24;
            cbMuteAudio.Location = new Point(col2Left, 10);
            cbClockOverlay.Location = new Point(col2Left, 42);

            // 시계 위치 콤보박스: 영문 텍스트가 전부 보이도록 180px 너비 및 안전 여백 확보
            cmbClockPosition.Size = new Size(180, 23);
            cmbClockPosition.Location = new Point(cbClockOverlay.Right + 8, 39);

            // 3. 상단 헤더 우측 컨트롤 정렬 (GitHub 버튼 우측 여백을 하단 버튼과 일치하게 8px로 밀착)
            if (headerPanel.Width >= 500)
            {
                int headerRight = headerPanel.Width - 8;
                btnGithub.Location = new Point(headerRight - btnGithub.Width, 11);
                langPanel.Location = new Point(btnGithub.Left - 8 - langPanel.Width, 11);
                btnUpdateNotice.Location = new Point(langPanel.Left - 8 - btnUpdateNotice.Width, 11);
            }

            // 4. 우측 확인/취소 버튼 및 바로 윗자리 백업/복원 버튼 정렬 (우측 끝 8px 일치)
            int panelW = bottomPanel.ClientSize.Width > 0 ? bottomPanel.ClientSize.Width : 812;
            int bottomRight = panelW - 8;
            okButton.Location = new Point(bottomRight - okButton.Width, 38);
            cancelButton.Location = new Point(okButton.Left - 8 - cancelButton.Width, 38);

            // 취소/저장 바로 윗자리로 백업/복원 배치
            btnExport.Location = new Point(cancelButton.Left, 6);
            btnExport.Size = new Size(cancelButton.Width, 28);
            btnImport.Location = new Point(okButton.Left, 6);
            btnImport.Size = new Size(okButton.Width, 28);
        }

        private void btnLangKor_Click(object sender, EventArgs e)
        {
            ApplyLanguage("ko");
        }

        private void btnLangEng_Click(object sender, EventArgs e)
        {
            ApplyLanguage("en");
        }

        /// <summary>
        /// 빌드 시 자동 스크린샷 캡처를 위해 개인 URL 대신 안전한 예시 URL을 주입하고 화면을 정돈합니다.
        /// </summary>
        public void PrepareForScreenshot(string exampleUrl = "https://example.com/screensaver")
        {
            if (prefsByScreenUserControl1 != null && prefsByScreenUserControl1.lvUrls != null)
            {
                prefsByScreenUserControl1.lvUrls.Items.Clear();
                prefsByScreenUserControl1.lvUrls.Items.Add(exampleUrl);
            }
            cbClockOverlay.Checked = true;
            cmbClockPosition.Enabled = true;
            cmbClockPosition.SelectedIndex = 0;
            btnUpdateNotice.Visible = false;
            ApplyLanguage("en");
        }

        /// <summary>
        /// 스크린샷 및 시연용으로 새 버전 업데이트 알림 배지를 활성화합니다.
        /// </summary>
        public void SetUpdateNoticeForDemo(string version = "1.0.7", string url = "https://github.com/muro-dot/Webview2_WebPage_Screensaver/releases")
        {
            btnUpdateNotice.Text = $"🚀 New: v{version}";
            btnUpdateNotice.Tag = url;
            btnUpdateNotice.Visible = true;
            btnUpdateNotice.BringToFront();
        }

        private void LoadValuesForTab(int screenNum)
        {
            if (screenNum < screenUserControls.Count)
            {
                var currentPrefsUserControl = screenUserControls[screenNum];
                loadUrlsForTabToControl(screenNum, currentPrefsUserControl);
                currentPrefsUserControl.nudRotationInterval.Value = Math.Max(1, prefsManager.GetRotationIntervalByScreen(screenNum));
                currentPrefsUserControl.cbRandomize.Checked = prefsManager.GetRandomizeFlagByScreen(screenNum);

                // 줌 팩터 반영
                int zoom = prefsManager.GetZoomFactorByScreen(screenNum);
                string zoomStr = $"{zoom}%";
                int idx = currentPrefsUserControl.cmbZoom.Items.IndexOf(zoomStr);
                currentPrefsUserControl.cmbZoom.SelectedIndex = idx >= 0 ? idx : 1;

                currentPrefsUserControl.ApplyLanguage(currentLanguage);
            }
        }

        private void UpdateTabTitles()
        {
            bool isKo = (currentLanguage == "ko");
            switch (prefsManager.MultiScreenMode)
            {
                case PreferencesManager.MultiScreenModeItem.Span:
                    if (screenTabControl.TabPages.Count > 0)
                        screenTabControl.TabPages[0].Text = isKo ? "통합 화면" : "Composite Display";
                    break;

                case PreferencesManager.MultiScreenModeItem.Mirror:
                    if (screenTabControl.TabPages.Count > 0)
                        screenTabControl.TabPages[0].Text = isKo ? "모든 모니터 (복제)" : "All Displays (Mirrored)";
                    break;

                case PreferencesManager.MultiScreenModeItem.Separate:
                    for (int i = 0; i < screenTabControl.TabPages.Count; i++)
                    {
                        string primaryIndicator = (i < Screen.AllScreens.Length && Screen.AllScreens[i].Primary)
                            ? (isKo ? " (기본)" : " (Primary)")
                            : string.Empty;
                        string prefix = isKo ? "화면" : "Screen";
                        screenTabControl.TabPages[i].Text = string.Format("{0} {1}{2}", prefix, i + 1, primaryIndicator);
                    }
                    break;
            }
        }

        private void ArrangeScreenTabs()
        {
            switch (prefsManager.MultiScreenMode)
            {
                case PreferencesManager.MultiScreenModeItem.Span:
                    RemoveExtraTabPages();
                    screenUserControls = new List<PrefsByScreenUserControl>() { prefsByScreenUserControl1 };
                    LoadValuesForTab(0);
                    break;

                case PreferencesManager.MultiScreenModeItem.Mirror:
                    RemoveExtraTabPages();
                    screenUserControls = new List<PrefsByScreenUserControl>() { prefsByScreenUserControl1 };
                    LoadValuesForTab(0);
                    break;

                case PreferencesManager.MultiScreenModeItem.Separate:
                    for (int i = 0; i < Screen.AllScreens.Length; i++)
                    {
                        TabPage tabPage = null; 

                        if (i >= screenTabControl.TabPages.Count)
                        {
                            tabPage = new TabPage();
                            tabPage.BackColor = ThemeManager.Colors.CardBackground;
                            tabPage.Padding = new Padding(12);
                            screenTabControl.TabPages.Add(tabPage);

                            if (i > 0)
                            {
                                var prefsByScreenUserControl = new PrefsByScreenUserControl
                                {
                                    Name = string.Format("prefsByScreenUserControl{0}", i + 1),
                                    Dock = DockStyle.Fill,
                                    BackColor = ThemeManager.Colors.CardBackground,
                                    Font = new Font("Segoe UI", 9f)
                                };
                                prefsByScreenUserControl.lvUrls.ContextMenuStrip =
                                    prefsByScreenUserControl1.lvUrls.ContextMenuStrip;
                                prefsByScreenUserControl.ApplyTheme(ThemeManager.IsLightTheme);
                                screenUserControls.Add(prefsByScreenUserControl);
                                tabPage.Controls.Add(prefsByScreenUserControl);
                            }
                        }
                        else if (screenTabControl.TabPages.Count == 1)
                        {
                            tabPage = screenTabControl.TabPages[0];
                            screenUserControls = new List<PrefsByScreenUserControl>() { prefsByScreenUserControl1 };
                        }

                        LoadValuesForTab(i);
                    }
                    break;
            }

            UpdateTabTitles();
        }

        private void RemoveExtraTabPages()
        {
            while (screenTabControl.TabPages.Count > 1)
            {
                screenTabControl.TabPages.RemoveAt(screenTabControl.TabPages.Count - 1);
            }
        }

        private void SetMultiScreenButtonFromMode()
        {
            switch (prefsManager.MultiScreenMode)
            {
                case PreferencesManager.MultiScreenModeItem.Span:
                    spanScreensButton.Checked = true;
                    break;
                case PreferencesManager.MultiScreenModeItem.Mirror:
                    mirrorScreensButton.Checked = true;
                    break;
                case PreferencesManager.MultiScreenModeItem.Separate:
                    separateScreensButton.Checked = true;
                    break;
            }
        }

        private void setMultiScreenModeFromButtonState()
        {
            if (spanScreensButton.Checked)
            {
                prefsManager.MultiScreenMode = PreferencesManager.MultiScreenModeItem.Span;
            }
            else if (mirrorScreensButton.Checked)
            {
                prefsManager.MultiScreenMode = PreferencesManager.MultiScreenModeItem.Mirror;
            }
            else
            {
                prefsManager.MultiScreenMode = PreferencesManager.MultiScreenModeItem.Separate;
            }

            prefsManager.ResetEffectiveScreensList();
        }

        private void readBackValuesFromUI()
        {
            try
            {
                prefsManager.CloseOnActivity = cbCloseOnActivity.Checked;
                prefsManager.MuteAudio = cbMuteAudio.Checked;
                prefsManager.InPrivate = cbInPrivate.Checked;
                prefsManager.ShowClockOverlay = cbClockOverlay.Checked;
                if (cmbClockPosition.SelectedIndex >= 0)
                {
                    prefsManager.ClockPositionPref = (PreferencesManager.ClockPosition)cmbClockPosition.SelectedIndex;
                }
                prefsManager.Language = currentLanguage;

                for (var i = 0; i < screenUserControls.Count; i++)
                {
                    var currentPrefsUserControl = screenUserControls[i];
                    List<string> urls = (from ListViewItem lvUrlsItem in currentPrefsUserControl.lvUrls.Items
                        select lvUrlsItem.Text).ToList();
                    prefsManager.SetUrlsForScreen(i, urls);
                    prefsManager.SetRotationIntervalForScreen(i,
                        (int) currentPrefsUserControl.nudRotationInterval.Value);
                    prefsManager.SetRandomizeFlagForScreen(i, currentPrefsUserControl.cbRandomize.Checked);

                    if (currentPrefsUserControl.cmbZoom.SelectedItem != null)
                    {
                        string zoomStr = currentPrefsUserControl.cmbZoom.SelectedItem.ToString().TrimEnd('%');
                        if (int.TryParse(zoomStr, out int z))
                        {
                            prefsManager.SetZoomFactorForScreen(i, z);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void loadUrlsForTabToControl(int screenNum, PrefsByScreenUserControl currentPrefsUserControl)
        {
            currentPrefsUserControl.lvUrls.Items.Clear();

            var urls = prefsManager.GetUrlsByScreen(screenNum);

            foreach (var url in urls)
            {
                currentPrefsUserControl.lvUrls.Items.Add(url);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            CleanupThemeEvents();

            if (DialogResult == DialogResult.OK)
            {
                readBackValuesFromUI();
                prefsManager.SavePreferences();
            }

            base.OnClosed(e);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            readBackValuesFromUI();
            ConfigExportImport.ExportToFile(prefsManager, this, currentLanguage);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (ConfigExportImport.ImportFromFile(prefsManager, this, currentLanguage))
            {
                cbCloseOnActivity.Checked = prefsManager.CloseOnActivity;
                cbMuteAudio.Checked = prefsManager.MuteAudio;
                cbInPrivate.Checked = prefsManager.InPrivate;
                cbClockOverlay.Checked = prefsManager.ShowClockOverlay;
                cmbClockPosition.Enabled = cbClockOverlay.Checked;
                cmbClockPosition.SelectedIndex = (int)prefsManager.ClockPositionPref;
                currentLanguage = prefsManager.Language ?? "ko";
                SetMultiScreenButtonFromMode();
                ArrangeScreenTabs();
                ApplyLanguage(currentLanguage);
            }
        }

        private void btnUpdateNotice_Click(object sender, EventArgs e)
        {
            string url = btnUpdateNotice.Tag?.ToString();
            if (!string.IsNullOrEmpty(url))
            {
                try { System.Diagnostics.Process.Start(url); } catch { }
            }
        }

        private void btnGithub_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/muro-dot/Webview2_WebPage_Screensaver");
            }
            catch { }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void anyMultiScreenModeButton_Click(object sender, EventArgs e)
        {
            readBackValuesFromUI();
            setMultiScreenModeFromButtonState();
            ArrangeScreenTabs();
        }
    }
}
