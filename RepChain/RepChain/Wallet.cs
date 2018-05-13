using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepChain
{
    [Serializable]
    public class Wallet
    {
        public Guid id { get; set; }

        public int balance { get; set; }

        public Wallet()
        {
            id = Guid.NewGuid();
            balance = 100;
        }
    }
}
