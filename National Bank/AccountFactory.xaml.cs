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
    /// Interaction logic for AccountFactory.xaml
    /// </summary>
    public partial class AccountFactory : Window
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

        private int atype=0;
        int autolinkid = (int)App.Current.Properties["autolinkid"];
        string goback = (string)App.Current.Properties["goback"];

        public AccountFactory()
        {
            InitializeComponent();

            comboBox.Items.Add("Ordem");
            comboBox.Items.Add("Prazo 6M");
            comboBox.Items.Add("Prazo 1A");
            comboBox.Items.Add("Prazo 3A");

            comboBox.SelectedIndex = 0;

        }

        private void buttonc_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Clear all fields?", "Clear Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                textBoxc.Text = "";
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (goback == "client")
            {
                ClientDetails window = new ClientDetails();
                window.Show();
                this.Hide();
            }
            else
            {
                Accounts window = new Accounts();
                window.Show();
                this.Hide();
            }
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            double aux = -1;
            try
            {
                if (double.TryParse(textBoxc.Text, out aux)) { 

                    if (comboBox.Text == "Ordem")
                    {
                        atype = 0;
                    }

                    else if (comboBox.Text == "Prazo 6M")
                    {
                        atype = 1;
                    }
                    else if (comboBox.Text == "Prazo 1A")
                    {
                        atype = 2;
                    }
                    else if (comboBox.Text == "Prazo 3A")
                    {
                        atype = 3;
                    }

                        
                        if (!refresh())
                        return;

                    SqlCommand cmd = new SqlCommand("SELECT interest from account_type where atype=" + atype);
                    cmd.Connection = cn;
                    double baseInterest = (double)(decimal)cmd.ExecuteScalar() * 100;

                    double interest = 0;

                    if (atype==1)
                    {
                        double capital = Convert.ToDouble(textBoxc.Text);
                        double jurofinal = ((capital * baseInterest) / 180) / 365;
                        double pormes = capital * jurofinal;
                        interest = pormes / 6;
                    }
                    else if (atype==2)
                    {
                        double capital = Convert.ToDouble(textBoxc.Text);
                        double jurofinal = ((capital * baseInterest) / 365) / 365;
                        double pormes = capital * jurofinal;
                        interest = pormes / (1 * 12);
                    }
                    else if (atype==3)
                    {
                        double capital = Convert.ToDouble(textBoxc.Text);
                        double jurofinal = ((capital * baseInterest) / 1095) / 365;
                        double pormes = capital * jurofinal;
                        interest = pormes / (3 * 12);
                    }

                    //make sure unique IDs are used when creating things


                    cmd = new SqlCommand("SELECT max(id) FROM ACCOUNTS");
                    cmd.Connection = cn;
                    int newID = (int)cmd.ExecuteScalar() + 1;

                    string b = aux.ToString().Replace(",", ".");
                    string i = interest.ToString().Replace(",", ".");

                     try
                     {
                        cmd = new SqlCommand("EXEC InsertAccount " + newID + ", " + b + ", " + atype + ", " + i);
                        cmd.Connection = cn;
                        cmd.ExecuteNonQuery();

                        if (goback == "client" && autolinkid != -1)
                        {
                            cmd = new SqlCommand("INSERT INTO CLIENT_ACCOUNTS (aid, cid) VALUES (" + newID + ", " + autolinkid + ")");
                            cmd.Connection = cn;
                            cmd.ExecuteNonQuery();

                            ClientDetails window = new ClientDetails();
                            window.Show();
                            this.Hide();
                        }
                        else {
                        Accounts window = new Accounts();
                        window.Show();
                        this.Hide();
                    }
                    }
                    catch (SqlException) { MessageBox.Show("Error updating database: the balance might be too high."); }

                }
                else
                {
                    MessageBox.Show("Invalid amount.");
                }
            }
            catch (OverflowException) { MessageBox.Show("The number is too big."); }

        }

    }
}
