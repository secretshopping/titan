using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Prem.PTC;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

public class FacebookManager
{
    //Old method
    private static string GetGlobalAccessToken(WebClient client)
    {
        try
        {
            string response = client.DownloadJsonString(
                String.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&client_secret={1}&grant_type=client_credentials",
                AppSettings.Facebook.ApplicationId, AppSettings.Facebook.AppSecret));

            return response;
        }
        catch (Exception ex)
        {
            return String.Empty;
        }
    }

    public static bool IsFanpageURLValid(string fanpageUrl)
    {
        bool isValid = false;
        long FanpageID;

        try
        {
            using (WebClient client = new WebClient())
            {
                string queryResult = client.DownloadJsonString
                   (String.Format("https://graph.facebook.com/?ids={0}&access_token={1}|{2}", HttpUtility.UrlEncode(fanpageUrl), AppSettings.Facebook.ApplicationId, AppSettings.Facebook.AppSecret));

                JObject obj = JObject.Parse(queryResult);

                isValid = (!String.IsNullOrEmpty(obj.First.First["name"].ToString()) && Int64.TryParse(obj.First.First["id"].ToString(), out FanpageID));

                client.Dispose();
            }

        }
        catch (Exception ex)
        {
        }

        return isValid;
    }

    public static bool IsIntegratedCorretly()
    {
        return FacebookManager.IsFanpageURLValid("https://facebook.com/usetitan"); 
    }
}