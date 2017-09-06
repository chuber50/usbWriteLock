using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace usbWriteLockTest.logic
{
    class PnpEventWatcher : IDisposable
    {
        // used for monitoring plugging and unplugging of USB devices.
        private ManagementEventWatcher _watcherAttach;
        private ManagementEventWatcher _watcherDetach;
        private Action _triggerAction;

        public PnpEventWatcher(Action triggerAction)
        {
            _triggerAction = triggerAction;
            //Win32_VolumeChangeEvent

            // Add USB plugged event watching
            _watcherAttach = new ManagementEventWatcher();
            _watcherAttach.EventArrived += attaching;
            _watcherAttach.Query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
            _watcherAttach.Start();

            // Add USB unplugged event watching
            _watcherDetach = new ManagementEventWatcher();
            _watcherDetach.EventArrived += detaching;
            _watcherDetach.Query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");
            _watcherDetach.Start();
        }

        public void Dispose()
        {
            _watcherAttach.Stop();
            _watcherDetach.Stop();
            //you may want to yield or Thread.Sleep
            _watcherAttach.Dispose();
            _watcherDetach.Dispose();
            //you may want to yield or Thread.Sleep
        }

        void attaching(object sender, EventArrivedEventArgs e)
        {
            if (sender != _watcherAttach) return;
            _triggerAction();
        }

        void detaching(object sender, EventArrivedEventArgs e)
        {
            if (sender != _watcherDetach) return;
            _triggerAction();
        }

        ~PnpEventWatcher()
        {
            this.Dispose();// for ease of readability I left out the complete Dispose pattern
        }
    }
}
