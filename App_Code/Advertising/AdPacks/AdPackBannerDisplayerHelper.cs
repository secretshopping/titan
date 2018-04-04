using Prem.PTC;
using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

public class AdPackBannerDisplayerHelper 
{
    private readonly string CacheName = "AdPackBannersAppCache";
    private readonly string CacheTimerName = "AdPackBannersAppTimerCache";
    private BannerAdvert.Type BannerType;

    public AdPackBannerDisplayerHelper(BannerAdvert.Type type) 
    {
        BannerType = type;
    }

    public List<AdPacksAdvert> Banners
    {
        get
        {
            List<AdPacksAdvert> result = GetBanners();
            return result;
        }
    }

    public Image GetBannerImage(AdPacksAdvert advert)
    {
        Image BannerImage = new Image();

        if (BannerType == BannerAdvert.Type.Constant)
        {
            BannerImage.ImageUrl = advert.ConstantImagePath;
            BannerImage.Width = AppSettings.RevShare.AdPack.PackConstantBannerWidth;
            BannerImage.Height = AppSettings.RevShare.AdPack.PackConstantBannerHeight;

        }
        else
        {
            BannerImage.ImageUrl = advert.NormalImagePath;
            BannerImage.Width = AppSettings.RevShare.AdPack.PackNormalBannerWidth;
            BannerImage.Height = AppSettings.RevShare.AdPack.PackNormalBannerHeight;
        }

        return BannerImage;
    }

    private List<AdPacksAdvert> GetBanners()
    {

        if (BannerType == BannerAdvert.Type.Normal)
        {
            var cache = new AdPackNormalBannersCache();
            return (List<AdPacksAdvert>)cache.Get();
        }
        else {
            var cache = new AdPackConstantBannersCache();
            return (List<AdPacksAdvert>)cache.Get();
        }
    }

    public HyperLink GetBannerHyperLink()
    {
        HyperLink _workingBanner = new HyperLink();

        //Border
        _workingBanner.BorderColor = System.Drawing.Color.FromArgb(220, 220, 220); //Grey
        _workingBanner.BorderStyle = BorderStyle.Solid;
        _workingBanner.BorderWidth = Unit.Pixel(1);
        _workingBanner.Target = "_blank";

        return _workingBanner;
    }

    public Image GetBannerImage(BannerAdvert advert)
    {
        Image banner = new Image();

        banner.ImageUrl = advert.ImagePath;
        banner.CssClass = "img-responsive";

        return banner;
    }
}