using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Resources;
using System;

public static class SurfAdsManager
{
    public static void BuySurfAds(int adPacksAdvertId, Member user, SurfAdsPack surfAdsPack, PurchaseBalances targetBalance)
    {
        var availablePacks = SurfAdsPack.GetAllActivePacks();

        if (surfAdsPack.Status != SurfAdsPackStatus.Active)
            throw new MsgException("Selected Surf Ads Pack is unavailable");

        //BUY ADPACKS
        var totalPrice = surfAdsPack.Price;

        PurchaseOption.ChargeBalance(user, totalPrice, PurchaseOption.Features.SurfAd.ToString(), targetBalance, string.Format("{0} purchase", U5004.SURFADS));

        AdPacksAdvert ad = new AdPacksAdvert(adPacksAdvertId);

        AdPack pack = new AdPack();
        pack.MoneyReturned = new Money(0);
        pack.AdPacksAdvertId = adPacksAdvertId;
        pack.TotalConstantBannerImpressions = pack.ConstantBannerImpressionsBought = 0;
        pack.NormalBannerImpressionsBought = pack.TotalNormalBannerImpressions = 0;
        pack.ClicksBought = surfAdsPack.Clicks;
        pack.PurchaseDate = DateTime.Now;
        pack.MoneyToReturn = pack.MoneyReturned = new Money(0);
        pack.UserCustomGroupId = -1;
        pack.UserId = user.Id;
        pack.DistributionPriority = new Decimal(0);
        pack.AdPackTypeId = -1;
        pack.DisplayTime = surfAdsPack.DisplayTime;
        pack.Save();

        //Pools
        PoolDistributionManager.AddProfit(ProfitSource.SurfAds, totalPrice);
    }

}