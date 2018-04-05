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
using System.Reflection;
using ExtensionMethods;
public partial class About : System.Web.UI.Page
{
    Member User;
    public int dailyRequiredClicks = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        User = Member.Current;
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnAdPacksEnabled && User.IsEarner);

        Form.Action = Request.RawUrl;
        
        var adsViewed = User.RSAPTCAdsViewed.Count;

        string coloredAdsViewed;
        dailyRequiredClicks = User.Membership.AdPackDailyRequiredClicks;

        if (adsViewed < dailyRequiredClicks)
            coloredAdsViewed = string.Format("<span style={0}>{1}</span>", "'color: #cb6f1b; font-weight: bold;'", adsViewed);
        else
            coloredAdsViewed = string.Format("<span style={0}>{1}</span>", "'color: #87a131; font-weight: bold;'", adsViewed);

        if (AppSettings.RevShare.DistributionTime == DistributionTimePolicy.EveryWeek)
        {
            string timeLeft = Prem.PTC.Utils.TimeSpanExtensions.ToFriendlyDisplay(AdPackManager.GetAdSurfDeadline(), 3);
           
            AdsWatchedInfo.Text = string.Format(U6003.ADPACKTIMEDESC, dailyRequiredClicks, coloredAdsViewed, dailyRequiredClicks, timeLeft);
        }
        else
        {
            AdsWatchedInfo.Text = U4200.ADPACKEARNMAINDESCRIPTION.Replace("%n%", dailyRequiredClicks.ToString()) +
                U4200.WATCHEDADPACKSADVERT.Replace("%n%", string.Format("{0}/{1}", coloredAdsViewed, dailyRequiredClicks));
        }
        
        //Checking condition to display appropriate message
        if (Member.IsLogged && !User.CheckAccessCustomizeTradeOwnSystem && User.CommissionToMainBalanceRequiredViewsMessageInt > 0)
            CommissionTransferInfo.Text = String.Format(U6010.COMMISSIONBALANCETRANSFERINFO, User.CommissionToMainBalanceRequiredViewsMessageInt, AppSettings.RevShare.AdPack.AdPackName);
        else if (Member.IsLogged && !User.CheckAccessCustomizeTradeOwnSystem && User.CommissionToMainBalanceRequiredViewsMessageInt == 0)
            CommissionTransferInfo.Text = String.Format(U6010.COMMISSIONBALANCETRANSFERNOACTIVEADPACKINFO, AppSettings.RevShare.AdPack.AdPackName);
        else
            CommissionTransferInfoDiv.Visible = false;


        AdsLiteral.Visible = AppSettings.RevShare.AdPack.IsAdListEnabled;
    }



    protected void AdPackRefreshUpdatePanel_Load(object sender, EventArgs e)
    {
        if (AppSettings.RevShare.AdPack.IsAdListEnabled)
        {
            AdsLiteral.Visible = false;

            AdsLiteral.Controls.Clear();

            //Display proper ads to the Member

            List<AdPacksAdvert> AvailableAdList = AdPackManager.GetAdPacksAdvertsForUsers(User.Membership.AdPackDailyRequiredClicks);

            for (int i = 0; i < AvailableAdList.Count; i++)
            {
                AdsLiteral.Controls.Add(GetAdHTML(AvailableAdList[i], User));
            }

            AdsLiteral.Visible = true;
        }
    }

    protected UserControl GetAdHTML(AdPacksAdvert Ad, Member User)
    {
        bool IsActive = true;

        if (Member.IsLogged)
            IsActive = !User.RSAPTCAdsViewed.Contains(Ad.Id);

        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/Advertisements/AdPacksAdvert.ascx");
        var parsedControl = objControl as IAdPackObjectControl;
        parsedControl.Object = Ad;

        PropertyInfo myProp1 = parsedControl.GetType().GetProperty("IsActive");
        myProp1.SetValue(parsedControl, IsActive, null);

        //parsedControl.
        parsedControl.DataBind();

        return objControl;
    }

    public enum AdsClickedStatus
    {
        INNACTIVE,
        ACTIVE
    }
}
