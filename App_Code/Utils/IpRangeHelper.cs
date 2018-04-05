using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan;

/// <summary>
/// Summary description for IpRangeHelper
/// </summary>
public class IpRangeHelper
{
    public static bool isOK(string restictedIps, string RequestIp)
    {
        if (restictedIps.Contains(RequestIp))
            return true;

        if (restictedIps.Contains("-"))
        {
            string[] allIps = restictedIps.Trim().Split(',');
            foreach (var ip in allIps)
            {
                if (ip.Trim().Contains("-") && IPExtensions.IsInRange(RequestIp, ip.Trim()))
                    return true;
            }
        }
        return false;
    }
}