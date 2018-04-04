using Prem.PTC.Utils;
using System.Reflection;


namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static partial class TitanFeatures
        {
            #region Available Roles Menu
            public static bool EarnersRoleEnabled { get { return GetValueIfNotDemo(appSettings.EarnersRoleEnabled, TitanProduct.Core); } set { appSettings.EarnersRoleEnabled = value; } }
            public static bool AdvertisersRoleEnabled { get { return GetValueIfNotDemo(appSettings.AdvertisersRoleEnabled, TitanProduct.Core); } set { appSettings.AdvertisersRoleEnabled = value; } }
            public static bool PublishersRoleEnabled { get { return GetValueIfNotDemo(appSettings.PublishersRoleEnabled, TitanProduct.AffiliateNetwork); } set { appSettings.PublishersRoleEnabled = value; } }
            #endregion

            #region Earn Menu
            public static bool EarnCPAGPTEnabled { get { return GetValueIfNotDemo(appSettings.EarnCPAGPTEnabled, TitanProduct.GPTPTC); } set { appSettings.EarnCPAGPTEnabled = value; } }
            public static bool EarnSearchEnabled
            {
                get
                {
                    return GetValueIfNotDemo(AppSettings.SearchAndVideo.SearchMode == GPTSeachMode.Off ? false : true, TitanProduct.GPTPTC);
                }
                set
                { AppSettings.SearchAndVideo.SearchMode = value == true ? GPTSeachMode.PayPerSearch : GPTSeachMode.Off; }
            }
            public static bool EarnOfferwallsEnabled { get { return GetValueIfNotDemo(appSettings.EarnOfferwallsEnabled, TitanProduct.GPTPTC); } set { appSettings.EarnOfferwallsEnabled = value; } }
            public static bool EarnAdsEnabled { get { return GetValueIfNotDemo(appSettings.EarnAdsEnabled, TitanProduct.GPTPTC); } set { appSettings.EarnAdsEnabled = value; } }
            public static bool HadCashLinksEnabledBefore6010Update { get { return appSettings.EarnCashLinksEnabled; } set { appSettings.EarnCashLinksEnabled = value; } }
            public static bool EarnVideoEnabled
            {
                get
                {
                    return GetValueIfNotDemo(AppSettings.SearchAndVideo.VideoMode == GPTVideoMode.Off ? false : true, TitanProduct.GPTPTC);
                }
                set
                {
                    AppSettings.SearchAndVideo.VideoMode = value == true ? GPTVideoMode.PayPerWatch : GPTVideoMode.Off;
                }
            }
            public static bool EarnContestsEnabled { get { return GetValueIfNotDemo(appSettings.EarnContestsEnabled, TitanProduct.Core); } set { appSettings.EarnContestsEnabled = value; } }
            public static bool EarnAdPacksEnabled { get { return GetValueIfNotDemo(appSettings.EarnAdPacksEnabled, TitanProduct.RevenueSharing); } set { appSettings.EarnAdPacksEnabled = value; } }
            public static bool EarnLikesEnabled { get { return GetValueIfNotDemo(appSettings.EarnLikesEnabled, TitanProduct.GPTPTC); } set { appSettings.EarnLikesEnabled = value; } }
            public static bool EarnRefBackEnabled { get { return GetValueIfNotDemo(appSettings.EarnRefBackEnabled, TitanProduct.GPTPTC); } set { appSettings.EarnRefBackEnabled = value; } }
            public static bool EarnTrafficExchangeEnabled { get { return GetValueIfNotDemo(appSettings.EarnTrafficExchangeEnabled, TitanProduct.GPTPTC); } set { appSettings.EarnTrafficExchangeEnabled = value; } }
            public static bool EarnTrafficGridEnabled { get { return GetValueIfNotDemo(appSettings.EarnTrafficGridEnabled, TitanProduct.GPTPTC); } set { appSettings.EarnTrafficGridEnabled = value; } }
            public static bool EarnCrowdFlowerEnabled { get { return GetValueIfNotDemo(appSettings.EarnCrowdFlowerEnabled, TitanProduct.GPTPTC); } set { appSettings.EarnCrowdFlowerEnabled = value; } }
            public static bool EarnPaidToPromoteEnabled { get { return GetValueIfNotDemo(appSettings.EarnPaidToPromoteEnabled, TitanProduct.GPTPTC); } set { appSettings.EarnPaidToPromoteEnabled = value; } }
            public static bool EarnCaptchaClaim { get { return GetValueIfNotDemo(appSettings.EarnCaptchaClaim, TitanProduct.Core); } set { appSettings.EarnCaptchaClaim = value; } }
            #endregion

            #region Advertise Menu
            public static bool AdvertCPAGPTEnabled { get { return GetValueIfNotDemo(appSettings.AdvertCPAGPTEnabled, TitanProduct.GPTPTC, TitanProduct.AffiliateNetwork); } set { appSettings.AdvertCPAGPTEnabled = value; } }
            public static bool AdvertTrafficExchangeEnabled { get { return GetValueIfNotDemo(appSettings.AdvertTrafficExchangeEnabled, TitanProduct.GPTPTC); } set { appSettings.AdvertTrafficExchangeEnabled = value; } }
            public static bool AdvertBannersEnabled { get { return GetValueIfNotDemo(appSettings.AdvertBannersEnabled, TitanProduct.GPTPTC, TitanProduct.RevenueSharing); } set { appSettings.AdvertBannersEnabled = value; } }
            public static bool AdvertFacebookEnabled { get { return GetValueIfNotDemo(appSettings.AdvertFacebookEnabled, TitanProduct.GPTPTC); } set { appSettings.AdvertFacebookEnabled = value; } }
            public static bool AdvertAdsEnabled { get { return GetValueIfNotDemo(appSettings.AdvertAdsEnabled, TitanProduct.GPTPTC); } set { appSettings.AdvertAdsEnabled = value; } }
            public static bool AdvertTrafficGridEnabled { get { return GetValueIfNotDemo(appSettings.AdvertTrafficGridEnabled, TitanProduct.GPTPTC); } set { appSettings.AdvertTrafficGridEnabled = value; } }
            public static bool AdvertAdPacksEnabled { get { return GetValueIfNotDemo(appSettings.AdvertAdPacksEnabled, TitanProduct.RevenueSharing); } set { appSettings.AdvertAdPacksEnabled = value; } }
            public static bool AdvertLoginAdsEnabled { get { return GetValueIfNotDemo(appSettings.AdvertLoginAdsEnabled, TitanProduct.Core); } set { appSettings.AdvertLoginAdsEnabled = value; } }
            public static bool AdvertSurfAdsEnabled { get { return GetValueIfNotDemo(appSettings.AdvertSurfAdsEnabled, TitanProduct.RevenueSharing); } set { appSettings.AdvertSurfAdsEnabled = value; } }
            public static bool AdvertMarketplaceEnabled { get { return GetValueIfNotDemo(appSettings.AdvertMarketplaceEnabled, TitanProduct.Core); } set { appSettings.AdvertMarketplaceEnabled = value; } }
            public static bool AdvertPtcOfferWallEnabled { get { return GetValueIfNotDemo(appSettings.AdvertPtcOfferWallEnabled, TitanProduct.AffiliateNetwork); } set { appSettings.AdvertPtcOfferWallEnabled = value; } }
            public static bool AdvertInTextAdsEnabled { get { return GetValueIfNotDemo(appSettings.AdvertInTextAdsEnabled, TitanProduct.AffiliateNetwork); } set { appSettings.AdvertInTextAdsEnabled = value; } }
            public static bool AdvertMyUrlsEnabled { get { return GetValueIfNotDemo(appSettings.AdvertMyUrlsEnabled, TitanProduct.AffiliateNetwork); } set { appSettings.AdvertMyUrlsEnabled = value; } }
            public static bool AdvertMiniVideoEnabled { get { return GetValueIfNotDemo(appSettings.AdvertMiniVideoEnabled, TitanProduct.GPTPTC); } set { appSettings.AdvertMiniVideoEnabled = value; } }
            public static bool AdvertPaidToPromoteEnabled { get { return GetValueIfNotDemo(appSettings.AdvertPaidToPromoteEnabled, TitanProduct.GPTPTC); } set { appSettings.AdvertPaidToPromoteEnabled = value; } }
            #endregion

            #region News Menu
            public static bool NewsHomepageEnabled { get { return GetValueIfNotDemo(appSettings.NewsHomepageEnabled, TitanProduct.News); } set { appSettings.NewsHomepageEnabled = value; } }
            public static bool NewsSharingArticlesEnabled { get { return GetValueIfNotDemo(appSettings.NewsSharingArticlesEnabled, TitanProduct.News); } set { appSettings.NewsSharingArticlesEnabled = value; } }
            public static bool NewsWritingArticlesEnabled { get { return GetValueIfNotDemo(appSettings.NewsWritingArticlesEnabled, TitanProduct.News); } set { appSettings.NewsWritingArticlesEnabled = value; } }
            #endregion 

            #region Invest Menu
            public static bool InvestmentPlatformHistoryEnabled
            {
                get { return GetValueIfNotDemo(appSettings.InvestmentPlatformHistoryEnabled, TitanProduct.InvestmentPlatform); }
                set { appSettings.InvestmentPlatformHistoryEnabled = value; }
            }
            public static bool InvestmentPlatformCalculatorEnabled
            {
                get { return GetValueIfNotDemo(appSettings.InvestmentPlatformCalculatorEnabled, TitanProduct.InvestmentPlatform); }
                set { appSettings.InvestmentPlatformCalculatorEnabled = value; }
            }
            public static bool InvestmentPlatformQueueSystemEnabled
            {
                get { return GetValueIfNotDemo(appSettings.InvestmentPlatformQueueSystemEnabled, TitanProduct.InvestmentPlatform); }
                set { appSettings.InvestmentPlatformQueueSystemEnabled = value; }
            }
            #endregion

            #region Referrals Menu
            public static bool ReferralsDirectEnabled { get { return GetValueIfNotDemo(appSettings.ReferralsDirectEnabled, TitanProduct.Core); } set { appSettings.ReferralsDirectEnabled = value; } }
            public static bool ReferralsBannersEnabled { get { return GetValueIfNotDemo(appSettings.ReferralsBannersEnabled, TitanProduct.Core); } set { appSettings.ReferralsBannersEnabled = value; } }
            public static bool ReferralsIndirectEnabled { get { return GetValueIfNotDemo(appSettings.ReferralsIndirectEnabled, TitanProduct.Core); } set { appSettings.ReferralsIndirectEnabled = value; } }
            public static bool ReferralsRentedEnabled { get { return GetValueIfNotDemo(appSettings.ReferralsRentedEnabled, TitanProduct.Core); } set { appSettings.ReferralsRentedEnabled = value; } }
            public static bool ReferralsStatsEnabled { get { return GetValueIfNotDemo(appSettings.ReferralsStatsEnabled, TitanProduct.Core); } set { appSettings.ReferralsStatsEnabled = value; } }
            public static bool ReferralsLeadershipEnabled { get { return GetValueIfNotDemo(appSettings.ReferralsLeadershipEnabled, TitanProduct.Core); } set { appSettings.ReferralsLeadershipEnabled = value; } }
            public static bool ReferralPoolRotatorEnabled { get { return GetValueIfNotDemo(appSettings.ReferralPoolRotatorEnabled, TitanProduct.Core); } set { appSettings.ReferralPoolRotatorEnabled = value; } }            
            public static bool ReferralMatrixEnabled { get { return GetValueIfNotDemo(appSettings.ReferralMatrixEnabled, TitanProduct.Core); } set { appSettings.ReferralMatrixEnabled = value; } }
            #endregion

            #region Revenue Sharing Menu

            public static bool CustomGroupsEnabled
            {
                get
                {
                    return GetValueIfNotDemo(AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.CustomGroups || AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticAndCustomGroups, TitanProduct.RevenueSharing);
                }
            }
            public static bool AdPacksCalculatorEnabled { get { return GetValueIfNotDemo(appSettings.AdPacksCalculatorEnabled, TitanProduct.RevenueSharing); } set { appSettings.AdPacksCalculatorEnabled = value; } }
   
            #endregion

            #region People Menu

            public static bool PeopleMessagesEnabled { get { return GetValueIfNotDemo(appSettings.PeopleMessagesEnabled, TitanProduct.Core); } set { appSettings.PeopleMessagesEnabled = value; } }
            public static bool PeopleFriendsEnabled { get { return GetValueIfNotDemo(appSettings.PeopleFriendsEnabled, TitanProduct.Core); } set { appSettings.PeopleFriendsEnabled = value; } }
            public static bool PeopleGamesEnabled { get { return GetValueIfNotDemo(appSettings.PeopleGamesEnabled, TitanProduct.GPTPTC); } set { appSettings.PeopleGamesEnabled = value; } }
            public static bool PeopleProfileEnabled { get { return GetValueIfNotDemo(appSettings.PeopleProfileEnabled, TitanProduct.Core); } set { appSettings.PeopleProfileEnabled = value; } }
            public static bool LeaderShipSystemEnabled { get { return GetValueIfNotDemo(appSettings.LeaderShipSystemEnabled, TitanProduct.Core); } set { appSettings.LeaderShipSystemEnabled = value; } }
            public static bool IsRepresentativesEnabled { get { return GetValueIfNotDemo(appSettings.IsRepresentativesEnabled, TitanProduct.Core); } set { appSettings.IsRepresentativesEnabled = value; } }

            #endregion

            #region Upgrade Menu

            public static bool UpgradeEnabled { get { return GetValueIfNotDemo(appSettings.UpgradeEnabled, TitanProduct.Core); } set { appSettings.UpgradeEnabled = value; } }

            #endregion

            #region Money Menu

            public static bool MoneyTransferEnabled { get { return GetValueIfNotDemo(appSettings.MoneyTransferEnabled, TitanProduct.Core); } set { appSettings.MoneyTransferEnabled = value; } }
            public static bool MoneyLogsEnabled { get { return GetValueIfNotDemo(appSettings.MoneyLogsEnabled, TitanProduct.Core); } set { appSettings.MoneyLogsEnabled = value; } }
            public static bool MoneyPayoutEnabled { get { return GetValueIfNotDemo(appSettings.MoneyPayoutEnabled, TitanProduct.Core); } set { appSettings.MoneyPayoutEnabled = value; } }
            public static bool MoneyDiceGameEnabled { get { return GetValueIfNotDemo(appSettings.IsDiceGameEnabled, TitanProduct.Core); } set { appSettings.IsDiceGameEnabled = value; } }
            public static bool MoneyGiftCardsEnabled
            {
                get
                {
                    return GetValueIfNotDemo(AppSettings.GiftCards.Mode == GiftCardMode.Off ? false : true, TitanProduct.Core);
                }
                set
                {
                    AppSettings.GiftCards.Mode = value == true ? GiftCardMode.OnWithManual : GiftCardMode.Off;
                }
            }

            public static bool MoneyCreditLineEnabled { get { return GetValueIfNotDemo(appSettings.MoneyCreditLineEnabled, TitanProduct.Core); } set { appSettings.MoneyCreditLineEnabled = value; } }
            public static bool MoneyJackpotEnabled { get { return GetValueIfNotDemo(appSettings.MoneyJackpotEnabled, TitanProduct.Core); } set { appSettings.MoneyJackpotEnabled = value; } }
            public static bool MoneyReceiptsEnabled { get { return GetValueIfNotDemo(appSettings.MoneyReceiptsEnabled, TitanProduct.Core); } set { appSettings.MoneyReceiptsEnabled = value; } }
            #endregion

            #region Trophies Menu
            public static bool TrophiesEnabled { get { return GetValueIfNotDemo(appSettings.TrophiesEnabled, TitanProduct.Core); } set { appSettings.TrophiesEnabled = value; } }
            #endregion

            #region Statistics Menu
            public static bool StatisticsMoneyEarnedEnabled { get { return GetValueIfNotDemo(appSettings.StatisticsMoneyEarnedEnabled, TitanProduct.Core); } set { appSettings.StatisticsMoneyEarnedEnabled = value; } }
            public static bool StatisticsPointsEarnedEnabled { get { return GetValueIfNotDemo(appSettings.StatisticsPointsEarnedEnabled, TitanProduct.Core); } set { appSettings.StatisticsPointsEarnedEnabled = value; } }
            public static bool StatisticsAdPacksEnabled { get { return GetValueIfNotDemo(appSettings.StatisticsAdPacksEnabled, TitanProduct.RevenueSharing); } set { appSettings.StatisticsAdPacksEnabled = value; } }
            public static bool StatisticsPTCClicksEnabled { get { return GetValueIfNotDemo(appSettings.StatisticsPTCClicksEnabled, TitanProduct.GPTPTC); } set { appSettings.StatisticsPTCClicksEnabled = value; } }
            public static bool StatisticsLeaderboardEnabled { get { return GetValueIfNotDemo(appSettings.StatisticsLeaderboardEnabled, TitanProduct.Core); } set { appSettings.StatisticsLeaderboardEnabled = value; } }
            #endregion

            #region Other
            public static bool TestimonialsEnabled { get { return GetValueIfNotDemo(appSettings.TestimonialsEnabled, TitanProduct.Core); } set { appSettings.TestimonialsEnabled = value; } }
            public static bool PaymentProofsEnabled { get { return GetValueIfNotDemo(appSettings.PaymentProofsEnabled, TitanProduct.Core); } set { appSettings.PaymentProofsEnabled = value; } }
            public static bool OfferLevelsEnabled { get { return GetValueIfNotDemo(appSettings.OfferLevelsEnabled, TitanProduct.Core); } set { appSettings.OfferLevelsEnabled = value; } }

            public static bool QuickStartGuideEnabled { get { return GetValueIfNotDemo(appSettings.QuickStartGuideEnabled, TitanProduct.Core); } set { appSettings.QuickStartGuideEnabled = value; } }
            #endregion

            #region Network
            public static bool SocialNetworkEnabled { get { return GetValueIfNotDemo(appSettings.SocialNetworkEnabled, TitanProduct.Core); } set { appSettings.SocialNetworkEnabled = value; } }
            #endregion

            #region Publish
            public static bool PublishInTextAdsEnabled { get { return GetValueIfNotDemo(appSettings.PublishInTextAdsEnabled, TitanProduct.AffiliateNetwork); } set { appSettings.PublishInTextAdsEnabled = value; } }
            public static bool PublishWebsitesEnabled { get { return GetValueIfNotDemo(appSettings.PublishWebsitesEnabled, TitanProduct.AffiliateNetwork); } set { appSettings.PublishWebsitesEnabled = value; } }
            public static bool PublishBannersEnabled { get { return GetValueIfNotDemo(appSettings.PublishBannersEnabled, TitanProduct.AffiliateNetwork); } set { appSettings.PublishBannersEnabled = value; } }
            public static bool PublishOfferWallsEnabled { get { return GetValueIfNotDemo(appSettings.PublishOfferWallsEnabled, TitanProduct.AffiliateNetwork); } set { appSettings.PublishOfferWallsEnabled = value; } }
            public static bool PublishPTCOfferWallsEnabled { get { return GetValueIfNotDemo(appSettings.PublishPTCOfferWallsEnabled, TitanProduct.AffiliateNetwork); } set { appSettings.PublishPTCOfferWallsEnabled = value; } }
            public static bool PublishGlobalPostbackEnabled { get { return GetValueIfNotDemo(appSettings.PublishGlobalPostbackEnabled, TitanProduct.AffiliateNetwork); } set { appSettings.PublishGlobalPostbackEnabled = value; } }
            #endregion

            #region Cryptocurrency Trading Platform
            public static bool CryptocurrencyTradingBuyEnabled { get { return GetValueIfNotDemo(appSettings.CryptocurrencyTradingBuyEnabled, TitanProduct.CryptocurrencyTrading); } set { appSettings.CryptocurrencyTradingBuyEnabled = value; } }
            public static bool CryptocurrencyTradingSellEnabled { get { return GetValueIfNotDemo(appSettings.CryptocurrencyTradingSellEnabled, TitanProduct.CryptocurrencyTrading); } set { appSettings.CryptocurrencyTradingSellEnabled = value; } }
            #endregion 

            #region Entertainment
            public static bool WebinarsEnabled { get { return GetValueIfNotDemo(appSettings.WebinarsEnabled, TitanProduct.Core); } set { appSettings.WebinarsEnabled = value; } }
            public static bool EBooksEnabled { get { return GetValueIfNotDemo(appSettings.EBooksEnabled, TitanProduct.Core); } set { appSettings.EBooksEnabled = value; } }
            public static bool SlotMachineEnabled { get { return GetValueIfNotDemo(appSettings.SlotMachineEnabled, TitanProduct.Core); } set { appSettings.SlotMachineEnabled = value; } }
            public static bool EntertainmentMiniVideoEnabled { get { return GetValueIfNotDemo(appSettings.EntertainmentMiniVideoEnabled, TitanProduct.GPTPTC); } set { appSettings.EntertainmentMiniVideoEnabled = value; } }
            public static bool RollDiceLotteryEnabled { get { return GetValueIfNotDemo(appSettings.RollDiceLotteryEnabled, TitanProduct.Core); }set { appSettings.RollDiceLotteryEnabled = value; } }

            public static bool JackpotPvpEnabled { get { return GetValueIfNotDemo(appSettings.JackpotPvpEnabled, TitanProduct.Core); } set { appSettings.JackpotPvpEnabled = value; } }
            #endregion

            #region ICO
            public static bool ICOInfoEnabled
            {
                get { return appSettings.ICOInfoEnabled; }
                set { appSettings.ICOInfoEnabled = value; }
            }

            public static bool ICOBuyEnabled
            {
                get { return appSettings.ICOBuyEnabled; }
                set { appSettings.ICOBuyEnabled = value; }
            }

            public static bool ICOHistoryEnabled
            {
                get { return appSettings.ICOHistoryEnabled; }
                set { appSettings.ICOHistoryEnabled = value; }
            }

            public static bool ICOStagesEnabled
            {
                get { return appSettings.ICOStagesEnabled; }
                set { appSettings.ICOStagesEnabled = value; }
            }

            #region InternalExchange
            public static bool InternalExchangeEnabled
            {
                get { return appSettings.InternalExchangeEnabled; }
                set { appSettings.InternalExchangeEnabled = value; }
            }
            public static bool InternalExchangeCurrentOrdersEnabled
            {
                get { return appSettings.InternalExchangeCurrentOrdersEnabled; }
                set { appSettings.InternalExchangeCurrentOrdersEnabled = value; }
            }

            public static bool InternalExchangeTradingHistoryEnabled
            {
                get { return appSettings.InternalExchangeTradingHistoryEnabled; }
                set { appSettings.InternalExchangeTradingHistoryEnabled = value; }
            }
            #endregion

            #endregion


            public static void Save()
            {
                appSettings.SaveTitanFeatures();
            }
            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded())
                    appSettings.ReloadTitanFeatures();
            }

            public static bool GetValueIfNotDemo(bool originalValue, TitanProduct requiredProduct)
            {
                if (AppSettings.IsDemo && AppSettings.Side == ScriptSide.Client && requiredProduct != TitanProduct.Core && UseTitanDemoHelper.IsProductsSet())
                    return UseTitanDemoHelper.GetProducts().Contains(((int)requiredProduct).ToString());

                return originalValue;
            }

            public static bool GetValueIfNotDemo(bool originalValue, TitanProduct requiredProduct1, TitanProduct requiredProduct2)
            {
                return GetValueIfNotDemo(originalValue, requiredProduct1) || GetValueIfNotDemo(originalValue, requiredProduct2);
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            #region Available Roles Columns

            [Column("EarnersRoleEnabled")]
            internal bool EarnersRoleEnabled
            {
                get { return _EarnersRoleEnabled; }
                set { _EarnersRoleEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AdvertisersRoleEnabled")]
            internal bool AdvertisersRoleEnabled
            {
                get { return _AdvertisersRoleEnabled; }
                set { _AdvertisersRoleEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("PublishersRoleEnabled")]
            internal bool PublishersRoleEnabled
            {
                get { return _PublishersRoleEnabled; }
                set { _PublishersRoleEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _EarnersRoleEnabled, _AdvertisersRoleEnabled, _PublishersRoleEnabled;

            #endregion
            #region Earn Columns

            [Column("EarnCPAGPTEnabled")]
            internal bool EarnCPAGPTEnabled
            {
                get { return _EarnCPAGPTEnabled; }
                set { _EarnCPAGPTEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("EarnOfferwallsEnabled")]
            internal bool EarnOfferwallsEnabled
            {
                get { return _EarnOfferwallsEnabled; }
                set { _EarnOfferwallsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("EarnAdsEnabled")]
            internal bool EarnAdsEnabled
            {
                get { return _EarnAdsEnabled; }
                set { _EarnAdsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("EarnCashLinksEnabled")]
            internal bool EarnCashLinksEnabled
            {
                get { return _EarnCashLinksEnabled; }
                set { _EarnCashLinksEnabled = value; SetUpToDateAsFalse();}
            }
    
            [Column("EarnContestsEnabled")]
            internal bool EarnContestsEnabled
            {
                get { return _EarnContestsEnabled; }
                set { _EarnContestsEnabled = value; SetUpToDateAsFalse(); }
            }
            [Column("EarnAdPacksEnabled")]
            internal bool EarnAdPacksEnabled
            {
                get { return _EarnAdPacksEnabled; }
                set { _EarnAdPacksEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("EarnLikesEnabled")]
            internal bool EarnLikesEnabled
            {
                get { return _EarnLikesEnabled; }
                set { _EarnLikesEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("EarnRefBackEnabled")]
            internal bool EarnRefBackEnabled
            {
                get { return _EarnRefBackEnabled; }
                set { _EarnRefBackEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("EarnTrafficExchangeEnabled")]
            internal bool EarnTrafficExchangeEnabled
            {
                get { return _EarnTrafficExchangeEnabled; }
                set { _EarnTrafficExchangeEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("EarnTrafficGridEnabled")]
            internal bool EarnTrafficGridEnabled
            {
                get { return _EarnTrafficGridEnabled; }
                set { _EarnTrafficGridEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("EarnCrowdFlowerEnabled")]
            internal bool EarnCrowdFlowerEnabled
            {
                get { return _EarnCrowdFlowerEnabled; }
                set { _EarnCrowdFlowerEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("EarnPaidToPromoteEnabled")]
            internal bool EarnPaidToPromoteEnabled
            {
                get { return _EarnPaidToPromoteEnabled; }
                set { _EarnPaidToPromoteEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("EarnCaptchaClaim")]
            internal bool EarnCaptchaClaim
            {
                get { return _EarnCaptchaClaim; }
                set { _EarnCaptchaClaim = value; SetUpToDateAsFalse(); }
            }

            bool _EarnCrowdFlowerEnabled, _EarnTrafficExchangeEnabled, _EarnRefBackEnabled, _EarnLikesEnabled, _EarnAdPacksEnabled, _EarnContestsEnabled,
                 _EarnAdsEnabled, _EarnOfferwallsEnabled, _EarnCPAGPTEnabled, _EarnTrafficGridEnabled, _EarnCashLinksEnabled, _EarnPaidToPromoteEnabled,
                 _EarnCaptchaClaim;
            
            #endregion
            #region Advertise Columns

            [Column("AdvertCPAGPTEnabled")]
            internal bool AdvertCPAGPTEnabled
            {
                get { return _AdvertCPAGPTEnabled; }
                set { _AdvertCPAGPTEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AdvertTrafficExchangeEnabled")]
            internal bool AdvertTrafficExchangeEnabled
            {
                get { return _AdvertTrafficExchangeEnabled; }
                set { _AdvertTrafficExchangeEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AdvertBannersEnabled")]
            internal bool AdvertBannersEnabled
            {
                get { return _AdvertBannersEnabled; }
                set { _AdvertBannersEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AdvertFacebookEnabled")]
            internal bool AdvertFacebookEnabled
            {
                get { return _AdvertFacebookEnabled; }
                set { _AdvertFacebookEnabled = value; SetUpToDateAsFalse(); }
            }
            [Column("AdvertAdsEnabled")]
            internal bool AdvertAdsEnabled
            {
                get { return _AdvertAdsEnabled; }
                set { _AdvertAdsEnabled = value; SetUpToDateAsFalse(); }
            }
            [Column("AdvertTrafficGridEnabled")]
            internal bool AdvertTrafficGridEnabled
            {
                get { return _AdvertTrafficGridEnabled; }
                set { _AdvertTrafficGridEnabled = value; SetUpToDateAsFalse(); }
            }
            [Column("AdvertAdPacksEnabled")]
            internal bool AdvertAdPacksEnabled
            {
                get { return _AdvertAdPacksEnabled; }
                set { _AdvertAdPacksEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AdvertLoginAdsEnabled")]
            internal bool AdvertLoginAdsEnabled
            {
                get { return _AdvertLoginAdsEnabled; }
                set { _AdvertLoginAdsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AdvertSurfAdsEnabled")]
            internal bool AdvertSurfAdsEnabled
            {
                get { return _AdvertSurfAdsEnabled; }
                set { _AdvertSurfAdsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AdvertMarketplaceEnabled")]
            internal bool AdvertMarketplaceEnabled
            {
                get { return _AdvertMarketplaceEnabled; }
                set { _AdvertMarketplaceEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AdvertPtcOfferWallEnabled")]
            internal bool AdvertPtcOfferWallEnabled
            {
                get { return _AdvertPtcOfferWallEnabled; }
                set { _AdvertPtcOfferWallEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AdvertInTextAdsEnabled")]
            internal bool AdvertInTextAdsEnabled
            {
                get { return _AdvertInTextAdsEnabled; }
                set { _AdvertInTextAdsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AdvertMyUrlsEnabled")]
            internal bool AdvertMyUrlsEnabled
            {
                get { return _AdvertMyUrlsEnabled; }
                set { _AdvertMyUrlsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AdvertMiniVideoEnabled")]
            internal bool AdvertMiniVideoEnabled
            {
                get { return _AdvertMiniVideoEnabled; }
                set { _AdvertMiniVideoEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AdvertPaidToPromoteEnabled")]
            internal bool AdvertPaidToPromoteEnabled
            {
                get { return _AdvertPaidToPromoteEnabled; }
                set { _AdvertPaidToPromoteEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _AdvertCPAGPTEnabled, _AdvertTrafficExchangeEnabled, _AdvertCashLinksEnabled, _AdvertLoginAdsEnabled, _AdvertMarketplaceEnabled,
                _AdvertBannersEnabled, _AdvertFacebookEnabled, _AdvertAdsEnabled, _AdvertTrafficGridEnabled, _AdvertAdPacksEnabled, _AdvertSurfAdsEnabled,
                _AdvertPtcOfferWallEnabled, _AdvertInTextAdsEnabled, _AdvertMyUrlsEnabled, _AdvertMiniVideoEnabled, _AdvertPaidToPromoteEnabled;

            #endregion
            #region News Columns

            [Column("NewsHomepageEnabled")]
            internal bool NewsHomepageEnabled
            {
                get { return _NewsHomepageEnabled; }
                set { _NewsHomepageEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("NewsSharingArticlesEnabled")]
            internal bool NewsSharingArticlesEnabled
            {
                get { return _NewsSharingArticlesEnabled; }
                set { _NewsSharingArticlesEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("NewsWritingArticlesEnabled")]
            internal bool NewsWritingArticlesEnabled
            {
                get { return _NewsWritingArticlesEnabled; }
                set { _NewsWritingArticlesEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _NewsHomepageEnabled, _NewsSharingArticlesEnabled, _NewsWritingArticlesEnabled;

            #endregion
            #region Referrals Columns

            [Column("ReferralsDirectEnabled")]
            internal bool ReferralsDirectEnabled
            {
                get { return _ReferralsDirectEnabled; }
                set { _ReferralsDirectEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("ReferralsBannersEnabled")]
            internal bool ReferralsBannersEnabled
            {
                get { return _ReferralsBannersEnabled; }
                set { _ReferralsBannersEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("ReferralsIndirectEnabled")]
            internal bool ReferralsIndirectEnabled
            {
                get { return _ReferralsIndirectEnabled; }
                set { _ReferralsIndirectEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("ReferralsRentedEnabled")]
            internal bool ReferralsRentedEnabled
            {
                get { return _ReferralsRentedEnabled; }
                set { _ReferralsRentedEnabled = value; SetUpToDateAsFalse(); }
            }
            [Column("ReferralsStatsEnabled")]
            internal bool ReferralsStatsEnabled
            {
                get { return _ReferralsStatsEnabled; }
                set { _ReferralsStatsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("ReferralsLeadershipEnabled")]
            internal bool ReferralsLeadershipEnabled
            {
                get { return _ReferralsLeadershipEnabled; }
                set { _ReferralsLeadershipEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("ReferralPoolRotatorEnabled")]
            internal bool ReferralPoolRotatorEnabled
            {
                get { return _ReferralPoolRotatorEnabled; }
                set { _ReferralPoolRotatorEnabled = value; SetUpToDateAsFalse(); }
            }
            
            [Column("IsRepresentativesEnabled")]
            internal bool IsRepresentativesEnabled
            {
                get { return _IsRepresentativesEnabled; }
                set { _IsRepresentativesEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("ReferralMatrixEnabled")]
            internal bool ReferralMatrixEnabled
            {
                get { return _ReferralMatrixEnabled; }
                set { _ReferralMatrixEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _ReferralsDirectEnabled, _ReferralsBannersEnabled, _ReferralsIndirectEnabled, _ReferralsStatsEnabled, _ReferralsRentedEnabled,
                _ReferralsLeadershipEnabled, _ReferralPoolRotatorEnabled, _IsRepresentativesEnabled, _ReferralMatrixEnabled;

            #endregion
            #region People Columns

            [Column("PeopleMessagesEnabled")]
            internal bool PeopleMessagesEnabled
            {
                get { return _PeopleMessagesEnabled; }
                set { _PeopleMessagesEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("PeopleFriendsEnabled")]
            internal bool PeopleFriendsEnabled
            {
                get { return _PeopleFriendsEnabled; }
                set { _PeopleFriendsEnabled = value; SetUpToDateAsFalse(); }
            }
            [Column("PeopleGamesEnabled")]
            internal bool PeopleGamesEnabled
            {
                get { return _PeopleGamesEnabled; }
                set { _PeopleGamesEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("PeopleProfileEnabled")]
            internal bool PeopleProfileEnabled
            {
                get { return _PeopleProfileEnabled; }
                set { _PeopleProfileEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("LeaderShipSystemEnabled")]
            internal bool LeaderShipSystemEnabled
            {
                get { return _LeaderShipSystemEnabled; }
                set { _LeaderShipSystemEnabled = value; SetUpToDateAsFalse(); }
            }           

            bool _PeopleMessagesEnabled, _PeopleFriendsEnabled, _PeopleGamesEnabled, _PeopleProfileEnabled, _LeaderShipSystemEnabled;

            #endregion
            #region Upgrade Columns

            [Column("UpgradeEnabled")]
            internal bool UpgradeEnabled
            {
                get { return _UpgradeEnabled; }
                set { _UpgradeEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _UpgradeEnabled;

            #endregion
            #region Money Columns

            [Column("MoneyTransferEnabled")]
            internal bool MoneyTransferEnabled
            {
                get { return _MoneyTransferEnabled; }
                set { _MoneyTransferEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("MoneyLogsEnabled")]
            internal bool MoneyLogsEnabled
            {
                get { return _MoneyLogsEnabled; }
                set { _MoneyLogsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("MoneyPayoutEnabled")]
            internal bool MoneyPayoutEnabled
            {
                get { return _MoneyPayoutEnabled; }
                set { _MoneyPayoutEnabled = value; SetUpToDateAsFalse(); }
            }


            [Column("MoneyJackpotEnabled")]
            internal bool MoneyJackpotEnabled
            {
                get { return _MoneyJackpotEnabled; }
                set { _MoneyJackpotEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("MoneyCreditLineEnabled")]
            internal bool MoneyCreditLineEnabled
            {
                get { return _MoneyCreditLineEnabled; }
                set { _MoneyCreditLineEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("InvoicesEnabled")]
            internal bool MoneyReceiptsEnabled
            {
                get { return _MoneyReceiptsEnabled; }
                set { _MoneyReceiptsEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _MoneyTransferEnabled, _MoneyLogsEnabled, _MoneyPayoutEnabled, _MoneyJackpotEnabled, _MoneyCreditLineEnabled, _MoneyReceiptsEnabled;

            #endregion
            #region Trophies Columns

            [Column("TrophiesEnabled")]
            internal bool TrophiesEnabled
            {
                get { return _TrophiesEnabled; }
                set { _TrophiesEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _TrophiesEnabled;

            #endregion
            #region Statistics Columns
            [Column("StatisticsMoneyEarnedEnabled")]
            internal bool StatisticsMoneyEarnedEnabled
            {
                get { return _StatisticsMoneyEarnedEnabled; }
                set { _StatisticsMoneyEarnedEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("StatisticsPointsEarnedEnabled")]
            internal bool StatisticsPointsEarnedEnabled
            {
                get { return _StatisticsPointsEarnedEnabled; }
                set { _StatisticsPointsEarnedEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("StatisticsAdPacksEnabled")]
            internal bool StatisticsAdPacksEnabled
            {
                get { return _StatisticsAdPacksEnabled; }
                set { _StatisticsAdPacksEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("StatisticsPTCClicksEnabled")]
            internal bool StatisticsPTCClicksEnabled
            {
                get { return _StatisticsPTCClicksEnabled; }
                set { _StatisticsPTCClicksEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("StatisticsLeaderboardEnabled")]
            internal bool StatisticsLeaderboardEnabled
            {
                get { return _StatisticsLeaderboardEnabled; }
                set { _StatisticsLeaderboardEnabled = value; SetUpToDateAsFalse(); }
            }


            bool _StatisticsMoneyEarnedEnabled, _StatisticsPointsEarnedEnabled, _StatisticsAdPacksEnabled, _StatisticsPTCClicksEnabled, _StatisticsLeaderboardEnabled;

            #endregion
            #region Other

            [Column("TestimonialsEnabled")]
            internal bool TestimonialsEnabled
            {
                get { return _TestimonialsEnabled; }
                set { _TestimonialsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("PaymentProofsEnabled")]
            internal bool PaymentProofsEnabled
            {
                get { return _PaymentProofsEnabled; }
                set { _PaymentProofsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("OfferLevelsEnabled")]
            internal bool OfferLevelsEnabled
            {
                get { return _OfferLevelsEnabled; }
                set { _OfferLevelsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("QuickStartGuideEnabled")]
            internal bool QuickStartGuideEnabled
            {
                get { return _QuickStartGuideEnabled; }
                set { _QuickStartGuideEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _TestimonialsEnabled, _PaymentProofsEnabled, _OfferLevelsEnabled, _QuickStartGuideEnabled;

            #endregion
            #region Network


            [Column("SocialNetworkEnabled")]
            internal bool SocialNetworkEnabled
            {
                get { return _SocialNetworkEnabled; }
                set { _SocialNetworkEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _SocialNetworkEnabled;
            #endregion
            #region Publish Columns



            [Column("PublishInTextAdsEnabled")]
            internal bool PublishInTextAdsEnabled
            {
                get { return _PublishInTextAdsEnabled; }
                set { _PublishInTextAdsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("PublishWebsitesEnabled")]
            internal bool PublishWebsitesEnabled
            {
                get { return _PublishWebsitesEnabled; }
                set { _PublishWebsitesEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("PublishBannersEnabled")]
            internal bool PublishBannersEnabled
            {
                get { return _PublishBannersEnabled; }
                set { _PublishBannersEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("PublishOfferWallsEnabled")]
            internal bool PublishOfferWallsEnabled
            {
                get { return _PublishOfferWallsEnabled; }
                set { _PublishOfferWallsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("PublishPTCOfferWallsEnabled")]
            internal bool PublishPTCOfferWallsEnabled
            {
                get { return _PublishPTCOfferWallsEnabled; }
                set { _PublishPTCOfferWallsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("PublishGlobalPostbackEnabled")]
            internal bool PublishGlobalPostbackEnabled
            {
                get { return _PublishGlobalPostbackEnabled; }
                set { _PublishGlobalPostbackEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _PublishInTextAdsEnabled, _PublishWebsitesEnabled, _PublishBannersEnabled, _PublishOfferWallsEnabled, _PublishPTCOfferWallsEnabled, _PublishGlobalPostbackEnabled;
            #endregion
            #region Entertainment Columns

            [Column("WebinarsEnabled")]
            internal bool WebinarsEnabled
            {
                get { return _WebinarsEnabled; }
                set { _WebinarsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("EBooksEnabled")]
            internal bool EBooksEnabled
            {
                get { return _EBooksEnabled; }
                set { _EBooksEnabled = value;  SetUpToDateAsFalse(); }
            }

            [Column("SlotMachineEnabled")]
            internal bool SlotMachineEnabled
            {
                get { return _SlotMachineEnabled; }
                set { _SlotMachineEnabled = value;  SetUpToDateAsFalse(); }
            }

            [Column("EntertainmentMiniVideoEnabled")]
            internal bool EntertainmentMiniVideoEnabled
            {
                get { return _EntertainmentMiniVideoEnabled; }
                set { _EntertainmentMiniVideoEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("RollDiceLotteryEnabled")]
            internal bool RollDiceLotteryEnabled
            {
                get { return _RollDiceLotteryEnabled; }
                set { _RollDiceLotteryEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("PvpJackpotEnabled")]
            internal bool JackpotPvpEnabled
            {
                get { return _JackpotPvpEnabled; }
                set { _JackpotPvpEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _WebinarsEnabled, _EBooksEnabled, _SlotMachineEnabled, _EntertainmentMiniVideoEnabled, _RollDiceLotteryEnabled, _JackpotPvpEnabled;
            #endregion
            #region BTCTradingPlatform Columns

            [Column("CryptocurrencyTradingBuyEnabled")]
            internal bool CryptocurrencyTradingBuyEnabled
            {
                get { return _CryptocurrencyTradingBuyEnabled; }
                set { _CryptocurrencyTradingBuyEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("CryptocurrencyTradingSellEnabled")]
            internal bool CryptocurrencyTradingSellEnabled
            {
                get { return _CryptocurrencyTradingSellEnabled; }
                set { _CryptocurrencyTradingSellEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _CryptocurrencyTradingBuyEnabled, _CryptocurrencyTradingSellEnabled;
            #endregion
            #region ICO
            [Column("ICOInfoEnabled")]
            internal bool ICOInfoEnabled
            {
                get { return _ICOInfoEnabled; }
                set { _ICOInfoEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("ICOBuyEnabled")]
            internal bool ICOBuyEnabled
            {
                get { return _ICOBuyEnabled; }
                set { _ICOBuyEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("ICOHistoryEnabled")]
            internal bool ICOHistoryEnabled
            {
                get { return _ICOHistoryEnabled; }
                set { _ICOHistoryEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("ICOStagesEnabled")]
            internal bool ICOStagesEnabled
            {
                get { return _ICOStagesEnabled; }
                set { _ICOStagesEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("ICOInternalExchangeEnabled")]
            internal bool InternalExchangeEnabled
            {
                get { return _ICOInternalExchangeEnabled; }
                set { _ICOInternalExchangeEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("InternalExchangeCurrentOrdersEnabled")]
            internal bool InternalExchangeCurrentOrdersEnabled
            {
                get { return _InternalExchangeCurrentOrdersEnabled; }
                set { _InternalExchangeCurrentOrdersEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("InternalExchangeTradingHistoryEnabled")]
            internal bool InternalExchangeTradingHistoryEnabled
            {
                get { return _InternalExchangeTradingHistoryEnabled; }
                set { _InternalExchangeTradingHistoryEnabled = value; SetUpToDateAsFalse(); }
            }

            private bool _ICOInfoEnabled, _ICOBuyEnabled, _ICOHistoryEnabled, _ICOStagesEnabled, _ICOInternalExchangeEnabled, _InternalExchangeCurrentOrdersEnabled, _InternalExchangeTradingHistoryEnabled;
            #endregion
            #region Investment Platform

            [Column("InvestmentPlatformHistoryEnabled")]
            internal bool InvestmentPlatformHistoryEnabled
            {
                get { return _InvestmentPlatformHistoryEnabled; }
                set { _InvestmentPlatformHistoryEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("InvestmentPlatformCalculatorEnabled")]
            internal bool InvestmentPlatformCalculatorEnabled
            {
                get { return _InvestmentPlatformCalculatorEnabled; }
                set { _InvestmentPlatformCalculatorEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("InvestmentPlatformQueueSystemEnabled")]
            internal bool InvestmentPlatformQueueSystemEnabled
            {
                get { return _InvestmentPlatformQueueSystemEnabled; }
                set { _InvestmentPlatformQueueSystemEnabled = value; SetUpToDateAsFalse(); }
            }

            private bool _InvestmentPlatformHistoryEnabled, _InvestmentPlatformCalculatorEnabled, _InvestmentPlatformQueueSystemEnabled;
            #endregion

            internal void SaveTitanFeatures()
            {
                SavePartially(IsUpToDate, buildTitanFeaturesProperties());
            }
            internal void ReloadTitanFeatures()
            {
                ReloadPartially(IsUpToDate, buildTitanFeaturesProperties());
            }

            private PropertyInfo[] buildTitanFeaturesProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.EarnersRoleEnabled)
                    .Append(x => x.AdvertisersRoleEnabled)
                    .Append(x => x.PublishersRoleEnabled)
                    .Append(x => x.EarnCPAGPTEnabled)
                    .Append(x => x.SearchMode)
                    .Append(x => x.EarnOfferwallsEnabled)
                    .Append(x => x.EarnAdsEnabled)
                    .Append(x => x.VideoMode)
                    .Append(x => x.EarnContestsEnabled)
                    .Append(x => x.EarnAdPacksEnabled)
                    .Append(x => x.EarnLikesEnabled)
                    .Append(x => x.EarnRefBackEnabled)
                    .Append(x => x.EarnTrafficExchangeEnabled)
                    .Append(x => x.EarnTrafficGridEnabled)
                    .Append(x => x.EarnCrowdFlowerEnabled)
                    .Append(x => x.AdvertCPAGPTEnabled)
                    .Append(x => x.AdvertTrafficExchangeEnabled)
                    .Append(x => x.AdvertBannersEnabled)
                    .Append(x => x.AdvertFacebookEnabled)
                    .Append(x => x.AdvertAdsEnabled)
                    .Append(x => x.AdvertTrafficGridEnabled)
                    .Append(x => x.AdvertAdPacksEnabled)
                    .Append(x => x.AdvertLoginAdsEnabled)
                    .Append(x => x.AdvertMarketplaceEnabled)
                    .Append(x => x.ReferralsDirectEnabled)
                    .Append(x => x.ReferralsBannersEnabled)
                    .Append(x => x.ReferralsIndirectEnabled)
                    .Append(x => x.ReferralsRentedEnabled)
                    .Append(x => x.ReferralsStatsEnabled)
                    .Append(x => x.ReferralsLeadershipEnabled)
                    .Append(x => x.ReferralPoolRotatorEnabled)
                    .Append(x => x.PeopleMessagesEnabled)
                    .Append(x => x.PeopleFriendsEnabled)
                    .Append(x => x.PeopleGamesEnabled)
                    .Append(x => x.PeopleProfileEnabled)
                    .Append(x => x.UpgradeEnabled)
                    .Append(x => x.MoneyTransferEnabled)
                    .Append(x => x.MoneyLogsEnabled)
                    .Append(x => x.MoneyPayoutEnabled)
                    .Append(x => x.MoneyJackpotEnabled)
                    .Append(x => x.IsDiceGameEnabled)
                    .Append(x => x.GiftCardMode)
                    .Append(x => x.TrophiesEnabled)
                    .Append(x => x.StatisticsMoneyEarnedEnabled)
                    .Append(x => x.StatisticsPointsEarnedEnabled)
                    .Append(x => x.StatisticsAdPacksEnabled)
                    .Append(x => x.StatisticsPTCClicksEnabled)
                    .Append(x => x.EarnCashLinksEnabled)
                    .Append(x => x.AdvertSurfAdsEnabled)
                    .Append(x => x.MoneyReceiptsEnabled)
                    .Append(x => x.TestimonialsEnabled)
                    .Append(x => x.SocialNetworkEnabled)
                    .Append(x => x.StatisticsLeaderboardEnabled)
                    .Append(x => x.PaymentProofsEnabled)
                    .Append(x => x.AdvertPtcOfferWallEnabled)
                    .Append(x => x.AdvertInTextAdsEnabled)
                    .Append(x => x.AdvertMyUrlsEnabled)
                    .Append(x => x.PublishWebsitesEnabled)
                    .Append(x => x.PublishInTextAdsEnabled)
                    .Append(x => x.PublishBannersEnabled)
                    .Append(x => x.PublishOfferWallsEnabled)
                    .Append(x => x.PublishPTCOfferWallsEnabled)
                    .Append(x => x.PublishGlobalPostbackEnabled)
                    .Append(x => x.IsRepresentativesEnabled)
                    .Append(x => x.WebinarsEnabled)
                    .Append(x => x.ReferralMatrixEnabled)
                    .Append(x => x.EBooksEnabled)
                    .Append(x => x.LeaderShipSystemEnabled)
                    .Append(x => x.SlotMachineEnabled)
                    .Append(x => x.OfferLevelsEnabled)
                    .Append(x => x.AdvertMiniVideoEnabled)
                    .Append(x => x.EntertainmentMiniVideoEnabled)
                    .Append(x => x.CryptocurrencyTradingBuyEnabled) 
                    .Append(x => x.CryptocurrencyTradingSellEnabled)
                    .Append(x => x.EarnPaidToPromoteEnabled)
                    .Append(x => x.AdvertPaidToPromoteEnabled)
                    .Append(x => x.RollDiceLotteryEnabled)
                    .Append(x => x.JackpotPvpEnabled)
                    .Append(x => x.ICOInfoEnabled)
                    .Append(x => x.ICOBuyEnabled)
                    .Append(x => x.ICOHistoryEnabled)
                    .Append(x => x.ICOStagesEnabled)
                    .Append(x => x.NewsHomepageEnabled)
                    .Append(x => x.NewsSharingArticlesEnabled)
                    .Append(x => x.NewsWritingArticlesEnabled)
                    .Append(x => x.InvestmentPlatformCalculatorEnabled)
                    .Append(x => x.InvestmentPlatformHistoryEnabled)
                    .Append(x => x.InvestmentPlatformQueueSystemEnabled)
                    .Append(x => x.InternalExchangeEnabled)
                    .Append(x => x.InternalExchangeCurrentOrdersEnabled)
                    .Append(x => x.InternalExchangeTradingHistoryEnabled)
                    ;    
                return exValues.Build(); 
            }
        }
    }
}
