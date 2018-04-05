using System;
using System.Web.Caching;

namespace Titan.InvestmentPlatform
{
    public class InvestmentPlatformPlansCache : CacheBase
    {
        protected override string Name { get { return "InvestmentPlatformPlansCache"; } }
        protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(5); } }
        protected override CacheItemPriority Priority { get { return CacheItemPriority.Normal; } }
        protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

        protected override object GetDataFromSource()
        {
            var query = string.Format(@"SELECT * FROM {0} ORDER BY [Number]", InvestmentPlatformPlan.TableName);
            return TableHelper.GetListFromRawQuery<InvestmentPlatformPlan>(query);
        }
    }
}