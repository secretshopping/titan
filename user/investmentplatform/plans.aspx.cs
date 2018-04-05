using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan.InvestmentPlatform;

public partial class user_investmentplatform_plans : System.Web.UI.Page
{
    private Member User = Member.CurrentInCache;
    private List<InvestmentPlatformPlan> availablePlans;
    private List<InvestmentUsersPlans> userActivePlans;
    private enum PlanAction { Buy, Upgrade };
    private enum InvestmentPlatformMode { Standard, Levels }
    private InvestmentPlatformMode CurrentMode
    {
        get
        {
            return AppSettings.InvestmentPlatform.LevelsEnabled ? InvestmentPlatformMode.Levels : InvestmentPlatformMode.Standard;
        }
    }
    private int DaysCounter;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.InvestmentPlatform.InvestmentPlatformEnabled);

        if (TitanFeatures.IsRofrique)
            BuyOptionsPlaceHolder.Visible = false;

        availablePlans = InvestmentPlatformManager.GetAllAvailablePlansForUser(User.Id);

        if (Request.QueryString["m"] != null)
            MenuButton_Click(ManageButton, null);

        SuccessPanel.Visible = false;
        ErrorPanel.Visible = false;

        InitManageViewControls();

        if (!Page.IsPostBack)
        {
            InitBuyViewControls();
            if (CurrentMode == InvestmentPlatformMode.Levels)
            {
                LevelsGridView.Columns[0].HeaderText = U5007.LEVELS.ToUpper();
                LevelsGridView.Columns[1].HeaderText = U4200.DEPOSIT;
                LevelsGridView.Columns[2].HeaderText = U4000.EARNINGS;
                LevelsGridView.Columns[3].HeaderText = U6012.FEEFOREACHDEPOSIT;
                LevelsGridView.Columns[4].HeaderText = U6012.MAXIMUMTIMESADAY;
                LevelsGridView.Columns[5].HeaderText = U6013.AVAILABLEFROM;
                LevelsGridView.Columns[6].HeaderText = U6013.AVAILABLEWITHPP;
            }
        }

        InitPlans();
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var viewIndex = Int32.Parse(button.CommandArgument);

        MenuMultiView.ActiveViewIndex = viewIndex;

        if (viewIndex == 1)
            InitManageViewControls();

        foreach (Button b in MenuButtonPlaceHolder.Controls)
            b.CssClass = "";

        button.CssClass = "ViewSelected";
    }

    protected void BuyOrUpgradePlan(PurchaseBalances balance)
    {  
        try
        {
            var plan = new InvestmentPlatformPlan(int.Parse(PlansDropDownList.SelectedValue));
            var activePlans = CurrentMode == InvestmentPlatformMode.Levels ? 0 : userActivePlans.Count;

            if (AppSettings.InvestmentPlatform.LevelsEnabled)
                InvestmentLevelsManager.CanUserDepositOnLevel(plan, User);

            if (plan.MaxPrice > Money.Zero)
            {
                if (AppSettings.InvestmentPlatform.InvestmentPlatformPlansPolicy == PlansPolicy.OneUpgradedPlan)
                    throw new MsgException(U6012.CANTUPGRADERANGEPLAN);

                var targetPrice = Money.Parse(RangePriceTextBox.Text);
                if (plan.CheckPlanPrice(targetPrice))                
                    InvestmentPlatformManager.BuyOrUpgradePlan(Member.Current, balance, plan, targetPrice);                
                else                
                    throw new MsgException(U6012.TYPECORRECTPRICE);                
            }
            else
                InvestmentPlatformManager.BuyOrUpgradePlan(Member.Current, balance, plan);

            //IF activePlans = 1, MEANS THAT WE UPGRADE PLAN (ON PlansPolicy.OneUpgradedPlan)
            if (AppSettings.InvestmentPlatform.InvestmentPlatformPlansPolicy == PlansPolicy.OneUpgradedPlan && activePlans == 1)
                SuccessTextLiteral.Text = string.Format(U6011.SUCCESSUPGRADEPLAN, plan.Name);
            else
                SuccessTextLiteral.Text = string.Format(U6006.SUCCESBOUGHTPLAN, plan.Name);

            SuccessPanel.Visible = true;

            availablePlans = InvestmentPlatformManager.GetAllAvailablePlansForUser(User.Id);
            InitBuyViewControls();
            InitPlans();
        }
        catch (Exception ex)
        {
            ErrorPanel.Visible = true;
            ErrorTextLiteral.Text = ex.Message;
            if (!(ex is MsgException))
                ErrorLogger.Log(ex);
        }
    }

    //TO DO
    protected void WithdrawAllMoneyFromSystem_Click(object sender, EventArgs e)
    {
        try
        {
            var userPlans = InvestmentPlatformManager.GetUserActivePlans(User.Id);

            foreach (var plan in userPlans)
            {
                plan.TryToWidthdrawlMoneyFromSystem(true);
            }

            SuccessPanel.Visible = true;
            SuccessTextLiteral.Text = U6010.INVPLATFORMMASSTRANSFERSUCCESS;
        }
        catch (MsgException mex)
        {
            ErrorPanel.Visible = true;
            ErrorTextLiteral.Text = mex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    private void InitPlans()
    {
        switch (CurrentMode)
        {
            case InvestmentPlatformMode.Standard:
                InvestmentPlatformPlaceHolder.Visible = true;
                InvestmentsPlansPlaceHolder.Controls.Clear();

                if (availablePlans.Count == 0)
                {
                    NoContetntPlaceHolder.Visible = true;
                    BuyOptionsPlaceHolder.Visible = false;
                    InvestmentPlatformPlaceHolder.Visible = false;
                    NoContentLiteral.Text = U6011.NOINVESTMENTPLANS;
                    return;
                }

                userActivePlans = InvestmentPlatformManager.GetUserActivePlans(User.Id);
                var isUpgrade = AppSettings.InvestmentPlatform.InvestmentPlatformPlansPolicy == PlansPolicy.OneUpgradedPlan && userActivePlans.Count > 0;

                if (isUpgrade)
                    PurchaseDescriptionLabel.Text = U6011.PLANUPGRADE;
                else
                    PurchaseDescriptionLabel.Text = U6011.PLANBUY;

                foreach (var plan in availablePlans)
                    InvestmentsPlansPlaceHolder.Controls.Add(GetAdHTML(plan));
                break;
            case InvestmentPlatformMode.Levels:
                LevelsPlaceHolder.Visible = true;
                break;
        }
    }

    private void InitBuyViewControls()
    {
        WithdrawAllMoneyFromSystem.Text = L1.TRANSFERMONEY;
        PlansButton.Text = CurrentMode == InvestmentPlatformMode.Standard ? U6006.PLANS : U5007.LEVELS;
        ManageButton.Text = L1.MANAGE;
        MainDescriptionP.InnerText = CurrentMode == InvestmentPlatformMode.Standard ? U6006.PLANSDESCRIPTION : U6012.LEVELSDESCRIPTION;

        PlansDropDownList.Items.Clear();

        foreach (var plan in availablePlans)
            PlansDropDownList.Items.Add(new ListItem(plan.Name, plan.Id.ToString()));

        //TMP ??
        //if (CurrentMode == InvestmentPlatformMode.Standard)
        {
            var targetFeature = PurchaseOption.Features.InvestmentPlatform;
            var purchaseOption = PurchaseOption.Get(targetFeature);

            PurchaseBalanceLabel.Text = string.Format("{0}: {1}", U6012.PURCHASEBALANCE, User.PurchaseBalance.ToString());
            CashBalanceLabel.Text = string.Format("{0}: {1}", U5008.CASHBALANCE, User.CashBalance.ToString());

            BuyFromPurchaseBalanceButton.Visible = PurchaseBalanceInfoPlaceHolder.Visible = purchaseOption.PurchaseBalanceEnabled;
            BuyFromCashBalanceButton.Visible = CashBalanceInfoPlaceHolder.Visible = AppSettings.Payments.CashBalanceEnabled && purchaseOption.CashBalanceEnabled;

            LangAdder.Add(BuyFromPurchaseBalanceButton, U6012.PAYVIAPURCHASEBALANCE);
            LangAdder.Add(BuyFromCashBalanceButton, U6005.PAYVIACASHBALANCE);
        }
        //else
        //    BuyFromPurchaseBalanceButton.Visible = BuyFromCashBalanceButton.Visible = PurchaseBalanceInfoPlaceHolder.Visible = CashBalanceInfoPlaceHolder.Visible = true; ///tmp

        LangAdder.Add(BuyViaPaymentProcessorButton, U6005.PAYVIAPAYMENTPROCESSOR);
        PlansDropDownList_SelectedIndexChanged(null, null);
        PurchaseDescriptionLabel.Text = U6011.PLANBUY;


        ///TMP
        //BuyFromPurchaseBalanceButton.Visible = true;
        ///TMP
    }

    private void InitManageViewControls()
    {
        //TO DO LATER
        //WithdrawAllMoneyFromSystem.Visible = !AppSettings.InvestmentPlatform.InvestmentPlatformDailyLimitsEnabled;
        UserPlanDetailsPlaceHolder.Controls.Clear();

        InformationLiteral.Text = string.Format("{0}: {1}", U6006.MINAMOUNTTOPAYOUT, User.Membership.InvestmentPlatformMinAmountToCredited);
        MoneyInSystemLabel.Text = string.Format("{0}: {1}", U6006.MONEYINSYSTEM, InvestmentUsersPlans.GetMoneyInSystemFromFinishedPlans(User.Id));

        var userPlans = InvestmentPlatformManager.GetUserActivePlans(User.Id);

        if (userPlans.Count > 0)
        {
            NoPlansPlaceHolder.Visible = false;
            InformationPanel.Visible = true;
            UsersPlanPlaceHolder.Visible = true;

            if (AppSettings.InvestmentPlatform.InvestmentPlatformPlansPolicy == PlansPolicy.OneUpgradedPlan)
            {
                var currentPlan = new InvestmentPlatformPlan(userPlans[0].PlanId);
                UserPlanDetailsPlaceHolder.Controls.Add(GetAdHTML(currentPlan, true, userPlans[0]));
                ManageDescription.Text = string.Format("{0}:", U6006.YOURPLAN);
            }
            else
            {
                userPlans.Sort(Comparison);

                foreach (var plan in userPlans)
                {
                    var mainPlan = new InvestmentPlatformPlan(plan.PlanId);
                    UserPlanDetailsPlaceHolder.Controls.Add(GetAdHTML(mainPlan, true, plan));
                }

                ManageDescription.Text = string.Format("{0}s:", U6006.YOURPLAN);
            }
        }
        else
        {
            InformationPanel.Visible = false;
            UsersPlanPlaceHolder.Visible = false;
            NoPlansPlaceHolder.Visible = true;
            NoPlansLabel.Text = U6006.YOUDONTHAVEANYPLAN;
        }
    }

    private UserControl GetAdHTML(InvestmentPlatformPlan plan, bool includedUsersEarning = false, InvestmentUsersPlans userPlan = null)
    {
        var objControl = (UserControl)Page.LoadControl("~/Controls/InvestmentPlatform/InvestmentPlanDetails.ascx");
        var parsedControl = objControl as IInvestmentPlanObjectControl;
        parsedControl.PlatformPlan = plan;

        if (includedUsersEarning)
        {
            parsedControl.IncludedUsersEarning = includedUsersEarning;
            parsedControl.UserPlan = userPlan;
        }

        parsedControl.DataBind();

        return (UserControl)parsedControl;
    }

    private int Comparison(InvestmentUsersPlans x, InvestmentUsersPlans y)
    {
        var plan1 = new InvestmentPlatformPlan(x.PlanId);
        var plan2 = new InvestmentPlatformPlan(y.PlanId);

        return plan1.Number < plan2.Number ? -1 : 1;
    }

    private void UpdatePriceLiteral()
    {
        if (PlansDropDownList.Items.Count > 0)
        {
            var plan = new InvestmentPlatformPlan(int.Parse(PlansDropDownList.SelectedValue));
            var text = string.Empty;

            if (plan.MaxPrice > Money.Zero)
            {
                RangePricePlaceHolder.Visible = true;
                RangePriceTextBox.Text = plan.Price.ToClearString();
                text = U6012.PRICERANGE;
            }
            else
            {
                RangePricePlaceHolder.Visible = false;
                text = L1.PRICE;
            }
            
            PriceLiteral.Text = string.Format("{0}: {1}", text, plan.GetPriceText());
        }
    }

    protected void BuyFromPurchaseBalanceButton_Click(object sender, EventArgs e)
    {
        BuyOrUpgradePlan(PurchaseBalances.Purchase);
    }

    protected void BuyFromCashBalanceButton_Click(object sender, EventArgs e)
    {
        BuyOrUpgradePlan(PurchaseBalances.Cash);
    }

    protected void BuyViaPaymentProcessorButton_Click(object sender, EventArgs e)
    {
        try
        {
            var selectedPlan = new InvestmentPlatformPlan(int.Parse(PlansDropDownList.SelectedValue));
            var targetPrice = selectedPlan.Price;

            if (AppSettings.InvestmentPlatform.LevelsEnabled)
                InvestmentLevelsManager.CanUserDepositOnLevel(selectedPlan, User);

            if (selectedPlan.MaxPrice > Money.Zero)
            {
                if (AppSettings.InvestmentPlatform.InvestmentPlatformPlansPolicy == PlansPolicy.OneUpgradedPlan)
                    throw new MsgException(U6012.CANTUPGRADERANGEPLAN);

                targetPrice = Money.Parse(RangePriceTextBox.Text);
                if (!selectedPlan.CheckPlanPrice(targetPrice))                    
                    throw new MsgException(U6012.TYPECORRECTPRICE);
            }

            var bg = new BuyInvestmentPlanButtonGenerator(User, selectedPlan, targetPrice);
            var buttonsText = string.Empty;

            if (CurrentMode == InvestmentPlatformMode.Levels)
            {
                if (selectedPlan.PaymentProcessor == PaymentProcessor.Null)
                    throw new MsgException(U6013.NOPAYMENTPROCESSORSCONNECTEDWITHLEVEL);

                /* TODO - Add options to pay with cryptocurrency(?)
                if(selectedPlan.PaymentProcessor == PaymentProcessor.Coinbase || selectedPlan.PaymentProcessor == PaymentProcessor.CoinPayments)
                    buttonsText = GenerateHTMLButtons.GetPaymentButton(bg, CryptocurrencyType);
                */

                var type = PaymentAccountDetails.GetGatewayType(selectedPlan.PaymentProcessor.ToString());
                buttonsText = GenerateHTMLButtons.GetPaymentButton(bg, type);
            }
            else
                buttonsText = GenerateHTMLButtons.GetPaymentButtons(bg);

            PaymentButtons.Text = buttonsText;

            PaymentProcessorsButtonPlaceHolder.Visible = true;
            BuyControlsPlaceHolder.Visible = false;
        }
        catch (Exception ex)
        {
            ErrorPanel.Visible = true;
            ErrorTextLiteral.Text = ex.Message;
            if (ex is MsgException == false)
                ErrorLogger.Log(ex);
        }
    }

    protected void PlansDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdatePriceLiteral();
    }

    protected void LevelsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var planId = Convert.ToInt32(e.Row.Cells[0].Text);
            var plan = new InvestmentPlatformPlan(planId);
            e.Row.Cells[0].Text = string.Format("L{0}. {1}", plan.Number, plan.Name);

            var amount = Money.Parse(e.Row.Cells[1].Text);
            e.Row.Cells[1].Text = amount.ToString();

            var roi = Convert.ToInt32(e.Row.Cells[2].Text);
            e.Row.Cells[2].Text = (amount * roi / 100).ToString();

            var fee = Money.Parse(e.Row.Cells[3].Text);
            e.Row.Cells[3].Text = fee.ToString();

            try
            {
                var pp = (PaymentProcessor)Convert.ToInt32(e.Row.Cells[6].Text);
                e.Row.Cells[6].Text = pp.ToString();
            }
            catch(Exception ex)
            {
                e.Row.Cells[6].Text = L1.NONE;
            }
        }
    }

    protected void LevelsSqlDataSource_Init(object sender, EventArgs e)
    {
        LevelsSqlDataSource.SelectCommand = string.Format("SELECT * FROM {0} WHERE {1} = {2} ORDER BY {3}",
            InvestmentPlatformPlan.TableName, InvestmentPlatformPlan.Columns.Status, (int)UniversalStatus.Active, InvestmentPlatformPlan.Columns.Number);
    }
}