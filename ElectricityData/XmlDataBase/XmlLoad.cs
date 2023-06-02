using Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace XmlDataBase
{
    public class XmlLoad
    {
        private readonly string xmlFilePath;    

        public XmlLoad(string xmlFilePath)
        {
            this.xmlFilePath = xmlFilePath;
            CreateXmlFile();
        }

        public List<Load> GetLoadData()                         
        {
            XDocument xmlDoc = XDocument.Load(xmlFilePath);        

            return xmlDoc.Root.Elements("Row")
                .Select(e => CreateLoadFromXml(e))
                .ToList();
        }

        public bool InsertLoad(Load load)
        {
            XDocument xmlDoc = XDocument.Load(xmlFilePath);

            int nextId = xmlDoc.Root.Elements("Row").Any()
                ? xmlDoc.Root.Elements("Row").Max(e => int.Parse(e.Element("Id").Value)) + 1       
                : 1;

            XElement newLoadElement = CreateLoadXmlElement(nextId, load);            
            xmlDoc.Root.Add(newLoadElement);
            xmlDoc.Save(xmlFilePath);

            return true;
        }

        public Load GetLoadByTimeStamp(DateTime timeStamp)
        {
            XDocument xmlDoc = XDocument.Load(xmlFilePath);

            XElement loadElement = xmlDoc.Root.Elements("Row").FirstOrDefault(e => DateTime.Parse(e.Element("TimeStamp").Value) == timeStamp);
            if (loadElement != null)
            {
                return CreateLoadFromXml(loadElement);                      
            }

            return null;
        }

        public void UpdateLoad(int id, Load updatedLoad)
        {
            XDocument xmlDoc = XDocument.Load(xmlFilePath);

            XElement loadElement = xmlDoc.Root.Elements("Row").FirstOrDefault(e => int.Parse(e.Element("Id").Value) == id);
            if (loadElement != null)
            {
                UpdateLoadXml(loadElement, updatedLoad);
                xmlDoc.Save(xmlFilePath);
            }
        }

        private Load CreateLoadFromXml(XElement loadElement)
        {
            return new Load
            {
                Id = int.Parse(loadElement.Element("Id").Value),
                ForecastValue = decimal.Parse(loadElement.Element("ForecastValue").Value, NumberFormatInfo.InvariantInfo),
                MeasuredValue = decimal.Parse(loadElement.Element("MeasuredValue").Value, NumberFormatInfo.InvariantInfo),
                AbsolutePercentageDeviation = decimal.Parse(loadElement.Element("AbsolutePercentageDeviation").Value, NumberFormatInfo.InvariantInfo),
                SquaredDeviation = decimal.Parse(loadElement.Element("SquaredDeviation").Value, NumberFormatInfo.InvariantInfo),
                TimeStamp = DateTime.Parse(loadElement.Element("TimeStamp").Value),
                ForecastFileId = int.Parse(loadElement.Element("ForecastFileId").Value),
                MeasuredFileId = int.Parse(loadElement.Element("MeasuredFileId").Value)

            };
        }

        private XElement CreateLoadXmlElement(int id, Load load)
        {
            return new XElement("Row",
                new XElement("Id", id),
                new XElement("ForecastValue", load.ForecastValue.ToString(NumberFormatInfo.InvariantInfo)),
                new XElement("MeasuredValue", load.MeasuredValue.ToString(NumberFormatInfo.InvariantInfo)),
                new XElement("AbsolutePercentageDeviation", load.AbsolutePercentageDeviation.ToString(NumberFormatInfo.InvariantInfo)),
                new XElement("SquaredDeviation", load.SquaredDeviation.ToString(NumberFormatInfo.InvariantInfo)),
                new XElement("TimeStamp", load.TimeStamp),
                new XElement("ForecastFileId", load.ForecastFileId),
                new XElement("MeasuredFileId", load.MeasuredFileId));
        }

        private void UpdateLoadXml(XElement loadElement, Load updatedLoad)
        {
            loadElement.Element("ForecastValue").Value = updatedLoad.ForecastValue.ToString(NumberFormatInfo.InvariantInfo);
            loadElement.Element("MeasuredValue").Value = updatedLoad.MeasuredValue.ToString(NumberFormatInfo.InvariantInfo);
            loadElement.Element("AbsolutePercentageDeviation").Value = updatedLoad.AbsolutePercentageDeviation.ToString(NumberFormatInfo.InvariantInfo);
            loadElement.Element("SquaredDeviation").Value = updatedLoad.SquaredDeviation.ToString(NumberFormatInfo.InvariantInfo);
            loadElement.Element("TimeStamp").Value = updatedLoad.TimeStamp.ToString();
            loadElement.Element("ForecastFileId").Value = updatedLoad.ForecastFileId.ToString();
            loadElement.Element("MeasuredFileId").Value = updatedLoad.MeasuredFileId.ToString();
        }

        private void CreateXmlFile()
        {
            XDocument newXmlDoc = new XDocument(new XElement("Rows"));
            newXmlDoc.Save(xmlFilePath);
        }
    }

}