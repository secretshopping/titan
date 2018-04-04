using Prem.PTC;
using System;
using System.Web.UI.WebControls;

public partial class user_investmentplatform_queuesystem : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.InvestmentPlatformQueueSystemEnabled);

        SuccessPanel.Visible = false;
        ErrorPanel.Visible = false;
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var viewIndex = Int32.Parse(button.CommandArgument);

        MenuMultiView.ActiveViewIndex = viewIndex;

        foreach (Button b in MenuButtonPlaceHolder.Controls)
            b.CssClass = "";

        button.CssClass = "ViewSelected";
    }
}