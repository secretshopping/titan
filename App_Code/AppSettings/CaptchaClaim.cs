using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Prem.PTC.Utils;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class CaptchaClaim
        {
            public static bool IsSetup
            {
                get
                {
                    return !(String.IsNullOrEmpty(CaptchaClaimSiteKey)
                        || String.IsNullOrEmpty(CaptchaClaimSecretKey));
                }
            }

            public static string CaptchaClaimSiteKey
            {
                get { return appSettings.CaptchaClaimSiteKey; }
                set
                {
                    appSettings.CaptchaClaimSiteKey = value;
                }
            }

            public static string CaptchaClaimSecretKey
            {
                get { return appSettings.CaptchaClaimSecretKey; }
                set
                {
                    appSettings.CaptchaClaimSecretKey = value;
                }
            }

            public static int CaptchaClaimHashes
            {
                get { return appSettings.CaptchaClaimHashes; }
                set
                {
                    appSettings.CaptchaClaimHashes = value;
                }
            }

            public static BalanceType CaptchaClaimPrizeType
            {
                get { return (BalanceType)appSettings.CaptchaClaimPrizeType; }

                set { appSettings.CaptchaClaimPrizeType = (int)value; }
            }

            public static decimal CaptchaClaimPrize
            {
                get { return appSettings.CaptchaClaimPrize; }
                set
                {
                    appSettings.CaptchaClaimPrize = value;
                }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadCaptchaClaim();
            }

            public static void Save()
            {
                appSettings.SaveCaptchaClaim();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("CaptchaClaimSiteKey")]
            internal string CaptchaClaimSiteKey { get { return _CaptchaClaimSiteKey; } set { _CaptchaClaimSiteKey = value; SetUpToDateAsFalse(); } }

            [Column("CaptchaClaimSecretKey")]
            internal string CaptchaClaimSecretKey { get { return _CaptchaClaimSecretKey; } set { _CaptchaClaimSecretKey = value; SetUpToDateAsFalse(); } }
            
            [Column("CaptchaClaimHashes")]
            internal int CaptchaClaimHashes { get { return _CaptchaClaimHashes; } set { _CaptchaClaimHashes = value; SetUpToDateAsFalse(); } }

            [Column("CaptchaClaimPrizeType")]
            internal int CaptchaClaimPrizeType { get { return _CaptchaClaimPrizeType; } set { _CaptchaClaimPrizeType = value; SetUpToDateAsFalse(); } }

            [Column("CaptchaClaimPrize")]
            internal decimal CaptchaClaimPrize { get { return _CaptchaClaimPrize; } set { _CaptchaClaimPrize = value; SetUpToDateAsFalse(); } }

            private int _CaptchaClaimHashes, _CaptchaClaimPrizeType;
            private decimal _CaptchaClaimPrize;
            private string _CaptchaClaimSiteKey, _CaptchaClaimSecretKey;

            //Save & reload section

            internal void ReloadCaptchaClaim()
            {
                ReloadPartially(IsUpToDate, buildCaptchaClaimProperties());
            }

            internal void SaveCaptchaClaim()
            {
                SavePartially(IsUpToDate, buildCaptchaClaimProperties());
            }

            private PropertyInfo[] buildCaptchaClaimProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.CaptchaClaimSiteKey)
                    .Append(x => x.CaptchaClaimSecretKey)
                    .Append(x => x.CaptchaClaimHashes)
                    .Append(x => x.CaptchaClaimPrizeType)
                    .Append(x => x.CaptchaClaimPrize);
                return exValues.Build();
            }
        }
    }
}