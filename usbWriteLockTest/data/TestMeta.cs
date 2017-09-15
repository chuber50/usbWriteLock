using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usbWriteLockTest.data
{
    public class TestMeta
    {
        public const int CMinfreespace = 3000000;

        public TestMeta(LogicalVolume volume)
        {
            if (volume != null)
            {
                volumePath = volume.name;
                preDirName = Path.Combine(volumePath, generateUniqueId());
                preFileName = Path.Combine(volumePath, generateUniqueId());
                hasValidVolume = true;
            }
        }

        public string volumePath { get; set; }
        public string preDirName { get; }
        public string preFileName { get; }
        public bool hasValidVolume { get; }

        public string getArbitraryFileName => Path.Combine(volumePath, generateUniqueId());

        private string generateUniqueId()
        {
            return "WLTMP_" + Guid.NewGuid().ToString("D");
        }
    }
}
