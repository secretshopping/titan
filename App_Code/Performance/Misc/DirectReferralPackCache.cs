using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Referrals;
using System;
using System.Collections.Generic;
using System.Web.Caching;

public class DirectReferralPackCache : CacheBase
{
    protected override string Name { get { return "DirectReferralPack"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(3); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.Low; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        var membershipDictionary = (Dictionary<int, int>)new DirectReferralPackMembershipDictionaryCache().Get();
        var packsList = new List<DirectReferralPack>();
        var membersCount = 0;

        foreach(var membership in membershipDictionary)
        {
            var query = string.Format(@"SELECT * FROM DirectReferralPacks WHERE Status = {0} AND NumberOfRefs <= {1} AND MembershipId = {2}",
                (int)UniversalStatus.Active, membership.Value, membership.Key);
            packsList.AddRange(TableHelper.GetListFromRawQuery<DirectReferralPack>(query));
            membersCount += membership.Value;
        }

        var noMembershipQuery = string.Format(@"SELECT * FROM DirectReferralPacks WHERE Status = {0} AND NumberOfRefs <= {1} AND MembershipId = 0",
                (int)UniversalStatus.Active, membersCount);
        packsList.AddRange(TableHelper.GetListFromRawQuery<DirectReferralPack>(noMembershipQuery));

        return packsList;
    }
}