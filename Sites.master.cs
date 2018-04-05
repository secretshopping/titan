using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Prem.PTC;

public partial class Sites : System.Web.UI.MasterPage
{
    public string TawkSourceID { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        GlobalMasterHelper.PageLoad();
        AdBlockManager.CheckDenyForAll();

        //Fix the rewriting URL postback
        form1.Action = Request.RawUrl;

        UserName.Text = Member.CurrentName;

        HeaderLiteral.Text = string.Format(@" 
                <base href='{0}' />
                <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
                <link rel=""shortcut icon"" type=""image/ico"" href='{1}'>
                <title>{2} | {3}</title>", BaseUrl, AppSettings.Site.FaviconImageURL, AppSettings.Site.Name,
            AppSettings.Site.Slogan);

        ThemeLiteral.Text = string.Format("<link rel='stylesheet' href='Themes/{0}/css/sites.css' />", AppSettings.Site.Theme);

        if (AppSettings.Communication.TawkLiveChatEnabled)
        {
            TawkChatPlaceHolder.Visible = true;
            TawkSourceID = AppSettings.Communication.TawkLiveChatKey;
        }

        PaymentProofsLink.Visible = AppSettings.TitanFeatures.PaymentProofsEnabled;
        FooterNewsLink.Visible = AppSettings.Site.LatestNewsEnabled;
    }

    public static string BaseUrl
    {
        get
        {
            HttpContext context = HttpContext.Current;
            string baseUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + context.Request.ApplicationPath.TrimEnd('/') + '/';
            return baseUrl;
        }
        
    }
}
