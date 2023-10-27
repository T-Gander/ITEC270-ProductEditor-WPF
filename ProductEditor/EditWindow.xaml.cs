using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFDemo1;

namespace ProductEditor
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        static string PreviousProductName {  get; set; }

        public EditWindow(Product product)
        {
            InitializeComponent();
            DatabaseManager mngr = new DatabaseManager();
            PreviousProductName = product.lblProductName.Content.ToString();
            txtProductName.Text = product.lblProductName.Content.ToString();
            string price = product.lblProductPrice.Content.ToString();
            txtPrice.Text = price.Replace("$","");
            cmbSupplier.ItemsSource = mngr.GetSuppliers(); //replace with a list of suppliers.
            cmbSupplier.SelectedItem = product.lblProductSupplier.Content.ToString();
            cmbCategory.ItemsSource = mngr.GetCategories();
            cmbCategory.SelectedItem = product.lblProductCategory.Content.ToString();
            //cmbSupplier.Text = product.lblProductSupplier.Content.ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            decimal price;
            string priceString = txtPrice.Text;
            bool priceIsCorrectFormat = decimal.TryParse(priceString, out price);

            if (priceIsCorrectFormat)
            {
                string productName = txtProductName.Text;
                string supplierName = cmbSupplier.SelectedItem.ToString();
                string categoryName = cmbCategory.SelectedItem.ToString();

                Product editedProduct = new Product(productName, supplierName, price, categoryName);

                //Update table
                DatabaseManager mngr = new DatabaseManager();

                mngr.EditProduct(editedProduct, PreviousProductName);
                Close();
            }
            else
            {
                MessageBox.Show("Something went wrong, make sure there are no illegal characters.");
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
