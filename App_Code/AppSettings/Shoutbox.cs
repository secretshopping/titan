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

        public static class Shoutbox
        {
            public static ShoutboxDisplayMode DisplayMode
            {
                get { return (ShoutboxDisplayMode)appSettings.ShoutboxDisplayMode; }
                set { appSettings.ShoutboxDisplayMode = (int)value; }
            }

            public static ShoutboxDisplayContent DisplayContent
            {
                get { return (ShoutboxDisplayContent)appSettings.ShoutboxDisplayContent; }
                set { appSettings.ShoutboxDisplayContent = (int)value; }
            }

            public static bool ShoutboxIconsEnabled
            {
                get { return (bool)appSettings.ShoutboxIconsEnabled; }
                set { appSettings.ShoutboxIconsEnabled = (bool)value; }
            }

            public static bool ShowMemberCountryFlag
            {
                get { return (bool)appSettings.ShowMemberCountryFlag; }
                set { appSettings.ShowMemberCountryFlag = (bool)value; }
            }

            public static bool ShowAdminCountryFlag
            {
                get { return (bool)appSettings.ShowAdminCountryFlag; }
                set { appSettings.ShowAdminCountryFlag = (bool)value; }
            }

            public static bool ExternalLinksEnabled
            {
                get { return (bool)appSettings.ExternalLinksEnabled; }
                set { appSettings.ExternalLinksEnabled = (bool)value; }
            }

            public static bool CommandsEnabled
            {
                get { return (bool)appSettings.CommandsEnabled; }
                set { appSettings.CommandsEnabled = (bool)value; }
            }
            
            public static bool TipCommandEnabled
            {
                get { return (bool)appSettings.TipCommandEnabled; }
                set { appSettings.TipCommandEnabled = (bool)value; }
            }

            public static bool ShowCPANetworkAndLink
            {
                get { return (bool)appSettings.ShowCPANetworkAndLink; }
                set { appSettings.ShowCPANetworkAndLink = (bool)value; }
            }

            public static string ShoutboxAdminColor
            {
                get { return appSettings.ShoutboxAdminColor; }
                set { appSettings.ShoutboxAdminColor = value; }
            }

            public static string ShoutboxMorderatorColor
            {
                get { return appSettings.ShoutboxMorderatorColor; }
                set { appSettings.ShoutboxMorderatorColor = value; }
            }

            public static bool DefaultBannedPolicyEnabled
            {
                get { return (bool)appSettings.DefaultBannedPolicyEnabled; }
                set { appSettings.DefaultBannedPolicyEnabled = (bool)value; }
            }

            public static bool ShowLastRegistrations
            {
                get { return appSettings.ShowLastRegistrations; }
                set { appSettings.ShowLastRegistrations = value; }
            }

            public static bool AreBannedWordsCaseSensitive
            {
                get { return appSettings.BannedWordsCaseSensitive; }
                set { appSettings.BannedWordsCaseSensitive = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadShoutbox();
            }

            public static void Save()
            {
                appSettings.SaveShoutbox();
            }
        }


        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("ShoutboxDisplayMode")]
            internal int ShoutboxDisplayMode { get { return S1; } set { S1 = value; SetUpToDateAsFalse(); } }

            [Column("ShoutboxDisplayContent")]
            internal int ShoutboxDisplayContent { get { return S2; } set { S2 = value; SetUpToDateAsFalse(); } }

            [Column("ShoutboxIconsEnabled")]
            internal bool ShoutboxIconsEnabled { get { return _ShoutboxIconsEnabled; } set { _ShoutboxIconsEnabled = value; SetUpToDateAsFalse(); } }

            [Column("ShoutboxShowCountry")]
            internal bool ShowMemberCountryFlag { get { return _ShowMemberCountryFlag; } set { _ShowMemberCountryFlag = value; SetUpToDateAsFalse(); } }

            [Column("ShoutboxShowAdminCountry")]
            internal bool ShowAdminCountryFlag { get { return _ShowAdminCountryFlag; } set { _ShowAdminCountryFlag = value; SetUpToDateAsFalse(); } }

            [Column("ExternalLinksEnabled")]
            internal bool ExternalLinksEnabled { get { return _ExternalLinksEnabled; } set { _ExternalLinksEnabled = value; SetUpToDateAsFalse(); } }

            [Column("CommandsEnabled")]
            internal bool CommandsEnabled { get { return _CommandsEnabled; } set { _CommandsEnabled = value; SetUpToDateAsFalse(); } }

            [Column("TipCommandEnabled")]
            internal bool TipCommandEnabled { get { return _TipCommandEnabled; } set { _TipCommandEnabled = value; SetUpToDateAsFalse(); } }

            [Column("ShoutBoxShowCPANetworkAndLink")]
            internal bool ShowCPANetworkAndLink { get { return _ShowCPANetworkAndLink; } set { _ShowCPANetworkAndLink = value; SetUpToDateAsFalse(); } }

            [Column("ShoutboxAdminColor")]
            internal string ShoutboxAdminColor { get { return _ShoutboxAdminColor; } set { _ShoutboxAdminColor = value; SetUpToDateAsFalse(); } }

            [Column("ShoutboxMorderatorColor")]
            internal string ShoutboxMorderatorColor { get { return _ShoutboxMorderatorColor; } set { _ShoutboxMorderatorColor = value; SetUpToDateAsFalse(); } }

            [Column("DefaultBannedPolicyEnabled")]
            internal bool DefaultBannedPolicyEnabled { get { return _DefaultBannedPolicyEnabled; } set { _DefaultBannedPolicyEnabled = value;  SetUpToDateAsFalse(); } }

            [Column("ShoutboxShowLastRegistrations")]
            internal bool ShowLastRegistrations { get { return _ShowLastRegistrations; } set { _ShowLastRegistrations = value; SetUpToDateAsFalse(); } }

            [Column("ShoutboxBannedWordsCaseSensitive")]
            internal bool BannedWordsCaseSensitive { get { return _BannedWordsCaseSensitive; } set { _BannedWordsCaseSensitive = value; SetUpToDateAsFalse(); } }


            private bool _ShoutboxIconsEnabled, _ShowMemberCountryFlag, _ExternalLinksEnabled, _CommandsEnabled, _TipCommandEnabled, _ShowAdminCountryFlag, _ShowCPANetworkAndLink,
                         _DefaultBannedPolicyEnabled, _ShowLastRegistrations, _BannedWordsCaseSensitive;
            private int S1, S2;
            string _ShoutboxAdminColor, _ShoutboxMorderatorColor;

            internal void ReloadShoutbox()
            {
                ReloadPartially(IsUpToDate, buildShoutboxProperties());
            }

            internal void SaveShoutbox()
            {
                SavePartially(IsUpToDate, buildShoutboxProperties());
            }

            private PropertyInfo[] buildShoutboxProperties()
            {
                var paymentsValues = new PropertyBuilder<AppSettingsTable>();
                paymentsValues
                    .Append(x => x.ShoutboxDisplayContent)
                    .Append(x => x.ShoutboxDisplayMode)
                    .Append(x => x.ShoutboxIconsEnabled)
                    .Append(x => x.ShowMemberCountryFlag)
                    .Append(x => x.ShowAdminCountryFlag)
                    .Append(x => x.ExternalLinksEnabled)
                    .Append(x => x.CommandsEnabled)
                    .Append(x => x.ShowCPANetworkAndLink)
                    .Append(x => x.ShoutboxAdminColor)
                    .Append(x => x.ShoutboxMorderatorColor)
                    .Append(x => x.TipCommandEnabled)
                    .Append(x => x.DefaultBannedPolicyEnabled)
                    .Append(x => x.ShowLastRegistrations)
                    .Append(x => x.BannedWordsCaseSensitive)
                    ;

                return paymentsValues.Build();
            }
        }

    }
}