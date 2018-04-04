using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Titan;

public class CommissionsCache : CacheBase
{
    protected override string Name { get { return "Commission"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromSeconds(15); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.BelowNormal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }
    protected override bool CachedInAdminPanel { get { return true; } }

    protected override object GetDataFromSource()
    {
        return TableHelper.GetListFromRawQuery<Commission>
                (string.Format("SELECT * FROM Commissions WHERE RefLevel <= {0}", AppSettings.Referrals.ReferralEarningsUpToTier));
    }
}