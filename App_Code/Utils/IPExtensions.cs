using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

public static class IPExtensions
{
    public static int ToInteger(this IPAddress IP)
    {
        int result = 0;

        byte[] bytes = IP.GetAddressBytes();
        result = (int)(bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3]);

        return result;
    }

    //returns 0 if equal
    //returns 1 if ip1 > ip2
    //returns -1 if ip1 < ip2
    public static int Compare(this IPAddress IP1, IPAddress IP2)
    {
        int ip1 = IP1.ToInteger();
        int ip2 = IP2.ToInteger();
        return (((ip1 - ip2) >> 0x1F) | (int)((uint)(-(ip1 - ip2)) >> 0x1F));
    }

    /// <summary>
    /// Decides wheather the specified IP is in specified range (X.X.X.X-X.X.X.X)
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static bool IsInRange(string ip, string range)
    {
        try
        {
            IPAddress InputIP = IPAddress.Parse(ip);
            string RangeLeft = range.Substring(0, range.IndexOf("-"));
            string RangeRight = range.Substring(range.IndexOf("-") + 1);

            IPAddress LeftIP = IPAddress.Parse(RangeLeft);
            IPAddress RightIP = IPAddress.Parse(RangeRight);

            if (IPExtensions.Compare(InputIP, LeftIP) >= 0
                && IPExtensions.Compare(InputIP, RightIP) <= 0)
                return true;
        }
        catch (Exception ex)
        { }
        return false;
    }
}