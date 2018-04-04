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
using Resources;
using Titan;
using System.Net;
using System.Reflection;
using Prem.PTC.Utils;
using Titan.PaidToPromote;
using MarchewkaOne.Titan.Balances;

public partial class About : System.Web.UI.Page
{
    #region Settings & Helpers
    private const string CanLoadAutoAdFlag = "CanLoadAutoAd";

    private int ReferrerId { get; set; }
    public string HashedTrafficAd { get; set; }
    protected WindowOpener windowOpener { get; set; }
    private bool IsCaptchaEnabled { get; set; }
    private bool CaptchaAnswer { get; set; }
    private string AdsCreatorUsername;
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

    public string HashedURL
    {
        get
        {
            return (string)Session["_HashedURL"];
        }
        set
        {
            Session["_HashedURL"] = value;
        }
    }

    private bool CanLoadAutoAd
    {
        get
        {
            if (ViewState[CanLoadAutoAdFlag] == null)
                return true;

            return (bool)ViewState[CanLoadAutoAdFlag];
        }
        set
        {
            ViewState[CanLoadAutoAdFlag] = value;
        }
    }

    //For dynamic captcha support in Update Panel
    public string GoogleJS { get; set; }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        AutoUrl.Value = string.Empty;

        #region CheckOpener
        //This page can be opened by different pages
        //Content differs based on opener 

