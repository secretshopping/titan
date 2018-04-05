using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Resources;
using Prem.PTC;
using Prem.PTC.Memberships;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Titan.Pages;

public partial class About : System.Web.UI.Page
{
    bool HideCashBalanceDepositCommissionColumn;
    Dictionary<string, bool> autopayHelpArray;
    List<string> hiddenProperties;
    Member User;

    protected void Page_Load(object sender, EventArgs e)
    {
        var targetFeature = PurchaseOption.Features.Upgrade;
        var purchaseOption = PurchaseOption.Get(targetFeature);

        if (purchaseOption.PurchaseBalanceEnabled)
            UpgradeFromAdBalance.Visible = true;
        else
        {
            UpgradeFromAdBalance.Visible = false;
            adBalanceInfo.Visible = false;
        }

        if (AppSettings.Payments.CashBalanceEnabled && purchaseOption.CashBalanceEnabled)
            UpgradeFromCashBalance.Visible = true;
        else
        {
            UpgradeFromCashBalance.Visible = false;
            cashBalanceInfo.Visible = false;
        }

        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.UpgradeEnabled);

        User = Member.CurrentInCache;

        if (!Page.IsPostBack)
        {
            //Lang
            LangAdder.Add(UpgradeFromAdBalance, U6012.PAYVIAPURCHASEBALANCE);
            LangAdder.Add(UpgradeFromCashBalance, U6005.PAYVIACASHBALANCE);
            LangAdder.Add(UpgradeViaPaymentProcessor, U6005.PAYVIAPAYMENTPROCESSOR);

            //Bind the data dor DDL
            BindDataToDDL();

            //Visibility
            UpgradeViaPaymentProcessorPlaceHolder.Visible = PaymentAccountDetails.AreIncomingPaymentProcessorsAvailable();
        }

        if (TitanFeatures.IsRofriqueWorkMines)
        {
            UpgradeFromAdBalancePlaceHolder.Visible = false;
            UpgradeViaPaymentProcessorPlaceHolder.Visible = false;
        }

        HideCashBalanceDepositCommissionColumn = AreAllCashBalanceDepositCommissionsZero();

        ddlOptions.Attributes.Add("onchange", "updatePrice();");
        autopayHelpArray = new Dictionary<string, bool>();

        Label10.Text = AppSettings.Memberships.TenPointsValue.ToClearString();
        LabelIle.Text = AppSettings.Memberships.UpgradePointsDiscount.ToString();

        //Display warning
        string WarningMessage = UpgradePageHelper.GetWarningMessage(User);
        WarningPanel.Visible = !String.IsNullOrEmpty(WarningMessage);
        WarningLiteral.Text = WarningMessage;

        AdBalanceLiteral.Text = User.PurchaseBalance.ToString();
        CashBalanceLiteral.Text = User.CashBalance.ToString();

        if (AppSettings.Points.LevelMembershipPolicyEnabled)
        {
            BuyUpgradePlaceHolder.Visible = false;
        }

        hiddenProperties = MembershipProperty.GetPropsToHideForClient();
        TypesMembershipProperties.Text = AdPackTypeMembershipMapper.Mapper.GetHtmlFromCache();
        AdPackPropsPlaceHolder.Visible = AppSettings.TitanFeatures.AdvertAdPacksEnabled;

