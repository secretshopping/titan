using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Text;
using System.Web.Caching;

public class DirectReferralPackMembersCache : CacheBase
{
    protected override string Name { get { return "DirectReferralPackMembers"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(3); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.BelowNormal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        var sb = new StringBuilder();
        sb.Append(string.Format(@"SELECT COUNT(*) FROM Users WHERE AccountStatusInt = {0} AND ReferrerId = -1 AND UserId != 1005",
            (int)MemberStatus.Active));

        if (AppSettings.DirectReferrals.DefaultReferrerEnabled)
            sb.Append(string.Format(" AND UserId != {0}", AppSettings.DirectReferrals.DefaultReferrerId));

        return TableHelper.SelectScalar(sb.ToString());       
    }
}