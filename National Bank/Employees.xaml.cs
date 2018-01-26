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
    /// Interaction logic for Employees.xaml
    /// </summary>
    public partial class Employees : Window
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

        string sql;

        public Employees()
        {
            InitializeComponent();

            sql = "SELECT id, name, salary FROM EMPLOYEES order by id";

            radioButton_Copy1.IsChecked = true;

        }

        public void header()
        {
            Style idStyle = App.Current.FindResource("HempI") as Style;
            Style nameStyle = App.Current.FindResource("HempN") as Style;
            Style sStyle = App.Current.FindResource("HempS") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;

            Border bordersout = new Border();
            StackPanel entry = new StackPanel();

            Border bordersID = new Border();
            Border bordersName = new Border();
            Border bordersS = new Border();
            TextBlock ids = new TextBlock();
            TextBlock names = new TextBlock();
            TextBlock s = new TextBlock();

            entry.Orientation = Orientation.Horizontal;

            bordersout.Style = spbStyle;
            bordersID.Style = idStyle;
            bordersName.Style = nameStyle;
            bordersS.Style = sStyle;
            ids.Style = textStyle;
            names.Style = textStyle;
            s.Style = textStyle;

            ids.Text = "Employee ID";
            names.Text = "Name";
            s.Text = "Salary";

            bordersID.Child = ids;
            bordersName.Child = names;
            bordersS.Child = s;
            entry.Children.Add(bordersID);
            entry.Children.Add(bordersName);
            entry.Children.Add(bordersS);
            bordersout.Child = entry;
            list.Items.Add(bordersout);
        }

        private void init(string sql)
        {
            header();

            if (!refresh())
                return;

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = cn;

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return; //no data
            }

            int t = dataSet.Tables[0].Rows.Count;

            Style idStyle = App.Current.FindResource("empIDBorderStyle") as Style;
            Style nameStyle = App.Current.FindResource("empNameBorderStyle") as Style;
            Style sStyle = App.Current.FindResource("empSBorderStyle") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;

            Border[] bordersout = new Border[t];
            StackPanel[] entry = new StackPanel[t];

            Border[] bordersID = new Border[t];
            Border[] bordersName = new Border[t];
            Border[] bordersS = new Border[t];
            TextBlock[] ids = new TextBlock[t];
            TextBlock[] names = new TextBlock[t];
            TextBlock[] s = new TextBlock[t];

            for (int i = 0; i < t; i++)
            {
                bordersout[i] = new Border();
                entry[i] = new StackPanel();
                entry[i].Orientation = Orientation.Horizontal;

                bordersID[i] = new Border();
                bordersName[i] = new Border();
                bordersS[i] = new Border();

                ids[i] = new TextBlock();
                names[i] = new TextBlock();
                s[i] = new TextBlock();

                bordersout[i].Style = spbStyle;
                bordersID[i].Style = idStyle;
                bordersName[i].Style = nameStyle;
                bordersS[i].Style = sStyle;
                ids[i].Style = textStyle;
                names[i].Style = textStyle;
                s[i].Style = textStyle;

                ids[i].Text = ((int)dataSet.Tables[0].Rows[i]["id"]).ToString();
                names[i].Text = (string)dataSet.Tables[0].Rows[i]["name"];
                s[i].Text = String.Format("{0:0.00}€", (decimal)dataSet.Tables[0].Rows[i]["salary"]);

                bordersID[i].Child = ids[i];
                bordersName[i].Child = names[i];
                bordersS[i].Child = s[i];
                entry[i].Children.Add(bordersID[i]);
                entry[i].Children.Add(bordersName[i]);
                entry[i].Children.Add(bordersS[i]);
                bordersout[i].Child = entry[i];
                list.Items.Add(bordersout[i]);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ManagerOptions window = new ManagerOptions();
            window.Show();
            this.Hide();
        }

        private void search(int idquery)
        {
            header();

            if (!refresh())
                return;

            SqlCommand cmd = new SqlCommand("SELECT id, name, salary FROM EMPLOYEES WHERE id=" + idquery);
            cmd.Connection = cn;

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            if (dataSet.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("That ID was not found in the database."); return; //no data
            }

            int t = dataSet.Tables[0].Rows.Count;


            Style idStyle = App.Current.FindResource("empIDBorderStyle") as Style;
            Style nameStyle = App.Current.FindResource("empNameBorderStyle") as Style;
            Style sStyle = App.Current.FindResource("empSBorderStyle") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;

            Border[] bordersout = new Border[t];
            StackPanel[] entry = new StackPanel[t];

            Border[] bordersID = new Border[t];
            Border[] bordersName = new Border[t];
            Border[] bordersS = new Border[t];
            TextBlock[] ids = new TextBlock[t];
            TextBlock[] names = new TextBlock[t];
            TextBlock[] s = new TextBlock[t];

            for (int i = 0; i < t; i++)
            {
                bordersout[i] = new Border();
                entry[i] = new StackPanel();
                entry[i].Orientation = Orientation.Horizontal;


                bordersID[i] = new Border();
                bordersName[i] = new Border();
                bordersS[i] = new Border();

                ids[i] = new TextBlock();
                names[i] = new TextBlock();
                s[i] = new TextBlock();

                bordersout[i].Style = spbStyle;
                bordersID[i].Style = idStyle;
                bordersName[i].Style = nameStyle;
                bordersS[i].Style = sStyle;
                ids[i].Style = textStyle;
                names[i].Style = textStyle;
                s[i].Style = textStyle;

                ids[i].Text = ((int)dataSet.Tables[0].Rows[i]["id"]).ToString();
                names[i].Text = (string)dataSet.Tables[0].Rows[i]["name"];
                s[i].Text = String.Format("{0:0.00}€", (decimal)dataSet.Tables[0].Rows[i]["salary"]);

                bordersID[i].Child = ids[i];
                bordersName[i].Child = names[i];
                bordersS[i].Child = s[i];
                entry[i].Children.Add(bordersID[i]);
                entry[i].Children.Add(bordersName[i]);
                entry[i].Children.Add(bordersS[i]);
                bordersout[i].Child = entry[i];
                list.Items.Add(bordersout[i]);

            }
        }

        private void search(string query)
        {
            header();

            if (!Regex.IsMatch(query, "^[^()\\*;+='\\\\/]*$") || query.Contains("--"))
            {
                MessageBox.Show("Invalid characters detected.");
                return;
            }


            if (!refresh())
                return;

            SqlCommand cmd = new SqlCommand("select * from employees WHERE name LIKE '%" + query + "%'");
            cmd.Connection = cn;

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            if (dataSet.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("That name (or part of it) were not found in the database."); return; //no data
            }

            int t = dataSet.Tables[0].Rows.Count;

            Style idStyle = App.Current.FindResource("empIDBorderStyle") as Style;
            Style nameStyle = App.Current.FindResource("empNameBorderStyle") as Style;
            Style sStyle = App.Current.FindResource("empSBorderStyle") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;

            Border[] bordersout = new Border[t];
            StackPanel[] entry = new StackPanel[t];

            Border[] bordersID = new Border[t];
            Border[] bordersName = new Border[t];
            Border[] bordersS = new Border[t];
            TextBlock[] ids = new TextBlock[t];
            TextBlock[] names = new TextBlock[t];
            TextBlock[] s = new TextBlock[t];

            for (int i = 0; i < t; i++)
            {
                bordersout[i] = new Border();
                entry[i] = new StackPanel();
                entry[i].Orientation = Orientation.Horizontal;

                bordersID[i] = new Border();
                bordersName[i] = new Border();
                bordersS[i] = new Border();

                ids[i] = new TextBlock();
                names[i] = new TextBlock();
                s[i] = new TextBlock();

                bordersout[i].Style = spbStyle;
                bordersID[i].Style = idStyle;
                bordersName[i].Style = nameStyle;
                bordersS[i].Style = sStyle;
                ids[i].Style = textStyle;
                names[i].Style = textStyle;
                s[i].Style = textStyle;

                ids[i].Text = ((int)dataSet.Tables[0].Rows[i]["id"]).ToString();
                names[i].Text = (string)dataSet.Tables[0].Rows[i]["name"];
                s[i].Text = String.Format("{0:0.00}€", (decimal)dataSet.Tables[0].Rows[i]["salary"]);

                bordersID[i].Child = ids[i];
                bordersName[i].Child = names[i];
                bordersS[i].Child = s[i];
                entry[i].Children.Add(bordersID[i]);
                entry[i].Children.Add(bordersName[i]);
                entry[i].Children.Add(bordersS[i]);
                bordersout[i].Child = entry[i];
                list.Items.Add(bordersout[i]);

            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            string query = searchBox.Text;
            int idquery = -1;

            if (query == "") { list.Items.Clear(); init(sql); }
            else
            {
                try { 
                if (int.TryParse(query, out idquery))
                {
                    list.Items.Clear();
                        try { 
                    search(idquery);
                        }
                        catch (SqlException) { MessageBox.Show("The number is too big."); }
                    }
                else
                {
                    list.Items.Clear();
                    search(query);
                }
                }
                catch (OverflowException) { MessageBox.Show("The number is too big."); }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            EmployeeFactory window = new EmployeeFactory();
            window.Show();
            this.Hide();
        }

        private void list_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) //details
        {
            ListView lv = sender as ListView;
            Border item = (Border)lv.SelectedItem;
            if (item != null && lv.SelectedIndex != 0)
            {
                if (!refresh())
                    return;

                TextBlock idcontainer = (TextBlock)((Border)((StackPanel)item.Child).Children[0]).Child;
                string id = idcontainer.Text;

                SqlCommand cmd = new SqlCommand("select * from employees join user_type on employees.utype=user_type.utype WHERE id=" + id);
                cmd.Connection = cn;

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);

                Employee eedit;

                eedit = new Employee((int)dataSet.Tables[0].Rows[0]["id"],
                    (string)dataSet.Tables[0].Rows[0]["name"],
                    (double)(decimal)dataSet.Tables[0].Rows[0]["salary"]);
                eedit.Address = (string)dataSet.Tables[0].Rows[0]["addr"];
                eedit.Postal = (string)dataSet.Tables[0].Rows[0]["postal"];
                eedit.CID = (string)dataSet.Tables[0].Rows[0]["citid"];
                eedit.Gender = (string)dataSet.Tables[0].Rows[0]["gender"];
                eedit.Username = (string)dataSet.Tables[0].Rows[0]["usern"];
                eedit.Type = (string)dataSet.Tables[0].Rows[0]["descr"];

                App.Current.Properties["eedit"] = eedit;
                EmployeeDetails window = new EmployeeDetails();
                window.Show();
                this.Hide();
                return;
            }
        }

        private void buttoncl_Click(object sender, RoutedEventArgs e)
        {
            list.Items.Clear();
            init(sql);
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            sql = "SELECT id, name, salary FROM EMPLOYEES order by name";
            list.Items.Clear();
            init(sql);
        }

        private void radioButton_Copy_Checked(object sender, RoutedEventArgs e)
        {
            sql = "SELECT id, name, salary FROM EMPLOYEES order by name desc";
            list.Items.Clear();
            init(sql);
        }

        private void radioButton_Checked2(object sender, RoutedEventArgs e)
        {
            sql = "SELECT id, name, salary FROM EMPLOYEES order by id";
            list.Items.Clear();
            init(sql);
        }
    }
}
