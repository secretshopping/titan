using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Members;

public delegate void CaptchaClaimIsValid(bool value);
public partial class Controls_CaptchaClaim : System.Web.UI.UserControl
{
    public event CaptchaClaimIsValid IsValid;

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            
        }
    }

    protected void ClaimCoinhiveButton_Click(object sender, EventArgs e)
    {
        bool isValid = new CaptchaClaim().VerifyAndClaim(HttpContext.Current.Request.Params["coinhive-token"]);

        if (IsValid != null)
        {
            IsValid(isValid);
        }
    }
}