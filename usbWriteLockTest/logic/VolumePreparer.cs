using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    File.Create(_testMeta.preFileName);
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
