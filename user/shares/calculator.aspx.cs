using Prem.PTC;
using Resources;
using System;

public partial class user_calculator : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.RevShare.AdPacksCalculatorEnabled);

        MainDescriptionP.InnerText = U6007.CALCULATORDESCRIPTION;
    }
}