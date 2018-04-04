using LukeSkywalker.IPNetwork;
using System.Collections.Generic;
using System.Net;
using System.Web;


public class IP
{
    /// <summary>
    /// Gets current visitor IP address
    /// </summary>
    public static string Current
    {
        get
        {
            string UserIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (HttpContext.Current.Request.ServerVariables["HTTP_CF_CONNECTING_IP"] != null && IpIsWithinCloudfareIPRange(UserIP))
                return HttpContext.Current.Request.ServerVariables["HTTP_CF_CONNECTING_IP"];

            return UserIP;
        }
    }

    public static bool IpIsWithinCloudfareIPRange(string ip)
    {
        IPAddress incomingIp = IPAddress.Parse(ip);

        var cache = new CloudfareIPCache();
        var range = (List <IPNetwork>)cache.Get();

        foreach (var network in range)
        {
            if (IPNetwork.Contains(network, incomingIp))
                return true;
        }
        return false;
    }

    public static bool IsIpInRange(string ip, string range)
    {
        IPNetwork IpNetwork = IPNetwork.Parse(range);
        IPAddress incomingIp = IPAddress.Parse(ip);
        return IPNetwork.Contains(IpNetwork, incomingIp);
    }

    public static bool IsLocalhost
    {
        get
        {
            return IP.Current == "::1";
        }
    }
}