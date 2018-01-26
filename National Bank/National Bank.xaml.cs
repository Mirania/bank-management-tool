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
    /// Interaction logic for Shares.xaml
    /// </summary>
    public partial class Shares : Window
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

        public Shares()
        {
            InitializeComponent();

            sql = "select nif, amt from shares join clients on cid=id order by amt";

            radioButton.IsChecked = true;

        }

        public void header()
        {
            Style clientIDStyle = App.Current.FindResource("HeaderSharesID") as Style;
            Style amountStyle = App.Current.FindResource("HeaderSharesAmt") as Style;
            Style valueStyle = App.Current.FindResource("HeaderSharesV") as Style;
            Style partStyle = App.Current.FindResource("HeaderSharesP") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;

            Border bordersout = new Border();
            StackPanel entry = new StackPanel();

            Border bordersID = new Border();
            Border bordersAmount = new Border();
            Border bordersValue = new Border();
            Border bordersPart = new Border();
            TextBlock ids = new TextBlock();
            TextBlock amounts = new TextBlock();
            TextBlock values = new TextBlock();
            TextBlock parts = new TextBlock();
            entry.Orientation = Orientation.Horizontal;

            bordersout.Style = spbStyle;
            bordersID.Style = clientIDStyle;
            bordersAmount.Style = amountStyle;
            bordersValue.Style = valueStyle;
            bordersPart.Style = partStyle;
            ids.Style = textStyle;
            amounts.Style = textStyle;
            values.Style = textStyle;
            parts.Style = textStyle;

            ids.Text = "Client NIF";
            amounts.Text = "# of shares";
            values.Text = "Net value";
            parts.Text = "% of shares";

            bordersID.Child = ids;
            bordersAmount.Child = amounts;
            bordersValue.Child = values;
            bordersPart.Child = parts;
            entry.Children.Add(bordersID);
            entry.Children.Add(bordersAmount);
            entry.Children.Add(bordersValue);
            entry.Children.Add(bordersPart);
            bordersout.Child = entry;
            list.Items.Add(bordersout);
        }

        public void init(string sql)
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

            int maxshares = (int)App.Current.Properties["maxshares"];
            double pershare = (double)App.Current.Properties["pershare"];

            Style clientIDStyle = App.Current.FindResource("sharesIDBorderStyle") as Style;
            Style amountStyle = App.Current.FindResource("sharesAmountBorderStyle") as Style;
            Style valueStyle = App.Current.FindResource("sharesValueBorderStyle") as Style;
            Style partStyle = App.Current.FindResource("sharesPartBorderStyle") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;

            Border[] bordersout = new Border[t];
            StackPanel[] entry = new StackPanel[t];

            Border[] bordersID = new Border[t];
            Border[] bordersAmount = new Border[t];
            Border[] bordersValue = new Border[t];
            Border[] bordersPart = new Border[t];
            TextBlock[] ids = new TextBlock[t];
            TextBlock[] amounts = new TextBlock[t];
            TextBlock[] values = new TextBlock[t];
            TextBlock[] parts = new TextBlock[t];

            for (int i = 0; i < t; i++)
            {
                bordersout[i] = new Border();
                entry[i] = new StackPanel();
                entry[i].Orientation = Orientation.Horizontal;

                bordersID[i] = new Border();
                bordersAmount[i] = new Border();
                bordersValue[i] = new Border();
                bordersPart[i] = new Border();

                ids[i] = new TextBlock();
                amounts[i] = new TextBlock();
                values[i] = new TextBlock();
                parts[i] = new TextBlock();

                bordersout[i].Style = spbStyle;
                bordersID[i].Style = clientIDStyle;
                bordersAmount[i].Style = amountStyle;
                bordersValue[i].Style = valueStyle;
                bordersPart[i].Style = partStyle;
                ids[i].Style = textStyle;
                amounts[i].Style = textStyle;
                values[i].Style = textStyle;
                parts[i].Style = textStyle;

                ids[i].Text = ((int)dataSet.Tables[0].Rows[i]["nif"]).ToString();
                amounts[i].Text = ((int)dataSet.Tables[0].Rows[i]["amt"]).ToString();
                values[i].Text = String.Format("{0:0.00}€", ((double)(int)dataSet.Tables[0].Rows[i]["amt"] * pershare));
                parts[i].Text = String.Format("{0:0.0}%", ((double)(int)dataSet.Tables[0].Rows[i]["amt"] / maxshares * 100));

                bordersID[i].Child = ids[i];
                bordersAmount[i].Child = amounts[i];
                bordersValue[i].Child = values[i];
                bordersPart[i].Child = parts[i];
                entry[i].Children.Add(bordersID[i]);
                entry[i].Children.Add(bordersAmount[i]);
                entry[i].Children.Add(bordersValue[i]);
                entry[i].Children.Add(bordersPart[i]);
                bordersout[i].Child = entry[i];
                list.Items.Add(bordersout[i]);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Hide();
        }

        public void search(int idquery)
        {
            header();

            if (!refresh())
                return;

            SqlCommand cmd = new SqlCommand("select nif, amt from shares join clients on cid=id WHERE nif=" + idquery);
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
            int maxshares = (int)App.Current.Properties["maxshares"];
            double pershare = (double)App.Current.Properties["pershare"];

            Style clientIDStyle = App.Current.FindResource("sharesIDBorderStyle") as Style;
            Style amountStyle = App.Current.FindResource("sharesAmountBorderStyle") as Style;
            Style valueStyle = App.Current.FindResource("sharesValueBorderStyle") as Style;
            Style partStyle = App.Current.FindResource("sharesPartBorderStyle") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;

            Border[] bordersout = new Border[t];
            StackPanel[] entry = new StackPanel[t];

            Border[] bordersID = new Border[t];
            Border[] bordersAmount = new Border[t];
            Border[] bordersValue = new Border[t];
            Border[] bordersPart = new Border[t];
            TextBlock[] ids = new TextBlock[t];
            TextBlock[] amounts = new TextBlock[t];
            TextBlock[] values = new TextBlock[t];
            TextBlock[] parts = new TextBlock[t];

            for (int i = 0; i < t; i++)
            {
                bordersout[i] = new Border();
                entry[i] = new StackPanel();
                entry[i].Orientation = Orientation.Horizontal;

                    bordersID[i] = new Border();
                    bordersAmount[i] = new Border();
                    bordersValue[i] = new Border();
                    bordersPart[i] = new Border();

                    ids[i] = new TextBlock();
                    amounts[i] = new TextBlock();
                    values[i] = new TextBlock();
                    parts[i] = new TextBlock();

                    bordersout[i].Style = spbStyle;
                    bordersID[i].Style = clientIDStyle;
                    bordersAmount[i].Style = amountStyle;
                    bordersValue[i].Style = valueStyle;
                    bordersPart[i].Style = partStyle;
                    ids[i].Style = textStyle;
                    amounts[i].Style = textStyle;
                    values[i].Style = textStyle;
                    parts[i].Style = textStyle;

                ids[i].Text = ((int)dataSet.Tables[0].Rows[i]["nif"]).ToString();
                amounts[i].Text = ((int)dataSet.Tables[0].Rows[i]["amt"]).ToString();
                values[i].Text = String.Format("{0:0.00}€", ((double)(int)dataSet.Tables[0].Rows[i]["amt"] * pershare));
                parts[i].Text = String.Format("{0:0.0}%", ((double)(int)dataSet.Tables[0].Rows[i]["amt"] / maxshares * 100));

                bordersID[i].Child = ids[i];
                    bordersAmount[i].Child = amounts[i];
                    bordersValue[i].Child = values[i];
                    bordersPart[i].Child = parts[i];
                    entry[i].Children.Add(bordersID[i]);
                    entry[i].Children.Add(bordersAmount[i]);
                    entry[i].Children.Add(bordersValue[i]);
                    entry[i].Children.Add(bordersPart[i]);
                    bordersout[i].Child = entry[i];
                    list.Items.Add(bordersout[i]);

            }
        }

        public void search(string query)
        {
            header();

            if (!Regex.IsMatch(query, "^[^()\\*;+='\\\\/]*$") || query.Contains("--"))
            {
                MessageBox.Show("Invalid characters detected.");
                return;
            }


            if (!refresh())
                return;

            SqlCommand cmd = new SqlCommand("select * from SHARES join clients ON shares.cid=clients.id WHERE name LIKE '%"+query+"%'");
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
            int maxshares = (int)App.Current.Properties["maxshares"];
            double pershare = (double)App.Current.Properties["pershare"];

            Style clientIDStyle = App.Current.FindResource("sharesIDBorderStyle") as Style;
            Style amountStyle = App.Current.FindResource("sharesAmountBorderStyle") as Style;
            Style valueStyle = App.Current.FindResource("sharesValueBorderStyle") as Style;
            Style partStyle = App.Current.FindResource("sharesPartBorderStyle") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;

            Border[] bordersout = new Border[t];
            StackPanel[] entry = new StackPanel[t];

            Border[] bordersID = new Border[t];
            Border[] bordersAmount = new Border[t];
            Border[] bordersValue = new Border[t];
            Border[] bordersPart = new Border[t];
            TextBlock[] ids = new TextBlock[t];
            TextBlock[] amounts = new TextBlock[t];
            TextBlock[] values = new TextBlock[t];
            TextBlock[] parts = new TextBlock[t];

            for (int i = 0; i < t; i++)
            {
                bordersout[i] = new Border();
                entry[i] = new StackPanel();
                entry[i].Orientation = Orientation.Horizontal;

                    bordersID[i] = new Border();
                    bordersAmount[i] = new Border();
                    bordersValue[i] = new Border();
                    bordersPart[i] = new Border();

                    ids[i] = new TextBlock();
                    amounts[i] = new TextBlock();
                    values[i] = new TextBlock();
                    parts[i] = new TextBlock();

                    bordersout[i].Style = spbStyle;
                    bordersID[i].Style = clientIDStyle;
                    bordersAmount[i].Style = amountStyle;
                    bordersValue[i].Style = valueStyle;
                    bordersPart[i].Style = partStyle;
                    ids[i].Style = textStyle;
                    amounts[i].Style = textStyle;
                    values[i].Style = textStyle;
                    parts[i].Style = textStyle;

                ids[i].Text = ((int)dataSet.Tables[0].Rows[i]["nif"]).ToString();
                amounts[i].Text = ((int)dataSet.Tables[0].Rows[i]["amt"]).ToString();
                values[i].Text = String.Format("{0:0.00}€", ((double)(int)dataSet.Tables[0].Rows[i]["amt"] * pershare));
                parts[i].Text = String.Format("{0:0.0}%", ((double)(int)dataSet.Tables[0].Rows[i]["amt"] / maxshares * 100));

                bordersID[i].Child = ids[i];
                    bordersAmount[i].Child = amounts[i];
                    bordersValue[i].Child = values[i];
                    bordersPart[i].Child = parts[i];
                    entry[i].Children.Add(bordersID[i]);
                    entry[i].Children.Add(bordersAmount[i]);
                    entry[i].Children.Add(bordersValue[i]);
                    entry[i].Children.Add(bordersPart[i]);
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

        private void buttoncl_Click(object sender, RoutedEventArgs e)
        {
            list.Items.Clear();
            init(sql);
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            sql = "select nif, amt from shares join clients on cid=id order by amt";
            list.Items.Clear();
            init(sql);
        }

        private void radioButton_Copy_Checked(object sender, RoutedEventArgs e)
        {
            sql = "select nif, amt from shares join clients on cid=id order by amt desc";
            list.Items.Clear();
            init(sql);

        }

        private void list_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            ListView lv = sender as ListView;
            Border item = (Border)lv.SelectedItem;
            if (item != null && lv.SelectedIndex != 0)
            {
                if (!refresh())
                    return;



                TextBlock idcontainer = (TextBlock)((Border)((StackPanel)item.Child).Children[0]).Child;
                string id = idcontainer.Text;

                SqlCommand cmd = new SqlCommand("SELECT * FROM CLIENTS JOIN SHARES ON cid=id and nif=" + id);
                cmd.Connection = cn;

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);

                Client useredit;

                useredit = new Client((int)dataSet.Tables[0].Rows[0]["id"],
                    (string)dataSet.Tables[0].Rows[0]["name"]);
                useredit.Address = (string)dataSet.Tables[0].Rows[0]["addr"];
                useredit.Postal = (string)dataSet.Tables[0].Rows[0]["postal"];
                useredit.CID = (string)dataSet.Tables[0].Rows[0]["citid"];
                useredit.Gender = (string)dataSet.Tables[0].Rows[0]["gender"];
                useredit.NIF = (int)dataSet.Tables[0].Rows[0]["nif"];
                useredit.Shares = (int)dataSet.Tables[0].Rows[0]["amt"];
                


                App.Current.Properties["useredit"] = useredit;
                App.Current.Properties["goback"] = "shares";
                ClientDetails window = new ClientDetails();
                window.Show();
                this.Hide();
                return;
            }
        }
    }
}
