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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            string logintype = (string)App.Current.Properties["logintype"];

            InitializeComponent();

            App.Current.Properties["goback"] = "";
            App.Current.Properties["autolinkid"] = -1;
            App.Current.Properties["mode"] = "other";
            App.Current.Properties["loancid"] = "";
            App.Current.Properties["loanaid"] = "";
            App.Current.Properties["accedit"] = null;
            App.Current.Properties["useredit"] = null;
            App.Current.Properties["eedit"] = null;
            App.Current.Properties["ledit"] = null;
            string n = (string)App.Current.Properties["Name"];

            if (logintype=="admin")
            {
                button4.IsEnabled = true;
                icon.Visibility = Visibility.Collapsed;
            }
            else
            {
                button4.IsEnabled = false;
                icon.Visibility = Visibility.Visible;
            }
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Clients window = new Clients();
            window.Show();
            this.Hide();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Loans window = new Loans();
            window.Show();
            this.Hide();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Stats window = new Stats();
            window.Show();
            this.Hide();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            ManagerOptions window = new ManagerOptions();
            window.Show();
            this.Hide();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            Accounts window = new Accounts();
            window.Show();
            this.Hide();
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            Shares window = new Shares();
            window.Show();
            this.Hide();
        }

        private void button0_Click(object sender, RoutedEventArgs e)
        {
            Login window = new Login();
            window.Show();
            this.Hide();
        }
    }
}
