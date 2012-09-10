using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;

namespace Lab2.Utils
{
    public class Helpers
    {
        /// <summary>
        /// Skapa en SHA1-hash baserat på input och konverterar hashen till en base64-sträng
        /// </summary>
        /// <param name="input">Sträng som skall hashas</param>
        /// <returns>Hashad sträng</returns>
        public static string CreateHash(string input)
        {
            var data = Encoding.Unicode.GetBytes(input); // Hashfunktionen behöver en Byte-array som input
            var hashData = new SHA1Managed().ComputeHash(data); // Här skapar vi vår hash
            return Convert.ToBase64String(hashData); // Vi vill returnera en sträng-representation av hashen
        }
    }
}