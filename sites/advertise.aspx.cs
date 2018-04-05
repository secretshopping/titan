using System;
using Prem.PTC;
using Prem.PTC.Payments;

public partial class sites_advertise : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdvertisePageLoad();
    }

    protected void AdvertisePageLoad()
    {
        if (TitanFeatures.IsTrafficThunder)
            Response.Redirect(AppSettings.Site.Url);

        if (!AppSettings.TitanFeatures.AdvertAdsEnabled)
            ptcLink.Visible = false;

        if (!AppSettings.TitanFeatures.AdvertBannersEnabled || AppSettings.BannerAdverts.AdvertisingPolicy == BannerPolicy.BannerBidding)
            bannerLink.Visible = false;

        if (!AppSettings.TitanFeatures.AdvertFacebookEnabled)
            facebookLink.Visible = false;

        if (!AppSettings.TitanFeatures.AdvertTrafficGridEnabled)
            trafficGridLink.Visible = false;

        if (!AppSettings.TitanFeatures.AdvertMarketplaceEnabled || !AppSettings.Marketplace.MarketplaceAllowPurchaseFromPaymentProcessors)
            marketplaceLink.Visible = false;

        if (!AppSettings.InvestmentPlatform.InvestmentPlatformEnabled || !AppSettings.TitanFeatures.InvestmentPlatformCalculatorEnabled)
            investmentPlatformCalculatorLink.Visible = false;

        div02.Visible = (ptcLink.Visible || bannerLink.Visible || facebookLink.Visible || trafficGridLink.Visible || marketplaceLink.Visible || investmentPlatformCalculatorLink.Visible) && PaymentAccountDetails.AreIncomingPaymentProcessorsAvailable();
    }
}