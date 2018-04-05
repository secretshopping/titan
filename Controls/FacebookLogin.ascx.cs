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
    public string ImageButtonURL { get; set; }

    protected void Page_Init(object sender, EventArgs e)
    {
        ImageButtonURL = "<i class=\"fa fa-facebook\"></i>Sign in with Facebook";
        if ((!Member.IsLogged && AppSettings.Authentication.IsFacebookEnabled))
        {
            FacebookLoginPanel.Visible = true;

            ControlLiteral.Text = "<a href=\"#\" onclick=\"fb_login();return false;\" class=\"btn btn-social btn-lg btn-block btn-facebook m-r-5 m-t-10 text-center\" style=\"padding-right: 45px;\">" + ImageButtonURL + "</a>";
        }
    }

}
