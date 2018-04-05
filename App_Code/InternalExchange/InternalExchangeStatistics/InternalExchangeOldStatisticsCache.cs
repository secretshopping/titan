using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class InternalExchangeOldStatisticsCache : CacheBase
{
    protected override string Name{ get { return "InternalExchangeOldStatistics"; } }

    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(15); } }

    protected override CacheItemPriority Priority { get { return CacheItemPriority.Normal; } }

    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        return TableHelper.GetListFromRawQuery<InternalExchangeStatistics>(InternalExchangeStatistics.SelectCommandForOld)
            .DefaultIfEmpty(new InternalExchangeStatistics()).SingleOrDefault();
    }
}