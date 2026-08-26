namespace Web_Page_Screensaver
{
    partial class PreferencesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.langPanel = new System.Windows.Forms.Panel();
            this.btnLangKor = new Web_Page_Screensaver.ModernButton();
            this.btnLangEng = new Web_Page_Screensaver.ModernButton();
            this.btnGithub = new Web_Page_Screensaver.ModernButton();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.multiScreenCard = new Web_Page_Screensaver.ModernCard();
            this.separateScreensButton = new System.Windows.Forms.RadioButton();
            this.mirrorScreensButton = new System.Windows.Forms.RadioButton();
            this.spanScreensButton = new System.Windows.Forms.RadioButton();
            this.lblMultiScreen = new System.Windows.Forms.Label();
            this.screenTabControl = new Web_Page_Screensaver.ModernTabControl();
            this.screenTabPage1 = new System.Windows.Forms.TabPage();
            this.prefsByScreenUserControl1 = new Web_Page_Screensaver.PrefsByScreenUserControl();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.cbCloseOnActivity = new System.Windows.Forms.CheckBox();
            this.cancelButton = new Web_Page_Screensaver.ModernButton();
            this.okButton = new Web_Page_Screensaver.ModernButton();
            this.screenModeTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.headerPanel.SuspendLayout();
            this.langPanel.SuspendLayout();
            this.multiScreenCard.SuspendLayout();
            this.screenTabControl.SuspendLayout();
            this.screenTabPage1.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.headerPanel.BackColor = System.Drawing.Color.Transparent;
            this.headerPanel.Controls.Add(this.langPanel);
            this.headerPanel.Controls.Add(this.btnGithub);
            this.headerPanel.Controls.Add(this.lblSubtitle);
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Location = new System.Drawing.Point(24, 16);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(712, 56);
            this.headerPanel.TabIndex = 0;
            // 
            // langPanel
            // 
            this.langPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.langPanel.Controls.Add(this.btnLangKor);
            this.langPanel.Controls.Add(this.btnLangEng);
            this.langPanel.Location = new System.Drawing.Point(472, 11);
            this.langPanel.Name = "langPanel";
            this.langPanel.Size = new System.Drawing.Size(126, 30);
            this.langPanel.TabIndex = 3;
            // 
            // btnLangKor
            // 
            this.btnLangKor.BackColor = System.Drawing.Color.Transparent;
            this.btnLangKor.BorderRadius = 4;
            this.btnLangKor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLangKor.FlatAppearance.BorderSize = 0;
            this.btnLangKor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLangKor.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnLangKor.IsSelected = true;
            this.btnLangKor.Location = new System.Drawing.Point(0, 0);
            this.btnLangKor.Name = "btnLangKor";
            this.btnLangKor.Size = new System.Drawing.Size(60, 30);
            this.btnLangKor.Style = Web_Page_Screensaver.ModernButtonStyle.Segment;
            this.btnLangKor.TabIndex = 0;
            this.btnLangKor.Text = "한국어";
            this.btnLangKor.UseVisualStyleBackColor = false;
            this.btnLangKor.Click += new System.EventHandler(this.btnLangKor_Click);
            // 
            // btnLangEng
            // 
            this.btnLangEng.BackColor = System.Drawing.Color.Transparent;
            this.btnLangEng.BorderRadius = 4;
            this.btnLangEng.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLangEng.FlatAppearance.BorderSize = 0;
            this.btnLangEng.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLangEng.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnLangEng.IsSelected = false;
            this.btnLangEng.Location = new System.Drawing.Point(64, 0);
            this.btnLangEng.Name = "btnLangEng";
            this.btnLangEng.Size = new System.Drawing.Size(60, 30);
            this.btnLangEng.Style = Web_Page_Screensaver.ModernButtonStyle.Segment;
            this.btnLangEng.TabIndex = 1;
            this.btnLangEng.Text = "ENG";
            this.btnLangEng.UseVisualStyleBackColor = false;
            this.btnLangEng.Click += new System.EventHandler(this.btnLangEng_Click);
            // 
            // btnGithub
            // 
            this.btnGithub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGithub.BackColor = System.Drawing.Color.Transparent;
            this.btnGithub.BorderRadius = 6;
            this.btnGithub.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGithub.FlatAppearance.BorderSize = 0;
            this.btnGithub.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGithub.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnGithub.Location = new System.Drawing.Point(608, 11);
            this.btnGithub.Name = "btnGithub";
            this.btnGithub.Size = new System.Drawing.Size(102, 30);
            this.btnGithub.Style = Web_Page_Screensaver.ModernButtonStyle.Ghost;
            this.btnGithub.TabIndex = 2;
            this.btnGithub.Text = "GitHub ↗";
            this.btnGithub.UseVisualStyleBackColor = false;
            this.btnGithub.Click += new System.EventHandler(this.btnGithub_Click);
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(161)))), ((int)(((byte)(170)))));
            this.lblSubtitle.Location = new System.Drawing.Point(2, 30);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(325, 15);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Display websites and live dashboards with Microsoft WebView2";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(245)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 2);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(271, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "WebView2 Web Screensaver";
            // 
            // multiScreenCard
            // 
            this.multiScreenCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.multiScreenCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(32)))));
            this.multiScreenCard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(56)))));
            this.multiScreenCard.BorderRadius = 8;
            this.multiScreenCard.Controls.Add(this.separateScreensButton);
            this.multiScreenCard.Controls.Add(this.mirrorScreensButton);
            this.multiScreenCard.Controls.Add(this.spanScreensButton);
            this.multiScreenCard.Controls.Add(this.lblMultiScreen);
            this.multiScreenCard.Location = new System.Drawing.Point(24, 78);
            this.multiScreenCard.Name = "multiScreenCard";
            this.multiScreenCard.Padding = new System.Windows.Forms.Padding(12, 10, 12, 10);
            this.multiScreenCard.Size = new System.Drawing.Size(712, 44);
            this.multiScreenCard.TabIndex = 1;
            // 
            // separateScreensButton
            // 
            this.separateScreensButton.AutoSize = true;
            this.separateScreensButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.separateScreensButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.separateScreensButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(245)))));
            this.separateScreensButton.Location = new System.Drawing.Point(450, 12);
            this.separateScreensButton.Name = "separateScreensButton";
            this.separateScreensButton.Size = new System.Drawing.Size(161, 19);
            this.separateScreensButton.TabIndex = 3;
            this.separateScreensButton.Tag = "MultiScreenMode";
            this.separateScreensButton.Text = "Separate (Each its own list)";
            this.screenModeTooltip.SetToolTip(this.separateScreensButton, "Configure individual URL list for each screen");
            this.separateScreensButton.UseVisualStyleBackColor = true;
            this.separateScreensButton.Click += new System.EventHandler(this.anyMultiScreenModeButton_Click);
            // 
            // mirrorScreensButton
            // 
            this.mirrorScreensButton.AutoSize = true;
            this.mirrorScreensButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mirrorScreensButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.mirrorScreensButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(245)))));
            this.mirrorScreensButton.Location = new System.Drawing.Point(300, 12);
            this.mirrorScreensButton.Name = "mirrorScreensButton";
            this.mirrorScreensButton.Size = new System.Drawing.Size(95, 19);
            this.mirrorScreensButton.TabIndex = 2;
            this.mirrorScreensButton.Tag = "MultiScreenMode";
            this.mirrorScreensButton.Text = "Mirror (Clone)";
            this.screenModeTooltip.SetToolTip(this.mirrorScreensButton, "Same websites shown on all monitors");
            this.mirrorScreensButton.UseVisualStyleBackColor = true;
            this.mirrorScreensButton.Click += new System.EventHandler(this.anyMultiScreenModeButton_Click);
            // 
            // spanScreensButton
            // 
            this.spanScreensButton.AutoSize = true;
            this.spanScreensButton.Checked = true;
            this.spanScreensButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.spanScreensButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.spanScreensButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(245)))));
            this.spanScreensButton.Location = new System.Drawing.Point(155, 12);
            this.spanScreensButton.Name = "spanScreensButton";
            this.spanScreensButton.Size = new System.Drawing.Size(74, 19);
            this.spanScreensButton.TabIndex = 1;
            this.spanScreensButton.TabStop = true;
            this.spanScreensButton.Tag = "MultiScreenMode";
            this.spanScreensButton.Text = "Span (All)";
            this.screenModeTooltip.SetToolTip(this.spanScreensButton, "Spread a single screen across all monitors");
            this.spanScreensButton.UseVisualStyleBackColor = true;
            this.spanScreensButton.Click += new System.EventHandler(this.anyMultiScreenModeButton_Click);
            // 
            // lblMultiScreen
            // 
            this.lblMultiScreen.AutoSize = true;
            this.lblMultiScreen.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMultiScreen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(245)))));
            this.lblMultiScreen.Location = new System.Drawing.Point(12, 14);
            this.lblMultiScreen.Name = "lblMultiScreen";
            this.lblMultiScreen.Size = new System.Drawing.Size(120, 15);
            this.lblMultiScreen.TabIndex = 0;
            this.lblMultiScreen.Text = "Multi-Monitor Mode:";
            // 
            // screenTabControl
            // 
            this.screenTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.screenTabControl.Controls.Add(this.screenTabPage1);
            this.screenTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.screenTabControl.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.screenTabControl.ItemSize = new System.Drawing.Size(140, 36);
            this.screenTabControl.Location = new System.Drawing.Point(24, 132);
            this.screenTabControl.Name = "screenTabControl";
            this.screenTabControl.SelectedIndex = 0;
            this.screenTabControl.Size = new System.Drawing.Size(712, 376);
            this.screenTabControl.TabIndex = 2;
            // 
            // screenTabPage1
            // 
            this.screenTabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(32)))));
            this.screenTabPage1.Controls.Add(this.prefsByScreenUserControl1);
            this.screenTabPage1.Location = new System.Drawing.Point(4, 40);
            this.screenTabPage1.Name = "screenTabPage1";
            this.screenTabPage1.Padding = new System.Windows.Forms.Padding(12);
            this.screenTabPage1.Size = new System.Drawing.Size(704, 332);
            this.screenTabPage1.TabIndex = 0;
            this.screenTabPage1.Text = "Screen 1";
            // 
            // prefsByScreenUserControl1
            // 
            this.prefsByScreenUserControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(32)))));
            this.prefsByScreenUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prefsByScreenUserControl1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prefsByScreenUserControl1.Location = new System.Drawing.Point(12, 12);
            this.prefsByScreenUserControl1.Margin = new System.Windows.Forms.Padding(0);
            this.prefsByScreenUserControl1.Name = "prefsByScreenUserControl1";
            this.prefsByScreenUserControl1.Size = new System.Drawing.Size(680, 308);
            this.prefsByScreenUserControl1.TabIndex = 0;
            // 
            // bottomPanel
            // 
            this.bottomPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bottomPanel.BackColor = System.Drawing.Color.Transparent;
            this.bottomPanel.Controls.Add(this.cbCloseOnActivity);
            this.bottomPanel.Controls.Add(this.cancelButton);
            this.bottomPanel.Controls.Add(this.okButton);
            this.bottomPanel.Location = new System.Drawing.Point(24, 518);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(712, 46);
            this.bottomPanel.TabIndex = 3;
            // 
            // cbCloseOnActivity
            // 
            this.cbCloseOnActivity.AutoSize = true;
            this.cbCloseOnActivity.Checked = true;
            this.cbCloseOnActivity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCloseOnActivity.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbCloseOnActivity.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cbCloseOnActivity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(245)))));
            this.cbCloseOnActivity.Location = new System.Drawing.Point(4, 12);
            this.cbCloseOnActivity.Name = "cbCloseOnActivity";
            this.cbCloseOnActivity.Size = new System.Drawing.Size(262, 21);
            this.cbCloseOnActivity.TabIndex = 0;
            this.cbCloseOnActivity.Text = "Exit screensaver on mouse movement";
            this.cbCloseOnActivity.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.BackColor = System.Drawing.Color.Transparent;
            this.cancelButton.BorderRadius = 6;
            this.cancelButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatAppearance.BorderSize = 0;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cancelButton.Location = new System.Drawing.Point(498, 6);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(98, 33);
            this.cancelButton.Style = Web_Page_Screensaver.ModernButtonStyle.Secondary;
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.BackColor = System.Drawing.Color.Transparent;
            this.okButton.BorderRadius = 6;
            this.okButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.FlatAppearance.BorderSize = 0;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.okButton.Location = new System.Drawing.Point(604, 6);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(108, 33);
            this.okButton.Style = Web_Page_Screensaver.ModernButtonStyle.Primary;
            this.okButton.TabIndex = 2;
            this.okButton.Text = "Save & Apply";
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // PreferencesForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(23)))));
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(760, 580);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.screenTabControl);
            this.Controls.Add(this.multiScreenCard);
            this.Controls.Add(this.headerPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(245)))));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(650, 480);
            this.Name = "PreferencesForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WebView2 Web Page Screensaver Settings";
            this.Load += new System.EventHandler(this.PreferencesForm_Load);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.langPanel.ResumeLayout(false);
            this.multiScreenCard.ResumeLayout(false);
            this.multiScreenCard.PerformLayout();
            this.screenTabControl.ResumeLayout(false);
            this.screenTabPage1.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Panel langPanel;
        private ModernButton btnLangKor;
        private ModernButton btnLangEng;
        private ModernButton btnGithub;
        private ModernCard multiScreenCard;
        private System.Windows.Forms.Label lblMultiScreen;
        private System.Windows.Forms.RadioButton spanScreensButton;
        private System.Windows.Forms.RadioButton mirrorScreensButton;
        private System.Windows.Forms.RadioButton separateScreensButton;
        private ModernTabControl screenTabControl;
        private System.Windows.Forms.TabPage screenTabPage1;
        private PrefsByScreenUserControl prefsByScreenUserControl1;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.CheckBox cbCloseOnActivity;
        private ModernButton cancelButton;
        private ModernButton okButton;
        private System.Windows.Forms.ToolTip screenModeTooltip;
    }
}