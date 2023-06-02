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
    public class XmlAudit
    {

        private readonly string xmlFilePath;

        public XmlAudit(string xmlFilePath)
        {
            this.xmlFilePath = xmlFilePath;
            CreateXmlFile();
        }

        public List<Audit> GetAuditData()
        {
            XDocument xmlDoc = XDocument.Load(xmlFilePath);

            return xmlDoc.Root.Elements("Row")
                .Select(e => CreateAuditFromXml(e))
                .ToList();
        }

        public void AddAudit(Audit audit)
        {
            InsertAudit(audit);
        }

        public bool InsertAudit(Audit audit)
        {
            XDocument xmlDoc = XDocument.Load(xmlFilePath);

            int nextId = xmlDoc.Root.Elements("Row").Any()
                ? xmlDoc.Root.Elements("Row").Max(e => int.Parse(e.Element("Id").Value)) + 1
                : 1;

            XElement newAuditElement = CreateAuditXmlElement(nextId, audit);
            xmlDoc.Root.Add(newAuditElement);
            xmlDoc.Save(xmlFilePath);

            return true;
        }

        private Audit CreateAuditFromXml(XElement auditElement)
        {
            return new Audit
            {
                Id = int.Parse(auditElement.Element("Id").Value),
                TimeStamp = DateTime.Parse(auditElement.Element("TimeStamp").Value),
                MessageType = (MessageType)Enum.Parse(typeof(MessageType), auditElement.Element("MessageType").Value),
                Message = auditElement.Element("Message").Value
            };
        }

        private XElement CreateAuditXmlElement(int id, Audit audit)
        {
            return new XElement("Row",
                new XElement("Id", id),
                new XElement("TimeStamp", audit.TimeStamp),
                new XElement("MessageType", audit.MessageType),
                new XElement("Message", audit.Message));
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
