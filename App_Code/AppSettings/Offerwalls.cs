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
        public static class Offerwalls
        {
            public static string UniversalHandlerPassword
            {
                get { return appSettings.UniversalHandlerPassword; }

                set { appSettings.UniversalHandlerPassword = value; }
            }

            public static string CrowdFlowerName
            {
                get { return appSettings.CrowdFlowerName; }

                set { appSettings.CrowdFlowerName = value; }
            }

            public static string CrowdFlowerKey
            {
                get { return appSettings.CrowdFlowerKey; }

                set { appSettings.CrowdFlowerKey = value; }
            }

            public static string MochiID
            {
                get { return appSettings.MochiID; }

                set { appSettings.MochiID = value; }
            }

            public static bool IsMochiOn
            {
                get { return appSettings.IsMochiOn; }

                set { appSettings.IsMochiOn = value; }
            }

            public static bool ConvertCrowdflowerToMainBalance
            {
                get { return appSettings.ConvertCrowdflowerToMainBalance; }

                set { appSettings.ConvertCrowdflowerToMainBalance = value; }
            }

            public static string FamobiGameUserId
            {
                get { return appSettings.FamobiGameUserId; }
                set { appSettings.FamobiGameUserId = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadOfferwalls();
            }

            public static void Save()
            {
                appSettings.SaveOfferwalls();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {

            #region Columns

            [Column("UniversalHandlerPassword")]
            internal string UniversalHandlerPassword { get { return _UniversalHandlerPassword; } set { _UniversalHandlerPassword = value; SetUpToDateAsFalse(); } }

            [Column("CrowdFlower1")]
            internal string CrowdFlowerName { get { return _CrowdFlowerName; } set { _CrowdFlowerName = value; SetUpToDateAsFalse(); } }

            [Column("CrowdFlower2")]
            internal string CrowdFlowerKey { get { return _CrowdFlowerKey; } set { _CrowdFlowerKey = value; SetUpToDateAsFalse(); } }

            [Column("ConvertCrowdflowerToMainBalance")]
            internal bool ConvertCrowdflowerToMainBalance { get { return _ConvertCrowdflowerToMainBalance; } set { _ConvertCrowdflowerToMainBalance = value; SetUpToDateAsFalse(); } }

            [Column("Games1")]
            internal string MochiID { get { return _MochiID; } set { _MochiID = value; SetUpToDateAsFalse(); } }

            [Column("Games2")]
            internal bool IsMochiOn { get { return _IsMochiOn; } set { _IsMochiOn = value; SetUpToDateAsFalse(); } }

            [Column("FamobiGameUserId")]
            internal string FamobiGameUserId { get { return _famobiGameUserId; } set { _famobiGameUserId = value; SetUpToDateAsFalse(); } }

            private string _UniversalHandlerPassword, _CrowdFlowerName, _CrowdFlowerKey, _MochiID, _famobiGameUserId;

            private bool _ConvertCrowdflowerToMainBalance, _IsMochiOn;

            #endregion

            #region Offerwalls

            internal void ReloadOfferwalls()
            {
                ReloadPartially(IsUpToDate, buildOfferwallsProperties());
            }

            internal void SaveOfferwalls()
            {
                SavePartially(IsUpToDate, buildOfferwallsProperties());
            }

            private PropertyInfo[] buildOfferwallsProperties()
            {
                var paymentsValues = new PropertyBuilder<AppSettingsTable>();
                paymentsValues
                    .Append(x => x.UniversalHandlerPassword)
                    .Append(x => x.CrowdFlowerName)
                    .Append(x => x.CrowdFlowerKey)
                    .Append(x => x.MochiID)
                    .Append(x => x.IsMochiOn)
                    .Append(x => x.ConvertCrowdflowerToMainBalance)
                    .Append(x => x.FamobiGameUserId)
                    ;

                return paymentsValues.Build();
            }

            #endregion Offerwalls
        }
    }
}
