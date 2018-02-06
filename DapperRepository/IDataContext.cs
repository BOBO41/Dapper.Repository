using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace DapperRepository
{
    public interface IDataContext
    {
        void Insert<T>(T item, IDbTransaction transaction = null) where T : BaseEntity;
        void InsertBulk<T>(IEnumerable<T> items, IDbTransaction transaction = null) where T : BaseEntity;
        void Update<T>(T item, IDbTransaction transaction = null) where T : BaseEntity;
        void UpdateBulk<T>(IEnumerable<T> items, IDbTransaction transaction = null) where T : BaseEntity;
        void Delete<T>(T item, IDbTransaction transaction = null) where T : BaseEntity;
        void DeleteBulk<T>(IEnumerable<T> items, IDbTransaction transaction = null) where T : BaseEntity;
        T Find<T>(int Id) where T : BaseEntity;
        T Find<T>(Expression<Func<T, bool>> expression) where T : BaseEntity;
        IEnumerable<T> FindAll<T>(Expression<Func<T, bool>> expression) where T : BaseEntity;
        void Execute(string commandText, object parameters = null, IDbTransaction transaction = null);
        IDataReader ExecuteReader(string commandText, object parameters = null, IDbTransaction transaction = null);
        T ExecuteScalar<T>(string commandText, object parameters = null, IDbTransaction transaction = null) where T : BaseEntity;
        void ExecuteProcedure(string storedProcedureName, object parameters = null, IDbTransaction transaction = null);
        IDataReader ExecuteReaderProcedure(string storedProcedureName, object parameters = null, IDbTransaction transaction = null);
        T ExecuteScalarProcedure<T>(string storedProcedureName, object parameters = null, IDbTransaction transaction = null) where T : BaseEntity;

        IDbConnection Connection { get; }
        void OpenConnection();
        IDbTransaction BeginTransaction();
    }
}