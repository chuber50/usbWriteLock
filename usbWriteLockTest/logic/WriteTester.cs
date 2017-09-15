using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using usbWriteLockTest.data;

namespace usbWriteLockTest.logic
{
    class WriteTester
    {
        private const string CMsgSuccess = "Success";
        private TestMeta _testMeta;

        public WriteTester(TestMeta testMeta)
        {
            _testMeta = testMeta;
        }

        public string test1_CreateFolder()
        {
            string msg = CMsgSuccess;
            string path = _testMeta.getArbitraryFileName;
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return $"Trying to create arbitrary folder {path}: {msg}";
        }

        public string test2_CreateFile()
        {
            string msg = CMsgSuccess;
            string path = _testMeta.getArbitraryFileName;
            try
            {
                File.Create(path);
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return $"Trying to create arbitrary file {path}: {msg}";
        }

        public string test3_OverwriteFile()
        {
            string msg = CMsgSuccess;
            try
            {
                File.Create(_testMeta.preFileName);
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return $"Trying to overwrite precreated file {_testMeta.preFileName}: {msg}";
        }

        public string test4_SetFileAttributes()
        {
            string msg = CMsgSuccess;
            try
            {
                File.SetAttributes(_testMeta.preFileName, FileAttributes.ReadOnly);
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return $"Trying to change attributes of precreated file {_testMeta.preFileName}: {msg}";
        }

        public string test5_WriteToFile()
        {
            string msg = CMsgSuccess;
            try
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(_testMeta.preFileName);
                file.WriteLine($"{DateTime.Now}: writeLock Test Write {Environment.NewLine}");
                file.Close();
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return $"Trying to write a line into precreated file {_testMeta.preFileName}: {msg}";
        }

        public string test5_SetDirCreateTime()
        {
            string msg = CMsgSuccess;
            try
            {
                Directory.SetCreationTime(_testMeta.preDirName, DateTime.Now);
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return $"Trying to change attributes of precreated folder {_testMeta.preFileName}: {msg}";
        }

        public string test6_DeleteFile()
        {
            string msg = CMsgSuccess;
            try
            {
                File.Delete(_testMeta.preFileName);
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return $"Trying to delete precreated file {_testMeta.preFileName}: {msg}";
        }

        public string test7_DeleteFolder()
        {
            string msg = CMsgSuccess;
            try
            {
                Directory.Delete(_testMeta.preDirName);
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return $"Trying to delete precreated folder {_testMeta.preDirName}: {msg}";
        }

        //TODO: FORMAT
    }
}
