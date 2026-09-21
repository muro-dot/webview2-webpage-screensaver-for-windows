using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Web_Page_Screensaver
{
    /// <summary>
    /// 추천 웹 화면보호기 프리셋 아이템 모델
    /// </summary>
    public class PresetItem
    {
        public string TitleKo { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionKo { get; set; }
        public string DescriptionEn { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }

        public string GetTitle(string lang) => lang == "ko" ? TitleKo : TitleEn;
        public string GetDescription(string lang) => lang == "ko" ? DescriptionKo : DescriptionEn;
    }

    /// <summary>
    /// 추천 프리셋 라이브러리 및 다이얼로그 제공자
    /// </summary>
    public static class PresetsManager
    {
        public static readonly List<PresetItem> Presets = new List<PresetItem>
        {
            new PresetItem
            {
                TitleKo = "🕒 Flip Clock (플립 시계)",
                TitleEn = "🕒 Minimal Flip Clock",
                DescriptionKo = "군더더기 없는 깔끔하고 감성적인 레트로 플립 디지털 시계",
                DescriptionEn = "Clean and aesthetic retro flip digital clock",
                Url = "https://flipclock.me/",
                Category = "Clock"
            },
            new PresetItem
            {
                TitleKo = "🌧️ Drive & Listen (도시 드라이브)",
                TitleEn = "🌧️ Drive & Listen",
                DescriptionKo = "세계 주요 도시의 실제 도로 드라이브 영상과 현지 라디오",
                DescriptionEn = "Dashcam drive videos through world cities with local radio",
                Url = "https://driveandlisten.herokuapp.com/",
                Category = "Ambient"
            },
            new PresetItem
            {
                TitleKo = "💻 Matrix Digital Rain (매트릭스)",
                TitleEn = "💻 Matrix Digital Rain",
                DescriptionKo = "영화 매트릭스 스타일의 3D 초록빛 디지털 코드 낙하 효과",
                DescriptionEn = "Cinematic 3D falling green code rain from The Matrix",
                Url = "https://rezmason.github.io/matrix/",
                Category = "Visual"
            },
            new PresetItem
            {
                TitleKo = "🪐 NASA 오늘의 천체 사진 (APOD)",
                TitleEn = "🪐 NASA Astronomy Picture of the Day",
                DescriptionKo = "NASA에서 매일 선정하는 신비롭고 웅장한 우주 천체 고화질 사진",
                DescriptionEn = "Daily breathtaking high-resolution astronomy photographs by NASA",
                Url = "https://apod.nasa.gov/apod/astropix.html",
                Category = "Space"
            },
            new PresetItem
            {
                TitleKo = "📊 Google Trends (실시간 검색어)",
                TitleEn = "📊 Google Trends Visualizer",
                DescriptionKo = "실시간 구글 인기 검색 트렌드를 인터랙티브 타일로 출력",
                DescriptionEn = "Interactive colorful tiles showing real-time trending Google searches",
                Url = "https://trends.google.com/trends/hottrends/visualize?nrow=5&ncol=5",
                Category = "Dashboard"
            },
            new PresetItem
            {
                TitleKo = "📈 Coin360 (암호화폐 히트맵)",
                TitleEn = "📈 Coin360 Crypto Heatmap",
                DescriptionKo = "비트코인, 이더리움 등 가상자산 시세를 한눈에 보는 실시간 히트맵",
                DescriptionEn = "Visual real-time interactive market heatmap of cryptocurrencies",
                Url = "https://coin360.com/",
                Category = "Finance"
            },
            new PresetItem
            {
                TitleKo = "🌌 Solar System 3D (태양계 뷰어)",
                TitleEn = "🌌 Solar System Scope 3D",
                DescriptionKo = "실시간 행성 궤도와 별자리를 탐색하는 정밀 3D 천문 시뮬레이션",
                DescriptionEn = "Interactive 3D real-time simulation of solar system and planets",
                Url = "https://www.solarsystemscope.com/iframe/",
                Category = "Space"
            },
            new PresetItem
            {
                TitleKo = "🌏 EarthCam Live (글로벌 라이브캠)",
                TitleEn = "🌏 EarthCam Live Cams",
                DescriptionKo = "뉴욕 타임스퀘어, 에펠탑 등 전 세계 랜드마크 실시간 라이브캠",
                DescriptionEn = "Live streaming webcams from top iconic landmarks worldwide",
                Url = "https://www.earthcam.com/",
                Category = "Ambient"
            }
        };

        /// <summary>
        /// 프리셋 선택 팝업 다이얼로그를 표시하고 선택된 URL 목록을 반환합니다.
        /// </summary>
        public static List<string> ShowPresetSelector(IWin32Window owner, string language)
        {
            using (var dialog = new PresetSelectionForm(language))
            {
                if (dialog.ShowDialog(owner) == DialogResult.OK)
                {
                    return dialog.SelectedUrls;
                }
            }
            return new List<string>();
        }
    }

    /// <summary>
    /// 모던 다크/라이트 테마가 적용된 프리셋 선택 팝업 다이얼로그
    /// </summary>
    public class PresetSelectionForm : Form
    {
        private string language;
        private ListView lvPresets;
        private ModernButton btnAdd;
        private ModernButton btnCancel;
        private Label lblHeader;
        private Label lblSub;

        public List<string> SelectedUrls { get; private set; } = new List<string>();

        public PresetSelectionForm(string lang)
        {
            language = lang ?? "ko";
            InitializePresetUI();
            ApplyTheme(ThemeManager.IsLightTheme);
        }

        private void InitializePresetUI()
        {
            bool isKo = (language == "ko");
            Text = isKo ? "추천 웹 화면보호기 프리셋 라이브러리" : "Recommended Web Screensaver Presets";
            ClientSize = new Size(740, 520);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;

            var panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 68,
                Padding = new Padding(20, 12, 20, 0)
            };

            lblHeader = new Label
            {
                Text = isKo ? "★ 추천 웹 화면보호기 둘러보기" : "★ Explore Recommended Screensavers",
                Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(18, 12)
            };

            lblSub = new Label
            {
                Text = isKo 
                    ? "원하는 프리셋을 체크하고 [목록에 추가]를 누르거나, 항목을 더블클릭하세요." 
                    : "Check presets and click [Add to List], or double-click any item to add.",
                Font = new Font("Segoe UI", 8.8f),
                ForeColor = Color.FromArgb(161, 161, 170),
                AutoSize = true,
                Location = new Point(20, 38)
            };
            panelHeader.Controls.Add(lblHeader);
            panelHeader.Controls.Add(lblSub);

            lvPresets = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                Font = new Font("Segoe UI", 9.5f),
                BorderStyle = BorderStyle.None
            };
            lvPresets.Columns.Add(isKo ? "프리셋 명칭" : "Preset Name", 250);
            lvPresets.Columns.Add(isKo ? "설명" : "Description", 440);

            foreach (var p in PresetsManager.Presets)
            {
                var lvi = new ListViewItem(p.GetTitle(language));
                lvi.SubItems.Add(p.GetDescription(language));
                lvi.Tag = p.Url;
                lvPresets.Items.Add(lvi);
            }

            // 더블클릭 시 즉시 해당 프리셋 추가
            lvPresets.DoubleClick += (s, e) =>
            {
                if (lvPresets.SelectedItems.Count > 0 && lvPresets.SelectedItems[0].Tag != null)
                {
                    SelectedUrls.Clear();
                    SelectedUrls.Add(lvPresets.SelectedItems[0].Tag.ToString());
                    DialogResult = DialogResult.OK;
                    Close();
                }
            };

            var panelBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(16, 12, 16, 12)
            };

            btnCancel = new ModernButton
            {
                Text = isKo ? "닫기" : "Close",
                Style = ModernButtonStyle.Secondary,
                Width = 84,
                Height = 36,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Location = new Point(510 - 84 - 10, 12),
                Cursor = Cursors.Hand
            };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            btnAdd = new ModernButton
            {
                Text = isKo ? "＋ 선택한 프리셋 목록에 추가" : "＋ Add Selected to List",
                Style = ModernButtonStyle.Primary,
                Width = 210,
                Height = 36,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Location = new Point(740 - 210 - 20, 12),
                Cursor = Cursors.Hand
            };
            btnAdd.Click += (s, e) =>
            {
                SelectedUrls.Clear();
                foreach (ListViewItem item in lvPresets.Items)
                {
                    if (item.Checked && item.Tag != null)
                    {
                        SelectedUrls.Add(item.Tag.ToString());
                    }
                }
                if (SelectedUrls.Count == 0 && lvPresets.SelectedItems.Count > 0)
                {
                    SelectedUrls.Add(lvPresets.SelectedItems[0].Tag.ToString());
                }

                if (SelectedUrls.Count == 0)
                {
                    MessageBox.Show(
                        isKo ? "추가할 프리셋을 최소 1개 이상 체크해 주세요." : "Please select at least one preset to add.",
                        isKo ? "안내" : "Notice",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                DialogResult = DialogResult.OK;
                Close();
            };

            panelBottom.Controls.Add(btnCancel);
            panelBottom.Controls.Add(btnAdd);

            var listContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 4, 20, 4)
            };
            listContainer.Controls.Add(lvPresets);

            // WinForms Docking: panelHeader(Top)와 panelBottom(Bottom)이 가장 먼저 공간을 확보하도록 하고,
            // 중앙 영역을 채우는 listContainer(Fill)는 반드시 SendToBack() 처리해야 하단 패널이 가려지지 않습니다.
            Controls.Add(listContainer);
            Controls.Add(panelBottom);
            Controls.Add(panelHeader);

            panelHeader.BringToFront();
            panelBottom.BringToFront();
            listContainer.SendToBack();
        }

        private void ApplyTheme(bool isLight)
        {
            var colors = ThemeManager.Colors;
            BackColor = colors.Background;
            ForeColor = colors.TextPrimary;
            lblHeader.ForeColor = colors.TextPrimary;

            lvPresets.BackColor = colors.InputBackground;
            lvPresets.ForeColor = colors.TextPrimary;

            ThemeManager.SetFormTitleBarTheme(Handle, isLight);
        }
    }
}
