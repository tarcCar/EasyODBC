using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyODBC.Attributes
{
    [AttributeUsage(AttributeTargets.Class |
                       AttributeTargets.Struct)
    ]
    public class TableAttribute:Attribute
    {
        public string tableName;
        public string dataBaseName;
    }
}
