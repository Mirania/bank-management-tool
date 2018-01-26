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
    /// Interaction logic for LoanFactory.xaml
    /// </summary>
    public partial class LoanFactory : Window
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

        string logintype = (string)App.Current.Properties["logintype"];

        string cid = (string)App.Current.Properties["loancid"];
        string cname = (string)App.Current.Properties["loancname"];
        string aid = (string)App.Current.Properties["loanaid"];

        int index;

        public LoanFactory()
        {
            InitializeComponent();
            index = -1;
            textBox0.IsEnabled = false;
            textBox2.IsEnabled = false;
            textBox5.IsEnabled = false;
            textBox0.Text = cid + " (" + cname + ")";
            textBox2.Text = aid;
            this.comboBox1.Items.Add("Credito a Habitaçao");
            this.comboBox1.Items.Add("Credito Pessoal");

            comboBox1.SelectedIndex = 0;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Clear all fields?", "Clear Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                textBox.Clear();
                textBox1.Clear();
                textBox4.Clear();
                textBox5.Clear();
            }
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            double ov = -1;
            double rv = -1;
            int time = -1;

            try {
            if (double.TryParse(textBox.Text, out ov) && double.TryParse(textBox1.Text, out rv)
                && int.TryParse(textBox4.Text, out time)
                )
            {
                if (!refresh())
                    return;

                if (rv>ov)
                    {
                        MessageBox.Show("Requested value must not be higher than the object's value!");
                        return;
                    }

                    //make sure unique IDs are used when creating things

                    SqlCommand cmd = new SqlCommand("SELECT max(id) FROM LOANS");
                    cmd.Connection = cn;
                    int newID = (int)cmd.ExecuteScalar() + 1;

                    string o = ov.ToString().Replace(",", ".");
                    string r = rv.ToString().Replace(",", ".");

                try {
                        double juro = juromensalfunc(ov, rv); //finds index too
                        double monthlyvalue = calcularmensal(ov, rv, juro);
                        string mo = monthlyvalue.ToString().Replace(",", ".");

                        try {
                            if (logintype == "admin")
                            {

                                cmd = new SqlCommand("EXEC ManagerInsertLoan "+ newID + ", " + cid + ", " + aid + ", " + o + ", " + r + ", " +index + ", " + time + ", "+mo);
                                cmd.Connection = cn;
                                cmd.ExecuteNonQuery();

                            }
                            else
                            {

                                cmd = new SqlCommand("EXEC EmployeeInsertLoan " + newID + ", " + cid + ", " + aid + ", " + o + ", " + r + ", " + index + ", " + time + ", " + mo);
                                cmd.Connection = cn;
                                cmd.ExecuteNonQuery();

                                MessageBox.Show("Your loan is now pending for approval.");
                            };
                            Loans window = new Loans();
                            window.Show();
                            this.Hide();
                        }
                        catch (SqlException) { MessageBox.Show("Error updating database: some values may be too big."); }
                    }
                    catch (OverflowException) { MessageBox.Show("Overflow in the monthly value calculation."); }
                } else
            {
                MessageBox.Show("Some of the fields may contain non-numeric values.");
            }
            }
            catch (OverflowException) { MessageBox.Show("One or more numbers are too big."); }
        }

        private void button_Click_2(object sender, RoutedEventArgs e)
        {
            double ov = 0;
            double rv = 0;
            int time = 0;
            try
            {
                if (double.TryParse(textBox.Text, out ov) && double.TryParse(textBox1.Text, out rv)
                    && int.TryParse(textBox4.Text, out time)
                    )
                {
                    if (rv > ov)
                    {
                        MessageBox.Show("Requested value must not be higher than the object's value!");
                        return;
                    }
                    double juro = juromensalfunc(ov, rv);
                    calcularmensal(ov, rv, juro);
                }
                else
                {
                    MessageBox.Show("Please insert numeric values in every field.");
                }
            } catch (OverflowException) { MessageBox.Show("One or more numbers are too big."); }

        }
        private double juromensalfunc(double ov, double rv)
        {

            double percent = rv / ov;

            if (percent <= 1 && percent >= 0.8)
            {
                if (comboBox1.Text == "Credito a Habitaçao")
                {
                    index = 0;
                }
                else
                {
                    index = 4;
                }
            }
            else if (percent < 0.8 && percent >= 0.6)
            {
                if (comboBox1.Text == "Credito a Habitaçao")
                {
                    index = 1;
                }
                else
                {
                    index = 5;
                }
            }
            else if (percent < 0.6 && percent >= 0.4)
            {
                if (comboBox1.Text == "Credito a Habitaçao")
                {
                    index = 2;
                }
                else
                {
                    index = 6;
                }
            }
            else if (percent < 0.4)
            {
                if (comboBox1.Text == "Credito a Habitaçao")
                {
                    index = 3;
                }
                else
                {
                    index = 7;
                }
            }

            if (!refresh())
                return 0;

            SqlCommand cmd = new SqlCommand("select interest from loan_type where ltype = " + index);
            cmd.Connection = cn;
            return (double)(decimal)cmd.ExecuteScalar();
        }
        private double calcularmensal(double ov, double rv, double juro)
        {
            double percent = rv / ov;
            double anosemmeses = Convert.ToDouble(textBox4.Text);
            double juromensal = (percent * juro)/ 12;
            double mensal = rv / anosemmeses;
            double mensalfinal = mensal + (mensal * juromensal);
            int mensalfinalarredondado = Convert.ToInt32(mensalfinal);
            textBox5.Text = mensalfinalarredondado.ToString();

            return mensalfinalarredondado;


        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            ChooseAccount window = new ChooseAccount();
            window.Show();
            this.Hide();
        }
    }
}
