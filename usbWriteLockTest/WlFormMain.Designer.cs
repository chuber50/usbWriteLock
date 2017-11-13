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
            this.grpBoxHash1 = new System.Windows.Forms.GroupBox();
            this.grdHashes = new System.Windows.Forms.DataGridView();
            this.label9 = new System.Windows.Forms.Label();
            this.lblTotalTracks = new System.Windows.Forms.Label();
            this.txtTotalTracks = new System.Windows.Forms.TextBox();
            this.lblTotalSectors = new System.Windows.Forms.Label();
            this.txtTotalSectors = new System.Windows.Forms.TextBox();
            this.lblTotalHeads = new System.Windows.Forms.Label();
            this.txtTotalHeads = new System.Windows.Forms.TextBox();
            this.lblTotalCyl = new System.Windows.Forms.Label();
            this.txtTotalCyl = new System.Windows.Forms.TextBox();
            this.lblTracksCyl = new System.Windows.Forms.Label();
            this.txtTracksCyl = new System.Windows.Forms.TextBox();
            this.lblSectorsTrack = new System.Windows.Forms.Label();
            this.txtSectorsTrack = new System.Windows.Forms.TextBox();
            this.lalBytesSector = new System.Windows.Forms.Label();
            this.txtBytesSector = new System.Windows.Forms.TextBox();
            this.lblTotalSize = new System.Windows.Forms.Label();
            this.txtTotalSize = new System.Windows.Forms.TextBox();
            this.lblModel = new System.Windows.Forms.Label();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.lblDriveName = new System.Windows.Forms.Label();
            this.txtDriveName = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.grpOperations = new System.Windows.Forms.GroupBox();
            this.btnPrepare = new System.Windows.Forms.Button();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.pictWorking = new System.Windows.Forms.PictureBox();
            this.btnCancelOp = new System.Windows.Forms.Button();
            this.btnResetResults = new System.Windows.Forms.Button();
            this.btnRunTests = new System.Windows.Forms.Button();
            this.lblPercentage = new System.Windows.Forms.Label();
            this.grpDrives.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDevices)).BeginInit();
            this.grpVolumes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdVolumes)).BeginInit();
            this.grpBoxInfo.SuspendLayout();
            this.grpBoxHash1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdHashes)).BeginInit();
            this.grpOperations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictWorking)).BeginInit();
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
            // 
            // btnCheckSum1
            // 
            this.btnCheckSum1.Location = new System.Drawing.Point(7, 341);
            this.btnCheckSum1.Name = "btnCheckSum1";
            this.btnCheckSum1.Size = new System.Drawing.Size(141, 23);
            this.btnCheckSum1.TabIndex = 1;
            this.btnCheckSum1.Text = "C&alulate device checksum";
            this.btnCheckSum1.UseVisualStyleBackColor = true;
            this.btnCheckSum1.Click += new System.EventHandler(this.btnCheckSum1_Click);
            // 
            // grpVolumes
            // 
            this.grpVolumes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpVolumes.Controls.Add(this.grdVolumes);
            this.grpVolumes.Location = new System.Drawing.Point(7, 292);
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
            this.grpBoxInfo.Controls.Add(this.grpBoxHash1);
            this.grpBoxInfo.Controls.Add(this.label9);
            this.grpBoxInfo.Controls.Add(this.lblTotalTracks);
            this.grpBoxInfo.Controls.Add(this.txtTotalTracks);
            this.grpBoxInfo.Controls.Add(this.lblTotalSectors);
            this.grpBoxInfo.Controls.Add(this.txtTotalSectors);
            this.grpBoxInfo.Controls.Add(this.lblTotalHeads);
            this.grpBoxInfo.Controls.Add(this.txtTotalHeads);
            this.grpBoxInfo.Controls.Add(this.lblTotalCyl);
            this.grpBoxInfo.Controls.Add(this.txtTotalCyl);
            this.grpBoxInfo.Controls.Add(this.lblTracksCyl);
            this.grpBoxInfo.Controls.Add(this.txtTracksCyl);
            this.grpBoxInfo.Controls.Add(this.lblSectorsTrack);
            this.grpBoxInfo.Controls.Add(this.txtSectorsTrack);
            this.grpBoxInfo.Controls.Add(this.lalBytesSector);
            this.grpBoxInfo.Controls.Add(this.txtBytesSector);
            this.grpBoxInfo.Controls.Add(this.lblTotalSize);
            this.grpBoxInfo.Controls.Add(this.txtTotalSize);
            this.grpBoxInfo.Controls.Add(this.lblModel);
            this.grpBoxInfo.Controls.Add(this.txtModel);
            this.grpBoxInfo.Controls.Add(this.lblDriveName);
            this.grpBoxInfo.Controls.Add(this.txtDriveName);
            this.grpBoxInfo.Controls.Add(this.grpVolumes);
            this.grpBoxInfo.Location = new System.Drawing.Point(512, 12);
            this.grpBoxInfo.Name = "grpBoxInfo";
            this.grpBoxInfo.Size = new System.Drawing.Size(516, 563);
            this.grpBoxInfo.TabIndex = 3;
            this.grpBoxInfo.TabStop = false;
            this.grpBoxInfo.Text = "Device Information";
            // 
            // grpBoxHash1
            // 
            this.grpBoxHash1.Controls.Add(this.grdHashes);
            this.grpBoxHash1.Location = new System.Drawing.Point(7, 430);
            this.grpBoxHash1.Name = "grpBoxHash1";
            this.grpBoxHash1.Size = new System.Drawing.Size(504, 127);
            this.grpBoxHash1.TabIndex = 25;
            this.grpBoxHash1.TabStop = false;
            this.grpBoxHash1.Text = "Computed Hashes";
            // 
            // grdHashes
            // 
            this.grdHashes.AllowUserToAddRows = false;
            this.grdHashes.AllowUserToDeleteRows = false;
            this.grdHashes.AllowUserToResizeRows = false;
            this.grdHashes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdHashes.BackgroundColor = System.Drawing.SystemColors.Window;
            this.grdHashes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdHashes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdHashes.Location = new System.Drawing.Point(3, 16);
            this.grdHashes.MultiSelect = false;
            this.grdHashes.Name = "grdHashes";
            this.grdHashes.ReadOnly = true;
            this.grdHashes.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.grdHashes.RowHeadersVisible = false;
            this.grdHashes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdHashes.ShowEditingIcon = false;
            this.grdHashes.Size = new System.Drawing.Size(498, 108);
            this.grdHashes.TabIndex = 2;
            this.grdHashes.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.grdHashes_DataBindingComplete);
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
            this.lblTotalTracks.Location = new System.Drawing.Point(16, 262);
            this.lblTotalTracks.Name = "lblTotalTracks";
            this.lblTotalTracks.Size = new System.Drawing.Size(67, 13);
            this.lblTotalTracks.TabIndex = 21;
            this.lblTotalTracks.Text = "Total Tracks";
            // 
            // txtTotalTracks
            // 
            this.txtTotalTracks.Location = new System.Drawing.Point(105, 259);
            this.txtTotalTracks.Name = "txtTotalTracks";
            this.txtTotalTracks.ReadOnly = true;
            this.txtTotalTracks.Size = new System.Drawing.Size(408, 20);
            this.txtTotalTracks.TabIndex = 20;
            // 
            // lblTotalSectors
            // 
            this.lblTotalSectors.AutoSize = true;
            this.lblTotalSectors.Location = new System.Drawing.Point(16, 235);
            this.lblTotalSectors.Name = "lblTotalSectors";
            this.lblTotalSectors.Size = new System.Drawing.Size(70, 13);
            this.lblTotalSectors.TabIndex = 19;
            this.lblTotalSectors.Text = "Total Sectors";
            // 
            // txtTotalSectors
            // 
            this.txtTotalSectors.Location = new System.Drawing.Point(105, 232);
            this.txtTotalSectors.Name = "txtTotalSectors";
            this.txtTotalSectors.ReadOnly = true;
            this.txtTotalSectors.Size = new System.Drawing.Size(408, 20);
            this.txtTotalSectors.TabIndex = 18;
            // 
            // lblTotalHeads
            // 
            this.lblTotalHeads.AutoSize = true;
            this.lblTotalHeads.Location = new System.Drawing.Point(16, 209);
            this.lblTotalHeads.Name = "lblTotalHeads";
            this.lblTotalHeads.Size = new System.Drawing.Size(65, 13);
            this.lblTotalHeads.TabIndex = 17;
            this.lblTotalHeads.Text = "Total Heads";
            // 
            // txtTotalHeads
            // 
            this.txtTotalHeads.Location = new System.Drawing.Point(105, 206);
            this.txtTotalHeads.Name = "txtTotalHeads";
            this.txtTotalHeads.ReadOnly = true;
            this.txtTotalHeads.Size = new System.Drawing.Size(408, 20);
            this.txtTotalHeads.TabIndex = 16;
            // 
            // lblTotalCyl
            // 
            this.lblTotalCyl.AutoSize = true;
            this.lblTotalCyl.Location = new System.Drawing.Point(16, 183);
            this.lblTotalCyl.Name = "lblTotalCyl";
            this.lblTotalCyl.Size = new System.Drawing.Size(76, 13);
            this.lblTotalCyl.TabIndex = 15;
            this.lblTotalCyl.Text = "Total Cylinders";
            // 
            // txtTotalCyl
            // 
            this.txtTotalCyl.Location = new System.Drawing.Point(105, 180);
            this.txtTotalCyl.Name = "txtTotalCyl";
            this.txtTotalCyl.ReadOnly = true;
            this.txtTotalCyl.Size = new System.Drawing.Size(408, 20);
            this.txtTotalCyl.TabIndex = 14;
            // 
            // lblTracksCyl
            // 
            this.lblTracksCyl.AutoSize = true;
            this.lblTracksCyl.Location = new System.Drawing.Point(16, 157);
            this.lblTracksCyl.Name = "lblTracksCyl";
            this.lblTracksCyl.Size = new System.Drawing.Size(82, 13);
            this.lblTracksCyl.TabIndex = 13;
            this.lblTracksCyl.Text = "Tracks/Cylinder";
            // 
            // txtTracksCyl
            // 
            this.txtTracksCyl.Location = new System.Drawing.Point(105, 154);
            this.txtTracksCyl.Name = "txtTracksCyl";
            this.txtTracksCyl.ReadOnly = true;
            this.txtTracksCyl.Size = new System.Drawing.Size(408, 20);
            this.txtTracksCyl.TabIndex = 12;
            // 
            // lblSectorsTrack
            // 
            this.lblSectorsTrack.AutoSize = true;
            this.lblSectorsTrack.Location = new System.Drawing.Point(16, 132);
            this.lblSectorsTrack.Name = "lblSectorsTrack";
            this.lblSectorsTrack.Size = new System.Drawing.Size(76, 13);
            this.lblSectorsTrack.TabIndex = 11;
            this.lblSectorsTrack.Text = "Sectors/Track";
            // 
            // txtSectorsTrack
            // 
            this.txtSectorsTrack.Location = new System.Drawing.Point(105, 129);
            this.txtSectorsTrack.Name = "txtSectorsTrack";
            this.txtSectorsTrack.ReadOnly = true;
            this.txtSectorsTrack.Size = new System.Drawing.Size(408, 20);
            this.txtSectorsTrack.TabIndex = 10;
            // 
            // lalBytesSector
            // 
            this.lalBytesSector.AutoSize = true;
            this.lalBytesSector.Location = new System.Drawing.Point(16, 107);
            this.lalBytesSector.Name = "lalBytesSector";
            this.lalBytesSector.Size = new System.Drawing.Size(69, 13);
            this.lalBytesSector.TabIndex = 9;
            this.lalBytesSector.Text = "Bytes/Sector";
            // 
            // txtBytesSector
            // 
            this.txtBytesSector.Location = new System.Drawing.Point(105, 104);
            this.txtBytesSector.Name = "txtBytesSector";
            this.txtBytesSector.ReadOnly = true;
            this.txtBytesSector.Size = new System.Drawing.Size(408, 20);
            this.txtBytesSector.TabIndex = 8;
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
            // grpOperations
            // 
            this.grpOperations.Controls.Add(this.lblPercentage);
            this.grpOperations.Controls.Add(this.btnPrepare);
            this.grpOperations.Controls.Add(this.rtbLog);
            this.grpOperations.Controls.Add(this.pictWorking);
            this.grpOperations.Controls.Add(this.btnCancelOp);
            this.grpOperations.Controls.Add(this.btnResetResults);
            this.grpOperations.Controls.Add(this.btnRunTests);
            this.grpOperations.Controls.Add(this.btnCheckSum1);
            this.grpOperations.Controls.Add(this.progressBar);
            this.grpOperations.Location = new System.Drawing.Point(12, 173);
            this.grpOperations.Name = "grpOperations";
            this.grpOperations.Size = new System.Drawing.Size(494, 402);
            this.grpOperations.TabIndex = 5;
            this.grpOperations.TabStop = false;
            this.grpOperations.Text = "Operations";
            // 
            // btnPrepare
            // 
            this.btnPrepare.Location = new System.Drawing.Point(7, 285);
            this.btnPrepare.Name = "btnPrepare";
            this.btnPrepare.Size = new System.Drawing.Size(141, 23);
            this.btnPrepare.TabIndex = 11;
            this.btnPrepare.Text = "&Prepare for Tests";
            this.btnPrepare.UseVisualStyleBackColor = true;
            this.btnPrepare.Click += new System.EventHandler(this.btnPrepare_Click);
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.SystemColors.Window;
            this.rtbLog.DetectUrls = false;
            this.rtbLog.Location = new System.Drawing.Point(7, 42);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(481, 218);
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
            // btnCancelOp
            // 
            this.btnCancelOp.Enabled = false;
            this.btnCancelOp.Location = new System.Drawing.Point(6, 370);
            this.btnCancelOp.Name = "btnCancelOp";
            this.btnCancelOp.Size = new System.Drawing.Size(141, 23);
            this.btnCancelOp.TabIndex = 8;
            this.btnCancelOp.Text = "&Cancel operation";
            this.btnCancelOp.UseVisualStyleBackColor = true;
            this.btnCancelOp.Click += new System.EventHandler(this.btnCancelOp_Click);
            // 
            // btnResetResults
            // 
            this.btnResetResults.Location = new System.Drawing.Point(153, 370);
            this.btnResetResults.Name = "btnResetResults";
            this.btnResetResults.Size = new System.Drawing.Size(140, 23);
            this.btnResetResults.TabIndex = 7;
            this.btnResetResults.Text = "&Reset results";
            this.btnResetResults.UseVisualStyleBackColor = true;
            this.btnResetResults.Click += new System.EventHandler(this.btnResetResults_Click);
            // 
            // btnRunTests
            // 
            this.btnRunTests.Location = new System.Drawing.Point(152, 341);
            this.btnRunTests.Name = "btnRunTests";
            this.btnRunTests.Size = new System.Drawing.Size(141, 23);
            this.btnRunTests.TabIndex = 6;
            this.btnRunTests.Text = "&Run tests";
            this.btnRunTests.UseVisualStyleBackColor = true;
            this.btnRunTests.Click += new System.EventHandler(this.btnRunTests_Click);
            // 
            // lblPercentage
            // 
            this.lblPercentage.AutoSize = true;
            this.lblPercentage.Location = new System.Drawing.Point(431, 21);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(0, 13);
            this.lblPercentage.TabIndex = 12;
            // 
            // WlFormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 586);
            this.Controls.Add(this.grpOperations);
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
            this.grpBoxHash1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdHashes)).EndInit();
            this.grpOperations.ResumeLayout(false);
            this.grpOperations.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictWorking)).EndInit();
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
        private System.Windows.Forms.Label lblTotalCyl;
        private System.Windows.Forms.TextBox txtTotalCyl;
        private System.Windows.Forms.Label lblTracksCyl;
        private System.Windows.Forms.TextBox txtTracksCyl;
        private System.Windows.Forms.Label lblSectorsTrack;
        private System.Windows.Forms.TextBox txtSectorsTrack;
        private System.Windows.Forms.Label lalBytesSector;
        private System.Windows.Forms.TextBox txtBytesSector;
        private System.Windows.Forms.Label lblTotalSize;
        private System.Windows.Forms.TextBox txtTotalSize;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Label lblDriveName;
        private System.Windows.Forms.GroupBox grpBoxHash1;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.GroupBox grpOperations;
        private System.Windows.Forms.Button btnResetResults;
        private System.Windows.Forms.Button btnRunTests;
        private System.Windows.Forms.Button btnCancelOp;
        private System.Windows.Forms.DataGridView grdHashes;
        private System.Windows.Forms.PictureBox pictWorking;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Button btnPrepare;
        private System.Windows.Forms.Label lblPercentage;
    }
}

