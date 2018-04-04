using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Prem.PTC.Referrals;
using Titan;
using Resources;

public partial class About : System.Web.UI.Page
{
    public string HashedTrafficAd { get; set; }
    private bool IsCaptchaEnabled = true; //Set to false to disable captcha
    public string targetUrl { get; set; }
    private int StoredAdId
    {
        get
        {
            return (int)Session["_StoredAID"];
        }
        set
        {
            Session["_StoredAID"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.PtcAdverts.IsExternalIFrameEnabled);

        if (TitanFeatures.IsAhmed)
        {
            RedirectToBannersPlaceHolder.Visible = true;
            TokenAdsPlaceHolder.Visible = true;
            TimerStylesPlaceHolder.Visible = true;
        }

        SiteName.Text = AppSettings.Site.Name;
        LangAdder.Add(CustomValidator1, L1.ER_BADCAPTCHA);
        LangAdder.Add(CreditAfterCaptcha, L1.SEND);
        LangAdder.Add(CloseRefreshButton, U4000.CLOSE);

        if (Request.Params.Get("__WATCHEDID") != null)
        {
            //Ad watched, proceed (with captcha)
            BeforePanel.Visible = false;
            string AdId = Request.Params.Get("__WATCHEDID");
            int Id = Int32.Parse(AdId);
            StoredAdId = Id;

            HashedTrafficAd = PTCSercurityManager.HashAd(AdId);

            if (IsCaptchaEnabled && Member.IsLogged)
            {
                AnimatedHeight = 260;
                CaptchaPanel.Visible = true;
                AdInfoContainer.Text = ""; //Disable the timer
            }
            else
            {
                ProcceedWithCredit(Id);
            }
        }
        else if (Request.Params.Get("__EVENTARGUMENT5") != null)
        {
            //Display ad before watch
            string AdId = Request.Params.Get("__EVENTARGUMENT5");
            Form.Action = "user/earn/asurf/asurf.aspx";
            if (!Prem.PTC.Security.SecurityManager.IsWatchingAdCookieSet())
            {
                PtcAdvert ad = new PtcAdvert();
                try
                {
                    ad = new PtcAdvert(Int32.Parse(AdId));
                    
                    string AdTime = ad.DisplayTime.TotalSeconds.ToString(); //get ad time in seconds

                    AdInfoContainer.Text = "<input type=\"hidden\" name=\"__WATCHEDID\" id=\"__WATCHEDID\" value=\"" + AdId + "\" />";
                    AdInfoContainer.Text += "<input type=\"hidden\" id=\"adtime\" value=\"" + AdTime + "\"/>";
                    targetUrl = ad.TargetUrl;
                    Prem.PTC.Security.SecurityManager.SetWatchingAdCookie(Convert.ToInt32(ad.DisplayTime.TotalSeconds));
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                    throw ex;
                }

            }
            else
                MultiplePanel.Visible = true;
        }
        WatchAdButton.OnClientClick = "openedWindow = window.open('" + targetUrl + "','_blank'); startTimer(); hideButton(); return false; ";
    }


    public int AnimatedHeight = 90;
    public static string BaseUrl
    {
        get
        {
            HttpContext context = HttpContext.Current;
            string baseUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + context.Request.ApplicationPath.TrimEnd('/') + '/';
            return baseUrl;
        }
    }

    protected void CreditAfterCaptcha_Click(object sender, EventArgs e)
    {
        RegisterUserValidationSummary.Visible = true;
        if (TitanCaptcha.IsValid)
        {
            //Credit
            ProcceedWithCredit(StoredAdId);
        }
        else
        {
            AnimatedHeight = 260;
        }
    }

    protected void ProcceedWithCredit(int Id)
    {
        //Stats
        var targetStatValue = new Prem.PTC.Statistics.Statistics(Prem.PTC.Statistics.StatisticsType.PTCClicks);
        targetStatValue.AddToData1(1);
        targetStatValue.Save();

        if (Member.IsLogged)
        {
            RegisterUserValidationSummary.Visible = true;
            //Anti-cheat: Check if ad is not already watched
            var User = Member.GetLoggedMember(Context);
            if (User.AdsViewed.Contains(Id))
            {
                DisplayInfo(L1.ALREADYWATCHED, false);
            }
            else
            {
                CaptchaPanel.Visible = false;

                var ad = new PtcAdvert(Id);

                //Use Crediter
                PtcCrediter Crediter = (PtcCrediter)CrediterFactory.Acquire(User, Titan.CreditType.PTC);
                Money Calculated = Crediter.CreditMember(ad);

                DisplayInfo(L1.YOUVEBEENCREDITED + " <b>" + Calculated.ToString() + "</b> " +
                    L1.AND + " <b>" + User.Membership.AdvertPointsEarnings.ToString() + "</b> " + AppSettings.PointsName + " " + L1.FORTHISAD, true);

                //Display "Feel free with Ad URL" text
                FeelFreeLiteral.Text = ": <a href=\"" + ad.TargetUrl + "\" target=\"_blank\">" + ad.TargetUrl + "</a>";

                //Force notifiaction refresh (Ads)
                NotificationManager.RefreshWithMember(NotificationType.NewAds, User);

                //Modify the ad info
                ad.Click();
                if (ad.ShouldBeFinished)
                {
                    ad.Status = AdvertStatus.Finished;
                    ad.Save();
                }
                else
                    ad.SaveClicks();

                AnimatedHeight = 90;
            }
        }
        else
        {
            //Non-logged in member
            //Just display 'you must login' message
            DisplayInfo(L1.YOUMUSTBELOGGED, false);
        }
        AdInfoContainer.Text = "";
    }

    protected void Validate_Captcha(object source, ServerValidateEventArgs args)
    {
        if (TitanCaptcha.IsValid)
        {
            args.IsValid = true;
        }
        else
        {
            args.IsValid = false;
        }
    }

    protected void DisplayInfo(string infoText, bool isSuccess)
    {
        AfterCounterPanel.Visible = true;
        InfoText.Text = infoText;

        if (isSuccess)
            StatusImage.ImageUrl = "~/Images/Misc/ok2mini.png";
        else
            StatusImage.ImageUrl = "~/Images/Misc/fail2mini.png";
    }
}
