using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class AdPackConstantBannersCache : CacheBase
{
    protected override string Name { get { return "AdPackConstantBanners"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(3); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.Normal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        List<AdPacksAdvert> result = null;
        string command = "SELECT DISTINCT apa.* FROM AdPacksAdverts apa INNER JOIN AdPacks ap ON apa.Id = ap.AdPacksAdvertId WHERE ap.TotalConstantBannerImpressions < ap.ConstantBannerImpressionsBought AND apa.ConstantImagePath IS NOT NULL";

        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            var highestSql = bridge.Instance.ExecuteRawCommandToDataTable(command);
            result = TableHelper.GetListFromDataTable<AdPacksAdvert>(highestSql, 100, false);
        }

        return result;
    }
}