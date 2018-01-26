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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Stats2.xaml
    /// </summary>
    public partial class Stats2 : Window
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

        public Stats2()
        {
            InitializeComponent();
            textBox4.IsEnabled = false;
            textBox5.IsEnabled = false;
            textBox6.IsEnabled = false;
            double aux = accspc();
            try
            {
                textBox4.Text = String.Format("{0:0.00}", loanspc());
                textBox5.Text = String.Format("{0:0.00}", aux);
                if (aux==0) { textBox6.Text = "0"; } else { textBox6.Text = String.Format("{0:0.00}", 1 / aux); }
                
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
        private double loanspc()
        {
            if (!refresh())
                return 0;

            SqlCommand cmd = new SqlCommand("select count( DISTINCT loans.id) as A, count( DISTINCT clients.id) as B from loans, clients where appr='yes'");
            cmd.Connection = cn;

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return 0; //no data
            }

            return (double)(int)dataSet.Tables[0].Rows[0]["A"]/(double)(int)dataSet.Tables[0].Rows[0]["B"];
        }

        private double accspc()
        {
            if (!refresh())
                return 0;

            SqlCommand cmd = new SqlCommand("select count( DISTINCT accounts.id) as A, count( DISTINCT clients.id) as B from accounts, clients");
            cmd.Connection = cn;

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return 0; //no data
            }

            return (double)(int)dataSet.Tables[0].Rows[0]["A"] / (double)(int)dataSet.Tables[0].Rows[0]["B"];
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Stats window = new Stats();
            window.Show();
            this.Hide();
        }

        private void button_adv(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("This button only exists to demonstrate the algorithms included in this program, "+
                "and would not be included in a \"real\" version of it. The values of some account balances will be changed, and active loans will have their "+
                "duration reduced by one month (if the account can pay for it), where applicable. Continue?",
                "Time Travel Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (!refresh())
                    return;
                try
                {
                    //verify
                    SqlCommand cmd = new SqlCommand("select count(id) from accounts");
                    cmd.Connection = cn;
                    if ((int)cmd.ExecuteScalar()==0)
                    {
                        MessageBox.Show("There are no accounts."); return;
                    }

                    //begin
                    cmd = new SqlCommand("EXEC TimeTravel");
                    cmd.Connection = cn;
                    cmd.ExecuteNonQuery();

                    //refresh
                    double aux = accspc();

                    textBox4.Text = String.Format("{0:0.00}", loanspc());
                    textBox5.Text = String.Format("{0:0.00}", aux);
                    if (aux == 0) { textBox6.Text = "0"; } else { textBox6.Text = String.Format("{0:0.00}", 1 / aux); }


                } catch (SqlException) { MessageBox.Show("Error updating database. Possible overflow."); }
            }
        }
    }
}
