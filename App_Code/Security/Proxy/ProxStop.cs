using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Text;

public class ProxStop
{
    private static string ApiKey { get { return AppSettings.Proxy.ProxStopApiKey; } }
    private static string IP_ApiUrl = "https://api.proxstop.com/ip.xml";
    private static string Balance_ApiUrl = "https://api.proxstop.com/balance.xml";
    private static string SMS_ApiUrl = "https://api.proxstop.com/telephonesms.xml";

    public static bool IsIPOk(string ip)
    {
        try
        {
            using (var client = new MyWebClient())
            {
                string results = client.DownloadString(IP_ApiUrl + "?key=" + ApiKey + "&ip=" + ip + "&ref=" + ip);
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(results);

                if (doc.SelectSingleNode("//failed/error_code") != null)
                {
                    // There is an lookup error
                    //ErrorLogger.Log("ProxStop IP Lookup error (" + ip + "): " + doc.SelectSingleNode("//failed/error_code/text()").Value + ": " + doc.SelectSingleNode("//failed/error_msg/text()").Value + "\"");
                }
                else if (doc.SelectSingleNode("//response/score") == null)
                {
                    // Service unavailable
                    throw new MsgException("The ProxStop service seems to be temporarily unavailable");
                }
                else if (Convert.ToInt32(doc.SelectSingleNode("//response/score/text()").Value) > 1)
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

    public static string SendSMSWithPIN(string countryPhoneCode, string telephoneNumber)
    {
        //Lets generate the PIN
        var rand = new Random();
        int PIN = rand.Next(101, 99998);

        try
        {
            using (var client = new MyWebClient())
            {
                string results = client.DownloadString(SMS_ApiUrl + "?key=" + ApiKey + "&country=" + countryPhoneCode + "&phone=" + telephoneNumber + "&code=" + PIN);
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(results);

                ErrorLogger.Log("Requested the following PIN: " + PIN);


                if (doc.SelectSingleNode("//failed/error_code") != null)
                {
                    // There is an lookup error
                    //ErrorLogger.Log("ProxStop SMS Lookup error: " + doc.SelectSingleNode("//failed/error_code/text()").Value + ": " + doc.SelectSingleNode("//failed/error_msg/text()").Value + "\"");

                    if (doc.SelectSingleNode("//failed/error_code/text()").Value == "NO_POINTS")
                    {
                        //Turn off SMS verification
                        //Cause we don't have any points left
                        AppSettings.Proxy.SMSType = ProxySMSType.None;
                        AppSettings.Proxy.Save();
                    }
                }
                else if (doc.SelectSingleNode("//response/ref") == null)
                {
                    // Service unavailable
                    throw new MsgException("The ProxStop service seems to be temporarily unavailable");
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


        return PIN.ToString();
    }

    public static int GetPointsLeft()
    {
        int balance = -1;
        try
        {
            using (var client = new MyWebClient())
            {
                //Try get balance info
                string results = client.DownloadString(Balance_ApiUrl + "?key=" + ApiKey);
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(results);
                if (doc.SelectSingleNode("//failed/error_code") != null)
                {
                    // There is an lookup error
                    throw new MsgException(doc.SelectSingleNode("//failed/error_code/text()").Value + ": " + doc.SelectSingleNode("//failed/error_msg/text()").Value + "\"");
                }
                else if (doc.SelectSingleNode("//response/balance") == null)
                {
                    // Service unavailable
                    throw new MsgException("The ProxStop service seems to be temporarily unavailable");
                }
                balance = Convert.ToInt32(doc.SelectSingleNode("//response/balance").InnerText);
                return balance;
            }
        }
        catch (MsgException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            //ErrorLogger.Log(ex);
            throw ex;
        }
    }
}