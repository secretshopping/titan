using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;


public class BannerImpressionsCache
{
    private static readonly string BannerName = "BA";
    private static readonly int BannerRefreshEverySeconds = 30;

    private static readonly string AdPackBannerName = "AP";
    private static readonly int AdPackBannerRefreshEverySeconds = 30;

    public static void AddBannerImpression(int bannerId)
    {
        string name = BannerName + bannerId;

        var pair = new KeyValuePair<DateTime, int>(DateTime.Now.AddSeconds(BannerRefreshEverySeconds), 1);

        if (HttpContext.Current.Cache[name] != null)
        {
            var cachedPair = (KeyValuePair<DateTime, int>)HttpContext.Current.Cache[name];
            pair = new KeyValuePair<DateTime, int>(cachedPair.Key, cachedPair.Value + 1);
        }

        HttpContext.Current.Cache.Insert(name, pair, null, pair.Key, Cache.NoSlidingExpiration,
            CacheItemPriority.Normal, new CacheItemRemovedCallback(BannerImpressionCacheRemoved));
    }

    public static void AddAdPackBannerImpression(int adPackBannerId, BannerAdvert.Type type)
    {
        string name = AdPackBannerName + adPackBannerId;

        var pair = new KeyValuePair<DateTime, int>(DateTime.Now.AddSeconds(AdPackBannerRefreshEverySeconds), 1);

        if (HttpContext.Current.Cache[name] != null)
        {
            var cachedPair = (KeyValuePair<DateTime, int>)HttpContext.Current.Cache[name];
            pair = new KeyValuePair<DateTime, int>(cachedPair.Key, cachedPair.Value + 1);
        }

        CacheItemRemovedCallback callback;

        if (type == BannerAdvert.Type.Constant)
            callback = new CacheItemRemovedCallback(ConstantAdPackBannerImpressionCacheRemoved);
        else
            callback = new CacheItemRemovedCallback(NormalAdPackBannerImpressionCacheRemoved);

        HttpContext.Current.Cache.Insert(name, pair, null, pair.Key, Cache.NoSlidingExpiration,
            CacheItemPriority.Normal, callback);
    }

    #region Callbacks

    private static void ConstantAdPackBannerImpressionCacheRemoved(string key, object value, CacheItemRemovedReason removedReason)
    {
        if (removedReason == CacheItemRemovedReason.Expired)
        {
            string adId = key.Substring(AdPackBannerName.Length);
            var pair = (KeyValuePair<DateTime, int>)value;
            TableHelper.ExecuteRawCommandNonQuery("EXEC AddAdPackConstantBannerImpressions " + adId + ", " + pair.Value);
        }
    }

    private static void NormalAdPackBannerImpressionCacheRemoved(string key, object value, CacheItemRemovedReason removedReason)
    {
        if (removedReason == CacheItemRemovedReason.Expired)
        {
            string adId = key.Substring(AdPackBannerName.Length);
            var pair = (KeyValuePair<DateTime, int>)value;
            TableHelper.ExecuteRawCommandNonQuery("EXEC AddAdPackNormalBannerImpressions " + adId + ", " + pair.Value);
        }
    }

    private static void BannerImpressionCacheRemoved(string key, object value, CacheItemRemovedReason removedReason)
    {
        if (removedReason == CacheItemRemovedReason.Expired)
        {
            string adId = key.Substring(BannerName.Length);
            var pair = (KeyValuePair<DateTime, int>)value;
            TableHelper.ExecuteRawCommandNonQuery("EXEC AddBannerImpressions " + adId + ", " + pair.Value);
        }
    }

    #endregion
}