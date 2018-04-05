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
        public static class SearchAndVideo
        {
            public static GPTSeachMode SearchMode
            {
                get { return (GPTSeachMode)appSettings.SearchMode; }
                set { appSettings.SearchMode = (int)value; }
            }

            public static GPTVideoMode VideoMode
            {
                get { return (GPTVideoMode)appSettings.VideoMode; }
                set { appSettings.VideoMode = (int)value; }
            }

            public static CreditVideoPolicy CreditVideoPolicy
            {
                get { return (CreditVideoPolicy)appSettings.CreditVideoPolicy; }
                set { appSettings.CreditVideoPolicy = (int)value; }
            }

            public static string YahooSearchAPI
            {
                get { return appSettings.YahooSearchAPI; }
                set { appSettings.YahooSearchAPI = value; }
            }

            public static int SearchCreditingBlockedInMinutes
            {
                get { return appSettings.SearchCreditingBlockedInMinutes; }
                set { appSettings.SearchCreditingBlockedInMinutes = value; }
            }

            public static int MaxPointsDailyForSearch
            {
                get { return appSettings.MaxPointsDailyForSearch; }
                set { appSettings.MaxPointsDailyForSearch = value; }
            }

            public static int MaxPointsDailyForVideo
            {
                get { return appSettings.MaxPointsDailyForVideo; }
                set { appSettings.MaxPointsDailyForVideo = value; }
            }

            public static int CreditAfterSetTime
            {
                get { return appSettings.CreditAfterSetTime; }
                set { appSettings.CreditAfterSetTime = value; }
            }

            public static bool VideoWidgetEnabled
            {
                get { return appSettings.VideoWidgetEnabled; }
                set { appSettings.VideoWidgetEnabled = value; }
            }

            public static string VideoWidgetCode
            {
                get { return appSettings.VideoWidgetCode; }
                set { appSettings.VideoWidgetCode = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadSearchAndVideo();
            }

            public static void Save()
            {
                appSettings.SaveSearchAndVideo();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("CreditVideoPolicy")]
            internal int CreditVideoPolicy { get { return _CreditVideoPolicy; } set { _CreditVideoPolicy = value; SetUpToDateAsFalse(); } }

            [Column("YahooSearchAPI")]
            internal string YahooSearchAPI { get { return _YahooSearchAPI; } set { _YahooSearchAPI = value; SetUpToDateAsFalse(); } }

            [Column("VideoMode")]
            internal int VideoMode { get { return _VideoMode; } set { _VideoMode = value; SetUpToDateAsFalse(); } }

            [Column("SearchMode")]
            internal int SearchMode { get { return _SearchMode; } set { _SearchMode = value; SetUpToDateAsFalse(); } }

            [Column("SearchTimerMinutes")]
            public int SearchCreditingBlockedInMinutes { get { return _SearchCreditingBlockedInMinutes; } set { _SearchCreditingBlockedInMinutes = value; SetUpToDateAsFalse(); } }

            [Column("MaxPointsDailyForSearch")]
            public int MaxPointsDailyForSearch { get { return _MaxPointsDailyForSearch; } set { _MaxPointsDailyForSearch = value; SetUpToDateAsFalse(); } }

            [Column("MaxPointsDailyForVideo")]
            public int MaxPointsDailyForVideo { get { return _MaxPointsDailyForVideo; } set { _MaxPointsDailyForVideo = value; SetUpToDateAsFalse(); } }

            [Column("CreditAfterSetTime")]
            public int CreditAfterSetTime { get { return _CreditAfterSetTime; } set { _CreditAfterSetTime = value; SetUpToDateAsFalse(); } }

            [Column("VideoWidgetEnabled")]
            public bool VideoWidgetEnabled { get { return _VideoWidgetEnabled; } set { _VideoWidgetEnabled = value; SetUpToDateAsFalse(); } }

            [Column("VideoWidgetCode")]
            public string VideoWidgetCode { get { return _VideoWidgetCode; } set { _VideoWidgetCode = value; SetUpToDateAsFalse(); } }

            private int _CreditVideoPolicy, _VideoMode, _SearchMode, _SearchCreditingBlockedInMinutes, _MaxPointsDailyForSearch, _MaxPointsDailyForVideo, _CreditAfterSetTime;
            private string _YahooSearchAPI, _VideoWidgetCode;
            private bool _VideoWidgetEnabled;

            //Save & reload section

            internal void ReloadSearchAndVideo()
            {
                ReloadPartially(IsUpToDate, buildSearchAndVideoProperties());
            }

            internal void SaveSearchAndVideo()
            {
                SavePartially(IsUpToDate, buildSearchAndVideoProperties());
            }

            private PropertyInfo[] buildSearchAndVideoProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.CreditVideoPolicy)
                    .Append(x => x.YahooSearchAPI)
                    .Append(x => x.VideoMode)
                    .Append(x => x.SearchCreditingBlockedInMinutes)
                    .Append(x => x.MaxPointsDailyForSearch)
                    .Append(x => x.MaxPointsDailyForVideo)
                    .Append(x => x.CreditAfterSetTime)
                    .Append(x => x.SearchMode)
                    .Append(x => x.VideoWidgetEnabled)
                    .Append(x => x.VideoWidgetCode);
                return exValues.Build();
            }
        }

    }

    public enum GPTSeachMode
    {
        Off = 0,
        PayPerSearch = 1
    }

    public enum GPTVideoMode
    {
        Off = 0,
        PayPerWatch = 1
    }

    public enum CreditVideoPolicy
    {
        After5s = 0
    }
}
