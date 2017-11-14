namespace usbWriteLockTest
{
    partial class WlFormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlFormMain));
            this.grpDrives = new System.Windows.Forms.GroupBox();
            this.grdDevices = new System.Windows.Forms.DataGridView();
            this.btnCheckSum1 = new System.Windows.Forms.Button();
            this.grpVolumes = new System.Windows.Forms.GroupBox();
            this.grdVolumes = new System.Windows.Forms.DataGridView();
            this.grpBoxInfo = new System.Windows.Forms.GroupBox();
            this.grpSecondHash = new System.Windows.Forms.GroupBox();
            this.txtSecondHash = new System.Windows.Forms.TextBox();
            this.grpBoxHash1 = new System.Windows.Forms.GroupBox();
            this.txtFirstHash = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lblTotalTracks = new System.Windows.Forms.Label();
            this.txtTotalTracks = new System.Windows.Forms.TextBox();
            this.lblTotalSectors = new System.Windows.Forms.Label();
            this.txtTotalSectors = new System.Windows.Forms.TextBox();
            this.lblTotalHeads = new System.Windows.Forms.Label();
            this.txtTotalHeads = new System.Windows.Forms.TextBox();
            this.lblTotalSize = new System.Windows.Forms.Label();
            this.txtTotalSize = new System.Windows.Forms.TextBox();
            this.lblModel = new System.Windows.Forms.Label();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.lblDriveName = new System.Windows.Forms.Label();
            this.txtDriveName = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.lblPercentage = new System.Windows.Forms.Label();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.pictWorking = new System.Windows.Forms.PictureBox();
            this.btnPrepare = new System.Windows.Forms.Button();
            this.btnCancelOp = new System.Windows.Forms.Button();
            this.btnRunTests = new System.Windows.Forms.Button();
            this.grpOperations = new System.Windows.Forms.GroupBox();
            this.btnCleanup = new System.Windows.Forms.Button();
            this.btnWriteProtectionDisabled = new System.Windows.Forms.Button();
            this.btnSecondHash = new System.Windows.Forms.Button();
            this.btnWriteEnabled = new System.Windows.Forms.Button();
            this.grpDrives.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDevices)).BeginInit();
            this.grpVolumes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdVolumes)).BeginInit();
            this.grpBoxInfo.SuspendLayout();
            this.grpSecondHash.SuspendLayout();
            this.grpBoxHash1.SuspendLayout();
            this.grpStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictWorking)).BeginInit();
            this.grpOperations.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDrives
            // 
            this.grpDrives.Controls.Add(this.grdDevices);
            this.grpDrives.Location = new System.Drawing.Point(12, 12);
            this.grpDrives.Name = "grpDrives";
            this.grpDrives.Size = new System.Drawing.Size(494, 155);
            this.grpDrives.TabIndex = 0;
            this.grpDrives.TabStop = false;
            this.grpDrives.Text = "USB Devices";
            // 
            // grdDevices
            // 
            this.grdDevices.AllowUserToAddRows = false;
            this.grdDevices.AllowUserToDeleteRows = false;
            this.grdDevices.AllowUserToResizeRows = false;
            this.grdDevices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdDevices.BackgroundColor = System.Drawing.SystemColors.Window;
            this.grdDevices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdDevices.Location = new System.Drawing.Point(3, 16);
            this.grdDevices.MultiSelect = false;
            this.grdDevices.Name = "grdDevices";
            this.grdDevices.ReadOnly = true;
            this.grdDevices.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.grdDevices.RowHeadersVisible = false;
            this.grdDevices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdDevices.ShowEditingIcon = false;
            this.grdDevices.Size = new System.Drawing.Size(488, 136);
            this.grdDevices.TabIndex = 0;
            this.grdDevices.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdDevices_CellClick);
            this.grdDevices.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.grdDevices_CellFormatting);
            this.grdDevices.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.grdDevices_DataError);
            // 
            // btnCheckSum1
            // 
            this.btnCheckSum1.Location = new System.Drawing.Point(272, 19);
            this.btnCheckSum1.Name = "btnCheckSum1";
            this.btnCheckSum1.Size = new System.Drawing.Size(103, 23);
            this.btnCheckSum1.TabIndex = 1;
            this.btnCheckSum1.Text = "First checksum";
            this.btnCheckSum1.UseVisualStyleBackColor = true;
            this.btnCheckSum1.Click += new System.EventHandler(this.btnCheckSum1_Click);
            // 
            // grpVolumes
            // 
            this.grpVolumes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpVolumes.Controls.Add(this.grdVolumes);
            this.grpVolumes.Location = new System.Drawing.Point(10, 189);
            this.grpVolumes.Name = "grpVolumes";
            this.grpVolumes.Size = new System.Drawing.Size(504, 132);
            this.grpVolumes.TabIndex = 2;
            this.grpVolumes.TabStop = false;
            this.grpVolumes.Text = "Volumes";
            // 
            // grdVolumes
            // 
            this.grdVolumes.AllowUserToAddRows = false;
            this.grdVolumes.AllowUserToDeleteRows = false;
            this.grdVolumes.AllowUserToResizeRows = false;
            this.grdVolumes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdVolumes.BackgroundColor = System.Drawing.SystemColors.Window;
            this.grdVolumes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdVolumes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdVolumes.Location = new System.Drawing.Point(3, 16);
            this.grdVolumes.MultiSelect = false;
            this.grdVolumes.Name = "grdVolumes";
            this.grdVolumes.ReadOnly = true;
            this.grdVolumes.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.grdVolumes.RowHeadersVisible = false;
            this.grdVolumes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdVolumes.ShowEditingIcon = false;
            this.grdVolumes.Size = new System.Drawing.Size(498, 113);
            this.grdVolumes.TabIndex = 1;
            // 
            // grpBoxInfo
            // 
            this.grpBoxInfo.Controls.Add(this.grpSecondHash);
            this.grpBoxInfo.Controls.Add(this.grpBoxHash1);
            this.grpBoxInfo.Controls.Add(this.label9);
            this.grpBoxInfo.Controls.Add(this.lblTotalTracks);
            this.grpBoxInfo.Controls.Add(this.txtTotalTracks);
            this.grpBoxInfo.Controls.Add(this.lblTotalSectors);
            this.grpBoxInfo.Controls.Add(this.txtTotalSectors);
            this.grpBoxInfo.Controls.Add(this.lblTotalHeads);
            this.grpBoxInfo.Controls.Add(this.txtTotalHeads);
            this.grpBoxInfo.Controls.Add(this.lblTotalSize);
            this.grpBoxInfo.Controls.Add(this.txtTotalSize);
            this.grpBoxInfo.Controls.Add(this.lblModel);
            this.grpBoxInfo.Controls.Add(this.txtModel);
            this.grpBoxInfo.Controls.Add(this.lblDriveName);
            this.grpBoxInfo.Controls.Add(this.txtDriveName);
            this.grpBoxInfo.Controls.Add(this.grpVolumes);
            this.grpBoxInfo.Location = new System.Drawing.Point(512, 12);
            this.grpBoxInfo.Name = "grpBoxInfo";
            this.grpBoxInfo.Size = new System.Drawing.Size(516, 451);
            this.grpBoxInfo.TabIndex = 3;
            this.grpBoxInfo.TabStop = false;
            this.grpBoxInfo.Text = "Device Information";
            // 
            // grpSecondHash
            // 
            this.grpSecondHash.Controls.Add(this.txtSecondHash);
            this.grpSecondHash.Location = new System.Drawing.Point(10, 383);
            this.grpSecondHash.Name = "grpSecondHash";
            this.grpSecondHash.Size = new System.Drawing.Size(503, 55);
            this.grpSecondHash.TabIndex = 26;
            this.grpSecondHash.TabStop = false;
            this.grpSecondHash.Text = "Second Checksum";
            // 
            // txtSecondHash
            // 
            this.txtSecondHash.Location = new System.Drawing.Point(9, 20);
            this.txtSecondHash.Name = "txtSecondHash";
            this.txtSecondHash.ReadOnly = true;
            this.txtSecondHash.Size = new System.Drawing.Size(488, 20);
            this.txtSecondHash.TabIndex = 0;
            // 
            // grpBoxHash1
            // 
            this.grpBoxHash1.Controls.Add(this.txtFirstHash);
            this.grpBoxHash1.Location = new System.Drawing.Point(9, 327);
            this.grpBoxHash1.Name = "grpBoxHash1";
            this.grpBoxHash1.Size = new System.Drawing.Size(504, 49);
            this.grpBoxHash1.TabIndex = 25;
            this.grpBoxHash1.TabStop = false;
            this.grpBoxHash1.Text = "First Checksum";
            // 
            // txtFirstHash
            // 
            this.txtFirstHash.Location = new System.Drawing.Point(10, 19);
            this.txtFirstHash.Name = "txtFirstHash";
            this.txtFirstHash.ReadOnly = true;
            this.txtFirstHash.Size = new System.Drawing.Size(488, 20);
            this.txtFirstHash.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 285);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(0, 13);
            this.label9.TabIndex = 23;
            // 
            // lblTotalTracks
            // 
            this.lblTotalTracks.AutoSize = true;
            this.lblTotalTracks.Location = new System.Drawing.Point(16, 161);
            this.lblTotalTracks.Name = "lblTotalTracks";
            this.lblTotalTracks.Size = new System.Drawing.Size(67, 13);
            this.lblTotalTracks.TabIndex = 21;
            this.lblTotalTracks.Text = "Total Tracks";
            // 
            // txtTotalTracks
            // 
            this.txtTotalTracks.Location = new System.Drawing.Point(105, 158);
            this.txtTotalTracks.Name = "txtTotalTracks";
            this.txtTotalTracks.ReadOnly = true;
            this.txtTotalTracks.Size = new System.Drawing.Size(408, 20);
            this.txtTotalTracks.TabIndex = 20;
            // 
            // lblTotalSectors
            // 
            this.lblTotalSectors.AutoSize = true;
            this.lblTotalSectors.Location = new System.Drawing.Point(16, 134);
            this.lblTotalSectors.Name = "lblTotalSectors";
            this.lblTotalSectors.Size = new System.Drawing.Size(70, 13);
            this.lblTotalSectors.TabIndex = 19;
            this.lblTotalSectors.Text = "Total Sectors";
            // 
            // txtTotalSectors
            // 
            this.txtTotalSectors.Location = new System.Drawing.Point(105, 131);
            this.txtTotalSectors.Name = "txtTotalSectors";
            this.txtTotalSectors.ReadOnly = true;
            this.txtTotalSectors.Size = new System.Drawing.Size(408, 20);
            this.txtTotalSectors.TabIndex = 18;
            // 
            // lblTotalHeads
            // 
            this.lblTotalHeads.AutoSize = true;
            this.lblTotalHeads.Location = new System.Drawing.Point(16, 108);
            this.lblTotalHeads.Name = "lblTotalHeads";
            this.lblTotalHeads.Size = new System.Drawing.Size(65, 13);
            this.lblTotalHeads.TabIndex = 17;
            this.lblTotalHeads.Text = "Total Heads";
            // 
            // txtTotalHeads
            // 
            this.txtTotalHeads.Location = new System.Drawing.Point(105, 105);
            this.txtTotalHeads.Name = "txtTotalHeads";
            this.txtTotalHeads.ReadOnly = true;
            this.txtTotalHeads.Size = new System.Drawing.Size(408, 20);
            this.txtTotalHeads.TabIndex = 16;
            // 
            // lblTotalSize
            // 
            this.lblTotalSize.AutoSize = true;
            this.lblTotalSize.Location = new System.Drawing.Point(16, 82);
            this.lblTotalSize.Name = "lblTotalSize";
            this.lblTotalSize.Size = new System.Drawing.Size(54, 13);
            this.lblTotalSize.TabIndex = 7;
            this.lblTotalSize.Text = "Total Size";
            // 
            // txtTotalSize
            // 
            this.txtTotalSize.Location = new System.Drawing.Point(105, 79);
            this.txtTotalSize.Name = "txtTotalSize";
            this.txtTotalSize.ReadOnly = true;
            this.txtTotalSize.Size = new System.Drawing.Size(408, 20);
            this.txtTotalSize.TabIndex = 6;
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.Location = new System.Drawing.Point(16, 57);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(36, 13);
            this.lblModel.TabIndex = 5;
            this.lblModel.Text = "Model";
            // 
            // txtModel
            // 
            this.txtModel.Location = new System.Drawing.Point(105, 54);
            this.txtModel.Name = "txtModel";
            this.txtModel.ReadOnly = true;
            this.txtModel.Size = new System.Drawing.Size(408, 20);
            this.txtModel.TabIndex = 4;
            // 
            // lblDriveName
            // 
            this.lblDriveName.AutoSize = true;
            this.lblDriveName.Location = new System.Drawing.Point(16, 32);
            this.lblDriveName.Name = "lblDriveName";
            this.lblDriveName.Size = new System.Drawing.Size(63, 13);
            this.lblDriveName.TabIndex = 3;
            this.lblDriveName.Text = "Drive Name";
            // 
            // txtDriveName
            // 
            this.txtDriveName.Location = new System.Drawing.Point(105, 29);
            this.txtDriveName.Name = "txtDriveName";
            this.txtDriveName.ReadOnly = true;
            this.txtDriveName.Size = new System.Drawing.Size(408, 20);
            this.txtDriveName.TabIndex = 0;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(7, 19);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(418, 17);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 4;
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.lblPercentage);
            this.grpStatus.Controls.Add(this.rtbLog);
            this.grpStatus.Controls.Add(this.pictWorking);
            this.grpStatus.Controls.Add(this.progressBar);
            this.grpStatus.Location = new System.Drawing.Point(12, 173);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(494, 290);
            this.grpStatus.TabIndex = 5;
            this.grpStatus.TabStop = false;
            this.grpStatus.Text = "Status";
            // 
            // lblPercentage
            // 
            this.lblPercentage.AutoSize = true;
            this.lblPercentage.Location = new System.Drawing.Point(431, 21);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(0, 13);
            this.lblPercentage.TabIndex = 12;
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.SystemColors.Window;
            this.rtbLog.DetectUrls = false;
            this.rtbLog.Location = new System.Drawing.Point(7, 42);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(481, 235);
            this.rtbLog.TabIndex = 10;
            this.rtbLog.Text = "";
            // 
            // pictWorking
            // 
            this.pictWorking.Enabled = false;
            this.pictWorking.Image = ((System.Drawing.Image)(resources.GetObject("pictWorking.Image")));
            this.pictWorking.Location = new System.Drawing.Point(472, 19);
            this.pictWorking.Name = "pictWorking";
            this.pictWorking.Size = new System.Drawing.Size(16, 15);
            this.pictWorking.TabIndex = 9;
            this.pictWorking.TabStop = false;
            this.pictWorking.Visible = false;
            // 
            // btnPrepare
            // 
            this.btnPrepare.Location = new System.Drawing.Point(7, 19);
            this.btnPrepare.Name = "btnPrepare";
            this.btnPrepare.Size = new System.Drawing.Size(120, 23);
            this.btnPrepare.TabIndex = 11;
            this.btnPrepare.Text = "&Prepare for Tests";
            this.btnPrepare.UseVisualStyleBackColor = true;
            this.btnPrepare.Click += new System.EventHandler(this.btnPrepare_Click);
            // 
            // btnCancelOp
            // 
            this.btnCancelOp.Enabled = false;
            this.btnCancelOp.Location = new System.Drawing.Point(909, 19);
            this.btnCancelOp.Name = "btnCancelOp";
            this.btnCancelOp.Size = new System.Drawing.Size(98, 23);
            this.btnCancelOp.TabIndex = 8;
            this.btnCancelOp.Text = "&Cancel operation";
            this.btnCancelOp.UseVisualStyleBackColor = true;
            this.btnCancelOp.Click += new System.EventHandler(this.btnCancelOp_Click);
            // 
            // btnRunTests
            // 
            this.btnRunTests.Location = new System.Drawing.Point(381, 19);
            this.btnRunTests.Name = "btnRunTests";
            this.btnRunTests.Size = new System.Drawing.Size(78, 23);
            this.btnRunTests.TabIndex = 6;
            this.btnRunTests.Text = "&Run tests";
            this.btnRunTests.UseVisualStyleBackColor = true;
            this.btnRunTests.Click += new System.EventHandler(this.btnRunTests_Click);
            // 
            // grpOperations
            // 
            this.grpOperations.Controls.Add(this.btnCleanup);
            this.grpOperations.Controls.Add(this.btnWriteProtectionDisabled);
            this.grpOperations.Controls.Add(this.btnSecondHash);
            this.grpOperations.Controls.Add(this.btnWriteEnabled);
            this.grpOperations.Controls.Add(this.btnPrepare);
            this.grpOperations.Controls.Add(this.btnCheckSum1);
            this.grpOperations.Controls.Add(this.btnRunTests);
            this.grpOperations.Controls.Add(this.btnCancelOp);
            this.grpOperations.Location = new System.Drawing.Point(12, 479);
            this.grpOperations.Name = "grpOperations";
            this.grpOperations.Size = new System.Drawing.Size(1016, 57);
            this.grpOperations.TabIndex = 12;
            this.grpOperations.TabStop = false;
            this.grpOperations.Text = "Operations";
            // 
            // btnCleanup
            // 
            this.btnCleanup.Location = new System.Drawing.Point(716, 20);
            this.btnCleanup.Name = "btnCleanup";
            this.btnCleanup.Size = new System.Drawing.Size(103, 23);
            this.btnCleanup.TabIndex = 15;
            this.btnCleanup.Text = "Cleanup Device";
            this.btnCleanup.UseVisualStyleBackColor = true;
            this.btnCleanup.Click += new System.EventHandler(this.btnCleanup_Click);
            // 
            // btnWriteProtectionDisabled
            // 
            this.btnWriteProtectionDisabled.Location = new System.Drawing.Point(576, 19);
            this.btnWriteProtectionDisabled.Name = "btnWriteProtectionDisabled";
            this.btnWriteProtectionDisabled.Size = new System.Drawing.Size(133, 23);
            this.btnWriteProtectionDisabled.TabIndex = 14;
            this.btnWriteProtectionDisabled.Text = "Write protection disabled";
            this.btnWriteProtectionDisabled.UseVisualStyleBackColor = true;
            // 
            // btnSecondHash
            // 
            this.btnSecondHash.Location = new System.Drawing.Point(465, 19);
            this.btnSecondHash.Name = "btnSecondHash";
            this.btnSecondHash.Size = new System.Drawing.Size(105, 23);
            this.btnSecondHash.TabIndex = 13;
            this.btnSecondHash.Text = "Second checksum";
            this.btnSecondHash.UseVisualStyleBackColor = true;
            this.btnSecondHash.Click += new System.EventHandler(this.btnSecondHash_Click);
            // 
            // btnWriteEnabled
            // 
            this.btnWriteEnabled.Location = new System.Drawing.Point(133, 19);
            this.btnWriteEnabled.Name = "btnWriteEnabled";
            this.btnWriteEnabled.Size = new System.Drawing.Size(133, 23);
            this.btnWriteEnabled.TabIndex = 12;
            this.btnWriteEnabled.Text = "Write protection enabled";
            this.btnWriteEnabled.UseVisualStyleBackColor = true;
            this.btnWriteEnabled.Click += new System.EventHandler(this.btnWriteEnabled_Click);
            // 
            // WlFormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 546);
            this.Controls.Add(this.grpOperations);
            this.Controls.Add(this.grpStatus);
            this.Controls.Add(this.grpBoxInfo);
            this.Controls.Add(this.grpDrives);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "WlFormMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "USB WriteLock Test Application";
            this.Load += new System.EventHandler(this.WlFormMain_Load);
            this.grpDrives.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdDevices)).EndInit();
            this.grpVolumes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdVolumes)).EndInit();
            this.grpBoxInfo.ResumeLayout(false);
            this.grpBoxInfo.PerformLayout();
            this.grpSecondHash.ResumeLayout(false);
            this.grpSecondHash.PerformLayout();
            this.grpBoxHash1.ResumeLayout(false);
            this.grpBoxHash1.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictWorking)).EndInit();
            this.grpOperations.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDrives;
        //private System.Windows.Forms.DataGridViewTextBoxColumn pnpDeviceIDDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn driveInfoDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button btnCheckSum1;
        private System.Windows.Forms.GroupBox grpVolumes;
        private System.Windows.Forms.DataGridView grdDevices;
        private System.Windows.Forms.DataGridView grdVolumes;
        private System.Windows.Forms.GroupBox grpBoxInfo;
        private System.Windows.Forms.TextBox txtDriveName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblTotalTracks;
        private System.Windows.Forms.TextBox txtTotalTracks;
        private System.Windows.Forms.Label lblTotalSectors;
        private System.Windows.Forms.TextBox txtTotalSectors;
        private System.Windows.Forms.Label lblTotalHeads;
        private System.Windows.Forms.TextBox txtTotalHeads;
        private System.Windows.Forms.Label lblTotalSize;
        private System.Windows.Forms.TextBox txtTotalSize;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Label lblDriveName;
        private System.Windows.Forms.GroupBox grpBoxHash1;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.Button btnRunTests;
        private System.Windows.Forms.Button btnCancelOp;
        private System.Windows.Forms.PictureBox pictWorking;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Button btnPrepare;
        private System.Windows.Forms.Label lblPercentage;
        private System.Windows.Forms.GroupBox grpOperations;
        private System.Windows.Forms.Button btnWriteEnabled;
        private System.Windows.Forms.Button btnSecondHash;
        private System.Windows.Forms.GroupBox grpSecondHash;
        private System.Windows.Forms.TextBox txtSecondHash;
        private System.Windows.Forms.TextBox txtFirstHash;
        private System.Windows.Forms.Button btnCleanup;
        private System.Windows.Forms.Button btnWriteProtectionDisabled;
    }
}

