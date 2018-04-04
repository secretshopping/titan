using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class InternalExchangeStatisticsCache : CacheBase
{
    protected override string Name{ get { return "InternalExchangeStatistics"; } }

    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromSeconds(15); } }

    protected override CacheItemPriority Priority { get { return CacheItemPriority.Normal; } }

    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        return TableHelper.GetListFromRawQuery<InternalExchangeStatistics>(InternalExchangeStatistics.SelectCommand)
            .DefaultIfEmpty(new InternalExchangeStatistics()).SingleOrDefault();
    }
}