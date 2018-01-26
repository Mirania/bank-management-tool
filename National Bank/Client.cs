using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class Client
    {
        int id;
        string name;
        string address;
        string postal;
        string cid;
        string gender;
        int nif;
        int shares;

        public Client(int id, string name)
        {
            this.id = id;
            this.name = name;
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

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        public string Address
        {
            get { return this.address; }
            set { this.address = value; }
        }
        public string Postal
        {
            get { return this.postal; }
            set { this.postal = value; }
        }
        public string Gender
        {
            get { return this.gender; }
            set { this.gender = value; }
        }
        public string CID
        {
            get { return this.cid; }
            set { this.cid = value; }
        }
        public int NIF
        {
            get
            {
                return this.nif;
            }
            set
            {
                this.nif = value;
            }

        }
        public int Shares
        {
            get
            {
                return this.shares;
            }
            set
            {
                this.shares = value;
            }

        }
    }
}
