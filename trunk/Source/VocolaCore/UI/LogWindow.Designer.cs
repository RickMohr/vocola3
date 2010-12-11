namespace Vocola
{
    partial class LogWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogWindow));
            this.btnClear = new System.Windows.Forms.Button();
            this.ddlLogLevel = new System.Windows.Forms.ComboBox();
            this.lblLogLevel = new System.Windows.Forms.Label();
            this.TheLogBox = new System.Windows.Forms.WebBrowser();
            this.chkAutoScroll = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(395, 10);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 32);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "&Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // ddlLogLevel
            // 
            this.ddlLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ddlLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlLogLevel.FormattingEnabled = true;
            this.ddlLogLevel.Items.AddRange(new object[] {
            "Errors Only",
            "Commands and Dictation",
            "Activity Details",
            "Full Trace"});
            this.ddlLogLevel.Location = new System.Drawing.Point(93, 15);
            this.ddlLogLevel.Name = "ddlLogLevel";
            this.ddlLogLevel.Size = new System.Drawing.Size(179, 24);
            this.ddlLogLevel.TabIndex = 2;
            this.ddlLogLevel.SelectedIndexChanged += new System.EventHandler(this.ddlLogLevel_SelectedIndexChanged);
            // 
            // lblLogLevel
            // 
            this.lblLogLevel.AutoSize = true;
            this.lblLogLevel.Location = new System.Drawing.Point(13, 18);
            this.lblLogLevel.Name = "lblLogLevel";
            this.lblLogLevel.Size = new System.Drawing.Size(74, 17);
            this.lblLogLevel.TabIndex = 1;
            this.lblLogLevel.Text = "&Log Level:";
            // 
            // TheLogBox
            // 
            this.TheLogBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TheLogBox.Location = new System.Drawing.Point(1, 50);
            this.TheLogBox.MinimumSize = new System.Drawing.Size(20, 20);
            this.TheLogBox.Name = "TheLogBox";
            this.TheLogBox.Size = new System.Drawing.Size(479, 557);
            this.TheLogBox.TabIndex = 0;
            // 
            // chkAutoScroll
            // 
            this.chkAutoScroll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoScroll.AutoSize = true;
            this.chkAutoScroll.Checked = true;
            this.chkAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoScroll.Location = new System.Drawing.Point(291, 17);
            this.chkAutoScroll.Name = "chkAutoScroll";
            this.chkAutoScroll.Size = new System.Drawing.Size(97, 21);
            this.chkAutoScroll.TabIndex = 3;
            this.chkAutoScroll.Text = "&Auto-scroll";
            this.chkAutoScroll.UseVisualStyleBackColor = true;
            // 
            // LogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 609);
            this.Controls.Add(this.chkAutoScroll);
            this.Controls.Add(this.TheLogBox);
            this.Controls.Add(this.lblLogLevel);
            this.Controls.Add(this.ddlLogLevel);
            this.Controls.Add(this.btnClear);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(500, 43);
            this.Name = "LogWindow";
            this.Text = "Vocola Log";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ComboBox ddlLogLevel;
        private System.Windows.Forms.Label lblLogLevel;
        private System.Windows.Forms.WebBrowser TheLogBox;
        private System.Windows.Forms.CheckBox chkAutoScroll;
    }
}
