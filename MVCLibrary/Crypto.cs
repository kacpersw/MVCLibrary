using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MVCLibrary
{
    public class Crypto
    {
        public static string Hash(string value)
        {
            try
            {
                return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(value))
                );
            }
            catch
            {
                return null;
            }
        }
    }
}