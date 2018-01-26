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
    /// Interaction logic for LoanDetails.xaml
    /// </summary>
    public partial class LoanDetails : Window
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

        Loan editing = (Loan)App.Current.Properties["ledit"];
        string goback = (string)App.Current.Properties["goback"];

        public LoanDetails()
        {
            InitializeComponent();

            t1.IsEnabled = false;
            t2.IsEnabled = false;
            t3.IsEnabled = false;
            t4.IsEnabled = false;
            t5.IsEnabled = false;
            t6.IsEnabled = false;
            t7.IsEnabled = false;
            t8.IsEnabled = false;
            t9.IsEnabled = false;
            t10.IsEnabled = false;

            t1.Text = editing.ID.ToString();
            t2.Text = editing.AccID.ToString();
            t3.Text = editing.Type;
            t4.Text = editing.UserID.ToString();
            t5.Text = editing.Name;

            t6.Text = String.Format("{0:0.00}", editing.Objvalue);
            t7.Text = editing.Months.ToString();
            t8.Text = String.Format("{0:0.00}", editing.Reqvalue);
            t9.Text = String.Format("{0:0.00}", editing.PerMonth);

            if (editing.Approved) {
                t10.Text = "Approved";
                a.IsEnabled = false;
                d.IsEnabled = false;
            }
            else {
                t10.Text = "Not approved";
                a.IsEnabled = true;
                d.IsEnabled = true;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e) //return
        {
            if (goback == "client")
            {
                ClientExtra window = new ClientExtra();
                window.Show();
                this.Hide();
            }
            else if (goback == "account")
            {
                AccountExtra window = new AccountExtra();
                window.Show();
                this.Hide();
            }
            else
            {
                if (editing.Approved)
                {
                    Loans window = new Loans();
                    window.Show();
                    this.Hide();
                }
                else
                {
                    LoansApprove window = new LoansApprove();
                    window.Show();
                    this.Hide();
                }
            }
        }

        private void buttona_Click(object sender, RoutedEventArgs e)
        {
            if (!refresh())
                return;

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to approve this loan?", "Approval Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {

                SqlCommand cmd = new SqlCommand("UPDATE LOANS SET appr='yes' WHERE id=" + editing.ID.ToString());
                cmd.Connection = cn;
                cmd.ExecuteNonQuery();

                LoansApprove window = new LoansApprove();
                window.Show();
                this.Hide();
            }
        }

        private void buttond_Click(object sender, RoutedEventArgs e)
        {
            if (!refresh())
                return;

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this loan?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
 
                SqlCommand cmd = new SqlCommand("EXEC DeleteLoan " + editing.ID);
                cmd.Connection = cn;
                cmd.ExecuteNonQuery();

                LoansApprove window = new LoansApprove();
                window.Show();
                this.Hide();
            }
        }
    }
}
