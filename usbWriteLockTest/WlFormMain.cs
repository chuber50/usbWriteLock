using System;
using System.ComponentModel;
using System.Windows.Forms;
using usbWriteLockTest.logic;

namespace usbWriteLockTest
{
    public partial class WlFormMain : Form
    {
        private PnpEventWatcher _watcher;
        private readonly DeviceCollector _deviceCollector = new DeviceCollector();
        private BackgroundWorker _hashWorker;

        public WlFormMain()
        {
            InitializeComponent();
        }

        private void WlFormMain_Load(object sender, EventArgs e)
        {
            grdDevices.DataSource = _deviceCollector.drives;
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

        private void updateGrid()
        {
            grdDevices.Update();
            grdDevices.Refresh();
        }

        private void updateDeviceGrid()
        {
            _deviceCollector.repollDevices();
            if (this.grdDevices.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = updateGrid;
                this.Invoke(d, new Object[] { });
            }
            else
            {
                updateGrid();
            }
        }

        delegate void StringArgReturningVoidDelegate();

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

        //http://www.infinitec.de/post/2007/06/09/Displaying-progress-updates-when-hashing-large-files.aspx
        private void btnCheckSum1_Click(object sender, EventArgs e)
        {
            DeviceHandler deviceHandler = new DeviceHandler(_deviceCollector.drives[grdDevices.CurrentCell.RowIndex]);
            deviceHandler.LockVolumes();

            //ProgressPercentage.Visible = true;
            //ProgressBar.Visible = true;
            //StatusText.Text = "Computing hash...";

            if (!_hashWorker.IsBusy)
            {
                _hashWorker.RunWorkerAsync();
            }
            deviceHandler.UnlockVolumes();
        }

        private void hashWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            AsyncHashCalculator hashCalculator = new AsyncHashCalculator(worker, _deviceCollector.drives[grdDevices.CurrentCell.RowIndex]);
            e.Result = hashCalculator.ComputeHash();
        }

        private void hashWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // Next, handle the case where the user canceled 
                // the operation.
                // Note that due to a race condition in 
                // the DoWork event handler, the Cancelled
                // flag may not have been set, even though
                // CancelAsync was called.
                lstBoxLog.Items.Add("Canceled");
            }
            else
            {
                // Finally, handle the case where the operation 
                // succeeded.
                grdHashes.DataSource = _deviceCollector.drives[grdDevices.CurrentCell.RowIndex].hashes;
                grdVolumes.Update();
                grdVolumes.Refresh();
            }

            // Enable the Start button.
            btnCheckSum1.Enabled = true;

            // Disable the Cancel button.
            btnCancelOp.Enabled = false;
        }

        // This event handler updates the progress bar.
        private void hashWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
        }

        private void grdDevices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtDriveName.Text = _deviceCollector.drives[e.RowIndex].driveName;
            txtModel.Text = _deviceCollector.drives[e.RowIndex].model;
            txtTotalSize.Text = _deviceCollector.drives[e.RowIndex].driveSize.ToString();
            txtSectorsTrack.Text = _deviceCollector.drives[e.RowIndex].sectorsPerTrack.ToString();
            txtTracksCyl.Text = _deviceCollector.drives[e.RowIndex].tracksPerCylinder.ToString();
            txtBytesSector.Text = _deviceCollector.drives[e.RowIndex].bytesPerSector.ToString();
            txtTotalCyl.Text = _deviceCollector.drives[e.RowIndex].totalCylinders.ToString();
            txtTotalHeads.Text = _deviceCollector.drives[e.RowIndex].totalHeads.ToString();
            txtTotalTracks.Text = _deviceCollector.drives[e.RowIndex].totalTracks.ToString();
            txtTotalSectors.Text = _deviceCollector.drives[e.RowIndex].totalSectors.ToString();
    
            grdVolumes.DataSource = _deviceCollector.drives[e.RowIndex].volumes;
            grdVolumes.Update();
            grdVolumes.Refresh();
        }

    }
}
