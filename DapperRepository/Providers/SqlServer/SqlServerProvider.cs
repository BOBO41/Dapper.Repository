using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DapperRepository.Providers
{
    public class SqlServerProvider : IProvider
    {
        #region Constant
        private const string INSERT_QUERY = "INSERT INTO [{0}]({1}) OUTPUT INSERTED.Id\r\nVALUES(@{2})";
        private const string UPDATE_QUERY = "UPDATE\r\n [{0}] SET {1} WHERE [{0}].[Id] = @Id";
        private const string DELETE_QUERY = "DELETE FROM [{0}] WHERE [{0}].[Id] = @Id";
        private const string SELECT_QUERY = "SELECT\r\n {1} FROM [{0}]";
        private const string SELECT_FIRST_QUERY = "SELECT TOP(1)\r\n {1} FROM [{0}]";
        #endregion

        #region Fields
        private IDbConnection _connection;
        #endregion

        #region Utilities
        protected virtual IEnumerable<string> GetColumns(Type entityType)
        {
            PropertyInfo[] props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return props.Select(p => p.Name);
        }

        protected virtual IEnumerable<string> GetColumnsWithoutIdentity(Type entityType)
        {
            PropertyInfo[] props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return props.Where(p => p.Name != "Id").Select(p => p.Name);
        }
        #endregion

        #region Methods
        public virtual IDbConnection CreateConnection(string connectionString)
        {
            _connection = new SqlConnection(connectionString);

            return _connection;
        }
        
        public virtual string InsertQuery(string tableName, object entity)
        {
            IEnumerable<string> columns = GetColumnsWithoutIdentity(entity.GetType());

            return string.Format(INSERT_QUERY,
                                 tableName,
                                 string.Join(",\r\n", columns.Select(p => string.Format("[{0}].[{1}]", tableName, p))),
                                 string.Join(",\r\n@", columns));
        }

        public virtual string UpdateQuery(string tableName,  object entity)
        {
            IEnumerable<string> parameters = GetColumnsWithoutIdentity(entity.GetType()).Select(p => string.Format("[{0}].[{1}] = @{1}", tableName, p));

            return string.Format(UPDATE_QUERY,
                                 tableName,
                                 string.Join(",\r\n", parameters));
        }

        public virtual string DeleteQuery(string tableName)
        {
            return string.Format(DELETE_QUERY,
                                 tableName);
        }

        public virtual string SelectQuery<T>(Expression<Func<T, bool>> expression, string tableName) where T : BaseEntity
        {
            IEnumerable<string> columns = GetColumns(typeof(T)).Select(p => string.Format("[{0}].[{1}]", tableName, p));

            string query = string.Format(SELECT_QUERY,
                tableName,
                string.Join(",\r\n", columns));

            if (expression != null)
            {
                var translater = new WhereBuilder();
                string whereClasure = translater.Translate(expression);

                if (!string.IsNullOrEmpty(whereClasure))
                    query = string.Format("{0} WHERE {1}", query, whereClasure);
            }

            return query;
        }

        public virtual string SelectFirstQuery<T>(Expression<Func<T, bool>> expression, string tableName) where T : BaseEntity
        {
            IEnumerable<string> columns = GetColumns(typeof(T)).Select(p => string.Format("[{0}].[{1}]", tableName, p));

            string query = string.Format(SELECT_FIRST_QUERY,
                tableName,
                string.Join(",\r\n", columns));

            if (expression != null)
            {
                var translater = new WhereBuilder();
                string whereClasure = translater.Translate(expression);

                if (!string.IsNullOrEmpty(whereClasure))
                    query = string.Format("{0} WHERE {1}", query, whereClasure);
            }

            return query;
        }
        #endregion
    }
}