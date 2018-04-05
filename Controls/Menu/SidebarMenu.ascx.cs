using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Prem.PTC;
using Prem.PTC.Members;
using System.Web.UI.HtmlControls;
using Prem.PTC.Memberships;
using Titan.Leadership;

public partial class Controls_SidebarMenu : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeSideBar();
        InitizlizeLeadershipInfo();

        if (!Page.IsPostBack)
        {
            if (!LeadershipSystemPlaceHolder.Visible)
                if (TitanFeatures.IsAhmed)
                    Earn4.Visible = false;

            if (TitanFeatures.IsEpadilla)
                S4DSPackages.Visible = true;

            if (TitanFeatures.IsTrafficThunder)
            {
                var AdPackList = TableHelper.GetListFromRawQuery<AdPackType>("SELECT * FROM AdPackTypes WHERE Status=1 ");
                int AdPackId = AdPackList[0].Id;

                int ActivePackages = (int)TableHelper.SelectScalar(String.Format("SELECT COUNT (Id) FROM AdPacks WHERE UserId = {0} AND AdPackTypeId = {1} AND MoneyReturned < MoneyToReturn", Member.CurrentId, AdPackId));
                int ExpiretPackages = (int)TableHelper.SelectScalar(String.Format("SELECT COUNT (Id) FROM AdPacks WHERE UserId = {0} AND AdPackTypeId = {1} AND MoneyReturned >= MoneyToReturn", Member.CurrentId, AdPackId));
                int TotalPackages = (int)TableHelper.SelectScalar(String.Format("SELECT COUNT (Id) FROM AdPacks WHERE UserId = {0} AND AdPackTypeId = {1}", Member.CurrentId, AdPackId));

                AdPackInfoActivePackages.Text  = String.Format("<td>{0}</td> <td>{1}</td>", ActivePackages.ToString(), "ACTIVE PACKAGES");
                AdPackInfoExpiredPackages.Text = String.Format("<td>{0}</td> <td>{1}</td>", ExpiretPackages.ToString(), "EXPIRED PACKAGES");
                AdPackInfoTotalPackages.Text   = String.Format("<td>{0}</td> <td>{1}</td>", TotalPackages.ToString(), "TOTAL PACKAGES");
                AdPackInfoPlaceHolder.Visible = true;
            }
        }
    }

    public void InitizlizeLeadershipInfo()
    {
        if (!AppSettings.TitanFeatures.LeaderShipSystemEnabled)
            return;

        var userRank = RanksUsers.GetCurrentUserRank(Member.CurrentId);

        if (userRank == null)
            return;

        LeadershipSystemPlaceHolder.Visible = true;

        var rank = new LeadershipRank(userRank.RankId);

        if(string.IsNullOrEmpty(rank.RankName))
            LeadershipSystemRankDetailLiteral.Text = rank.Rank.ToString();
        else
            LeadershipSystemRankDetailLiteral.Text = string.Format("{0} ({1})", rank.RankName, rank.Rank);
    }

    public void InitializeSideBar()
    {
        if (Member.IsLogged)
        {
            //Turn some features on and off
            try
            {
                AriPlaceHolder.Visible = TitanFeatures.isAri;
                if (TitanFeatures.isAri)
                {
                    bool matchingBonusQualified = AriRevShareDistribution.CheckReferalCondition(Member.CurrentId);

                    MatchingBonusLabel.Text = matchingBonusQualified ? "Qualified" : "Not Qualified";
                    DirectRefCountLabel.Text = Member.CurrentInCache.GetDirectReferralsCount().ToString();
                }

                PeopleProfileLink.HRef = HtmlCreator.GetProfileURL(Member.CurrentId, Member.CurrentName);
                if (Member.CurrentInCache.MembershipExpires == null)
                    ExpirationPlaceHolder.Visible = false;
                else
                {
                    MembershipExpiresLiteral.Text = Member.CurrentInCache.FormattedMembershipExpires;

                    if (Member.CurrentInCache.Membership.Id != Membership.Standard.Id && DateTime.Now.AddDays(3) > Member.CurrentInCache.MembershipExpires)
                        expireDateSpan.Attributes.Add("class", "f-s-10 text-danger");
                }

                var isEarner = Member.CurrentInCache.IsEarner;
                var isAdvertiser = Member.CurrentInCache.IsAdvertiser;
                var isPublisher = Member.CurrentInCache.IsPublisher;

                #region Publish
                var pWebsites = AppSettings.TitanFeatures.PublishWebsitesEnabled && isPublisher;
                var pBanners = AppSettings.TitanFeatures.PublishBannersEnabled && isPublisher;
                var pOfferWalls = AppSettings.TitanFeatures.PublishOfferWallsEnabled && isPublisher;
                var pGlobalPostback = AppSettings.TitanFeatures.PublishGlobalPostbackEnabled && isPublisher;
                var pPtcOfferWalls = AppSettings.TitanFeatures.PublishPTCOfferWallsEnabled && isPublisher;
                var pInTextAds = AppSettings.TitanFeatures.PublishInTextAdsEnabled && isPublisher;

                PublishMenu.Visible = AppSettings.TitanFeatures.PublishersRoleEnabled && (pWebsites || pBanners || pOfferWalls || pPtcOfferWalls || pGlobalPostback || pInTextAds);

                Publish1.Visible = pWebsites;
                Publish2.Visible = pBanners;
                Publish3.Visible = pOfferWalls;
                Publish4.Visible = pGlobalPostback;
                Publish5.Visible = pPtcOfferWalls;
                Publish6.Visible = pInTextAds;

                #endregion
                #region Earn

                var eCPAGPT = AppSettings.TitanFeatures.EarnCPAGPTEnabled && isEarner;
                var eAds = AppSettings.TitanFeatures.EarnAdsEnabled && isEarner;
                var eTrafficExchange = AppSettings.TitanFeatures.EarnTrafficExchangeEnabled && isEarner;
                var eSearch = AppSettings.TitanFeatures.EarnSearchEnabled && isEarner;
                var eVideo = AppSettings.TitanFeatures.EarnVideoEnabled && isEarner;
                var eLikes = AppSettings.TitanFeatures.EarnLikesEnabled && isEarner;
                var eTrafficGrid = AppSettings.TitanFeatures.EarnTrafficGridEnabled && isEarner;
                var eOfferwalls = AppSettings.TitanFeatures.EarnOfferwallsEnabled && isEarner;
                var eRefBack = AppSettings.TitanFeatures.EarnRefBackEnabled && isEarner;
                var eCrowdflower = AppSettings.TitanFeatures.EarnCrowdFlowerEnabled && isEarner;
                var ePaidToPromote = AppSettings.TitanFeatures.EarnPaidToPromoteEnabled && isEarner;
                var eCoinhiveClaim = AppSettings.TitanFeatures.EarnCaptchaClaim && isEarner;

                EarnMenu.Visible = AppSettings.TitanFeatures.EarnersRoleEnabled && (eCPAGPT || eAds || eTrafficExchange || eSearch || eVideo || eLikes
                    || eTrafficGrid || eOfferwalls || eRefBack || eCrowdflower || ePaidToPromote || eCoinhiveClaim);

                Earn1.Visible = eCPAGPT;
                Earn2.Visible = eAds;
                Earn4.Visible = eTrafficExchange;
                Earn5.Visible = eSearch;
                Earn6.Visible = eVideo;
                Earn7.Visible = eLikes;
                Earn8.Visible = eTrafficGrid;
                Earn9.Visible = eOfferwalls;
                Earn11.Visible = eRefBack;
                Earn12.Visible = eCrowdflower;
                Earn14.Visible = ePaidToPromote;
                Earn15.Visible = eCoinhiveClaim;

                #endregion
                #region Advertise

                var aCPAGPT = AppSettings.TitanFeatures.AdvertCPAGPTEnabled && isAdvertiser;
                var aBanners = AppSettings.TitanFeatures.AdvertBannersEnabled && isAdvertiser;
                var aAds = AppSettings.TitanFeatures.AdvertAdsEnabled && isAdvertiser;
                var aTrafficExchange = AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled && isAdvertiser;
                var aFacebook = AppSettings.TitanFeatures.AdvertFacebookEnabled && isAdvertiser;
                var aTrafficGrid = AppSettings.TitanFeatures.AdvertTrafficGridEnabled && isAdvertiser;
                var aLoginAds = AppSettings.TitanFeatures.AdvertLoginAdsEnabled && isAdvertiser;
                var aSurfAds = AppSettings.TitanFeatures.AdvertSurfAdsEnabled && isAdvertiser;
                var aPtcOfferWalls = AppSettings.TitanFeatures.AdvertPtcOfferWallEnabled && isAdvertiser;
                var aMyUrls = AppSettings.TitanFeatures.AdvertMyUrlsEnabled && isAdvertiser;
                var aInTextAds = AppSettings.TitanFeatures.AdvertInTextAdsEnabled && isAdvertiser;
                var aMiniVideos = AppSettings.TitanFeatures.AdvertMiniVideoEnabled && isAdvertiser;
                var aPaidToPromote = AppSettings.TitanFeatures.AdvertPaidToPromoteEnabled && isAdvertiser;

                AdvertiseMenu.Visible = AppSettings.TitanFeatures.AdvertisersRoleEnabled && (aCPAGPT || aBanners || aAds || aTrafficExchange || aFacebook || aTrafficGrid ||
                    aSurfAds || aPtcOfferWalls || aMyUrls || aInTextAds || aMiniVideos || aPaidToPromote || aLoginAds);

                Advertise1.Visible = aCPAGPT;
                Advertise2.Visible = aBanners;
                Advertise3.Visible = aAds;
                Advertise5.Visible = aTrafficExchange;
                Advertise6.Visible = aFacebook;
                Advertise7.Visible = aTrafficGrid;
                Advertise9.Visible = aLoginAds;
                Advertise10.Visible = aSurfAds;
                Advertise11.Visible = aMyUrls;
                Advertise12.Visible = aPtcOfferWalls;
                Advertise13.Visible = aInTextAds;
                Advertise14.Visible = aMiniVideos;
                Advertise15.Visible = aPaidToPromote;

                #endregion
                #region Investment Platform
                var iPlans = AppSettings.InvestmentPlatform.InvestmentPlatformEnabled;
                var iQueue = AppSettings.TitanFeatures.InvestmentPlatformQueueSystemEnabled;
                var iHistory = AppSettings.TitanFeatures.InvestmentPlatformHistoryEnabled;
                var iCalculator = AppSettings.TitanFeatures.InvestmentPlatformCalculatorEnabled && !AppSettings.InvestmentPlatform.LevelsEnabled;

                InvestmentPlatformMenu.Visible = iPlans || iQueue || iHistory || iCalculator;
                InvestmentPlanHistory.Visible = iHistory;
                InvestmentPlatformCalculator.Visible = iCalculator;
                InvestmentQueueSystem.Visible = iQueue;
                #endregion
                #region News
                var newsHomepage = AppSettings.TitanFeatures.NewsHomepageEnabled;
                var newsSharing = AppSettings.TitanFeatures.NewsSharingArticlesEnabled;
                var newsWriting = AppSettings.TitanFeatures.NewsWritingArticlesEnabled;

                NewsMenu.Visible = newsHomepage || newsWriting || newsSharing;
                NewsHomepage.Visible = newsHomepage;
                NewsSharingArticles.Visible = newsSharing;
                NewsWritingArticles.Visible = newsWriting;
                #endregion
                #region ICO
                var icoInfo = AppSettings.TitanFeatures.ICOInfoEnabled;
                var icoBuy = AppSettings.TitanFeatures.ICOBuyEnabled;
                var icoStages = AppSettings.TitanFeatures.ICOStagesEnabled;
                var icoHistory = AppSettings.TitanFeatures.ICOHistoryEnabled;
                
                ICOMenu.Visible = icoInfo || icoBuy || icoStages || icoHistory;
                ICOInfo.Visible = icoInfo;
                ICOBuy.Visible = icoBuy;
                ICOHistory.Visible = icoHistory;
                ICOStages.Visible = icoStages;

                #endregion
                #region InternalExchange


                var icoInternalExchange = AppSettings.TitanFeatures.InternalExchangeEnabled;
                var icoInternalExchangeCurrentOrders = AppSettings.TitanFeatures.InternalExchangeCurrentOrdersEnabled;
                var icoInternalExchangeTradingHistory = AppSettings.TitanFeatures.InternalExchangeTradingHistoryEnabled;

                InternalExchangeMenu.Visible = (icoInternalExchange || icoInternalExchangeCurrentOrders || icoInternalExchangeTradingHistory);
                InternalExchangeMain.Visible = icoInternalExchange;
                InternalExchangeOrders.Visible = icoInternalExchangeCurrentOrders;
                InternalExchangeHistory.Visible = icoInternalExchangeTradingHistory;

                #endregion
                #region Revenue Sharing

                var revShare = AppSettings.RevShare.IsRevShareEnabled;
                var eAdPacks = AppSettings.TitanFeatures.EarnAdPacksEnabled && isEarner;
                var aAdPacks = AppSettings.TitanFeatures.AdvertAdPacksEnabled && isEarner;
                var rGroups = AppSettings.TitanFeatures.CustomGroupsEnabled && isEarner;
                var rCalculator = AppSettings.TitanFeatures.AdPacksCalculatorEnabled && isEarner;

                RevenueSharingMenu.Visible = revShare && (eAdPacks || aAdPacks || rGroups || rCalculator);

                RevenueSharing1.Visible = eAdPacks;
                RevenueSharing2.Visible = aAdPacks;
                RevenueSharing3.Visible = rGroups;
                RevenueSharing4.Visible = rCalculator;

                #endregion
                #region CryptocurrencyTradingPlatform
                bool CryptocurrencyBuy = AppSettings.TitanFeatures.CryptocurrencyTradingBuyEnabled;
                bool CryptocurrencySell = AppSettings.TitanFeatures.CryptocurrencyTradingSellEnabled;

                if (!CryptocurrencyBuy && !CryptocurrencySell)
                    CryptocurrencyTradingPlatformMenu.Visible = false;

                CryptocurrencyPlatformBuy.Visible = CryptocurrencyBuy;
                CryptocurrencyPlatformSell.Visible = CryptocurrencySell;
                #endregion
                #region Marketplace

                var aMarketplace = AppSettings.TitanFeatures.AdvertMarketplaceEnabled;
                Marketplace.Visible = aMarketplace;

                #endregion
                #region Referrals

                var fDirect = AppSettings.TitanFeatures.ReferralsDirectEnabled;
                var fIndirect = AppSettings.TitanFeatures.ReferralsIndirectEnabled;
                var fRented = AppSettings.TitanFeatures.ReferralsRentedEnabled;
                var fBanners = AppSettings.TitanFeatures.ReferralsBannersEnabled;
                var fLeadership = AppSettings.TitanFeatures.ReferralsLeadershipEnabled;
                var fRotatorLink = AppSettings.TitanFeatures.ReferralPoolRotatorEnabled;

                ReferralsMenu.Visible = fDirect || fIndirect || fRented || fBanners || fLeadership || fRotatorLink;

                Referrals1.Visible = fDirect;
                Referrals2.Visible = fIndirect;
                Referrals3.Visible = fRented;
                Referrals5.Visible = fBanners;
                Referrals6.Visible = fLeadership;
                Referrals7.Visible = fRotatorLink;
                Referrals8.Visible = AppSettings.TitanFeatures.ReferralMatrixEnabled && AppSettings.Matrix.Type == MatrixType.Referral;

                #endregion
                #region Entertainment

                var eContests = AppSettings.TitanFeatures.EarnContestsEnabled;
                var eJackpot = AppSettings.TitanFeatures.MoneyJackpotEnabled;
                var eGames = AppSettings.TitanFeatures.PeopleGamesEnabled;
                var eTrophies = AppSettings.TitanFeatures.TrophiesEnabled;
                var eDiceGame = AppSettings.TitanFeatures.MoneyDiceGameEnabled && isEarner;
                var eWebinars = AppSettings.TitanFeatures.WebinarsEnabled;
                var eEBooks = AppSettings.TitanFeatures.EBooksEnabled;
                var eSlotMachine = AppSettings.TitanFeatures.SlotMachineEnabled;
                var eMiniVideos = AppSettings.TitanFeatures.EntertainmentMiniVideoEnabled;
                var eRollDiceLottery = AppSettings.TitanFeatures.RollDiceLotteryEnabled;
                var eJackpotPvp = AppSettings.TitanFeatures.JackpotPvpEnabled;

                EntertainmentMenu.Visible = eContests || eJackpot || eGames || eTrophies || eDiceGame || eWebinars || eEBooks || eSlotMachine || eMiniVideos || eRollDiceLottery || eJackpotPvp;

                Entertainment1.Visible = eContests;
                Entertainment2.Visible = eJackpot;
                Entertainment3.Visible = eGames;
                Entertainment4.Visible = eDiceGame;
                Entertainment5.Visible = eTrophies;
                Entertainment6.Visible = eWebinars;
                Entertainment7.Visible = eEBooks;
                Entertainment8.Visible = eSlotMachine;
                Entertainment9.Visible = eMiniVideos;
                Entertainment10.Visible = eRollDiceLottery;
                Entertainment11.Visible = eJackpotPvp;

                #endregion
                #region People

                var pMessages = AppSettings.TitanFeatures.PeopleMessagesEnabled;
                var pFriends = AppSettings.TitanFeatures.PeopleFriendsEnabled;
                var pProfile = AppSettings.TitanFeatures.PeopleProfileEnabled;
                var pTestimonials = AppSettings.TitanFeatures.TestimonialsEnabled;
                var pLeadershipSystem = AppSettings.TitanFeatures.LeaderShipSystemEnabled;
                var pRepresentatives = AppSettings.TitanFeatures.IsRepresentativesEnabled;

                PeopleMenu.Visible = pMessages || pFriends || pProfile || pTestimonials || pLeadershipSystem || pRepresentatives;

                People1.Visible = pMessages;
                People2.Visible = pFriends;
                People3.Visible = pProfile;
                People4.Visible = pTestimonials;
                People5LeadershipSystem.Visible = pLeadershipSystem;
                People5Representatives.Visible = pRepresentatives;

                #endregion
                #region Money

                var mTransfer = AppSettings.TitanFeatures.MoneyTransferEnabled;
                var mPayout = AppSettings.TitanFeatures.MoneyPayoutEnabled;
                var mGiftCards = AppSettings.TitanFeatures.MoneyGiftCardsEnabled;
                var mLogs = AppSettings.TitanFeatures.MoneyLogsEnabled;
                var mCreditLine = AppSettings.TitanFeatures.MoneyCreditLineEnabled;
                var mReceipts = AppSettings.TitanFeatures.MoneyReceiptsEnabled;
                MoneyMenu.Visible = mTransfer || mPayout || mGiftCards || mLogs || mCreditLine || mReceipts;

                Money1.Visible = mTransfer;
                Money2.Visible = mPayout;
                Money3.Visible = mGiftCards;
                Money4.Visible = mLogs;
                Money5.Visible = mCreditLine;
                Money6.Visible = mReceipts;
                #endregion
                #region Statistics

                var sMoney = AppSettings.TitanFeatures.StatisticsMoneyEarnedEnabled;
                var sPoints = AppSettings.TitanFeatures.StatisticsPointsEarnedEnabled;
                var sAdPacks = AppSettings.TitanFeatures.StatisticsAdPacksEnabled && isEarner;
                var sAds = AppSettings.TitanFeatures.StatisticsPTCClicksEnabled;
                var sLeaderboard = AppSettings.TitanFeatures.StatisticsLeaderboardEnabled;

                StatisticsMenu.Visible = sMoney || sPoints || sAdPacks || sAds || sLeaderboard;

                Statistics1.Visible = sMoney;
                Statistics2.Visible = sPoints;
                Statistics4.Visible = sAdPacks;
                Statistics5.Visible = sAds;
                Statistics6.Visible = sLeaderboard;

                #endregion
            }
            catch (Exception ex) { }
        }
        else
            MainPanel.Visible = false;

        //Mark as active current list element
        MarkActiveElement();
    }

    [Obsolete]
    public string ResolveURL(string path)
    {
        return (HttpContext.Current.Handler as Page).ResolveUrl(path);
    }

    #region Marking active menu element

    //For example
    //We don't have a separate menu item for Custom Groups - Add New page, so we want to link it
    //to Groups menu item element (Groups menu item will be highlighted when you are on Custom Groups - Add New page)
    public static Dictionary<string, string> MappedActiveElementsPaths =
        new Dictionary<string, string>()
        {
            { "/user/advert/groups/createcustomgroup.aspx", "/user/advert/groups/list.aspx" },
            { "/user/advert/groups/customgroup.aspx", "/user/advert/groups/list.aspx" },
            { "/user/advert/bannerse.aspx", "/user/advert/bannersoptions.aspx" },
            { "/user/advert/banners.aspx", "/user/advert/bannersoptions.aspx" }
        };

    public void MarkActiveElement()
    {
        string CurrentPath = HttpContext.Current.Request.Url.AbsolutePath;

        if (MappedActiveElementsPaths.ContainsKey(CurrentPath))
            CurrentPath = MappedActiveElementsPaths[CurrentPath];

        MarkActiveElement(CurrentPath);
    }

    private void MarkActiveElement(string CurrentPath)
    {
        try
        {
            foreach (var Control in MainPanel.Controls)
            {
                if (Control is HtmlGenericControl)
                {
                    HtmlGenericControl MainLi = (HtmlGenericControl)Control;
                    foreach (var SubLi in MainLi.Controls)
                    {
                        HtmlAnchor aHref = null;

                        if (SubLi is HtmlGenericControl)
                            aHref = GetAHrefControl((HtmlGenericControl)SubLi);
                        else if (SubLi is HtmlAnchor)
                            aHref = (HtmlAnchor)SubLi;

                        if (aHref != null && aHref.HRef == CurrentPath)
                        {
                            if (SubLi is HtmlGenericControl)
                                ((HtmlGenericControl)SubLi).Attributes["class"] += " active";

                            MainLi.Attributes["class"] += " active";
                            break;
                        }
                    }
                }
            }
        }
        catch (Exception ex) { }
    }

    private HtmlAnchor GetAHrefControl(HtmlGenericControl li)
    {
        HtmlAnchor result = null;
        foreach (Control control in li.Controls)
            if (control is HtmlAnchor)
                result = (HtmlAnchor)control;

        return result;
    }
    #endregion


    #region Notifications
    public string GetNotificationHTML(int number)
    {
        return HtmlCreator.GenerateMenuNotificationNumber(number);
    }

    public string GetNotificationHTML_Important(int number)
    {
        return HtmlCreator.GenerateMenuNotificationNumber_Important(number);
    }

    public int Earn
    {
        get
        {
            return NotificationManager.GetEarnSum();
        }
    }

    public int Ads
    {
        get
        {
            return NotificationManager.Get(NotificationType.NewAds);
        }
    }
    
    public int AdPacksAds
    {
        get
        {
            return NotificationManager.Get(NotificationType.NewAdPacksAds);
        }
    }

    public int CPAOffers
    {
        get
        {
            return NotificationManager.Get(NotificationType.NewCPAOffers);
        }
    }

    public int Facebook
    {
        get
        {
            return NotificationManager.Get(NotificationType.NewFacebookAds);
        }
    }

    public int DirectReferrals
    {
        get
        {
            return NotificationManager.Get(NotificationType.NewDirectReferrals);
        }
    }

    public int Referrals
    {
        get
        {
            return NotificationManager.Get(NotificationType.NewDirectReferrals) +
                NotificationManager.Get(NotificationType.UnassignedMatrixMembers);
        }
    }

    public int Messages
    {
        get
        {
            return NotificationManager.Get(NotificationType.NewMessages);
        }
    }

    public int UnassignedMatrixMembers
    {
        get
        {
            return NotificationManager.Get(NotificationType.UnassignedMatrixMembers);
        }
    }

    public int PendingRepresentativePaymentRequest
    {
        get
        {
            return NotificationManager.Get(NotificationType.PendingRepresentativePaymentRequest);
        }
    }
    #endregion
}