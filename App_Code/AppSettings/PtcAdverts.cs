using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class PtcAdverts
        {
            public static Money DescriptionCost
            {
                get { return appSettings.PtcAdvertDescriptionCost; }
                set { appSettings.PtcAdvertDescriptionCost = value; }
            }

            public static Money FontBoldCost
            {
                get { return appSettings.PtcAdvertFontBoldCost; }
                set { appSettings.PtcAdvertFontBoldCost = value; }
            }

            public static Money GeolocationCost
            {
                get { return appSettings.PtcAdvertGeolocationCost; }
                set { appSettings.PtcAdvertGeolocationCost = value; }
            }

            public static bool AdExposureProfitsEnabled
            {
                get { return appSettings.AdExposureProfitsEnabled; }
                set { appSettings.AdExposureProfitsEnabled = value; }
            }

            public static bool GeolocationStatus
            {
                get { return appSettings.GeolocationStatus; }
                set { appSettings.GeolocationStatus = value; }
            }

            public static bool ShowGeolocatedCountryFlags
            {
                get { return appSettings.ShowGeolocatedCountryFlags; }
                set { appSettings.ShowGeolocatedCountryFlags = value; }
            }

            public static bool IsExternalIFrameEnabled
            {
                get { return appSettings.IsExternalIFrameEnabled; }
                set { appSettings.IsExternalIFrameEnabled = value; }
            }

            public static PTCCategoryPolicy PTCCategoryPolicy
            {
                get { return (PTCCategoryPolicy)appSettings.PTCCategoryPolicyInt; }
                set { appSettings.PTCCategoryPolicyInt = (int)value; }
            }

            public static bool PTCStartPageEnabled
            {
                get { return appSettings.PTCStartPageEnabled; }
                set { appSettings.PTCStartPageEnabled = value; }
            }

            public static Money PTCStartPagePrice
            {
                get { return appSettings.PTCStartPagePrice; }
                set { appSettings.PTCStartPagePrice = value; }
            }

            public static bool PTCCreditsEnabled
            {
                get { return appSettings.PTCCreditsEnabled; }
                set { appSettings.PTCCreditsEnabled = value; }
            }

            public static string PTCDefaultCaptchaQuestion
            {
                get { return appSettings.PTCDefaultCaptchaQuestion; }
                set { appSettings.PTCDefaultCaptchaQuestion = value; }
            }

            public static bool PtcPriceBasedOnTotalActiveMembersEnabled
            {
                get { return appSettings.PtcPriceBasedOnTotalActiveMembersEnabled; }
                set { appSettings.PtcPriceBasedOnTotalActiveMembersEnabled = value; }
            }

            public static bool StarredAdsEnabled
            {
                get { return appSettings.StarredAdsEnabled; }
                set { appSettings.StarredAdsEnabled = value; }
            }

            public static Money StarredAdsPrice
            {
                get { return appSettings.StarredAdsPrice; }
                set { appSettings.StarredAdsPrice = value; }
            }

            public static bool FeedbackCaptchaEnabled
            {
                get { return appSettings.FeedbackCaptchaEnabled; }
                set { appSettings.FeedbackCaptchaEnabled = value; }
            }
            public static bool DisableMoneyEarningsInPTC
            {
                get { return appSettings.DisableMoneyEarningsInPTC; }
                set { appSettings.DisableMoneyEarningsInPTC = value; }
            }
            public static bool ShowAdvertiserAvatar
            {
                get { return appSettings.ShowAdvertiserAvatar; }
                set { appSettings.ShowAdvertiserAvatar = value; }
            }

            public static bool FavoriteAdsEnabled
            {
                get { return appSettings.FavoriteAdsEnabled; }
                set { appSettings.FavoriteAdsEnabled = value; }
            }

            public static bool DynamicPTCPriceEnabled
            {
                get { return appSettings.DynamicPTCPriceEnabled; }
                set { appSettings.DynamicPTCPriceEnabled = value; }
            }

            public static Money BasePricePer1000ViewsPtc
            {
                get { return appSettings.BasePricePer1000ViewsPtc; }
                set { appSettings.BasePricePer1000ViewsPtc = value; }
            }

            public static int DecimalPricePer1000ViewsPtc
            {
                get { return appSettings.DecimalPricePer1000ViewsPtc; }
                set { appSettings.DecimalPricePer1000ViewsPtc = value; }
            }

            public static bool RegistrationDiscountEnabled
            {
                get { return appSettings.RegistrationDiscountEnabled; }
                set { appSettings.RegistrationDiscountEnabled = value; }
            }

            public static int RegistrationDiscountValue
            {
                get { return appSettings.RegistrationDiscountValue; }
                set { appSettings.RegistrationDiscountValue = value; }
            }

            public static DateTime RegistrationDiscountStartDate
            {
                get { return appSettings.RegistrationDiscountStartDate; }
                set { appSettings.RegistrationDiscountStartDate = value; }
            }

            public static int RegistrationDiscountDays
            {
                get { return appSettings.RegistrationDiscountDays; }
                set { appSettings.RegistrationDiscountDays = value; }
            }

            public static bool BlockPtcAdvertsAfterMissingPointer
            {
                get { return appSettings.BlockPtcAdvertsAfterMissingPointer; }
                set { appSettings.BlockPtcAdvertsAfterMissingPointer = value; }
            }

            public static bool PTCImagesEnabled
            {
                get { return appSettings.PTCImagesEnabled; }
                set { appSettings.PTCImagesEnabled = value; }
            }

            public static bool ExposureCategoriesEnabled
            {
                get { return appSettings.ExposureCategoriesEnabled; }
                set { appSettings.ExposureCategoriesEnabled = value; }
            }

            public static PTCViewMode CashLinkViewEnabled
            {
                get { return (PTCViewMode)appSettings.CashLinkViewEnabledInt; }
                set { appSettings.CashLinkViewEnabledInt = (int)value; }
            }

            public static void Save()
            {
                appSettings.SavePtcAdverts();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadPtcAdverts();
            }

            public static bool CashLinkCrediterEnabled
            {
                get { return appSettings.CashLinkCrediterEnabled; }
                set { appSettings.CashLinkCrediterEnabled = value; }
            }

            public static bool AdvertPTCPackCashbackEnabled
            {
                get { return appSettings.AdvertPTCPackCashbackEnabled; }
                set { appSettings.AdvertPTCPackCashbackEnabled = value; }
            }
        }


        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("PtcAdvertDescriptionCost")]
            internal Money PtcAdvertDescriptionCost { get { return _ptcAdvertDescriptionCost; } set { _ptcAdvertDescriptionCost = value; SetUpToDateAsFalse(); } }

            [Column("PtcAdvertFontBoldCost")]
            internal Money PtcAdvertFontBoldCost { get { return _ptcAdvertFontBoldCost; } set { _ptcAdvertFontBoldCost = value; SetUpToDateAsFalse(); } }

            [Column("PtcAdvertGeolocationCost")]
            internal Money PtcAdvertGeolocationCost { get { return _ptcAdvertGeolocationCost; } set { _ptcAdvertGeolocationCost = value; SetUpToDateAsFalse(); } }

            [Column("AdExposureProfitsEnabled")]
            internal bool AdExposureProfitsEnabled { get { return _AdExposureProfitsEnabled; } set { _AdExposureProfitsEnabled = value; SetUpToDateAsFalse(); } }

            [Column("IsExternalIFrameEnabled")]
            public bool IsExternalIFrameEnabled { get { return _IsExternalIFrameEnabled; } set { _IsExternalIFrameEnabled = value; SetUpToDateAsFalse(); } }

            [Column("GeolocationStatus")]
            internal bool GeolocationStatus { get { return _GeolocationStatus; } set { _GeolocationStatus = value; SetUpToDateAsFalse(); } }

            [Column("ShowGeolocatedCountryFlags")]
            internal bool ShowGeolocatedCountryFlags { get { return _ShowGeolocatedCountryFlags; } set { _ShowGeolocatedCountryFlags = value; SetUpToDateAsFalse(); } }

            [Column("PTCCategoryPolicy")]
            internal int PTCCategoryPolicyInt { get { return _PTCCategoryPolicy; } set { _PTCCategoryPolicy = value; SetUpToDateAsFalse(); } }

            [Column("PTCStartPageEnabled")]
            internal bool PTCStartPageEnabled { get { return _PTCStartPageEnabled; } set { _PTCStartPageEnabled = value; SetUpToDateAsFalse(); } }

            [Column("PTCStartPagePrice")]
            internal Money PTCStartPagePrice { get { return _PTCStartPagePrice; } set { _PTCStartPagePrice = value; SetUpToDateAsFalse(); } }

            [Column("PTCCreditsEnabled")]
            internal bool PTCCreditsEnabled { get { return _PTCCreditsEnabled; } set { _PTCCreditsEnabled = value; SetUpToDateAsFalse(); } }

            [Column("PTCCaptchaQuestionPrice")]
            internal Money PTCCaptchaQuestionPrice { get { return _PTCCaptchaQuestionPrice; } set { _PTCCaptchaQuestionPrice = value; SetUpToDateAsFalse(); } }

            [Column("PTCDefaultCaptchaQuestion")]
            internal string PTCDefaultCaptchaQuestion { get { return _PTCDefaultCaptchaQuestion; } set { _PTCDefaultCaptchaQuestion = value; SetUpToDateAsFalse(); } }

            [Column("PtcPriceBasedOnTotalActiveMembersEnabled")]
            internal bool PtcPriceBasedOnTotalActiveMembersEnabled { get { return _PtcPriceBasedOnTotalActiveMembersEnabled; } set { _PtcPriceBasedOnTotalActiveMembersEnabled = value; SetUpToDateAsFalse(); } }

            [Column("StarredAdsEnabled")]
            internal bool StarredAdsEnabled { get { return _StarredAdsEnabled; } set { _StarredAdsEnabled = value; SetUpToDateAsFalse(); } }

            [Column("StarredAdsPrice")]
            internal Money StarredAdsPrice { get { return _StarredAdsPrice; } set { _StarredAdsPrice = value; SetUpToDateAsFalse(); } }

            [Column("FeedbackCaptchaEnabled")]
            internal bool FeedbackCaptchaEnabled { get { return _FeedbackCaptchaEnabled; } set { _FeedbackCaptchaEnabled = value; SetUpToDateAsFalse(); } }

            [Column("DisableMoneyEarningsInPTC")]
            internal bool DisableMoneyEarningsInPTC { get { return _DisableMoneyEarningsInPTC; } set { _DisableMoneyEarningsInPTC = value; SetUpToDateAsFalse(); } }

            [Column("ShowAdvertiserAvatar")]
            internal bool ShowAdvertiserAvatar { get { return _ShowAdvertiserAvatar; } set { _ShowAdvertiserAvatar = value; SetUpToDateAsFalse(); } }

            [Column("FavoriteAdsEnabled")]
            internal bool FavoriteAdsEnabled { get { return _FavoriteAdsEnabled; } set { _FavoriteAdsEnabled = value; SetUpToDateAsFalse(); } }

            [Column("DynamicPTCPriceEnabled")]
            internal bool DynamicPTCPriceEnabled { get { return _DynamicPTCPriceEnabled; } set { _DynamicPTCPriceEnabled = value; SetUpToDateAsFalse(); } }

            [Column("BasePricePer1000ViewsPtc")]
            internal Money BasePricePer1000ViewsPtc { get { return _BasePricePer1000ViewsPtc; } set { _BasePricePer1000ViewsPtc = value; SetUpToDateAsFalse(); } }

            [Column("DecimalPricePer1000ViewsPtc")]
            internal int DecimalPricePer1000ViewsPtc { get { return _DecimalPricePer1000ViewsPtc; } set { _DecimalPricePer1000ViewsPtc = value; SetUpToDateAsFalse(); } }

            [Column("RegistrationDiscountEnabled")]
            internal bool RegistrationDiscountEnabled { get { return _RegistrationDiscountEnabled; } set { _RegistrationDiscountEnabled = value; SetUpToDateAsFalse(); } }

            [Column("RegistrationDiscountValue")]
            internal int RegistrationDiscountValue { get { return _RegistrationDiscountValue; } set { _RegistrationDiscountValue = value; SetUpToDateAsFalse(); } }

            [Column("RegistrationDiscountStartDate")]
            internal DateTime RegistrationDiscountStartDate { get { return _RegistrationDiscountStartDate; } set { _RegistrationDiscountStartDate = value; SetUpToDateAsFalse(); } }

            [Column("RegistrationDiscountDays")]
            internal int RegistrationDiscountDays { get { return _RegistrationDiscountDays; } set { _RegistrationDiscountDays = value; SetUpToDateAsFalse(); } }

            [Column("BlockPtcAdvertsAfterMissingPointer")]
            internal bool BlockPtcAdvertsAfterMissingPointer { get { return _BlockPtcAdvertsAfterMissingPointer; } set { _BlockPtcAdvertsAfterMissingPointer = value; SetUpToDateAsFalse(); } }

            [Column("PTCImagesEnabled")]
            internal bool PTCImagesEnabled { get { return _PTCImagesEnabled; } set { _PTCImagesEnabled = value; SetUpToDateAsFalse(); } }

            [Column("ExposureCategoriesEnabled")]
            internal bool ExposureCategoriesEnabled { get { return _ExposureCategoriesEnabled; } set { _ExposureCategoriesEnabled = value; SetUpToDateAsFalse(); } }

            [Column("CashLinkViewEnabled")]
            internal int CashLinkViewEnabledInt { get { return _CashLinkViewEnabled; } set { _CashLinkViewEnabled = value; SetUpToDateAsFalse(); } }

            [Column("CashLinkCrediterEnabled")]
            internal bool CashLinkCrediterEnabled { get { return _CashLinkCrediterEnabled; } set { _CashLinkCrediterEnabled = value; SetUpToDateAsFalse(); } }

            [Column("AdvertPTCPackCashbackEnabled")]
            internal bool AdvertPTCPackCashbackEnabled { get { return _AdvertPTCPackCashbackEnabled; } set { _AdvertPTCPackCashbackEnabled = value; SetUpToDateAsFalse(); }
            }

            private Money _ptcAdvertDescriptionCost, _ptcAdvertFontBoldCost, _ptcAdvertGeolocationCost, _PTCStartPagePrice, _PTCCaptchaQuestionPrice,
                _StarredAdsPrice, _BasePricePer1000ViewsPtc;
            private bool _AdExposureProfitsEnabled, _ShowGeolocatedCountryFlags, _IsExternalIFrameEnabled, _PTCStartPageEnabled, _PTCCreditsEnabled,
                _PtcPriceBasedOnTotalActiveMembersEnabled, _StarredAdsEnabled, _FeedbackCaptchaEnabled, _DisableMoneyEarningsInPTC, _ShowAdvertiserAvatar,
                _FavoriteAdsEnabled, _DynamicPTCPriceEnabled, _RegistrationDiscountEnabled, _BlockPtcAdvertsAfterMissingPointer, _PTCImagesEnabled, _ExposureCategoriesEnabled, 
                _GeolocationStatus, _CashLinkCrediterEnabled, _AdvertPTCPackCashbackEnabled;
            private int _PTCCategoryPolicy, _DecimalPricePer1000ViewsPtc, _RegistrationDiscountValue, _RegistrationDiscountDays, _CashLinkViewEnabled;
            private string _PTCDefaultCaptchaQuestion;
            private DateTime _RegistrationDiscountStartDate;

            //Save & reload section
            internal void SavePtcAdverts()
            {
                SavePartially(IsUpToDate, buildPtcAdvertsProperties());
            }

            internal void ReloadPtcAdverts()
            {
                ReloadPartially(IsUpToDate, buildPtcAdvertsProperties());
            }

            private PropertyInfo[] buildPtcAdvertsProperties()
            {
                return new PropertyBuilder<AppSettingsTable>()
                    .Append(x => x.PtcAdvertDescriptionCost)
                    .Append(x => x.PtcAdvertFontBoldCost)
                    .Append(x => x.PtcAdvertGeolocationCost)
                    .Append(x => x.AdExposureProfitsEnabled)
                    .Append(x => x.GeolocationStatus)
                    .Append(x => x.ShowGeolocatedCountryFlags)
                    .Append(x => x.IsExternalIFrameEnabled)
                    .Append(x => x.PTCCategoryPolicyInt)
                    .Append(x => x.PTCStartPagePrice)
                    .Append(x => x.PTCStartPageEnabled)
                    .Append(x => x.PTCCreditsEnabled)
                    .Append(x => x.PTCCaptchaQuestionPrice)
                    .Append(x => x.PTCDefaultCaptchaQuestion)
                    .Append(x => x.PTCCaptchaQuestionPrice)
                    .Append(x => x.StarredAdsEnabled)
                    .Append(x => x.StarredAdsPrice)
                    .Append(x => x.FeedbackCaptchaEnabled)
                    .Append(x => x.DisableMoneyEarningsInPTC)
                    .Append(x => x.ShowAdvertiserAvatar)
                    .Append(x => x.FavoriteAdsEnabled)
                    .Append(x => x.DynamicPTCPriceEnabled)
                    .Append(x => x.BasePricePer1000ViewsPtc)
                    .Append(x => x.DecimalPricePer1000ViewsPtc)
                    .Append(x => x.PtcPriceBasedOnTotalActiveMembersEnabled)
                    .Append(x => x.RegistrationDiscountEnabled)
                    .Append(x => x.RegistrationDiscountValue)
                    .Append(x => x.RegistrationDiscountStartDate)
                    .Append(x => x.RegistrationDiscountDays)
                    .Append(x => x.BlockPtcAdvertsAfterMissingPointer)
                    .Append(x => x.PTCImagesEnabled)
                    .Append(x => x.ExposureCategoriesEnabled)
                    .Append(x => x.CashLinkViewEnabledInt)
                    .Append(x => x.CashLinkCrediterEnabled)
                    .Append(x => x.AdvertPTCPackCashbackEnabled)
                    .Build();
            }
            
        }
        public enum PTCCategoryPolicy
        {
            None = 0,
            Custom = 2,
        }

        public enum PTCViewMode
        {
            None = 0,
            PTC = 1,
            CashLink = 2
        }
    }
}