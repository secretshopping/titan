using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC.Utils;
using System.Reflection;


namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class DBArchiver
        {
            public static int BalanceLogsKeptForDays { get { return appSettings.BalanceLogsKeptForDays; } set { appSettings.BalanceLogsKeptForDays = value; } }
            public static int HistoryLogsKeptForDays { get { return appSettings.HistoryLogsKeptForDays; } set { appSettings.HistoryLogsKeptForDays = value; } }
            public static int IPHistoryLogsKeptForDays { get { return appSettings.IPHistoryLogsKeptForDays; } set { appSettings.IPHistoryLogsKeptForDays = value; } }
            public static int OfferwallsLogsKeptForDays { get { return appSettings.OfferwallsLogsKeptForDays; } set { appSettings.OfferwallsLogsKeptForDays = value; } }
            public static int PostBackLogsKeptForDays { get { return appSettings.PostBackLogsKeptForDays; } set { appSettings.PostBackLogsKeptForDays = value; } }
            
            public static void Save()
            {
                appSettings.SaveDBArchiver();
            }
            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadDBArchiver();
            }
        }
        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("BalanceLogsKeptForDays")]
            internal int BalanceLogsKeptForDays
            {
                get { return _BalanceLogsKeptForDays; }
                set { _BalanceLogsKeptForDays = value; SetUpToDateAsFalse(); }
            }
            [Column("HistoryLogsKeptForDays")]
            internal int HistoryLogsKeptForDays
            {
                get { return _HistoryLogsKeptForDays; }
                set { _HistoryLogsKeptForDays = value; SetUpToDateAsFalse(); }
            }
            [Column("IPHistoryLogsKeptForDays")]
            internal int IPHistoryLogsKeptForDays
            {
                get { return _IPHistoryLogsKeptForDays; }
                set { _IPHistoryLogsKeptForDays = value; SetUpToDateAsFalse(); }
            }
            [Column("OfferwallsLogsKeptForDays")]
            internal int OfferwallsLogsKeptForDays
            {
                get { return _OfferwallsLogsKeptForDays; }
                set { _OfferwallsLogsKeptForDays = value; SetUpToDateAsFalse(); }
            }
            [Column("PostBackLogsKeptForDays")]
            internal int PostBackLogsKeptForDays
            {
                get { return _PostBackLogsKeptForDays; }
                set { _PostBackLogsKeptForDays = value; SetUpToDateAsFalse(); }
            }

            int _BalanceLogsKeptForDays, _HistoryLogsKeptForDays, _IPHistoryLogsKeptForDays, _OfferwallsLogsKeptForDays, _PostBackLogsKeptForDays;


            internal void SaveDBArchiver()
            {
                SavePartially(IsUpToDate, buildDBArchiverProperties());
            }
            internal void ReloadDBArchiver()
            {
                ReloadPartially(IsUpToDate, buildDBArchiverProperties());
            }

            private PropertyInfo[] buildDBArchiverProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.BalanceLogsKeptForDays)
                    .Append(x => x.HistoryLogsKeptForDays)
                    .Append(x => x.IPHistoryLogsKeptForDays)
                    .Append(x => x.OfferwallsLogsKeptForDays)
                    .Append(x => x.PostBackLogsKeptForDays);

                return exValues.Build();
            }
        }
    }
}
