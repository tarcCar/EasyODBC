using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyODBC.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FieldAttribute:Attribute, IFieldAttribute
    {
        public bool IsTableField { get; set; } = true;
        public bool AutoGenered { get; set; } = false;
        public bool Insertable { get; set; } = true;
        public bool Updatable { get; set; } = true;
        public string FieldName { get; set; }
    }
}
