using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class AdvertBannersCache : CacheBase
{
    protected override string Name { get { return "AdvertBannersCache"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(3); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.Normal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        return BannerDimensionsDictionary.Initialize();
    }
}