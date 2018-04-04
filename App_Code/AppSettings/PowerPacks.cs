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
        public static class PowerPacks
        {
            public static string PackReqiuredMessage(int packNumber)
            {
                return string.Format("Power Pack #{0} required", packNumber);
            }

            public static string PackErrorMessage(int packNumber)
            {
                return string.Format("Purchase Power Pack #{0} first", packNumber);

            }

            public static string List
            {

                get
                {
                    if (Side == ScriptSide.AdminPanel)
                        return String.Empty;

                    return appSettings.PowerPacks;
                }

                set
                {
                    appSettings.PowerPacks = value;
                }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadPowerPacks();
            }

            public static void Save()
            {
                appSettings.SavePowerPacks();
            }
            public static bool HasPowerPack(string powerPackNumber)
            {
                var powerPacks = List.Split(';');

                if (powerPacks.Contains(powerPackNumber))
                    return true;
                return false;
            }
        }


        internal partial class AppSettingsTable : BaseTableObject
        {

            [Column("PowerPacks")]
            internal string PowerPacks { get { return _PowerPacks; } set { _PowerPacks = value; SetUpToDateAsFalse(); } }

            private string _PowerPacks;


            //Save & reload section

            internal void ReloadPowerPacks()
            {
                ReloadPartially(IsUpToDate, buildPowerPacksProperties());
            }

            internal void SavePowerPacks()
            {
                SavePartially(IsUpToDate, buildPowerPacksProperties());
            }

            private PropertyInfo[] buildPowerPacksProperties()
            {
                var paymentsValues = new PropertyBuilder<AppSettingsTable>();
                paymentsValues
                    .Append(x => x.PowerPacks);

                return paymentsValues.Build();
            }
        }

        
    }
}