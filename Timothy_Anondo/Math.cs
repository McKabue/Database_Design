using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Timothy_Anondo
{
    public class Math
    {
        public static string DoMath(string value1, string value2)
        {
            return (int.Parse(value1) + int.Parse(value2)).ToString();
        }


        public static string RandomNumber()
        {
            int maxSize = 4;
            //int minSize = 5;
            char[] chars = new char[62];
            string a = "1234567890";
            chars = a.ToCharArray();
            //int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            //size = maxSize;
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            { result.Append(chars[b % (chars.Length - 1)]); }
            return result.ToString();
        }
    }
}
