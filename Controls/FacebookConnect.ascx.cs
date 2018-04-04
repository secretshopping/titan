using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;
using System.Text;

public partial class Controls_FacebookLogin : System.Web.UI.UserControl
{
    public string ButtonText { get; set; }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!AppSettings.Authentication.IsFacebookEnabled || !Member.IsLogged)
        {
            this.Visible = false;
            return;
        }

        var user = Member.Current;

        if (string.IsNullOrEmpty(user.FacebookOAuthId))
        {
            ButtonText = U6004.CONNECTWITHFACEBOOK;

            FacebookLoginPanel.Visible = true;

            ControlLiteral.Text = "<a href=\"#\" onclick=\"fb_login();return false;\" class=\"btn btn-social btn-lg btn-block btn-facebook m-r-5 m-t-10 text-center\" style=\"padding-right: 45px;\"><i class=\"fa fa-facebook\"></i> " + ButtonText + "</a>";
        }
        else
        {
            ButtonText = string.Format(U6004.CONNECTEDTOFBAS, user.FacebookName);
            ControlLiteral.Text = "<a href=\"#\" class=\"btn btn-social btn-lg btn-block btn-facebook m-r-5 m-t-10 text-center\" style=\"padding-right: 45px;\"><i class=\"fa fa-facebook\"></i> " + ButtonText + "</a>";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
