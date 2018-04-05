using Prem.PTC;
using Resources;
using System;
using Titan.InvestmentPlatform;

public partial class Controls_InvestmentPlatform_InvestmentPlanDetails : System.Web.UI.UserControl, IInvestmentPlanObjectControl
{
    public InvestmentPlatformPlan PlatformPlan { get; set; }
    public InvestmentUsersPlans UserPlan { get; set; }
    public bool IncludedUsersEarning { get; set; }

    public string Name
    {
        get { return PlatformPlan.Name; }
    }

    public string Color
    {
        get { return PlatformPlan.Color; }
        set { PlatformPlan.Color = value; }
    }

    private string price;
    public string Price
    {
        get
        {
            if (string.IsNullOrEmpty(price))
            {
                if (PlatformPlan.MaxPrice == Money.Zero)
                    return PlatformPlan.Price.ToString();
                else
                    return string.Format("{0} - {1}", PlatformPlan.Price.ToString(), PlatformPlan.MaxPrice.ToString());
            }
            else
                return price;
        }
        set
        {
            price = value;
        }
    }

    public string ROI
    {
        get { return NumberUtils.FormatPercents(PlatformPlan.Roi); }
    }

    public string RepurchaseTime
    {
        get { return PlatformPlan.Time.ToString(); }
    }

    public string DailyLimit
    {
        get { return PlatformPlan.DailyLimit.ToString(); }
    }

    public string MonthlyLimit
    {
        get { return PlatformPlan.MonthlyLimit.ToString(); }
    }

    public string BinaryEarning
    {
        get { return NumberUtils.FormatPercents(PlatformPlan.BinaryEarning); }
    }

    public string EarningDelay
    {
        get { return PlatformPlan.EarningDaysDelay.ToString(); }
    }

    public string DailyEarning
    {
        get
        {
            try
            {
                if (PlatformPlan.MaxPrice != Money.Zero)
                {
                    var min = PlatformPlan.Price * PlatformPlan.Roi / 100 / PlatformPlan.Time;
                    var max = PlatformPlan.MaxPrice * PlatformPlan.Roi / 100 / PlatformPlan.Time;

                    return string.Format("{0} - {1}", min.ToString(), max.ToString());
                }

                return (PlatformPlan.Price * PlatformPlan.Roi / 100 / PlatformPlan.Time).ToString();
            }
            catch(Exception e)
            {
                return Money.Zero.ToString();
            }
        }
    }

    public string MonthlyEarning
    {
        get
        {
            try
            {
                if(PlatformPlan.MaxPrice != Money.Zero)
                {
                    var min = PlatformPlan.Price * PlatformPlan.Roi / 100 / PlatformPlan.Time * 30;
                    var max = PlatformPlan.MaxPrice * PlatformPlan.Roi / 100 / PlatformPlan.Time * 30;

                    return string.Format("{0} - {1}", min.ToString(), max.ToString());
                }

                return (PlatformPlan.Price * PlatformPlan.Roi / 100 / PlatformPlan.Time * 30).ToString();
            }
            catch (Exception e)
            {
                return Money.Zero.ToString();
            }
        }
    }    

    public string TotalEarning
    {
        get
        {
            if (PlatformPlan.MaxPrice != Money.Zero)
            {
                var min = PlatformPlan.TotalMinDefaultEarning();
                var max = PlatformPlan.TotalMaxEarning();

                return string.Format("{0} - {1}", min.ToString(), max.ToString());
            }

            return PlatformPlan.TotalMinDefaultEarning().ToString();
        }
    }

    public Money MoneyInSystem
    {
        get { return UserPlan.MoneyInSystem; }
    }

    public override void DataBind()
    {
        base.DataBind();

        var limitsEnabled = AppSettings.InvestmentPlatform.InvestmentPlatformDailyLimitsEnabled;

        if (IncludedUsersEarning)
        {
            ProgressBarContainer.Visible = true;
            LabelProgressBar.Text = HtmlCreator.GenerateProgressHTML(0.0m, UserPlan.MoneyReturned.ToDecimal(), UserPlan.MoneyToReturn.ToDecimal());
            LabelProgressBarDescription.Text = string.Format("{0}/{1}", U4200.MONEYRETURNED, U6006.MONEYTORETURN);
            LabelProgressBarValues.Text = string.Format("{0}/{1}", UserPlan.MoneyReturned.ToString(), UserPlan.MoneyToReturn.ToString());

            if (AppSettings.InvestmentPlatform.InvestmentPlatformPlansPolicy == PlansPolicy.UnlimitedPlans)
                DescriptionTable.Visible = false;

            MoneyInSystemLabel.Text = string.Format("{0}: {1}", U6006.MONEYINSYSTEM, UserPlan.MoneyInSystem);

            WithdrawMoneyFromSystem.Attributes.Add("data-moneyinsystem", UserPlan.MoneyInSystem.ToClearString());
            var isBonus = UserPlan.LastWithdrawalDate == null && PlatformPlan.EndBonus > Money.Zero;
            WithdrawMoneyFromSystem.Attributes.Add("data-isbonusavaible", isBonus.ToString());
            WithdrawMoneyFromSystem.Visible = !limitsEnabled;
            WithdrawMoneyFromSystem.Text = L1.TRANSFERMONEY;

            InvPlanPlaceHolder.CssClass = "bordered";

            if (UserPlan.MoneyInSystem == Money.Zero)
                WithdrawMoneyFromSystem.Style.Add("visibility", "hidden");

            price = UserPlan.Price.ToString();
        }

        LimitsPlaceHolder.Visible = limitsEnabled;
        EarningsPlaceHolder.Visible = !limitsEnabled;        

        EarningDelayTab.Visible = !(EarningDelay == "0");
        BonusTab.Visible = PlatformPlan.EndBonus > Money.Zero;

        switch (AppSettings.InvestmentPlatform.InvestmentPlatformSpeedUpOption)
        {
            case SpeedUpOptions.None:
                CreditingEarningTab.Visible = false;
                break;
            case SpeedUpOptions.Matrix:
                CreditingTextLabel.Text = U6006.BINARYEARNING;                
                break;
            case SpeedUpOptions.Referrals:
                CreditingTextLabel.Text = U6010.REFERRALEARNING;
                break;
        }
        
        HeaderPrice.Visible = !TitanFeatures.IsRetireYoung;
        InvestmentTab.Visible = TitanFeatures.IsRetireYoung;        

        ErrorPanel.Visible = false;
        SuccessPanel.Visible = false;
    }

    protected void WithdrawMoneyFromSystem_Click(object sender, System.EventArgs e)
    {
        if (MoneyInSystem == Money.Zero)
            return;

        try
        {
            UserPlan.TryToWidthdrawlMoneyFromSystem();

            //SUCCESS
            WithdrawMoneyFromSystem.Visible = false;
            MoneyInSystemLabel.Text = string.Format("{0}: {1}", U6006.MONEYINSYSTEM, MoneyInSystem);

            SuccessPanel.Visible = true;
            SuccessTextLiteral.Text = U3501.TRANSFEROK;
        }
        catch (MsgException ex)
        {
            ErrorPanel.Visible = true;
            ErrorTextLiteral.Text = ex.Message;
        }
    }
}