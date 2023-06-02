using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public enum DatabaseType
    {
        [EnumMember]Xml = 0,
        [EnumMember]InMemory = 1,
    }
}
