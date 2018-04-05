using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.News
{
    public class InfluencerCodeHelper
    {
        public static string ToCode(int userId)
        {
            return Encryption.Encrypt(userId.ToString());
        }

        public static int ToUserId(string code)
        {
            return Convert.ToInt32(Encryption.Decrypt(code));
        }
    }
}