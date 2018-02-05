using System;
using System.Data;
using System.Linq.Expressions;

namespace DapperRepository.Providers
{
    public interface IProvider
    {
        IDbConnection CreateConnection(string connectionString);
        string InsertQuery(string tableName, object entity);
        string UpdateQuery(string tableName, object entity);
        string DeleteQuery(string tableName);
        string SelectQuery<T>(Expression<Func<T, bool>> expression, string tableName) where T : BaseEntity;
        string SelectFirstQuery<T>(Expression<Func<T, bool>> expression, string tableName) where T : BaseEntity;
    }
}