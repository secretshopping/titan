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
        public static class VacationAndInactivity
        {

            public static bool IsEnabled
            {
                get { return appSettings.IsVacationModeEnabled; }
                set
                {
                    appSettings.IsVacationModeEnabled = value;
                }
            }

            public static Money CostPerDay
            {
                get { return appSettings.VacationModeCostPerDay; }
                set
                {
                    appSettings.VacationModeCostPerDay = value;
                }
            }

            public static int DaysToInactivityCharge
            {
                get { return appSettings.DaysToInactivityCharge; }
                set
                {
                    appSettings.DaysToInactivityCharge = value;
                }
            }

            public static Money InactivityChargePerDay
            {
                get { return appSettings.InactivityChargePerDay; }
                set
                {
                    appSettings.InactivityChargePerDay = value;
                }
            }


            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadVacationMode();
            }

            public static void Save()
            {
                appSettings.SaveVacationMode();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("DaysToInactivityCharge")]
            internal int DaysToInactivityCharge { get { return _DaysToInactivityCharge; } set { _DaysToInactivityCharge = value; SetUpToDateAsFalse(); } }

            [Column("VacationModeCostPerDay")]
            internal Money VacationModeCostPerDay { get { return _VacationModeCostPerDay; } set { _VacationModeCostPerDay = value; SetUpToDateAsFalse(); } }

            [Column("InactivityChargePerDay")]
            internal Money InactivityChargePerDay { get { return _InactivityChargePerDay; } set { _InactivityChargePerDay = value; SetUpToDateAsFalse(); } }

            [Column("IsVacationModeEnabled")]
            internal bool IsVacationModeEnabled { get { return _IsVacationModeEnabled; } set { _IsVacationModeEnabled = value; SetUpToDateAsFalse(); } }

            private int _DaysToInactivityCharge;
            private Money _VacationModeCostPerDay, _InactivityChargePerDay;
            private bool _IsVacationModeEnabled;

            //Save & reload section

            internal void ReloadVacationMode()
            {
                ReloadPartially(IsUpToDate, buildVacationModeProperties());
            }

            internal void SaveVacationMode()
            {
                SavePartially(IsUpToDate, buildVacationModeProperties());
            }

            private PropertyInfo[] buildVacationModeProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.IsVacationModeEnabled)
                    .Append(x => x.VacationModeCostPerDay)
                    .Append(x => x.DaysToInactivityCharge)
                    .Append(x => x.InactivityChargePerDay);
                return exValues.Build();
            }
        }

    }
}