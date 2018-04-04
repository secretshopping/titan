using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Titan.Leadership;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class LeadershipSystem
        {
            public static string Name
            {
                get { return appSettings.LeadershipSystemName; }
                set { appSettings.LeadershipSystemName = value; }
            }

            public static string Description
            {
                get { return appSettings.LeadershipSystemDescription; }
                set { appSettings.LeadershipSystemDescription = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadLeadershipSystem();
            }

            public static void Save()
            {
                appSettings.SaveLeadershipSystem();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("LeadershipSystemName")]
            internal string LeadershipSystemName { get { return _LeadershipSystemName; } set { _LeadershipSystemName = value; SetUpToDateAsFalse(); } }

            [Column("LeadershipSystemDescription")]
            internal string LeadershipSystemDescription { get { return _LeadershipSystemDescription; } set { _LeadershipSystemDescription = value;  SetUpToDateAsFalse(); } }

            private string _LeadershipSystemName, _LeadershipSystemDescription;

            internal void ReloadLeadershipSystem()
            {
                ReloadPartially(IsUpToDate, buildLeadershipSystemProperties());
            }

            internal void SaveLeadershipSystem()
            {
                SavePartially(IsUpToDate, buildLeadershipSystemProperties());
            }

            private PropertyInfo[] buildLeadershipSystemProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.LeadershipSystemName)
                    .Append(x => x.LeadershipSystemDescription);
                return exValues.Build();
            }
        }

    }
}