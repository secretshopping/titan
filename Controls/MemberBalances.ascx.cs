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
using Prem.PTC.Advertising;
using Prem.PTC.Texts;

public partial class Controls_MemberBalances : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RefreshBalances(false);
    }

    public void RefreshBalances(bool refreshMember = false)
    {
        if (Member.IsLogged)
        {
            Member member = Member.CurrentInCache;

            if (refreshMember)
                member = Member.Current;

            MainBalanceLabel.Text = member.MainBalance.ToString();
            TrafficBalanceLabel.Text = member.TrafficBalance.ToString();
            AdBalanceLabel.Text = member.PurchaseBalance.ToString();
            MemberBalancesControlPointsLabel.Text = member.PointsBalance.ToString();

            if (AppSettings.Payments.CommissionBalanceEnabled)
            {
                CommissionBalancePlaceHolder.Visible = true;
                CommissionBalanceLabel.Text = member.CommissionBalance.ToString();
            }

            if (AppSettings.PtcAdverts.PTCCreditsEnabled)
            {
                AdCreditsBalanceTrId.Visible = true;
                AdCreditsBalanceLabel.Text = member.PTCCredits.ToString();
            }

            if (!AppSettings.TitanFeatures.EarnTrafficExchangeEnabled && !AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled)
                trafficBalanceTrId.Visible = false;

            if (AppSettings.Payments.CashBalanceEnabled)
            {
                CashBalancePlaceHolder.Visible = true;
                CashBalanceLabel.Text = member.CashBalance.ToString();
            }

            PointsBalanceRow.Visible = AppSettings.Points.PointsEnabled;
        }
    }

}
