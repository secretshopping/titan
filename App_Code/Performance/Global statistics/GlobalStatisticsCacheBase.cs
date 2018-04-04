using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

public abstract class GlobalStatisticsCacheBase : CacheBase
{
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(3); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.High; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }
}