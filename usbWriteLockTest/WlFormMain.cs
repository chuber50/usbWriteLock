using System;
using System.ComponentModel;
using System.Windows.Forms;
using usbWriteLockTest.data;
using usbWriteLockTest.logic;

namespace usbWriteLockTest
{
    public partial class WlFormMain : Form
    {
        private PnpEventWatcher _watcher;
        private DeviceCollector _deviceCollector;
        private BackgroundWorker _hashWorker;

        public WlFormMain()
        {
            InitializeComponent();
        }

        private void WlFormMain_Load(object sender, EventArgs e)
        {
            try
            {
                _deviceCollector = new DeviceCollector();
            }
            catch (UnauthorizedAccessException)
            {
                okMsgBox("Write locking must be unloaded when starting the test application.");
                Application.Exit();
            }
            
            grdDevices.DataSource = _deviceCollector.drives;
            if (_deviceCollector.drives.Count > 0)
            {
                updateDetails(0);
            }

            _watcher = new PnpEventWatcher(this.updateDeviceGrid);
            _hashWorker = new BackgroundWorker();
            _hashWorker.WorkerReportsProgress = true;
            _hashWorker.WorkerSupportsCancellation = true;

            _hashWorker.DoWork +=
                hashWorker_DoWork;
            _hashWorker.RunWorkerCompleted +=
                hashWorker_RunWorkerCompleted;
            _hashWorker.ProgressChanged +=
                hashWorker_ProgressChanged;

            logAdd("Application initialized", false);
        }

        

        delegate void StringArgReturningVoidDelegate();

        //http://www.infinitec.de/post/2007/06/09/Displaying-progress-updates-when-hashing-large-files.aspx
        private void btnCheckSum1_Click(object sender, EventArgs e)
        {
            if (_deviceCollector.drives.Count > 0)
            {
                if (!_hashWorker.IsBusy)
                {
                    lockInterface();
                    logAdd($"Started calculating hash.");
                    _hashWorker.RunWorkerAsync();
                }
            }
            else
            {
                okMsgBox("Hash calculation can only be executed with a disk selected in the 'USB Devices' grid.");
            }

        }

        private void hashWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            DeviceHandler deviceHandler = new DeviceHandler(_deviceCollector.drives[grdDevices.CurrentCell.RowIndex]);
            deviceHandler.LockVolumes();

            AsyncHashCalculator hashCalculator = new AsyncHashCalculator(worker, _deviceCollector.drives[grdDevices.CurrentCell.RowIndex]);
            e.Result = hashCalculator.computeHash();

            if (worker != null && worker.CancellationPending)
            {
                e.Cancel = true;
            }

            deviceHandler.UnlockVolumes();            
        }

