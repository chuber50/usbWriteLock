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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grdDevices = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDevices)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.grdDevices);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(417, 286);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "USB Devices";
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
            this.grdDevices.Size = new System.Drawing.Size(411, 267);
            this.grdDevices.TabIndex = 0;
            this.grdDevices.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.grdDevices_CellFormatting);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(595, 261);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // WlFormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 545);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Name = "WlFormMain";
            this.Text = "WlFormMain";
            this.Load += new System.EventHandler(this.WlFormMain_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdDevices)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView grdDevices;
        private System.Windows.Forms.DataGridViewTextBoxColumn pnpDeviceIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn driveInfoDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button button1;
    }
}

