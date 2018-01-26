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
using System.Text.RegularExpressions;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for EmployeeDetails.xaml
    /// </summary>
    public partial class EmployeeDetails : Window
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

        Employee editing = (Employee)App.Current.Properties["eedit"];

        public EmployeeDetails()
        {
            InitializeComponent();

            t1.IsEnabled = false;
            t2.IsEnabled = false;
            t4.IsEnabled = false;
            t6.IsEnabled = false;
            t8.IsEnabled = false;
            t9.IsEnabled = false;

            t1.Text = editing.ID.ToString();
            t2.Text = editing.CID.ToString();
            t3.Text = editing.Postal;
            t4.Text = editing.Name;
            t5.Text = editing.Address;
            t6.Text = editing.Gender;
            t7.Text = String.Format("{0:0.00}", editing.Salary);
            t8.Text = editing.Username;
            if (editing.Type=="employee") { t9.Text = "Employee"; }
            else if (editing.Type == "admin") { t9.Text = "Admin"; }


        }

        private void button_Click(object sender, RoutedEventArgs e) //return
        {
            Employees window = new Employees();
            window.Show();
            this.Hide();
        }

        private void button1_Click(object sender, RoutedEventArgs e) //save
        {
            double s = -1;

            try { 
            if (double.TryParse(t7.Text, out s))
            {
                if (!refresh())
                    return;

                string postal = t3.Text;
                string addr = t5.Text;

                    if (!Regex.IsMatch(postal, "^[^()\\*;+='\\\\/]*$") || !Regex.IsMatch(addr, "^[^()\\*;+='\\\\/]*$") || postal.Contains("--") || addr.Contains("--"))
                    {
                        MessageBox.Show("Invalid characters detected.");
                        return;
                    }

                    string b = s.ToString().Replace(",", ".");

                    try
                    {
                        SqlCommand cmd = new SqlCommand("UPDATE EMPLOYEES SET postal='" + postal + "', addr='" + addr + "', salary=" + b + " WHERE id=" + editing.ID.ToString());
                        cmd.Connection = cn;
                        cmd.ExecuteNonQuery();
                    }
            catch (SqlException) { MessageBox.Show("Error updating database: the balance might be too high, or some fields may contain too much text."); }

            Employees window = new Employees();
                window.Show();
                this.Hide();
            } else
            {
                MessageBox.Show("Invalid salary.");
            }
            }
            catch (OverflowException) { MessageBox.Show("The number is too big."); }
        }

        private void buttondel_Click(object sender, RoutedEventArgs e)
        {
            if (!refresh())
                return;

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this employee?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {

                int idquery = editing.ID;

                SqlCommand cmd = new SqlCommand("DELETE FROM EMPLOYEES WHERE id=" + idquery);
                cmd.Connection = cn;
                cmd.ExecuteNonQuery();

                Employees window = new Employees();
                window.Show();
                this.Hide();
            }
        }
    }
}
