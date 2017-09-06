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
            this.grpDrives = new System.Windows.Forms.GroupBox();
            this.btnCheckSum = new System.Windows.Forms.Button();
            this.grpVolumes = new System.Windows.Forms.GroupBox();
            this.grdDevices = new System.Windows.Forms.DataGridView();
            this.grdVolumes = new System.Windows.Forms.DataGridView();
            this.grpDrives.SuspendLayout();
            this.grpVolumes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDevices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdVolumes)).BeginInit();
            this.SuspendLayout();
            // 
            // grpDrives
            // 
            this.grpDrives.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDrives.Controls.Add(this.grdDevices);
            this.grpDrives.Location = new System.Drawing.Point(12, 12);
            this.grpDrives.Name = "grpDrives";
            this.grpDrives.Size = new System.Drawing.Size(1016, 155);
            this.grpDrives.TabIndex = 0;
            this.grpDrives.TabStop = false;
            this.grpDrives.Text = "USB Devices";
            // 
            // btnCheckSum
            // 
            this.btnCheckSum.Location = new System.Drawing.Point(38, 297);
            this.btnCheckSum.Name = "btnCheckSum";
            this.btnCheckSum.Size = new System.Drawing.Size(145, 23);
            this.btnCheckSum.TabIndex = 1;
            this.btnCheckSum.Text = "Calulate device checksum";
            this.btnCheckSum.UseVisualStyleBackColor = true;
            this.btnCheckSum.Click += new System.EventHandler(this.button1_Click);
            // 
            // grpVolumes
            // 
            this.grpVolumes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpVolumes.Controls.Add(this.grdVolumes);
            this.grpVolumes.Location = new System.Drawing.Point(12, 173);
            this.grpVolumes.Name = "grpVolumes";
            this.grpVolumes.Size = new System.Drawing.Size(1016, 100);
            this.grpVolumes.TabIndex = 2;
            this.grpVolumes.TabStop = false;
            this.grpVolumes.Text = "Volumes";
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
            this.grdDevices.Size = new System.Drawing.Size(1010, 136);
            this.grdDevices.TabIndex = 0;
            this.grdDevices.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdDevices_CellClick);
            this.grdDevices.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.grdDevices_CellFormatting);
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
            this.grdVolumes.Size = new System.Drawing.Size(1010, 81);
            this.grdVolumes.TabIndex = 1;
            // 
            // WlFormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 675);
            this.Controls.Add(this.grpVolumes);
            this.Controls.Add(this.btnCheckSum);
            this.Controls.Add(this.grpDrives);
            this.Name = "WlFormMain";
            this.Text = "WlFormMain";
            this.Load += new System.EventHandler(this.WlFormMain_Load);
            this.grpDrives.ResumeLayout(false);
            this.grpVolumes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdDevices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdVolumes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDrives;
        //private System.Windows.Forms.DataGridViewTextBoxColumn pnpDeviceIDDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn driveInfoDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button btnCheckSum;
        private System.Windows.Forms.GroupBox grpVolumes;
        private System.Windows.Forms.DataGridView grdDevices;
        private System.Windows.Forms.DataGridView grdVolumes;
    }
}

