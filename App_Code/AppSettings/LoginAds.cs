using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC.Utils;
using System.Reflection;
/// <summary>
/// Summary description for BitCoin
/// </summary>

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static partial class LoginAds
        {
            public static Money Price { get { return appSettings.Price; } set { appSettings.Price = value; } }
            public static bool IsGeolocationEnabled { get { return appSettings.IsGeolocationEnabled; } set { appSettings.IsGeolocationEnabled = value; } }
            public static int AdsPerDay { get { return appSettings.AdsPerDay; } set { appSettings.AdsPerDay = value; } }
            public static int DisplayTime { get { return appSettings.DisplayTime; } set { appSettings.DisplayTime = value; } }
            public static bool LoginAdsCreditsEnabled { get { return appSettings.LoginAdsCreditsEnabled; } set { appSettings.LoginAdsCreditsEnabled = value; } }
            public static bool IsAdflyEnabled { get { return appSettings.IsAdflyLoginAdEnabled; } set { appSettings.IsAdflyLoginAdEnabled = value; } }
            public static string AdflyUserId { get { return appSettings.AdflyUserId; } set { appSettings.AdflyUserId = value; } }

            public static void Save()
            {
                appSettings.SaveLoginAds();
            }
            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) { appSettings.ReloadLoginAds(); }
            }           
        }
        internal partial class AppSettingsTable : BaseTableObject
        {
            #region Columns

            [Column("IsLoginAdsGeolocationEnabled")]
            internal bool IsGeolocationEnabled
            {
                get { return _IsGeolocationEnabled; }
                set { _IsGeolocationEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("LoginAdsPrice")]
            internal Money Price
            {
                get { return _Price; }
                set { _Price = value; SetUpToDateAsFalse(); }
            }

            [Column("LoginAdsPerDay")]
            internal int AdsPerDay
            {
                get { return _AdsPerDay; }
                set { _AdsPerDay = value; SetUpToDateAsFalse(); }
            }

            [Column("LoginAdsDisplayTime")]
            internal int DisplayTime
            {
                get { return _DisplayTime; }
                set { _DisplayTime = value; SetUpToDateAsFalse(); }
            }

            [Column("LoginAdsCreditsEnabled")]
            internal bool LoginAdsCreditsEnabled
            {
                get { return _LoginAdsCreditsEnabled; }
                set { _LoginAdsCreditsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("IsAdflyLoginAdEnabled")]
            internal bool IsAdflyLoginAdEnabled
            {
                get { return _IsAdflyLoginAdEnabled; }
                set { _IsAdflyLoginAdEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AdflyUserId")]
            internal string AdflyUserId
            {
                get { return _AdflyUserId; }
                set { _AdflyUserId = value; SetUpToDateAsFalse(); }
            }


            bool _IsGeolocationEnabled, _LoginAdsCreditsEnabled, _IsAdflyLoginAdEnabled;
            Int32 _AdsPerDay, _DisplayTime;
            Money _Price;
            string _AdflyUserId;
            #endregion

            internal void SaveLoginAds()
            {
                SavePartially(IsUpToDate, buildLoginAdsProperties());
            }
            internal void ReloadLoginAds()
            {
                ReloadPartially(IsUpToDate, buildLoginAdsProperties());
            }

            private PropertyInfo[] buildLoginAdsProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.IsGeolocationEnabled)
                    .Append(x => x.Price)
                    .Append(x => x.AdsPerDay)
                    .Append(x => x.DisplayTime)
                    .Append(x => x.IsAdflyLoginAdEnabled)
                    .Append(x => x.AdflyUserId)
                    .Append(x => x.LoginAdsCreditsEnabled);
                return exValues.Build();
            }          
        }
    }
}
