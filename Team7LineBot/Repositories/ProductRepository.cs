using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Team7LineBot.Models;
using Dapper;

namespace Team7LineBot.Repositories
{
    public class ProductRepository
    {
        private static string connString;
        private SqlConnection conn;

        public ProductRepository()
        {
            if (string.IsNullOrEmpty(connString))
            {
                connString = ConfigurationManager.ConnectionStrings["WineDB"].ConnectionString;
            }

            conn = new SqlConnection(connString);

        }

        public List<Product> GetProducts()
        {
            List<Product> products;

            using (conn = new SqlConnection(connString))
            {
                string sql = @"select ProductID, ProductName, UnitPrice from Products";

                products = conn.Query<Product>(sql).ToList();
            }

                return products;
        }

        public List<Product> GetProducts(string search)
        {
            List<Product> products;

            using (conn = new SqlConnection(connString))
            {
                string sql = @"select top 10 * from Products
                                where ProductName LIKE @ProductName";

                products = conn.Query<Product>(sql, new { ProductName = "%" + search + "%" }).ToList();
            }

                return products;
        }

        public List<Product> GetHotProducts()
        {
            List<Product> products;

            using (conn = new SqlConnection(connString))
            {
                string sql = @"with t1 (ProductId, Total)
                            as
                            (
	                            select top 10 ProductID, SUM(Quantity) as Total 
	                            from [Order Details]
	                            group by ProductID
	                            order by Total desc
                            )
                            select p.ProductID, p.ProductName, p.Origin, p.Year, p.Capacity,
		                    p.UnitPrice, p.Stock, p.Grade, p.Variety, p.Area, p.Picture, p.Introduction, p.CategoryID
                            from t1 as t
                            INNER JOIN Products as p on p.ProductID = t.ProductId";

                products = conn.Query<Product>(sql).ToList();
            }

            return products;
        }

        public List<Product> GetNewProducts()
        {
            List<Product> products;

            using (conn)
            {
                string sql = @"select top 10 * from Products
                            where Year = (select top 1 Year from Products order by Year desc)";

                products = conn.Query<Product>(sql).ToList();
            }

            return products;
        }

        public List<Product> GetExpensiveProducts()
        {
            List<Product> products;

            using (conn)
            {
                string sql = @"select top 10 * from Products
                            where UnitPrice >= 10000
                            order by UnitPrice desc";

                products = conn.Query<Product>(sql).ToList();
            }

            return products;
        }

    }
}