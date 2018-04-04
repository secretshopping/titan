using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using Prem.PTC.Advertising;
using Resources;
using Prem.PTC;
using Titan.Shares;

public partial class About : System.Web.UI.Page
{
    //After clicking banner a postback is sent here
    //It calculates the click and redirects to target page
    private const string SESSION_ID = "TIT_watchedbanids";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["id"] != null)
        {
            var te = new Encryption();
            int BannerId = te.DecryptBannerId(Request.Params["id"]);
            BannerAdvert banner = new BannerAdvert(BannerId);

            var list = getWatchedBannersCollection();
            if (!list.Contains(BannerId) && !AppSettings.BannerAdverts.ImpressionsEnabled)
            {
                banner.Click();
                list.Add(banner.Id);
                saveWatchedBannersCollection(list);
            }
            Response.Redirect(banner.TargetUrl);
        }
        else if (Request.QueryString["redirect"] != null)
        {
            string redirect = Request.QueryString["redirect"];
            Response.Redirect("~/" + redirect);
        }
        else if (Request.QueryString["rid"] != null)
        {
            //RSA ads
            var te = new Encryption();
            int BannerId = te.DecryptBannerId(Request.Params["rid"]);
            AdPacksAdvert banner = new AdPacksAdvert(BannerId);
            Response.Redirect(banner.TargetUrl);
        }
    }

    private List<int> getWatchedBannersCollection()
    {
        if (Session[SESSION_ID] == null)
            return new List<int>();
        else
            return (List<int>)Session[SESSION_ID];
    }

    private void saveWatchedBannersCollection(List<int> list)
    {
        Session[SESSION_ID] = list;
    }

}