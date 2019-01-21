using System;
using System.Security.Cryptography;
using System.Text;

namespace BtcMarketsApiClient.Helpers
{
    public class SecurityHelper
    {
        public static string ComputeHash(string privateKey, string data)
        {
            var encoding = Encoding.UTF8;
            using (var hasher = new HMACSHA512(Convert.FromBase64String(privateKey)))
            {
                return Convert.ToBase64String(hasher.ComputeHash(encoding.GetBytes(data)));
            }
        }
    }
}