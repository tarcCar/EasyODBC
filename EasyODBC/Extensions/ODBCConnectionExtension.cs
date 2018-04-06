using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyODBC.Extensions
{
    public static class ODBCConnectionExtension
    {
        public static IEnumerable<T> Query<T>(this OdbcConnection connection, string query, object param = null, OdbcTransaction transaction = null)
        {
            using (OdbcCommand command = Helpers.QueryHelper.CreateCommandQuery(query, param))
            {
                return QueryCommand<T>(command, connection, transaction);
            }
        }

        public static T QueryFirstOrDefault<T>(this OdbcConnection connection, string query, object param = null, OdbcTransaction transaction = null)
        {
            using (OdbcCommand command = Helpers.QueryHelper.CreateCommandQuery(query, param))
            {
                return QueryCommandSingleResult<T>(command, connection, transaction);
            }
        }

        public static int Insert<T>(this OdbcConnection connection, T obj, OdbcTransaction transaction = null)
        {
            OdbcCommand command = Helpers.QueryHelper.CreateCommandInsertQuery<T>(obj);
            return ExecuteCommand(command, connection, transaction);
        }

        public static int Update<T>(this OdbcConnection connection, T obj, OdbcTransaction transaction = null)
        {
            using (OdbcCommand command = Helpers.QueryHelper.CreateUpdateQuery<T>(obj))
            {
                return ExecuteCommand(command, connection, transaction);
            }
        }

        public static int Delete<T>(this OdbcConnection connection, T obj, OdbcTransaction transaction = null)
        {
            using (OdbcCommand command = Helpers.QueryHelper.CreateCommandDeleteQuery<T>(obj))
            {
                return ExecuteCommand(command, connection, transaction);
            }
        }

        public static T FindByID<T>(this OdbcConnection connection, object id, OdbcTransaction transaction = null)
        {
            using (OdbcCommand command = Helpers.QueryHelper.CreateCommandFindByIDQuery<T>(id))
            {
                return QueryCommandSingleResult<T>(command, connection, transaction);
            }
        }

        public static IEnumerable<T> FindAll<T>(this OdbcConnection connection, OdbcTransaction transaction = null)
        {
            using (OdbcCommand command = Helpers.QueryHelper.CreateCommandFindAll<T>())
            {
                return QueryCommand<T>(command, connection, transaction);
            }
        }

        public static int Execute<T>(this OdbcConnection connection, string query, object param = null, OdbcTransaction transaction = null)
        {
            using (OdbcCommand command = Helpers.QueryHelper.CreateCommandQuery(query, param))
            {
                return ExecuteCommand(command, connection, transaction);
            }
        }

        private static int ExecuteCommand(OdbcCommand command, OdbcConnection connection, OdbcTransaction transaction)
        {
            command.Connection = connection;
            command.Transaction = transaction;
            return command.ExecuteNonQuery();
        }

        private static T QueryCommandSingleResult<T>(OdbcCommand command, OdbcConnection connection, OdbcTransaction transaction)
        {
            command.Connection = connection;
            command.Transaction = transaction;
            using (OdbcDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
            {
                return reader.ToObject<T>();
            }
        }

        private static IEnumerable<T> QueryCommand<T>(OdbcCommand command, OdbcConnection connection, OdbcTransaction transaction)
        {
            command.Connection = connection;
            command.Transaction = transaction;
            using (OdbcDataReader reader = command.ExecuteReader())
            {
                return reader.ToEnumerable<T>();
            }
        }

    }
}
