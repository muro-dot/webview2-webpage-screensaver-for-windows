using System;
using System.Drawing;
using System.Windows.Forms;

namespace Web_Page_Screensaver
{
    public partial class PrefsByScreenUserControl : UserControl
    {
        private ListViewItem newlyAddedItem = null;
        private string currentLanguage = "en";
        private Action themeChangeHandler;

        public PrefsByScreenUserControl()
        {
            InitializeComponent();
            ApplyModernStyles();

            themeChangeHandler = () => ApplyTheme(ThemeManager.IsLightTheme);
            ThemeManager.ThemeChanged += themeChangeHandler;

            ApplyTheme(ThemeManager.IsLightTheme);
        }

        public void ApplyTheme(bool isLight)
        {
            var colors = ThemeManager.Colors;

            BackColor = colors.CardBackground;

            // URL 목록 영역
            listCard.BackColor = colors.InputBackground;
            listCard.BorderColor = colors.CardBorder;
            lvUrls.BackColor = colors.InputBackground;
            lvUrls.ForeColor = colors.TextPrimary;

            // 하단 회전 주기 옵션 카드
            optionsCard.BackColor = colors.CardBackground;
            optionsCard.BorderColor = colors.CardBorder;
            nudRotationInterval.BackColor = colors.InputBackground;
            nudRotationInterval.ForeColor = colors.TextPrimary;

            // 라벨 및 체크박스 텍스트 색상
            lblRotation.ForeColor = colors.TextPrimary;
            lblSeconds.ForeColor = colors.TextSecondary;
            lblZoom.ForeColor = colors.TextPrimary;
            cmbZoom.BackColor = colors.InputBackground;
            cmbZoom.ForeColor = colors.TextPrimary;
            cbRandomize.ForeColor = colors.TextPrimary;

            // 컨트롤들 다시 그리기
            listCard.Invalidate();
            optionsCard.Invalidate();
            btnAddUrl.Invalidate();
            btnPresets.Invalidate();
            btnUp.Invalidate();
            btnDown.Invalidate();
            btnEdit.Invalidate();
            btnPreview.Invalidate();
            btnDelete.Invalidate();
            cbRandomize.Invalidate();
        }

        private void ApplyModernStyles()
        {
            // ListView 컬럼 폭 자동 조절
            if (lvUrls.Columns.Count > 0)
            {
                lvUrls.Columns[0].Width = Math.Max(200, lvUrls.ClientSize.Width - 10);
            }

            lvUrls.Resize += (s, e) =>
            {
                if (lvUrls.Columns.Count > 0)
                {
                    lvUrls.Columns[0].Width = Math.Max(200, lvUrls.ClientSize.Width - 10);
                }
            };

            // 콤보박스 기본 선택 (100%)
            if (cmbZoom.SelectedIndex < 0)
            {
                cmbZoom.SelectedIndex = 1; // 100%
            }
        }

        public void ApplyLanguage(string lang)
        {
            currentLanguage = lang;
            bool isKo = (lang == "ko");

            btnAddUrl.Text = isKo ? "＋ URL 추가" : "＋ Add URL";
            btnPresets.Text = isKo ? "★ 프리셋" : "★ Presets";
            btnUp.Text = isKo ? "▲ 위로" : "▲ Move Up";
            btnDown.Text = isKo ? "▼ 아래로" : "▼ Move Down";
            btnEdit.Text = isKo ? "✎ 수정" : "✎ Edit";
            btnPreview.Text = isKo ? "👁 미리보기" : "👁 Preview";
            btnDelete.Text = isKo ? "✕ 삭제" : "✕ Delete";

            lblRotation.Text = isKo ? "전환 주기:" : "Rotate every:";
            lblSeconds.Text = isKo ? "초" : "sec";
            lblZoom.Text = isKo ? "화면 배율:" : "Zoom:";
            cbRandomize.Text = isKo ? "무작위 순서 (Shuffle)" : "Shuffle order";

            urlButtonsTooltip.SetToolTip(btnUp, isKo ? "선택한 URL을 위로 이동합니다 (Alt+▲)" : "Move selected URL up (Alt+▲)");
            urlButtonsTooltip.SetToolTip(btnDown, isKo ? "선택한 URL을 아래로 이동합니다 (Alt+▼)" : "Move selected URL down (Alt+▼)");
            urlButtonsTooltip.SetToolTip(btnAddUrl, isKo ? "목록에 새 사이트 URL을 추가하고 인라인으로 편집합니다" : "Add a new URL and edit inline");
            urlButtonsTooltip.SetToolTip(btnPresets, isKo ? "추천 웹 화면보호기 프리셋 라이브러리를 열어 간편하게 추가합니다" : "Open and add recommended screensaver presets");
            urlButtonsTooltip.SetToolTip(btnEdit, isKo ? "선택한 URL을 목록에서 직접 수정합니다 (F2 / 더블클릭)" : "Edit selected URL directly in list (F2 / Double-click)");
            urlButtonsTooltip.SetToolTip(btnPreview, isKo ? "선택한 URL을 실시간 화면보호기 창으로 미리 봅니다" : "Preview selected URL in live screensaver window");
            urlButtonsTooltip.SetToolTip(btnDelete, isKo ? "선택한 URL을 삭제합니다 (Del)" : "Delete selected URLs (Del)");
        }

