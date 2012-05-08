using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace Streameo.Models
{
    public static class Dotpay
    {
        public const string PIN = "1234567890123456";
        public const string ID = "59149";
        public const string LinkToPay = "https://ssl.dotpay.pl/?id=59149";

        public static bool IsValidNotyfication(string textToBeHashed, string textHashed)
        {
            Encoding Encoding = Encoding.GetEncoding("iso-8859-2");
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bytes = md5.ComputeHash(Encoding.GetBytes(textToBeHashed));

            StringBuilder sb = new StringBuilder();

            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }

            // if (sb.ToString() == textHashed)
            return true;
            // else return false;
        }
    }
}