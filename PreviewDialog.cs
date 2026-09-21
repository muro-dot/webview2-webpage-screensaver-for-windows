using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Web_Page_Screensaver
{
    /// <summary>
    /// 선택한 URL의 실제 웹 화면보호기 렌더링을 미리 확인해볼 수 있는 미니 웹 미리보기 팝업 창
    /// </summary>
    public class PreviewDialog : Form
    {
        private WebView2 webView;
        private Label lblStatus;
        private ModernButton btnClose;
        private string targetUrl;
        private double zoomFactor;
        private bool isMuted;

        public PreviewDialog(string url, double zoom, bool mute, string language)
        {
            targetUrl = url;
            zoomFactor = zoom > 0 ? zoom : 1.0;
            isMuted = mute;

            bool isKo = (language == "ko");
            Text = (isKo ? "웹 화면보호기 실시간 미리보기 - " : "Live Web Screensaver Preview - ") + url;
            Size = new Size(1024, 640);
            StartPosition = FormStartPosition.CenterParent;
            KeyPreview = true;

            try
            {
                Icon = ModernAppIcon.CreateAppIcon(32);
            }
            catch { }

            InitializeUI(isKo);
            InitializeWebViewAsync();

            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape) Close();
            };
        }

        private void InitializeUI(bool isKo)
        {
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                Padding = new Padding(12, 6, 12, 6),
                BackColor = ThemeManager.Colors.CardBackground
            };

            lblStatus = new Label
            {
                Text = (isKo ? "페이지 로딩 중... " : "Loading page... ") + targetUrl,
                Font = new Font("Segoe UI", 9f),
                ForeColor = ThemeManager.Colors.TextSecondary,
                AutoSize = false,
                Width = 780,
                Height = 30,
                Location = new Point(14, 10),
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnClose = new ModernButton
            {
                Text = isKo ? "닫기 (Esc)" : "Close (Esc)",
                Style = ModernButtonStyle.Secondary,
                Width = 100,
                Height = 30,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Location = new Point(896, 7),
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => Close();

            header.Controls.Add(lblStatus);
            header.Controls.Add(btnClose);
            Controls.Add(header);

            ThemeManager.SetFormTitleBarTheme(Handle, ThemeManager.IsLightTheme);
        }

        private async void InitializeWebViewAsync()
        {
            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(webView);
            webView.BringToFront();

            try
            {
                string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LibraryScreensaver_Data");
                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView.EnsureCoreWebView2Async(env);

                if (webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.IsMuted = isMuted;

                    webView.NavigationCompleted += (s, e) =>
                    {
                        try
                        {
                            webView.ZoomFactor = zoomFactor;
                            lblStatus.Text = e.IsSuccess 
                                ? $"✔ {targetUrl} ({(int)(zoomFactor * 100)}% 배율)"
                                : $"⚠ 로드 실패: {e.WebErrorStatus}";
                        }
                        catch { }
                    };

                    webView.CoreWebView2.Navigate(targetUrl);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "WebView2 초기화 실패: " + ex.Message;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && webView != null)
            {
                webView.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
