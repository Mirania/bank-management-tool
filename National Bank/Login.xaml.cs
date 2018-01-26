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
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private SqlConnection getcn()
        {
            string con = "";
            try
            {
                con = System.IO.File.ReadAllText(@"sqlstring.txt");
            }
            catch (System.IO.FileNotFoundException) {
                con = "Server=tcp: *snip*\\SQLSERVER2012,8293; Database=p4g4; User Id=p4g4; Password=pword";    
            }

            App.Current.Properties["db"] = con;
            return new SqlConnection(con);

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

        public Login()
        {

            cn = getcn();
            cn.Open();

            App.Current.Properties["maxshares"] = 1000000;
            App.Current.Properties["pershare"] = 22.75;

            InitializeComponent();

        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e) //enter pressed
        {
            if (e.Key == Key.Return)
            {
                verificar();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            verificar();
        }

        public void verificar()
        {
            if (!refresh())
                return;

            string user = username.Text;
            string pw = password.Password;

            if (!Regex.IsMatch(user, "^[^()\\*;+='\\\\/]*$") || !Regex.IsMatch(pw, "^[^()\\*;+='\\\\/]*$") || user.Contains("--") || pw.Contains("--"))
            {
                MessageBox.Show("Invalid characters detected.");
                return;
            }

            SqlCommand cmd = new SqlCommand("EXEC GetLoginInfo '"+user+"'");
            cmd.Connection = cn;

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            if (dataSet.Tables[0].Rows.Count > 0)
            {
                    if (dataSet.Tables[0].Rows[0]["pword"].ToString() == pw)
                    {
                        App.Current.Properties["logintype"] = dataSet.Tables[0].Rows[0]["descr"];
                        MainWindow main = new MainWindow();
                        main.Show();
                        this.Hide();
                    } else
                {
                    MessageBox.Show("Wrong password.");
                    return;
                }
                
            } else
            {
                MessageBox.Show("Username not found.");
            }
            
        }


    }


}
