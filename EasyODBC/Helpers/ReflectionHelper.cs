using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyODBC.Helpers
{
    public static class ReflectionHelper
    {
        public static string GetTableName(object obj)
        {
            Type type = obj.GetType();
            return GetTableName(type);
        }
        public static string GetTableName(Type type)
        {
            string dataBase = "";
            string table = type.Name;
            object[] attributes = type.GetCustomAttributes(typeof(Attributes.TableAttribute), false);

            if (attributes.Count() > 0)
            {
                Attributes.TableAttribute tableAttribute = attributes[0] as Attributes.TableAttribute;
                string dataBaseName = tableAttribute.dataBaseName;
                string tableName = tableAttribute.tableName;

                if (!string.IsNullOrEmpty(tableName))
                    table = tableName;
                if (!string.IsNullOrEmpty(dataBaseName))
                    dataBase = string.Concat(dataBaseName, ".");
            }

            return string.Concat(dataBase, table);
        }
        public static IList<Models.ClassProperty> GetClassProperties(object obj)
        {
            List<Models.ClassProperty> properties = new List<Models.ClassProperty>();
            IList<System.Reflection.PropertyInfo> props = GetProperties(obj);

            foreach (var prop in props)
            {
                properties.Add(new Models.ClassProperty(prop.Name, prop.GetValue(obj, null)));
            }
            return properties;
        }

        private static IList<System.Reflection.PropertyInfo> GetProperties(object obj)
        {
            Type type = obj.GetType();
            IList<System.Reflection.PropertyInfo> props = new List<System.Reflection.PropertyInfo>(type.GetProperties());
            return props;
        }
        private static string GetAttributeFieldName(Attributes.IFieldAttribute attribute)
        {
            string fieldName = attribute.FieldName;

            if (!string.IsNullOrWhiteSpace(fieldName))
                return fieldName;
            else
                return null;
        }
        public static IList<string> GetTypePropertiesFieldsNames(Type type)
        {
            IList<string> names = new List<string>();
            IList<System.Reflection.PropertyInfo> properties = type.GetProperties();
            foreach (var property in properties)
            {
                string propertyName = property.Name;
                object[] IDattributes = property.GetCustomAttributes(typeof(Attributes.IDAttribute), false);
                object[] FieldAttributes = property.GetCustomAttributes(typeof(Attributes.FieldAttribute), false);
                if (IDattributes.Count() > 0)
                {
                    string fieldName = GetAttributeFieldName(IDattributes[0] as Attributes.IFieldAttribute);
                    if (fieldName != null)
                    {
                        names.Add(fieldName);
                    }
                    else
                    {
                        names.Add(propertyName);
                    }
                }
                else if (FieldAttributes.Count() > 0)
                {
                    Attributes.FieldAttribute attribute = FieldAttributes[0] as Attributes.FieldAttribute;
                    if (!attribute.IsTableField)
                        continue;

                    string fieldName = GetAttributeFieldName(attribute);
                    if (fieldName != null)
                    {
                        names.Add(fieldName);
                    }
                    else
                    {
                        names.Add(propertyName);
                    }
                }
                else
                {
                    names.Add(propertyName);
                }
            }
            return names;
        }
        public static IList<Models.ClassProperty> GetClassPropertiesWithFieldNamesAndValues(object obj)
        {
            List<Models.ClassProperty> properties = new List<Models.ClassProperty>();
            IList<System.Reflection.PropertyInfo> props = GetProperties(obj);

            foreach (System.Reflection.PropertyInfo prop in props)
            {
                Models.ClassProperty classProperty = GetClassProperty(prop, obj);

                if (classProperty != null)
                    properties.Add(classProperty);
            }

            return properties;
        }

        private static Models.ClassProperty GetClassProperty(System.Reflection.PropertyInfo property, object obj)
        {
            bool autoGenered = false;
            bool insertable = true;
            bool updatable = true;

            object[] IDattributes = property.GetCustomAttributes(typeof(Attributes.IDAttribute), false);
            object[] FieldAttributes = property.GetCustomAttributes(typeof(Attributes.FieldAttribute), false);

            if (IDattributes.Count() > 0)
            {
                return GetIdClassProperty(obj, property, IDattributes[0] as Attributes.IDAttribute);
            }
            else if (FieldAttributes.Count() > 0)
            {
                Attributes.FieldAttribute fieldAttribute = FieldAttributes[0] as Attributes.FieldAttribute;

                if (!fieldAttribute.IsTableField)
                    return null;

                autoGenered = fieldAttribute.AutoGenered;

                if (autoGenered)
                    insertable = false;
                else
                    insertable = fieldAttribute.Insertable;

                updatable = fieldAttribute.Updatable;

                return GetClassProperty(fieldAttribute, property.Name, property.GetValue(obj, null), insertable, updatable, autoGenered);
            }
            else
            {
                return GetClassProperty(null, property.Name, property.GetValue(obj, null), insertable, updatable, autoGenered); ;
            }
        }

        private static Models.ClassProperty GetClassProperty(Attributes.IFieldAttribute attribute, string propertyName, object value, bool insertable, bool updatable, bool autoGenered)
        {
            string fieldName = propertyName;

            if (attribute != null)
            {
                fieldName = attribute.FieldName;

                if (string.IsNullOrEmpty(fieldName))
                    fieldName = propertyName;
            }

            return new Models.ClassProperty(propertyName, value, fieldName, insertable, updatable, autoGenered);
        }

        public static Models.ClassProperty GetIdClassProperty(object obj)
        {
            IList<System.Reflection.PropertyInfo> props = GetProperties(obj);
            foreach (System.Reflection.PropertyInfo prop in props)
            {
                object[] IDattributes = prop.GetCustomAttributes(typeof(Attributes.IDAttribute), false);
                if (IDattributes.Count() > 0)
                {
                    return GetIdClassProperty(obj, prop, IDattributes[0] as Attributes.IDAttribute);
                }
            }

            throw new Exceptions.IDFieldNotFoundException("Field ID not found, make sure that the ID attribute is in the target class");
        }

        public static string GetIdFieldName(Type type)
        {
            IList<System.Reflection.PropertyInfo> props = type.GetProperties();
            foreach (System.Reflection.PropertyInfo prop in props)
            {
                object[] IDattributes = prop.GetCustomAttributes(typeof(Attributes.IDAttribute), false);
                if (IDattributes.Count() > 0)
                {
                    Attributes.IDAttribute attribute = IDattributes[0] as Attributes.IDAttribute;
                    string fieldName = GetAttributeFieldName(attribute);
                    if (fieldName != null)
                        return fieldName;
                    else
                        return prop.Name;
                }
            }

            throw new Exceptions.IDFieldNotFoundException("Field ID not found, make sure that the ID attribute is in the target class");
        }
        private static Models.ClassProperty GetIdClassProperty(object obj, System.Reflection.PropertyInfo prop, Attributes.IDAttribute idAttribute)
        {
            bool autoGenered = idAttribute.AutoGenered;
            bool insertable = !idAttribute.AutoGenered;
            bool updatable = false;
            return GetClassProperty(idAttribute, prop.Name, prop.GetValue(obj, null), insertable, updatable, autoGenered);
        }
    }
}
