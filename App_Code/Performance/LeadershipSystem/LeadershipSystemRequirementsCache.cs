using System;
using System.Web.Caching;
using Titan.Leadership;

public class LeadershipSystemRequirementsCache : CacheBase
{
    protected override string Name { get { return "LeadershipSystemRequirements"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(10); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.Normal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }
    protected override object GetDataFromSource()
    {
        var query = LeadershipRankRequirements.RequirementsSortedListQuery;
        return TableHelper.GetListFromRawQuery<LeadershipRankRequirements>(query);
    }
}