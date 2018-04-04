using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Net;
using System.Net.Sockets;

public class GeoIPReader
{
    private static readonly string DatabasePathIPv4 = "~/Plugins/GeoIP/GeoIP.dat";
    private static readonly string DatabasePathIPv6 = "~/Plugins/GeoIP/GeoIPv6.dat";

    private FileStream fileInput;
    private bool _close;
    private static long COUNTRY_BEGIN = 16776960;

    public GeoIPReader()
    {
    }

    /// <summary>
    /// Reads Country Code from GeoIP database. If result is not available, "-" is returned
    /// </summary>
    /// <param name="ip"></param>
    public string LookupCode(string ip)
    {
        string code = "-"; //Default country code
        IPAddress addr;

        try
        {
            addr = IPAddress.Parse(ip);

            fileInput = new FileStream(HttpContext.Current.Server.MapPath(GetDatabasePath(addr)), FileMode.Open, FileAccess.Read);

            code = lookupCountryCode(addr);
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            code = "-";
        }
        finally
        {
            if (fileInput != null)
                fileInput.Close();
        }

        return code;
    }

    private long addrToNum(IPAddress addr)
    {
        long ipnum = 0;
        byte[] b = addr.GetAddressBytes();
        for (int i = 0; i < 4; ++i)
        {
            long y = b[i];
            if (y < 0)
            {
                y += 256;
            }
            ipnum += y << ((3 - i) * 8);
        }
        //HttpContext.Current.Response.Write(ipnum);
        return ipnum;
    }

    private string GetDatabasePath(IPAddress address)
    {
        if (address.AddressFamily == AddressFamily.InterNetworkV6)
            return DatabasePathIPv6;

        return DatabasePathIPv4;
    }

    private string lookupCountryCode(IPAddress addr)
    {
        return (CountryManager.countryCode[(int)seekCountry(0, addrToNum(addr), 31)]);
    }

    private string lookupCountryName(string str)
    {
        IPAddress addr;
        try
        {
            addr = IPAddress.Parse(str);
        }
        catch (FormatException e)
        {
            return "Unknown";
        }
        return lookupCountryName(addr);
    }

    private string lookupCountryName(IPAddress addr)
    {
        return (CountryManager.countryName[(int)seekCountry(0, addrToNum(addr), 31)]);
    }

    private long seekCountry(long offset, long ipnum, int depth)
    {
        byte[] buf = new byte[6];
        long[] x = new long[2];
        if (depth < 0)
        {
            return 0; // N/A
        }
        try
        {
            fileInput.Seek(6 * offset, 0);
            fileInput.Read(buf, 0, 6);
        }
        catch (IOException e)
        {
            throw new IOException("IO Exception");
        }
        for (int i = 0; i < 2; i++)
        {
            x[i] = 0;
            for (int j = 0; j < 3; j++)
            {
                int y = buf[i * 3 + j];
                if (y < 0)
                {
                    y += 256;
                }
                x[i] += (y << (j * 8));
            }
        }

        if ((ipnum & (1 << depth)) > 0)
        {
            if (x[1] >= COUNTRY_BEGIN)
            {
                return x[1] - COUNTRY_BEGIN;
            }
            return seekCountry(x[1], ipnum, depth - 1);
        }
        else
        {
            if (x[0] >= COUNTRY_BEGIN)
            {
                return x[0] - COUNTRY_BEGIN;
            }
            return seekCountry(x[0], ipnum, depth - 1);
        }
    }
}



