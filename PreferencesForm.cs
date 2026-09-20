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
        }

        private void PreferencesForm_Load(object sender, EventArgs e)
        {
            cbCloseOnActivity.Checked = prefsManager.CloseOnActivity;

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

            Text = isKo ? "웹 화면보호기 설정 v1.0.4" : "WebView2 Web Page Screensaver Settings v1.0.4";
            lblTitle.Text = isKo ? "WebView2 웹 화면보호기 v1.0.4" : "WebView2 Web Screensaver v1.0.4";
            lblSubtitle.Text = isKo ? "웹사이트 및 대시보드를 고해상도 화면보호기로 출력합니다" : "Display websites and live dashboards with Microsoft WebView2";
            lblMultiScreen.Text = isKo ? "다중 모니터 모드:" : "Multi-Monitor Mode:";

            spanScreensButton.Text = isKo ? "확장 (전체 통합)" : "Span (All)";
            mirrorScreensButton.Text = isKo ? "복제 (동일 출력)" : "Mirror (Clone)";
            separateScreensButton.Text = isKo ? "개별 (모니터별 독립)" : "Separate (Each its own list)";

            screenModeTooltip.SetToolTip(spanScreensButton, isKo ? "모든 모니터를 하나의 큰 화면으로 합쳐서 표시합니다" : "Spread a single screen across all monitors");
            screenModeTooltip.SetToolTip(mirrorScreensButton, isKo ? "모든 모니터에 똑같은 웹사이트를 동시에 복제 출력합니다" : "Same websites shown on all monitors");
            screenModeTooltip.SetToolTip(separateScreensButton, isKo ? "각 모니터마다 서로 다른 웹사이트 목록을 설정합니다" : "Configure individual URL list for each screen");

            cbCloseOnActivity.Text = isKo ? "마우스 움직임 시 화면보호기 종료" : "Exit screensaver on mouse movement";
            cancelButton.Text = isKo ? "취소" : "Cancel";
            okButton.Text = isKo ? "저장 및 적용" : "Save & Apply";

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

        private void btnLangKor_Click(object sender, EventArgs e)
        {
            ApplyLanguage("ko");
        }

        private void btnLangEng_Click(object sender, EventArgs e)
        {
            ApplyLanguage("en");
        }

        private void LoadValuesForTab(int screenNum)
        {
            if (screenNum < screenUserControls.Count)
            {
                var currentPrefsUserControl = screenUserControls[screenNum];
                loadUrlsForTabToControl(screenNum, currentPrefsUserControl);
                currentPrefsUserControl.nudRotationInterval.Value = Math.Max(1, prefsManager.GetRotationIntervalByScreen(screenNum));
                currentPrefsUserControl.cbRandomize.Checked = prefsManager.GetRandomizeFlagByScreen(screenNum);
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
                for (var i = 0; i < screenUserControls.Count; i++)
                {
                    var currentPrefsUserControl = screenUserControls[i];
                    List<string> urls = (from ListViewItem lvUrlsItem in currentPrefsUserControl.lvUrls.Items
                        select lvUrlsItem.Text).ToList();
                    prefsManager.SetUrlsForScreen(i, urls);
                    prefsManager.SetRotationIntervalForScreen(i,
                        (int) currentPrefsUserControl.nudRotationInterval.Value);
                    prefsManager.SetRandomizeFlagForScreen(i, currentPrefsUserControl.cbRandomize.Checked);
                    prefsManager.CloseOnActivity = cbCloseOnActivity.Checked;
                    prefsManager.Language = currentLanguage;
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
