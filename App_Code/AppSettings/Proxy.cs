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

        public static class Proxy
        {
            public static ProxySMSType SMSType
            {
                get { return (ProxySMSType)appSettings.ProxySMSType; }

                set { appSettings.ProxySMSType = (int)value; }
            }

            public static string BlockScriptApiKey
            {
                get { return appSettings.BlockScriptApiKey; }

                set { appSettings.BlockScriptApiKey = value; }
            }

            public static string BlockScriptUrl
            {
                get { return appSettings.BlockScriptUrl; }

                set { appSettings.BlockScriptUrl = value; }
            }

            public static ProxyProviderType ProxyProviderType
            {
                get { return (ProxyProviderType)appSettings.ProxyProviderType; }

                set { appSettings.ProxyProviderType = (int)value; }
            }

            public static string ProxStopApiKey
            {
                get { return appSettings.ProxStopApiKey; }

                set { appSettings.ProxStopApiKey = value; }
            }

            public static IPVerificationPolicy IPPolicy
            {
                get { return (IPVerificationPolicy)appSettings.IPPolicy; }

                set { appSettings.IPPolicy = (int)value; }
            }

            public static string IpQualityScoreKey
            {
                get { return appSettings.IpQualityScoreKey; }

                set { appSettings.IpQualityScoreKey = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadProxy();
            }

            public static void Save()
            {
                appSettings.SaveProxy();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("IPPolicy")]
            internal int IPPolicy { get { return _IPPolicy; } set { _IPPolicy = value; SetUpToDateAsFalse(); } }

            [Column("ProxStopApiKey")]
            internal string ProxStopApiKey { get { return _ProxStopApiKey; } set { _ProxStopApiKey = value; SetUpToDateAsFalse(); } }

            [Column("BlockScriptApiKey")]
            internal string BlockScriptApiKey { get { return _BlockScriptApiKey; } set { _BlockScriptApiKey = value; SetUpToDateAsFalse(); } }

            [Column("ProxySMSType")]
            internal int ProxySMSType { get { return _ProxySMSType; } set { _ProxySMSType = value; SetUpToDateAsFalse(); } }

            [Column("BlockScriptUrl")]
            internal string BlockScriptUrl { get { return _BlockScriptUrl; } set { _BlockScriptUrl = value; SetUpToDateAsFalse(); } }

            [Column("ProxyProviderType")]
            internal int ProxyProviderType { get { return _ProxyProviderType; } set { _ProxyProviderType = value; SetUpToDateAsFalse(); } }

            [Column("IpQualityScoreKey")]
            internal string IpQualityScoreKey { get { return _IpQualityScoreKey; } set { _IpQualityScoreKey = value; SetUpToDateAsFalse(); } }


            int _IPPolicy, _ProxySMSType, _ProxyProviderType;

            string _ProxStopApiKey, _BlockScriptApiKey, _BlockScriptUrl, _IpQualityScoreKey;

            internal void ReloadProxy()
            {
                ReloadPartially(IsUpToDate, buildProxyProperties());
            }

            internal void SaveProxy()
            {
                SavePartially(IsUpToDate, buildProxyProperties());
            }

            private PropertyInfo[] buildProxyProperties()
            {
                var paymentsValues = new PropertyBuilder<AppSettingsTable>();
                paymentsValues
                    .Append(x => x.IPPolicy)
                    .Append(x => x.ProxStopApiKey)
                    .Append(x => x.BlockScriptApiKey)
                    .Append(x => x.ProxySMSType)
                    .Append(x => x.BlockScriptUrl)
                    .Append(x => x.ProxyProviderType)
                    .Append(x => x.IpQualityScoreKey)
                    ;

                return paymentsValues.Build();
            }
        }
    }
}