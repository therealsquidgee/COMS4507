using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepChain
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            Console.WriteLine("Generating first block...");
            RepChain.blockchain.Add(new Block("Block 1", "0"));
            Console.WriteLine("");

            while (true)
            {
                Console.WriteLine("Trying to mine block " + (i + 1) + "...");
                RepChain.blockchain.ElementAt(i).mineBlock(RepChain.difficulty);
                if (RepChain.isChainValid())
                {
                    RepChain.PrintChain();
                    RepChain.blockchain.Add(
                        new Block("Block " + (i + 2), RepChain.blockchain.ElementAt(RepChain.blockchain.Count - 1).hash));
                    i++;
                }

                //Console.ReadKey();
                Console.WriteLine("");
            }
        }
    }
}
