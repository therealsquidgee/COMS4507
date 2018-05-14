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
        //Transaction chain path
        static readonly string transChainPath = "C:/transChain.json";
        static readonly string repChainPath = "C:/repChain.json";
        static readonly string walletPath = "C:/wallets.json";

        public static RepChain repChain = new RepChain(5);
        public static RepChain transChain = new RepChain(4);


        public static List<Customer> customers;

        public static Wallet bankWallet;

        public static Random rand;

        public static EventWaitHandle waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "SHARED_BY_ALL_PROCESSES_FOR_WALLETS");

        static void Main(string[] args)
        {

            rand = new Random();

            /*if (File.Exists(walletPath))
            {
                wallets = ReadWallets();
            }
            else
            {
                wallets = new List<Wallet>();
            }*/

            bankWallet = new Wallet();
            bankWallet.balance += 9900;
            //wallets.Add(wallet);
            //WriteWallets(wallets);

            transChain.blockchain = CheckLedger();
            var blockchain = transChain.blockchain;
            var i = blockchain.Count - 1;

            customers = new List<Customer>();
            customers = InitCustomers();

            var repaymentRate = 0.05;

            //Console.ReadKey();

            while (true)
            {

                for (int j = 0; j < customers.Count; j++)
                {
                    var customer = customers.ElementAt(j);

                    var repaymentChance = rand.NextDouble() * (1 / customer.defaultFactor);

                    if (repaymentChance < repaymentRate)
                    {
                        DefaultPayment(customer);
                    }
                    else
                    {
                        MakePayment(customer);
                    }
                }

                //Console.WriteLine("Trying to mine Transaction " + (i + 1) + "...");

                /*Customer nextPayer = randomWallet();

                var amount = nextPayer.installmentAmount/*rand.Next(1, 11)*/;

                /*while (nextPayer == nextPayee && nextPayer.balance >= amount)
                {
                    nextPayer = randomWallet();
                }

                blockchain.Add(
                    new Transaction(amount,
                    blockchain.Last().hash,
                    wallet, nextPayer.wallet, wallet));

                mineTransaction();*/

                Console.ReadKey();
                Console.WriteLine("");
            }
        }

        private static void MakePayment(Customer customer)
        {
            customer.remainingAmount -= customer.installmentAmount;
            customer.paymentsMade++;

            transChain.blockchain.Add(new Transaction(customer.installmentAmount,
                transChain.blockchain.Last().hash,
                bankWallet, customer.wallet, bankWallet));

            mineTransaction();

            //TODO: Add RepChain logic in here.

            if (customer.paymentsMade == customer.numInstallments)
            {
                LoanRepaid(customer);
                return;
            }

            var msg = string.Format("Customer {0} has made a payment of {1}, remaining balance is {2}, {3} payments left.",
                customer.id,
                customer.installmentAmount,
                customer.remainingAmount,
                customer.numInstallments - customer.paymentsMade);
            Console.WriteLine(msg);
        }

        private static void LoanRepaid(Customer customer)
        {
            customer.loansRepaid++;

            //TODO: Add RepChain logic here.

            Console.ForegroundColor = ConsoleColor.Green;
            var msg = string.Format("Customer {0} has repaid their loan, {1} loans repaid, {2} loans defaulted.",
                customer.id,
                customer.loansRepaid,
                customer.loansDefaulted);
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;

            customer.Reset();
        }

        private static void DefaultPayment(Customer customer)
        {
            customer.paymentsMissed++;
            customer.defaultFactor++;

            //TODO: Add Repchain logic here if necessary.

            if (customer.paymentsMissed == customer.maxPaymentsMissed)
            {
                DefaultLoan(customer);
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            var msg = string.Format("Customer {0} has missed a payment, this is their {1}th missed payment, only {2} more payments can be missed, remaining balance is {3}.",
                customer.id,
                customer.paymentsMissed,
                customer.maxPaymentsMissed - customer.paymentsMissed - 1,
                customer.remainingAmount);
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void DefaultLoan(Customer customer)
        {
            customer.loansDefaulted++;

            //TODO: Add Repchain logic in here.

            Console.ForegroundColor = ConsoleColor.Red;
            var msg = string.Format("Customer {0} has defaulted on their loan, {1} loans repaid, {2} loans defaulted.",
                customer.id,
                customer.loansRepaid,
                customer.loansDefaulted);
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;

            customer.Reset();
        }

        private static void mineTransaction()
        {
            var blockchain = transChain.blockchain;
            var i = blockchain.Count - 1;

            /*if(blockchain.Count == 1)
            {
                blockchain.Last().mineTransaction(transChain.difficulty)

                WriteBlockchain(blockchain);
                return;
            }*/

                blockchain.Last().mineTransaction(transChain.difficulty);

            var temp = CheckLedger();

            /*wallets = ReadWallets();
            while (wallets.Count <= 1)
            {
                wallets = ReadWallets();
            };*/
            if (transChain.isChainValid() && blockchain.Count >= temp.Count)
            {
                var verifiedTransaction = blockchain.Last();

                Console.WriteLine("Transaction Mined: " + verifiedTransaction.hash);
                Console.WriteLine("");
                //transChain.PrintChain();

                //Decrease payer
                if(bankWallet.id == verifiedTransaction.payer.id)
                {
                    bankWallet.balance -= verifiedTransaction.value;
                }
                else
                {
                    customers.FirstOrDefault(x => x.wallet.id == verifiedTransaction.payer.id)
                        .wallet.balance -= verifiedTransaction.value;
                }

                //Increase payee ALWAYS BANK FOR NOW
                if (bankWallet.id == verifiedTransaction.payee.id)
                {
                    bankWallet.balance += verifiedTransaction.value;
                }
                else
                {
                    customers.FirstOrDefault(x => x.wallet.id == verifiedTransaction.payee.id)
                        .wallet.balance += verifiedTransaction.value;
                }

                //Reward miner ALWAYS BANK FOR NOW
                bankWallet.balance += RepChain.reward;
                /*wallets.FirstOrDefault(x => x.id == verifiedTransaction.miner.id)
                    .balance += RepChain.reward;*/

                //WriteWallets(wallets);

                //Wallet nextPayee = randomWallet();
                /*Customer nextPayer = randomWallet();

                var amount = nextPayer.installmentAmount/*rand.Next(1, 11)*/;

                /*while (nextPayer == nextPayee && nextPayer.balance >= amount)
                {
                    nextPayer = randomWallet();
                }

                blockchain.Add(
                    new Transaction(amount,
                    verifiedTransaction.hash,
                    wallet, nextPayer.wallet, wallet));*/

                WriteBlockchain(blockchain);

                i++;
            }
            else if (blockchain.Count != CheckLedger().Count)
            {
                var tempChain = new RepChain(transChain.difficulty);
                tempChain.blockchain = ReadBlockchain();
                if (tempChain.isChainValid())
                {
                    transChain.blockchain = tempChain.blockchain;
                    blockchain = transChain.blockchain;
                    i = blockchain.Count - 1;
                }
                else
                {
                    Console.WriteLine("Rejecting chain, rolling back.");
                    if (tempChain.RollBack())
                    {
                        transChain.blockchain = tempChain.blockchain;
                        blockchain = transChain.blockchain;
                    }
                    WriteBlockchain(blockchain);
                }
            }
        }

        private static List<Customer> InitCustomers()
        {
            var numCustomers = rand.Next(5, 11);

            for (int j = 0; j < numCustomers; j++)
            {
                var loanAmount = rand.Next(100, 1001);

                var customer = new Customer(j, loanAmount);
                customers.Add(customer);

                var prevHash = transChain.blockchain.Count == 0 ? "0" : transChain.blockchain.Last().hash;

                transChain.blockchain.Add(new Transaction(loanAmount, prevHash,
                    bankWallet, bankWallet, customer.wallet));

                mineTransaction();
            }

            return customers;
        }

        private static Customer randomWallet()
        {
            return customers.ElementAt(rand.Next(0, customers.Count));
        }

        private static void WriteBlockchain(List<Transaction> blockchain)
        {
            transChain.waitHandle.WaitOne();
            File.WriteAllText(transChainPath, JsonConvert.SerializeObject(blockchain));
            transChain.waitHandle.Set();
        }

        private static List<Transaction> ReadBlockchain()
        {
            var blockchain = new List<Transaction>();

            transChain.waitHandle.WaitOne();
            blockchain = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(transChainPath));
            transChain.waitHandle.Set();

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
            if (File.Exists(transChainPath))
            {
                blockchain = ReadBlockchain();
            }
            else
            {
                //Console.WriteLine("Generating first Transaction...");

                blockchain = new List<Transaction>();

                //blockchain.Add(new Transaction(0, "0", wallet, wallet, wallet));

                //WriteBlockchain(blockchain);

                //WriteWallets(wallets);
            }

            Console.WriteLine("");
            return blockchain;
        }
    }
}
