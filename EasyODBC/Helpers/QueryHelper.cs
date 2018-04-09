using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyODBC.Helpers
{
    public static class QueryHelper
    {
        static readonly System.Text.RegularExpressions.Regex NamedParameterPattern = new System.Text.RegularExpressions.Regex(@"\?(\w+)");

        public static System.Data.Odbc.OdbcCommand CreateCommandQuery(string sql, object param = null)
        {
            if (param == null)
                return new System.Data.Odbc.OdbcCommand(sql);

            IList<Models.ClassProperty> properties = ReflectionHelper.GetClassPropertiesWithFieldNamesAndValues(param);
            return CreateCommandWithNamedParameters(sql, properties);
        }

        private static System.Data.Odbc.OdbcCommand CreateCommandWithNamedParameters(string sql, object value)
        {
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
            int parameterIndex = 0;
            cmd.CommandText = NamedParameterPattern.Replace(sql, (m) =>
            {
                string key = m.Groups[1].Value;
                return ReplaceNamedQuery(cmd, ref parameterIndex, key, value);
            });
            return cmd;
        }

        private static System.Data.Odbc.OdbcCommand CreateCommandWithNamedParameters(string sql,Models.ClassProperty property)
        {
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
            int parameterIndex = 0;
            cmd.CommandText = NamedParameterPattern.Replace(sql, (m) =>
            {
                string key = m.Groups[1].Value;

                if (property.Field != key)
                    throw new ArgumentException("Named query parameter and the property field name are different");

                object value = property.Value;
                return ReplaceNamedQuery(cmd, ref parameterIndex, key, value);
            });
            return cmd;
        }
       
        private static System.Data.Odbc.OdbcCommand CreateCommandWithNamedParameters(string sql, IEnumerable<Models.ClassProperty> properties)
        {

            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
            int parameterIndex = 0;
            cmd.CommandText = NamedParameterPattern.Replace(sql, (m) =>
            {
                string key = m.Groups[1].Value;
                object value = properties.FirstOrDefault(p => p.Field == key).Value;
                return ReplaceNamedQuery(cmd, ref parameterIndex, key, value);
            });
            return cmd;
        }

        private static string ReplaceNamedQuery(System.Data.Odbc.OdbcCommand cmd, ref int parameterIndex, string key, object value)
        {
            string parameterName = string.Format("{0}_{1}", key, parameterIndex++);

            if(value == null)
            {
                cmd.Parameters.AddWithValue(parameterName,DBNull.Value);
                return "?";
            }

            if ((value as string) != null || (value as IEnumerable) == null)
            {
                if (value is decimal || value is Decimal
                || value is Double || value is double
                || value is float
                )
                {
                    cmd.Parameters.AddWithValue(parameterName, value.ToString().Replace(",","."));
                }
                else
                {
                    cmd.Parameters.AddWithValue(parameterName, value ?? DBNull.Value);
                }

                return "?";
            }
            else
            {
                IEnumerable<object> enumerable = ((IEnumerable)value).Cast<object>();
                int i = 0;
                foreach (var el in enumerable)
                {
                    string elementName = string.Format("{0}_{1}", parameterName, i++);
                    cmd.Parameters.AddWithValue(elementName, el ?? DBNull.Value);
                }
                return string.Join(",", enumerable.Select(_ => "?"));
            }
        }

        private static string NamedParametersForInsert(IEnumerable<Models.ClassProperty> properties)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var property in properties)
            {
                if (sb.Length == 0)
                    sb.Append(string.Concat("(?",property.Field));
                else
                    sb.Append(string.Concat(",?", property.Field));
            }
            sb.Append(")");
            return sb.ToString();
        }

        public static System.Data.Odbc.OdbcCommand CreateCommandInsertQuery<T>(T obj)
        {
            IEnumerable<Models.ClassProperty> properties =
                                        ReflectionHelper.GetClassPropertiesWithFieldNamesAndValues(obj)
                                        .Where(p => p.Insertable).ToList();

            string tableName = ReflectionHelper.GetTableName(obj);
            string fields = ConcatInsertFields(properties);
            string namedParameters = NamedParametersForInsert(properties);
            string sql = string.Format("INSERT INTO {0} ({1}) VALUES {2}", tableName, fields, namedParameters);
            System.Data.Odbc.OdbcCommand command = CreateCommandWithNamedParameters(sql, properties);
            return command;
        }

        private static string ConcatInsertFields(IEnumerable<Models.ClassProperty> properties)
        {
            IEnumerable<string> fields = properties.Select(s => s.Field);
            return string.Join(",", fields);
        }

        
        public static System.Data.Odbc.OdbcCommand CreateCommandDeleteQuery<T>(T obj)
        {
            Models.ClassProperty idClassProperty = ReflectionHelper.GetIdClassProperty(obj);

            if (idClassProperty.Value == null || string.IsNullOrWhiteSpace(idClassProperty.Value.ToString()))
                throw new ArgumentException("In the delete method the ID field can not be null or empty");

            string fieldId = idClassProperty.Field;
            string tableName = ReflectionHelper.GetTableName(obj);

            string sql =  string.Format("DELETE FROM {0} WHERE {1} = ?{1}", tableName, fieldId);
            return CreateCommandWithNamedParameters(sql, idClassProperty);
        }

        public static System.Data.Odbc.OdbcCommand CreateCommandFindAll<T>()
        {
            Type type = typeof(T);
            IEnumerable<string> propertiesNames =
                                        ReflectionHelper.GetTypePropertiesFieldsNames(type);

            string tableName = ReflectionHelper.GetTableName(type);
            string sql = string.Format("SELECT {0} FROM {1}", string.Join(",", propertiesNames), tableName);
            return new System.Data.Odbc.OdbcCommand(sql);
        }

        public static System.Data.Odbc.OdbcCommand CreateCommandFindByIDQuery<T>(object id)
        {
            if (id == null || string.IsNullOrWhiteSpace(id.ToString()))
                throw new ArgumentException("In the find by id method the ID field can not be null or empty");

            Type type = typeof(T);
            IEnumerable<string> propertiesNames =
                                        ReflectionHelper.GetTypePropertiesFieldsNames(type);

            string tableName = ReflectionHelper.GetTableName(type);
            string nameIdField = ReflectionHelper.GetIdFieldName(type);
            string sql = string.Format("SELECT {0} FROM {1} WHERE {2} = ?{2}", string.Join(",", propertiesNames), tableName, nameIdField);
            return CreateCommandWithNamedParameters(sql, id);
        }

        public static System.Data.Odbc.OdbcCommand CreateUpdateQuery<T>(T obj)
        {
            Models.ClassProperty idClassProperty = ReflectionHelper.GetIdClassProperty(obj);

            if (idClassProperty.Value == null || string.IsNullOrWhiteSpace(idClassProperty.Value.ToString()))
                throw new ArgumentException("In the update method the ID field can not be null or empty");

            IList<Models.ClassProperty> properties =
                                       ReflectionHelper.GetClassPropertiesWithFieldNamesAndValues(obj)
                                       .Where(p => p.Updatable).ToList();
            string fieldsWithNewValues = ConcatUpdateFields(properties);

            properties.Add(idClassProperty);
            string fieldId = idClassProperty.Field;
            string tableName = ReflectionHelper.GetTableName(obj);
            string sql = string.Format("UPDATE {0} SET {1} WHERE {2} = ?{2}", tableName, fieldsWithNewValues, fieldId);
            return CreateCommandWithNamedParameters(sql, properties);
        }

        private static string ConcatUpdateFields(IEnumerable<Models.ClassProperty> properties)
        {
            IEnumerable<string> values =
                properties.Select(s => string.Concat(s.Field, "=?", s.Field));
            return string.Join(",", values);
        }
    }
}
