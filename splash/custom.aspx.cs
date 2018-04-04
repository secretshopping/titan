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

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AppSettings.SplashPage.SplashPageEnabled)
            Response.Redirect("~/default.aspx");

        if (Request.QueryString["id"] == null)
            Response.Redirect("~/default.aspx");

        int pageId = Convert.ToInt32(Request.QueryString["id"]);

        try
        {
            CustomSplashPage page = new CustomSplashPage(pageId);

            PageLiteral.Text = Server.HtmlDecode(page.Text);

            if (ReferrerUtils.GetReferrerName() == null)
                ReferrerUtils.SetReferrer(page.UserId);
        }
        catch (Exception ex)
        { }

    }

}