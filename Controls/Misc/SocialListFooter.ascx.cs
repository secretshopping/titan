using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using iTextSharp.text;
using Prem.PTC;

public partial class Controls_Misc_SocialListFooter : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        facebook.Visible = SetControl(facebook, AppSettings.Site.FacebookLink);
        instagram.Visible = SetControl(instagram, AppSettings.Site.InstagramLink);
        twitter.Visible = SetControl(twitter, AppSettings.Site.TwitterLink);
        googlePlus.Visible = SetControl(googlePlus, AppSettings.Site.GooglePlusLink);
        dribbble.Visible = SetControl(dribbble, AppSettings.Site.DribbbleLink);
    }

    private bool SetControl(HtmlAnchor control, string settings)
    {
        if (string.IsNullOrEmpty(settings))
            return false;

        control.HRef = settings;
        control.Target = "_blank";

        return true;
    }
}