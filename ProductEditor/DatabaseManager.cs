using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient; //Sql using statement
using System.Configuration;

namespace WPFDemo1
{
    public class DatabaseManager
    {
        string connectionString = ConfigurationManager.ConnectionStrings["localConnection"].ConnectionString;

        public List<ProductRecords> GetProducts()
        {
            var products = new List<ProductRecords>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select * from products",conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //Set up object with properties we want and add to list.
                    ProductRecords p = new ProductRecords();
                    p.Id = reader.GetInt32(0);
                    p.ProductName = reader.GetString(1);
                    p.SupplierID = reader.GetInt32(2);
                    p.Price = reader.GetDecimal(5);

                    products.Add(p);
                }
            }
            return products;
        }

        public List<ProductRecords> SearchProducts(string search)
        {
            var products = new List<ProductRecords>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd;
                
                cmd = new SqlCommand($"select * from products where ProductName = @search", conn);
                cmd.Parameters.Add(new SqlParameter("@search", search));
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //Set up object with properties we want and add to list.
                    ProductRecords p = new ProductRecords();
                    p.Id = reader.GetInt32(0);
                    p.ProductName = reader.GetString(1);
                    p.SupplierID = reader.GetInt32(2);
                    p.Price = reader.GetDecimal(5);

                    products.Add(p);
                }
            }
            return products;
        }
    }
}
