using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [DataContract]
    public class Load
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime TimeStamp { get; set; }
        [DataMember]
        public decimal ForecastValue { get; set; }
        [DataMember]
        public decimal MeasuredValue { get; set; }
        [DataMember]
        public decimal AbsolutePercentageDeviation { get; set; }
        [DataMember]
        public decimal SquaredDeviation { get; set; }
        [DataMember]
        public int ForecastFileId { get; set; }
        [DataMember]
        public int MeasuredFileId { get; set; }

        static int counter = 1;

        public Load() { }

        public Load(DateTime timeStamp, decimal forecastValue, int forecastFileId)             
        {
            Id = counter++;
            TimeStamp = timeStamp;
            ForecastValue = forecastValue;
            ForecastFileId = forecastFileId;
        }
        public Load(DateTime timeStamp, int measuredFileId, decimal measuredValue)              
        {
            Id = counter++;
            TimeStamp = timeStamp;
            MeasuredValue = measuredValue;
            MeasuredFileId = measuredFileId;
        }
    }
}
