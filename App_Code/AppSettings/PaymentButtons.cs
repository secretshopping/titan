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
        public static class PaymentButtons
        {
            public static bool AdPackNormalPaymentButtonEnabled { get { return appSettings.AdPackNormalPaymentButtonEnabled; } set { appSettings.AdPackNormalPaymentButtonEnabled = value; } }
            public static bool AdPackProcessorButtonEnabled { get { return appSettings.AdPackProcessorButtonEnabled; } set { appSettings.AdPackProcessorButtonEnabled = value; } }

            public static bool UpgradeNormalPaymentButtonEnabled { get { return appSettings.UpgradeNormalPaymentButtonEnabled; } set { appSettings.UpgradeNormalPaymentButtonEnabled = value; } }

            public static bool UpgradeProcessorButtonEnabled { get { return appSettings.UpgradeProcessorButtonEnabled; } set { appSettings.UpgradeProcessorButtonEnabled = value; } }
            public static void Save()
            {
                appSettings.SavePaymentButtons();
            }
            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadPaymentButtons();
            }
        }
        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("AdPackNormalPaymentButtonEnabled")]
            internal bool AdPackNormalPaymentButtonEnabled
            {
                get { return _AdPackNormalPaymentButtonEnabled; }
                set { _AdPackNormalPaymentButtonEnabled = value; SetUpToDateAsFalse(); }
            }
            [Column("AdPackProcessorButtonEnabled")]
            internal bool AdPackProcessorButtonEnabled
            {
                get { return _AdPackProcessorButtonEnabled; }
                set { _AdPackProcessorButtonEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("UpgradeNormalPaymentButtonEnabled")]
            internal bool UpgradeNormalPaymentButtonEnabled
            {
                get { return _UpgradeNormalPaymentButtonEnabled; }
                set { _UpgradeNormalPaymentButtonEnabled = value; SetUpToDateAsFalse(); }
            }
            [Column("UpgradeProcessorButtonEnabled")]
            internal bool UpgradeProcessorButtonEnabled
            {
                get { return _UpgradeProcessorButtonEnabled; }
                set { _UpgradeProcessorButtonEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _AdPackNormalPaymentButtonEnabled, _AdPackProcessorButtonEnabled, _UpgradeNormalPaymentButtonEnabled, _UpgradeProcessorButtonEnabled;

            internal void SavePaymentButtons()
            {
                SavePartially(IsUpToDate, buildPaymentButtonsProperties());
            }
            internal void ReloadPaymentButtons()
            {
                ReloadPartially(IsUpToDate, buildPaymentButtonsProperties());
            }

            private PropertyInfo[] buildPaymentButtonsProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.AdPackNormalPaymentButtonEnabled)
                    .Append(x => x.AdPackProcessorButtonEnabled)
                    .Append(x => x.UpgradeNormalPaymentButtonEnabled)
                    .Append(x => x.UpgradeProcessorButtonEnabled);

                return exValues.Build();
            }
        }
    }
}