        private void hashWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                logAdd($"Hashworker: {e.Error.Message}");
            }
            else if (e.Cancelled)
            {
                logAdd("Hashworker has been cancelled by user.");
            }
            else
            {
                logAdd($"Hashworker successfully computed hash: {(string)e.Result}");
            }

            updateForm();

            unlockInterface(); 
        }

        private void hashWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage <= 100 ? e.ProgressPercentage : 100;
        }

        private void grdDevices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            updateDetails(e.RowIndex);
        }

        private void updateDetails(int rowIndex)
        {
            if (rowIndex >= 0 && _deviceCollector.drives.Count >= rowIndex)
            {
                txtDriveName.Text = _deviceCollector.drives[rowIndex].driveName;
                txtModel.Text = _deviceCollector.drives[rowIndex].model;
                txtTotalSize.Text = _deviceCollector.drives[rowIndex].driveSize.ToString();
                txtSectorsTrack.Text = _deviceCollector.drives[rowIndex].sectorsPerTrack.ToString();
                txtTracksCyl.Text = _deviceCollector.drives[rowIndex].tracksPerCylinder.ToString();
                txtBytesSector.Text = _deviceCollector.drives[rowIndex].bytesPerSector.ToString();
                txtTotalCyl.Text = _deviceCollector.drives[rowIndex].totalCylinders.ToString();
                txtTotalHeads.Text = _deviceCollector.drives[rowIndex].totalHeads.ToString();
                txtTotalTracks.Text = _deviceCollector.drives[rowIndex].totalTracks.ToString();
                txtTotalSectors.Text = _deviceCollector.drives[rowIndex].totalSectors.ToString();

                grdVolumes.DataSource = null;
                grdVolumes.DataSource = _deviceCollector.drives[rowIndex].volumes;
                grdHashes.DataSource = null;
                grdHashes.DataSource = _deviceCollector.drives[rowIndex].hashes;

            }
        }

        private void grdDevices_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.grdDevices.Columns[e.ColumnIndex].HeaderText.Equals("Total Size"))
            {
                if (e.Value != null)
                {
                    convertByteColumn(e);
                }
            }
        }

        private void updateForm()
        {
            grdDevices.DataSource = null;
            grdDevices.DataSource = _deviceCollector.drives;
            if (grdDevices.CurrentRow != null)
            {
                updateDetails(grdDevices.CurrentRow.Index);
            } 
        }

        private void updateDeviceGrid()
        {
            _deviceCollector.repollDevices();
            if (this.grdDevices.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = updateForm;
                this.Invoke(d, new object[] { });
            }
            else
            {
                updateForm();
            }
        }

        private void grdHashes_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            grdHashes.Columns[0].Width = 40;
        }

        private void btnCancelOp_Click(object sender, EventArgs e)
        {
            progressBar.Value = 0;
            pictWorking.Visible = false;
            _hashWorker.CancelAsync();
        }

        private void btnResetResults_Click(object sender, EventArgs e)
        {
            //_deviceCollector.drives.ForEach(d => d.hashes.Clear());
            _deviceCollector.ClearHashes();
            if (grdDevices.CurrentRow != null) updateDetails(grdDevices.CurrentRow.Index);
        }

        public void logAdd(string logMsg, bool printDevice = true)
        {
            if (printDevice)
            {
                rtbLog.AppendText(Environment.NewLine + DateTime.Now.ToLocalTime() +
                                  $" {grdDevices.CurrentRow?.Cells[0].Value ?? ""} (SN {grdDevices.CurrentRow?.Cells[3].Value ?? ""}): "
                                  + logMsg + Environment.NewLine);
            }
            else
            {
                rtbLog.AppendText(Environment.NewLine + DateTime.Now.ToLocalTime() + ": " + logMsg + Environment.NewLine);
            }
            
            rtbLog.ScrollToCaret();
        }

        private void lockInterface()
        {
            btnCancelOp.Enabled = true;
            btnCheckSum1.Enabled = false;
            pictWorking.Visible = true;
            pictWorking.Enabled = true;
            grdDevices.Enabled = false;
            grdVolumes.Enabled = false;
            grdHashes.Enabled = false;
            btnRunTests.Enabled = false;
            btnResetResults.Enabled = false;
            btnPrepare.Enabled = false;
        }

        private void unlockInterface()
        {
            progressBar.Value = 0;
            btnCancelOp.Enabled = false;
            btnCheckSum1.Enabled = true;
            pictWorking.Visible = false;
            pictWorking.Enabled = false;
            grdDevices.Enabled = true;
            grdVolumes.Enabled = true;
            grdHashes.Enabled = true;
            btnRunTests.Enabled = true;
            btnResetResults.Enabled = true;
            btnPrepare.Enabled = true;
        }

        //https://social.msdn.microsoft.com/Forums/windows/en-US/62f5b477-5311-4de5-bc18-fbd29bbfc9e2/setting-an-image-column-in-a-datagrid-view-based-on-a-value-in-the-database-c?forum=winformsdatacontrols
        private void convertByteColumn(DataGridViewCellFormattingEventArgs formatting)
        {
            if (formatting.Value != null)
            {
                try
                {
                    long bytes;
                    bytes = Convert.ToInt64(formatting.Value);
                    string size = "0 Bytes";

                    //GB
                    if (bytes >= 1073741824.0)
                        size = $"{bytes / 1073741824.0:##.##}" + " GB";
                    //MB
                    else if (bytes >= 1048576.0)
                        size = $"{bytes / 1048576.0:##.##}" + " MB";
                    //KB
                    else if (bytes >= 1024.0)
                        size = $"{bytes / 1024.0:##.##}" + " KB";
                    //Bytes
                    else if (bytes > 0 && bytes < 1024.0)
                        size = bytes + " Bytes";

                    formatting.Value = size;
                    formatting.FormattingApplied = true;
                }
                catch (FormatException)
                {
                    formatting.FormattingApplied = false;
                }
            }

        }

        private void btnRunTests_Click(object sender, EventArgs e)
        {
            if (_deviceCollector.drives.Count > 0)
            {
                DialogResult result = yncMsgBox("This action could delete all contents on your drive! Continue?");
                if (result == DialogResult.Yes)
                {
                    TestMeta testMeta = _deviceCollector.drives[grdDevices.CurrentRow.Index].getTestVolume();
                    if (testMeta.hasValidVolume)
                    {
                        WriteTester writeTester = new WriteTester(testMeta);
                        logAdd(writeTester.test1_CreateFolder());
                        logAdd(writeTester.test2_CreateFile());
                        logAdd(writeTester.test3_OverwriteFile());
                        logAdd(writeTester.test4_SetFileAttributes());
                        logAdd(writeTester.test5_SetDirCreateTime());
                        logAdd(writeTester.test6_DeleteFile());
                        logAdd(writeTester.test7_DeleteFolder());
                    }
                }
            }
            else
            {
                okMsgBox("Tests can only be executed with a disk selected in the 'USB Devices' grid.");
            }
        }

        private void btnPrepare_Click(object sender, EventArgs e)
        {
            if (_deviceCollector.drives.Count > 0) {
                DialogResult result = yncMsgBox("Write locking must be disabled when preparing the volume for testing! Continue?");
                if (result == DialogResult.Yes)
                {
                    TestMeta testMeta = _deviceCollector.drives[grdDevices.CurrentRow.Index].getTestVolume();
                    VolumePreparer volumePreparer = new VolumePreparer(testMeta);
                    logAdd(volumePreparer.CreateFile());
                    logAdd(volumePreparer.CreateFolder());
                }
            } else
            {
                okMsgBox("Preparation can only be executed with a disk selected in the 'USB Devices' grid.");
            }
            
        }

        private void okMsgBox(string text)
        {
            MessageBox.Show(text,
                @"USB Writelock Test Application",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
        }

        private DialogResult yncMsgBox(string text)
        {
            return MessageBox.Show(text, @"USB Writelock Test Application", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }

    }
}
