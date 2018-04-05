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
        public static class CryptocurrencyTrading
        {
            public static string CryptocurrencySign { get { return appSettings.CryptocurrencySign; } set { appSettings.CryptocurrencySign = value; } }

            public static string CryptocurrencyCode { get { return appSettings.CryptocurrencyCode; } set { appSettings.CryptocurrencyCode = value; } }

        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            #region Columns

            [Column("CCPlatformCryptocurrencySign")]
            internal string CryptocurrencySign
            {
                get { return _CryptocurrencSign; }
                set { _CryptocurrencSign = value; SetUpToDateAsFalse(); }
            }

            [Column("CCPlatformCryptocurrencyCode")]
            internal string CryptocurrencyCode
            {
                get { return _CryptocurrencyCode; }
                set { _CryptocurrencyCode = value; SetUpToDateAsFalse(); }
            }

            string _CryptocurrencSign, _CryptocurrencyCode;
            int _EscrowTime;
            #endregion

            internal void SaveCryptocurrencyPlatformSettings()
            {
                SavePartially(IsUpToDate, buildCryptocurrencyTradingPlatformProperties());
            }

            internal void ReloadCryptocurrencyPlatformSettings()
            {
                ReloadPartially(IsUpToDate, buildCryptocurrencyTradingPlatformProperties());
            }

            private PropertyInfo[] buildCryptocurrencyTradingPlatformProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.CryptocurrencyCode)
                    .Append(x => x.CryptocurrencySign);

                return exValues.Build();
            }

        }
    }
}

