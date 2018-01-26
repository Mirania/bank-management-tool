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
    /// Interaction logic for ClientFactory.xaml
    /// </summary>
    public partial class ClientFactory : Window
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

        public ClientFactory()
        {
            InitializeComponent();

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Clients window = new Clients();
            window.Show();
            this.Hide();
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
            {
            if (!refresh())
                return;

            if (!Regex.IsMatch(textBox.Text, "^[^()\\*;+='\\\\/]*$") || !Regex.IsMatch(textBox2.Text, "^[^()\\*;+='\\\\/]*$") || !Regex.IsMatch(textBox3.Text, "^[^()\\*;+='\\\\/]*$") || textBox.Text.Contains("--") || textBox2.Text.Contains("--") || textBox3.Text.Contains("--"))
            {
                MessageBox.Show("Invalid characters detected.");
                return;
            }

            //make sure unique IDs are used when creating things

            SqlCommand cmd = new SqlCommand("SELECT max(id) FROM CLIENTS");
            cmd.Connection = cn;
            int newID = (int)cmd.ExecuteScalar() + 1;

            string gender;

                    if (radioButton.IsChecked == true) {
                        gender = "Male";
                    } else if (radioButton2.IsChecked == true)
                    {
                        gender = "Female";
                    } else { gender = "Other"; }

            try
            {
                int aux = -1;
                long aux2 = -1;
                if (!int.TryParse(textBoxnif.Text, out aux))
                {
                    MessageBox.Show("Invalid NIF.");
                    return;
                }
                if (!long.TryParse(textBox4.Text, out aux2))
                {
                    MessageBox.Show("Invalid Citizen ID.");
                    return;
                }

                try
                {
                    cmd = new SqlCommand("INSERT INTO CLIENTS (id, name, addr, postal, citid, gender, nif) VALUES (" + newID + ", '" + textBox.Text + "', '" + textBox2.Text + "', '" + textBox3.Text + "', '" + textBox4.Text + "', '" + gender + "', "+aux+")");
                    cmd.Connection = cn;
                    cmd.ExecuteNonQuery();

                    Clients window = new Clients();
                    window.Show();
                    this.Hide();
                }
                catch (SqlException) { MessageBox.Show("Error updating database: the NIF might be too big, or some fields may contain too much text."); }
            }
            catch (OverflowException) { MessageBox.Show("The number is too big."); }



            }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Clear all fields?", "Clear Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                textBox.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
            }
        }
    }
}
