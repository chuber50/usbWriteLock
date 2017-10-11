using System;
using System.Management;

namespace usbWriteLockTest.logic
{
    class PnpEventWatcher : IDisposable
    {
        private readonly ManagementEventWatcher _pnpWatcher;
        private readonly Action _triggerAction;

        public PnpEventWatcher(Action triggerAction)
        {
            _triggerAction = triggerAction; // delegate 

            // 'GROUP WITHIN' 3 collects all events over three seconds to avoid several events firing on one pnp event
            _pnpWatcher = new ManagementEventWatcher();
            _pnpWatcher.EventArrived += pnpDetected;
            _pnpWatcher.Query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 OR EventType = 3 GROUP WITHIN 3");
            _pnpWatcher.Start();
        }

        void pnpDetected(object sender, EventArrivedEventArgs e)
        {
            if (sender != _pnpWatcher) return;
            _triggerAction();
        }

        ~PnpEventWatcher()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            _pnpWatcher.Stop();
            _pnpWatcher.Dispose();
        }
    }
}
