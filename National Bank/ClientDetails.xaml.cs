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
    /// Interaction logic for ClientDetails.xaml
    /// </summary>
    public partial class ClientDetails : Window
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

        Client editing = (Client)App.Current.Properties["useredit"];
        string goback = (string)App.Current.Properties["goback"];

        public ClientDetails()
        {
            InitializeComponent();


            t1.IsEnabled = false;
            t2.IsEnabled = false;
            t4.IsEnabled = false;
            t6.IsEnabled = false;
            tnif.IsEnabled = false;
            ts.IsEnabled = false;

            t1.Text = editing.ID.ToString();
            t2.Text = editing.CID.ToString();
            t3.Text = editing.Postal;
            t4.Text = editing.Name;
            t5.Text = editing.Address;
            t6.Text = editing.Gender;
            tnif.Text = editing.NIF.ToString();
            ts.Text = editing.Shares.ToString();

            init();
        }

        public void header()
        {
            Style idStyle = App.Current.FindResource("HaccID") as Style;
            Style typeStyle = App.Current.FindResource("HaccT") as Style;
            Style balanceStyle = App.Current.FindResource("HaccB") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;

            Border bordersout = new Border();
            StackPanel entry = new StackPanel();

            Border bordersID = new Border();
            Border bordersType = new Border();
            Border bordersBalance = new Border();
            TextBlock ids = new TextBlock();
            TextBlock types = new TextBlock();
            TextBlock balances = new TextBlock();

            entry.Orientation = Orientation.Horizontal;

            bordersout.Style = spbStyle;
            bordersID.Style = idStyle;
            bordersType.Style = typeStyle;
            bordersBalance.Style = balanceStyle;
            ids.Style = textStyle;
            types.Style = textStyle;
            balances.Style = textStyle;

            ids.Text = "Account ID";
            types.Text = "Type";
            balances.Text = "Balance";

            bordersID.Child = ids;
            bordersType.Child = types;
            bordersBalance.Child = balances;
            entry.Children.Add(bordersID);
            entry.Children.Add(bordersType);
            entry.Children.Add(bordersBalance);
            bordersout.Child = entry;
            list.Items.Add(bordersout);
        }

        private void init()
        {
            header();

            if (!refresh())
                return;

            SqlCommand cmd = new SqlCommand("select accounts.id, descr, balance from ((clients join client_accounts on id=cid) join accounts on accounts.id=aid) join account_type on accounts.atype=account_type.atype where cid="+editing.ID.ToString());
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

            Style idStyle = App.Current.FindResource("accountIDBorderStyle") as Style;
            Style typeStyle = App.Current.FindResource("accountTypeBorderStyle") as Style;
            Style balanceStyle = App.Current.FindResource("accountBalanceBorderStyle") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;

            Border[] bordersout = new Border[t];
            StackPanel[] entry = new StackPanel[t];

            Border[] bordersID = new Border[t];
            Border[] bordersType = new Border[t];
            Border[] bordersBalance = new Border[t];
            TextBlock[] ids = new TextBlock[t];
            TextBlock[] types = new TextBlock[t];
            TextBlock[] balances = new TextBlock[t];

            for (int i = 0; i < t; i++)
            {
                bordersout[i] = new Border();
                entry[i] = new StackPanel();
                entry[i].Orientation = Orientation.Horizontal;

                bordersID[i] = new Border();
                bordersType[i] = new Border();
                bordersBalance[i] = new Border();

                ids[i] = new TextBlock();
                types[i] = new TextBlock();
                balances[i] = new TextBlock();

                bordersout[i].Style = spbStyle;
                bordersID[i].Style = idStyle;
                bordersType[i].Style = typeStyle;
                bordersBalance[i].Style = balanceStyle;
                ids[i].Style = textStyle;
                types[i].Style = textStyle;
                balances[i].Style = textStyle;

                ids[i].Text = ((int)dataSet.Tables[0].Rows[i]["id"]).ToString();
                types[i].Text = (string)dataSet.Tables[0].Rows[i]["descr"];
                balances[i].Text = String.Format("{0:0.00}€", (decimal)dataSet.Tables[0].Rows[i]["balance"]);

                bordersID[i].Child = ids[i];
                bordersType[i].Child = types[i];
                bordersBalance[i].Child = balances[i];
                entry[i].Children.Add(bordersID[i]);
                entry[i].Children.Add(bordersType[i]);
                entry[i].Children.Add(bordersBalance[i]);
                bordersout[i].Child = entry[i];
                list.Items.Add(bordersout[i]);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e) //save
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

            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE CLIENTS SET postal='" + postal + "', addr='" + addr + "' WHERE id=" + editing.ID.ToString());
                cmd.Connection = cn;
                cmd.ExecuteNonQuery();

                if (goback == "shares")
                {
                    Shares window = new Shares();
                    window.Show();
                    this.Hide();
                }
                else
                {
                    Clients window = new Clients();
                    window.Show();
                    this.Hide();
                }
            } catch (SqlException) { MessageBox.Show("Error updating database: some fields may contain too much text.");}
            }

        private void button_Click(object sender, RoutedEventArgs e) //return
        {
            if (goback == "shares")
            {
                Shares window = new Shares();
                window.Show();
                this.Hide();
            }
            else
            {
                Clients window = new Clients();
                window.Show();
                this.Hide();
            }
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            if (!refresh())
                return;

            string postal = t3.Text;
            string addr = t5.Text;

            if (!Regex.IsMatch(postal, "^[^()\\*;+='\\\\/]*$") || !Regex.IsMatch(addr, "^[^()\\*;+='\\\\/]*$") || postal.Contains("--") || addr.Contains("--"))
            {
                MessageBox.Show("In an attempt to save your changes, invalid characters were detected in the text fields.");
                return;
            }

            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE CLIENTS SET postal='" + postal + "', addr='" + addr + "' WHERE id=" + editing.ID.ToString());
                cmd.Connection = cn;
                cmd.ExecuteNonQuery();

                editing.Address = addr;
                editing.Postal = postal;

                ClientExtra window = new ClientExtra();
                window.Show();
                this.Hide();
            }
            catch (SqlException) { MessageBox.Show("In an attempt to save your changes, there was an error updating the database: some fields may contain too much text."); }
        }

        private void list_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!refresh())
                return;

                ListView lv = sender as ListView;
                Border item = (Border)lv.SelectedItem;
                if (item != null && lv.SelectedIndex != 0)
                {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this connection?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    TextBlock idcontainer = (TextBlock)((Border)((StackPanel)item.Child).Children[0]).Child;
                    string id = idcontainer.Text;

                    SqlCommand cmd = new SqlCommand("DELETE FROM CLIENT_ACCOUNTS WHERE aid=" + id + " AND cid=" + editing.ID.ToString());
                    cmd.Connection = cn;
                    cmd.ExecuteNonQuery();

                    list.Items.Clear();
                    init();
                    return;

                }
            }
        }

        private void buttondel_Click(object sender, RoutedEventArgs e)
        {
            if (!refresh())
                return;

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this client?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("EXEC DeleteClient " + editing.ID);
                    cmd.Connection = cn;
                    cmd.ExecuteNonQuery();

                    if (goback == "shares")
                    {
                        Shares window = new Shares();
                        window.Show();
                        this.Hide();
                    }
                    else
                    {
                        Clients window = new Clients();
                        window.Show();
                        this.Hide();
                    }

                } catch (SqlException) { MessageBox.Show("Error deleting client."); }
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e) //add
        {
            App.Current.Properties["mode"] = "other";
            ChooseAccount window = new ChooseAccount();
            window.Show();
            this.Hide();
        }

        private void buttonautolink(object sender, RoutedEventArgs e)
        {
            App.Current.Properties["goback"] = "client";
            App.Current.Properties["autolinkid"] = editing.ID;
            AccountFactory window = new AccountFactory();
            window.Show();
            this.Hide();
        }
    }
    }
