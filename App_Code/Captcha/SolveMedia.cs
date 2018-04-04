using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;

public class SolveMedia
{
    public static bool Verify(string input, string challenge)
    {
        using (WebClient client = new WebClient())
        {
            byte[] response =
            client.UploadValues("http://verify.solvemedia.com/papi/verify", new NameValueCollection()
            {
               { "privatekey", AppSettings.Captcha.SolveMediaVKey },
               { "challenge", challenge },
               { "response", input },
               { "remoteip", Member.GetCurrentIP(HttpContext.Current.Request) }
            });

            string result = System.Text.Encoding.UTF8.GetString(response);

            if (result.StartsWith("true"))
                return true;
            return false;
        }
    }
}