        PaymentProcessorsButtonPlaceholder.Visible = false;
    }

    private void BindDataToDDL()
    {
        Money FirstElementPrice = Money.Zero;

        ddlOptions.Items.Clear();
        ddlOptions.Items.AddRange(UpgradePageHelper.GetMembershipPacks(User, ref FirstElementPrice));
        ddlOptions.DataBind();

        PriceLiteral.Text = FirstElementPrice.ToString();
    }


    protected void UpgradeGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Create an instance of the datarow
            var rowData = e.Row.Cells[1].Text;

            //Set proper width
            //e.Row.Cells[0].ControlStyle.Width = Unit.Pixel(230);
            foreach (TableCell tc in e.Row.Cells)
                if (tc != e.Row.Cells[0])
                {
                    //tc.ControlStyle.Width = Unit.Pixel(50);
                    tc.ControlStyle.CssClass = "text-center";
                }


            if (hiddenProperties.Any(p => p == e.Row.Cells[0].Text))
                e.Row.CssClass = "displaynone";

            //Add image to index 1 (name)
            if (e.Row.RowIndex == 1)
            {
                int indexOfUpgrade = 0;
                foreach (TableCell tc in e.Row.Cells)
                    if (tc != e.Row.Cells[0])
                    {
                        string upgradeName = tc.Text;
                        string imageName = "standardbox";
                        if (upgradeName != Membership.Standard.Name)
                        {
                            //Because max premiumimagename is 7
                            //I know it is shitty work
                            imageName = (indexOfUpgrade > 7) ? "premiumbox7" : "premiumbox" + indexOfUpgrade.ToString();
                        }
                        tc.Text = "<div class=\"text-center\"><p><strong>" + upgradeName + "</strong></p><img src=\"Images/OneSite/Upgrade/" + imageName + ".png\" /></div>";
                        indexOfUpgrade++;
                    }
            }

            e.Row.Cells[0].Text = MembershipProperty.GetResourceLabel(e.Row.Cells[0].Text);


            foreach (TableCell tc in e.Row.Cells)
                if (tc != e.Row.Cells[0])
                {
                    tc.Text = MembershipProperty.Format(e.Row.RowIndex, tc.Text);
                }


            //Add color
            if (e.Row.RowIndex == 12)
                foreach (TableCell tc in e.Row.Cells)
                    if (tc != e.Row.Cells[0])
                        tc.Text = "<div class=\"upgrade-table-label\" style=\"background-color:" + tc.Text + ";\">&nbsp;</div>";


            //Hide AutoPay price if no autopay
            if (e.Row.RowIndex == 15)
                foreach (TableCell tc in e.Row.Cells)
                    if (tc != e.Row.Cells[0] && tc.Text.Contains("$0.000"))
                        tc.Text = "&nbsp;";

        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            foreach (TableCell tc in e.Row.Cells)
                tc.Text = "&nbsp;";
        }
    }

    protected void upgradeFromAdBalance_Click(object sender, EventArgs e)
    {
        upgradeMembershipFromBalance(PurchaseBalances.Purchase);
    }

    protected void upgradeFromCashBalance_Click(object sender, EventArgs e)
    {
        upgradeMembershipFromBalance(PurchaseBalances.Cash);
    }

    private void upgradeMembershipFromBalance(PurchaseBalances balanceType)
    {
        SuccMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;

        try
        {
            AppSettings.DemoCheck();

            MembershipPack pack = new MembershipPack(Int32.Parse(ddlOptions.SelectedValue));
            Member user = Member.Current;

            Membership.BuyPack(user, pack, balanceType);

            Response.Redirect("~/status.aspx?type=upgradeok");
        }
        catch (MsgException ex)
        {
            ErrorMessagePanel.Visible = true;

            if (!TitanFeatures.IsRofriqueWorkMines)
                ErrorMessage.Text = ex.Message;
            else
                ErrorMessage.Text = "YOU DONT HAVE ENOUGH FUNDS IN YOUR CASH BALANCE. PLEASE CLICK ON DEPOSIT FUNDS TO TOP UP YOUR CASH BALANCE.";
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            throw ex;
        }
    }

    protected void upgradeViaPaymentProcessor_Click(object sender, EventArgs e)
    {
        User = Member.Current;

        PaymentProcessorsButtonPlaceholder.Visible = true;
        BuyUpgradePlaceHolder.Visible = false;

        MembershipPack pack = new MembershipPack(Int32.Parse(ddlOptions.SelectedValue));
        Money PackPrice = pack.GetPrice(User);

        PriceLiteral.Text = PackPrice.ToString();

        // Buy membership directly via Paypal, etc.
        var bg = new UpgradeMembershipButtonGenerator(User, PackPrice, pack);
        PaymentButtons.Text = GenerateHTMLButtons.GetPaymentButtons(bg);
    }

    #region Commissions

    protected void BindCommissionsGridViewDataSource()
    {
        CommissionsGridView_DataSource.SelectCommand = string.Format("SELECT * FROM Commissions WHERE RefLevel <= {0} AND MembershipId = {1} ORDER BY RefLevel",
            AppSettings.Referrals.ReferralEarningsUpToTier, MembershipDDL.SelectedValue);
    }

    protected void CommissionsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        if (row.RowType == DataControlRowType.DataRow)
        {
            if (!AppSettings.TitanFeatures.AdvertBannersEnabled)
            {
                row.Cells[2].CssClass = "displaynone";
                CommissionsGridView.Columns[2].HeaderStyle.CssClass = "displaynone";
            }
            if (!AppSettings.TitanFeatures.AdvertAdPacksEnabled)
            {
                row.Cells[3].CssClass = "displaynone";
                CommissionsGridView.Columns[3].HeaderStyle.CssClass = "displaynone";
            }
            if (!AppSettings.TitanFeatures.EarnOfferwallsEnabled)
            {
                row.Cells[4].CssClass = "displaynone";
                CommissionsGridView.Columns[4].HeaderStyle.CssClass = "displaynone";
            }
            if (!AppSettings.TitanFeatures.EarnCPAGPTEnabled)
            {
                row.Cells[5].CssClass = "displaynone";
                CommissionsGridView.Columns[5].HeaderStyle.CssClass = "displaynone";
            }
            if (!AppSettings.TitanFeatures.AdvertTrafficGridEnabled)
            {
                row.Cells[6].CssClass = "displaynone";
                CommissionsGridView.Columns[6].HeaderStyle.CssClass = "displaynone";
            }
            if (!AppSettings.Payments.CashBalanceEnabled || HideCashBalanceDepositCommissionColumn)
            {
                row.Cells[7].CssClass = "displaynone";
                CommissionsGridView.Columns[7].HeaderStyle.CssClass = "displaynone";
            }

            for (int i = 1; i <= 7; i++)
                row.Cells[i].Text = NumberUtils.FormatPercents(row.Cells[i].Text);
        }
        CommissionsGridView.Columns[0].HeaderText = U5009.TIER;
        CommissionsGridView.Columns[1].HeaderText = U5009.UPGRADEPURCHASE;
        CommissionsGridView.Columns[2].HeaderText = U5009.BANNERPURCHASE;
        CommissionsGridView.Columns[3].HeaderText = string.Format(U5009.ADPACKPURCHASE, AppSettings.RevShare.AdPack.AdPackName);
        CommissionsGridView.Columns[4].HeaderText = U5009.OFFERWALLS;
        CommissionsGridView.Columns[5].HeaderText = "CPA/GPT";
        CommissionsGridView.Columns[6].HeaderText = U5009.TRAFFICGRIDPURCHASE;
        CommissionsGridView.Columns[7].HeaderText = U6005.CASHBALANCEDEPOSIT;
    }

    private bool AreAllCashBalanceDepositCommissionsZero()
    {
        List<Commission> commissions = (List<Commission>)new CommissionsCache().Get();
        decimal sumOfCashBalanceCommissions = 0;
        foreach (var commission in commissions)
        {
            sumOfCashBalanceCommissions += commission.CashBalanceDepositPercent;
        }
        return Decimal.Equals(sumOfCashBalanceCommissions, 0);
    }

    protected void MembershipDDL_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindCommissionsGridViewDataSource();
    }

    protected void BindDataToMembershipsDDL()
    {
        var list = new Dictionary<string, string>();
        var memberships = TableHelper.SelectRows<Membership>(TableHelper.MakeDictionary("Status", (int)MembershipStatus.Active));

        for (int i = 0; i < memberships.Count; i++)
        {
            list.Add(memberships[i].Id.ToString(), memberships[i].Name);
        }

        MembershipDDL.DataSource = list;
        MembershipDDL.DataTextField = "Value";
        MembershipDDL.DataValueField = "Key";
        MembershipDDL.DataBind();
    }

    protected void MembershipDDL_Init(object sender, EventArgs e)
    {
        BindDataToMembershipsDDL();
    }

    protected void CommissionsGridView_DataSource_Load(object sender, EventArgs e)
    {
        BindCommissionsGridViewDataSource();
    }


    #endregion
}
