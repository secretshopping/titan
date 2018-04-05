using Prem.PTC;
using System;
using System.Web;

public partial class user_ico_info : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.ICOInfoEnabled);

        if (!IsPostBack)
        {
            DescriptionLiteral.Text = HttpUtility.HtmlDecode(AppSettings.ICO.ICOInformationHTML);
        }
    }
}