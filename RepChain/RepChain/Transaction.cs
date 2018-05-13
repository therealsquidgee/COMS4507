using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepChain
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Transaction
    {
        public Wallet miner;
        public Wallet payer;
        public Wallet payee;

        public String hash;
        public String previousHash;
        public int value; //our data will be a simple message.
        public long timeStamp; // in epoch time (seconds)
        public int nonce;
        public Random rand = new Random();

        //Transaction Constructor.
        public Transaction(int value, String previousHash, Wallet miner, Wallet payer, Wallet payee)
        {
            this.value = value;
            this.previousHash = previousHash;
            this.timeStamp = (long) (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
            this.nonce = 0;

            this.miner = miner;
            this.payer = payer;
            this.payee = payee;

            this.hash = calculateHash();
        }

        public String calculateHash()
        {
            String calculatedhash = StringUtil.ApplySHA256(
                    previousHash +
                    timeStamp +
                    nonce +
                    value +
                    payer.id.ToString() +
                    payee.id.ToString()
                    );
            return calculatedhash;
        }

        public void mineTransaction(int difficulty)
        {
            var target = new String(new char[difficulty]).Replace('\0', '0'); //Create a string with difficulty * "0" 
            while (hash.Substring(0, difficulty) != target)
            {
                /*Console.WriteLine(hash.Substring(0, difficulty));
                Console.WriteLine(target);
                Console.WriteLine("");*/
                nonce = rand.Next();
                hash = calculateHash();
            }
        }
    }
}
