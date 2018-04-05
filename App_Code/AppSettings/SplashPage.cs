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
        public static class SplashPage
        {
            public static bool SplashPageEnabled { get { return appSettings.SplashPageEnabled; } set { appSettings.SplashPageEnabled = value; } }
            public static string SplashPageYoutubeUrl { get { return appSettings.SplashPageYoutubeUrl; } set { appSettings.SplashPageYoutubeUrl = value; } }
            public static string SplashPageSlogan { get { return appSettings.SplashPageSlogan; } set { appSettings.SplashPageSlogan = value; } }

            public static void Save()
            {
                appSettings.SaveSplashPage();
            }
            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadSplashPage();
            }
        }
        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("SplashPageEnabled")]
            internal bool SplashPageEnabled
            {
                get { return _SplashPageEnabled; }
                set { _SplashPageEnabled = value; SetUpToDateAsFalse(); }
            }
            [Column("SplashPageYoutubeUrl")]
            internal string SplashPageYoutubeUrl
            {
                get { return _SplashPageYoutubeUrl; }
                set { _SplashPageYoutubeUrl = value; SetUpToDateAsFalse(); }
            }
            [Column("SplashPageSlogan")]
            internal string SplashPageSlogan
            {
                get { return _SplashPageSlogan; }
                set { _SplashPageSlogan = value; SetUpToDateAsFalse(); }
            }

            bool _SplashPageEnabled;
            string _SplashPageYoutubeUrl, _SplashPageSlogan;


            internal void SaveSplashPage()
            {
                SavePartially(IsUpToDate, buildSplashPageProperties());
            }
            internal void ReloadSplashPage()
            {
                ReloadPartially(IsUpToDate, buildSplashPageProperties());
            }

            private PropertyInfo[] buildSplashPageProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.SplashPageEnabled)
                    .Append(x => x.SplashPageYoutubeUrl)
                    .Append(x => x.SplashPageSlogan);

                return exValues.Build();
            }
        }
    }
}
