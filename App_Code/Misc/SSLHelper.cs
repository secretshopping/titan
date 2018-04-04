using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

public enum SSLType
{
    Null = 0,
    No = 1,
    Yes = 3
}

public class SSLHelper
{
    public static bool IsWebsiteHasSSLCertificateInstalled(string domainName)
    {
        HttpWebResponse response = null;
        try
        {
            WebRequestUtils.SetUpSecurityProtocols();

            var link = "https://" + domainName + "/";
            var request = (HttpWebRequest)WebRequest.Create(link);

            request.Method = "GET";
            response = (HttpWebResponse)request.GetResponse();

            if (response != null)
            {
                response.Close();
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;            
        }
    }

    public static bool IsServerSideHTTPSRedirectionEnabled(string domainName)
    {
        HttpWebResponse response = null;
        try
        {
            WebRequestUtils.SetUpSecurityProtocols();

            var link = "http://" + domainName + "/Handlers/Utils/ServerTime.ashx"; //This page will not be redirected by TITAN
            var request = (HttpWebRequest)WebRequest.Create(link);

            request.Method = "GET";
            response = (HttpWebResponse)request.GetResponse();

            if (response != null)
            {
                response.Close();
            }

            return response.ResponseUri.Scheme == "https";
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}