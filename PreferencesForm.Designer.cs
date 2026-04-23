namespace Web_Page_Screensaver
{
    using global::Web_Page_Screensaver;

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
            this.label1 = new System.Windows.Forms.Label();
            this.llProjectLocationUrl = new System.Windows.Forms.LinkLabel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.cbCloseOnActivity = new System.Windows.Forms.CheckBox();
            this.screenTabControl = new System.Windows.Forms.TabControl();
            this.screenTabPage1 = new System.Windows.Forms.TabPage();
            this.prefsByScreenUserControl1 = new Web_Page_Screensaver.PrefsByScreenUserControl();
            this.spanScreensButton = new System.Windows.Forms.RadioButton();
            this.separateScreensButton = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mirrorScreensButton = new System.Windows.Forms.RadioButton();
            this.multiScreenGroup = new System.Windows.Forms.GroupBox();
            this.screenModeTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.screenTabControl.SuspendLayout();
            this.screenTabPage1.SuspendLayout();
            this.multiScreenGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(101, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Web Page Screensaver";
            // 
            // llProjectLocationUrl
            // 
            this.llProjectLocationUrl.AutoSize = true;
            this.llProjectLocationUrl.Location = new System.Drawing.Point(29, 29);
            this.llProjectLocationUrl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.llProjectLocationUrl.Name = "llProjectLocationUrl";
            this.llProjectLocationUrl.Size = new System.Drawing.Size(430, 15);
            this.llProjectLocationUrl.TabIndex = 1;
            this.llProjectLocationUrl.TabStop = true;
            this.llProjectLocationUrl.Text = "https://github.com/muro-dot/Webview2_WebPage_Screensaver";
            this.llProjectLocationUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llProjectLocationUrl_LinkClicked);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(454, 487);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 27);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(562, 487);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 27);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // cbCloseOnActivity
            // 
            this.cbCloseOnActivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbCloseOnActivity.AutoSize = true;
            this.cbCloseOnActivity.Checked = true;
            this.cbCloseOnActivity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCloseOnActivity.Location = new System.Drawing.Point(20, 460);
            this.cbCloseOnActivity.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbCloseOnActivity.Name = "cbCloseOnActivity";
            this.cbCloseOnActivity.Size = new System.Drawing.Size(210, 19);
            this.cbCloseOnActivity.TabIndex = 6;
            this.cbCloseOnActivity.Text = "Close on mouse movement";
            this.cbCloseOnActivity.UseVisualStyleBackColor = true;
            // 
            // screenTabControl
            // 
            this.screenTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.screenTabControl.Controls.Add(this.screenTabPage1);
            this.screenTabControl.Location = new System.Drawing.Point(20, 112);
            this.screenTabControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.screenTabControl.Name = "screenTabControl";
            this.screenTabControl.SelectedIndex = 0;
            this.screenTabControl.Size = new System.Drawing.Size(646, 328);
            this.screenTabControl.TabIndex = 13;
            // 
            // screenTabPage1
            // 
            this.screenTabPage1.Controls.Add(this.prefsByScreenUserControl1);
            this.screenTabPage1.Location = new System.Drawing.Point(4, 25);
            this.screenTabPage1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.screenTabPage1.Name = "screenTabPage1";
            this.screenTabPage1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.screenTabPage1.Size = new System.Drawing.Size(638, 299);
            this.screenTabPage1.TabIndex = 0;
            this.screenTabPage1.Text = "Screen 1";
            this.screenTabPage1.UseVisualStyleBackColor = true;
            // 
            // prefsByScreenUserControl1
            // 
            this.prefsByScreenUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prefsByScreenUserControl1.BackColor = System.Drawing.Color.White;
            this.prefsByScreenUserControl1.Location = new System.Drawing.Point(0, 0);
            this.prefsByScreenUserControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.prefsByScreenUserControl1.Name = "prefsByScreenUserControl1";
            this.prefsByScreenUserControl1.Size = new System.Drawing.Size(639, 303);
            this.prefsByScreenUserControl1.TabIndex = 21;
            this.prefsByScreenUserControl1.Load += new System.EventHandler(this.prefsByScreenUserControl1_Load);
            // 
            // spanScreensButton
            // 
            this.spanScreensButton.AutoSize = true;
            this.spanScreensButton.Checked = true;
            this.spanScreensButton.Location = new System.Drawing.Point(100, 9);
            this.spanScreensButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.spanScreensButton.Name = "spanScreensButton";
            this.spanScreensButton.Size = new System.Drawing.Size(62, 19);
            this.spanScreensButton.TabIndex = 14;
            this.spanScreensButton.TabStop = true;
            this.spanScreensButton.Tag = "MultiScreenMode";
            this.spanScreensButton.Text = "Span";
            this.screenModeTooltip.SetToolTip(this.spanScreensButton, "All for One!");
            this.spanScreensButton.UseVisualStyleBackColor = true;
            this.spanScreensButton.Click += new System.EventHandler(this.anyMultiScreenModeButton_Click);
            // 
            // separateScreensButton
            // 
            this.separateScreensButton.AutoSize = true;
            this.separateScreensButton.Location = new System.Drawing.Point(237, 9);
            this.separateScreensButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.separateScreensButton.Name = "separateScreensButton";
            this.separateScreensButton.Size = new System.Drawing.Size(86, 19);
            this.separateScreensButton.TabIndex = 15;
            this.separateScreensButton.TabStop = true;
            this.separateScreensButton.Tag = "MultiScreenMode";
            this.separateScreensButton.Text = "Separate";
            this.screenModeTooltip.SetToolTip(this.separateScreensButton, "Each to their own.");
            this.separateScreensButton.UseVisualStyleBackColor = true;
            this.separateScreensButton.Click += new System.EventHandler(this.anyMultiScreenModeButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 12);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 15);
            this.label4.TabIndex = 16;
            this.label4.Text = "Multiscreen:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 91);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Website URLs";
            // 
            // mirrorScreensButton
            // 
            this.mirrorScreensButton.AutoSize = true;
            this.mirrorScreensButton.Location = new System.Drawing.Point(168, 9);
            this.mirrorScreensButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.mirrorScreensButton.Name = "mirrorScreensButton";
            this.mirrorScreensButton.Size = new System.Drawing.Size(64, 19);
            this.mirrorScreensButton.TabIndex = 17;
            this.mirrorScreensButton.TabStop = true;
            this.mirrorScreensButton.Tag = "MultiScreenMode";
            this.mirrorScreensButton.Text = "Mirror";
            this.screenModeTooltip.SetToolTip(this.mirrorScreensButton, "One for All!");
            this.mirrorScreensButton.UseVisualStyleBackColor = true;
            this.mirrorScreensButton.Click += new System.EventHandler(this.anyMultiScreenModeButton_Click);
            // 
            // multiScreenGroup
            // 
            this.multiScreenGroup.Controls.Add(this.label4);
            this.multiScreenGroup.Controls.Add(this.mirrorScreensButton);
            this.multiScreenGroup.Controls.Add(this.spanScreensButton);
            this.multiScreenGroup.Controls.Add(this.separateScreensButton);
            this.multiScreenGroup.Location = new System.Drawing.Point(20, 53);
            this.multiScreenGroup.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.multiScreenGroup.Name = "multiScreenGroup";
            this.multiScreenGroup.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.multiScreenGroup.Size = new System.Drawing.Size(325, 32);
            this.multiScreenGroup.TabIndex = 18;
            this.multiScreenGroup.TabStop = false;
            // 
            // PreferencesForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(690, 522);
            this.Controls.Add(this.multiScreenGroup);
            this.Controls.Add(this.screenTabControl);
            this.Controls.Add(this.cbCloseOnActivity);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.llProjectLocationUrl);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(378, 402);
            this.Name = "PreferencesForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Web Page Screensaver Settings";
            this.Load += new System.EventHandler(this.PreferencesForm_Load);
            this.screenTabControl.ResumeLayout(false);
            this.screenTabPage1.ResumeLayout(false);
            this.multiScreenGroup.ResumeLayout(false);
            this.multiScreenGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel llProjectLocationUrl;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox cbCloseOnActivity;
        private System.Windows.Forms.TabControl screenTabControl;
        private System.Windows.Forms.TabPage screenTabPage1;
        private System.Windows.Forms.RadioButton spanScreensButton;
        private System.Windows.Forms.RadioButton separateScreensButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton mirrorScreensButton;
        private System.Windows.Forms.GroupBox multiScreenGroup;
        private PrefsByScreenUserControl prefsByScreenUserControl1;
        private System.Windows.Forms.ToolTip screenModeTooltip;
    }
}