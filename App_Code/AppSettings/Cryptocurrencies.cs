using Prem.PTC.Utils;
using System.Reflection;
using Titan.Cryptocurrencies;

namespace Prem.PTC
{
    public static partial class AppSettings
    {

        public static class Cryptocurrencies
        {
            public static string CoinbaseAPIKey { get { return appSettings.CoinbaseAPIKey; } set { appSettings.CoinbaseAPIKey = value; } }
            public static string CoinbaseAPISecret { get { return appSettings.CoinbaseAPISecret; } set { appSettings.CoinbaseAPISecret = value; } }
            public static bool IsCoinbaseMerchant { get { return appSettings.IsCoinbaseMerchant; } set { appSettings.IsCoinbaseMerchant = value; } }            

            public static string BlocktrailAPIKey { get { return appSettings.BlocktrailAPIKey; } set { appSettings.BlocktrailAPIKey = value; } }
            public static string BlocktrailAPIKeySecret { get { return appSettings.BlocktrailAPIKeySecret; } set { appSettings.BlocktrailAPIKeySecret = value; } }
            public static string BlocktrailWalletIdentifier { get { return appSettings.BlocktrailWalletIdentifier; } set { appSettings.BlocktrailWalletIdentifier = value; } }
            public static string BlocktrailWalletPassword { get { return appSettings.BlocktrailWalletPassword; } set { appSettings.BlocktrailWalletPassword = value; } }

            public static string CoinPaymentsApiKey { get { return appSettings.CoinPaymentsApiKey; } set { appSettings.CoinPaymentsApiKey = value; } }
            public static string CoinPaymentsSecretPIN { get { return appSettings.CoinPaymentsSecretPIN; } set { appSettings.CoinPaymentsSecretPIN = value; } }
            public static string CoinPaymentsYourMerchantID { get { return appSettings.CoinPaymentsYourMerchantID; } set { appSettings.CoinPaymentsYourMerchantID = value; } }
            public static string CoinPaymentsIPNSecret { get { return appSettings.CoinPaymentsIPNSecret; } set { appSettings.CoinPaymentsIPNSecret = value; } }
          
            public static CoinbaseAddressesPolicy CoinbaseAddressesPolicy
            {
                get { return (CoinbaseAddressesPolicy)appSettings.CoinbaseAddressesPolicy; }
                set { appSettings.CoinbaseAddressesPolicy = (int)value; }
            }

            public static void Save()
            {
                appSettings.SaveBitCoins();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadBitCoins();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {

            [Column("CoinPaymentsApiKey")]
            internal string CoinPaymentsApiKey
            {
                get { return _CoinPaymentsApiKey; }
                set { _CoinPaymentsApiKey = value; SetUpToDateAsFalse(); }
            }

            [Column("CoinPaymentsSecretPIN")]
            internal string CoinPaymentsSecretPIN
            {
                get { return _CoinPaymentsSecretPIN; }
                set { _CoinPaymentsSecretPIN = value; SetUpToDateAsFalse(); }
            }

            [Column("CoinPaymentsYourMerchantID")]
            internal string CoinPaymentsYourMerchantID
            {
                get { return _CoinPaymentsYourMerchantID; }
                set { _CoinPaymentsYourMerchantID = value; SetUpToDateAsFalse(); }
            }

            [Column("CoinPaymentsIPNSecret")]
            internal string CoinPaymentsIPNSecret
            {
                get { return _CoinPaymentsIPNSecret; }
                set { _CoinPaymentsIPNSecret = value; SetUpToDateAsFalse(); }
            }            

            [Column("CoinbaseAPIKey")]
            internal string CoinbaseAPIKey
            {
                get { return _CoinbaseAPIKey; }
                set { _CoinbaseAPIKey = value; SetUpToDateAsFalse(); }
            }

            [Column("CoinbaseAPISecret")]
            internal string CoinbaseAPISecret
            {
                get { return _CoinbaseAPISecret; }
                set { _CoinbaseAPISecret = value; SetUpToDateAsFalse(); }
            }

            [Column("CoinbaseAddressesPolicy")]
            internal int CoinbaseAddressesPolicy
            {
                get { return _CoinbaseAddressesPolicy; }
                set { _CoinbaseAddressesPolicy = value; SetUpToDateAsFalse(); }
            }

            [Column("IsCoinbaseMerchant")]
            internal bool IsCoinbaseMerchant
            {
                get { return _IsCoinbaseMerchant; }
                set { _IsCoinbaseMerchant = value; SetUpToDateAsFalse(); }
            }

            [Column("BlocktrailAPIKey")]
            internal string BlocktrailAPIKey
            {
                get { return _BlocktrailAPIKey; }
                set { _BlocktrailAPIKey = value; SetUpToDateAsFalse(); }
            }

            [Column("BlocktrailAPIKeySecret")]
            internal string BlocktrailAPIKeySecret
            {
                get { return _BlocktrailAPIKeySecret; }
                set { _BlocktrailAPIKeySecret = value; SetUpToDateAsFalse(); }
            }

            [Column("BlocktrailWalletIdentifier")]
            internal string BlocktrailWalletIdentifier
            {
                get { return _BlocktrailWalletIdentifier; }
                set { _BlocktrailWalletIdentifier = value; SetUpToDateAsFalse(); }
            }

            [Column("BlocktrailWalletPassword")]
            internal string BlocktrailWalletPassword
            {
                get { return _BlocktrailWalletPassword; }
                set { _BlocktrailWalletPassword = value; SetUpToDateAsFalse(); }
            }
            
            bool  _IsCoinbaseMerchant;
            string _CoinPaymentsApiKey, _CoinPaymentsSecretPIN, _CoinPaymentsYourMerchantID,
                _CoinbaseAPIKey, _CoinPaymentsIPNSecret, _CoinbaseAPISecret, _BlocktrailAPIKeySecret, _BlocktrailAPIKey, _BlocktrailWalletIdentifier, _BlocktrailWalletPassword;
            int _CoinbaseAddressesPolicy;

            internal void SaveBitCoins()
            {
                SavePartially(IsUpToDate, buildBitCoinsProperties());
            }
            internal void ReloadBitCoins()
            {
                ReloadPartially(IsUpToDate, buildBitCoinsProperties());
            }

            private PropertyInfo[] buildBitCoinsProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.CoinPaymentsApiKey)
                    .Append(x => x.CoinPaymentsSecretPIN)
                    .Append(x => x.CoinPaymentsYourMerchantID)
                    .Append(x => x.CoinPaymentsIPNSecret)
                    .Append(x => x.CoinbaseAPIKey)
                    .Append(x => x.CoinbaseAPISecret)
                    .Append(x => x.CoinbaseAddressesPolicy)
                    .Append(x => x.IsCoinbaseMerchant)
                    .Append(x => x.BlocktrailAPIKey)
                    .Append(x => x.BlocktrailAPIKeySecret)
                    .Append(x => x.BlocktrailWalletIdentifier)
                    .Append(x => x.BlocktrailWalletPassword);

                return exValues.Build();
            }
        }
    }

}
