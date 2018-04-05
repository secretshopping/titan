using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class PtcAdvertCache : CacheBase
{
    protected override string Name { get { return "GetAllActiveAds"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromSeconds(15); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.Normal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        if (!AppSettings.PtcAdverts.StarredAdsEnabled)
            return TableHelper.GetListFromRawQuery<PtcAdvert>("SELECT * FROM PtcAdverts WHERE Status = " + (int)AdvertStatus.Active);

        return TableHelper.GetListFromRawQuery<PtcAdvert>("SELECT * FROM PtcAdverts WHERE Status = " + (int)AdvertStatus.Active + " ORDER BY IsStarredAd DESC");
    }
}