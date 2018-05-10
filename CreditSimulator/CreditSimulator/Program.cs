using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditSimulator
{
    class Program
    {
        public static List<Customer> customers;
        public static Random rand = new Random();

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Welcome to the Credit Simulator, press any key to start...");
            Console.ReadKey();

            customers = InitCustomers();

            Console.WriteLine("");

            Console.WriteLine("Now that these loans have been taken out, press any key to start progressing through the weekly repayments...");
            Console.ReadKey();
            Console.WriteLine("");

            var i = 1;

            while (true)
            {
                Console.WriteLine("Week " + i + ":");
                var repaymentRate = rand.NextDouble() * 0.03;

                for (int j = 0; j < customers.Count; j++)
                {
                    var customer = customers.ElementAt(j);

                    var repaymentChance = rand.NextDouble() * (1/customer.defaultFactor);

                    if(repaymentChance < repaymentRate)
                    {
                        DefaultPayment(customer);
                    }
                    else
                    {
                        MakePayment(customer);
                    }
                }

                Console.ReadKey();
                Console.WriteLine("");
                i++;
            }
        }

        private static void MakePayment(Customer customer)
        {
            customer.remainingAmount -= customer.installmentAmount;
            customer.paymentsMade++;

            if(customer.paymentsMade == customer.numInstallments)
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

            if(customer.paymentsMissed == customer.maxPaymentsMissed)
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

            Console.ForegroundColor = ConsoleColor.Red;
            var msg = string.Format("Customer {0} has defaulted on their loan, {1} loans repaid, {2} loans defaulted.",
                customer.id,
                customer.loansRepaid,
                customer.loansDefaulted);
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;

            customer.Reset();
        }

        private static List<Customer> InitCustomers()
        {
            var numCustomers = rand.Next(5, 11);

            var customers = new List<Customer>();

            for(int i = 0; i < numCustomers; i++)
            {
                var loanAmount = rand.Next(100, 1001);

                var customer = new Customer(i, loanAmount);
                customers.Add(customer);
            }

            return customers;
        }
    }
}
