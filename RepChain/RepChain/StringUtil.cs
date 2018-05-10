using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace RepChain
{
    public class StringUtil
    {
        //Applies Sha256 to a string and returns the result. 
        public static String ApplySHA256(String input)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                SHA256Managed hashstring = new SHA256Managed();
                byte[] hash = hashstring.ComputeHash(bytes);
                string hashString = string.Empty;
                foreach (byte x in hash)
                {
                    hashString += String.Format("{0:x2}", x);
                }
                return hashString;
            }
            catch (Exception e)
            {
                return "Exception: " + e.Message;
            }
        }
    }
}
