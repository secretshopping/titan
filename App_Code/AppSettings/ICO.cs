using Prem.PTC.Utils;
using System.Reflection;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class ICO
        {
            public static bool ICOStartNewStageIfPreviousEndedEarlierEnabled
            {
                get { return appSettings.ICOStartNewStageIfPreviousEndedEarlierEnabled; }
                set { appSettings.ICOStartNewStageIfPreviousEndedEarlierEnabled = value; }
            }

            public static int ICOPurchaseLimitPerUserPer15mins
            {
                get { return appSettings.ICOPurchaseLimitPerUserPer15mins; }
                set { appSettings.ICOPurchaseLimitPerUserPer15mins = value; }
            }

            public static string ICOInformationHTML
            {
                get { return appSettings.ICOInformationHTML; }
                set { appSettings.ICOInformationHTML = value; }
            }

            public static void Reload()
            {
                appSettings.ReloadICOSettings();
            }

            public static void Save()
            {
                appSettings.SaveICOSettings();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            #region Columns
            [Column("ICOStartNewStageIfPreviousEndedEarlierEnabled")]
            internal bool ICOStartNewStageIfPreviousEndedEarlierEnabled
            {
                get { return _ICOStartNewStageIfPreviousEndedEarlierEnabled; }
                set { _ICOStartNewStageIfPreviousEndedEarlierEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("ICOPurchaseLimitPerUserPer15mins")]
            internal int ICOPurchaseLimitPerUserPer15mins
            {
                get { return _ICOPurchaseLimitPerUserPer15mins; }
                set { _ICOPurchaseLimitPerUserPer15mins = value; SetUpToDateAsFalse(); }
            }

            [Column("ICOInformationHTML")]
            internal string ICOInformationHTML
            {
                get { return _ICOInformationHTML; }
                set { _ICOInformationHTML = value; SetUpToDateAsFalse(); }
            }

            private int _ICOPurchaseLimitPerUserPer15mins;
            private bool _ICOStartNewStageIfPreviousEndedEarlierEnabled;
            private string _ICOInformationHTML;
            #endregion

            internal void SaveICOSettings()
            {
                SavePartially(IsUpToDate, buildICOProperties());
            }

            internal void ReloadICOSettings()
            {
                ReloadPartially(IsUpToDate, buildICOProperties());
            }

            private PropertyInfo[] buildICOProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.ICOStartNewStageIfPreviousEndedEarlierEnabled)
                    .Append(x => x.ICOPurchaseLimitPerUserPer15mins)
                    .Append(x => x.ICOInformationHTML);

                return exValues.Build();
            }

        }
    }
}

