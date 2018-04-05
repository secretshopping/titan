using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;

public class ProxyManager
{
    [Obsolete]
    public static bool IsIPNotAProxy(string ip)
    {
        return IsProxy(ip) == false;
    }

    /// <summary>
    /// Checks if given IP is a Proxy or not
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public static bool IsProxy(string ip)
    {

        if (AppSettings.Proxy.ProxyProviderType == ProxyProviderType.BlockedCom && !string.IsNullOrEmpty(AppSettings.Proxy.BlockScriptApiKey))
            return !BlockScript.IsIPOk(ip);

        if (AppSettings.Proxy.ProxyProviderType == ProxyProviderType.ProxStop && !string.IsNullOrEmpty(AppSettings.Proxy.ProxStopApiKey))
            return !ProxStop.IsIPOk(ip);

        if (AppSettings.Proxy.ProxyProviderType == ProxyProviderType.IpQualityScore && !string.IsNullOrEmpty(AppSettings.Proxy.IpQualityScoreKey))
            return !IpQualityScore.IsIPOk(ip);
        
        //If no tools are available, we can't check it
        return false;
    }
}