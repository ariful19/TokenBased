using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace tockenBased.Models
{
    public static class Actions
    {
        static byte[] Salt
        {
            get
            {
                return Encoding.UTF8.GetBytes("assalamualaikum");
            }
        }

        public static bool IsValide(string password, byte[] hash)
        {
            return hash.SequenceEqual(getHash(password));
        }

        public static byte[] getHash(string password)
        {
            var r = RIPEMD160.Create();
            var bfr = password.GetBytes().Concat(Salt).ToArray();
            return r.ComputeHash(bfr);
        }

        public static byte[] GetBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        private static byte[] key = new byte[] { 12, 22, 33, 12, 12, 42, 33, 12, 12, 22, 33, 12, 12, 42, 33, 12, 12, 22, 33, 12, 12, 42, 33, 12, 12, 22, 33, 12, 12, 42, 33, 12 };
        private static byte[] iv = new byte[] { 12, 22, 33, 12, 12, 42, 33, 12, 12, 22, 33, 12, 12, 42, 33, 12 };
        public static string EncryptObject(object obj)
        {
            var c = Rijndael.Create();
            using (var enc = c.CreateEncryptor(key, iv))
            {
                using (var ms = new MemoryStream())
                {
                    using (var cr = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                    {
                        var bfr = Newtonsoft.Json.JsonConvert.SerializeObject(obj).GetBytes();
                        cr.Write(bfr, 0, bfr.Length);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static T DecryptObject<T>(string str)
        {
            var c = Rijndael.Create();
            using (var enc = c.CreateDecryptor(key, iv))
            {
                using (var ms = new MemoryStream())
                {
                    using (var cr = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                    {
                        var bfr = Convert.FromBase64String(str);
                        cr.Write(bfr, 0, bfr.Length);
                    }

                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(ms.ToArray()));
                }
            }
        }
    }
}