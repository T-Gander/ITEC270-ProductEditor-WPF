using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient; //Sql using statement
using System.Configuration;
using System.Diagnostics;
using System.Windows;

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

                SqlCommand cmd;

                cmd = new SqlCommand($"select ProductID, ProductName, (select CompanyName from Suppliers s where p.SupplierID = s.SupplierID) [SupplierName], UnitPrice from products p order by ProductName", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //Set up object with properties we want and add to list.
                    ProductRecords p = new ProductRecords();
                    p.Id = reader.GetInt32(0);
                    p.ProductName = reader.GetString(1);
                    p.SupplierID = reader.GetString(2);
                    p.Price = reader.GetDecimal(3);

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
                
                cmd = new SqlCommand($"select ProductID, ProductName, (select CompanyName from Suppliers s where p.SupplierID = s.SupplierID) [SupplierName], UnitPrice from products p where ProductName like @search", conn);
                cmd.Parameters.Add(new SqlParameter("@search", search));
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //Set up object with properties we want and add to list.
                    ProductRecords p = new ProductRecords();
                    p.Id = reader.GetInt32(0);
                    p.ProductName = reader.GetString(1);
                    p.SupplierID = reader.GetString(2);
                    p.Price = reader.GetDecimal(3);

                    products.Add(p);
                }
            }
            return products;
        }

        public List<string> GetSuppliers()
        {
            var suppliers = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd;

                cmd = new SqlCommand($"select CompanyName from suppliers", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    suppliers.Add(reader.GetString(0));
                }
            }
            return suppliers;
        }

        public void EditProduct(Product product, string previousName)
        {
            decimal price;
            bool priceFormatCorrect = decimal.TryParse(product.lblProductPrice.Content.ToString(), out price);

            if (priceFormatCorrect)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd;
                    SqlCommand getSupplierID;

                    string productName = product.lblProductName.Content.ToString();
                    string productSupplier = product.lblProductSupplier.Content.ToString();

                    getSupplierID = new SqlCommand("select SupplierID from suppliers where CompanyName = @ProductSupplier", conn);
                    getSupplierID.Parameters.AddWithValue("@ProductSupplier", productSupplier);

                    int newSupplierID = 0;

                    using (SqlDataReader reader = getSupplierID.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            newSupplierID = reader.GetInt32(0); // Directly retrieve the integer value
                        }
                    }

                    cmd = new SqlCommand(@$"
                    update products
                    set ProductName = @ProductName, SupplierID = @NewSupplierID, UnitPrice = @Price
                    where ProductName = @PreviousName"
                    , conn);

                    cmd.Parameters.AddWithValue("@ProductName", productName);
                    cmd.Parameters.AddWithValue("@NewSupplierID", newSupplierID);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@PreviousName", previousName);

                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                MessageBox.Show("Incorrect pricing format.");
            }
        }

        public void AddProduct(Product product)
        {
            decimal price;
            bool priceFormatCorrect = decimal.TryParse(product.lblProductPrice.Content.ToString(), out price);

            if (priceFormatCorrect)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd;
                    SqlCommand getSupplierID;

                    string productName = product.lblProductName.Content.ToString();
                    string productSupplier = product.lblProductSupplier.Content.ToString();
                    int supplierID = 0;

                    getSupplierID = new SqlCommand("select SupplierID from suppliers where CompanyName = @ProductSupplier", conn);
                    getSupplierID.Parameters.AddWithValue("@ProductSupplier", productSupplier);

                    cmd = new SqlCommand(@$"
                    insert into products (ProductName, SupplierID, UnitPrice)
                    values (@ProductName, @SupplierID, @Price)"
                    , conn);

                    using (SqlDataReader reader = getSupplierID.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            supplierID = reader.GetInt32(0); // Directly retrieve the integer value
                        }
                    }

                    if(supplierID != 0)
                    {
                        cmd.Parameters.AddWithValue("@SupplierID", supplierID);
                        cmd.Parameters.AddWithValue("@ProductName", productName);
                        cmd.Parameters.AddWithValue("@Price", price);

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Incorrect pricing format.");
            }
        }

        public void DeleteProduct(Product product)
        {
            decimal price;
            bool priceFormatCorrect = decimal.TryParse(product.lblProductPrice.Content.ToString(), out price);

            if (priceFormatCorrect)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd;
                    SqlCommand getProductID;

                    int productID = 0;
                    string productName = product.lblProductName.Content.ToString();

                    getProductID = new SqlCommand("select ProductID from products where ProductName = @ProductName", conn);
                    getProductID.Parameters.AddWithValue("@ProductName", productName);

                    cmd = new SqlCommand(@$"
                    delete from products
                    where ProductID = @ProductID
                    "
                    , conn);

                    using (SqlDataReader reader = getProductID.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            productID = reader.GetInt32(0); // Directly retrieve the integer value
                        }
                    }

                    if (productID != 0)
                    {
                        cmd.Parameters.AddWithValue("@ProductID", productID);

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Incorrect pricing format.");
            }
        }
    }
}
