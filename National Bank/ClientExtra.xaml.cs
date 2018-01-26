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
    /// Interaction logic for ClientExtra.xaml
    /// </summary>
    public partial class ClientExtra : Window
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

        public ClientExtra()
        {
            InitializeComponent();

            init();
        }

        public void header()
        {
            Style accountIDStyle = App.Current.FindResource("HloansID") as Style;
            Style typeStyle = App.Current.FindResource("HloansT2") as Style;
            Style valueStyle = App.Current.FindResource("HloansV") as Style;
            Style interestStyle = App.Current.FindResource("HloansI2") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;

            Border bordersout = new Border();
            StackPanel entry = new StackPanel();

            Border bordersID = new Border();
            Border bordersType = new Border();
            Border bordersValue = new Border();
            Border bordersInterest = new Border();
            TextBlock ids = new TextBlock();
            TextBlock types = new TextBlock();
            TextBlock values = new TextBlock();
            TextBlock interests = new TextBlock();

            entry.Orientation = Orientation.Horizontal;

            bordersout.Style = spbStyle;
            bordersID.Style = accountIDStyle;
            bordersType.Style = typeStyle;
            bordersValue.Style = valueStyle;
            bordersInterest.Style = interestStyle;
            ids.Style = textStyle;
            types.Style = textStyle;
            values.Style = textStyle;
            interests.Style = textStyle;

            ids.Text = "Loan ID";
            types.Text = "Type";
            values.Text = "Requested Value";
            interests.Text = "Monthly Value";

            bordersID.Child = ids;
            bordersType.Child = types;
            bordersValue.Child = values;
            bordersInterest.Child = interests;
            entry.Children.Add(bordersID);
            entry.Children.Add(bordersType);
            entry.Children.Add(bordersValue);
            entry.Children.Add(bordersInterest);
            bordersout.Child = entry;
            list.Items.Add(bordersout);
        }

        public void init()
        {
            header();

            if (!refresh())
                return;

            int cid = editing.ID;

            SqlCommand cmd = new SqlCommand("select id, descr, reqval, value from ((loans join loan_final_interest on lid=id) join loan_type on loans.ltype=loan_type.ltype) where cid=" + cid + " and appr='yes'");
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

            Style accountIDStyle = App.Current.FindResource("loanIDBorderStyle") as Style;
            Style typeStyle = App.Current.FindResource("loanType2BorderStyle") as Style;
            Style valueStyle = App.Current.FindResource("loanValueBorderStyle") as Style;
            Style interestStyle = App.Current.FindResource("loanInterest2BorderStyle") as Style;
            Style textStyle = App.Current.FindResource("BorderTextStyle") as Style;
            Style spbStyle = App.Current.FindResource("spBorderStyle") as Style;

            Border[] bordersout = new Border[t];
            StackPanel[] entry = new StackPanel[t];

            Border[] bordersID = new Border[t];
            Border[] bordersType = new Border[t];
            Border[] bordersValue = new Border[t];
            Border[] bordersInterest = new Border[t];
            TextBlock[] ids = new TextBlock[t];
            TextBlock[] types = new TextBlock[t];
            TextBlock[] values = new TextBlock[t];
            TextBlock[] interests = new TextBlock[t];

            for (int i = 0; i < t; i++)
            {
                bordersout[i] = new Border();
                entry[i] = new StackPanel();
                entry[i].Orientation = Orientation.Horizontal;

                    bordersID[i] = new Border();
                    bordersType[i] = new Border();
                    bordersValue[i] = new Border();
                    bordersInterest[i] = new Border();

                    ids[i] = new TextBlock();
                    types[i] = new TextBlock();
                    values[i] = new TextBlock();
                    interests[i] = new TextBlock();

                    bordersout[i].Style = spbStyle;
                    bordersID[i].Style = accountIDStyle;
                    bordersType[i].Style = typeStyle;
                    bordersValue[i].Style = valueStyle;
                    bordersInterest[i].Style = interestStyle;
                    ids[i].Style = textStyle;
                    types[i].Style = textStyle;
                    values[i].Style = textStyle;
                    interests[i].Style = textStyle;

                ids[i].Text = ((int)dataSet.Tables[0].Rows[i]["id"]).ToString();
                types[i].Text = (string)dataSet.Tables[0].Rows[i]["descr"];
                values[i].Text = String.Format("{0:0.00}€", (decimal)dataSet.Tables[0].Rows[i]["reqval"]);
                interests[i].Text = String.Format("{0:0.00}€/month", (decimal)dataSet.Tables[0].Rows[i]["value"]);

                bordersID[i].Child = ids[i];
                    bordersType[i].Child = types[i];
                    bordersValue[i].Child = values[i];
                    bordersInterest[i].Child = interests[i];
                    entry[i].Children.Add(bordersID[i]);
                    entry[i].Children.Add(bordersType[i]);
                    entry[i].Children.Add(bordersValue[i]);
                    entry[i].Children.Add(bordersInterest[i]);
                    bordersout[i].Child = entry[i];
                    list.Items.Add(bordersout[i]);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ClientDetails window = new ClientDetails();
            window.Show();
            this.Hide();
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

                SqlCommand cmd = new SqlCommand("select loans.id, loans.cid, loans.aid, name, objval, reqval, months, descr, interest, value, appr from ((loans join loan_type on loans.ltype = loan_type.ltype) join loan_final_interest on loan_final_interest.lid = id) join clients on cid=clients.id where loans.id=" + id + " and appr='yes'");
                cmd.Connection = cn;

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);

                Loan ledit;

                ledit = new Loan((int)dataSet.Tables[0].Rows[0]["id"],
                    (string)dataSet.Tables[0].Rows[0]["descr"],
                    (double)(decimal)dataSet.Tables[0].Rows[0]["reqval"],
                    (double)(decimal)dataSet.Tables[0].Rows[0]["value"]);
                ledit.AccID = (int)dataSet.Tables[0].Rows[0]["aid"];
                ledit.UserID = (int)dataSet.Tables[0].Rows[0]["cid"];
                ledit.Objvalue = (double)(decimal)dataSet.Tables[0].Rows[0]["objval"];
                ledit.Months = (int)dataSet.Tables[0].Rows[0]["months"];
                ledit.Name = (string)dataSet.Tables[0].Rows[0]["name"];
                ledit.Approved = true;

                App.Current.Properties["ledit"] = ledit;
                App.Current.Properties["goback"] = "client";
                LoanDetails window = new LoanDetails();
                window.Show();
                this.Hide();
                return;
            }
        }
    }
}
