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
using System.Data.SqlClient;
using System.Data;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Stats.xaml
    /// </summary>
    public partial class Stats : Window
    {
        private SqlConnection getcn()
        {
            return new SqlConnection((string)App.Current.Properties["db"]);
        }

        SqlConnection cn;

        private bool refresh()
        {
            if (cn == null)
                cn = getcn();

            if (cn.State != ConnectionState.Open)
                cn.Open();

            return cn.State == ConnectionState.Open;
        }

        public Stats()
        {
            InitializeComponent();

            textBox4.IsEnabled = false;
            textBox5.IsEnabled = false;
            textBox6.IsEnabled = false;
            textBox7.IsEnabled = false;
            textBox8.IsEnabled = false;
            try { 
            textBox4.Text = String.Format("{0:0.00}€", totalmoneyintheaccounts());
            textBox5.Text = totalaccounts().ToString();
            textBox6.Text = String.Format("{0:0.00}€", moneyloaned());
            textBox7.Text = totalloans();
            textBox8.Text = sharesSold();
            }
            catch (OverflowException) { MessageBox.Show("Overflow while attempting to calculate stats."); }
            catch (SqlException) { MessageBox.Show("Error accessing database."); }

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Hide();
            
        }
        private double  totalmoneyintheaccounts()
        {
            if (!refresh())
                return 0;

            SqlCommand cmd = new SqlCommand("SELECT sum(balance) FROM ACCOUNTS");
            cmd.Connection = cn;
            return (double)(decimal)cmd.ExecuteScalar();
        }
        private double totalaccounts()
        {
            if (!refresh())
                return 0;

            SqlCommand cmd = new SqlCommand("select count(id) from accounts");
            cmd.Connection = cn;
            return (double)(int)cmd.ExecuteScalar();
        }
        private double moneyloaned()
        {
            if (!refresh())
                return 0;

            SqlCommand cmd = new SqlCommand("select sum(reqval) from loans where appr='yes'");
            cmd.Connection = cn;
            return (double)(decimal)cmd.ExecuteScalar();
        }
        private string totalloans()
        {
            if (!refresh())
                return "0 approved / 0 pending";

            SqlCommand cmd = new SqlCommand("select count(id) from loans where appr='yes'");
            cmd.Connection = cn;
            int appr = (int)cmd.ExecuteScalar();
            cmd = new SqlCommand("select count(id) from loans where appr='no'");
            cmd.Connection = cn;
            int notAppr = (int)cmd.ExecuteScalar();

            return appr+" approved / "+notAppr+" pending";

        }
        private string sharesSold()
        {
            if (!refresh())
                return "0% (0 sold)";

            SqlCommand cmd = new SqlCommand("select sum(amt) from shares");
            cmd.Connection = cn;
            double sold = (double)(int)cmd.ExecuteScalar();
            double maxshares = (int)App.Current.Properties["maxshares"];

            double p = sold / maxshares;

            return String.Format("{0:0.00}% ({1} sold)", p, sold);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Stats2 window = new Stats2();
            window.Show();
            this.Hide();
        }
    }
}
