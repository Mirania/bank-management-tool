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
    /// Interaction logic for AccountDetails.xaml
    /// </summary>
    public partial class AccountDetails : Window
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

        public AccountDetails()
        {
            InitializeComponent();

            t1.IsEnabled = false;
            t2.IsEnabled = false;
            comboBox.IsEnabled = false;
            comboBox.Items.Add("Ordem");
            comboBox.Items.Add("Prazo 6M");
            comboBox.Items.Add("Prazo 1A");
            comboBox.Items.Add("Prazo 3A");

            if (editing.Type=="Ordem")
            {
                comboBox.SelectedIndex = 0;
            } else if (editing.Type=="Prazo 6M")
            {
                comboBox.SelectedIndex = 1;
            }
            else if (editing.Type == "Prazo 1A")
            {
                comboBox.SelectedIndex = 2;
            }
            else if (editing.Type == "Prazo 3A")
            {
                comboBox.SelectedIndex = 3;
            }
            t1.Text = editing.ID.ToString();
            t2.Text = String.Format("{0:0.00}", editing.Interest);
            t3.Text = String.Format("{0:0.00}", editing.Balance);

            init();
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

        private void init()
        {
            header();

            if (!refresh())
                return;

            SqlCommand cmd = new SqlCommand("SELECT cid, name, nif FROM CLIENT_ACCOUNTS JOIN CLIENTS ON cid=id WHERE aid="+editing.ID.ToString());
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

                ids[i].Text = ((int)dataSet.Tables[0].Rows[i]["cid"]).ToString();
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

        private void button1_Click(object sender, RoutedEventArgs e) //save
        {
            decimal balance = -1;
            try
            {
                if (decimal.TryParse(t3.Text, out balance))
                {
                    if (!refresh())
                        return;

                    SqlCommand cmd = new SqlCommand("SELECT * FROM ACCOUNT_TYPE WHERE descr='" + comboBox.SelectedItem.ToString() + "'");
                    cmd.Connection = cn;

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    int atype = (int)dataSet.Tables[0].Rows[0]["atype"];
                    string b = balance.ToString().Replace(",", ".");

                    try
                    {
                        cmd = new SqlCommand("UPDATE ACCOUNTS SET balance=" + b + ", atype=" + atype + " WHERE id=" + editing.ID.ToString());
                        cmd.Connection = cn;
                        cmd.ExecuteNonQuery();


                        Accounts window = new Accounts();
                        window.Show();
                        this.Hide();
                    }
                    catch (SqlException) { MessageBox.Show("Error updating database: the balance might be too high."); }
                }
                else
                {
                    MessageBox.Show("Invalid balance.");
                }
            }
            catch (OverflowException) { MessageBox.Show("The number is too big."); }
        }

        private void button2_Click(object sender, RoutedEventArgs e) //add
        {
            App.Current.Properties["mode"] = "other";
            ChooseClient window = new ChooseClient();
            window.Show();
            this.Hide();

        }

        private void button_Click(object sender, RoutedEventArgs e) //return
        {
            Accounts window = new Accounts();
            window.Show();
            this.Hide();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedIndex==0)
            {
                t2.Text = String.Format("{0:0.0}%", 0.0 * 100);
            } else if (comboBox.SelectedIndex == 1)
            {
                t2.Text = String.Format("{0:0.0}%", 0.02 * 100);
            }
            else if (comboBox.SelectedIndex == 2)
            {
                t2.Text = String.Format("{0:0.0}%", 0.025 * 100);
            }
            else if (comboBox.SelectedIndex == 3)
            {
                t2.Text = String.Format("{0:0.0}%", 0.03 * 100);
            }
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            decimal balance = -1;
            try
            {
                if (decimal.TryParse(t3.Text, out balance))
                {
                    if (!refresh())
                        return;

                    SqlCommand cmd = new SqlCommand("SELECT * FROM ACCOUNT_TYPE WHERE descr='" + comboBox.SelectedItem.ToString() + "'");
                    cmd.Connection = cn;

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    int atype = (int)dataSet.Tables[0].Rows[0]["atype"];
                    string b = balance.ToString().Replace(",", ".");

                    try
                    {
                        cmd = new SqlCommand("UPDATE ACCOUNTS SET balance=" + b + ", atype=" + atype + " WHERE id=" + editing.ID.ToString());
                        cmd.Connection = cn;
                        cmd.ExecuteNonQuery();

                        editing.Balance = (double)balance;


                        AccountExtra window = new AccountExtra();
                        window.Show();
                        this.Hide();
                    }
                    catch (SqlException) { MessageBox.Show("In an attempt to save your changes, there was an error updating the database: the balance might be too high."); }
                }
                else
                {
                    MessageBox.Show("In an attempt to save your changes, an invalid balance was detected.");
                }
            }
            catch (OverflowException) { MessageBox.Show("In an attempt to save your changes, an error ocurred: the balance might be too high."); }

   

        }

        private void list_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) //del
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

                    SqlCommand cmd = new SqlCommand("DELETE FROM CLIENT_ACCOUNTS WHERE cid=" + id + " AND aid="+editing.ID.ToString());
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

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this account?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("EXEC DeleteAccount " + editing.ID);
                    cmd.Connection = cn;
                    cmd.ExecuteNonQuery();

                    Accounts window = new Accounts();
                    window.Show();
                    this.Hide();
                }
                catch (SqlException) { MessageBox.Show("Error deleting account."); }


            }
        }
    }
}
