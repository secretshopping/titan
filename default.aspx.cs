using System;
using System.Web.UI;
using Prem.PTC;

public partial class _Default : System.Web.UI.Page
{
    public string TawkSourceID { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        //TITAN News has its own homepage
        if (AppSettings.TitanFeatures.NewsHomepageEnabled)
            Response.Redirect("~/sites/defaultnews.aspx");

        //Theme redirection for demo
        if (AppSettings.IsDemo && AppSettings.Site.Theme != "titan" && !Request.CurrentExecutionFilePath.StartsWith("/Themes"))
            Server.Transfer(String.Format("~/Themes/{0}/default.aspx", AppSettings.Site.Theme));

        GlobalMasterHelper.PageLoad();
        AdBlockManager.CheckDenyForAll();

        if (Request.QueryString["u"] != null)
        {
            //Save the referer in the cookies
            ReferrerUtils.SetReferrer(Request.QueryString["u"]);

            if (!Page.IsPostBack)
                PoolRotatorManager.TryAddLinkView(ReferrerUtils.GetReferrerName());
        }
        else if (Request.QueryString["ref"] != null)
        {
            //Save the referer in the session
            Session["PaidToPromoteReferer"] = Request.QueryString["ref"];
        }
        
        TestimonialsLiteral.Text = Testimonial.GetTestimonials();
        if (AppSettings.Site.IsEUCookiePolicyEnabled)
            ScriptLiteral.Text += "<script src=\"Scripts/EU.js\" type=\"text/javascript\"></script>";

        if (AppSettings.Communication.TawkLiveChatEnabled)
        {
            TawkChatPlaceHolder.Visible = true;
            TawkSourceID = AppSettings.Communication.TawkLiveChatKey;
        }

        PaymentProofsLink.Visible = AppSettings.TitanFeatures.PaymentProofsEnabled;
        FooterNewsLink.Visible = AppSettings.Site.LatestNewsEnabled;

        var cache = new PayoutsScrollingBarCashe().Get();
        LatestPayoutsLiteral.Text = cache.ToString();

    }
}