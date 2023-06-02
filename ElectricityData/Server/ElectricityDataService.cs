using Common.Files;
using Common.Interface;
using Common.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ElectricityDataService : IElectricityData
    {
        public delegate void Update(List<Load> loads);                            
        public event Update UpdateEvent;                                            

        private DatabaseManager _dataBaseManager;
        private string databaseType = ConfigurationManager.AppSettings["databaseType"];
        private string deviation = ConfigurationManager.AppSettings["deviatonType"];
        public ElectricityDataService()
        {
            _dataBaseManager = new DatabaseManager(databaseType);
            UpdateEvent += _dataBaseManager.UpdateData;
        }

        public void HandleFile(StreamFile streamFile, FileType fileType)
        {
            Load load;
            DateTime timeStamp;
            if (ValidateFile(streamFile.FileStream))                            
            {
                ImportedFile importedFile = new ImportedFile(streamFile.FileName);      
                _dataBaseManager.AddImportedFile(importedFile);
                streamFile.FileStream.Position = 0;                                     
                using (StreamReader reader = new StreamReader(streamFile.FileStream))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');

                        if ((values.Length == 2 && DateTime.TryParse(values[0], out timeStamp))         
                            && decimal.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal dataValue))
                        {
                            if (fileType == FileType.Prog)      
                            {
                                load = _dataBaseManager.GetLoadByTimeStamp(timeStamp);
                                if (load == null)                                                          
                                {
                                    load = new Load(timeStamp, Math.Round(dataValue, 2), importedFile.Id);
                                    _dataBaseManager.AddLoad(load);
                                }
                                else
                                {
                                    load.ForecastValue = Math.Round(dataValue, 2);
                                    load.ForecastFileId = importedFile.Id;
                                    _dataBaseManager.UpdateLoad(load);
                                }
                            }
                            else
                            {
                                load = _dataBaseManager.GetLoadByTimeStamp(timeStamp);  
                                if (load == null)
                                {
                                    load = new Load(timeStamp, importedFile.Id, Math.Round(dataValue, 2));
                                    _dataBaseManager.AddLoad(load);
                                }
                                else
                                {
                                    load.MeasuredValue = Math.Round(dataValue, 2);
                                    load.MeasuredFileId = importedFile.Id;
                                    _dataBaseManager.UpdateLoad(load);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void CalculateDeviation()                        
        {
            if(deviation.Equals("absolutePercentage"))          
            {
                AbsolutePercentageDeviation();
            }
            else
            {
                SquaredDeviation();
            }
        }

        private void AbsolutePercentageDeviation()
        {
            List<Load> loads;

            loads = _dataBaseManager.GetLoads();        

            foreach (var load in loads)
            {
                if (load.ForecastValue != 0 && load.MeasuredValue != 0)     
                {
                    decimal forecastValue = load.ForecastValue;
                    decimal measuredValue = load.MeasuredValue;

                    decimal absolutePercentageDeviation = Math.Abs(measuredValue - forecastValue) / measuredValue * 100;

                    load.AbsolutePercentageDeviation = Math.Round(absolutePercentageDeviation, 2);    
                }
            }
            UpdateEvent.Invoke(loads);    
        }
        private void SquaredDeviation()
        {
            List<Load> loads;

            loads = _dataBaseManager.GetLoads();

            foreach (var load in loads)
            {
                if (load.ForecastValue != 0 && load.MeasuredValue != 0)
                {
                    decimal forecastValue = load.ForecastValue;
                    decimal measuredValue = load.MeasuredValue;

                    decimal squaredDeviation = (measuredValue - forecastValue) / measuredValue;
                    squaredDeviation = squaredDeviation * squaredDeviation;

                    load.SquaredDeviation = Math.Round(squaredDeviation, 3);
                }
            }
            UpdateEvent.Invoke(loads);
        }

        private bool ValidateFile(MemoryStream fileStream)
        {
            StreamReader reader = new StreamReader(fileStream);
            List<string> lines = new List<string>();
            Audit audit = null;

            fileStream.Position = 0;
            while (!reader.EndOfStream)
            {
                lines.Add(reader.ReadLine());
            }

            if (lines.Count == 0)
            {
                audit = new Audit(DateTime.Now, MessageType.Error, "Datoteka je prazna.");
                _dataBaseManager.AddAudit(audit);
                return false;
            }

            int[] expectedRowCounts = { 23, 24, 25, 26 };
            int expectedRowCount = lines.Count;

            if (!expectedRowCounts.Contains(expectedRowCount))
            {
                audit = new Audit(DateTime.Now, MessageType.Error, "Broj redova u datoteci ne odgovara očekivanim vrednostima.");
                _dataBaseManager.AddAudit(audit);
                return false;
            }
            foreach (string line in lines)
            {
                if (line.Contains("TIME_STAMP"))
                    continue;
                string[] parts = line.Split(',');
                if (parts.Length != 2 || !DateTime.TryParse(parts[0], out _) || !double.TryParse(parts[1], out _))
                {
                    audit = new Audit(DateTime.Now, MessageType.Error, "Neispravan format reda u datoteci.");
                    _dataBaseManager.AddAudit(audit);
                    return false;
                }
            }

            try
            {
                fileStream.Position = 0;

                if (fileStream.Length == 0)
                {
                    audit = new Audit(DateTime.Now, MessageType.Error, "Datoteka je prazna, oštećena ili zaključana.");
                    _dataBaseManager.AddAudit(audit);
                    return false;
                }
            }
            catch (Exception)
            {
                audit = new Audit(DateTime.Now, MessageType.Error, "Greška prilikom pristupa datoteci. Možda je zaključana ili nedostupna.");
                _dataBaseManager.AddAudit(audit);
                return false;
            }


            return true;
        }

        public List<Load> GetLoadData()
        {
            return _dataBaseManager.GetLoads();
        }
        public List<Audit> GetAuditData()
        {
            return _dataBaseManager.GetAudits();
        }
        public List<ImportedFile> GetImportedFileData()
        {
            return _dataBaseManager.GetImportedFiles();
        }
    }
}
