namespace Vocola
{
    partial class OptionsWindow
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsWindow));
            this.chkCommandSequencesWsr = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCommandFolderPath = new System.Windows.Forms.TextBox();
            this.btnBrowseCommandFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtCustomUsingSet = new System.Windows.Forms.TextBox();
            this.radUsingCustom = new System.Windows.Forms.RadioButton();
            this.radUsingVocola2 = new System.Windows.Forms.RadioButton();
            this.radUsingVocola3 = new System.Windows.Forms.RadioButton();
            this.btnBrowseExtensionFolder = new System.Windows.Forms.Button();
            this.txtExtensionFolderPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkRequireControlNamePrefix = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lnkHelpBuiltinCommands = new System.Windows.Forms.LinkLabel();
            this.dgvBuiltinCommands = new System.Windows.Forms.DataGridView();
            this.chkDisableWsrDictationScratchpad = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pnlRecognizerWsr = new System.Windows.Forms.Panel();
            this.numMaxSequencedCommandsWsr = new System.Windows.Forms.NumericUpDown();
            this.pnlRecognizerDns = new System.Windows.Forms.Panel();
            this.btnBrowseNatLinkFolder = new System.Windows.Forms.Button();
            this.txtNatLinkFolderPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numMaxSequencedCommandsDns = new System.Windows.Forms.NumericUpDown();
            this.chkCommandSequencesDns = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.radRecognizerWsr = new System.Windows.Forms.RadioButton();
            this.radRecognizerDns = new System.Windows.Forms.RadioButton();
            this.Include = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBuiltinCommands)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.pnlRecognizerWsr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxSequencedCommandsWsr)).BeginInit();
            this.pnlRecognizerDns.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxSequencedCommandsDns)).BeginInit();
            this.SuspendLayout();
            // 
            // chkCommandSequencesWsr
            // 
            this.chkCommandSequencesWsr.AutoSize = true;
            this.chkCommandSequencesWsr.Location = new System.Drawing.Point(30, 45);
            this.chkCommandSequencesWsr.Margin = new System.Windows.Forms.Padding(2);
            this.chkCommandSequencesWsr.Name = "chkCommandSequencesWsr";
            this.chkCommandSequencesWsr.Size = new System.Drawing.Size(163, 17);
            this.chkCommandSequencesWsr.TabIndex = 11;
            this.chkCommandSequencesWsr.Text = "Enable command &sequences";
            this.chkCommandSequencesWsr.UseVisualStyleBackColor = true;
            this.chkCommandSequencesWsr.CheckedChanged += new System.EventHandler(this.chkCommandSequencesWsr_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Command &folder:";
            // 
            // txtCommandFolderPath
            // 
            this.txtCommandFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCommandFolderPath.Location = new System.Drawing.Point(9, 24);
            this.txtCommandFolderPath.Margin = new System.Windows.Forms.Padding(2);
            this.txtCommandFolderPath.Name = "txtCommandFolderPath";
            this.txtCommandFolderPath.Size = new System.Drawing.Size(314, 20);
            this.txtCommandFolderPath.TabIndex = 1;
            // 
            // btnBrowseCommandFolder
            // 
            this.btnBrowseCommandFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseCommandFolder.Location = new System.Drawing.Point(327, 23);
            this.btnBrowseCommandFolder.Margin = new System.Windows.Forms.Padding(2);
            this.btnBrowseCommandFolder.Name = "btnBrowseCommandFolder";
            this.btnBrowseCommandFolder.Size = new System.Drawing.Size(60, 22);
            this.btnBrowseCommandFolder.TabIndex = 2;
            this.btnBrowseCommandFolder.Text = "&Browse...";
            this.btnBrowseCommandFolder.UseVisualStyleBackColor = true;
            this.btnBrowseCommandFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(60, 64);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "&Maximum number:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(331, 577);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 19);
            this.btnCancel.TabIndex = 102;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(270, 577);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(56, 19);
            this.btnOK.TabIndex = 101;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtCustomUsingSet);
            this.groupBox1.Controls.Add(this.radUsingCustom);
            this.groupBox1.Controls.Add(this.radUsingVocola2);
            this.groupBox1.Controls.Add(this.radUsingVocola3);
            this.groupBox1.Location = new System.Drawing.Point(9, 321);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(378, 96);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Base $using set";
            // 
            // txtCustomUsingSet
            // 
            this.txtCustomUsingSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomUsingSet.Location = new System.Drawing.Point(84, 64);
            this.txtCustomUsingSet.Margin = new System.Windows.Forms.Padding(2);
            this.txtCustomUsingSet.Name = "txtCustomUsingSet";
            this.txtCustomUsingSet.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCustomUsingSet.Size = new System.Drawing.Size(286, 20);
            this.txtCustomUsingSet.TabIndex = 10;
            // 
            // radUsingCustom
            // 
            this.radUsingCustom.AutoSize = true;
            this.radUsingCustom.Location = new System.Drawing.Point(11, 65);
            this.radUsingCustom.Margin = new System.Windows.Forms.Padding(2);
            this.radUsingCustom.Name = "radUsingCustom";
            this.radUsingCustom.Size = new System.Drawing.Size(69, 17);
            this.radUsingCustom.TabIndex = 9;
            this.radUsingCustom.TabStop = true;
            this.radUsingCustom.Text = "&Custom --";
            this.radUsingCustom.UseVisualStyleBackColor = true;
            this.radUsingCustom.CheckedChanged += new System.EventHandler(this.radUsing_CheckedChanged);
            // 
            // radUsingVocola2
            // 
            this.radUsingVocola2.AutoSize = true;
            this.radUsingVocola2.Location = new System.Drawing.Point(11, 43);
            this.radUsingVocola2.Margin = new System.Windows.Forms.Padding(2);
            this.radUsingVocola2.Name = "radUsingVocola2";
            this.radUsingVocola2.Size = new System.Drawing.Size(255, 17);
            this.radUsingVocola2.TabIndex = 8;
            this.radUsingVocola2.TabStop = true;
            this.radUsingVocola2.Text = "Vocola &2 default -- Library, Library.DragonLegacy";
            this.radUsingVocola2.UseVisualStyleBackColor = true;
            this.radUsingVocola2.CheckedChanged += new System.EventHandler(this.radUsing_CheckedChanged);
            // 
            // radUsingVocola3
            // 
            this.radUsingVocola3.AutoSize = true;
            this.radUsingVocola3.Location = new System.Drawing.Point(11, 21);
            this.radUsingVocola3.Margin = new System.Windows.Forms.Padding(2);
            this.radUsingVocola3.Name = "radUsingVocola3";
            this.radUsingVocola3.Size = new System.Drawing.Size(281, 17);
            this.radUsingVocola3.TabIndex = 7;
            this.radUsingVocola3.TabStop = true;
            this.radUsingVocola3.Text = "Vocola &3 default -- Library, Library.Main, Library.Pointer";
            this.radUsingVocola3.UseVisualStyleBackColor = true;
            this.radUsingVocola3.CheckedChanged += new System.EventHandler(this.radUsing_CheckedChanged);
            // 
            // btnBrowseExtensionFolder
            // 
            this.btnBrowseExtensionFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseExtensionFolder.Location = new System.Drawing.Point(327, 61);
            this.btnBrowseExtensionFolder.Margin = new System.Windows.Forms.Padding(2);
            this.btnBrowseExtensionFolder.Name = "btnBrowseExtensionFolder";
            this.btnBrowseExtensionFolder.Size = new System.Drawing.Size(60, 22);
            this.btnBrowseExtensionFolder.TabIndex = 5;
            this.btnBrowseExtensionFolder.Text = "Br&owse...";
            this.btnBrowseExtensionFolder.UseVisualStyleBackColor = true;
            this.btnBrowseExtensionFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
            // 
            // txtExtensionFolderPath
            // 
            this.txtExtensionFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExtensionFolderPath.Location = new System.Drawing.Point(9, 62);
            this.txtExtensionFolderPath.Margin = new System.Windows.Forms.Padding(2);
            this.txtExtensionFolderPath.Name = "txtExtensionFolderPath";
            this.txtExtensionFolderPath.Size = new System.Drawing.Size(314, 20);
            this.txtExtensionFolderPath.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 46);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "E&xtensions folder:";
            // 
            // chkRequireControlNamePrefix
            // 
            this.chkRequireControlNamePrefix.AutoSize = true;
            this.chkRequireControlNamePrefix.Location = new System.Drawing.Point(30, 24);
            this.chkRequireControlNamePrefix.Margin = new System.Windows.Forms.Padding(2);
            this.chkRequireControlNamePrefix.Name = "chkRequireControlNamePrefix";
            this.chkRequireControlNamePrefix.Size = new System.Drawing.Size(273, 17);
            this.chkRequireControlNamePrefix.TabIndex = 16;
            this.chkRequireControlNamePrefix.Text = "&Require saying \"Click\" before name of clickable item";
            this.chkRequireControlNamePrefix.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lnkHelpBuiltinCommands);
            this.groupBox2.Controls.Add(this.dgvBuiltinCommands);
            this.groupBox2.Location = new System.Drawing.Point(10, 421);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(377, 149);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "&Enable built-in commands";
            // 
            // lnkHelpBuiltinCommands
            // 
            this.lnkHelpBuiltinCommands.AutoSize = true;
            this.lnkHelpBuiltinCommands.Location = new System.Drawing.Point(341, -1);
            this.lnkHelpBuiltinCommands.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnkHelpBuiltinCommands.Name = "lnkHelpBuiltinCommands";
            this.lnkHelpBuiltinCommands.Size = new System.Drawing.Size(29, 13);
            this.lnkHelpBuiltinCommands.TabIndex = 19;
            this.lnkHelpBuiltinCommands.TabStop = true;
            this.lnkHelpBuiltinCommands.Text = "Help";
            this.lnkHelpBuiltinCommands.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHelpBuiltinCommands_LinkClicked);
            // 
            // dgvBuiltinCommands
            // 
            this.dgvBuiltinCommands.AllowUserToAddRows = false;
            this.dgvBuiltinCommands.AllowUserToDeleteRows = false;
            this.dgvBuiltinCommands.AllowUserToResizeColumns = false;
            this.dgvBuiltinCommands.AllowUserToResizeRows = false;
            this.dgvBuiltinCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBuiltinCommands.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvBuiltinCommands.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBuiltinCommands.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvBuiltinCommands.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBuiltinCommands.ColumnHeadersVisible = false;
            this.dgvBuiltinCommands.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Include,
            this.Description});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvBuiltinCommands.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvBuiltinCommands.GridColor = System.Drawing.SystemColors.Control;
            this.dgvBuiltinCommands.Location = new System.Drawing.Point(6, 17);
            this.dgvBuiltinCommands.Margin = new System.Windows.Forms.Padding(2);
            this.dgvBuiltinCommands.Name = "dgvBuiltinCommands";
            this.dgvBuiltinCommands.RowHeadersVisible = false;
            this.dgvBuiltinCommands.RowTemplate.Height = 24;
            this.dgvBuiltinCommands.Size = new System.Drawing.Size(365, 126);
            this.dgvBuiltinCommands.TabIndex = 18;
            // 
            // chkDisableWsrDictationScratchpad
            // 
            this.chkDisableWsrDictationScratchpad.AutoSize = true;
            this.chkDisableWsrDictationScratchpad.Location = new System.Drawing.Point(30, 3);
            this.chkDisableWsrDictationScratchpad.Margin = new System.Windows.Forms.Padding(2);
            this.chkDisableWsrDictationScratchpad.Name = "chkDisableWsrDictationScratchpad";
            this.chkDisableWsrDictationScratchpad.Size = new System.Drawing.Size(327, 17);
            this.chkDisableWsrDictationScratchpad.TabIndex = 103;
            this.chkDisableWsrDictationScratchpad.Text = "Enable Vocola &dictation, by disabling WSR dictation scratchpad";
            this.chkDisableWsrDictationScratchpad.UseVisualStyleBackColor = true;
            this.chkDisableWsrDictationScratchpad.CheckedChanged += new System.EventHandler(this.chkDisableWsrDictationScratchpad_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.pnlRecognizerWsr);
            this.groupBox3.Controls.Add(this.pnlRecognizerDns);
            this.groupBox3.Controls.Add(this.radRecognizerWsr);
            this.groupBox3.Controls.Add(this.radRecognizerDns);
            this.groupBox3.Location = new System.Drawing.Point(9, 87);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(378, 229);
            this.groupBox3.TabIndex = 104;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Speech Recognizer";
            // 
            // pnlRecognizerWsr
            // 
            this.pnlRecognizerWsr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlRecognizerWsr.BackColor = System.Drawing.Color.Transparent;
            this.pnlRecognizerWsr.Controls.Add(this.numMaxSequencedCommandsWsr);
            this.pnlRecognizerWsr.Controls.Add(this.chkDisableWsrDictationScratchpad);
            this.pnlRecognizerWsr.Controls.Add(this.chkCommandSequencesWsr);
            this.pnlRecognizerWsr.Controls.Add(this.chkRequireControlNamePrefix);
            this.pnlRecognizerWsr.Controls.Add(this.label1);
            this.pnlRecognizerWsr.Location = new System.Drawing.Point(0, 140);
            this.pnlRecognizerWsr.Name = "pnlRecognizerWsr";
            this.pnlRecognizerWsr.Size = new System.Drawing.Size(378, 85);
            this.pnlRecognizerWsr.TabIndex = 112;
            // 
            // numMaxSequencedCommandsWsr
            // 
            this.numMaxSequencedCommandsWsr.Location = new System.Drawing.Point(157, 62);
            this.numMaxSequencedCommandsWsr.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numMaxSequencedCommandsWsr.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxSequencedCommandsWsr.Name = "numMaxSequencedCommandsWsr";
            this.numMaxSequencedCommandsWsr.Size = new System.Drawing.Size(36, 20);
            this.numMaxSequencedCommandsWsr.TabIndex = 104;
            this.numMaxSequencedCommandsWsr.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // pnlRecognizerDns
            // 
            this.pnlRecognizerDns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlRecognizerDns.BackColor = System.Drawing.Color.Transparent;
            this.pnlRecognizerDns.Controls.Add(this.btnBrowseNatLinkFolder);
            this.pnlRecognizerDns.Controls.Add(this.txtNatLinkFolderPath);
            this.pnlRecognizerDns.Controls.Add(this.label5);
            this.pnlRecognizerDns.Controls.Add(this.numMaxSequencedCommandsDns);
            this.pnlRecognizerDns.Controls.Add(this.chkCommandSequencesDns);
            this.pnlRecognizerDns.Controls.Add(this.label4);
            this.pnlRecognizerDns.Location = new System.Drawing.Point(0, 36);
            this.pnlRecognizerDns.Name = "pnlRecognizerDns";
            this.pnlRecognizerDns.Size = new System.Drawing.Size(378, 82);
            this.pnlRecognizerDns.TabIndex = 111;
            // 
            // btnBrowseNatLinkFolder
            // 
            this.btnBrowseNatLinkFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseNatLinkFolder.Location = new System.Drawing.Point(316, 17);
            this.btnBrowseNatLinkFolder.Margin = new System.Windows.Forms.Padding(2);
            this.btnBrowseNatLinkFolder.Name = "btnBrowseNatLinkFolder";
            this.btnBrowseNatLinkFolder.Size = new System.Drawing.Size(60, 22);
            this.btnBrowseNatLinkFolder.TabIndex = 110;
            this.btnBrowseNatLinkFolder.Text = "&Browse...";
            this.btnBrowseNatLinkFolder.UseVisualStyleBackColor = true;
            this.btnBrowseNatLinkFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
            // 
            // txtNatLinkFolderPath
            // 
            this.txtNatLinkFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNatLinkFolderPath.Location = new System.Drawing.Point(30, 18);
            this.txtNatLinkFolderPath.Margin = new System.Windows.Forms.Padding(2);
            this.txtNatLinkFolderPath.Name = "txtNatLinkFolderPath";
            this.txtNatLinkFolderPath.Size = new System.Drawing.Size(280, 20);
            this.txtNatLinkFolderPath.TabIndex = 109;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 3);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(132, 13);
            this.label5.TabIndex = 108;
            this.label5.Text = "NatLink &Installation Folder:";
            // 
            // numMaxSequencedCommandsDns
            // 
            this.numMaxSequencedCommandsDns.Location = new System.Drawing.Point(159, 59);
            this.numMaxSequencedCommandsDns.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numMaxSequencedCommandsDns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxSequencedCommandsDns.Name = "numMaxSequencedCommandsDns";
            this.numMaxSequencedCommandsDns.Size = new System.Drawing.Size(36, 20);
            this.numMaxSequencedCommandsDns.TabIndex = 107;
            this.numMaxSequencedCommandsDns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkCommandSequencesDns
            // 
            this.chkCommandSequencesDns.AutoSize = true;
            this.chkCommandSequencesDns.Location = new System.Drawing.Point(30, 42);
            this.chkCommandSequencesDns.Margin = new System.Windows.Forms.Padding(2);
            this.chkCommandSequencesDns.Name = "chkCommandSequencesDns";
            this.chkCommandSequencesDns.Size = new System.Drawing.Size(163, 17);
            this.chkCommandSequencesDns.TabIndex = 105;
            this.chkCommandSequencesDns.Text = "Enable command &sequences";
            this.chkCommandSequencesDns.UseVisualStyleBackColor = true;
            this.chkCommandSequencesDns.CheckedChanged += new System.EventHandler(this.chkCommandSequencesDns_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(60, 61);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 106;
            this.label4.Text = "&Maximum number:";
            // 
            // radRecognizerWsr
            // 
            this.radRecognizerWsr.AutoSize = true;
            this.radRecognizerWsr.Location = new System.Drawing.Point(11, 121);
            this.radRecognizerWsr.Name = "radRecognizerWsr";
            this.radRecognizerWsr.Size = new System.Drawing.Size(204, 17);
            this.radRecognizerWsr.TabIndex = 1;
            this.radRecognizerWsr.TabStop = true;
            this.radRecognizerWsr.Text = "&Windows Speech Recognition (WSR)";
            this.radRecognizerWsr.UseVisualStyleBackColor = true;
            this.radRecognizerWsr.CheckedChanged += new System.EventHandler(this.radRecognizer_CheckedChanged);
            // 
            // radRecognizerDns
            // 
            this.radRecognizerDns.AutoSize = true;
            this.radRecognizerDns.Location = new System.Drawing.Point(11, 19);
            this.radRecognizerDns.Name = "radRecognizerDns";
            this.radRecognizerDns.Size = new System.Drawing.Size(246, 17);
            this.radRecognizerDns.TabIndex = 0;
            this.radRecognizerDns.TabStop = true;
            this.radRecognizerDns.Text = "Dragon &NaturallySpeaking (DNS), with NatLink";
            this.radRecognizerDns.UseVisualStyleBackColor = true;
            this.radRecognizerDns.CheckedChanged += new System.EventHandler(this.radRecognizer_CheckedChanged);
            // 
            // Include
            // 
            this.Include.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Include.DataPropertyName = "Include";
            this.Include.FillWeight = 1F;
            this.Include.HeaderText = "";
            this.Include.MinimumWidth = 25;
            this.Include.Name = "Include";
            // 
            // Description
            // 
            this.Description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Description.DataPropertyName = "Description";
            this.Description.FillWeight = 99F;
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            // 
            // OptionsWindow
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(396, 603);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnBrowseExtensionFolder);
            this.Controls.Add(this.txtExtensionFolderPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBrowseCommandFolder);
            this.Controls.Add(this.txtCommandFolderPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.Options_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBuiltinCommands)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.pnlRecognizerWsr.ResumeLayout(false);
            this.pnlRecognizerWsr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxSequencedCommandsWsr)).EndInit();
            this.pnlRecognizerDns.ResumeLayout(false);
            this.pnlRecognizerDns.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxSequencedCommandsDns)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkCommandSequencesWsr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCommandFolderPath;
        private System.Windows.Forms.Button btnBrowseCommandFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtCustomUsingSet;
        private System.Windows.Forms.RadioButton radUsingCustom;
        private System.Windows.Forms.RadioButton radUsingVocola2;
        private System.Windows.Forms.RadioButton radUsingVocola3;
        private System.Windows.Forms.Button btnBrowseExtensionFolder;
        private System.Windows.Forms.TextBox txtExtensionFolderPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkRequireControlNamePrefix;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.LinkLabel lnkHelpBuiltinCommands;
        private System.Windows.Forms.CheckBox chkDisableWsrDictationScratchpad;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radRecognizerWsr;
        private System.Windows.Forms.RadioButton radRecognizerDns;
        private System.Windows.Forms.Button btnBrowseNatLinkFolder;
        private System.Windows.Forms.TextBox txtNatLinkFolderPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numMaxSequencedCommandsDns;
        private System.Windows.Forms.CheckBox chkCommandSequencesDns;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numMaxSequencedCommandsWsr;
        private System.Windows.Forms.Panel pnlRecognizerWsr;
        private System.Windows.Forms.Panel pnlRecognizerDns;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Include;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridView dgvBuiltinCommands;
    }
}
