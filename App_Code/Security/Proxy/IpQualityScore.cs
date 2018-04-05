using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;

public class IpQualityScore
{
    private static string ApiKey { get { return AppSettings.Proxy.IpQualityScoreKey; } }
    private static string ApiUrl = @"https://ipqualityscore.com/api/xml/ip/";

    public static bool IsIPOk(string ip)
    {
        try
        {
            using (var client = new MyWebClient())
            {
                string results = client.DownloadString(ApiUrl + ApiKey + "/" + ip);
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(results);

                if (doc.SelectSingleNode("//result/success/text()").Value != "true")
                {
                    // There is an lookup error
                    //ErrorLogger.Log("ProxStop IP Lookup error (" + ip + "): " + doc.SelectSingleNode("//failed/error_code/text()").Value + ": " + doc.SelectSingleNode("//failed/error_msg/text()").Value + "\"");
                }
                else if (doc.SelectSingleNode("//result/proxy") == null)
                {
                    // Service unavailable
                    throw new MsgException("The IpQualityScore service seems to be temporarily unavailable");
                }
                else if (doc.SelectSingleNode("//result/mobile/text()").Value == "true" && float.Parse(doc.SelectSingleNode("//result/fraud_score/text()").Value) >= 75.0f)
                {
                    return false;
                }
                else if (doc.SelectSingleNode("//result/proxy/text()").Value == "true")
                {
                    //The IP is a known proxy
                    return false;
                }
                else
                {
                    //The IP is safe
                    return true;
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
}