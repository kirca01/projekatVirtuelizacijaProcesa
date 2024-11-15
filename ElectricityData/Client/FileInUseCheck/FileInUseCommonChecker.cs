﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.FileInUseCheck
{
    public class FileInUseCommonChecker : IFileInUseChecker                 
    {
        private const int TIMEOUT = 10;
        private const int WAIT_MILLISECONDS = 500;

        public bool IsFileInUse(string filePath)
        {
            int cnt = 0;
            while (cnt < TIMEOUT && CheckIsFileInUse(new FileInfo(filePath)))
            {
                Thread.Sleep(WAIT_MILLISECONDS);
                cnt++;
            }
            if (cnt >= TIMEOUT)
                return true;
            return false;
        }

        private bool CheckIsFileInUse(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {

                return true;
            }
            finally
            {
                if (stream != null)

                    stream.Dispose();
            }
            return false;
        }
    }
}
