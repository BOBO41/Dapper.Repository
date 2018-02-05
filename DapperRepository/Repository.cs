﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace DapperRepository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        #region Fields
        private readonly IDataContext _dataContext;
        #endregion

        #region Ctor
        public Repository()
        {
            _dataContext = new DataContext("DbConnection");
        }
        #endregion

        #region Sync Methods
        public void Insert(T item)
        {
            try
            {
                _dataContext.Insert(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void InsertBulk(IEnumerable<T> items)
        {

        }

        public void Update(T item)
        {
            try
            {
                _dataContext.Update(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void UpdateBulk(IEnumerable<T> items)
        {

        }

        public void Delete(T item)
        {
            try
            {
                _dataContext.Delete(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void DeleteBulk(IEnumerable<T> items)
        {

        }

        public T Find(int Id)
        {
            return _dataContext.Find<T>(Id);
        }

        public T Find(Expression<Func<T, bool>> expression)
        {
            return _dataContext.Find(expression);
        }

        public IList<T> FindAll(Expression<Func<T, bool>> expression)
        {
            return _dataContext.FindAll(expression).ToList();
        }
        #endregion
    }
}
