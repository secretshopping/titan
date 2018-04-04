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
        public static partial class TrafficExchange
        {
            public static int TimeBetweenAdsRedirectInSeconds{ get { return appSettings.TimeBetweenAdsRedirectInSeconds; } set { appSettings.TimeBetweenAdsRedirectInSeconds = value; } }
            public static bool CreditBasedOnDurationEnabled { get { return appSettings.CreditBasedOnDurationEnabled; } set { appSettings.CreditBasedOnDurationEnabled = value; } }

            public static int AmountOfAdsToWatchForcedByAdmin { get { return appSettings.NumberOfAdsToWatchForcedByAdmin; } set { appSettings.NumberOfAdsToWatchForcedByAdmin = value; } }

            public static int AmountOfPointsPerWatchForcedByAdmin { get { return appSettings.NumberOfPointsPerWatchForcedByAdmin; } set { appSettings.NumberOfPointsPerWatchForcedByAdmin = value; } }

            public static void Save()
            {
                appSettings.SaveTrafficExchange();
            }
            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) { appSettings.ReloadTrafficExchange(); }
            }           
        }
        internal partial class AppSettingsTable : BaseTableObject
        {
            #region Columns

            [Column("TimeBetweenTEAdsRedirectInSeconds")]
            internal int TimeBetweenAdsRedirectInSeconds
            {
                get { return _TimeBetweenAdsRedirectInSeconds; }
                set { _TimeBetweenAdsRedirectInSeconds = value; SetUpToDateAsFalse(); }
            }

            [Column("TECreditBasedOnDurationEnabled")]
            internal bool CreditBasedOnDurationEnabled
            {
                get { return _CreditBasedOnDurationEnabled; }
                set { _CreditBasedOnDurationEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AmountOfAdsToWatchForcedByAdmin")]
            internal int NumberOfAdsToWatchForcedByAdmin
            {
                get { return _NumberOfAdsToWatchForcedByAdmin; }
                set { _NumberOfAdsToWatchForcedByAdmin = value; SetUpToDateAsFalse(); }
            }

            [Column("AmountOfPointsPerWatchForcedByAdmin")]
            internal int NumberOfPointsPerWatchForcedByAdmin
            {
                get { return _NumberOfPointsPerWatchForcedByAdmin; }
                set { _NumberOfPointsPerWatchForcedByAdmin = value; SetUpToDateAsFalse(); }
            }

            int _TimeBetweenAdsRedirectInSeconds, _NumberOfAdsToWatchForcedByAdmin, _NumberOfPointsPerWatchForcedByAdmin;
            bool _CreditBasedOnDurationEnabled;
            #endregion

            internal void SaveTrafficExchange()
            {
                SavePartially(IsUpToDate, buildTrafficExchangeProperties());
            }
            internal void ReloadTrafficExchange()
            {
                ReloadPartially(IsUpToDate, buildTrafficExchangeProperties());
            }

            private PropertyInfo[] buildTrafficExchangeProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.TimeBetweenAdsRedirectInSeconds)
                    .Append(x => x.CreditBasedOnDurationEnabled)
                    .Append(x => x.NumberOfAdsToWatchForcedByAdmin)
                    .Append(x => x.NumberOfPointsPerWatchForcedByAdmin);
                return exValues.Build();
            }          
        }
    }
}
