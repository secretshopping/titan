using Prem.PTC.Utils;
using System.Reflection;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class MiniVideo
        {
            public static int MiniVideoKeepDays
            {
                get { return appSettings.MiniVideoKeepDays; }
                set { appSettings.MiniVideoKeepDays = value; }
            }

            public static int MiniVideoKeepForUserDays
            {
                get { return appSettings.MiniVideoKeepForUserDays; }
                set { appSettings.MiniVideoKeepForUserDays = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadMiniVideo();
            }

            public static void Save()
            {
                appSettings.SaveMiniVideo();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("MiniVideoKeepDays")]
            internal int MiniVideoKeepDays
            {
                get { return _MiniVideoKeepDays; }
                set { _MiniVideoKeepDays = value; SetUpToDateAsFalse(); }
            }

            [Column("MiniVideoKeepForUserDays")]
            internal int MiniVideoKeepForUserDays
            {
                get { return _MiniVideoKeepForUserDays; }
                set { _MiniVideoKeepForUserDays = value; SetUpToDateAsFalse(); }
            }

            private int _MiniVideoKeepDays, _MiniVideoKeepForUserDays;

            internal void ReloadMiniVideo()
            {
                ReloadPartially(IsUpToDate, buildMiniVideoProperties());
            }

            internal void SaveMiniVideo()
            {
                SavePartially(IsUpToDate, buildMiniVideoProperties());
            }

            private PropertyInfo[] buildMiniVideoProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.MiniVideoKeepDays)
                    .Append(x => x.MiniVideoKeepForUserDays);

                return exValues.Build();
            }
        }
    }
}