using Prem.PTC.Utils;
using System.Reflection;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static partial class AffiliateNetwork
        {
            public static bool AffiliateNetworkEnabled
            {
                get { return appSettings.AffiliateNetworkEnabled; }
                set { appSettings.AffiliateNetworkEnabled = value; }
            }

            public static int MinutesBetweenExternalBannerClicksPerIp
            {
                get { return appSettings.MinutesBetweenExternalBannerClicksPerIp; }
                set { appSettings.MinutesBetweenExternalBannerClicksPerIp = value; }
            }

            public static void Save()
            {
                appSettings.SaveAffiliateNetwork();
            }
            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) { appSettings.ReloadAffiliateNetwork(); }
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            #region Columns

            [Column("AffiliateNetworkEnabled")]
            internal bool AffiliateNetworkEnabled
            {
                get { return _AffiliateNetworkEnabled; }
                set { _AffiliateNetworkEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("MinutesBetweenExternalBannerClicksPerIp")]
            internal int MinutesBetweenExternalBannerClicksPerIp
            {
                get { return _MinutesBetweenExternalBannerClicksPerIp; }
                set { _MinutesBetweenExternalBannerClicksPerIp = value; SetUpToDateAsFalse(); }
            }

            bool _AffiliateNetworkEnabled;
            int _MinutesBetweenExternalBannerClicksPerIp;
            #endregion

            internal void SaveAffiliateNetwork()
            {
                SavePartially(IsUpToDate, buildAffiliateNetworkProperties());
            }
            internal void ReloadAffiliateNetwork()
            {
                ReloadPartially(IsUpToDate, buildAffiliateNetworkProperties());
            }

            private PropertyInfo[] buildAffiliateNetworkProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.AffiliateNetworkEnabled)
                    .Append(x => x.MinutesBetweenExternalBannerClicksPerIp);
                return exValues.Build();
            }
        }
    }
}
