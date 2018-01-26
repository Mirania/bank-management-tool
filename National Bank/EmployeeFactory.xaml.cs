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
using System.Text.RegularExpressions;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for EmployeeFactory.xaml
    /// </summary>
    public partial class EmployeeFactory : Window
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

        public EmployeeFactory()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Employees window = new Employees();
            window.Show();
            this.Hide();
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
            {
                double salary = -1;
            try { 
            if (double.TryParse(textBox4.Text, out salary))
                {

                if (!refresh())
                    return;

                if (salary>=10000)
                    {
                        MessageBox.Show("The salary is too high!");
                        return;
                    }

                    //make sure unique IDs are used when creating things

                    SqlCommand cmd = new SqlCommand("SELECT max(id) FROM EMPLOYEES");
                    cmd.Connection = cn;
                    int newID = (int)cmd.ExecuteScalar() + 1;

                    string gender;

                string b = salary.ToString().Replace(",", ".");

                if (radioButton.IsChecked == true)
                {
                    gender = "Male";
                }
                else if (radioButton1.IsChecked == true)
                {
                    gender = "Female";
                }
                else { gender = "Other"; }

                long aux2 = -1;

                try
                {
                    if (!long.TryParse(textBox3.Text, out aux2))
                    {
                        MessageBox.Show("Invalid Citizen ID.");
                        return;
                    }

                        if (!Regex.IsMatch(textBox.Text, "^[^()\\*;+='\\\\/]*$") || !Regex.IsMatch(textBox1.Text, "^[^()\\*;+='\\\\/]*$") ||
                            !Regex.IsMatch(textBox2.Text, "^[^()\\*;+='\\\\/]*$") || !Regex.IsMatch(textBoxu.Text, "^[^()\\*;+='\\\\/]*$") ||
                            !Regex.IsMatch(textBoxp.Password, "^[^()\\*;+='\\\\/]*$") || textBox.Text.Contains("--") || textBox1.Text.Contains("--") ||
                                textBox2.Text.Contains("--") || textBoxu.Text.Contains("--") || textBoxp.Password.Contains("--"))
                        {
                            MessageBox.Show("Invalid characters detected.");
                            return;
                        }

                        try {
                    cmd = new SqlCommand("EXEC InsertEmployee " + newID + ", '" + textBox.Text + "', '" + textBox1.Text + "', '" + textBox2.Text + "', '" + textBox3.Text + "', '" + gender + "', " + b + ", '" + textBoxu.Text + "', '" + textBoxp.Password + "'");
                    cmd.Connection = cn;
                    cmd.ExecuteNonQuery();

                    Employees window = new Employees();
                    window.Show();
                    this.Hide();
                    }

                    catch (SqlException) { MessageBox.Show("Error inserting employee. The username might already be in use."); }
                }
                catch (OverflowException) { MessageBox.Show("The Citizen ID number is too big."); }
            }
                else
                {
                    MessageBox.Show("Invalid salary.");
                }
            }
            catch (OverflowException) { MessageBox.Show("The number is too big."); }
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Clear all fields?", "Clear Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                textBox.Text = "";
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBoxu.Text = "";
                textBoxp.Password = "";
            }
        }
    }
}
