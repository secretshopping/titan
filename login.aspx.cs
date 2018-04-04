using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using Resources;
using Prem.PTC.Members;
using Prem.PTC;
using Prem.PTC.Utils;
using Titan;

public partial class About : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Member.IsLogged)
            Response.Redirect("~/user/default.aspx");

        if (TitanFeatures.IsAdzbuzz)
            Titan.CustomFeatures.AdzbuzzOAuth.CreateRedirect();

        DemoInfoPlaceholder.Visible = AppSettings.IsDemo;
    }


}
