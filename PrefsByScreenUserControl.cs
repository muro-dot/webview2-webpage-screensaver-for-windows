using System;
using System.Drawing;
using System.Windows.Forms;

namespace Web_Page_Screensaver
{
    public partial class PrefsByScreenUserControl : UserControl
    {
        private ListViewItem editingItem = null;
        private string currentLanguage = "en";

        public PrefsByScreenUserControl()
        {
            InitializeComponent();
            ApplyModernStyles();
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
        }

        public void ApplyLanguage(string lang)
        {
            currentLanguage = lang;
            bool isKo = (lang == "ko");

            if (editingItem == null)
            {
                btnAddUrl.Text = isKo ? "+ URL 추가" : "+ Add URL";
            }
            else
            {
                btnAddUrl.Text = isKo ? "✓ 수정 완료" : "✓ Update";
            }

            btnUp.Text = isKo ? "▲ 위로" : "▲ Move Up";
            btnDown.Text = isKo ? "▼ 아래로" : "▼ Move Down";
            btnEdit.Text = isKo ? "✎ 수정" : "✎ Edit";
            btnDelete.Text = isKo ? "삭제" : "Delete";

            lblRotation.Text = isKo ? "웹사이트 전환 주기:" : "Rotate website every:";
            lblSeconds.Text = isKo ? "초" : "seconds";
            cbRandomize.Text = isKo ? "무작위 순서 재생 (Shuffle)" : "Shuffle display order";

            urlButtonsTooltip.SetToolTip(btnUp, isKo ? "선택한 URL을 위로 이동합니다" : "Move selected URL up");
            urlButtonsTooltip.SetToolTip(btnDown, isKo ? "선택한 URL을 아래로 이동합니다" : "Move selected URL down");
            urlButtonsTooltip.SetToolTip(btnEdit, isKo ? "선택한 URL을 수정합니다" : "Edit selected URL");
            urlButtonsTooltip.SetToolTip(btnDelete, isKo ? "선택한 URL을 삭제합니다" : "Delete selected URLs");
        }

        private void tbNewUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                HandleAddOrUpdate();
            }
            else if (e.KeyCode == Keys.Escape && editingItem != null)
            {
                // Escape로 수정 취소
                CancelEdit();
            }
        }

        private void btnAddUrl_Click(object sender, EventArgs e)
        {
            HandleAddOrUpdate();
        }

        private void HandleAddOrUpdate()
        {
            string url = tbNewUrl.Text.Trim();

            if (string.IsNullOrEmpty(url))
            {
                if (editingItem != null)
                {
                    CancelEdit();
                }
                return;
            }

            // 프로토콜 보정
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
            {
                url = "https://" + url;
            }

            if (editingItem != null)
            {
                // 수정 완료
                editingItem.Text = url;
                editingItem.Selected = true;
                editingItem.EnsureVisible();
                CancelEdit();
            }
            else
            {
                // 신규 추가
                var item = new ListViewItem(url);
                lvUrls.Items.Add(item);
                item.Selected = true;
                item.EnsureVisible();
                tbNewUrl.Clear();
                tbNewUrl.Focus();
            }
        }

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
                editingItem = lvUrls.SelectedItems[0];
                tbNewUrl.Text = editingItem.Text;
                tbNewUrl.Focus();
                tbNewUrl.SelectAll();

                bool isKo = (currentLanguage == "ko");
                btnAddUrl.Text = isKo ? "✓ 수정 완료" : "✓ Update";
            }
        }

        private void CancelEdit()
        {
            editingItem = null;
            tbNewUrl.Clear();
            bool isKo = (currentLanguage == "ko");
            btnAddUrl.Text = isKo ? "+ URL 추가" : "+ Add URL";
        }

        private void lvUrls_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 다른 항목 선택 시 수정 중이던 항목 취소
            if (editingItem != null && (lvUrls.SelectedItems.Count == 0 || lvUrls.SelectedItems[0] != editingItem))
            {
                CancelEdit();
            }
        }

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

        private void DeleteAllSelectedUrls_Click(object sender, EventArgs e)
        {
            if (editingItem != null && editingItem.Selected)
            {
                CancelEdit();
            }

            for (int i = lvUrls.Items.Count - 1; i >= 0; i--)
            {
                if (lvUrls.Items[i].Selected)
                {
                    lvUrls.Items[i].Remove();
                }
            }
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
    }
}
