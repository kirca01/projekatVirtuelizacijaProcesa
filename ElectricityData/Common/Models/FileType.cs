using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public enum FileType
    {
        [EnumMember] Prog = 0,
        [EnumMember] Ostv = 1,
    }
}
