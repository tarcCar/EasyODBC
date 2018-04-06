using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyODBC.Models
{
    public class ClassProperty
    {
        public ClassProperty(string name, object value, string field = null, bool insertable = true, bool updatable = true, bool autoGenered = false)
        {
            Name = name;
            Value = value;
            Field = field;
            Insertable = insertable;
            Updatable = updatable;
            AutoGenered = autoGenered;
        }

        public string Name { get; private set; }
        public object Value { get; private set; }
        public string Field { get; private set; }
        
        public bool Insertable { get; private set; }
        public bool Updatable { get; private set; }
        public bool AutoGenered { get; private set; }

        public string GetNameWithParameterIdentifier(string identifier)
        {
            return string.Concat(identifier, Name, identifier);
        }
    }
}
