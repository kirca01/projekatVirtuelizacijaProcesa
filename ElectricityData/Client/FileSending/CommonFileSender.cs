using Client.FileInUseCheck;
using Common.Files;
using Common.Interface;
using Common.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.FileSending
{
    public class FileSender : IFileSender
    {
        private readonly IElectricityData proxy;
        private readonly IFileInUseChecker fileInUseChecker;
        public FileSender(IElectricityData proxy, IFileInUseChecker fileInUseChecker)
        {
            this.proxy = proxy;
            this.fileInUseChecker = fileInUseChecker;
        }

        public void SendFiles(string path)                                      
        {                                                                       
            FileType fileType;
            string[] files = GetAllFiles(path);
            foreach (string filePath in files)
            {
                fileType = filePath.Contains("forecast") ? FileType.Prog : FileType.Ostv;
                SendFile(filePath, fileType);
            }
        }

        public void SendFile(string filePath, FileType fileType)                               
        {                                                                                       
            var fileName = Path.GetFileName(filePath);
            StreamFile streamFile = new StreamFile(GetMemoryStream(filePath), fileName);
            proxy.HandleFile(streamFile, fileType);                                             
            streamFile.Dispose();                                                               
        }
        
        private MemoryStream GetMemoryStream(string filePath)                          
        {
            // IMPLEMENTIRATI
            MemoryStream ms = new MemoryStream();
            if (fileInUseChecker.IsFileInUse(filePath))
            {
                Console.WriteLine($"Cannot process the file {Path.GetFileName(filePath)}. It's being in use by another process or it has been deleted.");
                return ms;
            }

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(ms);
                fileStream.Close();
            }
            return ms;
        }
        private static string[] GetAllFiles(string path)
        {
            return Directory.GetFiles(path, "*.csv*", SearchOption.TopDirectoryOnly);
        }

    }
}
