using Common.Models;
using InMemoryDataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlDataBase;

namespace DataAccess
{
    public class DatabaseManager                   
    {
        private string databaseType;
        private string xmlLoadPath = "data\\TBL_LOAD.xml";
        private string xmlAudithPath = "data\\TBL_AUDIT.xml";
        private string xmlImportedFilePath = "data\\TBL_IMPORTED_FILE.xml";


        private XmlLoad xmlLoad;
        private XmlAudit xmlAudit;
        private XmlImportedFile xmlImportedFile;
        public DatabaseManager(string Database)
        {
            databaseType = Database;
            if (databaseType == DatabaseType.Xml.ToString())             
            {
                xmlLoad = new XmlLoad(xmlLoadPath);
                xmlAudit = new XmlAudit(xmlAudithPath);
                xmlImportedFile = new XmlImportedFile(xmlImportedFilePath);
            }
        }

        public void UpdateData(List<Load> loads)
        {
            if (databaseType == DatabaseType.Xml.ToString())           
            {
                foreach(Load l in loads)
                {
                    xmlLoad.UpdateLoad(l.Id, l);                        
                }
            }
            else
            {
                foreach (Load l in loads)
                {
                    InMemoryLoad.Instance.UpdateLoad(l.Id, l);          
                }
            }
        }

        public List<Load> GetLoads()
        {
            if (databaseType == DatabaseType.Xml.ToString())
            {
                return xmlLoad.GetLoadData();
            }
            else
            {
                return InMemoryLoad.Instance.GetLoadData();
            }
        }

        public List<Audit> GetAudits()
        {
            if (databaseType == DatabaseType.Xml.ToString())
            {
                return xmlAudit.GetAuditData();
            }
            else
            {
                return InMemoryAudit.Instance.GetAuditData();
            }
        }
        public List<ImportedFile> GetImportedFiles()
        {
            if (databaseType == DatabaseType.Xml.ToString())
            {
                return xmlImportedFile.GetImportedFiles();
            }
            else
            {
                return InMemoryImportedFile.Instance.GetImportedFileData();
            }
        }

        public void AddLoad(Load load)
        {
            if (databaseType == DatabaseType.Xml.ToString())
            {
                xmlLoad.InsertLoad(load);
            }
            else
            {
                InMemoryLoad.Instance.InsertLoad(load.Id, load);
            }
        }
        public Load GetLoadByTimeStamp(DateTime timeStamp)
        {
            Load load;
            if (databaseType == DatabaseType.Xml.ToString())
            {
                load = xmlLoad.GetLoadByTimeStamp(timeStamp);
                
            }
            else
            {
                load = InMemoryLoad.Instance.GetLoadByTimeStamp(timeStamp);
            }
            return load;
        }
        public void UpdateLoad(Load load)
        {
            if (databaseType == DatabaseType.Xml.ToString())
            {
                xmlLoad.UpdateLoad(load.Id, load);
            }
            else
            {
                InMemoryLoad.Instance.UpdateLoad(load.Id, load);
            }
        }
        public void AddImportedFile(ImportedFile importedFile)
        {
            if (databaseType == DatabaseType.Xml.ToString())
            {
                xmlImportedFile.AddImportedFile(importedFile);
            }
            else
            {
                InMemoryImportedFile.Instance.InsertImportedFile(importedFile.Id, importedFile);
            }
        }
        public void AddAudit(Audit audit)
        {
            if (databaseType == DatabaseType.Xml.ToString())
            {
                xmlAudit.AddAudit(audit);
            }
            else
            {
                InMemoryAudit.Instance.InsertAudit(audit.Id, audit);
            }
        }
    }
}
