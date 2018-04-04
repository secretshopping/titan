using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;

public partial class user_advert_bannersoptions : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (AppSettings.TitanFeatures.AdvertisersRoleEnabled && AppSettings.TitanFeatures.AdvertBannersEnabled && Member.CurrentInCache.IsAdvertiser && 
            AppSettings.TitanFeatures.PublishersRoleEnabled && AppSettings.TitanFeatures.PublishBannersEnabled && Member.CurrentInCache.IsPublisher)
        {
            SubLiteral.Text = U6002.ADVERTISEON;
            CurrentWebsiteButton.Text = AppSettings.Site.Name;
            OuterWebsiteButton.Text = U6002.EXTERNALWEBSITES;

            if (AppSettings.BannerAdverts.AdvertisingPolicy == BannerPolicy.SellingPackages)
                CurrentWebsiteButton.PostBackUrl = "/user/advert/banners.aspx";
            else if (AppSettings.BannerAdverts.AdvertisingPolicy == BannerPolicy.BannerBidding)
                CurrentWebsiteButton.PostBackUrl = "/user/advert/bannersb.aspx";

            OuterWebsiteButton.PostBackUrl = "/user/advert/bannerse.aspx";
        }
        else
        {
            Response.Redirect("~/user/advert/banners.aspx");
        }
    }
}