using System;
using System.Text;
using System.Security.Cryptography;

namespace Communication
{
    public class Tools
    {
        /// <summary>
        /// This method makes hash of a string. To match the CAPICOM hash
        /// we convert the string to UNICODE first.
        /// </summary>
        /// <returns></returns>
        public static string HashString(string str)
        {
            // First, we create the hash provider
            SHA1 sha = new SHA1CryptoServiceProvider();
            string hashedValue = string.Empty;

            // Create the hash
            byte[] hashedData = sha.ComputeHash(Encoding.Unicode.GetBytes(str));

            // Convert the hash into string, looping for each byte
            foreach (byte b in hashedData)
            {
                hashedValue += String.Format("{0,2:X2}", b);
            }

            return hashedValue;
        }
    }
}