        if (Request.Params.Get("f") != null)
        {
            if (Request.Params.Get("f") == "0")
            {
                windowOpener = WindowOpener.TrafficGrid;
                AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnTrafficGridEnabled);
            }
            else if (Request.Params.Get("f") == "1")
            {
                windowOpener = WindowOpener.PTC;
                AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnAdsEnabled);
            }
            else if (Request.Params.Get("f") == "2")
            {
                windowOpener = WindowOpener.AdPack;
                AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnAdPacksEnabled);
            }
            else if (Request.Params.Get("f") == "3")
                windowOpener = WindowOpener.URLChecker;
            else if (Request.Params.Get("f") == "5")
                windowOpener = WindowOpener.LoginAd;
            else if (Request.Params.Get("f") == "6")
            {
                //Ahmed's users don't have access to this page (only daily task allowed)
                if (TitanFeatures.IsAhmed && Member.CurrentInCache.NumberOfWatchedTrafficAdsToday >= AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin)
                    Response.Redirect("~/user/default.aspx");

                windowOpener = WindowOpener.TrafficExchange;
                AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnTrafficExchangeEnabled);
            }
        }
        else if (Request.Params.Get("ref") != null)
        {
            windowOpener = WindowOpener.PaidToPromote;
            //AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnPaidToPromoteEnabled);
            ReferrerId = Convert.ToInt32(Request.Params.Get("ref"));
        }
        else
            Response.Redirect("~/default.aspx");

        #endregion

        if (windowOpener == WindowOpener.PTC || windowOpener == WindowOpener.AdPack ||
            windowOpener == WindowOpener.LoginAd || windowOpener == WindowOpener.TrafficExchange || windowOpener == WindowOpener.PaidToPromote)
            IsCaptchaEnabled = true; //Set to false to disable captcha

        if (windowOpener == WindowOpener.PTC && (AppSettings.Site.TrafficExchangeMod || AppSettings.PtcAdverts.IsExternalIFrameEnabled))
            Response.Redirect("~/user/default.aspx");

        SiteName.Text = AppSettings.Site.Name;
        LangAdder.Add(CustomValidator1, L1.ER_BADCAPTCHA);
        LangAdder.Add(CreditAfterCaptcha, L1.SEND);
        LangAdder.Add(CloseRefreshButton, U4000.CLOSE);
        LangAdder.Add(AdCloseRefreshButton, U4000.CLOSE);
        LangAdder.Add(CheckerCloseRefreshButton, U4200.URLCHECKCLOSE);

        if (Request.Params.Get("__WATCHEDID") != null)
        {
            string AdId = Request.Params.Get("__WATCHEDID");
            HashedTrafficAd = PTCSercurityManager.HashAd(AdId);
            int Id = Int32.Parse(AdId);
            StoredAdId = Id;
            var ad = (windowOpener == WindowOpener.TrafficGrid) ? new TrafficGridAdvert(Convert.ToInt32(AdId)) : null;
            //Ad watched, proceed
            BeforePanel.Visible = false;

            //Anti-cheat: Check if system is ON and member didnt reach his hits limit
            //VERIFY

            #region TrafficGrid
            if (windowOpener == WindowOpener.TrafficGrid)
            {
                Member User;
                if (windowOpener != WindowOpener.LoginAd && Member.IsLogged)
                    User = Member.Current;
                else
                    User = new Member();

                if (User.TrafficGridHitsToday >= User.Membership.TrafficGridTrials || !TrafficGridManager.IsOn)
                    ErrorPanel.Visible = true;
                else
                {
                    CreditedPanel.Visible = true;
                    User.TrafficGridHitsToday++;
                    User.SaveTrafficGridData();

                    try
                    {
                        TrafficGridManager.GetPrizeAndSaveIt(User);
                    }
                    catch (MsgException ex)
                    {
                        TextInformation.Text = ex.Message.Replace("[OK]", "");

                        if (ex.Message.Contains("[OK]"))
                        {
                            TextInformation.Text = Resources.L1.YOUWON + " " + TextInformation.Text;

                            User.TrafficGridTotalWons++;
                            User.SaveTrafficGridData();

                            bool ToSave = User.TryToAddAchievements(
                        Prem.PTC.Achievements.Achievement.GetProperAchievements(
                        Prem.PTC.Achievements.AchievementType.AfterWinningInTrafficGrid, Convert.ToInt32(User.TrafficGridTotalWons)));

                            if (ToSave)
                                User.Save();

                        }

                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex);
                        throw ex;
                    }

                    //Modify the ad info
                    ad.Click();
                    if (ad.ShouldBeFinished)
                    {
                        ad.Status = AdvertStatus.Finished;
                        ad.Save();
                    }
                    else
                        ad.SaveClicks();
                }
                AdInfoContainer.Text = "";
            }
            #endregion
            else if (windowOpener == WindowOpener.PTC || windowOpener == WindowOpener.AdPack || windowOpener == WindowOpener.LoginAd
                || windowOpener == WindowOpener.TrafficExchange)
            {

                if (IsCaptchaEnabled && Member.IsLogged)
                {
                    if (!AppSettings.PtcAdverts.FeedbackCaptchaEnabled)
                        CaptchaPanel.Visible = true;
                    else
                        FeedbackCaptchaPanel.Visible = true;

                    AdInfoContainer.Text = ""; //Disable the timer

                    if (windowOpener == WindowOpener.TrafficExchange)
                    {
                        //Display queued ads
                        //ThumbnailsLiteral.Controls.Clear();
                        //LoadTrafficExchangeThumbnails();
                    }
                    else if (windowOpener == WindowOpener.AdPack)
                    {
                        //Display queued ads
                        ThumbnailsLiteral.Controls.Clear();
                        LoadAdPackThumbnails();
                    }
                    else if (windowOpener == WindowOpener.PTC)
                    {
                        //Display queued ads
                        ThumbnailsLiteral.Controls.Clear();
                        if (Request.QueryString["auto"] != null)
                            LoadAdThumbnails();
                    }
                }
                else
                {
                    if (windowOpener == WindowOpener.PTC)
                        ProcceedWithAdCredit(Id);
                    else if (windowOpener == WindowOpener.AdPack)
                        ProcceedWithAdPackCredit(Id);
                    else if (windowOpener == WindowOpener.LoginAd)
                        ProcceedWithLoginAdCredit(Id);
                    else if (windowOpener == WindowOpener.TrafficExchange)
                        ProcceedWithTrafficExchangeCredit(Id);
                }
            }
            else if (windowOpener == WindowOpener.PaidToPromote)
            {
                if (IsCaptchaEnabled)
                {
                    CaptchaPanel.Visible = true;
                    AdInfoContainer.Text = ""; //Disable the timer                    
                }
                else
                    ProcceedWithPaidToPromoteAdCredit(Id);
            }
        }
        #region TrafficGrid
        else if (windowOpener == WindowOpener.TrafficGrid && Request.Params.Get("__EVENTARGUMENT5") != null && Request.Params.Get("__EVENTARGUMENT5") == "YES" && TrafficGridManager.IsOn)
        {
            //Display ad before watch
            string AdId = Request.Params.Get("__EVENTARGUMENT5");
            Form.Action = "user/earn/surf.aspx?f=0";

            if (!PTCSercurityManager.IsLocked)
            {
                try
                {
                    var ad = TrafficGridManager.GetRandomAdvertisement();

                    AdsCreatorUsername = SetAdCreatorUsername(ad.CreatorUsername);

                    int AdTime = Convert.ToInt32(ad.DisplayTime.TotalSeconds);
                    int FixedTime = Convert.ToInt32(AdTime * ((double)Member.CurrentInCache.Membership.TrafficGridShorterAd / 100));

                    AdInfoContainer.Text = "<input type=\"hidden\" name=\"__WATCHEDID\" id=\"__WATCHEDID\" value=\"" + ad.Id + "\" />";
                    AdInfoContainer.Text += "<input type=\"hidden\" id=\"adtime\" value=\"" + FixedTime.ToString() + "\"/>";

                    AdFrame.Text = "<iframe id=\"targetIframe1\" width=\"100%\" src=\"" + ad.TargetUrl + "\" style=\"margin: 0 auto; border-left: 0; border-right: 0; border-bottom: 0;\"></iframe>";

                    PTCSercurityManager.Lock(Convert.ToInt32(ad.DisplayTime.TotalSeconds));
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
        #endregion
        else if (windowOpener == WindowOpener.PTC && (Request.Params.Get("__EVENTARGUMENT5") != null || Request.QueryString["auto"] != null))
        {
            //Display ad before watch
            string AdId = "";

            if (Request.QueryString["auto"] != null)
            {
                //AUTO MODE
                int AutoId = Convert.ToInt32(Request.QueryString["auto"]);

                //Check if member logged
                if (!Member.IsLogged)
                {
                    var msg = string.Format("Browser: {0}, OS: {1}, AutoId: {2}, EventArgs: {3}, Ses.TOut: {4}, Context.Name: {5}",
                        Browser.Current, OperationSystem.Current, AutoId, Request.Params.Get("__EVENTARGUMENT5"), Session.Timeout, HttpContext.Current.User.Identity.Name);
                    ErrorLogger.Log(msg);
                    Response.Redirect("~/login.aspx");
                }

                var user = Member.Current;
                List<PtcAdvert> AvailableAdList = PtcAdvert.GetUnwatchedAdsForUser(user);
                if (AutoId == 0 || AvailableAdList.Count < 1)
                {
                    Response.Redirect("~/user/earn/ads.aspx");
                }
                else
                {
                    AdId = AvailableAdList[0].Id.ToString();

                    //Display queued ads
                    ThumbnailsLiteral.Controls.Clear();
                    LoadAdThumbnails();

                    Form.Action = "user/earn/surf.aspx?f=1&auto=" + AutoId;
                }
            }
            else
            {
                AdId = Request.Params.Get("__EVENTARGUMENT5");
                Form.Action = "user/earn/surf.aspx?f=1";
            }

            PtcAdvert ad = new PtcAdvert(Int32.Parse(AdId));
            AdsCreatorUsername = SetAdCreatorUsername(ad.CreatorUsername);

            if (!PTCSercurityManager.IsLocked)
            {
                try
                {
                    var info = GetAdInfo(Convert.ToInt32(AdId));
                    string AdTime = string.Empty;
                    string AdURL = info.Key;

                    int displayTime = (int)ad.DisplayTime.TotalSeconds;
                    PTCSercurityManager.Lock(displayTime);

                    AdTime = (displayTime).ToString();

                    AdInfoContainer.Text = "<input type=\"hidden\" name=\"__WATCHEDID\" id=\"__WATCHEDID\" value=\"" + AdId + "\" />";
                    AdInfoContainer.Text += "<input type=\"hidden\" id=\"adtime\" value=\"" + AdTime + "\"/>";

                    AdFrame.Text = "<iframe id=\"targetIframe1\" width=\"100%\" src=\"" + AdURL + "\" style=\"margin: 0 auto; border-left: 0; border-right: 0; border-bottom: 0;\"></iframe>";
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                    throw ex;
                }

            }
            else
            {
                ThumbnailsLiteral.Controls.Clear();
                MultiplePanel.Visible = true;
            }
        }
        else if (windowOpener == WindowOpener.AdPack && (Request.Params.Get("__EVENTARGUMENT5") != null || Request.QueryString["auto"] != null))
        {
            //Display ad before watch
            string AdId = "";

            if (Request.QueryString["auto"] != null)
            {
                if (CanLoadAutoAd == true)
                {
                    //AUTO MODE
                    int AutoId = Convert.ToInt32(Request.QueryString["auto"]);

                    var user = Member.Current;
                    List<AdPacksAdvert> AvailableAdList = AdPackManager.GetUnwatchedAdsForUser(user);

                    CanLoadAutoAd = false;

                    if (AutoId == 0 || AvailableAdList.Count < 1)
                    {
                        Response.Redirect("~/user/earn/adpacks.aspx");
                    }
                    else
                    {
                        AdId = AvailableAdList[0].Id.ToString();

                        //Display queued ads
                        ThumbnailsLiteral.Controls.Clear();
                        LoadAdPackThumbnails();

                        Form.Action = "user/earn/surf.aspx?f=2&auto=" + AutoId;
                    }
                }
            }
            else
            {
                AdId = Request.Params.Get("__EVENTARGUMENT5");
                Form.Action = "user/earn/surf.aspx?f=2&auto=1";
            }

            if (String.IsNullOrEmpty(AdId))
                AdId = StoredAdId.ToString();

            AdPacksAdvert ad = new AdPacksAdvert(Int32.Parse(AdId));
            AdsCreatorUsername = SetAdCreatorUsername(ad.CreatorUserId);

            if (!PTCSercurityManager.IsLocked)
            {
                try
                {
                    var info = GetAdInfo(Convert.ToInt32(AdId));
                    string AdTime = string.Empty;
                    string AdURL = info.Key;
                    int displayTime = AdPackManager.GetAdDisplayTime(ad);
                    PTCSercurityManager.Lock(displayTime);

                    AdTime = (displayTime).ToString();

                    AdInfoContainer.Text = "<input type=\"hidden\" name=\"__WATCHEDID\" id=\"__WATCHEDID\" value=\"" + AdId + "\" />";
                    AdInfoContainer.Text += "<input type=\"hidden\" id=\"adtime\" value=\"" + AdTime + "\"/>";


                    AdFrame.Text = "<iframe id=\"targetIframe1\" width=\"100%\" src=\"" + AdURL + "\" style=\"margin: 0 auto; border-left: 0; border-right: 0; border-bottom: 0;\"></iframe>";


                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                    throw ex;
                }

            }
            else
            {
                ThumbnailsLiteral.Controls.Clear();
                MultiplePanel.Visible = true;
            }
        }
        else if (windowOpener == WindowOpener.TrafficExchange && (Request.Params.Get("__EVENTARGUMENT5") != null || Request.QueryString["auto"] != null))
        {
            //Display ad before watch
            string AdId = "";
            if (Request.QueryString["auto"] != null)
            {
                if (CanLoadAutoAd == true)
                {
                    //AUTO MODE
                    int AutoId = Convert.ToInt32(Request.QueryString["auto"]);
                    //Check if member logged
                    if (!Member.IsLogged)
                        Context.RewritePath("~/login.aspx");


                    var user = Member.Current;
                    var AvailableAdList = TrafficExchangeManager.GetAdsAvailableForUser(user);

                    //Setting flag from ad download ability
                    CanLoadAutoAd = false;

                    if ((AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin != 0) && (user.NumberOfWatchedTrafficAdsToday >= AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin))
                        Response.Redirect("~/user/default.aspx");

                    if (AutoId == 0 || AvailableAdList.Count < 1)
                    {
                        Response.Redirect("~/user/earn/trafficexchange.aspx");
                    }
                    else
                    {
                        AdId = AvailableAdList[0].Id.ToString();

                        //Display queued ads
                        //ThumbnailsLiteral.Controls.Clear();
                        //LoadTrafficExchangeThumbnails();

                        Form.Action = "user/earn/surf.aspx?f=6&auto=" + AutoId;
                    }
                }
            }
            else
            {
                AdId = Request.Params.Get("__EVENTARGUMENT5");
                Form.Action = "user/earn/surf.aspx?f=6";
            }

            if (String.IsNullOrEmpty(AdId))
                AdId = StoredAdId.ToString();

            TrafficExchangeAdvert ad = new TrafficExchangeAdvert(Int32.Parse(AdId));
            AdsCreatorUsername = SetAdCreatorUsername(ad.CreatorUsername);

            if (!PTCSercurityManager.IsLocked)
            {
                try
                {
                    var info = GetAdInfo(Convert.ToInt32(AdId));
                    string AdTime = string.Empty;
                    string AdURL = info.Key;

                    PTCSercurityManager.Lock(Convert.ToInt32(ad.DisplayTime.TotalSeconds));

                    AdTime = info.Value.TotalSeconds.ToString();

                    AdInfoContainer.Text = "<input type=\"hidden\" name=\"__WATCHEDID\" id=\"__WATCHEDID\" value=\"" + AdId + "\" />";
                    AdInfoContainer.Text += "<input type=\"hidden\" id=\"adtime\" value=\"" + AdTime + "\"/>";

                    //Get subpages
                    var sublist = TableHelper.SelectRows<TrafficExchangeSubpage>(TableHelper.MakeDictionary("PtcAdId", ad.Id));
                    if (sublist.Count > 0)
                    {
                        Random rand = new Random();
                        int probability = rand.Next(-1, sublist.Count);
                        if (probability != -1)
                        {
                            AdURL = info.Key + sublist[probability].SubPage;
                        }
                    }

                    if (ad.Description == TrafficSource.Anonymous)
                    {
                        //Anon
                        AdFrame.Text = "<iframe id=\"targetIframe1\" width=\"100%\" src=\"about:blank\" style=\"margin: 0 auto; border-left: 0; border-right: 0; border-bottom: 0;\"></iframe>";
                        ScriptLiteral.Text = @"<script>function load() {
    var postdata = '<form id=formX5 action=\'" + AdURL + @"\'>' +
                            '</form>';
            top.frames[0].document.body.innerHTML = postdata;
            top.frames[0].document.getElementById('formX5').submit();
                        } </script>";
                    }
                    else
                    {
                        AdFrame.Text = "<iframe id=\"targetIframe1\" width=\"100%\" src=\"" + AdURL + "\" style=\"margin: 0 auto; border-left: 0; border-right: 0; border-bottom: 0;\"></iframe>";
                    }

                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                    throw ex;
                }

            }
            else
            {
                ThumbnailsLiteral.Controls.Clear();
                MultiplePanel.Visible = true;
            }
        }
        else if (windowOpener == WindowOpener.LoginAd)
        {
            Form.Action = "user/earn/surf.aspx?f=5";

            var adToDisplay = LoginManager.GetLoginAd(Member.Current);
            AdsCreatorUsername = SetAdCreatorUsername(adToDisplay.CreatorUserId);

            if (adToDisplay == null)
            {
                LoginManager.LoginAdWatched = true;
                Response.Redirect("~/user/default.aspx?afterlogin=1");
            }


            var AdTime = AppSettings.LoginAds.DisplayTime.ToString(); //get ad time in seconds

            var AdURL = adToDisplay.TargetUrl;


            if (!PTCSercurityManager.IsLocked)
            {
                try
                {
                    AdInfoContainer.Text = "<input type=\"hidden\" name=\"__WATCHEDID\" id=\"__WATCHEDID\" value=\"" + adToDisplay.Id + "\" />";
                    AdInfoContainer.Text += "<input type=\"hidden\" id=\"adtime\" value=\"" + AdTime + "\"/>";

                    AdFrame.Text = "<iframe id=\"targetIframe1\" width=\"100%\" src=\"" + AdURL + "\" style=\"margin: 0 auto; border-left: 0; border-right: 0; border-bottom: 0;\"></iframe>";

                    PTCSercurityManager.Lock(Convert.ToInt32(AdTime));
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
        else if (windowOpener == WindowOpener.URLChecker)
        {
            //URL Checker
            BeforePanel.Visible = false;
            BeforePanelURLChecker.Visible = true;
            ErrorLiteralChecker.Text = U4200.URLCHECKBAD;
            if (Request.Params.Get("__CHECKURLMESSAGE") != null)
            {
                AdInfoContainer.Text = "";
                BeforePanel.Visible = false;
                BeforePanelURLChecker.Visible = false;
                URLCheckerOK.Visible = true;
            }

            else if (Request.Params.Get("__EVENTARGUMENT5") != null)
            {
                string urlToCheck = Request.Params.Get("__EVENTARGUMENT5");

                Form.Action = "user/earn/surf.aspx?f=3";
                try
                {
                    urlToCheck = new UrlFactory(urlToCheck).CreateInstance().ReplaceUrl();

                    UrlVerifier.Check(urlToCheck);
                    HashedURL = Encryption.Encrypt(urlToCheck);
                    AdInfoContainer.Text = "<input type='hidden' name='__CHECKURLMESSAGE' id='__CHECKURLMESSAGE' value='ok' />";
                    AdInfoContainer.Text += "<input type=\"hidden\" id=\"adtime\" value=\"1\"/>";
                    AdFrame.Text = "<iframe id=\"targetIframe1\" width=\"100%\" src=\"" + urlToCheck + "\" style=\"margin: 0 auto; border-left: 0; border-right: 0; border-bottom: 0;\"></iframe>";
                }
                catch (MsgException ex)
                {
                    ErrorLiteralChecker.Text = ex.Message;
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                    throw ex;
                }
            }
        }
        else if (windowOpener == WindowOpener.PaidToPromote)
        {
            Form.Action = string.Format("user/earn/surf.aspx?ref={0}", ReferrerId);

            BannerPanel.DimensionId = AppSettings.PaidToPromote.BannerDimensionId;

            //Display ad before watch                              
            var AvailableAdList = PaidToPromoteManager.GetActiveAdsForCurrentIP(IP.Current);
            var ptpAdId = AvailableAdList[0].Id.ToString();
            var advert = new PaidToPromoteAdvert(Int32.Parse(ptpAdId));

            if (!PTCSercurityManager.IsLocked)
            {
                try
                {
                    var AdTime = AppSettings.PaidToPromote.AdDuration;
                    var AdURL = advert.TargetUrl;

                    PTCSercurityManager.Lock(AdTime);

                    AdInfoContainer.Text = "<input type=\"hidden\" name=\"__WATCHEDID\" id=\"__WATCHEDID\" value=\"" + ptpAdId + "\" />";
                    AdInfoContainer.Text += "<input type=\"hidden\" id=\"adtime\" value=\"" + AdTime.ToString() + "\"/>";

                    AdFrame.Text = "<iframe id=\"targetIframe1\" width=\"100%\" src=\"" + AdURL + "\" style=\"margin: 0 auto; border-left: 0; border-right: 0; border-bottom: 0;\"></iframe>";
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                    throw ex;
                }
            }
        }

        if (AppSettings.GlobalAdvertsSettings.AdvertisersInfoOnSurfPage)
        {
            if (Session["SurfBannerAdvertID"] != null && (int)Session["SurfBannerAdvertID"] != 0 && !string.IsNullOrEmpty(new BannerAdvert((int)Session["SurfBannerAdvertID"]).CreatorUsername))
            {
                BannerAdvertiserHolder.Visible = true;
                var bannerAdvertiser = new Member(new BannerAdvert((int)Session["SurfBannerAdvertID"]).CreatorUsername);

                bannerAdvertiserImage.Src = ResolveClientUrl(bannerAdvertiser.AvatarUrl);
                bannerAdvertiserLink.HRef = HtmlCreator.GetProfileURL(bannerAdvertiser);
                BannerAdvertiserInfoLiteral.Text = bannerAdvertiser.Name;
            }

            if (!string.IsNullOrEmpty(AdsCreatorUsername))
            {
                AdAdvertiserHolder.Visible = true;

                var adAdvertiser = new Member(AdsCreatorUsername);

                adAdvertiserImage.Src = ResolveClientUrl(adAdvertiser.AvatarUrl);
                adAdvertiserLink.HRef = HtmlCreator.GetProfileURL(adAdvertiser);
                AdAdvertiserInfoLiteral.Text = adAdvertiser.Name;
            }
        }
    }

    protected void LoadPTPThumbnails()
    {
        int limitThumbnailsTo = 15;

        ThumbnailsLiteral.Visible = true;
        Member user = Member.Current;

        List<AdPacksAdvert> AvailableAdList = AdPackManager.GetUnwatchedAdsForUser(user);

        try
        {
            for (int i = 0; i < AvailableAdList.Count && i < limitThumbnailsTo; i++)
            {
                if (i == 0)
                    ThumbnailsLiteral.Controls.Add(GetAdPackThumbnail(AvailableAdList[i], true));
                else
                    ThumbnailsLiteral.Controls.Add(GetAdPackThumbnail(AvailableAdList[i]));

            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            throw ex;
        }
    }

    protected void LoadAdPackThumbnails()
    {
        int limitThumbnailsTo = 15;

        ThumbnailsLiteral.Visible = true;
        Member user = Member.Current;

        List<AdPacksAdvert> AvailableAdList = AdPackManager.GetUnwatchedAdsForUser(user);

        try
        {
            for (int i = 0; i < AvailableAdList.Count && i < limitThumbnailsTo; i++)
            {
                if (i == 0)
                    ThumbnailsLiteral.Controls.Add(GetAdPackThumbnail(AvailableAdList[i], true));
                else
                    ThumbnailsLiteral.Controls.Add(GetAdPackThumbnail(AvailableAdList[i]));

            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            throw ex;
        }
    }

    protected UserControl GetAdPackThumbnail(AdPacksAdvert ad, bool isCurrentlyWatched = false)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/Advertisements/AdPackAdvertThumbnail.ascx");
        var parsedControl = objControl as IAdPackObjectControl;
        parsedControl.Object = ad;

        PropertyInfo myProp = parsedControl.GetType().GetProperty("IsCurrentlyWatched");
        myProp.SetValue(parsedControl, isCurrentlyWatched, null);

        parsedControl.DataBind();

        return objControl;
    }

    protected void LoadAdThumbnails()
    {
        int limitThumbnailsTo = 15;

        ThumbnailsLiteral.Visible = true;
        Member user = Member.Current;

        List<PtcAdvert> AvailableAdList = PtcAdvert.GetUnwatchedAdsForUser(user);

        try
        {
            for (int i = 0; i < AvailableAdList.Count && i < limitThumbnailsTo; i++)
            {
                if (i == 0)
                    ThumbnailsLiteral.Controls.Add(GetAdThumbnail(AvailableAdList[i], true));
                else
                    ThumbnailsLiteral.Controls.Add(GetAdThumbnail(AvailableAdList[i]));

            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            throw ex;
        }
    }

    protected UserControl GetAdThumbnail(PtcAdvert ad, bool isCurrentlyWatched = false)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/Advertisements/PtcAdThumbnail.ascx");
        var parsedControl = objControl as IPtcAdvertObjectControl;
        parsedControl.Object = ad;

        PropertyInfo myProp = parsedControl.GetType().GetProperty("IsCurrentlyWatched");
        myProp.SetValue(parsedControl, isCurrentlyWatched, null);

        parsedControl.DataBind();

        return objControl;
    }

    /*
    protected void LoadTrafficExchangeThumbnails()
    {
        ThumbnailsLiteral.Visible = true;

        Member user = Member.Current;

        var AvailableAdList = TrafficExchangeManager.GetAdsAvailableForUser(user);
        try
        {
            for (int i = 0; i < AvailableAdList.Count; i++)
            {
                if (i == 0)
                    ThumbnailsLiteral.Controls.Add(GetTrafficThumbnail(AvailableAdList[i], true));
                else
                    ThumbnailsLiteral.Controls.Add(GetTrafficThumbnail(AvailableAdList[i]));

            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            throw ex;
        }
    }
    */

    protected UserControl GetTrafficThumbnail(TrafficExchangeAdvert ad, bool isCurrentlyWatched = false)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/Advertisements/ExchangeAdThumbnail.ascx");
        var parsedControl = objControl as ITrafficExchangeObjectControl;
        parsedControl.Object = ad;

        PropertyInfo myProp = parsedControl.GetType().GetProperty("IsCurrentlyWatched");
        myProp.SetValue(parsedControl, isCurrentlyWatched, null);

        parsedControl.DataBind();

        return objControl;
    }

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
            if (windowOpener == WindowOpener.PTC)
                ProcceedWithAdCredit(StoredAdId);
            else if (windowOpener == WindowOpener.AdPack)
                ProcceedWithAdPackCredit(StoredAdId);
            else if (windowOpener == WindowOpener.LoginAd)
                ProcceedWithLoginAdCredit(StoredAdId);
            else if (windowOpener == WindowOpener.TrafficExchange)
                ProcceedWithTrafficExchangeCredit(StoredAdId);
            else if (windowOpener == WindowOpener.PaidToPromote)
                ProcceedWithPaidToPromoteAdCredit(StoredAdId);
        }
    }
    protected void FavoriteAdsImageButton_Click(object sender, EventArgs e)
    {
        FavoriteAdsImageButton.Visible = false;

        if (Member.IsLogged)
        {
            if (FavoriteAd.IsFavorite(Member.CurrentId, StoredAdId, FavoriteAdType.PTC))
            {
                FavoriteAd.RemoveFromFavorites(Member.CurrentId, StoredAdId, FavoriteAdType.PTC);
                FavoriteResult.Text = U5006.REMOVEDFROMF;
            }
            else
            {
                FavoriteAd.AddToFavorites(Member.CurrentId, StoredAdId, FavoriteAdType.PTC);
                FavoriteResult.Text = U5006.ADDEDTOF;
            }
        }
    }


    protected void CreditAfterFeedbackCaptcha_Click(object sender, EventArgs e)
    {
        ImageButton btn = (ImageButton)(sender);
        CaptchaAnswer = btn.CommandArgument == "1";

        //Works only for PTC      
        if (windowOpener == WindowOpener.PTC)
            ProcceedWithAdCredit(StoredAdId);
    }

    protected void ProcceedWithTrafficExchangeCredit(int Id)
    {
        if (Member.IsLogged)
        {
            var User = Member.Current;

            CaptchaPanel.Visible = false;

            var info = GetAdInfo(Id);
            var ad = new TrafficExchangeAdvert(Id);

            String MessageForUser = String.Empty;

            var adsList = TrafficExchangeManager.GetAdsAvailableForUser(User).Select(x=> x.Id);
            if (!adsList.Contains(Id))
                DisplayInfo(L1.ALREADYWATCHED, false);            
            else
            {
                //For Ahmed's platform, we credit Points not cash
                if (TitanFeatures.IsAhmed)
                {
                    CloseRefreshButton.Visible = false;

                    User.NumberOfWatchedTrafficAdsToday++;

                    //If earning per view is 0, there is no credit action and no info about Points.
                    if (AppSettings.TrafficExchange.AmountOfPointsPerWatchForcedByAdmin > 0)
                    {
                        String EarnInfo = String.Format("{0} <b>{1}</b> {2} {3}", L1.YOUVEBEENCREDITED, AppSettings.TrafficExchange.AmountOfPointsPerWatchForcedByAdmin.ToString(), AppSettings.PointsName, U6010.POINTSFORADWATCH);
                        User.AddToPointsBalance(AppSettings.TrafficExchange.AmountOfPointsPerWatchForcedByAdmin, "Task Bonus");
                        User.PointsToday += AppSettings.TrafficExchange.AmountOfPointsPerWatchForcedByAdmin;
                        User.TotalPointsGenerated += AppSettings.TrafficExchange.AmountOfPointsPerWatchForcedByAdmin;
                        User.IncreasePointsEarnings(AppSettings.TrafficExchange.AmountOfPointsPerWatchForcedByAdmin);
                        MessageForUser += EarnInfo;
                    }
                    //Render text for Ahmed - Success of Daily Task or Info about Daily Task collected views
                    else if (AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin == User.NumberOfWatchedTrafficAdsToday)
                    {
                        MessageForUser += "<br />" + U6010.DAILYTASKCOMPLETED;
                        CloseRefreshButton.Visible = true;
                    }
                    else
                    {
                        String ProgressInfo = String.Format(U6010.YOUCOMPLETED, User.NumberOfWatchedTrafficAdsToday, AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin);
                        MessageForUser += "<br />" + ProgressInfo;
                    }

                    User.Save();
                    DisplayInfo(MessageForUser, true);
                }
                else
                {
                    //Use Crediter
                    TrafficExchangeCrediter Crediter = (TrafficExchangeCrediter)CrediterFactory.Acquire(User, CreditType.TrafficExchange);
                    Money Calculated = Crediter.CreditMember(ad);

                    MessageForUser = L1.YOUVEBEENCREDITED + " <b>" + Calculated.ToString() + "</b> " + L1.FORTHISAD;

                    //When forcing ad views and there are Points per every view, also credit Points
                    if (AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin > 0)
                    {
                        User.NumberOfWatchedTrafficAdsToday++;
                        if (AppSettings.TrafficExchange.AmountOfPointsPerWatchForcedByAdmin > 0)
                        {
                            User.AddToPointsBalance(AppSettings.TrafficExchange.AmountOfPointsPerWatchForcedByAdmin, "Task Bonus");
                            User.PointsToday += AppSettings.TrafficExchange.AmountOfPointsPerWatchForcedByAdmin;
                            User.TotalPointsGenerated += AppSettings.TrafficExchange.AmountOfPointsPerWatchForcedByAdmin;
                            User.IncreasePointsEarnings(AppSettings.TrafficExchange.AmountOfPointsPerWatchForcedByAdmin);

                            MessageForUser += String.Format("<br> {0} <b>{1}</b> {2} {3}", L1.YOUVEBEENCREDITED, AppSettings.TrafficExchange.AmountOfPointsPerWatchForcedByAdmin.ToString(), AppSettings.PointsName, U6010.POINTSFORADWATCH);
                        }

                        if (AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin == User.NumberOfWatchedTrafficAdsToday)
                        {
                            MessageForUser += "<br />" + U6010.DAILYTASKCOMPLETED;
                            CloseRefreshButton.Visible = true;
                        }
                        User.Save();
                    }

                    DisplayInfo(MessageForUser, true);
                }


                //Display "Feel free with Ad URL" text
                FeelFreeLiteral1.Text = FeelFreeLiteral2.Text = FeelFreeLiteral3.Text = ": <a href=\"" + info.Key + "\" target=\"_blank\">"
                    + info.Key + "</a>";

                //NOW INCREASE AD HITS !!!!
                if (ad.Advertiser.CreatedBy == Advertiser.Creator.Member)
                {
                    Member AdvertiserUser = new Member(ad.Advertiser.MemberUsername);
                    AdvertiserUser.IncreaseStatClicks(1);
                    AdvertiserUser.SaveStatClicks();
                }

                //Modify the ad info
                ad.Click();
                if (ad.ShouldBeFinished)
                {
                    ad.Status = AdvertStatus.Finished;
                    ad.Save();
                }
                else
                    ad.SaveClicks();

                //Ad has just been watched
                if (AppSettings.IsDemo == false)
                    TrafficExchangeManager.AddAdvertToRecentlyWatched(User, Id);
            }
            if (Request.QueryString["auto"] != null && !MultiplePanel.Visible)
            {
                int AutoId = Convert.ToInt32(Request.QueryString["auto"]);

                //Redirect automatically or after clicking "Next" button
                if (AppSettings.TrafficExchange.TimeBetweenAdsRedirectInSeconds == -1)
                {
                    HomeButton.Text = U4000.CLOSE;
                    NextAddButton.Text = U4000.NEXT;

                    //If user completed daily task, no more traffic exchange
                    if ((AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin != 0) &&
                        User.NumberOfWatchedTrafficAdsToday >= AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin)
                    {
                        NextAddButton.Visible = false;
                        HomeButton.Visible = true;
                        HomeButton.PostBackUrl = "user/default.aspx";
                    }
                    else
                    {
                        NextAddButton.Visible = true;
                        NextAddButton.PostBackUrl = string.Format("user/earn/surf.aspx?f=6&auto={0}", AutoId + 1);

                        if (TitanFeatures.IsAhmed)
                            HomeButton.Visible = false;
                    }
                }
                else
                {
                    if ((AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin != 0) && (AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin <= User.NumberOfWatchedTrafficAdsToday))
                        Response.Redirect("~/user/default.aspx");
                    else
                        Response.AppendHeader("Refresh", string.Format("{0};url=surf.aspx?f=6&auto={1}", AppSettings.TrafficExchange.TimeBetweenAdsRedirectInSeconds, AutoId + 1));
                }
            }
        }
        else
        {
            //Non-logged in member
            //Just display 'you must login' message
            DisplayInfo(L1.YOUMUSTBELOGGED, false);
        }
        AdInfoContainer.Text = "";

        CanLoadAutoAd = true;
    }

    protected void ProcceedWithPaidToPromoteAdCredit(int Id)
    {
        CaptchaPanel.Visible = false;

        PaidToPromoteManager.TryToCreditAdAndReferrer(Id, IP.Current, ReferrerId);

        var link = string.Format("{0}?ref={1} ", AppSettings.Site.Url, ReferrerId);
        var ad = new PaidToPromoteAdvert(Id);

        PaidToPromotePanel.Visible = true;
        PaidToPromoteLabel.Text = string.Format("Advertise here for only {0} = USD CPM<br><a href='{1}'>{1}</a>", AppSettings.PaidToPromote.CostPerMillePrice.ToString(), link);
        FeelFreePTPLiteral.Text = string.Format("<a href='{0}'>{0}</a>", ad.TargetUrl);

        AdInfoContainer.Text = "";
    }

    protected void ProcceedWithLoginAdCredit(int Id)
    {
        //VERIFY NOT CHECKING IF LOGGED
        if (Member.IsLogged)
        {
            RegisterUserValidationSummary.Visible = true;
            CaptchaPanel.Visible = false;

            var info = GetAdInfo(Id);

            LoginAd Ad = new LoginAd(Id);

            //add to number of views
            Ad.TotalViews += 1;

            Ad.Save();

            LoginManager.LoginAdWatched = true;

            DisplayInfo(U4200.THANKSFORVIEWINGTHISAD, true);
            //Display "Feel free with Ad URL" text
            FeelFreeLiteral1.Text = FeelFreeLiteral2.Text = FeelFreeLiteral3.Text = ": <a href=\"" + info.Key + "\" target=\"_blank\">"
                + info.Key + "</a>";

            DashboardButton.Visible = true;
            DashboardButton.Text = U4200.GOTODASHBOARD;
            DashboardButton.PostBackUrl = "user/default.aspx?afterlogin=1";
        }
        else
        {
            //Non-logged in member
            //Just display 'you must login' message
            DisplayInfo(L1.YOUMUSTBELOGGED, false);
        }
        AdInfoContainer.Text = "";
    }

    protected void ProcceedWithAdPackCredit(int Id)
    {
        if (Member.IsLogged)
        {
            RegisterUserValidationSummary.Visible = true;

            var User = Member.GetLoggedMember(Context);

            if (User.RSAPTCAdsViewed.Contains(Id))
                DisplayInfo(L1.ALREADYWATCHED, false);
            else
            {
                CaptchaPanel.Visible = false;

                var info = GetAdInfo(Id);

                string coloredAdsViewed;

                //Userize
                List<int> av = User.RSAPTCAdsViewed;

                AdPacksAdvert Ad = new AdPacksAdvert(Id);

                //add to number of clicks
                AdPackManager.AddClickToAdPacksAd(Id);

                av.Add(Ad.Id);
                User.RSAPTCAdsViewed = av;
                if (TitanFeatures.IsTradeOwnSystem)
                    User.AdPackViewedCounter += 1;

                var adsViewed = User.RSAPTCAdsViewed.Count;
                int dailyRequiredClicks = User.Membership.AdPackDailyRequiredClicks;

                if (adsViewed < dailyRequiredClicks)
                    coloredAdsViewed = string.Format("<span style={0}>{1}</span>", "'color: #cb6f1b; font-weight: bold;'", adsViewed);
                else
                    coloredAdsViewed = string.Format("<span style={0}>{1}</span>", "'color: #87a131; font-weight: bold;'", adsViewed);

                var displayInfo = U4200.WATCHEDADPACKSADVERT.Replace("%n%", string.Format("{0}/{1}", coloredAdsViewed, dailyRequiredClicks));

                DisplayInfo(displayInfo, true);

                //Display "Feel free with Ad URL" text
                FeelFreeLiteral1.Text = FeelFreeLiteral2.Text = FeelFreeLiteral3.Text = ": <a href=\"" + info.Key + "\" target=\"_blank\">"
                    + info.Key + "</a>";

                //Force notifiaction refresh 
                NotificationManager.RefreshWithMember(NotificationType.NewAdPacksAds, User);

                //Poinst Bonus
                if (AppSettings.Points.PointsEnabled)
                {
                    var reward = User.Membership.AdPacksAdsPointsReward;
                    var note = string.Format("{0}: reward for watched ad", AppSettings.RevShare.AdPack.AdPackName);
                    User.AddToPointsBalance(reward, note);
                }

                User.Save();
            }

            if (Request.QueryString["auto"] != null && !MultiplePanel.Visible)
            {
                int AutoId = Convert.ToInt32(Request.QueryString["auto"]);

                //Redirect automatically or after clicking "Next" button
                if (AppSettings.RevShare.AdPack.TimeBetweenAdsRedirectInSeconds == -1)
                {
                    NextAddButton.Visible = true;
                    NextAddButton.Text = U4000.NEXT;
                    HomeButton.Text = U4000.CLOSE;
                    NextAddButton.PostBackUrl = string.Format("{0}user/earn/surf.aspx?f=2&auto={1}", AppSettings.Site.Url, AutoId + 1);
                }
                else
                    AutoUrl.Value = string.Format("{0}user/earn/surf.aspx?f=2&auto={1}", AppSettings.Site.Url, AutoId + 1);

            }
        }
        else
        {
            //Non-logged in member
            //Just display 'you must login' message
            DisplayInfo(L1.YOUMUSTBELOGGED, false);
        }
        AdInfoContainer.Text = "";
        CanLoadAutoAd = true;
    }

    protected void ProcceedWithAdCredit(int Id)
    {
        //Stats
        //var targetStatValue = new Prem.PTC.Statistics.Statistics(Prem.PTC.Statistics.StatisticsType.PTCClicks);
        //targetStatValue.AddToData1(1);
        //targetStatValue.Save();
        //Commented out for performance

        if (Member.IsLogged)
        {
            RegisterUserValidationSummary.Visible = true;

            //Anti-cheat: Check if ad is not already watched
            var User = Member.Current;

            //Favorite Ads
            if (AppSettings.PtcAdverts.FavoriteAdsEnabled)
                FavoriteAdsPanel.Visible = true;

            if (User.AdsViewedCount >= User.Membership.MaxDailyPtcClicks)
                DisplayInfo(string.Format(U6000.CANTWATCHMOREPTC, User.AdsViewed.Count), false);
            else if (User.AdsViewed.Contains(Id))
                DisplayInfo(L1.ALREADYWATCHED, false);
            else
            {
                CaptchaPanel.Visible = false;
                FeedbackCaptchaPanel.Visible = false;

                var ad = new PtcAdvert(Id);

                if (AppSettings.PtcAdverts.CashLinkCrediterEnabled)
                {
                    Money calculated = Money.Zero;
                    //PTC - Cash Link Crediter
                    if (TitanFeatures.IsRevolca)
                    {
                        RevolcaCashLinkCrediter crediter = new RevolcaCashLinkCrediter(User);
                        calculated = crediter.Credit(ad);
                    }
                    else
                    {
                        CashLinkCrediter crediter = new CashLinkCrediter(User);
                        calculated = crediter.Credit(ad);
                    }
                    DisplayInfo(L1.YOUVEBEENCREDITED + " <b>" + calculated.ToString() + "</b> " + L1.FORTHISAD, true);
                }
                else
                {
                    string displayInfoText = L1.YOUVEBEENCREDITED;

                    if (AppSettings.Points.PointsEnabled && User.Membership.AdvertPointsEarnings > 0)
                        displayInfoText += " <b>" + User.Membership.AdvertPointsEarnings.ToString() + "</b> " + AppSettings.PointsName + " +";

                    if (AppSettings.PtcAdverts.PTCCreditsEnabled && User.Membership.PTCCreditsPerView > 0)
                        displayInfoText += " <b>" + User.Membership.PTCCreditsPerView.ToString() + "</b> " + U5006.ADCREDITS + " +";

                    //PTC 
                    PtcCrediter Crediter = (PtcCrediter)CrediterFactory.Acquire(User, Titan.CreditType.PTC);

                    Money Calculated = Crediter.CreditMember(ad, Request.QueryString["auto"] != null);

                    if (!AppSettings.PtcAdverts.DisableMoneyEarningsInPTC)
                        displayInfoText += " <b>" + Calculated.ToString() + "</b> +";

                    displayInfoText = displayInfoText.TrimEnd('+');

                    if (TitanFeatures.IsRofriqueWorkMines)
                        DisplayInfo(displayInfoText + " for this mining session.", true);
                    else
                        DisplayInfo(displayInfoText + " " + L1.FORTHISAD, true);
                }

                //Force notifiaction refresh 
                NotificationManager.RefreshWithMember(NotificationType.NewAds, User);

                //Display "Feel free with Ad URL" text
                FeelFreeLiteral1.Text = FeelFreeLiteral2.Text = FeelFreeLiteral3.Text = ": <a href=\"" + ad.TargetUrl + "\" target=\"_blank\">" + ad.TargetUrl + "</a>";

                //Modify the ad info
                if (!AppSettings.PtcAdverts.FeedbackCaptchaEnabled)
                    ad.Click();
                else
                    ad.ClickWithFeedbackCaptcha(CaptchaAnswer ? 1 : 0, CaptchaAnswer ? 0 : 1);

                ad.SaveClicks();

                if (ad.ShouldBeFinished)
                {
                    ad.Status = AdvertStatus.Finished;
                    ad.SaveStatus();
                }
            }
            if (Request.QueryString["auto"] != null && !MultiplePanel.Visible)
            {
                int AutoId = Convert.ToInt32(Request.QueryString["auto"]);

                //Redirect automatically or after clicking "Next" button
                if (AppSettings.RevShare.AdPack.TimeBetweenAdsRedirectInSeconds == -1)
                {
                    NextAddButton.Visible = true;
                    NextAddButton.Text = U4000.NEXT;
                    HomeButton.Text = U4000.CLOSE;
                    NextAddButton.PostBackUrl = string.Format("{0}user/earn/surf.aspx?f=1&auto={1}", AppSettings.Site.Url, AutoId + 1);
                }
                else
                {
                    AutoUrl.Value = string.Format("{0}user/earn/surf.aspx?f=1&auto={1}", AppSettings.Site.Url, AutoId + 1);
                }
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
        PTCSercurityManager.Release();
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
        HomeButton.Visible = false;
        NextAddButton.Visible = false;

        if (TitanFeatures.IsRofriqueWorkMines)
            FinalInfoPlaceHolder.Visible = false;

        if (isSuccess)
        {
            if (windowOpener == WindowOpener.TrafficExchange)
            {
                AdCloseRefreshButton.Visible = false;
                if (AppSettings.TrafficExchange.TimeBetweenAdsRedirectInSeconds == -1)
                {
                    HomeButton.Visible = true;
                    NextAddButton.Visible = true;
                }
            }
            else if (windowOpener == WindowOpener.AdPack)
            {
                AdCloseRefreshButton.Visible = false;
                if (AppSettings.RevShare.AdPack.TimeBetweenAdsRedirectInSeconds == -1)
                {
                    HomeButton.Visible = true;
                    NextAddButton.Visible = true;
                }
            }
            else if (windowOpener == WindowOpener.LoginAd)
            {
                AdCloseRefreshButton.Visible = false;
            }
        }
        InfoText.Text = infoText;

        if (isSuccess)
            StatusImage.ImageUrl = "~/Images/Misc/ok2mini.png";
        else
            StatusImage.ImageUrl = "~/Images/Misc/fail2mini.png";
    }

    protected enum WindowOpener
    {
        TrafficGrid = 0,
        PTC = 1,
        AdPack = 2,
        URLChecker = 3,
        LoginAd = 5,
        TrafficExchange = 6,
        PaidToPromote = 7,
    }

    private KeyValuePair<string, TimeSpan> GetAdInfo(int id)
    {
        if (windowOpener == WindowOpener.PTC)
        {
            PtcAdvert ad = new PtcAdvert(id);
            AdsCreatorUsername = SetAdCreatorUsername(ad.CreatorUsername);
            if (AppSettings.PtcAdverts.FeedbackCaptchaEnabled)
            {
                FeedbackCaptchaQuestionLabel.Text = ad.CaptchaQuestion;
                YesCountLabel.Text = "(" + ad.CaptchaYesAnswers + ")";
                NoCountLabel.Text = "(" + ad.CaptchaNoAnswers + ")";

                //Advertiser Avatar
                if (AppSettings.PtcAdverts.ShowAdvertiserAvatar && ad.Advertiser.Is(Advertiser.Creator.Member))
                {
                    AvatarPanel.Visible = true;
                    AvatarInfo.DisplayMember = new Member(ad.AdvertiserUserId);
                    AvatarInfo.DataBind();
                }
            }
            return new KeyValuePair<string, TimeSpan>(ad.TargetUrl, ad.DisplayTime);
        }
        else if (windowOpener == WindowOpener.AdPack)
        {
            AdPacksAdvert ad = new AdPacksAdvert(id);
            AdsCreatorUsername = SetAdCreatorUsername(ad.CreatorUserId);
            return new KeyValuePair<string, TimeSpan>(ad.TargetUrl, new TimeSpan(0, 0, AdPackManager.GetAdDisplayTime(ad)));
        }
        else if (windowOpener == WindowOpener.LoginAd)
        {
            LoginAd ad = new LoginAd(id);
            AdsCreatorUsername = SetAdCreatorUsername(ad.CreatorUserId);
            return new KeyValuePair<string, TimeSpan>(ad.TargetUrl, new TimeSpan(0, 0, AppSettings.LoginAds.DisplayTime));
        }
        else if (windowOpener == WindowOpener.TrafficExchange)
        {
            TrafficExchangeAdvert ad = new TrafficExchangeAdvert(id);
            AdsCreatorUsername = SetAdCreatorUsername(ad.CreatorUsername);
            return new KeyValuePair<string, TimeSpan>(ad.TargetUrl, ad.DisplayTime);
        }

        return new KeyValuePair<string, TimeSpan>();
    }

    protected void NextAddButton_Click(object sender, EventArgs e)
    {
        AfterCounterPanel.Visible = false;
        BeforePanel.Visible = true;
    }

    private string SetAdCreatorUsername(int userId)
    {
        try
        {
            return new Member(userId).Name;
        }
        catch (Exception e)
        {
            return string.Empty;
        }
    }

    private string SetAdCreatorUsername(string name)
    {
        if (string.IsNullOrEmpty(name))
            return string.Empty;
        return name;
    }
}