        #region URL 추가 / 수정 / 삭제 (인라인 편집 지원)

        /// <summary>
        /// 새 URL 항목을 목록 끝에 추가하고 즉시 인라인 편집 모드로 진입합니다.
        /// </summary>
        private void btnAddUrl_Click(object sender, EventArgs e)
        {
            var item = new ListViewItem("https://");
            lvUrls.Items.Add(item);

            // 포커스 및 선택
            foreach (ListViewItem old in lvUrls.SelectedItems)
            {
                old.Selected = false;
            }
            item.Selected = true;
            item.Focused = true;
            item.EnsureVisible();

            newlyAddedItem = item;

            // UI 스레드 디스패치로 안전하게 인라인 편집 시작
            BeginInvoke((MethodInvoker)(() =>
            {
                if (item != null && item.ListView != null && !item.ListView.IsDisposed)
                {
                    item.BeginEdit();
                }
            }));
        }

        /// <summary>
        /// 선택된 항목의 인라인 편집을 시작합니다.
        /// </summary>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            StartEditSelected();
        }

        private void lvUrls_DoubleClick(object sender, EventArgs e)
        {
            StartEditSelected();
        }

        private void StartEditSelected()
        {
            if (lvUrls.SelectedItems.Count > 0)
            {
                var item = lvUrls.SelectedItems[0];
                BeginInvoke((MethodInvoker)(() =>
                {
                    if (item != null && item.ListView != null && !item.ListView.IsDisposed)
                    {
                        item.BeginEdit();
                    }
                }));
            }
        }

        /// <summary>
        /// 인라인 편집 완료 또는 취소 시 처리 (프로토콜 보정 및 빈 값 정리)
        /// </summary>
        private void lvUrls_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            var item = lvUrls.Items[e.Item];
            string editedText = e.Label;

