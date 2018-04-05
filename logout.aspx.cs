using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using Prem.PTC.Members;
using System.Web.UI.WebControls;

public partial class About : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Member.CurrentInCache.Logout(Response);
        }
        catch (Exception ex) { }

        FormsAuthentication.SignOut();
        Session.Abandon();

        try
        {
            SessionManager.ClearCookiesAfterLogout();
        }
        catch (Exception ex) { }

        Response.Redirect("status.aspx?type=logoutok&id=logout");
    }
}
