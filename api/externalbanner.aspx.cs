using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan.Publisher;

public partial class externalbanner : System.Web.UI.Page
{
    public string targetUrl { get; private set; }

    private ExternalBannerManager manager;
    Uri _urlReferer;
    private Uri urlReferer
    {
        get
        {
            if (_urlReferer == null)
            {
                _urlReferer = ViewState["UrlReferer"] as Uri;
            }
            return _urlReferer;
        }
        set
        {

            _urlReferer = value;
            ViewState["UrlReferer"] = _urlReferer;
        }
    }
    private bool isAuthorized { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            urlReferer = Request.UrlReferrer;
        }

        int dimensionsId;
        int publishersWebsiteId;

        isAuthorized = urlReferer != null && urlReferer.Host != Request.Url.Host && Request.QueryString["d"] != null && int.TryParse(Request.QueryString["d"], out dimensionsId)
            && int.TryParse(Request.QueryString[GlobalPostback.Parameters.PublishersWebsiteId], out publishersWebsiteId);

        dimensionsId = Convert.ToInt32(Request.QueryString["d"]);
        publishersWebsiteId = Convert.ToInt32(Request.QueryString[GlobalPostback.Parameters.PublishersWebsiteId]);

        if (!isAuthorized)
        {
            this.Visible = false;
            return;
        }

        manager = new ExternalBannerManager(urlReferer.Host, dimensionsId, publishersWebsiteId);

        var banner = manager.Banner;

        if (banner != null)
        {
            targetUrl = banner.Url;
            BannerLink.ImageUrl = banner.ImagePath.Replace("~", AppSettings.Site.Url);
            BannerLink.Width = banner.Image.Width;
            BannerLink.Height = banner.Image.Height;
        }
    }

    protected void BannerLink_Click(object sender, EventArgs e)
    {
        if (manager != null)
        {
            manager.CreditForBannerClick(IP.Current);
        }
    }
}