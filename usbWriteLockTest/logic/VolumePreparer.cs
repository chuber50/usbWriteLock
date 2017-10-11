using System;
using System.IO;
using usbWriteLockTest.data;

namespace usbWriteLockTest.logic
{
    class VolumePreparer
    {
        private const string CMsgSuccess = "Success";
        private readonly TestMeta _testMeta;

        public VolumePreparer(TestMeta testMeta)
        {
            _testMeta = testMeta;
        }

        public string CreateFolder()
        {
            string msg = CMsgSuccess;
            try
            {
                Directory.CreateDirectory(_testMeta.preDirName);
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return $"Trying to create folder {_testMeta.preDirName}: {msg}";
        }

        public string CreateFile()
        {
            string msg = CMsgSuccess;
            try
            {
                if (File.Exists(_testMeta.preFileName))
                {
                    msg = "Already exists.";
                }
                else
                {
                    var tmpHdl = File.Create(_testMeta.preFileName);
                    tmpHdl.Close();
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return $"Trying to create file {_testMeta.preDirName}: {msg}";
        }
    }
}
