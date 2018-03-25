using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tockenBased.Models
{
    public class UserViewModel
    {
        public string name { get; set; }
        public int age { get; set; }
        public string address { get; set; }
        public string p1 { get; set; }
        public string p2 { get; set; }
    }
    public class User
    {
        public Guid ID { get; set; }
        public string name { get; set; }
        public int age { get; set; }
        public string address { get; set; }
        public byte[] pwdhash { get; set; }
    }

    public class TokenObj
    {
        public string userid { get; set; }
        public DateTime Expires { get; set; }

        public bool isExpired()
        {            
            return Expires < DateTime.Now;
        }
        public TokenObj AddTime()
        {
            Expires = DateTime.Now.AddSeconds(20);
            return this;
        }
        public string ToTokenString()
        {
            return Actions.EncryptObject(this);
        }

        public static TokenObj Get(string token)
        {
            return Actions.DecryptObject<TokenObj>(token);
        }
    }
}