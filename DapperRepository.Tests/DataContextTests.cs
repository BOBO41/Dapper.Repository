using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DapperRepository.Tests
{
    [TestClass]
    public class DataContextTests
    {
        [TestMethod]
        public void InstanceCreation()
        {
            DataContext context = new DataContext("DbConnection");

            Assert.AreNotEqual(context, null);

            context.Dispose();
        }

        [TestMethod]
        public void GetConnection()
        {
            DataContext context = new DataContext("DbConnection");
            var connection = context.Connection;

            Assert.AreEqual(connection.State, System.Data.ConnectionState.Open);

            context.Dispose();
        }

        [TestMethod]
        public void BeginTransaction()
        {
            DataContext context = new DataContext("DbConnection");
            var transaction = context.BeginTransaction();

            Assert.AreEqual(transaction.Connection.State, System.Data.ConnectionState.Open);

            context.Dispose();
        }

        [TestMethod]
        public void Insert()
        {
            DataContext context = new DataContext("DbConnection");

            var product = new Product();
            context.Insert(product);

            Assert.AreNotEqual(product.Id, 0);

            context.Dispose();
        }

        public void InsertBulk()
        {
            DataContext context = new DataContext("DbConnection");

            var products = new List<Product> { new Product(), new Product(), new Product() };
            int rowCount = context.InsertBulk(products);

            Assert.AreNotEqual(rowCount, 0);

            context.Dispose();
        }

        [TestMethod]
        public void Update()
        {
            DataContext context = new DataContext("DbConnection");

            var product = new Product();
            context.Update(product);

            Assert.AreNotEqual(product.Id, 0);

            context.Dispose();
        }

        public void UpdateBulk()
        {
            DataContext context = new DataContext("DbConnection");

            var products = new List<Product> { new Product(), new Product(), new Product() };
            int rowCount = context.UpdateBulk(products);

            Assert.AreNotEqual(rowCount, 0);

            context.Dispose();
        }

        [TestMethod]
        public void Delete()
        {
            DataContext context = new DataContext("DbConnection");

            var product = new Product();
            context.Delete(product);

            Assert.AreNotEqual(product.Id, 0);

            context.Dispose();
        }

        public void DeleteBulk()
        {
            DataContext context = new DataContext("DbConnection");

            var products = new List<Product> { new Product(), new Product(), new Product() };
            int rowCount = context.DeleteBulk(products);

            Assert.AreNotEqual(rowCount, 0);

            context.Dispose();
        }
    }
}