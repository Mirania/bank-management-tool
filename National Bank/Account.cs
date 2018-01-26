using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    public class Account
    {
        double balance;
        int id;
        string type;
        double interest;

        public Account(int id, string type, double balance)
        {
            this.balance = balance;
            this.id = id;
            this.type = type;
            if (type=="Ordem")
            {
                interest = 0;
            } else if (type=="Prazo 6M")
            {
                interest = 0.02;
            }
            else if (type == "Prazo 1A")
            {
                interest = 0.025;
            }
            else if (type == "Prazo 3A")
            {
                interest = 0.03;
            }
        }

        public double Balance
        {
            get
            {
                return this.balance;
            }
            set
            {
                this.balance = value;
            }
        }

        public int ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }

        }

        public string Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
        public double Interest
        {
            get { return this.interest; }
            set { this.interest = value; }
        }
    }
}
