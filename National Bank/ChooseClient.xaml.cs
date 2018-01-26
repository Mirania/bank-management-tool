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
    /// Interaction logic for ChooseClient.xaml
    /// </summary>
    public partial class ChooseClient : Window
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

        Account editing = (Account)App.Current.Properties["accedit"];
        string mode = (string)App.Current.Properties["mode"];

        string sql;

        public ChooseClient()
        {
            InitializeComponent();

            if (mode == "loan")
            {
                sql = "select DISTINCT id, name, nif from clients join CLIENT_ACCOUNTS on id=cid order by id";
            }
            else
            {
                sql = "select id, name, nif from clients except (SELECT cid, name, nif FROM CLIENT_ACCOUNTS JOIN CLIENTS ON cid=id and aid=" + editing.ID.ToString() + ") order by id";
            }

            radioButton_Copy1.IsChecked = true;
        }

        public void header()
        {
            Style idStyle = App.Current.FindResource("HuserID") as Style;
            Style nameStyle = App.Current.FindResource("HuserN") as Style;
            Style nifStyle = App.Current.FindResource("HuserNIF") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;

            Border bordersout = new Border();
            StackPanel entry = new StackPanel();

            Border bordersID = new Border();
            Border bordersName = new Border();
            Border bordersNif = new Border();
            TextBlock ids = new TextBlock();
            TextBlock names = new TextBlock();
            TextBlock nifs = new TextBlock();

            entry.Orientation = Orientation.Horizontal;

            bordersout.Style = spbStyle;
            bordersID.Style = idStyle;
            bordersName.Style = nameStyle;
            bordersNif.Style = nifStyle;
            ids.Style = textStyle;
            names.Style = textStyle;
            nifs.Style = textStyle;

            ids.Text = "Client ID";
            names.Text = "Name";
            nifs.Text = "NIF";

            bordersID.Child = ids;
            bordersName.Child = names;
            bordersNif.Child = nifs;
            entry.Children.Add(bordersID);
            entry.Children.Add(bordersName);
            entry.Children.Add(bordersNif);
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

            Style idStyle = App.Current.FindResource("clientIDBorderStyle") as Style;
            Style nameStyle = App.Current.FindResource("clientNameBorderStyle") as Style;
            Style nifStyle = App.Current.FindResource("clientNIFBorderStyle") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;
            Border[] bordersout = new Border[t];
            StackPanel[] entry = new StackPanel[t];

            Border[] bordersID = new Border[t];
            Border[] bordersName = new Border[t];
            Border[] bordersNif = new Border[t];
            TextBlock[] ids = new TextBlock[t];
            TextBlock[] names = new TextBlock[t];
            TextBlock[] nifs = new TextBlock[t];

            for (int i = 0; i < t; i++)
            {
                bordersout[i] = new Border();
                entry[i] = new StackPanel();
                entry[i].Orientation = Orientation.Horizontal;

                bordersID[i] = new Border();
                bordersName[i] = new Border();
                bordersNif[i] = new Border();

                ids[i] = new TextBlock();
                names[i] = new TextBlock();
                nifs[i] = new TextBlock();

                bordersout[i].Style = spbStyle;
                bordersID[i].Style = idStyle;
                bordersName[i].Style = nameStyle;
                bordersNif[i].Style = nifStyle;
                ids[i].Style = textStyle;
                names[i].Style = textStyle;
                nifs[i].Style = textStyle;

                ids[i].Text = ((int)dataSet.Tables[0].Rows[i]["id"]).ToString();
                names[i].Text = (string)dataSet.Tables[0].Rows[i]["name"];
                nifs[i].Text = ((int)dataSet.Tables[0].Rows[i]["nif"]).ToString();

                bordersID[i].Child = ids[i];
                bordersName[i].Child = names[i];
                bordersNif[i].Child = nifs[i];
                entry[i].Children.Add(bordersID[i]);
                entry[i].Children.Add(bordersName[i]);
                entry[i].Children.Add(bordersNif[i]);
                bordersout[i].Child = entry[i];
                list.Items.Add(bordersout[i]);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (mode == "loan")
            {
                Loans window = new Loans();
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

        private void button1_Click(object sender, RoutedEventArgs e) //go back
        {
            if (mode == "loan")
            {
                Loans window = new Loans();
                window.Show();
                this.Hide();
            }
            else
            {
                AccountDetails window = new AccountDetails();
                window.Show();
                this.Hide();
            }
        }

        private void list_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) //pick
        {
            ListView lv = sender as ListView;
            Border item = (Border)lv.SelectedItem;
            if (item != null && lv.SelectedIndex != 0)
            {
                if (!refresh())
                    return;

                TextBlock idcontainer = (TextBlock)((Border)((StackPanel)item.Child).Children[0]).Child;
                string id = idcontainer.Text;

                if (mode == "loan")
                {
                    TextBlock namecontainer = (TextBlock)((Border)((StackPanel)item.Child).Children[1]).Child;
                    string name = namecontainer.Text;

                    App.Current.Properties["loancid"] = id;
                    App.Current.Properties["loancname"] = name;

                    ChooseAccount window = new ChooseAccount();
                    window.Show();
                    this.Hide();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO CLIENT_ACCOUNTS (aid, cid) VALUES (" + editing.ID.ToString() + ", " + id + ")");
                    cmd.Connection = cn;
                    cmd.ExecuteNonQuery();

                    AccountDetails window = new AccountDetails();
                    window.Show();
                    this.Hide();
                }

                
            }
        }

        private void search(int idquery)
        {
            header();

            if (!refresh())
                return;

            SqlCommand cmd;

            if (mode == "loan")
            {
                cmd = new SqlCommand("select DISTINCT id, name, nif from clients join CLIENT_ACCOUNTS on id=cid where nif=" + idquery);
            }
            else
            {
                cmd = new SqlCommand("select id, name, nif from clients where nif=" + idquery + " except (SELECT cid, name, nif FROM CLIENT_ACCOUNTS JOIN CLIENTS ON cid=id and aid= " + editing.ID.ToString() + ")");
            }

            cmd.Connection = cn;

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            if (dataSet.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("That NIF was not found in the database."); return; //no data
            }

            int t = dataSet.Tables[0].Rows.Count;

            Style idStyle = App.Current.FindResource("clientIDBorderStyle") as Style;
            Style nameStyle = App.Current.FindResource("clientNameBorderStyle") as Style;
            Style nifStyle = App.Current.FindResource("clientNIFBorderStyle") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;
            Border[] bordersout = new Border[t];
            StackPanel[] entry = new StackPanel[t];

            Border[] bordersID = new Border[t];
            Border[] bordersName = new Border[t];
            Border[] bordersNif = new Border[t];
            TextBlock[] ids = new TextBlock[t];
            TextBlock[] names = new TextBlock[t];
            TextBlock[] nifs = new TextBlock[t];

            for (int i = 0; i < t; i++)
            {
                bordersout[i] = new Border();
                entry[i] = new StackPanel();
                entry[i].Orientation = Orientation.Horizontal;

                bordersID[i] = new Border();
                bordersName[i] = new Border();
                bordersNif[i] = new Border();

                ids[i] = new TextBlock();
                names[i] = new TextBlock();
                nifs[i] = new TextBlock();

                bordersout[i].Style = spbStyle;
                bordersID[i].Style = idStyle;
                bordersName[i].Style = nameStyle;
                bordersNif[i].Style = nifStyle;
                ids[i].Style = textStyle;
                names[i].Style = textStyle;
                nifs[i].Style = textStyle;

                ids[i].Text = ((int)dataSet.Tables[0].Rows[i]["id"]).ToString();
                names[i].Text = (string)dataSet.Tables[0].Rows[i]["name"];
                nifs[i].Text = ((int)dataSet.Tables[0].Rows[i]["nif"]).ToString();

                bordersID[i].Child = ids[i];
                bordersName[i].Child = names[i];
                bordersNif[i].Child = nifs[i];
                entry[i].Children.Add(bordersID[i]);
                entry[i].Children.Add(bordersName[i]);
                entry[i].Children.Add(bordersNif[i]);
                bordersout[i].Child = entry[i];
                list.Items.Add(bordersout[i]);
            }
        }

        private void search(string query)
        {
            header();

            if (!refresh())
                return;

            SqlCommand cmd;

            if (mode == "loan")
            {
                cmd = new SqlCommand("select DISTINCT id, name, nif from clients join CLIENT_ACCOUNTS on id=cid where name LIKE '%"+query+"%'");
            }
            else
            {
               cmd = new SqlCommand("select id, name, nif from clients where name LIKE '%" + query + "%' except(SELECT cid, name, nif FROM CLIENT_ACCOUNTS JOIN CLIENTS ON cid=id and aid = " + editing.ID.ToString() + ")");
            }

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

            Style idStyle = App.Current.FindResource("clientIDBorderStyle") as Style;
            Style nameStyle = App.Current.FindResource("clientNameBorderStyle") as Style;
            Style nifStyle = App.Current.FindResource("clientNIFBorderStyle") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;
            Border[] bordersout = new Border[t];
            StackPanel[] entry = new StackPanel[t];

            Border[] bordersID = new Border[t];
            Border[] bordersName = new Border[t];
            Border[] bordersNif = new Border[t];
            TextBlock[] ids = new TextBlock[t];
            TextBlock[] names = new TextBlock[t];
            TextBlock[] nifs = new TextBlock[t];

            for (int i = 0; i < t; i++)
            {
                bordersout[i] = new Border();
                entry[i] = new StackPanel();
                entry[i].Orientation = Orientation.Horizontal;

                bordersID[i] = new Border();
                bordersName[i] = new Border();
                bordersNif[i] = new Border();

                ids[i] = new TextBlock();
                names[i] = new TextBlock();
                nifs[i] = new TextBlock();

                bordersout[i].Style = spbStyle;
                bordersID[i].Style = idStyle;
                bordersName[i].Style = nameStyle;
                bordersNif[i].Style = nifStyle;
                ids[i].Style = textStyle;
                names[i].Style = textStyle;
                nifs[i].Style = textStyle;

                ids[i].Text = ((int)dataSet.Tables[0].Rows[i]["id"]).ToString();
                names[i].Text = (string)dataSet.Tables[0].Rows[i]["name"];
                nifs[i].Text = ((int)dataSet.Tables[0].Rows[i]["nif"]).ToString();

                bordersID[i].Child = ids[i];
                bordersName[i].Child = names[i];
                bordersNif[i].Child = nifs[i];
                entry[i].Children.Add(bordersID[i]);
                entry[i].Children.Add(bordersName[i]);
                entry[i].Children.Add(bordersNif[i]);
                bordersout[i].Child = entry[i];
                list.Items.Add(bordersout[i]);
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e) //search
        {
            string query = searchBox.Text;
            int idquery = -1;

            if (query == "") { list.Items.Clear(); init(sql); }
            else
            {
                try
                {
                    if (int.TryParse(query, out idquery))
                    {
                        list.Items.Clear();
                        try
                        {
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

        private void buttoncl_Click(object sender, RoutedEventArgs e)
        {
            list.Items.Clear();
            init(sql);
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (mode == "loan")
            {
                sql = "select DISTINCT clients.id, name, nif from (clients join CLIENT_ACCOUNTS on id=cid) join accounts on accounts.id=aid WHERE balance>0 order by name";
            }
            else
            {
                sql = "select id, name, nif from clients except (SELECT cid, name, nif FROM CLIENT_ACCOUNTS JOIN CLIENTS ON cid=id and aid=" + editing.ID.ToString() + ") order by name";
            }
            list.Items.Clear();
            init(sql);
        }

        private void radioButton_Copy_Checked(object sender, RoutedEventArgs e)
        {
            if (mode == "loan")
            {
                sql = "select DISTINCT clients.id, name, nif from (clients join CLIENT_ACCOUNTS on id=cid) join accounts on accounts.id=aid WHERE balance>0 order by name desc";
            }
            else
            {
                sql = "select id, name, nif from clients except (SELECT cid, name, nif FROM CLIENT_ACCOUNTS JOIN CLIENTS ON cid=id and aid=" + editing.ID.ToString() + ") order by name desc";
            }
            list.Items.Clear();
            init(sql);
        }

        private void radioButton_Checked2(object sender, RoutedEventArgs e)
        {
            if (mode == "loan")
            {
                sql = "select DISTINCT clients.id, name, nif from (clients join CLIENT_ACCOUNTS on id=cid) join accounts on accounts.id=aid WHERE balance>0 order by id";
            }
            else
            {
                sql = "select id, name, nif from clients except (SELECT cid, name, nif FROM CLIENT_ACCOUNTS JOIN CLIENTS ON cid=id and aid=" + editing.ID.ToString() + ") order by id";
            }
            list.Items.Clear();
            init(sql);
        }
    }
}
