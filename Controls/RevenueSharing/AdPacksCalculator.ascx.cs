using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.UI.WebControls;

public partial class Controls_Misc_AdPacksCalculator : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AppSettings.RevShare.IsRevShareEnabled)        
            return;
        
        if (!Page.IsPostBack)
        {
            if (AppSettings.RevShare.IsRevShareEnabled && AppSettings.RevShare.AdPack.DistributionPolicy == DistributionPolicy.Fixed
                && AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.Classic)
                InitControls();
            else
            {
                ErrorMessagePlaceHolder.Visible = true;
                ErrorMessageLabel.Text = U6007.CALCULATORERROR;
            }
        }
        FailureP.Visible = false;
    }

    protected void InitControls()
    {
        CalculatorMainPlaceHolder.Visible = true;

        var cache = new AdPackTypesCache();
        Dictionary<int, AdPackType> adPacksTypeList = (Dictionary<int, AdPackType>)cache.Get();

        AdPackDropDownList.Items.Clear();
        foreach (var type in adPacksTypeList)
        {
            if(type.Value.Status == Prem.PTC.Advertising.AdPackTypeStatus.Active)
                AdPackDropDownList.Items.Add(new ListItem(type.Value.Name, type.Value.Id.ToString()));
        }

        TitleLiteral.Text = string.Format(U6007.ADPACKCALTULATOR, AppSettings.RevShare.AdPack.AdPackNamePlural);
        CalculateButton.Text = U6007.CALCULATE;
    }

    protected void CalculateButton_Click(object sender, EventArgs e)
    {
        ResultPlaceHolder.Visible = false;

        var adpackType = new AdPackType(int.Parse(AdPackDropDownList.SelectedValue));
        var numberOfPacks = 0;
        DateTime startDate = DateTime.ParseExact(startDateInput.Value, "MM/dd/yyyy", Thread.CurrentThread.CurrentCulture);

        try
        {
            numberOfPacks = Convert.ToInt32(AdPacksCountTextBox.Text);
        }
        catch (Exception ex)
        {
            FailureP.Visible = true;
            FailureText.Text = U4000.BADFORMAT;
            return;
        }

        ResultPlaceHolder.Visible = true;

        var user = Member.CurrentInCache;
        var dailyEarning = adpackType.Price * adpackType.FixedDistributionValuePercent / 100;
        int daysToFinish = 0;

        if (dailyEarning != Money.Zero)
            daysToFinish = ((adpackType.Price * adpackType.PackReturnValuePercentage / 100) / dailyEarning).GetTotalsUp();

        if (AppSettings.RevShare.DistributionTime == DistributionTimePolicy.EveryWeek)
        {
            EarnsLiteral.Text = U6007.WEEKLY;
            dailyEarning *= 7;
        }
        else
        {
            EarnsLiteral.Text = L1.DAILY;
        }

        var adBalanceRepurchase = user.Membership.AdPackAdBalanceReturnPercentage * adpackType.AdBalanceReturnPercentage / 100;
        var adBalanceEarning = dailyEarning * numberOfPacks * adBalanceRepurchase / 100;
        var mainBalanceEarning = (100 - adBalanceRepurchase) * dailyEarning * numberOfPacks / 100;

        FinishDateLiteral.Text = string.Format("{0}: <strong>{1}</strong>", U6007.FINISHDATE, startDate.AddDays(daysToFinish).ToShortDateString());

        EarnsLiteral.Text += string.Format(" {0}: {1}: <strong>{2}</strong>, {3}: <strong>{4}</strong>",
            U4000.EARNINGS, L1.MAINBALANCE, mainBalanceEarning.ToString(), U6012.PURCHASEBALANCE, adBalanceEarning.ToString());

    }
}