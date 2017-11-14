using System;
using System.IO;

namespace usbWriteLockTest.data
{
    public class TestMeta
    {
        public const int CMinfreespace = 3000000;
        private const string GuidPrefix = "WLTMP_";

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
            return GuidPrefix + Guid.NewGuid().ToString("D");
        }

        public void Clear()
        {
            try
            {
                Directory.Delete(preDirName, true);
                File.Delete(preFileName);
            }
            catch (Exception)
            {
                // this will not succeed if the write blocking mechanism is still applied
            }
        }
    }
}
