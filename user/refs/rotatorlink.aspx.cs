using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Referrals;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RotatorLink : System.Web.UI.Page
{
    enum RotatorBalance
    {
        AdCredits = 1,
        AdBalance = 2
    }

    private RotatorBalance selectedBalance
    {
        get
        {
            if (AppSettings.Referrals.ReferralPoolRotatorPricePerMonthInAdCredits > 0)
                return RotatorBalance.AdCredits;
            
            return RotatorBalance.AdBalance;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.ReferralPoolRotatorEnabled);

        if (!IsPostBack)
        {            
            Button1.Text = L1.MANAGE;
            Button2.Text = ParticipateButton.Text = L1.BUY;
            int UserId = Member.CurrentId;

            if (PoolRotatorManager.IsInPool(UserId))
            {
                ParticipatePlaceHolder.Visible = true;
                NoParticipatePlaceHolder.Visible = false;

                var linkUser = PoolRotatorManager.Get(UserId);

                LinkOpensLabel.Text = linkUser.ClicksDelivered.ToString();
                //ReferralsGainedLabel.Text = linkUser.ReferralsDelivered.ToString();
                RotatorExpiresLiteral.Text = linkUser.Expires.ToString();
                RotatorLinkLiteral.Text = String.Format("<a href='{0}'>{0}</a>", linkUser.GetLink());
            }

            if (selectedBalance == RotatorBalance.AdCredits)
                PriceLiteral.Text = AppSettings.Referrals.ReferralPoolRotatorPricePerMonthInAdCredits.ToString() + " " + U5006.ADCREDITS;
            else if (selectedBalance == RotatorBalance.AdBalance)
                PriceLiteral.Text = AppSettings.Referrals.ReferralPoolRotatorPricePerMonth.ToString();

            PriceLiteral.Text += " / 30 " + L1.DAYS.ToLower();
            MembersInPoolLabel.Text = PoolRotatorStatistics.MembersInThePool.ToString();
        }
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        SPanel.Visible = false;
        EPanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        MenuMultiView.ActiveViewIndex = viewIndex;

        if (viewIndex == 0)
            Response.Redirect("rotatorlink.aspx");

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";
    }

    public void ParticipateButton_Click(object sender, EventArgs e)
    {
        SPanel.Visible = false;
        EPanel.Visible = false;

        try
        {
            Member user = Member.Current;
            if (selectedBalance == RotatorBalance.AdCredits)
            {
                if (user.PTCCredits < AppSettings.Referrals.ReferralPoolRotatorPricePerMonthInAdCredits)
                    throw new MsgException(L1.NOTENOUGHFUNDS);

                user.SubstractFromPTCCredits(AppSettings.Referrals.ReferralPoolRotatorPricePerMonthInAdCredits, "Rotator Link");
            }

            else if (selectedBalance == RotatorBalance.AdBalance)
            {
                if (user.PurchaseBalance < AppSettings.Referrals.ReferralPoolRotatorPricePerMonth)
                    throw new MsgException(L1.NOTENOUGHFUNDS);

                user.SubtractFromPurchaseBalance(AppSettings.Referrals.ReferralPoolRotatorPricePerMonth, "Rotator Link");
            }
            user.SaveBalances();

            if (PoolRotatorManager.IsInPool(user.Id))
            {
                //Prolong
                var link = PoolRotatorManager.Get(user.Id);
                link.Expires = link.Expires.AddDays(30);
                link.Save();
            }
            else
            {
                PoolRotatorManager.AddToPool(user.Id);
            }

            SPanel.Visible = true;
            SText.Text = L1.OP_SUCCESS;
        }
        catch (MsgException ex)
        {
            EPanel.Visible = true;
            EText.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }
}