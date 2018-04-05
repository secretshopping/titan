using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;


public class DomainUpdater
{
    private static readonly string WWW = "www.";

    public static void UpdateWithWWWRedirection(string domain)
    {
        try
        {        
            string Url = "http://" + domain + "/";

            var request = WebRequest.Create(Url) as HttpWebRequest;

            using (var client = new MyWebClient())
            {
                client.DownloadStringWithHeaders(Url);
                string response = client.ResponseUri.Host.ToLower();
                if (response.StartsWith(WWW) && !domain.StartsWith(WWW))
                {
                    //Add WWW
                    UpdateDomain(domain, WWW + domain);
                }
                else if (!response.StartsWith(WWW) && domain.StartsWith(WWW))
                {
                    //Remove WWW
                    UpdateDomain(domain, domain.Substring(WWW.Length));
                }
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    public static void UpdateWithSSLPolicy()
    {
        string domain = HttpContext.Current.User.Identity.Name;

        if (AppSettings.Site.Url != "http://" + domain + "/" && AppSettings.SSL.Type == SSLType.No)
        {
            AppSettings.Site.Url = "http://" + domain + "/";
            AppSettings.Site.Save();
        }

        if (AppSettings.Site.Url != "https://" + domain + "/" && AppSettings.SSL.Type == SSLType.Yes)
        {
            AppSettings.Site.Url = "https://" + domain + "/";
            AppSettings.Site.Save();
        }
   
    }

    public static void CheckServerTimeDifference()
    {
        try
        {
            using (var client = new WebClient())
            {
                string time = client.DownloadStringWithHeaders(AppSettings.Site.Url + "Handlers/Utils/ServerTime.ashx");
                DateTime serverTime = DateTime.Parse(time, CultureInfo.CreateSpecificCulture("en-US"));
                DateTime adminTime = DateTime.Now;

                TimeSpan difference = serverTime.Subtract(adminTime);
                int differenceInt = Convert.ToInt32(difference.TotalHours);

                if (differenceInt != AppSettings.Misc.ServerTimeDifference)
                {
                    AppSettings.Misc.ServerTimeDifference = differenceInt;
                }
            }
        }
        catch (Exception ex)
        { }
    }

    public static void CheckOfferWallPassword()
    {
        if (AppSettings.Offerwalls.UniversalHandlerPassword == "NOTSET")
        {
            string hash1 = MemberAuthenticationService.ComputeHash(HttpContext.Current.User.Identity.Name);
            string hash2 = MemberAuthenticationService.ComputeHash(DateTime.Now.ToString());
            hash1 = hash1.Remove(30);
            hash2 = hash2.Remove(30);
            string hash3 = hash1 + hash2;
            hash3 = hash3.Replace("+", "");
            hash3 = hash3.Replace("=", "");
            hash3 = hash3.Replace("?", "");
            hash3 = hash3.Replace("&", "");
            hash3 = hash3.Replace("/", "");

            AppSettings.Offerwalls.UniversalHandlerPassword = hash3;
        }
    }

    private static void UpdateDomain(string oldDomain, string newDomain)
    {
        using (var bridge = ParserPool.Acquire(Database.Service))
        {
            var result = bridge.Instance.ExecuteRawCommandScalar(String.Format("UPDATE [AuthenticationPairs] SET WebsiteDomain = '{0}' WHERE WebsiteDomain = '{1}'", 
                newDomain, oldDomain));
        }
    }
}
