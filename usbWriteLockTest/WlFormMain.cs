using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using usbWriteLockTest.data;
using usbWriteLockTest.logic;

namespace usbWriteLockTest
{
    public partial class WlFormMain : Form
    {
        private PnpEventWatcher _watcher;
        private readonly DeviceCollector _deviceCollector = new DeviceCollector();

        public WlFormMain()
        {
            InitializeComponent();
        }

        private void WlFormMain_Load(object sender, EventArgs e)
        {
            grdDevices.DataSource = _deviceCollector.drives;
            _watcher = new PnpEventWatcher(this.updateDeviceGrid);
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

            BackgroundWorker.RunWorkerAsync(_deviceCollector.drives[grdDevices.CurrentCell.RowIndex);

            deviceHandler.GenerateChecksum();
            deviceHandler.UnlockVolumes();
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
