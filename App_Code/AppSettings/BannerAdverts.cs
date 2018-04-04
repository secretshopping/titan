using Prem.PTC.Advertising;
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
        public static class BannerAdverts
        {
            public static bool IsConstantBannerEnabled
            {
                get { return appSettings.BannerAdvertConstantBannerEnabled; }
                set { appSettings.BannerAdvertConstantBannerEnabled = value; }
            }

            public static int ConstantBannerWidth
            {
                get { return appSettings.BannerAdvertConstantBannerWidth; }
                set { appSettings.BannerAdvertConstantBannerWidth = value; }
            }

            public static int ConstantBannerHeight
            {
                get { return appSettings.BannerAdvertConstantBannerHeight; }
                set { appSettings.BannerAdvertConstantBannerHeight = value; }
            }

            public static int NormalBannerWidth
            {
                get { return appSettings.NormalBannerWidth; }
                set { appSettings.NormalBannerWidth = value; }
            }

            public static int NormalBannerHeight
            {
                get { return appSettings.NormalBannerHeight; }
                set { appSettings.NormalBannerHeight = value; }
            }

            public static int LostBidsReturnPercent
            {
                get { return appSettings.LostBidReturnPercent; }
                set { appSettings.LostBidReturnPercent = value; }
            }

            public static BannerPolicy AdvertisingPolicy
            {
                get { return (BannerPolicy)appSettings.BannerAdvertisingPolicy; }
                set { appSettings.BannerAdvertisingPolicy = (int)value; }
            }

            public static bool ImpressionsEnabled
            {
                get { return appSettings.AreBannerImpressionsEnabled; }
                set { appSettings.AreBannerImpressionsEnabled = value; }
            }

            public static bool GeoloactionEnabled
            {
                get { return appSettings.BannerGeoloactionEnabled; }
                set { appSettings.BannerGeoloactionEnabled = value; }
            }

            public static Money GeolocationCost
            {
                get { return appSettings.BannerGeolocationCost; }
                set { appSettings.BannerGeolocationCost = value; }
            }

            public static Money StartingAmount
            {
                get { return appSettings.StartingAmount; }
                set { appSettings.StartingAmount = value; }
            }

            public static Money StartingAmountConstant
            {
                get { return appSettings.StartingAmountConstant; }
                set { appSettings.StartingAmountConstant = value; }
            }

            public static Money BidAmount
            {
                get { return appSettings.BidAmount; }
                set { appSettings.BidAmount = value; }
            }

            public static Money BidAmountConstant
            {
                get { return appSettings.BidAmountConstant; }
                set { appSettings.BidAmountConstant = value; }
            }

            public static int AuctionsPerDay
            {
                get { return appSettings.AuctionsPerDay; }
                set { appSettings.AuctionsPerDay = value; }
            }

            public static int SurfBannerDimensionsID
            {
                get { return appSettings.DefaultBannerDimensionsID; }
                set { appSettings.DefaultBannerDimensionsID = value; }
            }

            public static bool HideAllBannersEnabled
            {
                get { return appSettings.HideAllBannersEnabled; }
                set { appSettings.HideAllBannersEnabled = value; }
            }

            public static TimeSpan AuctionTime { get { return TimeSpan.FromHours(24 / AuctionsPerDay); } }

            public static void Save()
            {
                appSettings.SaveBannerAdverts();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadBannerAdverts();
            }

            public static string ClicksImpressionsTypeString
            {
                get
                {
                    return AppSettings.BannerAdverts.ImpressionsEnabled ? "Impressions" : "Clicks"; 
                }
            }

            public static string GetEndModeText(End.Mode mode)
            {
                if (mode == End.Mode.Clicks)
                    return ClicksImpressionsTypeString;

                return mode.ToString();
            }
        }


        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("BannerAdvertConstantBannerEnabled")]
            internal bool BannerAdvertConstantBannerEnabled { get { return _bannerAdvertConstantBannerEnabled; } set { _bannerAdvertConstantBannerEnabled = value; SetUpToDateAsFalse(); } }

            [Column("BannerAdvertConstantBannerWidth")]
            internal int BannerAdvertConstantBannerWidth { get { return _bannerAdvertConstantBannerWidth; } set { _bannerAdvertConstantBannerWidth = value; SetUpToDateAsFalse(); } }

            [Column("BannerAdvertConstantBannerHeight")]
            internal int BannerAdvertConstantBannerHeight { get { return _bannerAdvertConstantBannerHeight; } set { _bannerAdvertConstantBannerHeight = value; SetUpToDateAsFalse(); } }

            [Column("NormalBannerWidth")]
            internal int NormalBannerWidth { get { return _NormalBannerWidth; } set { _NormalBannerWidth = value; SetUpToDateAsFalse(); } }

            [Column("NormalBannerHeight")]
            internal int NormalBannerHeight { get { return _NormalBannerHeight; } set { _NormalBannerHeight = value; SetUpToDateAsFalse(); } }

            [Column("BannerAdvertisingPolicy")]
            internal int BannerAdvertisingPolicy { get { return _BannerAdvertisingPolicy; } set { _BannerAdvertisingPolicy = value; SetUpToDateAsFalse(); } }

            [Column("LostBidReturnPercent")]
            internal int LostBidReturnPercent { get { return _LostBidReturnPercent; } set { _LostBidReturnPercent = value; SetUpToDateAsFalse(); } }

            [Column("AreBannerImpressionsEnabled")]
            internal bool AreBannerImpressionsEnabled { get { return _AreBannerImpressionsEnabled; } set { _AreBannerImpressionsEnabled = value; SetUpToDateAsFalse(); } }

            [Column("BannerGeoloactionEnabled")]
            internal bool BannerGeoloactionEnabled { get { return _BannerGeoloactionEnabled; } set { _BannerGeoloactionEnabled = value; SetUpToDateAsFalse(); } }

            [Column("BannerGeolocationCost")]
            internal Money BannerGeolocationCost { get { return _BannerGeolocationCost; } set { _BannerGeolocationCost = value; SetUpToDateAsFalse(); } }

            [Column("BidAmount")]
            internal Money BidAmount { get { return _BidAmount; } set { _BidAmount = value; SetUpToDateAsFalse(); } }

            [Column("BidAmountConstant")]
            internal Money BidAmountConstant { get { return _BidAmountConstant; } set { _BidAmountConstant = value; SetUpToDateAsFalse(); } }

            [Column("StartingAmount")]
            internal Money StartingAmount { get { return _StartingAmount; } set { _StartingAmount = value; SetUpToDateAsFalse(); } }

            [Column("StartingAmountConstant")]
            internal Money StartingAmountConstant { get { return _StartingAmountConstant; } set { _StartingAmountConstant = value; SetUpToDateAsFalse(); } }

            [Column("AuctionsPerDay")]
            internal int AuctionsPerDay { get { return _AuctionsPerDay; } set { _AuctionsPerDay = value; SetUpToDateAsFalse(); } }

            [Column("DefaultBannerDimensionsID")]
            internal int DefaultBannerDimensionsID { get { return _DefaultBannerDimensionsID; } set { _DefaultBannerDimensionsID = value; SetUpToDateAsFalse(); } }

            [Column("HideAllBannersEnabled")]
            internal bool HideAllBannersEnabled { get { return _HideAllBannersEnabled; } set { _HideAllBannersEnabled = value; SetUpToDateAsFalse(); } }

            private int _bannerAdvertConstantBannerWidth, _bannerAdvertConstantBannerHeight, _NormalBannerWidth, _NormalBannerHeight, _BannerAdvertisingPolicy, _LostBidReturnPercent,
                _AuctionsPerDay, _DefaultBannerDimensionsID;
            private bool _bannerAdvertConstantBannerEnabled, _AreBannerImpressionsEnabled, _BannerGeoloactionEnabled, _HideAllBannersEnabled;
            private Money _BannerGeolocationCost, _BidAmount, _StartingAmount, _StartingAmountConstant, _BidAmountConstant;

            //Save & reload section

            internal void SaveBannerAdverts()
            {
                SavePartially(IsUpToDate, buildBannerAdvertsProperties());
            }

            internal void ReloadBannerAdverts()
            {
                ReloadPartially(IsUpToDate, buildBannerAdvertsProperties());
            }

            private PropertyInfo[] buildBannerAdvertsProperties()
            {
                return new PropertyBuilder<AppSettingsTable>()
                    .Append(x => x.BannerAdvertConstantBannerEnabled)
                    .Append(x => x.BannerAdvertConstantBannerWidth)
                    .Append(x => x.BannerAdvertConstantBannerHeight)
                    .Append(x => x.NormalBannerWidth)
                    .Append(x => x.NormalBannerHeight)
                    .Append(x => x.BannerAdvertisingPolicy)
                    .Append(x => x.AreBannerImpressionsEnabled)
                    .Append(x => x.BannerGeoloactionEnabled)
                    .Append(x => x.BannerGeolocationCost)
                    .Append(x => x.BidAmount)
                    .Append(x => x.LostBidReturnPercent)
                    .Append(x => x.StartingAmount)
                    .Append(x => x.StartingAmountConstant)
                    .Append(x => x.BidAmountConstant)
                    .Append(x => x.AuctionsPerDay)
                    .Append(x => x.DefaultBannerDimensionsID)
                    .Append(x => x.HideAllBannersEnabled)
                    .Build();
            }
        }

    }
}