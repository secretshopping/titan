using System;
using Prem.PTC.Utils;
using System.Reflection;


namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public enum AdPacksPolicy { Standard = 0, HYIP = 1 }

        public static partial class RevShare
        {
            public static bool IsRevShareEnabled { get { return appSettings.IsRevShareEnabled; } set { appSettings.IsRevShareEnabled = value; } }

            public static string AdminUsername { get { return appSettings.AdminUsername; } set { appSettings.AdminUsername = value; } }
            public static int AdminUserId { get { return appSettings.AdminUserId; } set { appSettings.AdminUserId = value; } }
            public static int DailyPoolDurationTime { get { return appSettings.DailyPoolDurationTime; } set { appSettings.DailyPoolDurationTime = value; } }
            public static bool RequireAPToViewCashLinks { get { return appSettings.RequireAPToViewCashLinks; } set { appSettings.RequireAPToViewCashLinks = value; } }
            public static bool StopRoiAfterMembershipExpiration { get { return appSettings.StopRoiAfterMembershipExpiration; } set { appSettings.StopRoiAfterMembershipExpiration = value; } }
            public static bool AdPacksCalculatorEnabled { get { return appSettings.AdPacksCalculatorEnabled; } set { appSettings.AdPacksCalculatorEnabled = value; } }
            public static bool AdPackPurchasesViaCommissionBalanceEnabled { get { return appSettings.AdPackPurchasesViaCommissionBalanceEnabled; } set { appSettings.AdPackPurchasesViaCommissionBalanceEnabled = value; } }

            public static DistributionTimePolicy DistributionTime
            {
                get { return (DistributionTimePolicy)appSettings.DistributionTime; }
                set { appSettings.DistributionTime = (int)value; }
            }

            public static AdPacksPolicy AdPacksPolicy
            {
                get { return (AdPacksPolicy)appSettings.AdPacksPolicy; }
                set { appSettings.AdPacksPolicy = (int)value; }
            }

            public static int HourlyDistributionsMadeToday
            {
                get { return appSettings.HourlyDistributionsMadeToday; }
                set { appSettings.HourlyDistributionsMadeToday = value; }
            }

            public static DayOfWeek DayOfWeekDistribution
            {
                get
                {
                    return (DayOfWeek)appSettings.DayOfWeekDistributionInt;
                }
                set
                {
                    appSettings.DayOfWeekDistributionInt = (int)value;
                }
            }

            public static void Save()
            {
                appSettings.SaveAdPack();
                appSettings.SaveRevShare();
            }
            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) { appSettings.ReloadAdPack(); appSettings.ReloadRevShare(); }
            }

            public static class AdPack
            {
                public static string AdPackName { get { return appSettings.AdPackName; } set { appSettings.AdPackName = value; } }
                public static string AdPackNamePlural { get { return appSettings.AdPackNamePlural; } set { appSettings.AdPackNamePlural = value; } }
                public static Int32 PackNormalBannerHeight { get { return appSettings.PackNormalBannerHeight; } set { appSettings.PackNormalBannerHeight = value; } }
                public static Int32 PackNormalBannerWidth { get { return appSettings.PackNormalBannerWidth; } set { appSettings.PackNormalBannerWidth = value; } }
                public static Int32 PackConstantBannerHeight { get { return appSettings.PackConstantBannerHeight; } set { appSettings.PackConstantBannerHeight = value; } }
                public static Int32 PackConstantBannerWidth { get { return appSettings.PackConstantBannerWidth; } set { appSettings.PackConstantBannerWidth = value; } }
             
                public static Int32 TimeBetweenAdsRedirectInSeconds { get { return appSettings.TimeBetweenAdPackAdsRedirectInSeconds; } set { appSettings.TimeBetweenAdPackAdsRedirectInSeconds = value; } }
                public static bool EnableAdvertChange { get { return appSettings.EnableAdvertChange; } set { appSettings.EnableAdvertChange = value; } }
                public static Decimal DirectReferersProfitPercentage { get { return appSettings.DirectReferersProfitPercentage; } set { appSettings.DirectReferersProfitPercentage = value; } }
                public static bool IsStartPageEnabled { get { return appSettings.IsStartPageEnabled; } set { appSettings.IsStartPageEnabled = value; } }
                public static Money StartPagePrice { get { return appSettings.StartPagePrice; } set { appSettings.StartPagePrice = value; } }
                public static bool IsStartSurfingEnabled { get { return appSettings.IsStartSurfingEnabled; } set { appSettings.IsStartSurfingEnabled = value; } }
                public static bool IsAdListEnabled { get { return appSettings.IsAdListEnabled; } set { appSettings.IsAdListEnabled = value; } }
                public static bool IsTimeClickExchangeEnabled { get { return appSettings.IsTimeClickExchangeEnabled; } set { appSettings.IsTimeClickExchangeEnabled = value; } }
                public static bool BuyAdPacksDirectlyFromPaymentProcessorOnly { get { return appSettings.BuyAdPacksDirectlyFromPaymentProcessorOnly; } set { appSettings.BuyAdPacksDirectlyFromPaymentProcessorOnly = value; } }
                public static bool RevShareCustomGroupRewardsEnabled { get { return appSettings.RevShareCustomGroupRewardsEnabled; } set { appSettings.RevShareCustomGroupRewardsEnabled = value; } }
                public static bool EnableAdvertAutoApproval { get { return appSettings.EnableAdvertAutoApproval; } set { appSettings.EnableAdvertAutoApproval = value; } }
                public static Int32 AdminsAdvertDisplayTime { get { return appSettings.AdminsAdvertDisplayTime; } set { appSettings.AdminsAdvertDisplayTime = value; } }
                public static bool BuyAdPacksForReferralsEnabled { get { return appSettings.BuyAdPacksForReferralsEnabled; } set { appSettings.BuyAdPacksForReferralsEnabled = value; } }
                public static int MaxAdPacksForOtherUser { get { return appSettings.MaxAdPacksForOtherUser; } set { appSettings.MaxAdPacksForOtherUser = value; } }
                public static decimal MaxDailyROIPercent { get { return appSettings.MaxDailyROIPercent; } set { appSettings.MaxDailyROIPercent = value; } }
                public static bool HideAdPackTypesWhenOneEnabled { get { return appSettings.HideAdPackTypesWhenOneEnabled; } set { appSettings.HideAdPackTypesWhenOneEnabled = value; } }           
                public static bool InstantAccrualsEnabled { get { return appSettings.InstantAccrualsEnabled; } set { appSettings.InstantAccrualsEnabled = value; } }



                public static GroupPolicy GroupPolicy
                {
                    get
                    {
                        return (GroupPolicy)appSettings.PolicyInt;
                    }
                    set { appSettings.PolicyInt = (int)value; }
                }

                public static CustomReturnOption CustomReturnOption
                {
                    get
                    {
                        return (CustomReturnOption)appSettings.CustomReturnOptionInt;
                    }
                    set { appSettings.CustomReturnOptionInt  = (int)value; }
                }

                public static DistributionPolicy DistributionPolicy
                {
                    get
                    {
                        return (DistributionPolicy)appSettings.DistributionPolicyInt;
                    }
                    set { appSettings.DistributionPolicyInt = (int)value; }
                }

                public static void Save()
                {
                    appSettings.SaveAdPack();
                }
                public static void Reload()
                {
                    if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadAdPack();
                }
            }
        }
        internal partial class AppSettingsTable : BaseTableObject
        {
            #region General RevShare Settings Columns

            [Column("IsRevShareEnabled")]
            internal bool IsRevShareEnabled
            {
                get { return _IsRevShareEnabled; }
                set { _IsRevShareEnabled = value; SetUpToDateAsFalse(); }
            }

           [Column("EnableAdPackWithoutAnyAdvertising")]
            internal bool EnableAdPackWithoutAnyAdvertising
            {
                get { return _EnableAdPackWithoutAnyAdvertising; }
                set { _EnableAdPackWithoutAnyAdvertising = value; SetUpToDateAsFalse(); }
            }

            [Column("EnableAdvertChange")]
            internal bool EnableAdvertChange
            {
                get { return _EnableAdvertChange; }
                set { _EnableAdvertChange = value; SetUpToDateAsFalse(); }
            }

            [Column("EnableAdvertAutoApproval")]
            internal bool EnableAdvertAutoApproval
            {
                get { return _EnableAdvertAutoApproval; }
                set { _EnableAdvertAutoApproval = value; SetUpToDateAsFalse(); }
            }

            [Column("AdminUsername")]
            internal string AdminUsername
            {
                get { return _AdminUsername; }
                set { _AdminUsername = value; SetUpToDateAsFalse(); }
            }

            [Column("AdminUserId")]
            internal int AdminUserId
            {
                get { return _AdminUserId; }
                set { _AdminUserId = value; SetUpToDateAsFalse(); }
            }

            [Column("DailyPoolDurationTime")]
            internal int DailyPoolDurationTime
            {
                get { return _DailyPoolDurationTime; }
                set { _DailyPoolDurationTime = value; SetUpToDateAsFalse(); }
            }

            [Column("DistributionTime")]
            internal int DistributionTime
            {
                get { return _DistributionTime; }
                set { _DistributionTime = value; SetUpToDateAsFalse(); }
            }
            
            [Column("HourlyDistributionsMadeToday")]
            internal int HourlyDistributionsMadeToday
            {
                get { return _HourlyDistributionsMadeToday; }
                set { _HourlyDistributionsMadeToday = value; SetUpToDateAsFalse(); }
            }

            [Column("RequireAPToViewCashLinks")]
            internal bool RequireAPToViewCashLinks
            {
                get { return _RequireAPToViewCashLinks; }
                set { _RequireAPToViewCashLinks = value; SetUpToDateAsFalse(); }
            }

            [Column("StopRoiAfterMembershipExpiration")]
            internal bool StopRoiAfterMembershipExpiration
            {
                get { return _StopRoiAfterMembershipExpiration; }
                set { _StopRoiAfterMembershipExpiration = value; SetUpToDateAsFalse(); }
            }      

            [Column("AdPacksPolicy")]
            internal int AdPacksPolicy
            {
                get { return _AdPacksPolicy; }
                set { _AdPacksPolicy = value;  SetUpToDateAsFalse();  }
            }

            [Column("DayOfWeekDistribution")]
            internal int DayOfWeekDistributionInt
            {
                get { return _DayOfWeekDistributionInt; }
                set { _DayOfWeekDistributionInt = value;  SetUpToDateAsFalse(); }
            }

            [Column("AdPacksCalculatorEnabled")]
            internal bool AdPacksCalculatorEnabled
            {
                get { return _AdPacksCalculatorEnabled; }
                set { _AdPacksCalculatorEnabled = value; SetUpToDateAsFalse(); }
            }
            
            [Column("AdPackPurchasesViaCommissionBalanceEnabled")]
            internal bool AdPackPurchasesViaCommissionBalanceEnabled
            {
                get { return _AdPackPurchasesViaCommissionBalanceEnabled; }
                set { _AdPackPurchasesViaCommissionBalanceEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _IsRevShareEnabled, _EnableAdPackWithoutAnyAdvertising, _EnableAdvertChange, _EnableAdvertAutoApproval, _RequireAPToViewCashLinks,
                _StopRoiAfterMembershipExpiration, _AdPacksCalculatorEnabled, _AdPackPurchasesViaCommissionBalanceEnabled;
            Int32 _DailyPoolDurationTime, _DistributionTime, _HourlyDistributionsMadeToday, _AdminUserId, _AdPacksPolicy, _DayOfWeekDistributionInt;
            string _AdminUsername;

            #endregion

            #region AdPack Columns

            [Column("RevSharePolicy")]
            internal Int32 PolicyInt
            {
                get { return _Policy; }
                set { _Policy = value; SetUpToDateAsFalse(); }
            }

            [Column("CustomReturnOption")]
            internal Int32 CustomReturnOptionInt
            {
                get { return _CustomReturnOption; }
                set { _CustomReturnOption = value; SetUpToDateAsFalse(); }
            }

            [Column("RevShareDistributionPolicy")]
            internal Int32 DistributionPolicyInt
            {
                get { return _DistributionPolicy; }
                set { _DistributionPolicy = value; SetUpToDateAsFalse(); }
            }

            [Column("AdPackName")]
            internal string AdPackName
            {
                get { return _AdPackName; }
                set { _AdPackName = value; SetUpToDateAsFalse(); }
            }

            [Column("AdPackNamePlural")]
            internal string AdPackNamePlural
            {
                get { return _AdPackNamePlural; }
                set { _AdPackNamePlural = value; SetUpToDateAsFalse(); }
            }

            [Column("PackNormalBannerHeight")]
            internal Int32 PackNormalBannerHeight
            {
                get { return _PackNormalBannerHeight; }
                set { _PackNormalBannerHeight = value; SetUpToDateAsFalse(); }
            }

            [Column("PackNormalBannerWidth")]
            internal Int32 PackNormalBannerWidth
            {
                get { return _PackNormalBannerWidth; }
                set { _PackNormalBannerWidth = value; SetUpToDateAsFalse(); }
            }

            [Column("PackConstantBannerHeight")]
            internal Int32 PackConstantBannerHeight
            {
                get { return _PackConstantBannerHeight; }
                set { _PackConstantBannerHeight = value; SetUpToDateAsFalse(); }
            }


            [Column("PackConstantBannerWidth")]
            internal Int32 PackConstantBannerWidth
            {
                get { return _PackConstantBannerWidth; }
                set { _PackConstantBannerWidth = value; SetUpToDateAsFalse(); }
            }

            
            [Column("DailyRequiredAdClicks")]
            internal Int32 DailyRequiredAdClicks
            {
                get { return _DailyRequiredAdClicks; }
                set { _DailyRequiredAdClicks = value; SetUpToDateAsFalse(); }
            }

            [Column("TimeBetweenAdPackAdsRedirectInSeconds")]
            internal Int32 TimeBetweenAdPackAdsRedirectInSeconds
            {
                get { return _TimeBetweenAdPackAdsRedirectInSeconds; }
                set { _TimeBetweenAdPackAdsRedirectInSeconds = value; SetUpToDateAsFalse(); }
            }

            [Column("AdPackDirectReferersProfitPercentage")]
            internal decimal DirectReferersProfitPercentage
            {
                get { return _DirectReferersProfitPercentage; }
                set { _DirectReferersProfitPercentage = value; SetUpToDateAsFalse(); }
            }

            [Column("IsStartPageEnabled")]
            internal bool IsStartPageEnabled
            {
                get { return _IsStartPageEnabled; }
                set { _IsStartPageEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("StartPagePrice")]
            internal Money StartPagePrice
            {
                get { return _StartPagePrice; }
                set { _StartPagePrice = value; SetUpToDateAsFalse(); }
            }

            [Column("IsAdPackStartSurfingEnabled")]
            internal bool IsStartSurfingEnabled
            {
                get { return _IsStartSurfingEnabled; }
                set { _IsStartSurfingEnabled = value; SetUpToDateAsFalse(); }
            }
            
            [Column("IsAdPackAdListEnabled")]
            internal bool IsAdListEnabled
            {
                get { return _IsAdListEnabled; }
                set { _IsAdListEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("IsTimeClickExchangeEnabled")]
            internal bool IsTimeClickExchangeEnabled
            {
                get { return _IsTimeClickExchangeEnabled; }
                set { _IsTimeClickExchangeEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("BuyAdPacksDirectlyFromPaymentProcessorOnly")]
            internal bool BuyAdPacksDirectlyFromPaymentProcessorOnly
            {
                get { return _BuyAdPacksDirectlyFromPaymentProcessorOnly; }
                set { _BuyAdPacksDirectlyFromPaymentProcessorOnly = value; SetUpToDateAsFalse(); }
            }

            [Column("RevShareCustomGroupRewardsEnabled")]
            internal bool RevShareCustomGroupRewardsEnabled
            {
                get { return _RevShareCustomGroupRewardsEnabled; }
                set { _RevShareCustomGroupRewardsEnabled = value; SetUpToDateAsFalse(); }
            }
            
            [Column("PackPTCDurationInSeconds")]
            internal int AdminsAdvertDisplayTime
            {
                get { return _AdminsAdvertDisplayTime; }
                set { _AdminsAdvertDisplayTime = value; SetUpToDateAsFalse(); }
            }

            [Column("BuyAdPacksForReferralsEnabled")]
            internal bool BuyAdPacksForReferralsEnabled
            {
                get { return _BuyAdPacksForReferralsEnabled; }
                set { _BuyAdPacksForReferralsEnabled = value; SetUpToDateAsFalse(); }
                
            }

            [Column("MaxAdPacksForOtherUser")]
            internal int MaxAdPacksForOtherUser
            {
                get { return _MaxAdPacksForOtherUser; }
                set { _MaxAdPacksForOtherUser = value; SetUpToDateAsFalse(); }
            }

            [Column("MaxDailyROIPercent")]
            internal decimal MaxDailyROIPercent
            {
                get { return _MaxDailyROIPercent; }
                set { _MaxDailyROIPercent = value; SetUpToDateAsFalse(); }
            }

            [Column("HideAdPackTypesWhenOneEnabled")]
            internal bool HideAdPackTypesWhenOneEnabled
            {
                get { return _HideAdPackTypesWhenOneEnabled; }
                set { _HideAdPackTypesWhenOneEnabled = value; SetUpToDateAsFalse(); }

            }

            [Column("InstantAccrualsEnabled")]
            internal bool InstantAccrualsEnabled
            {
                get { return _InstantAccrualsEnabled; }
                set { _InstantAccrualsEnabled = value; SetUpToDateAsFalse(); }
            }

            int _Policy, _TimeBetweenAdPackAdsRedirectInSeconds, _AdminsAdvertDisplayTime, _MaxAdPacksForOtherUser;
            int _PackNormalBannerHeight, _PackNormalBannerWidth, _PackConstantBannerHeight, _PackConstantBannerWidth, _DailyRequiredAdClicks, _CustomReturnOption, _DistributionPolicy;
            Money _StartPagePrice;
            string _AdPackName, _AdPackNamePlural;
            decimal _DirectReferersProfitPercentage, _MaxDailyROIPercent;
            bool _IsStartPageEnabled, _IsStartSurfingEnabled, _IsAdListEnabled, _IsTimeClickExchangeEnabled, _InstantAccrualsEnabled,
                _BuyAdPacksDirectlyFromPaymentProcessorOnly, _RevShareCustomGroupRewardsEnabled, _BuyAdPacksForReferralsEnabled,
                _HideAdPackTypesWhenOneEnabled;
            #endregion

            #region Save & Reload AdPack
            internal void SaveAdPack()
            {
                SavePartially(IsUpToDate, buildAdPackProperties());
            }
            internal void ReloadAdPack()
            {
                ReloadPartially(IsUpToDate, buildAdPackProperties());
            }

            private PropertyInfo[] buildAdPackProperties()
            {
                   var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.AdPackName)
                    .Append(x => x.AdPackNamePlural)
                    .Append(x => x.PolicyInt)
                    .Append(x => x.CustomReturnOptionInt)
                    .Append(x => x.DistributionPolicyInt)
                    .Append(x => x.PackNormalBannerHeight)
                    .Append(x => x.PackNormalBannerWidth)
                    .Append(x => x.PackConstantBannerHeight)
                    .Append(x => x.PackConstantBannerWidth)
                    .Append(x => x.EnableAdvertChange)
                    .Append(x => x.DirectReferersProfitPercentage)
                    .Append(x => x.StartPagePrice)
                    .Append(x => x.IsStartPageEnabled)
                    .Append(x => x.IsStartSurfingEnabled)
                    .Append(x => x.IsAdListEnabled)
                    .Append(x => x.DailyRequiredAdClicks)
                    .Append(x => x.IsTimeClickExchangeEnabled)
                    .Append(x => x.BuyAdPacksDirectlyFromPaymentProcessorOnly)
                    .Append(x => x.RevShareCustomGroupRewardsEnabled)
                    .Append(x => x.EnableAdvertAutoApproval)
                    .Append(x => x.BuyAdPacksForReferralsEnabled)
                    .Append(x => x.MaxAdPacksForOtherUser)
                    .Append(x => x.AdminsAdvertDisplayTime)
                    .Append(x => x.MaxDailyROIPercent)
                    .Append(x => x.InstantAccrualsEnabled)
                    .Append(x => x.HideAdPackTypesWhenOneEnabled);

                return exValues.Build();
            }

            #endregion

            internal void SaveRevShare()
            {
                SavePartially(IsUpToDate, buildRevShareProperties());
            }
            internal void ReloadRevShare()
            {
                ReloadPartially(IsUpToDate, buildRevShareProperties());
            }

            private PropertyInfo[] buildRevShareProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.IsRevShareEnabled)
                    .Append(x => x.AdminUsername)
                    .Append(x => x.DailyPoolDurationTime)
                    .Append(x => x.DistributionTime)
                    .Append(x => x.TimeBetweenAdPackAdsRedirectInSeconds)
                    .Append(x => x.EnableAdPackWithoutAnyAdvertising)
                    .Append(x => x.HourlyDistributionsMadeToday)
                    .Append(x => x.AdminUserId)
                    .Append(x => x.RequireAPToViewCashLinks)
                    .Append(x => x.StopRoiAfterMembershipExpiration)
                    .Append(x => x.AdPacksPolicy)
                    .Append(x => x.DayOfWeekDistributionInt)
                    .Append(x => x.AdPacksCalculatorEnabled)
                    .Append(x => x.AdPackPurchasesViaCommissionBalanceEnabled)
                    ;
                
                return exValues.Build();
            }

        }
    }
}
