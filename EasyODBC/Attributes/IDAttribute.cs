using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyODBC.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IDAttribute:Attribute, IFieldAttribute
    {
        public bool AutoGenered { get; set; } = true;
        public string FieldName { get; set; }
      
    }
}
