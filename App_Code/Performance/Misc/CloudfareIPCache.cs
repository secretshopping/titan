using LukeSkywalker.IPNetwork;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using Titan;

public class CloudfareIPCache : CacheBase
{
    protected override string Name { get { return "CloudfareIPCache"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromHours(1); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.High; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        List<string> IPresult = new List<string>();
        List<IPNetwork> result = new List<IPNetwork>();

        using (WebClient client = new MyWebClient())
        {
            try
            {
                string data = client.DownloadString("https://www.cloudflare.com/ips-v4");
                string line = null;
                StringReader strReader = new StringReader(data);

                while((line = strReader.ReadLine()) != null)
                {
                    IPresult.Add(line);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }

        foreach (var ip in IPresult)
            result.Add(IPNetwork.Parse(ip));

        return result;
    }
}