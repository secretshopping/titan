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
        public static class Captcha
        {
            public static CaptchaType Type
            {
                get { return (CaptchaType)appSettings.CaptchaType; }

                set { appSettings.CaptchaType = (int)value; }
            }

            public static string ReCAPTCHASiteKey
            {
                get { return appSettings.ReCAPTCHASiteKey; }
                set
                {
                    appSettings.ReCAPTCHASiteKey = value;
                }
            }

            public static string ReCAPTCHASecretKey
            {
                get { return appSettings.ReCAPTCHASecretKey; }
                set
                {
                    appSettings.ReCAPTCHASecretKey = value;
                }
            }

            public static string SolveMediaVKey
            {
                get { return appSettings.SolveMediaVKey; }
                set
                {
                    appSettings.SolveMediaVKey = value;
                }
            }

            public static string SolveMediaCKey
            {
                get { return appSettings.SolveMediaCKey; }
                set
                {
                    appSettings.SolveMediaCKey = value;
                }
            }

            public static string CoinhiveSiteKey
            {
                get { return appSettings.CoinhiveSiteKey; }
                set
                {
                    appSettings.CoinhiveSiteKey = value;
                }
            }

            public static string CoinhiveSecretKey
            {
                get { return appSettings.CoinhiveSecretKey; }
                set
                {
                    appSettings.CoinhiveSecretKey = value;
                }
            }

            public static int CoinhiveHashes
            {
                get { return appSettings.CoinhiveHashes; }
                set
                {
                    appSettings.CoinhiveHashes = value;
                }
            }

            public static bool AllowMembersToChooseCaptcha
            {
                get { return appSettings.AllowMembersToChooseCaptcha; }
                set
                {
                    appSettings.AllowMembersToChooseCaptcha = value;
                }
            }


            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadCaptcha();
            }

            public static void Save()
            {
                appSettings.SaveCaptcha();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("CaptchaType")]
            internal int CaptchaType { get { return _CaptchaType; } set { _CaptchaType = value; SetUpToDateAsFalse(); } }

            [Column("ReCAPTCHASiteKey")]
            internal string ReCAPTCHASiteKey { get { return _ReCAPTCHASiteKey; } set { _ReCAPTCHASiteKey = value; SetUpToDateAsFalse(); } }

            [Column("ReCAPTCHASecretKey")]
            internal string ReCAPTCHASecretKey { get { return _ReCAPTCHASecretKey; } set { _ReCAPTCHASecretKey = value; SetUpToDateAsFalse(); } }

            [Column("SolveMediaVKey")]
            internal string SolveMediaVKey { get { return _SolveMediaVKey; } set { _SolveMediaVKey = value; SetUpToDateAsFalse(); } }

            [Column("SolveMediaCKey")]
            internal string SolveMediaCKey { get { return _SolveMediaCKey; } set { _SolveMediaCKey = value; SetUpToDateAsFalse(); } }

            [Column("CoinhiveSiteKey")]
            internal string CoinhiveSiteKey { get { return _CoinhiveSiteKey; } set { _CoinhiveSiteKey = value; SetUpToDateAsFalse(); } }

            [Column("CoinhiveSecretKey")]
            internal string CoinhiveSecretKey { get { return _CoinhiveSecretKey; } set { _CoinhiveSecretKey = value; SetUpToDateAsFalse(); } }

            [Column("CoinhiveHashes")]
            internal int CoinhiveHashes { get { return _CoinhiveHashes; } set { _CoinhiveHashes = value; SetUpToDateAsFalse(); } }

            [Column("AllowMembersToChooseCaptcha")]
            internal bool AllowMembersToChooseCaptcha { get { return _AllowMembersToChooseCaptcha; } set { _AllowMembersToChooseCaptcha = value; SetUpToDateAsFalse(); } }

            private int _CaptchaType, _CoinhiveHashes;
            private string _ReCAPTCHASiteKey, _ReCAPTCHASecretKey, _SolveMediaVKey, _SolveMediaCKey, _CoinhiveSiteKey, _CoinhiveSecretKey;
            private bool _AllowMembersToChooseCaptcha;

            //Save & reload section

            internal void ReloadCaptcha()
            {
                ReloadPartially(IsUpToDate, buildCaptchaProperties());
            }

            internal void SaveCaptcha()
            {
                SavePartially(IsUpToDate, buildCaptchaProperties());
            }

            private PropertyInfo[] buildCaptchaProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.CaptchaType)
                    .Append(x => x.ReCAPTCHASiteKey)
                    .Append(x => x.ReCAPTCHASecretKey)
                    .Append(x => x.SolveMediaVKey)
                    .Append(x => x.SolveMediaCKey)
                    .Append(x => x.CoinhiveSiteKey)
                    .Append(x => x.CoinhiveSecretKey)
                    .Append(x => x.CoinhiveHashes)
                    .Append(x => x.AllowMembersToChooseCaptcha);
                return exValues.Build();
            }
        }

    }
}