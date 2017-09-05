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
        private ManagementEventWatcher watcherAttach;
        private ManagementEventWatcher watcherDetach;
        private Action TriggerAction;

        public PnpEventWatcher(Action triggerAction)
        {
            TriggerAction = triggerAction;
            //Win32_VolumeChangeEvent

            // Add USB plugged event watching
            watcherAttach = new ManagementEventWatcher();
            watcherAttach.EventArrived += Attaching;
            watcherAttach.Query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
            watcherAttach.Start();

            // Add USB unplugged event watching
            watcherDetach = new ManagementEventWatcher();
            watcherDetach.EventArrived += Detaching;
            watcherDetach.Query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");
            watcherDetach.Start();
        }

        public void Dispose()
        {
            watcherAttach.Stop();
            watcherDetach.Stop();
            //you may want to yield or Thread.Sleep
            watcherAttach.Dispose();
            watcherDetach.Dispose();
            //you may want to yield or Thread.Sleep
        }

        void Attaching(object sender, EventArrivedEventArgs e)
        {
            if (sender != watcherAttach) return;
            TriggerAction();
        }

        void Detaching(object sender, EventArrivedEventArgs e)
        {
            if (sender != watcherDetach) return;
            TriggerAction();
        }

        ~PnpEventWatcher()
        {
            this.Dispose();// for ease of readability I left out the complete Dispose pattern
        }
    }
}
