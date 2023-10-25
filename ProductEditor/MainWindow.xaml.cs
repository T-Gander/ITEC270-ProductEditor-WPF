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

        public MainWindow()
        {
            InitializeComponent();
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

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private Task ShowPopup<TPopup>(TPopup popup)    //Stack overflow
            where TPopup : Window
        {
            var task = new TaskCompletionSource<object>();
            popup.Owner = Application.Current.MainWindow;
            popup.Closed += (s, a) => task.SetResult(null);
            popup.Show();
            popup.Focus();
            return task.Task;
        }
    }
}
