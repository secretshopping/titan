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
        public static class SlotMachine
        {
            public static int SlotMachineMinRewardValue
            {
                get { return appSettings.SlotMachineMinRewardValue; }
                set { appSettings.SlotMachineMinRewardValue = value; }
            }

            public static int SlotMachineMaxRewardValue
            {
                get { return appSettings.SlotMachineMaxRewardValue; }
                set { appSettings.SlotMachineMaxRewardValue = value; }
            }

            public static int SlotMachineMinChancesGiven
            {
                get { return appSettings.SlotMachineMinChancesGiven; }
                set { appSettings.SlotMachineMinChancesGiven = value; }
            }

            public static int SlotMachineMaxChancesGiven
            {
                get { return appSettings.SlotMachineMaxChancesGiven; }
                set { appSettings.SlotMachineMaxChancesGiven = value; }
            }

            public static int SlotMachineMinWinToDisplayInLatestActivity
            {
                get { return appSettings.SlotMachineMinWinToDisplayInLatestActivity; }
                set { appSettings.SlotMachineMinWinToDisplayInLatestActivity = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadSlotMachine();
            }

            public static void Save()
            {
                appSettings.SaveSlotMachine();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {

            [Column("SlotMachineMinRewardValue")]
            internal int SlotMachineMinRewardValue { get { return _SlotMachineMinRewardValue; } set { _SlotMachineMinRewardValue = value; SetUpToDateAsFalse(); } }

            [Column("SlotMachineMaxRewardValue")]
            internal int SlotMachineMaxRewardValue { get { return _SlotMachineMaxRewardValue; } set { _SlotMachineMaxRewardValue = value; SetUpToDateAsFalse(); } }

            [Column("SlotMachineMinChancesGiven")]
            internal int SlotMachineMinChancesGiven { get { return _SlotMachineMinChancesGiven; } set { _SlotMachineMinChancesGiven = value; SetUpToDateAsFalse(); } }

            [Column("SlotMachineMaxChancesGiven")]
            internal int SlotMachineMaxChancesGiven { get { return _SlotMachineMaxChancesGiven; } set { _SlotMachineMaxChancesGiven = value; SetUpToDateAsFalse(); } }

            [Column("SlotMachineMinWinToDisplayInLatestActivity")]
            internal int SlotMachineMinWinToDisplayInLatestActivity { get { return _SlotMachineMinWinToDisplayInLatestActivity; } set { _SlotMachineMinWinToDisplayInLatestActivity = value; SetUpToDateAsFalse(); } }

            private int _SlotMachineMinRewardValue, _SlotMachineMaxRewardValue, _SlotMachineMinChancesGiven, _SlotMachineMaxChancesGiven, _SlotMachineMinWinToDisplayInLatestActivity;

            //Save & reload section

            internal void ReloadSlotMachine()
            {
                ReloadPartially(IsUpToDate, buildSlotMachineProperties());
            }

            internal void SaveSlotMachine()
            {
                SavePartially(IsUpToDate, buildSlotMachineProperties());
            }

            private PropertyInfo[] buildSlotMachineProperties()
            {
                var addonsProperties = new PropertyBuilder<AppSettingsTable>();
                addonsProperties
                    .Append(x => x.SlotMachineMinChancesGiven)
                    .Append(x => x.SlotMachineMaxChancesGiven)
                    .Append(x => x.SlotMachineMaxRewardValue)
                    .Append(x => x.SlotMachineMinRewardValue)
                    .Append(x => x.SlotMachineMinWinToDisplayInLatestActivity);

                return addonsProperties.Build();
            }
        }
    }
}