            // 사용자가 수정을 취소했거나 아무것도 입력하지 않은 경우
            if (editedText == null)
            {
                // 새로 추가된 행인데 취소되었거나 기본 템플릿 그대로인 경우 자동 제거
                if (newlyAddedItem == item && (string.IsNullOrWhiteSpace(item.Text) || item.Text == "https://"))
                {
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        item.Remove();
                    }));
                }
                newlyAddedItem = null;
                return;
            }

            editedText = editedText.Trim();

            // 빈 값이 입력된 경우
            if (string.IsNullOrEmpty(editedText) || editedText == "https://" || editedText == "http://")
            {
                e.CancelEdit = true;
                if (newlyAddedItem == item)
                {
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        item.Remove();
                    }));
                }
                newlyAddedItem = null;
                return;
            }

            // 프로토콜(https://) 자동 보정
            if (!editedText.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !editedText.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
                !editedText.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
            {
                editedText = "https://" + editedText;
            }

            e.CancelEdit = true; // 프레임워크 기본 대입 대신 보정된 텍스트 직접 할당
            item.Text = editedText;
            newlyAddedItem = null;
        }

        /// <summary>
        /// 키보드 단축키 (F2: 수정, Del: 삭제, Alt+Up/Down: 순서 이동)
        /// </summary>
        private void lvUrls_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                StartEditSelected();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
                DeleteAllSelectedUrls_Click(sender, e);
            }
            else if (e.Alt && e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                MoveAllSelectedUrlsUp_Click(sender, e);
            }
            else if (e.Alt && e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                MoveAllSelectedUrlsDown_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Insert)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                btnAddUrl_Click(sender, e);
            }
        }

        private void DeleteAllSelectedUrls_Click(object sender, EventArgs e)
        {
            for (int i = lvUrls.Items.Count - 1; i >= 0; i--)
            {
                if (lvUrls.Items[i].Selected)
                {
                    lvUrls.Items[i].Remove();
                }
            }
        }

        #endregion

        #region 순서 이동 로직

        private void MoveAllSelectedUrlsDown_Click(object sender, EventArgs e)
        {
            bool gapFound = false;

            for (int i = lvUrls.Items.Count - 1; i >= 0; i--)
            {
                if (lvUrls.Items[i].Selected)
                {
                    if (gapFound)
                    {
                        Swap(lvUrls.Items, i, i + 1);
                    }
                }
                else
                {
                    gapFound = true;
                }
            }

            lvUrls.Select();
        }

        private void MoveAllSelectedUrlsUp_Click(object sender, EventArgs e)
        {
            bool gapFound = false;

            for (int i = 0; i < lvUrls.Items.Count; i++)
            {
                if (lvUrls.Items[i].Selected)
                {
                    if (gapFound)
                    {
                        Swap(lvUrls.Items, i, i - 1);
                    }
                }
                else
                {
                    gapFound = true;
                }
            }

            lvUrls.Select();
        }

        private static void Swap(ListView.ListViewItemCollection itemsList, int indexA, int indexB)
        {
            var a = Math.Min(itemsList.Count - 1, Math.Max(0, indexA));
            var b = Math.Min(itemsList.Count - 1, Math.Max(0, indexB));
            if (a != b)
            {
                var itemA = (ListViewItem)itemsList[a].Clone();
                bool itemASelected = itemsList[a].Selected;
                var itemB = (ListViewItem)itemsList[b].Clone();
                bool itemBSelected = itemsList[b].Selected;
                itemsList[a] = itemB;
                itemsList[a].Selected = itemBSelected;
                itemsList[b] = itemA;
                itemsList[b].Selected = itemASelected;
            }
        }

        #endregion

        #region 추천 프리셋 선택 및 실시간 미리보기

        /// <summary>
        /// 추천 웹 화면보호기 프리셋 팝업을 열어 선택된 URL을 현재 목록에 추가합니다.
        /// </summary>
        private void btnPresets_Click(object sender, EventArgs e)
        {
            var urls = PresetsManager.ShowPresetSelector(this.FindForm(), currentLanguage);
            if (urls != null && urls.Count > 0)
            {
                foreach (var u in urls)
                {
                    var item = new ListViewItem(u);
                    lvUrls.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// 목록에서 선택된 URL을 실제 화면보호기 모달 창으로 즉시 미리 봅니다.
        /// </summary>
        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (lvUrls.SelectedItems.Count > 0)
            {
                string rawUrl = lvUrls.SelectedItems[0].Text;
                var parsed = ScreensaverUrlItem.Parse(rawUrl);
                if (!string.IsNullOrWhiteSpace(parsed.Url))
                {
                    double zoom = 1.0;
                    if (cmbZoom.SelectedItem != null)
                    {
                        string zoomStr = cmbZoom.SelectedItem.ToString().TrimEnd('%');
                        if (double.TryParse(zoomStr, out double z)) zoom = z / 100.0;
                    }

                    using (var dlg = new PreviewDialog(parsed.Url, zoom, true, currentLanguage))
                    {
                        dlg.ShowDialog(this.FindForm());
                    }
                }
            }
            else
            {
                bool isKo = (currentLanguage == "ko");
                MessageBox.Show(
                    isKo ? "미리 볼 URL을 목록에서 먼저 선택해 주세요." : "Please select a URL from the list to preview.",
                    isKo ? "안내" : "Notice",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        #endregion
    }
}
