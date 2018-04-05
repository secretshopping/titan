using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class Addons
        {
            public static bool IsCustomCounterEnabled
            {
                get { return appSettings.IsCustomCounterEnabled; }
                set { appSettings.IsCustomCounterEnabled = value; }
            }

            public static string CustomCounterTitle
            {
                get { return appSettings.CustomCounterTitle; }
                set { appSettings.CustomCounterTitle = value; }
            }

            public static DateTime CustomCounterDeadLine
            {
                get { return appSettings.CustomCounterDeadLine; }
                set { appSettings.CustomCounterDeadLine = value; }
            }

            public static bool IsProgressiveUpgradeEnabled
            {
                get { return appSettings.IsProgressiveUpgradeEnabled; }
                set { appSettings.IsProgressiveUpgradeEnabled = value; }
            }

            public static bool ShowLastJackpotsWinnersOnUserDashboard
            {
                get { return appSettings.ShowLastJackpotsWinnersOnUserDashboard; }
                set { appSettings.ShowLastJackpotsWinnersOnUserDashboard = value; }
            }

            public static bool PvpJackpotForceEveryUsertoAlwaysWin
            {
                get { return appSettings.PvpJackpotForceEveryUsertoAlwaysWinEnabled; }
                set { appSettings.PvpJackpotForceEveryUsertoAlwaysWinEnabled = value; }
            }

            public static int PvpJackpotBotWinChancePercent
            {
                get { return appSettings.PvpJackpotBotWinChancePercent; }
                set { appSettings.PvpJackpotBotWinChancePercent = value; }
            }

            public static bool PvpJackpotBotEnabled
            {
                get { return appSettings.PvpJackpotBotEnabled; }
                set { appSettings.PvpJackpotBotEnabled = value; }
            }

            public static int PvpJackpotOpponentSearchTime
            {
                get { return appSettings.PvpJackpotOpponentSearchTime; }
                set { appSettings.PvpJackpotOpponentSearchTime = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadAddons();
            }

            public static void Save()
            {
                appSettings.SaveAddons();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {

            [Column("IsCustomCounterEnabled")]
            internal bool IsCustomCounterEnabled { get { return _IsCustomCounterEnabled; } set { _IsCustomCounterEnabled = value; SetUpToDateAsFalse(); } }

            [Column("CustomCounterTitle")]
            internal string CustomCounterTitle { get { return _CustomCounterTitle; } set { _CustomCounterTitle = value; SetUpToDateAsFalse(); } }

            [Column("CustomCounterDeadLine")]
            internal DateTime CustomCounterDeadLine { get { return _CustomCounterDeadLine; } set { _CustomCounterDeadLine = value; SetUpToDateAsFalse(); } }

            [Column("IsProgressiveUpgradeEnabled")]
            internal bool IsProgressiveUpgradeEnabled { get { return _IsProgressiveUpgradeEnabled; } set { _IsProgressiveUpgradeEnabled = value; SetUpToDateAsFalse(); } }

            [Column("ShowLastJackpotsWinnersOnUserDashboard")]
            internal bool ShowLastJackpotsWinnersOnUserDashboard { get { return _ShowLastJackpotsWinnersOnUserDashboard; } set { _ShowLastJackpotsWinnersOnUserDashboard = value; SetUpToDateAsFalse(); } }

            [Column("PvpJackpotForceEveryUsertoAlwaysWin")]
            internal bool PvpJackpotForceEveryUsertoAlwaysWinEnabled { get { return _PvpJackpotForceEveryUsertoAlwaysWin; } set { _PvpJackpotForceEveryUsertoAlwaysWin = value; SetUpToDateAsFalse(); } }

            [Column("PvpJackpotBotWinChancePercent")]
            internal int PvpJackpotBotWinChancePercent { get { return _PvpJackpotBotWinChancePercent; } set { _PvpJackpotBotWinChancePercent = value; SetUpToDateAsFalse(); } }

            [Column("PvpJackpotBotEnabled")]
            internal bool PvpJackpotBotEnabled { get { return _PvpJackpotBotEnabled; } set { _PvpJackpotBotEnabled = value; SetUpToDateAsFalse(); } }

            [Column("PvpJackpotOpponentSearchTime")]
            internal int PvpJackpotOpponentSearchTime { get { return _PvpJackpotOpponentSearchTime; } set { _PvpJackpotOpponentSearchTime = value; SetUpToDateAsFalse(); } }

            private bool _IsCustomCounterEnabled, _IsProgressiveUpgradeEnabled, _ShowLastJackpotsWinnersOnUserDashboard, _PvpJackpotForceEveryUsertoAlwaysWin, _PvpJackpotBotEnabled;
            private string _CustomCounterTitle;
            private DateTime _CustomCounterDeadLine;
            private int _PvpJackpotBotWinChancePercent, _PvpJackpotOpponentSearchTime;

            //Save & reload section

            internal void ReloadAddons()
            {
                ReloadPartially(IsUpToDate, buildAddonsProperties());
            }

            internal void SaveAddons()
            {
                SavePartially(IsUpToDate, buildAddonsProperties());
            }

            private PropertyInfo[] buildAddonsProperties()
            {
                var addonsProperties = new PropertyBuilder<AppSettingsTable>();
                addonsProperties
                    .Append(x => x.IsCustomCounterEnabled)
                    .Append(x => x.CustomCounterTitle)
                    .Append(x => x.CustomCounterDeadLine)
                    .Append(x => x.IsProgressiveUpgradeEnabled)
                    .Append(x => x.ShowLastJackpotsWinnersOnUserDashboard)
                    .Append(x => x.PvpJackpotForceEveryUsertoAlwaysWinEnabled)
                    .Append(x => x.PvpJackpotBotWinChancePercent)
                    .Append(x => x.PvpJackpotBotEnabled)
                    .Append(x => x.PvpJackpotOpponentSearchTime);

                return addonsProperties.Build();
            }
        }

    }
}