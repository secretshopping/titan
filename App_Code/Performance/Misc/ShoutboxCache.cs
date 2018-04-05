using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Titan;

public class ShoutboxCache : CacheBase
{
    protected override string Name { get { return "Shoutbox"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromSeconds(45); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.BelowNormal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    public int MessagesInFeed { get; set; }

    protected override object GetDataFromSource()
    {
        return ShoutboxManager.GetLatestRecords(MessagesInFeed);
    }
}