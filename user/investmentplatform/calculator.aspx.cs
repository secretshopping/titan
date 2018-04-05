using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;

public partial class user_investment_calculator : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {        
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.InvestmentPlatformCalculatorEnabled);

        MainDescriptionP.InnerText = U6008.INVESTMENTCALCULATORDESCRIPTION;

        if (!Member.IsLogged)        
            Master.HideSidebars();        
    }
}