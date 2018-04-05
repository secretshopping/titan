using System;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Advertising;
using Prem.PTC.Achievements;
using Resources;
using Prem.PTC.Members;
using Titan.MiniVideos;
using Titan.CryptocurrencyPlatform;
using Titan.InvestmentPlatform;

public class HtmlCreator
{
    //HTML Charts Colors

    public static string ChartColor1 = AppSettings.Site.MainColor.Replace("#", "");

    public static string ChartBackgroundColor = "f8f8f7";
    public static string InnerChartBgColor = "ffffff";
    public static string InnerChartBorderColor = "dfe0dd";
    public static string DotBorderColor = AppSettings.Site.DarkColor.Replace("#", "");
    public static string ChartBackgroundColorInRefTable = "ffffff";
    public static string ChartBaseFontColor = "adadad";

    /// <summary>
    /// Generates menu (top/user) notification numbers. E.g. Earn (3)
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string GenerateMenuNotificationNumber(int number)
    {
        if (number != 0)
        {
            StringBuilder finalcode = new StringBuilder("<span class=\"badge pull-right bg-green m-l-5 m-r-5\">");
            finalcode.Append(number);
            finalcode.Append("</span>");
            return finalcode.ToString();
        }
        return "";
    }

    public static string GenerateMenuNotificationNumber_Important(int number)
    {
        if (number != 0)
        {
            StringBuilder finalcode = new StringBuilder("<span class=\"badge pull-right bg-red m-l-5 m-r-5\">");
            finalcode.Append(number);
            finalcode.Append("</span>");
            return finalcode.ToString();
        }
        return "";
    }
    /// <summary>
    /// Genrates Avatar + colored username pair
    /// Remember: if you cahnge it change also function below
    /// </summary>
    /// <param name="avatarUrl">Can be a relative path</param>
    /// <param name="nameColor"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string CreateAvatarPlusUsername(Member user)
    {
        StringBuilder finalcode = new StringBuilder();
        finalcode.Append("<img class=\"globavatar\" src=\"")
                 .Append((HttpContext.Current.Handler as Page).ResolveUrl(user.AvatarUrl))
                 .Append("\" style=\"width:20px;height:20px;\" /><span style=\"color:")
                 .Append(user.Color)
                 .Append(";\">");

        if (!TitanFeatures.IsClickmyad)
        {
            finalcode.Append("<a href = '")
                     .Append(GetProfileURL(user))
                     .Append("'>")
                     .Append(user.Name)
                     .Append("</a>");
        }
        else
        {
            finalcode.Append(user.Name);
        }

        finalcode.Append("</span>");

        return finalcode.ToString();
    }

    /// <summary>
    /// Gets pure username from Avatar + colored username pair
    /// </summary>
    /// <param name="pair"></param>
    /// <returns></returns>
    public static string GetUsernameFromAvatarPair(string pair)
    {
        string userName = pair;
        userName = userName.Substring(userName.IndexOf("\">") + 2);
        userName = userName.Substring(0, userName.Length - 7);
        return userName;
    }

    /// <summary>
    /// Converts relative path to full URL. E.g. ~/asd to http://site.com/asd
    /// If doesn't start with ~/ it does nothing
    /// </summary>
    /// <param name="relativePath">Must start with ~/</param>
    /// <returns></returns>
    public static string ConvertToFullUrl(string relativePath)
    {
        if (relativePath.StartsWith("~/"))
        {
            var url = HttpContext.Current.Request.Url;
            var port = url.Port != 80 ? (":" + url.Port) : String.Empty;

            return String.Format("{0}://{1}{2}{3}",
                   url.Scheme, url.Host, port, VirtualPathUtility.ToAbsolute(relativePath));
        }
        else
            return relativePath;
    }

