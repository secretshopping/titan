using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;

public class TitanPage : System.Web.UI.Page
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (AppSettings.IsDemo)
        {
            if (Request.QueryString["products"] != null)
                UseTitanDemoHelper.SetProducts(Request.QueryString["products"]);

            if (Request.QueryString["theme"] != null)
                UseTitanDemoHelper.SetTheme(Request.QueryString["theme"]);

            if (Page.MasterPageFile == "/Sites.master" && AppSettings.Site.Theme != "titan")
                Page.MasterPageFile = String.Format("/Themes/{0}/Sites.master", AppSettings.Site.Theme);
        }

    }

}