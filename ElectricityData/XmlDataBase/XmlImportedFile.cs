using Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XmlDataBase
{
    public class XmlImportedFile
    {
        private readonly string xmlFilePath;

        public XmlImportedFile(string xmlFilePath)
        {
            this.xmlFilePath = xmlFilePath;
            CreateXmlFile();
        }

        public List<ImportedFile> GetImportedFiles()
        {
            XDocument xmlDoc = XDocument.Load(xmlFilePath);

            return xmlDoc.Root.Elements("Row")
                .Select(e => CreateImportedFileFromXml(e))
                .ToList();
        }

        public void AddImportedFile(ImportedFile importedFile)
        {
            InsertImportedFile(importedFile);
        }

        public bool InsertImportedFile(ImportedFile importedFile)
        {
            XDocument xmlDoc = XDocument.Load(xmlFilePath);

            int nextId = xmlDoc.Root.Elements("Row").Any()
                ? xmlDoc.Root.Elements("Row").Max(e => int.Parse(e.Element("Id").Value)) + 1
                : 1;

            XElement newImportedFileElement = CreateImportedFileXmlElement(nextId, importedFile);
            xmlDoc.Root.Add(newImportedFileElement);
            xmlDoc.Save(xmlFilePath);

            return true;
        }

        private ImportedFile CreateImportedFileFromXml(XElement importedFileElement)
        {
            return new ImportedFile
            {
                Id = int.Parse(importedFileElement.Element("Id").Value),
                FileName = importedFileElement.Element("FileName").Value
            };
        }

        private XElement CreateImportedFileXmlElement(int id, ImportedFile importedFile)
        {
            return new XElement("Row",
                new XElement("Id", id),
                new XElement("FileName", importedFile.FileName));
        }

        private void CreateXmlFile()
        {
            if (!File.Exists(xmlFilePath))
            {
                XDocument newXmlDoc = new XDocument(new XElement("Rows"));
                newXmlDoc.Save(xmlFilePath);
            }
        }
    }

}