    public static string TrafficExchangeGetAdCode(Money UserEarnedMoney, Money RentedRefEarnings, TimeSpan DisplayTime)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("<div class=\"Abox\"");
        sb.Append(" title=\"");
        sb.Append(L1.BYVIEWINGEARN + " (exchangable for more Traffic Exchange credits)");
        sb.Append(":<br/> <div class=ABlist>&bull; ");
        sb.Append("<b>" + UserEarnedMoney.ToString() + "</b>");
        sb.Append("<br/>&bull; ");
        sb.Append("<b>" + RentedRefEarnings.ToString() + "</b> " + L1.FROMRR);
        sb.Append("<br/></div><br/>");
        sb.Append(L1.THISADLASTS + " ");
        sb.Append(DisplayTime.Seconds);
        sb.Append(" " + L1.SECONDS + "\"");

        return sb.ToString();
    }

    public static string GetColoredAVGValue(double avg)
    {
        return avg.ToString("0.0");
    }

    public static string GenerateRentRefBox(int refCount, Money price, bool isActive, int PACKID)
    {
        var sb = new StringBuilder();
        string class1 = isActive ? "HAbox col-md-3 col-sm-6" : "HAboxclicked col-md-3 col-sm-6";
        string class2 = isActive ? "ABtitle" : "ABtitleclicked";
        string class3 = isActive ? "" : "desaturated";
        string insert = isActive ? "onclick=\"buyPack(" + PACKID.ToString() + ");\"" : "";

        sb.Append("<div class=\"" + class1 + "\" " + insert + ">")
          .Append("<div class=\"" + class2 + "\">" + refCount + " ")
          .Append(L1.REFERRALS)
          .Append("</div><div class=\"ABinfo\">")
          .Append(price.ToString())
          .Append("<img src=\"")
          .Append(AppSettings.Site.FaviconImageURL)
          .Append("\" class=\"")
          .Append(class3)
          .Append("\" /></div></div>");

        return sb.ToString();
    }

    public static string GetColoredStatus(MiniVideoStatus status)
    {

        if (status == MiniVideoStatus.Active)
            return "<span style=\"color:#519a06\">" + L1.ACTIVE + "</span>";

        if (status == MiniVideoStatus.Paused)
            return "<span style=\"color:#CC9933\">" + L1.PAUSED + "</span>";

        if (status == MiniVideoStatus.Finished)
            return "<span style=\"color:#660099\">" + L1.FINISHED + "</span>";

        if (status == MiniVideoStatus.Deleted)
            return "<span style=\"color:#FF0000\">" + U6000.DELETE + "</span>";

        return ""; //for compilator
    }

    public static string GetColoredStatus(Prem.PTC.Advertising.AdvertStatus status)
    {

        if (status == Prem.PTC.Advertising.AdvertStatus.Active)
            return "<span class=\"text-success\">" + L1.ACTIVE + "</span>";

        if (status == Prem.PTC.Advertising.AdvertStatus.Paused)
            return "<span class=\"text-warning\">" + L1.PAUSED + "</span>";

        if (status == Prem.PTC.Advertising.AdvertStatus.Stopped)
            return "<span class=\"text-danger\">" + L1.STOPPED + "</span>";

        if (status == Prem.PTC.Advertising.AdvertStatus.Rejected)
            return "<span class=\"text-danger\">" + L1.REJECTED + "</span>";

        if (status == Prem.PTC.Advertising.AdvertStatus.Finished)
            return "<span class=\"text-danger\">" + L1.FINISHED + "</span>";

        if (status == Prem.PTC.Advertising.AdvertStatus.WaitingForAcceptance)
            return "<span class=\"text-warning\">" + L1.AWAITINGAPPROVAL + "</span>";

        return ""; //for compilator
    }

    public static string GetColoredStatus(MarketplaceIPNStatus status)
    {
        if (status == MarketplaceIPNStatus.Confirmed)
            return "<span class=\"text-success\">" + U5006.CONFIRMED + "</span>";

        if (status == MarketplaceIPNStatus.Pending)
            return "<span class=\"text-warning\">" + L1.PENDING + "</span>";

        return ""; //for compilator
    }

    public static string GetColoredStatus(Titan.OfferwallStatus Status)
    {
        if (Status == Titan.OfferwallStatus.Active)
            return "<span class=\"text-success\">" + L1.ACTIVE + "</span>";

        if (Status == Titan.OfferwallStatus.Paused)
            return "<span class=\"text-warning\">" + L1.PAUSED + "</span>";

        return ""; //for compilator
    }

    public static string GetColoredStatus(Titan.NetworkStatus Status)
    {
        if (Status == Titan.NetworkStatus.Active)
            return "<span class=\"text-success\">" + L1.ACTIVE + "</span>";

        if (Status == Titan.NetworkStatus.Paused)
            return "<span class=\"text-warning\">" + L1.PAUSED + "</span>";

        return ""; //for compilator
    }

    public static string GetColoredStatus(UniversalStatus Status)
    {
        if (Status == UniversalStatus.Active)
            return "<span class=\"text-success\">" + L1.ACTIVE + "</span>";

        if (Status == UniversalStatus.Paused)
            return "<span class=\"text-warning\">" + L1.PAUSED + "</span>";

        return ""; //for compilator
    }

    public static string GetColoredStatus(BaseStatus Status)
    {
        if (Status == BaseStatus.Active)
            return "<span class=\"text-success\">" + L1.ACTIVE + "</span>";

        if (Status == BaseStatus.Hidden)
            return "<span class=\"text-warning\">Hidden</span>";

        return ""; //for compilator
    }

    public static string GetColoredStatus(MemberStatus Status)
    {
        if (Status == MemberStatus.Active)
            return "<span class=\"text-success\">" + Status.ToString() + "</span>";

        if (Status == MemberStatus.AwaitingSMSPIN || Status == MemberStatus.VacationMode)
            return "<span class=\"text-warning\">" + Status.ToString() + "</span>";

        if (Status == MemberStatus.Cancelled || Status == MemberStatus.Expired || Status == MemberStatus.Inactive)
            return "<span class=\"text-danger\">" + Status.ToString() + "</span>";

        return "<span class=\"text-danger\">" + Status.ToString() + "</span>";
    }

    public static string GetColoredStatus(CryptocurrencyOfferStatus status)
    {
        if (status == CryptocurrencyOfferStatus.Active)
            return "<span style=\"color:#519a06\">" + L1.ACTIVE + "</span>";

        if (status == CryptocurrencyOfferStatus.Paused)
            return "<span style=\"color:#CC9933\">" + L1.PAUSED + "</span>";

        if (status == CryptocurrencyOfferStatus.Deleted)
            return "<span style=\"color:#CC0000\">" + U6010.REMOVED + "</span>";

        if (status == CryptocurrencyOfferStatus.Finished)
            return "<span style=\"color:#660099\">" + L1.FINISHED + "</span>";

        return ""; //for compilator       
    }

    public static string GetColoredStatus(PublishersWebsiteStatus Status)
    {
        if (Status == PublishersWebsiteStatus.Accepted)
            return "<span class=\"text-success\">" + Status.ToString() + "</span>";

        else if (Status == PublishersWebsiteStatus.Pending)
            return "<span class=\"text-warning\">" + Status.ToString() + "</span>";

        else //(Status == PublishersWebsiteStatus.Rejected)
            return "<span class=\"text-danger\">" + Status.ToString() + "</span>";
    }

    public static string GetColoredStatus(PlanStatus status)
    {
        if (status == PlanStatus.Active)
            return "<span style=\"color:#519a06\">" + L1.ACTIVE + "</span>";

        if (status == PlanStatus.Paused)
            return "<span style=\"color:#CC9933\">" + L1.PAUSED + "</span>";

        if (status == PlanStatus.Removed)
            return "<span style=\"color:#CC0000\">" + U6010.REMOVED + "</span>";

        if (status == PlanStatus.Finished)
            return "<span style=\"color:#660099\">" + L1.FINISHED + "</span>";

        return ""; //for compilator       
    }

    public static string GetColoredStatus(TicketStatus status)
    {
        if (status == TicketStatus.Finished)
            return "<span style=\"color:#519a06\">" + U6012.RECEIVED + "</span>";

        if (status == TicketStatus.ReceivingMoney)
            return "<span style=\"color:#660099\">" + U6012.DEPOSITING + "</span>";

        if (status == TicketStatus.WaitingInQueue)
            return "<span style=\"color:#CC9933\">" + U6012.WAITING + "</span>";        

        return ""; //for compilator       
    }

    public static string GetColoredTransactionStatus(CryptocurrencyTransactionStatus status)
    {
        if (status == CryptocurrencyTransactionStatus.AwaitingPayment)
            return "<span style=\"color:red\">" + U6010.AWAITINGPAYMENTCONFIRM + "</span>";

        if (status == CryptocurrencyTransactionStatus.AwaitingPaymentConfirmation)
            return "<span style=\"color:orange\">" + U6010.AWAITINGPAYMENTRECEIVEDCONFIRM + "</span>";

        if (status == CryptocurrencyTransactionStatus.Finished)
            return "<span style=\"color:green\">" + L1.FINISHED + "</span>";

        if (status == CryptocurrencyTransactionStatus.NotPaid)
            return "<span style=\"color:red\"><b>" + U6010.BUYERDIDNTPAID + "</b></span>";

        if (status == CryptocurrencyTransactionStatus.PaymentNotConfirmed)
            return "<span style=\"color:red\"><b>" + U6010.SELLERDIDNTRECEIVEPAYMENT + "</b></span>";

        return ""; //for compilator       
    }

    public static string GetColoredTime(int Minutes)
    {
        String Color = "green";
        if (Minutes <= 15)
            Color = "red";
        else if(Minutes <= 30)
        {
            Color = "orange";

            if(TitanFeatures.IsRofriqueWorkMines)
                Color = "#348fe2";
        }
            

        return String.Format("<span style=\"color:{0}\"> {1} {2}</span>", Color, Minutes, U4200.MINUTES);
    }

    public static string GetCheckboxCheckedImage()
    {
        return "<span class=\"fa fa-check\"></span>";
    }

    public static string GetCheckboxUncheckedImage()
    {
        return "&nbsp;";
    }

    public static string GenerateAllAcheivementHTML(Achievement achiv)
    {
        return "<span class=\"trofeum\" title=\"" + achiv.Name + ". " + achiv.GetText() + ". "
        + L1.POINTSREWARD + ": " + achiv.Points + "\" style=\"padding:0 10px 0 10px\"><img src=\"Images/Achievements/onebit_" +
        achiv.ImageId.ToString() + ".png\" style=\"width:16px;height:16px;\" class=\"imagegrey\" /></span>";
    }

    public static string GenerateUserAcheivementHTML(Achievement achiv, bool IsForumAchievement = false)
    {
        string dimensions = "width:32px;height:32px";
        string style = "padding:0 10px 0 10px;";
        if (IsForumAchievement)
        {
            dimensions = "width:13px;height:13px";
            style = "padding:0 4px 0 0;";
        }

        return "<span class=\"trofeum\" title=\"" + achiv.Name + ". " + achiv.GetText() + ". "
        + L1.POINTSREWARD + ": " + achiv.Points + "\" style=\"" + style + "\"><img src=\"Images/Achievements/onebit_" +
        achiv.ImageId.ToString() + ".png\" style=\"" + dimensions + ";\"/></span>";
    }

    public static string GenerateSmallAcheivementHTML(Achievement achiv)
    {
        return "<img src=\"Images/Achievements/onebit_" +
        achiv.ImageId.ToString() + ".png\" style=\"width:13px;height:13px;padding-right:3px\"/>";
    }

    public static string GenerateAdProgressHTML(Advert Ad, String ProgressStatus = "")
    {
        string progress = "";

        switch (Ad.Ends.EndMode)
        {
            case End.Mode.Clicks:
                progress = Convert.ToString(Ad.Clicks);
                break;
            case End.Mode.Days:
                var interval = Ad.ActiveTime;
                string formatedInterval = string.Empty;
                if (interval.Days > 0)
                    formatedInterval = Convert.ToInt32(interval.Days).ToString();
                else if (interval.Hours > 0)
                    formatedInterval = Convert.ToInt32(interval.Hours).ToString() + "h";
                else if (interval.TotalMinutes > 0)
                    formatedInterval = Convert.ToInt32(interval.TotalMinutes).ToString() + "m";

                progress = formatedInterval;
                break;
            case End.Mode.Endless:
            case End.Mode.Null:
                progress = "0";
                break;
        }

        string tempText = Ad.ProgressInPercent + "% (" + progress + "/" + Ad.Ends.ToString() + ")";

        string result = @"<div class=""progress m-b-0 " + ProgressStatus + @""" style=""width: 100%;"">
               <div class=""progress-bar progress-bar-info"" style=""width:" + Ad.ProgressInPercent + @"%;"">
                   <span class=""text-center;"">
                   " + tempText + @"                                               
                   </span>
               </div>
           </div>";

        return result;
    }

    public static string GenerateCPAAdProgressHTML(int ActualClick, int TargetClicks, string text = "")
    {
        if (string.IsNullOrWhiteSpace(text))
            text = U4200.ACTIONS;

        int Percent = 0;
        try
        {
            Percent = Convert.ToInt32(((double)ActualClick / (double)TargetClicks) * 100);
            if (Percent > 100) Percent = 100;
        }
        catch (Exception ex) { }

        string tempText = Percent + "% (" + ActualClick + "/" + TargetClicks + " " + text + ")";

        string result = @"<div class=""progress m-b-0"" style =""width: 100%;"">
               <div class=""progress-bar progress-bar-info"" style=""width:" + Percent + @"%;"">
                   <span class=""text: center;"">
                   " + tempText + @"                                               
                   </span>
               </div>
           </div>";

        return result;
    }

    public static string GenerateCPAAdProgressHTML(decimal ActualClick, decimal TargetClicks, string text = "")
    {
        if (string.IsNullOrWhiteSpace(text))
            text = U4200.ACTIONS;

        return GenerateProgressHTML(0, ActualClick, TargetClicks, text);
    }

    public static string GenerateProgressHTML(decimal start, decimal current, decimal end, string text = "")
    {
        decimal Percent = 0;
        try
        {
            if (end == start) Percent = 100;
            Percent = (((current - start) / (end - start)) * 100);
            if (Percent > 100) Percent = 100;
        }
        catch (Exception ex) { }

        string tempText = Math.Floor(Percent) + "% (" + current + "/" + end + " " + text + ")";

        string result = @"<div class=""progress m-b-0"" style =""width: 100%;"">
               <div class=""progress-bar progress-bar-info"" style=""width:" + Math.Floor(Percent) + @"%;"">
                   <span class=""text: center;"">
                   " + tempText + @"                                               
                   </span>
               </div>
           </div>";

        return result;
    }

    public static string GetProfileURL(int userId, string username)
    {
        if (!AppSettings.TitanFeatures.SocialNetworkEnabled)
            return AppSettings.Site.Url + "sites/profile.aspx?u=" + username;

        return AppSettings.Site.Url + "user/network/profile.aspx?userId=" + userId;
    }

    public static string GetProfileURL(Member user)
    {
        return GetProfileURL(user.Id, user.Name);
    }

    [Obsolete]
    public static string GPTModeHideHTML
    {
        get
        {
            if (Prem.PTC.AppSettings.Site.Mode == WebsiteMode.GPT)
                return "style=\"display:none;\"";
            return "";
        }
    }

    [Obsolete]
    public static string TEModeHideHTML
    {
        get
        {
            if (Prem.PTC.AppSettings.Site.Mode == WebsiteMode.TrafficExchange)
                return "style=\"display:none;\"";
            return "";
        }
    }

}