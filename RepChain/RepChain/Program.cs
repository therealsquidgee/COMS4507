using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepChain
{
    class Program
    {
        static readonly string path = "C:/blockchain.json";
        public static RepChain repChain = new RepChain();

        public static Wallet wallet = new Wallet();

        public static EventWaitHandle waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "SHARED_BY_ALL_PROCESSES_FOR_WALLETS");

        static void Main(string[] args)
        {
            repChain.blockchain = CheckLedger();
            var blockchain = repChain.blockchain;
            var i = blockchain.Count - 1;

            //Console.ReadKey();

            while (true)
            {
                Console.WriteLine("Trying to mine Transaction " + (i + 1) + "...");
                blockchain.ElementAt(i).mineTransaction(RepChain.difficulty);
                var temp = CheckLedger();
                if (repChain.isChainValid() && blockchain.Count >= temp.Count)
                {
                    Console.WriteLine("Transaction Mined: " + blockchain.ElementAt(blockchain.Count - 1).hash);
                    Console.WriteLine("");
                    repChain.PrintChain();

                    blockchain.Add(
                        new Transaction("Transaction " + (i + 2), blockchain.ElementAt(blockchain.Count - 1).hash));

                    WriteBlockchain(blockchain);

                    i++;
                }
                else if(blockchain.Count != CheckLedger().Count)
                {
                    var tempChain = new RepChain();
                    tempChain.blockchain = ReadBlockchain();
                    if (tempChain.isChainValid())
                    {
                        repChain.blockchain = tempChain.blockchain;
                        blockchain = repChain.blockchain;
                        i = blockchain.Count - 1;
                    }
                    else
                    {
                        Console.WriteLine("Rejecting chain, rolling back.");
                        if (tempChain.RollBack())
                        {
                            repChain.blockchain = tempChain.blockchain;
                            blockchain = repChain.blockchain;
                        }
                        WriteBlockchain(blockchain);
                    }
                }

                //Console.ReadKey();
                Console.WriteLine("");
            }
        }

        private static void WriteBlockchain(List<Transaction> blockchain)
        {
            repChain.waitHandle.WaitOne();
            File.WriteAllText(path, JsonConvert.SerializeObject(blockchain));
            repChain.waitHandle.Set();
        }

        private static List<Transaction> ReadBlockchain()
        {
            var blockchain = new List<Transaction>();

            repChain.waitHandle.WaitOne();
            blockchain = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(path));
            repChain.waitHandle.Set();

            return blockchain;
        }

        private static void WriteWallets(List<Wallet> blockchain)
        {
            waitHandle.WaitOne();
            File.WriteAllText(path, JsonConvert.SerializeObject(blockchain));
            waitHandle.Set();
        }

        private static List<Transaction> ReadWallets()
        {
            var blockchain = new List<Transaction>();

            waitHandle.WaitOne();
            blockchain = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(path));
            waitHandle.Set();

            return blockchain;
        }

        private static List<Transaction> CheckLedger()
        {
            List<Transaction> blockchain;
            //Check blockchain exists
            if (File.Exists(path))
            {
                blockchain = ReadBlockchain();
            }
            else
            {
                Console.WriteLine("Generating first Transaction...");

                blockchain = new List<Transaction>();

                blockchain.Add(new Transaction(0, "0", wallet, wallet, wallet));

                WriteBlockchain(blockchain);

                wallet.balance += RepChain.reward;
            }

            Console.WriteLine("");
            return blockchain;
        }
    }
}
