using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;

public partial class Controls_Misc_FacebookHandlers : System.Web.UI.UserControl
{
    protected void Page_Init(object sender, EventArgs e)
    {
        //This page has its own separate Facebook OGraph Information
        if (!Page.IsPostBack && (Request.Url.AbsolutePath.StartsWith("/sites/article.aspx") || Request.Url.AbsolutePath.StartsWith("/article/")))
        {
            this.Visible = false;
        }
    }

    public static string FacebookOgraphImageURL
    {
        get
        {
            return AppSettings.Site.LogoImageURL.StartsWith("http") ? AppSettings.Site.LogoImageURL : AppSettings.Site.Url + AppSettings.Site.LogoImageURL;
        }
    }
}