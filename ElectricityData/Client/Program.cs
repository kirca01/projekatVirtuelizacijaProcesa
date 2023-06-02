using Client.FileInUseCheck;
using Client.FileSending;
using Common.Interface;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IElectricityData> factory = new ChannelFactory<IElectricityData>("ElectricityDataService");
            IElectricityData proxy = factory.CreateChannel();

            List<Load> loads = new List<Load>();
            List<Audit> audits = new List<Audit>();
            List<ImportedFile> importedFiles = new List<ImportedFile>();

            Console.WriteLine("Unesite putanju forecast csv fajlova: ");
            string forecastPath = Console.ReadLine();
            Console.WriteLine("Unesite putanju measured csv fajlova: ");
            string measuredPath = Console.ReadLine();

            IFileSender fileSender = GetFileSender(proxy, GetFileInUseChecker());               
            fileSender.SendFiles(forecastPath);                                             
            fileSender.SendFiles(measuredPath);     

            proxy.CalculateDeviation();

            Console.WriteLine("In memory Data: ");

            loads = proxy.GetLoadData();
            audits = proxy.GetAuditData();
            importedFiles = proxy.GetImportedFileData();

            Console.WriteLine("LOADS: ");

            foreach (Load load in loads)
            {
                Console.WriteLine($"Load ID: {load.Id}");
                Console.WriteLine($"Forecast value: {load.ForecastValue}");
                Console.WriteLine($"Measured value: {load.MeasuredValue}");
                Console.WriteLine($"Absolute percentage deviation: {load.AbsolutePercentageDeviation}");
                Console.WriteLine($"Squared deviation: {load.SquaredDeviation}");
                Console.WriteLine($"Time stamp: {load.TimeStamp}");

                Console.WriteLine("---------------------------");
            }
            Console.WriteLine("AUDITS: ");

            foreach (Audit audit in audits)
            {
                Console.WriteLine($"Audit ID: {audit.Id}");
                Console.WriteLine($"Audit message: {audit.Message}");
                Console.WriteLine($"Audit type: {audit.MessageType}");
                Console.WriteLine($"Audit time stamp: {audit.TimeStamp}");

                Console.WriteLine("---------------------------");
            }
            foreach (ImportedFile iFile in importedFiles)
            {
                Console.WriteLine($"Imported file ID: {iFile.Id}");
                Console.WriteLine($"File name: {iFile.FileName}");

                Console.WriteLine("---------------------------");
            }

            Console.ReadLine();
        }

        private static IFileInUseChecker GetFileInUseChecker()                          
        {
            return new FileInUseCommonChecker();
        }
        private static IFileSender GetFileSender(IElectricityData proxy, IFileInUseChecker fileInUseChecker)
        {
            return new FileSender(proxy, fileInUseChecker);
        }
    }
}
