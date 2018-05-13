using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepChain
{
    class RepChain
    {
        public List<Transaction> blockchain;

        public static int difficulty = 5;

        public EventWaitHandle waitHandle;

        public static int reward = 10;

        public RepChain()
        {
            blockchain = new List<Transaction>();
            waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "SHARED_BY_ALL_PROCESSES");
        }

        public void PrintChain()
        {
            for (int i = 0; i <blockchain.Count; i++)
            {
                var transaction = blockchain.ElementAt(i);
                Console.WriteLine("Miner: " + transaction.miner.id.ToString());
                Console.WriteLine("Payer: " + transaction.payer.id.ToString());
                Console.WriteLine("Payee: " + transaction.payee.id.ToString());
                Console.WriteLine("Hash: " + transaction.hash);
                Console.WriteLine("Prev Hash: " + transaction.previousHash);
                Console.WriteLine("Data: "+ transaction.value);
                Console.WriteLine("Timestamp: " + transaction.timeStamp);
                Console.WriteLine("Nonce: " + transaction.nonce);
                Console.WriteLine();
            }
        }

        public bool isChainValid()
        {
            Transaction currentTransaction;
            Transaction previousTransaction;

            String hashTarget = new String(new char[difficulty]).Replace('\0', '0');

            var wallets = new List<Wallet>();
            wallets.Add(blockchain.First().miner);

            //loop through blockchain to check hashes:
            for (int i = 1; i < blockchain.Count; i++)
            {
                currentTransaction = blockchain.ElementAt(i);
                previousTransaction = blockchain.ElementAt(i - 1);
                //compare registered hash and calculated hash:
                if (currentTransaction.hash != currentTransaction.calculateHash())
                {
                    Console.WriteLine("Current Hashes not equal");
                    return false;
                }
                //compare previous hash and registered previous hash
                if (previousTransaction.hash != currentTransaction.previousHash)
                {
                    Console.WriteLine("Previous Hashes not equal");
                    return false;
                }

                //check if hash is solved
                if (currentTransaction.hash.Substring(0, difficulty) != hashTarget)
                {
                    //If the next Transaction to mine has been included, the chain is still valid.
                    if(blockchain.ElementAt(i).nonce == 0 && i == blockchain.Count - 1)
                    {

                    }
                    else
                    {
                        Console.WriteLine("This Transaction hasn't been mined");
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Rolls back a blockchain to the last valid transaction, 
        /// returns true if there was a valid transaction, false otherwise.
        /// </summary>
        internal bool RollBack()
        {
            while (!isChainValid() && blockchain.Count != 0)
            {
                blockchain.RemoveAt(blockchain.Count - 1);
            }

            if(blockchain.Count == 0)
            {
                return false;
            }
            return true;
        }
    }
}
