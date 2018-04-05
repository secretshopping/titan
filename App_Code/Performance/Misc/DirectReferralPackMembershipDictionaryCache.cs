using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;

public class DirectReferralPackMembershipDictionaryCache : CacheBase
{
    protected override string Name { get { return "DirectReferralPackMembershipDictionary"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(3); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.BelowNormal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        var membershipsList = Membership.GetAll();
        var membershipsDictionary = new Dictionary<int, int>();

        foreach(var membership in membershipsList.FindAll(x => x.Status == MembershipStatus.Active))
        {
            var sb = new StringBuilder();

            sb.Append(string.Format(@"SELECT COUNT(*) FROM Users WHERE AccountStatusInt = {0} AND ReferrerId = -1 AND UserId != 1005 AND UpgradeId = {1}",
                (int)MemberStatus.Active, membership.Id));

            if (AppSettings.DirectReferrals.DefaultReferrerEnabled)
                sb.Append(string.Format(" AND UserId != {0}", AppSettings.DirectReferrals.DefaultReferrerId));

            var count = (int)TableHelper.SelectScalar(sb.ToString());
            membershipsDictionary.Add(membership.Id, count);
        }

        return membershipsDictionary;
    }
}