using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Prem.PTC.Memberships;

public class MembershipsCache : CacheBase
{
    protected override string Name { get { return "Memberships"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(5); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.Normal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        return TableHelper.GetListFromRawQuery<Membership>(string.Format("SELECT * FROM Memberships WHERE Status = {0} ORDER BY MinPointsToHaveThisLevel ASC", (int)MembershipStatus.Active));
    }
}