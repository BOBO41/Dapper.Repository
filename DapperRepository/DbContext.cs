using Dapper;
using DapperRepository.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq.Expressions;

namespace DapperRepository
{
    public class DataContext : IDataContext, IDisposable
    {
        private readonly IProvider _provider;
        private readonly IDbConnection _connection;

        public DataContext(string connectionName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionName];

            _provider = ProviderHelper.GetProvider(connectionString.ProviderName);
            _connection = _provider.CreateConnection(connectionString.ConnectionString);
        }

        #region Sync Methods
        public virtual void Insert<T>(T item, IDbTransaction transaction = null) where T : BaseEntity
        {
            string sqlCommand = _provider.InsertQuery(typeof(T).Name, item);
            item.Id = _connection.ExecuteScalar<int>(sqlCommand, item, transaction);
        }

        public virtual void Update<T>(T item, IDbTransaction transaction = null) where T : BaseEntity
        {
            string sqlCommand = _provider.UpdateQuery(typeof(T).Name, item);
            _connection.Execute(sqlCommand, item, transaction);
        }

        public virtual void Delete<T>(T item, IDbTransaction transaction = null) where T : BaseEntity
        {
            string sqlCommand = _provider.DeleteQuery(typeof(T).Name);
            _connection.Execute(sqlCommand, new { item.Id }, transaction);
        }

        public virtual T Find<T>(int Id) where T : BaseEntity
        {
            string sqlCommand = _provider.SelectFirstQuery<T>(t => t.Id == Id, typeof(T).Name);

            return _connection.QueryFirst<T>(sqlCommand, new { Id });
        }

        public virtual T Find<T>(Expression<Func<T, bool>> expression) where T : BaseEntity
        {
            string sqlCommand = _provider.SelectFirstQuery<T>(expression, typeof(T).Name);
            var parameters = new ExpressionHelper().GetWhereParemeters(expression);

            return _connection.QueryFirst<T>(sqlCommand, parameters);
        }

        public virtual IEnumerable<T> FindAll<T>(Expression<Func<T, bool>> expression) where T : BaseEntity
        {
            IEnumerable<T> items = new List<T>();
            string sqlCommand = _provider.SelectQuery<T>(expression, typeof(T).Name);
            var parameters = new ExpressionHelper().GetWhereParemeters(expression);

            return _connection.Query<T>(sqlCommand, parameters);
        }

        public virtual void Execute(string sqlCommand, object parameters = null, IDbTransaction transaction = null)
        {
            _connection.Execute(sqlCommand, parameters, transaction);
        }

        public virtual IDataReader ExecuteReader(string sqlCommand, object parameters = null, IDbTransaction transaction = null)
        {
            return _connection.ExecuteReader(sqlCommand, parameters, transaction);
        }

        public virtual T ExecuteScalar<T>(string sqlCommand, object parameters = null, IDbTransaction transaction = null) where T : BaseEntity
        {
            return _connection.ExecuteScalar<T>(sqlCommand, parameters, transaction);
        }

        public virtual void ExecuteProcedure(string storedProcedureName, object parameters = null, IDbTransaction transaction = null)
        {
            _connection.Execute(sql: storedProcedureName,
                param: parameters,
                transaction: transaction,
                commandType: CommandType.StoredProcedure);
        }

        public virtual IDataReader ExecuteReaderProcedure(string storedProcedureName, object parameters = null, IDbTransaction transaction = null)
        {
            return _connection.ExecuteReader(sql: storedProcedureName,
                param: parameters,
                transaction: transaction,
                commandType: CommandType.StoredProcedure);
        }

        public virtual T ExecuteScalarProcedure<T>(string storedProcedureName, object parameters = null, IDbTransaction transaction = null) where T : BaseEntity
        {
            return _connection.ExecuteScalar<T>(sql: storedProcedureName,
                param: parameters,
                transaction: transaction,
                commandType: CommandType.StoredProcedure);
        }
        #endregion

        public virtual IDbTransaction BeginTransaction()
        {
            OpenConnection();

            return _connection.BeginTransaction();
        }

        public virtual void OpenConnection()
        {
            if (_connection != null &&
                _connection.State != ConnectionState.Open &&
                _connection.State != ConnectionState.Connecting)
                _connection.Open();
        }

        public virtual IDbConnection Connection
        {
            get
            {
                OpenConnection();

                return _connection;
            }
        }

        public virtual void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
        }
    }
}