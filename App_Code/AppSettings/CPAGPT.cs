using Prem.PTC.Members;
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
        public static class CPAGPT
        {
            public static int MoneyTakenFromCPAOffersPercent
            {
                get { return appSettings.MoneyTakenFromCPAOffersPercent; }
                set
                {
                    appSettings.MoneyTakenFromCPAOffersPercent = value;
                }
            }

            public static Money MinimalCPAPrice
            {
                get { return appSettings.MinCPAPrice; }
                set
                {
                    appSettings.MinCPAPrice = value;
                }
            }

            public static bool DeviceTypeDistinctionEnabled
            {
                get { return appSettings.DeviceTypeDistinctionEnabled; }
                set
                {
                    appSettings.DeviceTypeDistinctionEnabled = value;
                }
            }

            public static bool ReadOnlyModeEnabled
            {
                get { return appSettings.AnonymousAccessEnabled; }
                set
                {
                    appSettings.AnonymousAccessEnabled = value;
                }
            }

            public static bool AutoApprovalEnabled
            {
                get { return appSettings.CPAGPTAutoApprovalEnabled; }
                set
                {
                    appSettings.CPAGPTAutoApprovalEnabled = value;
                }
            }

            public static bool DailyNotDailyButtonsEnabled
            {
                get { return appSettings.CPAGPTDailyNotDailyButtonsEnabled; }
                set
                {
                    appSettings.CPAGPTDailyNotDailyButtonsEnabled = value;
                }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadCPAGPT();
            }

            public static void Save()
            {
                appSettings.SaveCPAGPT();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("MoneyTakenFromCPAOffersPercent")]
            internal int MoneyTakenFromCPAOffersPercent { get { return _MoneyTakenFromCPAOffersPercent; } set { _MoneyTakenFromCPAOffersPercent = value; SetUpToDateAsFalse(); } }

            [Column("MinCPAPrice1")]
            internal Money MinCPAPrice { get { return _MinCPAPrice; } set { _MinCPAPrice = value; SetUpToDateAsFalse(); } }

            [Column("CPAGPTDeviceTypeDistinctionEnabled")]
            internal bool DeviceTypeDistinctionEnabled { get { return _DeviceTypeDistinctionEnabled; } set { _DeviceTypeDistinctionEnabled = value; SetUpToDateAsFalse(); } }

            [Column("CPAGPTAnonymousAccessEnabled")]
            internal bool AnonymousAccessEnabled { get { return _AnonymousAccessEnabled; } set { _AnonymousAccessEnabled = value; SetUpToDateAsFalse(); } }

            [Column("CPAGPTAutoApprovalEnabled")]
            internal bool CPAGPTAutoApprovalEnabled { get { return _CPAGPTAutoApprovalEnabled; } set { _CPAGPTAutoApprovalEnabled = value; SetUpToDateAsFalse(); } }

            [Column("CPAGPTDailyNotDailyButtonsEnabled")]
            internal bool CPAGPTDailyNotDailyButtonsEnabled { get { return _CPAGPTDailyNotDailyButtonsEnabled; } set { _CPAGPTDailyNotDailyButtonsEnabled = value; SetUpToDateAsFalse(); } }

            private int _MoneyTakenFromCPAOffersPercent;
            private Money _MinCPAPrice;
            private bool _DeviceTypeDistinctionEnabled, _AnonymousAccessEnabled, _CPAGPTAutoApprovalEnabled, _CPAGPTDailyNotDailyButtonsEnabled;

            //Save & reload section

            internal void ReloadCPAGPT()
            {
                ReloadPartially(IsUpToDate, buildCPAGPTProperties());
            }

            internal void SaveCPAGPT()
            {
                SavePartially(IsUpToDate, buildCPAGPTProperties());
            }

            private PropertyInfo[] buildCPAGPTProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.MoneyTakenFromCPAOffersPercent)
                    .Append(x => x.MinCPAPrice)
                    .Append(x => x.DeviceTypeDistinctionEnabled)
                    .Append(x => x.AnonymousAccessEnabled)
                    .Append(x => x.CPAGPTAutoApprovalEnabled)
                    .Append(x => x.CPAGPTDailyNotDailyButtonsEnabled);
                return exValues.Build();
            }
        }

    }
}