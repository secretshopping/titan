using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;
using Prem.PTC;
using Prem.PTC.Members;

public class Coinhive
{
    public static bool Verify(string token)
    {
        using (WebClient client = new WebClient())
        {
            byte[] response =
            client.UploadValues("https://api.coinhive.com/token/verify", new NameValueCollection()
            {
               { "secret", AppSettings.Captcha.CoinhiveSecretKey },
               { "token", token },
               { "hashes", AppSettings.Captcha.CoinhiveHashes.ToString() }
            });
            
            JToken result = JObject.Parse( System.Text.Encoding.UTF8.GetString(response));
            bool isValid = (bool)result.SelectToken("success");

            if (isValid)
                return true;

            return false;
        }
    }
}