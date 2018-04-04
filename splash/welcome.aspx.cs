using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using Prem.PTC.Security;
using Prem.PTC;
using System.Security.Cryptography;
using Resources;
using Prem.PTC.Utils.NVP;

public partial class About : System.Web.UI.Page
{

    public static string BaseUrl
    {
        get
        {
            HttpContext context = HttpContext.Current;
            string baseUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + context.Request.ApplicationPath.TrimEnd('/') + '/';
            return baseUrl;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AppSettings.SplashPage.SplashPageEnabled)
            Response.Redirect("~/default.aspx");

        form2.Action = Request.RawUrl;

        //Insert Youtube URL
        if (AppSettings.SplashPage.SplashPageYoutubeUrl != "")
        {
            SplashPageYoutubeUrl.Text = "<iframe style='height: 200px; width: 350px;' src='"+ AppSettings.SplashPage.SplashPageYoutubeUrl + "' ></iframe>";
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