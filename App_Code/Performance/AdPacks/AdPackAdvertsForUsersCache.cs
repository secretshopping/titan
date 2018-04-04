using Prem.PTC;
using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class AdPackAdvertsForUsersCache : CacheBase
{
    protected override string Name { get { return "AdPacksAdvertsForUsers"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(3); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.AboveNormal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }
    protected override object GetDataFromSource()
    {
        DataTable usersAds;
        var cache = new AdminIdCache();
        string query;
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            if (TitanFeatures.IsClickmyad)
                query = string.Format(@"SELECT DISTINCT Id, Status, TargetUrl, ConstantImagePath, NormalImagePath, 
                                                                            CreatorUserId, Title, Description, 
																			StartPageDate, Priority
                                                                            FROM AdPacksAdverts WHERE Status = {0};", (int)AdvertStatus.Active);
            else
                query = string.Format(@"SELECT DISTINCT apa.Id, apa.Status, apa.TargetUrl, apa.ConstantImagePath, apa.NormalImagePath, 
                                                                            apa.CreatorUserId, apa.Title, apa.Description, 
																			apa.StartPageDate, apa.Priority
                                                                            FROM AdPacksAdverts apa 
                                                                            JOIN AdPacks ap ON apa.Id = ap.AdPacksAdvertId 
                                                                            WHERE ap.TotalClicks < ap.ClicksBought 
                                                                            AND (apa.CreatorUserId != {0} OR (apa.CreatorUserId = {0} AND apa.Priority != 0))
                                                                            AND apa.Status = {1}               
                                                                            ;", (int)cache.Get(), (int)AdvertStatus.Active);

            usersAds = bridge.Instance.ExecuteRawCommandToDataTable(query);
                      
        }

        return usersAds;
    }
}