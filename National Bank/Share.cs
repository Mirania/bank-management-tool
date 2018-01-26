using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class Share
    {
        int cid;
        int amt;

        public Share(int cid, int amt)
        {
            this.cid = cid;
            this.amt = amt;
        }

        public int CID
        {
            get { return this.cid; }
            set { this.cid = value; }
        }
        public int Amt
        {
            get { return this.amt; }
            set { this.amt = value; }
        }
    }
}
