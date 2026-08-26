using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Web_Page_Screensaver
{
    public partial class PreferencesForm : Form 
    {
        private PreferencesManager prefsManager = new PreferencesManager();
        private List<PrefsByScreenUserControl> screenUserControls;
        private string currentLanguage = "ko";

        public PreferencesForm()
        {
            InitializeComponent();
            RemoveExtraTabPages();
            screenTabControl.TabPages[0].Text = "Main Display";
            screenUserControls = new List<PrefsByScreenUserControl>() { prefsByScreenUserControl1 };
            LoadValuesForTab(0);
        }

        private void PreferencesForm_Load(object sender, EventArgs e)
        {
            cbCloseOnActivity.Checked = prefsManager.CloseOnActivity;

            // 저장된 언어 설정 로드 (기본 ko)
            currentLanguage = !string.IsNullOrEmpty(prefsManager.Language) ? prefsManager.Language : "ko";

            if (Screen.AllScreens.Length <= 1)
            {
                multiScreenCard.Enabled = false;
            }
            else
            {
                multiScreenCard.Enabled = true;
                SetMultiScreenButtonFromMode();
                ArrangeScreenTabs();
            }

            ApplyLanguage(currentLanguage);
        }

        public void ApplyLanguage(string lang)
        {
            currentLanguage = lang;
            prefsManager.Language = lang;
            bool isKo = (lang == "ko");

            btnLangKor.IsSelected = isKo;
            btnLangEng.IsSelected = !isKo;

            Text = isKo ? "웹 화면보호기 설정 v1.0.3" : "WebView2 Web Page Screensaver Settings v1.0.3";
            lblTitle.Text = isKo ? "WebView2 웹 화면보호기 v1.0.3" : "WebView2 Web Screensaver v1.0.3";
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
                            tabPage.BackColor = DarkColors.CardBackground;
                            tabPage.Padding = new Padding(12);
                            screenTabControl.TabPages.Add(tabPage);

                            if (i > 0)
                            {
                                var prefsByScreenUserControl = new PrefsByScreenUserControl
                                {
                                    Name = string.Format("prefsByScreenUserControl{0}", i + 1),
                                    Dock = DockStyle.Fill,
                                    BackColor = DarkColors.CardBackground,
                                    Font = new Font("Segoe UI", 9f)
                                };
                                prefsByScreenUserControl.lvUrls.ContextMenuStrip =
                                    prefsByScreenUserControl1.lvUrls.ContextMenuStrip;
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
