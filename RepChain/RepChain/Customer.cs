using RepChain;
using System;

namespace RepChain
{
    public class Customer
    {
        public static Random rand = new Random();

        public Wallet wallet { get; set; }

        public int id { get; set; }

        public int initialLoan { get; set; }

        public double remainingAmount { get; set; }

        public int paymentsMade { get; set; }

        public int maxPaymentsMissed { get; set; }

        public double installmentAmount { get; set; }

        public int numInstallments { get; set; }

        public int paymentsMissed { get; set; }

        public int loansRepaid { get; set; }

        public int loansDefaulted { get; set; }

        public int defaultFactor { get; set; }

        public Customer(int id, int loanAmount)
        {
            this.wallet = new Wallet();

            this.id = id;

            this.initialLoan = loanAmount;

            this.remainingAmount = loanAmount;

            this.maxPaymentsMissed = rand.Next(3, 5);

            this.numInstallments = rand.Next(10, 21);

            this.installmentAmount = ((double)loanAmount) / ((double)numInstallments);

            this.paymentsMissed = 0;

            this.loansRepaid = 0;

            this.loansDefaulted = 0;

            this.paymentsMade = 0;

            this.defaultFactor = 1;

            Console.ForegroundColor = ConsoleColor.White;
            var msg = string.Format("Customer {0} loaned {1} to be paid back in {2} installments of {3}",
                    id, loanAmount, numInstallments, installmentAmount);
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void Reset()
        {
            this.id = id;

            this.initialLoan = rand.Next(100, 1001);

            this.remainingAmount = initialLoan;

            this.maxPaymentsMissed = rand.Next(3, 5);

            this.numInstallments = rand.Next(10, 21);

            this.installmentAmount = ((double)initialLoan) / ((double)numInstallments);

            this.paymentsMissed = 0;

            this.paymentsMade = 0;

            this.defaultFactor = 1;

            Console.ForegroundColor = ConsoleColor.White;
            var msg = string.Format("Customer {0} loaned {1} to be paid back in {2} installments of {3}",
                    id, initialLoan, numInstallments, installmentAmount);
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
