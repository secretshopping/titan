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

public partial class Points : System.Web.UI.Page
{
    Member user;
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.StatisticsPointsEarnedEnabled);
        user = Member.CurrentInCache;
        if (!IsPostBack)
        {
            Button1.Text = U5001.TOTAL;
            Button2.Text = L1.REFERRALS;
            TotalLiteral.Text = user.TotalPointsEarned.ToString();
            TotalRefLiteral.Text = user.TotalDirectReferralsPointsEarned.ToString();
            UserStats.StatTitle = DRStats.StatTitle = U5001.TOTALPOINTSCREDITEDTOYOU.Replace("%n%", AppSettings.PointsName);
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
