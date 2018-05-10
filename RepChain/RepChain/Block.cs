using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepChain
{
    public class Block
    {
        public String hash;
        public String previousHash;
        public String data; //our data will be a simple message.
        public long timeStamp; // in epoch time (seconds)
        public int nonce;

        //Block Constructor.
        public Block(String data, String previousHash)
        {
            this.data = data;
            this.previousHash = previousHash;
            this.timeStamp = (long) (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
            this.nonce = 0;
            this.hash = calculateHash();
        }

        public String calculateHash()
        {
            String calculatedhash = StringUtil.ApplySHA256(
                    previousHash +
                    timeStamp +
                    nonce +
                    data
                    );
            return calculatedhash;
        }

        public void mineBlock(int difficulty)
        {
            var target = new String(new char[difficulty]).Replace('\0', '0'); //Create a string with difficulty * "0" 
            while (hash.Substring(0, difficulty) != target)
            {
                /*Console.WriteLine(hash.Substring(0, difficulty));
                Console.WriteLine(target);
                Console.WriteLine("");*/
                nonce++;
                hash = calculateHash();
            }
            Console.WriteLine("Block Mined: " + hash);
        }
    }
}
