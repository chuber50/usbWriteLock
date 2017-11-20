using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
        private BindingList<UsbDrive> _dataSource;
        private ApplicationState _state;
        private UsbDrive _actDrive;

        public WlFormMain()
        {
            InitializeComponent();
        }

        private void WlFormMain_Load(object sender, EventArgs e)
        {
            try
            {
                _deviceCollector = new DeviceCollector();
                _dataSource = new BindingList<UsbDrive>(_deviceCollector.drives);
            }
            catch (UnauthorizedAccessException)
            {
                okMsgBox("Write locking must be unloaded when starting the test application.");
                Application.Exit();
            }

            grdDevices.DataSource = _dataSource;
            if (_dataSource != null && _dataSource.Count > 0)
            {
                _actDrive = _dataSource[grdDevices.CurrentCell.RowIndex];
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

            nextState(ApplicationState.PrepareForTests);
        }

        //http://www.infinitec.de/post/2007/06/09/Displaying-progress-updates-when-hashing-large-files.aspx
        private void btnCheckSum1_Click(object sender, EventArgs e)
        {
            if (_dataSource.Count > 0)
            {
                if (!_hashWorker.IsBusy)
                {
                    Debug.Assert(_state == ApplicationState.FirstChecksum || _state == ApplicationState.SecondChecksum);
                    if (_state == ApplicationState.FirstChecksum)
                    {
                        nextState(ApplicationState.FirstHashCalculation);
                    }
                    else
                    {
                        nextState(ApplicationState.SecondHashCalculation);
                    }

                    logAdd($"Started calculating hash.");
                    _hashWorker.RunWorkerAsync();
                }
            }
            else
            {
                okMsgBox("Hash calculation can only be executed with a disk selected in the 'USB Devices' grid.");
            }

        }

        // the backgroundworker process which creates the hash
        private void hashWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            DeviceHandler deviceHandler = new DeviceHandler(_dataSource[grdDevices.CurrentCell.RowIndex]);
            deviceHandler.LockVolumes();

            AsyncHashCalculator hashCalculator =
                new AsyncHashCalculator(worker, _dataSource[grdDevices.CurrentCell.RowIndex]);
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
                nextState(ApplicationState.CleanupDevice);
            }
            else if (e.Cancelled)
            {
                logAdd("Hashworker has been cancelled by user.");
                nextState(ApplicationState.CleanupDevice);
            }
            else
            {
                logAdd($"Hashworker successfully computed hash: {(string) e.Result}");
                Debug.Assert(_state == ApplicationState.FirstHashCalculation || _state == ApplicationState.SecondHashCalculation);
                if (_state == ApplicationState.FirstHashCalculation)
                {
                    txtFirstHash.Text = _actDrive.firstHash;
                    nextState(ApplicationState.RunTests);
                }
                else
                {
                    txtFirstHash.Text = _actDrive.firstHash;
                    txtSecondHash.Text = _actDrive.secondHash;

                    if (_dataSource[grdDevices.CurrentCell.RowIndex].hashesAreEqual())
                    {
                        txtFirstHash.BackColor = Color.DarkSeaGreen;
                        txtSecondHash.BackColor = Color.DarkSeaGreen;
                    }
                    else
                    {
                        txtFirstHash.BackColor = Color.LightCoral;
                        txtSecondHash.BackColor = Color.LightCoral;
                    }
                    nextState(ApplicationState.CleanupDevice);
                }
            }

            //updateDeviceGrid();
        }

        private void hashWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage <= 100 ? e.ProgressPercentage : 100;
            this.lblPercentage.Text = e.ProgressPercentage <= 100 ? e.ProgressPercentage.ToString() + " %" : "100 %";
        }

        private void grdDevices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            _actDrive = _dataSource[grdDevices.CurrentCell.RowIndex];
            updateDetails(e.RowIndex);
        }

        private void updateDetails(int rowIndex)
        {
            if (rowIndex >= 0 && _dataSource.Count >= rowIndex)
            {
                txtDriveName.Text = _dataSource[rowIndex].driveName;
                txtModel.Text = _dataSource[rowIndex].model;
                txtTotalSize.Text = _dataSource[rowIndex].driveSize.ToString();
                txtTotalHeads.Text = _dataSource[rowIndex].totalHeads.ToString();
                txtTotalTracks.Text = _dataSource[rowIndex].totalTracks.ToString();
                txtTotalSectors.Text = _dataSource[rowIndex].totalSectors.ToString();

                grdVolumes.DataSource = null;
                grdVolumes.DataSource = _dataSource[rowIndex].volumes;

                txtFirstHash.Text = _dataSource[rowIndex].firstHash;
                txtSecondHash.Text = _dataSource[rowIndex].secondHash;
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
            if (grdDevices.CurrentRow != null)
            {
                updateDetails(grdDevices.CurrentRow.Index);
                _actDrive = _dataSource[grdDevices.CurrentCell.RowIndex];
            }
        }

        private void updateDeviceGrid()
        {
            _deviceCollector.repollDevices();
            _dataSource = new BindingList<UsbDrive>(_deviceCollector.drives);
            

            DataGridView dg = grdDevices;
            Action d = () =>
            {
                if (!_dataSource.Contains(_actDrive))
                {
                    btnCleanup.PerformClick();
                    nextState(ApplicationState.PrepareForTests);
                }

                dg.DataSource = _dataSource;
                updateForm();
            };
            dg.Invoke(d);

        }

        private void btnCancelOp_Click(object sender, EventArgs e)
        {
            _hashWorker.CancelAsync();
            nextState(ApplicationState.CleanupDevice);
        }

        private void btnRunTests_Click(object sender, EventArgs e)
        {
            if (_dataSource.Count > 0)
            {
                DialogResult result = yncMsgBox("This action could delete all contents on your drive! Continue?");
                if (result == DialogResult.Yes)
                {
                    TestMeta testMeta = _dataSource[grdDevices.CurrentRow.Index].getTestVolume();
                    if (testMeta.hasValidVolume)
                    {
                        WriteTester writeTester = new WriteTester(testMeta);
                        logAdd(writeTester.test1_CreateFolder());
                        logAdd(writeTester.test2_CreateFile());
                        logAdd(writeTester.test3_OverwriteFile());
                        logAdd(writeTester.test4_SetFileAttributes());
                        logAdd(writeTester.test5_WriteToFile());
                        logAdd(writeTester.test55_SetDirCreateTime());
                        logAdd(writeTester.test6_DeleteFile());
                        logAdd(writeTester.test7_DeleteFolder());
                        logAdd(writeTester.test65_HardLink());
                        logAdd(writeTester.test8_ShellScript());
                    }
                    nextState(ApplicationState.SecondChecksum);
                }
            }
            else
            {
                okMsgBox("Tests can only be executed with a disk selected in the 'USB Devices' grid.");
            }
        }

        private void btnPrepare_Click(object sender, EventArgs e)
        {
            if (_dataSource.Count > 0)
            {
                DialogResult result =
                    yncMsgBox("Write locking must be disabled when preparing the volume for testing! Continue?");
                if (result == DialogResult.Yes)
                {
                    logAdd("Preparation process started.");
                    TestMeta testMeta = _dataSource[grdDevices.CurrentRow.Index].getTestVolume();
                    VolumePreparer volumePreparer = new VolumePreparer(testMeta);
                    logAdd(volumePreparer.CreateFile());
                    logAdd(volumePreparer.CreateFolder());
                    logAdd("Preparation process finished.");
                    nextState(ApplicationState.WriteProtectionEnabled);
                }
            }
            else
            {
                okMsgBox("Preparation can only be executed with a disk selected in the 'USB Devices' grid.");
            }
        }

        private void btnWriteEnabled_Click(object sender, EventArgs e)
        {
            nextState(ApplicationState.FirstChecksum);
        }

        private void btnSecondHash_Click(object sender, EventArgs e)
        {
            btnCheckSum1_Click(sender, e);
        }

        private void btnCleanup_Click(object sender, EventArgs e)
        {
            _deviceCollector.ClearHashes();
            if (_actDrive != null)
            {
                logAdd("Cleaning up preparation files.");
                _actDrive.testMeta.Clear();
                _actDrive.testMeta = null;
            }

            if (grdDevices.CurrentRow != null) updateDetails(grdDevices.CurrentRow.Index);
            nextState(ApplicationState.PrepareForTests);
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
            return MessageBox.Show(text, @"USB Writelock Test Application", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
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
                rtbLog.AppendText(
                    Environment.NewLine + DateTime.Now.ToLocalTime() + ": " + logMsg + Environment.NewLine);
            }

            rtbLog.ScrollToCaret();
        }

        private void grdDevices_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            okMsgBox("The device was removed. Application will be set to initial state.");
            e.ThrowException = false;
            btnCleanup.PerformClick();
        }

        // the "state machine" implementation for the assistant function
        private void nextState(ApplicationState nextState)
        {
            _state = nextState;
            switch (nextState)
            {
                case ApplicationState.PrepareForTests:
                    progressBar.Value = 0;
                    btnCancelOp.Enabled = false;
                    btnCheckSum1.Enabled = false;
                    pictWorking.Visible = false;
                    pictWorking.Enabled = false;
                    lblPercentage.Visible = false;
                    try
                    {
                        // causes null reference with datasource when device removed 
                        // while hashworker running
                        grdDevices.Enabled = true;
                    }
                    catch (Exception e)
                    {
                        logAdd(e.ToString());
                    }

                    grdVolumes.Enabled = true;
                    btnRunTests.Enabled = false;
                    btnPrepare.Enabled = true;
                    btnWriteEnabled.Enabled = false;
                    btnWriteProtectionDisabled.Enabled = false;
                    btnSecondHash.Enabled = false;
                    btnCleanup.Enabled = false;
                    txtFirstHash.Text = String.Empty;
                    txtSecondHash.Text = String.Empty;
                    txtFirstHash.BackColor = DefaultBackColor;
                    txtSecondHash.BackColor = DefaultBackColor;
                    break;
                case ApplicationState.WriteProtectionEnabled:
                    progressBar.Value = 0;
                    btnCancelOp.Enabled = true;
                    btnCheckSum1.Enabled = false;
                    pictWorking.Visible = false;
                    pictWorking.Enabled = false;
                    lblPercentage.Visible = false;
                    grdDevices.Enabled = false;
                    grdVolumes.Enabled = false;
                    btnRunTests.Enabled = false;
                    btnPrepare.Enabled = false;
                    btnWriteEnabled.Enabled = true;
                    btnWriteProtectionDisabled.Enabled = false;
                    btnSecondHash.Enabled = false;
                    btnCleanup.Enabled = false;
                    break;
                case ApplicationState.FirstChecksum:
                    progressBar.Value = 0;
                    btnCancelOp.Enabled = true;
                    btnCheckSum1.Enabled = true;
                    pictWorking.Visible = false;
                    pictWorking.Enabled = false;
                    lblPercentage.Visible = false;
                    grdDevices.Enabled = false;
                    grdVolumes.Enabled = false;
                    btnRunTests.Enabled = false;
                    btnPrepare.Enabled = false;
                    btnWriteEnabled.Enabled = false;
                    btnWriteProtectionDisabled.Enabled = false;
                    btnSecondHash.Enabled = false;
                    btnCleanup.Enabled = false;
                    break;
                case ApplicationState.FirstHashCalculation:
                    btnCancelOp.Enabled = true;
                    btnCheckSum1.Enabled = false;
                    pictWorking.Visible = true;
                    pictWorking.Enabled = true;
                    this.lblPercentage.Visible = true;
                    grdDevices.Enabled = false;
                    grdVolumes.Enabled = false;
                    btnRunTests.Enabled = false;
                    btnPrepare.Enabled = false;
                    btnWriteEnabled.Enabled = false;
                    btnWriteProtectionDisabled.Enabled = false;
                    btnSecondHash.Enabled = false;
                    btnCleanup.Enabled = false;
                    break;
                case ApplicationState.RunTests:
                    progressBar.Value = 0;
                    btnCancelOp.Enabled = true;
                    btnCheckSum1.Enabled = false;
                    pictWorking.Visible = false;
                    pictWorking.Enabled = false;
                    lblPercentage.Visible = false;
                    grdDevices.Enabled = false;
                    grdVolumes.Enabled = false;
                    btnRunTests.Enabled = true;
                    btnPrepare.Enabled = false;
                    btnWriteEnabled.Enabled = false;
                    btnWriteProtectionDisabled.Enabled = false;
                    btnSecondHash.Enabled = false;
                    btnCleanup.Enabled = false;
                    break;
                case ApplicationState.SecondChecksum:
                    progressBar.Value = 0;
                    btnCancelOp.Enabled = true;
                    btnCheckSum1.Enabled = false;
                    pictWorking.Visible = false;
                    pictWorking.Enabled = false;
                    lblPercentage.Visible = false;
                    grdDevices.Enabled = false;
                    grdVolumes.Enabled = false;
                    btnRunTests.Enabled = false;
                    btnPrepare.Enabled = false;
                    btnWriteEnabled.Enabled = false;
                    btnWriteProtectionDisabled.Enabled = false;
                    btnSecondHash.Enabled = true;
                    btnCleanup.Enabled = false;
                    break;
                case ApplicationState.SecondHashCalculation:
                    btnCancelOp.Enabled = true;
                    btnCheckSum1.Enabled = false;
                    pictWorking.Visible = true;
                    pictWorking.Enabled = true;
                    this.lblPercentage.Visible = true;
                    grdDevices.Enabled = false;
                    grdVolumes.Enabled = false;
                    btnRunTests.Enabled = false;
                    btnPrepare.Enabled = false;
                    btnWriteEnabled.Enabled = false;
                    btnWriteProtectionDisabled.Enabled = false;
                    btnSecondHash.Enabled = false;
                    btnCleanup.Enabled = false;
                    break;
                case ApplicationState.CleanupDevice:
                    progressBar.Value = 0;
                    btnCancelOp.Enabled = false;
                    btnCheckSum1.Enabled = false;
                    pictWorking.Visible = false;
                    pictWorking.Enabled = false;
                    lblPercentage.Visible = false;
                    grdDevices.Enabled = false;
                    grdVolumes.Enabled = false;
                    btnRunTests.Enabled = false;
                    btnPrepare.Enabled = false;
                    btnWriteEnabled.Enabled = false;
                    btnWriteProtectionDisabled.Enabled = false;
                    btnSecondHash.Enabled = false;
                    btnCleanup.Enabled = true;
                    break;
            }
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
    }
}
