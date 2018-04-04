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
using Titan.ICO;

public class InternalExchangeCache : CacheBase
{
    protected override string Name { get { return "InternalExchangeChart"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(1); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.BelowNormal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        return InternalExchangeChartData.GetChartData();
    }
}