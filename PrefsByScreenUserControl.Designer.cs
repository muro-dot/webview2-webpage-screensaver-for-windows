namespace Web_Page_Screensaver
{
    partial class PrefsByScreenUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listCard = new Web_Page_Screensaver.ModernCard();
            this.lvUrls = new System.Windows.Forms.ListView();
            this.chUrl = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.inputCard = new Web_Page_Screensaver.ModernCard();
            this.tbNewUrl = new System.Windows.Forms.TextBox();
            this.btnAddUrl = new Web_Page_Screensaver.ModernButton();
            this.btnUp = new Web_Page_Screensaver.ModernButton();
            this.btnDown = new Web_Page_Screensaver.ModernButton();
            this.btnEdit = new Web_Page_Screensaver.ModernButton();
            this.btnDelete = new Web_Page_Screensaver.ModernButton();
            this.optionsCard = new Web_Page_Screensaver.ModernCard();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.nudRotationInterval = new System.Windows.Forms.NumericUpDown();
            this.lblRotation = new System.Windows.Forms.Label();
            this.cbRandomize = new System.Windows.Forms.CheckBox();
            this.urlButtonsTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.listCard.SuspendLayout();
            this.inputCard.SuspendLayout();
            this.optionsCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRotationInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // listCard
            // 
            this.listCard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(17)))));
            this.listCard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(56)))));
            this.listCard.BorderRadius = 6;
            this.listCard.Controls.Add(this.lvUrls);
            this.listCard.Location = new System.Drawing.Point(0, 0);
            this.listCard.Name = "listCard";
            this.listCard.Padding = new System.Windows.Forms.Padding(6);
            this.listCard.Size = new System.Drawing.Size(680, 182);
            this.listCard.TabIndex = 0;
            // 
            // lvUrls
            // 
            this.lvUrls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(17)))));
            this.lvUrls.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvUrls.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chUrl});
            this.lvUrls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvUrls.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lvUrls.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(245)))));
            this.lvUrls.FullRowSelect = true;
            this.lvUrls.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvUrls.HideSelection = false;
            this.lvUrls.LabelEdit = true;
            this.lvUrls.Location = new System.Drawing.Point(6, 6);
            this.lvUrls.Name = "lvUrls";
            this.lvUrls.Size = new System.Drawing.Size(668, 170);
            this.lvUrls.TabIndex = 0;
            this.lvUrls.UseCompatibleStateImageBehavior = false;
            this.lvUrls.View = System.Windows.Forms.View.Details;
            this.lvUrls.SelectedIndexChanged += new System.EventHandler(this.lvUrls_SelectedIndexChanged);
            this.lvUrls.DoubleClick += new System.EventHandler(this.lvUrls_DoubleClick);
            // 
            // chUrl
            // 
            this.chUrl.Text = "URL";
            this.chUrl.Width = 650;
            // 
            // inputCard
            // 
            this.inputCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(17)))));
            this.inputCard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(56)))));
            this.inputCard.BorderRadius = 6;
            this.inputCard.Controls.Add(this.tbNewUrl);
            this.inputCard.Location = new System.Drawing.Point(0, 192);
            this.inputCard.Name = "inputCard";
            this.inputCard.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.inputCard.Size = new System.Drawing.Size(564, 32);
            this.inputCard.TabIndex = 1;
            // 
            // tbNewUrl
            // 
            this.tbNewUrl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(17)))));
            this.tbNewUrl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbNewUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbNewUrl.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tbNewUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(245)))));
            this.tbNewUrl.Location = new System.Drawing.Point(8, 6);
            this.tbNewUrl.Name = "tbNewUrl";
            this.tbNewUrl.Size = new System.Drawing.Size(548, 18);
            this.tbNewUrl.TabIndex = 0;
            this.tbNewUrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbNewUrl_KeyDown);
            // 
            // btnAddUrl
            // 
            this.btnAddUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddUrl.BackColor = System.Drawing.Color.Transparent;
            this.btnAddUrl.BorderRadius = 6;
            this.btnAddUrl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddUrl.FlatAppearance.BorderSize = 0;
            this.btnAddUrl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddUrl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAddUrl.Location = new System.Drawing.Point(572, 192);
            this.btnAddUrl.Name = "btnAddUrl";
            this.btnAddUrl.Size = new System.Drawing.Size(108, 32);
            this.btnAddUrl.Style = Web_Page_Screensaver.ModernButtonStyle.Primary;
            this.btnAddUrl.TabIndex = 2;
            this.btnAddUrl.Text = "+ Add URL";
            this.btnAddUrl.UseVisualStyleBackColor = false;
            this.btnAddUrl.Click += new System.EventHandler(this.btnAddUrl_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUp.BackColor = System.Drawing.Color.Transparent;
            this.btnUp.BorderRadius = 6;
            this.btnUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUp.FlatAppearance.BorderSize = 0;
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnUp.Location = new System.Drawing.Point(0, 232);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(96, 30);
            this.btnUp.Style = Web_Page_Screensaver.ModernButtonStyle.Secondary;
            this.btnUp.TabIndex = 3;
            this.btnUp.Text = "▲ Move Up";
            this.urlButtonsTooltip.SetToolTip(this.btnUp, "Move selected URL up");
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.Click += new System.EventHandler(this.MoveAllSelectedUrlsUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDown.BackColor = System.Drawing.Color.Transparent;
            this.btnDown.BorderRadius = 6;
            this.btnDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDown.FlatAppearance.BorderSize = 0;
            this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDown.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDown.Location = new System.Drawing.Point(104, 232);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(108, 30);
            this.btnDown.Style = Web_Page_Screensaver.ModernButtonStyle.Secondary;
            this.btnDown.TabIndex = 4;
            this.btnDown.Text = "▼ Move Down";
            this.urlButtonsTooltip.SetToolTip(this.btnDown, "Move selected URL down");
            this.btnDown.UseVisualStyleBackColor = false;
            this.btnDown.Click += new System.EventHandler(this.MoveAllSelectedUrlsDown_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.BackColor = System.Drawing.Color.Transparent;
            this.btnEdit.BorderRadius = 6;
            this.btnEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEdit.FlatAppearance.BorderSize = 0;
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnEdit.Location = new System.Drawing.Point(484, 232);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(92, 30);
            this.btnEdit.Style = Web_Page_Screensaver.ModernButtonStyle.Secondary;
            this.btnEdit.TabIndex = 5;
            this.btnEdit.Text = "✎ Edit";
            this.urlButtonsTooltip.SetToolTip(this.btnEdit, "Edit selected URL");
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.BackColor = System.Drawing.Color.Transparent;
            this.btnDelete.BorderRadius = 6;
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDelete.Location = new System.Drawing.Point(584, 232);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(96, 30);
            this.btnDelete.Style = Web_Page_Screensaver.ModernButtonStyle.Danger;
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "Delete";
            this.urlButtonsTooltip.SetToolTip(this.btnDelete, "Delete selected URLs");
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.DeleteAllSelectedUrls_Click);
            // 
            // optionsCard
            // 
            this.optionsCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.optionsCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(28)))));
            this.optionsCard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(56)))));
            this.optionsCard.BorderRadius = 6;
            this.optionsCard.Controls.Add(this.lblSeconds);
            this.optionsCard.Controls.Add(this.nudRotationInterval);
            this.optionsCard.Controls.Add(this.lblRotation);
            this.optionsCard.Controls.Add(this.cbRandomize);
            this.optionsCard.Location = new System.Drawing.Point(0, 270);
            this.optionsCard.Name = "optionsCard";
            this.optionsCard.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.optionsCard.Size = new System.Drawing.Size(680, 42);
            this.optionsCard.TabIndex = 7;
            // 
            // lblSeconds
            // 
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSeconds.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(161)))), ((int)(((byte)(170)))));
            this.lblSeconds.Location = new System.Drawing.Point(232, 13);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(50, 15);
            this.lblSeconds.TabIndex = 3;
            this.lblSeconds.Text = "seconds";
            // 
            // nudRotationInterval
            // 
            this.nudRotationInterval.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(17)))));
            this.nudRotationInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudRotationInterval.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.nudRotationInterval.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(245)))));
            this.nudRotationInterval.Location = new System.Drawing.Point(165, 9);
            this.nudRotationInterval.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudRotationInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRotationInterval.Name = "nudRotationInterval";
            this.nudRotationInterval.Size = new System.Drawing.Size(60, 24);
            this.nudRotationInterval.TabIndex = 2;
            this.nudRotationInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudRotationInterval.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // lblRotation
            // 
            this.lblRotation.AutoSize = true;
            this.lblRotation.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblRotation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(245)))));
            this.lblRotation.Location = new System.Drawing.Point(12, 13);
            this.lblRotation.Name = "lblRotation";
            this.lblRotation.Size = new System.Drawing.Size(147, 15);
            this.lblRotation.TabIndex = 1;
            this.lblRotation.Text = "Rotate website every:";
            // 
            // cbRandomize
            // 
            this.cbRandomize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbRandomize.AutoSize = true;
            this.cbRandomize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbRandomize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbRandomize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(245)))));
            this.cbRandomize.Location = new System.Drawing.Point(490, 12);
            this.cbRandomize.Name = "cbRandomize";
            this.cbRandomize.Size = new System.Drawing.Size(168, 19);
            this.cbRandomize.TabIndex = 0;
            this.cbRandomize.Text = "Shuffle display order";
            this.cbRandomize.UseVisualStyleBackColor = true;
            // 
            // PrefsByScreenUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(32)))));
            this.Controls.Add(this.optionsCard);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnAddUrl);
            this.Controls.Add(this.inputCard);
            this.Controls.Add(this.listCard);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "PrefsByScreenUserControl";
            this.Size = new System.Drawing.Size(680, 316);
            this.listCard.ResumeLayout(false);
            this.inputCard.ResumeLayout(false);
            this.inputCard.PerformLayout();
            this.optionsCard.ResumeLayout(false);
            this.optionsCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRotationInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ModernCard listCard;
        public System.Windows.Forms.ListView lvUrls;
        private System.Windows.Forms.ColumnHeader chUrl;
        private ModernCard inputCard;
        private System.Windows.Forms.TextBox tbNewUrl;
        private ModernButton btnAddUrl;
        private ModernButton btnUp;
        private ModernButton btnDown;
        private ModernButton btnEdit;
        private ModernButton btnDelete;
        private ModernCard optionsCard;
        private System.Windows.Forms.Label lblRotation;
        public System.Windows.Forms.NumericUpDown nudRotationInterval;
        private System.Windows.Forms.Label lblSeconds;
        public System.Windows.Forms.CheckBox cbRandomize;
        private System.Windows.Forms.ToolTip urlButtonsTooltip;
    }
}
