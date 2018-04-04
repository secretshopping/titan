using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;


public class BlockScript
{
    private static string ApiKey { get { return AppSettings.Proxy.BlockScriptApiKey; } }
    private static string ApiUrl { get { return AppSettings.Proxy.BlockScriptUrl; } }

    public static bool IsIPOk(string ip)
    {
        try
        {
            using (var client = new MyWebClient())
            {
                string results = client.DownloadString(ApiUrl + "?blockscript=api&api_key=" + ApiKey + "&action=test_ipv4&ip=" + ip);
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(results);

                if (doc.SelectSingleNode("//response/status") != null && doc.SelectSingleNode("//response/status/text()").Value == "SUCCESS")
                {
                    //We got successful response
                    //Now check the IP
                    if (doc.SelectSingleNode("//response/ip/blocked/text()").Value == "YES")
                    {
                        //IP blocked
                        return false;
                    }
                    else if (doc.SelectSingleNode("//response/ip/blocked/text()").Value == "NO")
                    {
                        //IP OK
                        return true;
                    }
                }
                else
                {
                    throw new MsgException("The BlockScript service seems to be temporarily unavailable");
                }
            }
        }
        catch (MsgException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }

        return true;
    }

    /// <summary>
    /// BlockScript limits the number of queries that can be sent to the API in a 24 hour period. This API displays the 
    /// total number of queries allowed by your license key and the total number of queries remaining 
    /// in the current 24 hour period. A query to lookup an IP takes the form of:
    /// </summary>
    /// <returns></returns>
    public static string GetQueriesRemaining()
    {
        try
        {
            using (var client = new MyWebClient())
            {
                string results = client.DownloadString(ApiUrl + "?blockscript=api&api_key=" + ApiKey + "&action=queries");
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(results);

                string parsedResult = doc.SelectSingleNode("//response/msg/text()").Value;

                return parsedResult;
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            return "n/a";
        }
    }
}