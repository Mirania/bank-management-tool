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
    /// Interaction logic for ChooseAccount.xaml
    /// </summary>
    public partial class ChooseAccount : Window
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

        Client editing = (Client)App.Current.Properties["useredit"];
        string mode = (string)App.Current.Properties["mode"];
        string loancid = (string)App.Current.Properties["loancid"];

        public ChooseAccount()
        {
            InitializeComponent();

            if (mode == "loan")
            {
                sql = "select accounts.id, descr, balance from ((clients join client_accounts on id = cid) join accounts on accounts.id = aid) join account_type on accounts.atype = account_type.atype where cid=" + loancid +" and balance>0 order by id";
            }
            else
            {
                sql = "SELECT id, balance, ACCOUNTS.atype, descr FROM ACCOUNTS JOIN ACCOUNT_TYPE ON ACCOUNTS.atype=ACCOUNT_TYPE.atype EXCEPT (SELECT id, balance, ACCOUNTS.atype, descr FROM CLIENT_ACCOUNTS JOIN (ACCOUNTS JOIN ACCOUNT_TYPE ON ACCOUNTS.atype=ACCOUNT_TYPE.atype) ON aid=id and cid=" + editing.ID.ToString() + ") order by id";
            }

            radioButton_Copy1.IsChecked = true;

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
                Clients window = new Clients();
                window.Show();
                this.Hide();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e) //go back
        {
            if (mode == "loan")
            {
                ChooseClient window = new ChooseClient();
                window.Show();
                this.Hide();
            }
            else
            {
                ClientDetails window = new ClientDetails();
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
                    App.Current.Properties["loanaid"] = id;

                    LoanFactory window = new LoanFactory();
                    window.Show();
                    this.Hide();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO CLIENT_ACCOUNTS (aid, cid) VALUES (" + id + ", " + editing.ID.ToString() + ")");
                    cmd.Connection = cn;
                    cmd.ExecuteNonQuery();

                    ClientDetails window = new ClientDetails();
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
                cmd = new SqlCommand("select accounts.id, descr, balance from ((clients join client_accounts on id = cid) join accounts on accounts.id = aid) join account_type on accounts.atype = account_type.atype where cid=" + loancid + " and balance>0 and accounts.id=" + idquery);
            }
            else
            {
                cmd = new SqlCommand("SELECT id, balance, ACCOUNTS.atype, descr FROM ACCOUNTS JOIN ACCOUNT_TYPE ON ACCOUNTS.atype=ACCOUNT_TYPE.atype where id="+idquery+" EXCEPT (SELECT id, balance, ACCOUNTS.atype, descr FROM CLIENT_ACCOUNTS JOIN (ACCOUNTS JOIN ACCOUNT_TYPE ON ACCOUNTS.atype=ACCOUNT_TYPE.atype) ON aid=id and cid=" + editing.ID.ToString() + ") order by descr");
            }

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

        private void button3_Click(object sender, RoutedEventArgs e) //search
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
                    MessageBox.Show("Invalid ID.");
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

        private void radioButton_Copy2_Checked(object sender, RoutedEventArgs e)
        {
            if (mode == "loan")
            {
                sql = "select accounts.id, descr, balance from ((clients join client_accounts on id = cid) join accounts on accounts.id = aid) join account_type on accounts.atype = account_type.atype where cid=" + loancid + " and balance>0 order by descr";
            }
            else
            {
                sql = "SELECT id, balance, ACCOUNTS.atype, descr FROM ACCOUNTS JOIN ACCOUNT_TYPE ON ACCOUNTS.atype=ACCOUNT_TYPE.atype EXCEPT (SELECT id, balance, ACCOUNTS.atype, descr FROM CLIENT_ACCOUNTS JOIN (ACCOUNTS JOIN ACCOUNT_TYPE ON ACCOUNTS.atype=ACCOUNT_TYPE.atype) ON aid=id and cid=" + editing.ID.ToString() + ") order by descr";
            }
            list.Items.Clear();
            init(sql);
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (mode == "loan")
            {
                sql = "select accounts.id, descr, balance from ((clients join client_accounts on id = cid) join accounts on accounts.id = aid) join account_type on accounts.atype = account_type.atype where cid=" + loancid + " and balance>0 order by balance";
            }
            else
            {
                sql = "SELECT id, balance, ACCOUNTS.atype, descr FROM ACCOUNTS JOIN ACCOUNT_TYPE ON ACCOUNTS.atype=ACCOUNT_TYPE.atype EXCEPT (SELECT id, balance, ACCOUNTS.atype, descr FROM CLIENT_ACCOUNTS JOIN (ACCOUNTS JOIN ACCOUNT_TYPE ON ACCOUNTS.atype=ACCOUNT_TYPE.atype) ON aid=id and cid=" + editing.ID.ToString() + ") order by balance";
            }
            list.Items.Clear();
            init(sql);
        }

        private void radioButton_Copy_Checked(object sender, RoutedEventArgs e)
        {
            if (mode == "loan")
            {
                sql = "select accounts.id, descr, balance from ((clients join client_accounts on id = cid) join accounts on accounts.id = aid) join account_type on accounts.atype = account_type.atype where cid=" + loancid + " and balance>0 order by balance desc";
            }
            else
            {
                sql = "SELECT id, balance, ACCOUNTS.atype, descr FROM ACCOUNTS JOIN ACCOUNT_TYPE ON ACCOUNTS.atype=ACCOUNT_TYPE.atype EXCEPT (SELECT id, balance, ACCOUNTS.atype, descr FROM CLIENT_ACCOUNTS JOIN (ACCOUNTS JOIN ACCOUNT_TYPE ON ACCOUNTS.atype=ACCOUNT_TYPE.atype) ON aid=id and cid=" + editing.ID.ToString() + ") order by balance desc";
            }
            list.Items.Clear();
            init(sql);
        }

        private void radioButton_Copy2_Checked2(object sender, RoutedEventArgs e)
        {
            if (mode == "loan")
            {
                sql = "select accounts.id, descr, balance from ((clients join client_accounts on id = cid) join accounts on accounts.id = aid) join account_type on accounts.atype = account_type.atype where cid=" + loancid + " and balance>0 order by id";
            }
            else
            {
                sql = "SELECT id, balance, ACCOUNTS.atype, descr FROM ACCOUNTS JOIN ACCOUNT_TYPE ON ACCOUNTS.atype=ACCOUNT_TYPE.atype EXCEPT (SELECT id, balance, ACCOUNTS.atype, descr FROM CLIENT_ACCOUNTS JOIN (ACCOUNTS JOIN ACCOUNT_TYPE ON ACCOUNTS.atype=ACCOUNT_TYPE.atype) ON aid=id and cid=" + editing.ID.ToString() + ") order by id";
            }
            list.Items.Clear();
            init(sql);
        }
    }
}
