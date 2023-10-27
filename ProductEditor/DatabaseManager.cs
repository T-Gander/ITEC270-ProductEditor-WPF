using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient; //Sql using statement
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

                cmd = new SqlCommand($"select ProductID, ProductName, (select CompanyName from Suppliers s where p.SupplierID = s.SupplierID) [SupplierName], (select CategoryName from Categories c where p.CategoryID = c.CategoryID) [CategoryName], UnitPrice from products p order by ProductName", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //Set up object with properties we want and add to list.
                    ProductRecords p = new ProductRecords();
                    p.Id = reader.GetInt32(0);
                    p.ProductName = reader.GetString(1);
                    p.SupplierID = reader.GetString(2);
                    p.CategoryName = reader.GetString(3);
                    p.Price = reader.GetDecimal(4);

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
                
                cmd = new SqlCommand($"select ProductID, ProductName, (select CompanyName from Suppliers s where p.SupplierID = s.SupplierID) [SupplierName], (select CategoryName from Categories c where p.CategoryID = c.CategoryID) [CategoryName], UnitPrice from products p where ProductName like @search", conn);
                cmd.Parameters.Add(new SqlParameter("@search", search));
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //Set up object with properties we want and add to list.
                    ProductRecords p = new ProductRecords();
                    p.Id = reader.GetInt32(0);
                    p.ProductName = reader.GetString(1);
                    p.SupplierID = reader.GetString(2);
                    p.CategoryName = reader.GetString(3);
                    p.Price = reader.GetDecimal(4);
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

        public List<string> GetCategories()
        {
            var suppliers = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd;

                cmd = new SqlCommand($"select CategoryName from Categories", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    suppliers.Add(reader.GetString(0));
                }
            }
            return suppliers;
        }

        private int GetSupplierIDSQL(Product product, SqlConnection conn)
        {
            SqlCommand getSupplierID;

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

            return newSupplierID;
        }

        private int GetCategoryIDSQL(Product product, SqlConnection conn)
        {
            SqlCommand getCategoryID;

            string productCategory = product.lblProductCategory.Content.ToString();

            getCategoryID = new SqlCommand("select CategoryID from categories where CategoryName = @CategoryName", conn);
            getCategoryID.Parameters.AddWithValue("@CategoryName", productCategory);

            int newCategoryID = 0;

            using (SqlDataReader reader = getCategoryID.ExecuteReader())
            {
                if (reader.Read())
                {
                    newCategoryID = reader.GetInt32(0); // Directly retrieve the integer value
                }
            }

            return newCategoryID;
        }

        private int GetProductIDSQL(Product product, SqlConnection conn)
        {
            SqlCommand getProductID;

            string productName = product.lblProductName.Content.ToString();

            getProductID = new SqlCommand("select ProductID from products where ProductName = @ProductName", conn);
            getProductID.Parameters.AddWithValue("@ProductName", productName);

            int newProductID = 0;

            using (SqlDataReader reader = getProductID.ExecuteReader())
            {
                if (reader.Read())
                {
                    newProductID = reader.GetInt32(0); // Directly retrieve the integer value
                }
            }

            return newProductID;
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

                    string productName = product.lblProductName.Content.ToString();

                    cmd = new SqlCommand(@$"
                    update products
                    set ProductName = @ProductName, SupplierID = @NewSupplierID, CategoryID = @NewCategoryID, UnitPrice = @Price
                    where ProductName = @PreviousName"
                    , conn);

                    int categoryID = GetCategoryIDSQL(product, conn);
                    int supplierID = GetSupplierIDSQL(product, conn);

                    if(categoryID != 0 && supplierID != 0)
                    {
                        cmd.Parameters.AddWithValue("@ProductName", productName);
                        cmd.Parameters.AddWithValue("@NewSupplierID", supplierID);
                        cmd.Parameters.AddWithValue("@NewCategoryID", categoryID);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@PreviousName", previousName);

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

                    cmd = new SqlCommand(@$"
                    insert into products (ProductName, SupplierID, CategoryID, UnitPrice)
                    values (@ProductName, @SupplierID, @CategoryID, @Price)"
                    , conn);

                    int categoryID = GetCategoryIDSQL(product, conn);
                    int supplierID = GetSupplierIDSQL(product, conn);

                    if (supplierID != 0 && categoryID != 0)
                    {
                        cmd.Parameters.AddWithValue("@CategoryID", categoryID);
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
            string priceToCheck = product.lblProductPrice.Content.ToString().Replace("$", "");
            bool priceFormatCorrect = decimal.TryParse(priceToCheck, out price);

            if (priceFormatCorrect)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd;

                    int productID = GetProductIDSQL(product, conn);

                    cmd = new SqlCommand(@$"
                    delete from products
                    where ProductID = @ProductID
                    "
                    , conn);

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
