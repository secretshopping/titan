using System;
using System.Web.Caching;
using Titan.Leadership;

public class LeadershipSystemRanksCache : CacheBase
{
    protected override string Name { get { return "LeadershipSystemRanks"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(10); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.Normal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }
    protected override object GetDataFromSource()
    {
        var query = string.Format(@"SELECT * FROM {0} WHERE {2} <> {3} ORDER BY {1} ASC",
            LeadershipRank.TableName, LeadershipRank.Columns.Rank, LeadershipRank.Columns.RankStatus, (int)UniversalStatus.Deleted);
        return TableHelper.GetListFromRawQuery<LeadershipRank>(query);
    }
}