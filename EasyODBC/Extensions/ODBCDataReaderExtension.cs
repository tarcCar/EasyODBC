using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyODBC.Extensions
{
    public static class ODBCDataReaderExtension
    {
        public static T ToObject<T>(this System.Data.Odbc.OdbcDataReader reader)
        {
            IList<System.Reflection.PropertyInfo> properties = typeof(T).GetProperties();
            if (reader.Read())
            {
                if (Type.GetTypeCode(typeof(T)) != TypeCode.Object)
                {
                    return BuildItem<T>(reader);
                }
                else
                {
                    return BuildItem<T>(reader, properties);
                }
            }
            else
            {
                return default(T);
            }
        }

        public static IEnumerable<T> ToEnumerable<T>(this System.Data.Odbc.OdbcDataReader reader)
        {
            IList<T> result = new List<T>();
            if (Type.GetTypeCode(typeof(T)) != TypeCode.Object)
            {
                while (reader.Read())
                {
                    var item = BuildItem<T>(reader);
                    result.Add(item);
                }
            }
            else
            {
                IEnumerable<System.Reflection.PropertyInfo> properties = typeof(T).GetProperties();
                while (reader.Read())
                {
                    var item = BuildItem<T>(reader, properties);
                    result.Add(item);
                }
            }
            return result;
        }

        private static T BuildItem<T>(System.Data.Odbc.OdbcDataReader reader)
        {
            var fieldCount = reader.FieldCount;

            if (fieldCount > 1)
                throw new Exception("Number of fields in the result of the search can not exceed 1");

            var columnName = reader.GetName(0);

            if (reader[columnName] == DBNull.Value)
                return default(T);
            else
            {
                var data = reader[columnName];
                if (data is T)
                {
                    return (T)data;
                }
                try
                {
                    return (T)Convert.ChangeType(data, typeof(T));
                }
                catch (InvalidCastException)
                {
                    throw;
                }
            }
        }
        private static T BuildItem<T>(System.Data.Odbc.OdbcDataReader reader, IEnumerable<System.Reflection.PropertyInfo> properties)
        {
            Type type = typeof(T);
            var item = (T)Activator.CreateInstance(type);

            var fieldCount = reader.FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var property = properties.FirstOrDefault((prop) =>
                {
                    if (string.Equals(columnName, prop.Name, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                    else
                        return false;
                });
                if (property != null)
                {
                    if (reader[columnName] == DBNull.Value)
                        property.SetValue(item, null, null);
                    else
                        property.SetValue(item, reader[columnName], null);
                }
            }
            return item;
        }
    }
}
