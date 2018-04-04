using Prem.PTC;
using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;

public class UseTitanIPCache : CacheBase
{
    protected override string Name { get { return "UseTitanIPCache"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(60); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.BelowNormal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }
    protected override object GetDataFromSource()
    {
        var list = new List<string>();
        var addresses = new IPAddress[0];

        try
        {
            const string host = "admin.usetitan.com";
            addresses = Dns.GetHostAddresses(host);
        }
        catch (Exception e){}

        list.Add(addresses.Length == 0 ? "64.235.37.192" : addresses[0].ToString());

        return list;
    }
}