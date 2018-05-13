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
        static readonly string blockchainPath = "C:/blockchain.json";
        static readonly string walletPath = "C:/wallets.json";
        public static RepChain repChain = new RepChain();

        public static Wallet wallet;
        public static List<Wallet> wallets;

        public static Random rand;

        public static EventWaitHandle waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "SHARED_BY_ALL_PROCESSES_FOR_WALLETS");

        static void Main(string[] args) { 

            if (File.Exists(walletPath))
            {
                wallets = ReadWallets();
            }
            else
            {
                wallets = new List<Wallet>();
            }

            wallet = new Wallet();
            wallets.Add(wallet);
            WriteWallets(wallets);

            repChain.blockchain = CheckLedger();
            var blockchain = repChain.blockchain;
            var i = blockchain.Count - 1;

            rand = new Random();

            //Console.ReadKey();

            while (true)
            {

                Console.WriteLine("Trying to mine Transaction " + (i + 1) + "...");
                blockchain.ElementAt(i).mineTransaction(RepChain.difficulty);
                var temp = CheckLedger();

                wallets = ReadWallets();
                while (wallets.Count <= 1)
                {
                    wallets = ReadWallets();
                };
                if (repChain.isChainValid() && blockchain.Count >= temp.Count)
                {
                    var verifiedTransaction = blockchain.Last();

                    Console.WriteLine("Transaction Mined: " + verifiedTransaction.hash);
                    Console.WriteLine("");
                    repChain.PrintChain();

                    //Decrease payer
                    wallets.FirstOrDefault(x => x.id == verifiedTransaction.payer.id)
                        .balance -= verifiedTransaction.value;

                    //Increase payee
                    wallets.FirstOrDefault(x => x.id == verifiedTransaction.payee.id)
                        .balance += verifiedTransaction.value;

                    //Reward miner
                    wallets.FirstOrDefault(x => x.id == verifiedTransaction.miner.id)
                        .balance += RepChain.reward;

                    WriteWallets(wallets);

                    Wallet nextPayee = randomWallet();
                    Wallet nextPayer = randomWallet();

                    var amount = 10/*rand.Next(1, 11)*/;

                    while (nextPayer == nextPayee && nextPayer.balance >= amount)
                    {
                        nextPayer = randomWallet();
                    }

                    blockchain.Add(
                        new Transaction(amount, 
                        verifiedTransaction.hash,
                        wallets.FirstOrDefault(x => x.id == wallet.id), nextPayer, nextPayee));

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

        private static Wallet randomWallet()
        {
            return wallets.ElementAt(rand.Next(0, wallets.Count));
        }

        private static void WriteBlockchain(List<Transaction> blockchain)
        {
            repChain.waitHandle.WaitOne();
            File.WriteAllText(blockchainPath, JsonConvert.SerializeObject(blockchain));
            repChain.waitHandle.Set();
        }

        private static List<Transaction> ReadBlockchain()
        {
            var blockchain = new List<Transaction>();

            repChain.waitHandle.WaitOne();
            blockchain = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(blockchainPath));
            repChain.waitHandle.Set();

            return blockchain;
        }

        private static void WriteWallets(List<Wallet> wallets)
        {
            waitHandle.WaitOne();
            File.WriteAllText(walletPath, JsonConvert.SerializeObject(wallets));
            waitHandle.Set();
        }

        private static List<Wallet> ReadWallets()
        {
            waitHandle.WaitOne();
            var wallets = JsonConvert.DeserializeObject<List<Wallet>>(File.ReadAllText(walletPath));
            waitHandle.Set();

            return wallets;
        }

        private static List<Transaction> CheckLedger()
        {
            List<Transaction> blockchain;
            //Check blockchain exists
            if (File.Exists(blockchainPath))
            {
                blockchain = ReadBlockchain();
            }
            else
            {
                Console.WriteLine("Generating first Transaction...");

                blockchain = new List<Transaction>();

                blockchain.Add(new Transaction(0, "0", wallet, wallet, wallet));

                WriteBlockchain(blockchain);

                WriteWallets(wallets);
            }

            Console.WriteLine("");
            return blockchain;
        }
    }
}
