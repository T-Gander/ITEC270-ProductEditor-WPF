using ProductEditor;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFDemo1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string SearchTerm = string.Empty;
        public static Product EditableProduct;

        public MainWindow()
        {
            InitializeComponent();
            DatabaseManager mngr = new DatabaseManager();

            var listProducts = mngr.GetProducts();

            if (listProducts.Count > 0)
            {
                foreach (var p in listProducts)
                {
                    Product pc = new Product(p);
                    lstResults.Items.Add(pc);
                }
            }
            else
            {
                lstResults.Items.Add("No Products Found");
            }

        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            lstResults.Items.Clear();
            SearchTerm = String.Empty;
            DatabaseManager mngr = new DatabaseManager();

            await ShowPopup(new SearchWindow());

            var listProducts = mngr.SearchProducts(SearchTerm);

            if(listProducts.Count > 0)
            {
                foreach (var p in listProducts)
                {
                    Product pc = new Product(p);
                    lstResults.Items.Add(pc);
                }
            }
            else
            {
               lstResults.Items.Add("No Products Found");
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if(lstResults.SelectedIndex != -1)
            {
                DatabaseManager mngr = new DatabaseManager();

                EditableProduct = lstResults.SelectedItem as Product;

                await ShowPopup(new EditWindow(EditableProduct));

                lstResults.Items.Clear();

                var listProducts = mngr.GetProducts();

                foreach (var p in listProducts)
                {
                    Product pc = new Product(p);
                    lstResults.Items.Add(pc);
                }
            }
            else
            {
                MessageBox.Show("Select a product to edit!");
            }
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            DatabaseManager mngr = new DatabaseManager();

            Product NewProduct = new Product();

            await ShowPopup(new AddWindow(NewProduct));

            lstResults.Items.Clear();

            var listProducts = mngr.GetProducts();

            foreach (var p in listProducts)
            {
                Product pc = new Product(p);
                lstResults.Items.Add(pc);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstResults.SelectedIndex != -1)
            {
                DatabaseManager mngr = new DatabaseManager();

                EditableProduct = lstResults.SelectedItem as Product;

                mngr.DeleteProduct(EditableProduct);

                lstResults.Items.Clear();

                var listProducts = mngr.GetProducts();

                foreach (var p in listProducts)
                {
                    Product pc = new Product(p);
                    lstResults.Items.Add(pc);
                }
            }
        }

        private Task ShowPopup<TPopup>(TPopup popup)    //Stack overflow
            where TPopup : Window
        {
            var task = new TaskCompletionSource<object>();
            popup.Owner = Application.Current.MainWindow;
            popup.Closed += (s, a) => task.SetResult(null);
            popup.ShowDialog();
            popup.Focus();
            return task.Task;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            lstResults.Items.Clear();
            DatabaseManager mngr = new DatabaseManager();

            var listProducts = mngr.GetProducts();

            if (listProducts.Count > 0)
            {
                foreach (var p in listProducts)
                {
                    Product pc = new Product(p);
                    lstResults.Items.Add(pc);
                }
            }
            else
            {
                lstResults.Items.Add("No Products Found");
            }
        }
    }
}
