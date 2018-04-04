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

public class MembershipsAllStatusCache : CacheBase
{
    protected override string Name { get { return "MembershipsAllStatusCache"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(5); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.Normal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }
    protected override bool CachedInAdminPanel { get { return true; } }

    protected override object GetDataFromSource()
    {
        return TableHelper.GetListFromRawQuery<Membership>("SELECT * FROM Memberships ORDER BY MinPointsToHaveThisLevel ASC");
    }
}