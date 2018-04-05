using System.Reflection;
using Prem.PTC.Utils;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class GlobalAdvertsSettings
        {
            public static bool RequireFocusOnAdvertsSurf
            {
                get { return appSettings.RequireFocusOnAdvertsSurf; }
                set { appSettings.RequireFocusOnAdvertsSurf = value; }
            }

            public static bool CountOfMembersFromCountryInGeoLocation
            {
                get { return appSettings.CountOfMembersFromCountryInGeoLocation; }
                set { appSettings.CountOfMembersFromCountryInGeoLocation = value; }
            }

            public static bool AdvertisersInfoOnSurfPage
            {
                get { return appSettings.AdvertisersInfoOnSurfPage; }
                set { appSettings.AdvertisersInfoOnSurfPage = value; }
            }

            public static void Save()
            {
                appSettings.SaveGlobalAdvertsSettings();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadGlobalAdvertsSettings();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("RequireFocusOnAdvertsSurf")]
            internal bool RequireFocusOnAdvertsSurf { get { return _RequireFocusOnAdvertsSurf; } set { _RequireFocusOnAdvertsSurf = value; SetUpToDateAsFalse(); } }
            
            [Column("CountOfMembersFromCountryInGeoLocation")]
            internal bool CountOfMembersFromCountryInGeoLocation { get { return _CountOfMembersFromCountryInGeoLocation; } set { _CountOfMembersFromCountryInGeoLocation = value; SetUpToDateAsFalse(); } }

            [Column("AdvertisersInfoOnSurfPage")]
            internal bool AdvertisersInfoOnSurfPage { get { return _AdvertisersInfoOnSurfPage; } set { _AdvertisersInfoOnSurfPage = value; SetUpToDateAsFalse(); } }

            private bool _RequireFocusOnAdvertsSurf, _CountOfMembersFromCountryInGeoLocation, _AdvertisersInfoOnSurfPage;            

            //Save & reload section
            internal void SaveGlobalAdvertsSettings()
            {
                SavePartially(IsUpToDate, buildGlobalAdvertsSettingsProperties());
            }

            internal void ReloadGlobalAdvertsSettings()
            {
                ReloadPartially(IsUpToDate, buildGlobalAdvertsSettingsProperties());
            }

            private PropertyInfo[] buildGlobalAdvertsSettingsProperties()
            {
                return new PropertyBuilder<AppSettingsTable>()
                    .Append(x => x.RequireFocusOnAdvertsSurf)
                    .Append(x => x.CountOfMembersFromCountryInGeoLocation)
                    .Append(x => x.AdvertisersInfoOnSurfPage)
                    .Build();
            }
        }
    }
}