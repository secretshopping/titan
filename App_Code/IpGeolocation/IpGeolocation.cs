using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public static class IpGeolocation
{
    public static IpGeolocationInfo GetInfo(string ip)
    {
        return IpGeolocationFactory.Get<FreeGeoIpProvider>().GetGeoLocationInfo(ip);
    }
}