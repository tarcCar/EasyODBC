using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyODBC.Attributes
{
    public interface IFieldAttribute
    {
        bool AutoGenered { get; set; }
        string FieldName { get; set; }
    }
}
