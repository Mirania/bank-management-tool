using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class Employee
    {
        int id;
        string name;
        string address;
        string postal;
        string gender;
        double salary;
        string cid;
        string username;
        string type;

        public Employee(int id, string name, double salary)
        {
            this.id = id;
            this.name = name;
            this.salary = salary;
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
        public double Salary
        {
            get { return this.salary; }
            set { this.salary = value; }
        }
        public string Username
        {
            get { return this.username; }
            set { this.username = value; }
        }
        public string Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
    }
}
