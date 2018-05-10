using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepChain
{
    class RepChain
    {
        public static List<Block> blockchain = new List<Block>();

        public static int difficulty = 5;

        public static void PrintChain()
        {
            for (int i = 0; i <blockchain.Count; i++)
            {
                var block = blockchain.ElementAt(i);
                Console.WriteLine("Hash: " + block.hash);
                Console.WriteLine("Prev Hash: " + block.previousHash);
                Console.WriteLine("Data: "+ block.data);
                Console.WriteLine("Timestamp: " + block.timeStamp);
                Console.WriteLine("Nonce: " + block.nonce);
                Console.WriteLine();
            }
        }

        public static bool isChainValid()
        {
            Block currentBlock;
            Block previousBlock;

            String hashTarget = new String(new char[difficulty]).Replace('\0', '0');

            //loop through blockchain to check hashes:
            for (int i = 1; i < blockchain.Count; i++)
            {
                currentBlock = blockchain.ElementAt(i);
                previousBlock = blockchain.ElementAt(i - 1);
                //compare registered hash and calculated hash:
                if (currentBlock.hash != currentBlock.calculateHash())
                {
                    Console.WriteLine("Current Hashes not equal");
                    return false;
                }
                //compare previous hash and registered previous hash
                if (previousBlock.hash != currentBlock.previousHash)
                {
                    Console.WriteLine("Previous Hashes not equal");
                    return false;
                }

                //check if hash is solved
                if (currentBlock.hash.Substring(0, difficulty) != hashTarget)
                {
                    Console.WriteLine("This block hasn't been mined");
                    return false;
                }
            }
            return true;
        }
    }
}
