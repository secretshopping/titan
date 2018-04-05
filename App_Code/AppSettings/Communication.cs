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
        public static class Communication
        {

            public static string TawkLiveChatKey
            {
                get { return appSettings.TawkLiveChatKey; }
                set
                {
                    appSettings.TawkLiveChatKey = value;
                }
            }

            public static bool TawkLiveChatEnabled
            {
                get { return appSettings.TawkLiveChatEnabled; }
                set
                {
                    appSettings.TawkLiveChatEnabled = value;
                }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadCommunication();
            }

            public static void Save()
            {
                appSettings.SaveCommunication();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("TawkLiveChatKey")]
            internal string TawkLiveChatKey { get { return _TawkLiveChatKey; } set { _TawkLiveChatKey = value; SetUpToDateAsFalse(); } }

            [Column("TawkChatEnabled")]
            internal bool TawkLiveChatEnabled { get { return _TawkLiveChatEnabled; } set { _TawkLiveChatEnabled = value; SetUpToDateAsFalse(); } }

            private string _TawkLiveChatKey;
            private bool _TawkLiveChatEnabled;

            //Save & reload section

            internal void ReloadCommunication()
            {
                ReloadPartially(IsUpToDate, buildCommunicationProperties());
            }

            internal void SaveCommunication()
            {
                SavePartially(IsUpToDate, buildCommunicationProperties());
            }

            private PropertyInfo[] buildCommunicationProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.TawkLiveChatKey)
                    .Append(x => x.TawkLiveChatEnabled);
                return exValues.Build();
            }
        }

    }
}