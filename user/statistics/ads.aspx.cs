using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using System.Data;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Titan.Shares;
using Prem.PTC.Advertising;
using Titan;

public partial class PTCClicks : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Member User;

        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.StatisticsPTCClicksEnabled);
        User = Member.CurrentInCache;
        if (!IsPostBack)
        {
            Button1.Text = U5001.YOURACTIONS;
            Button2.Text = L1.REFERRALS;
            ReferralsStatistics.StatTitle = L1.DIRECT + " " + L1.REFERRALS + " " + L1.CLICKSSMALL;
            TotalLiteral.Text = User.TotalClicks.ToString();
            TotalLiteral1.Text = User.TotalDRPTCClicks.ToString();
            
            TotalLiteralCashLink.Text = User.TotalEarnedFromCashLinks.ToString();
            TotalRefLiteral.Text = User.TotalEarnedFromDRCashLinks.ToString();
        }
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";
    }
}
