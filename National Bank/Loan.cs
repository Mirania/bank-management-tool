using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class Loan
    {
        int id;
        int userid;
        double objvalue;
        double reqvalue;
        string type;
        int accid;
        int months;
        double permonth;
        string name;
        bool appr;

        public Loan(int id, string type, double val, double permonth)
        {
            this.id = id;
            this.type = type;
            this.reqvalue = val;
            this.permonth = permonth;
        }

        public int UserID
        {
            get { return this.userid; }
            set { this.userid = value; }
        }
        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }
        public string Type
        {
            get { return this.type; }
            set { this.type= value; }
        }
        public double Reqvalue
        {
            get { return this.reqvalue; }
            set { this.reqvalue = value; }
        }

        public double PerMonth
        {
            get { return this.permonth; }
            set { this.permonth = value; }
        }

        public double Objvalue
        {
            get { return this.objvalue; }
            set { this.objvalue = value; }
        }

        public int AccID
        {
            get { return this.accid; }
            set { this.accid = value; }
        }

        public int Months
        {
            get { return this.months; }
            set { this.months = value; }
        }
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        public bool Approved
        {
            get { return this.appr; }
            set { this.appr = value; }
        }
    }
}
