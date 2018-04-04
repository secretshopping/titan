using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExtensionMethods;
using Prem.PTC;
using Prem.PTC.Members;
using MarchewkaOne.Titan.Balances;
using Titan.Publisher.Security;
using Titan;

public class ExternalBannerManager
{
    public PublishersWebsite PublishersWebsite { get; private set; }
    public ExternalBannerAdvert Banner { get; private set; }
    public ExternalBannerManager(string host, int dimensionsId, int publishersWebsiteId)
    {
        PublishersWebsite = PublishersWebsite.GetActiveWebsite(host, publishersWebsiteId);
        Banner = GetBanner(dimensionsId);
    }

    public ExternalBannerAdvert GetBanner(int dimensionsId)
    {
        if (PublishersWebsite == null)
            return null;

        var banners = (List<ExternalBannerAdvert>)new ExternalBannerAdvertCache(PublishersWebsite, dimensionsId).Get();

        if (banners.Count == 0)
            return null;

        var randomIndex = new Random();

        return banners[randomIndex.Next(0, banners.Count)];
    }

    public void CreditForBannerClick(string ip)
    {
        if (Banner != null)
        {
            var tracker = new ActionTrackerCreator<ExternalBannerActionTracker>(PublishersWebsite.Id, ip, Banner.Id).GetOrCreate();
            tracker.HandleAction(HandleSuccessfulClick);
        }
    }

    private void HandleSuccessfulClick()
    {
        var crediter = new ExternalBannerCrediter(Banner.MoneyPerClick, PublishersWebsite.UserId);
        var moneyLeftForPools = crediter.Credit();

        Banner.AddClick();
    }
}