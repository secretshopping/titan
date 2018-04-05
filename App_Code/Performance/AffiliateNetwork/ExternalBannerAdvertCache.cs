using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

/// <summary>
/// Summary description for ExternalBannerAdvertCache
/// </summary>
public class ExternalBannerAdvertCache : CacheBase
{
    PublishersWebsite _publishersWebsite;
    PublishersWebsite publishersWebsite
    {
        get { return _publishersWebsite; }
        set
        {
            if (value == null)
                throw new ArgumentNullException("publishersWebsite");
            _publishersWebsite = value;
        }
    }
    int dimensionsId { get; set; }
    public ExternalBannerAdvertCache(PublishersWebsite publishersWebsite, int dimensionsId)
    {
        this.publishersWebsite = publishersWebsite;
        this.dimensionsId = dimensionsId;
    }

    protected override TimeSpan KeepCacheFor
    {
        get
        {
            return TimeSpan.FromMinutes(5);
        }
    }

    protected override string Name
    {
        get
        {
            return "ExternalBannerAdvertCache";
        }
    }

    protected override CacheItemPriority Priority
    {
        get
        {
            return CacheItemPriority.Normal;
        }
    }

    protected override CacheItemRemovedCallback RemovedCallback
    {
        get
        {
            return null;
        }
    }

    protected override object GetDataFromSource()
    {
        var banners = new List<ExternalBannerAdvert>();
        banners = publishersWebsite.GetActiveBanners(dimensionsId);
        return banners;
    }
}