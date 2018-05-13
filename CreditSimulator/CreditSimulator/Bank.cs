using RepChain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditSimulator
{
    class Bank
    {
        public static Random rand = new Random();

        public Wallet wallet { get; set; }

        public int id { get; set; }

        public List<Customer> customers { get; set; }

        public Bank(int id)
        {
            this.wallet = new Wallet();

            this.id = id;

            this.customers = new List<Customer>();
        }

        public void MakeLoan(int amount, Customer customer)
        {

        }
    }
}
