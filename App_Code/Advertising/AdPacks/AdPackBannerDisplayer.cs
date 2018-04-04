using Prem.PTC;
using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

public static class AdPackBannerDisplayer
{
    private static Random random = new Random();

    public static HyperLink GetBanner(BannerAdvert.Type type, int indexOnPage)
    {
        AdPackBannerDisplayerHelper Helper = new AdPackBannerDisplayerHelper(type);
        List<AdPacksAdvert> allBanners = Helper.Banners;

        //Randomize the banner
        int bannerIndex = random.Next(0, allBanners.Count);

        HyperLink Banner = Helper.GetBannerHyperLink();

        //Do we have enough banners to display?
        if (allBanners.Count > 0 && allBanners.Count > indexOnPage)
        {
            AdPacksAdvert chosenBanner = allBanners[bannerIndex];

            //Adding the impression
            AddBannerImpression(chosenBanner.Id, type);

            //Finishing
            Banner.NavigateUrl = AppSettings.Site.Url + "link.aspx?rid=" + (new Encryption()).EncryptBannerId(chosenBanner.Id);
            Banner.Controls.Clear();
            Banner.Controls.Add(Helper.GetBannerImage(chosenBanner));
        }

        return Banner;
    }

    private static void AddBannerImpression(int adPacksAdvertId, BannerAdvert.Type type)
    {
        BannerImpressionsCache.AddAdPackBannerImpression(adPacksAdvertId, type);
    }
}