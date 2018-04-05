using System;
using System.Web.Caching;

namespace Titan.InvestmentPlatform
{
    public class InvestmentUsersPlansCache : CacheBase
    {
        protected override string Name { get { return "InvestmentUsersPlans"; } }
        protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(1); } }
        protected override CacheItemPriority Priority { get { return CacheItemPriority.Normal; } }
        protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

        protected override object GetDataFromSource()
        {
            var query = string.Format(@"SELECT * FROM {0}", InvestmentUsersPlans.TableName);
            return TableHelper.GetListFromRawQuery<InvestmentUsersPlans>(query);
        }
    }
}