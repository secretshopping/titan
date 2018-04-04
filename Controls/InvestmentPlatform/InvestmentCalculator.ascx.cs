using Prem.PTC;
using Resources;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Titan.InvestmentPlatform;

public partial class Controls_Misc_InvestmentCalculator : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            if (AppSettings.InvestmentPlatform.InvestmentPlatformEnabled)
                InitControls();        
    }

    protected void InitControls()
    {
        var cache = new InvestmentPlatformPlansCache();
        List<InvestmentPlatformPlan> planTypesList = (List<InvestmentPlatformPlan>)cache.Get();

        InvestmentPlanDropDownList.Items.Clear();
        foreach (var type in planTypesList)
            if (type.Status == UniversalStatus.Active)
                InvestmentPlanDropDownList.Items.Add(new ListItem(type.Name, type.Id.ToString()));

        TitleLiteral.Text = U6008.PROFITABILITY;
        CalculateButton.Text = U6007.CALCULATE;
    }

    protected void CalculateButton_Click(object sender, EventArgs e)
    {
        try
        {
            ResultPlaceHolder.Visible = true;

            var planType = new InvestmentPlatformPlan(int.Parse(InvestmentPlanDropDownList.SelectedValue));
            var dailyEarning = planType.Price * planType.Roi / 100 / planType.Time;

            var daily = dailyEarning.ToString();
            var weekly = (dailyEarning * 7).ToString();
            var monthly = (dailyEarning * 30).ToString();
            var halfYear = (dailyEarning * 180).ToString();
            var total = planType.TotalMinDefaultEarning().ToString();

            if (planType.MaxPrice > Money.Zero)
            {
                var maxDailyEarning = planType.MaxPrice * planType.Roi / 100 / planType.Time;

                daily = string.Format("{0} - {1}", dailyEarning.ToString(), maxDailyEarning.ToString());
                weekly = string.Format("{0} - {1}", (dailyEarning * 7).ToString(), (maxDailyEarning * 7).ToString());
                monthly = string.Format("{0} - {1}", (dailyEarning * 30).ToString(), (maxDailyEarning * 30).ToString());
                halfYear = string.Format("{0} - {1}", (dailyEarning * 180).ToString(), (maxDailyEarning * 180).ToString());
                total = string.Format("{0} - {1}", planType.TotalMinDefaultEarning().ToString(), planType.TotalMaxEarning().ToString());
            }

            DailyEarningsLiteral.Text = daily;
            WeeklyEarningsLiteral.Text = weekly;
            MonthlyEarningsLiteral.Text = monthly;
            HalfYearlyEarningsLiteral.Text = halfYear;
            TotalEarningsLitereal.Text = total;

            if (dailyEarning * 180 > planType.Price * planType.Roi / 100)
                HalfYearTab.Visible = false;
            if (dailyEarning * 30 > planType.Price * planType.Roi / 100)
                MonthlyTab.Visible = false;

            if (TitanFeatures.IsParras2k)
                HalfYearlyEarningsLiteral.Text = planType.Price + " + " + (dailyEarning * 180).ToString();
        }
        catch (Exception ex)
        {
            ResultPlaceHolder.Visible = false;
        }
    }
}