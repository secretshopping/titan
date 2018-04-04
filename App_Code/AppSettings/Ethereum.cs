using System;
using Prem.PTC.Utils;
using System.Reflection;
using Titan.Cryptocurrencies;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class Ethereum
        {
            public static bool ERC20TokenEnabled                { get { return appSettings.IsERC20TokenEnabled; } set { appSettings.IsERC20TokenEnabled = value; } }
            public static string ERC20TokenContract             { get { return appSettings.ERC20TokenContract; } set { appSettings.ERC20TokenContract = value; } }
            public static string ERC20TokenImageUrl             { get { return appSettings.ERC20TokenImageUrl; } set { appSettings.ERC20TokenImageUrl = value; } } 
            public static Money ERC20TokenRate                  { get { return appSettings.ERC20TokenRate; } set { appSettings.ERC20TokenRate = value; } }
            public static bool ERC20TokensFreezeSystemEnabled { get { return appSettings.ERC20TokensFreezeSystemEnabled; } set { appSettings.ERC20TokensFreezeSystemEnabled = value; } }
            public static int ERC20TokensFreezeTimeDays { get { return appSettings.ERC20TokensFreezeTimeDays; } set { appSettings.ERC20TokensFreezeTimeDays = value; } }

            #region ICOStatistics

            public static CryptocurrencyMoney ERC20StatsCirculatingSupply { get { return new CryptocurrencyMoney(CryptocurrencyType.ERC20Token, appSettings.ERC20StatsCirculatingSupply); } set { appSettings.ERC20StatsCirculatingSupply = value.ToDecimal(); } }

            public static CryptocurrencyMoney ERC20StatsReservedSupply { get { return new CryptocurrencyMoney(CryptocurrencyType.ERC20Token, appSettings.ERC20StatsReservedSupply); } set { appSettings.ERC20StatsReservedSupply = value.ToDecimal(); } }

            public static CryptocurrencyMoney ERC20StatsBurnedSupply { get { return new CryptocurrencyMoney(CryptocurrencyType.ERC20Token, appSettings.ERC20StatsBurnedSupply); } set { appSettings.ERC20StatsBurnedSupply = value.ToDecimal(); } }

            public static CryptocurrencyMoney ERC20StatsTotalSupply { get { return new CryptocurrencyMoney(CryptocurrencyType.ERC20Token, appSettings.ERC20StatsTotalSupply); } set { appSettings.ERC20StatsTotalSupply = value.ToDecimal(); } }
            
            #endregion

            public static String ParityUrl { get { return appSettings.ParityUrl; } set { appSettings.ParityUrl = value; } }

            public static void Save()
            {
                appSettings.SaveEthereum();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadEthereum();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            #region Columns
         
            [Column("ERC20TokenContract")]
            internal string ERC20TokenContract
            {
                get { return _ERC20TokenContract; }
                set { _ERC20TokenContract = value; SetUpToDateAsFalse(); }
            }

            [Column("ERC20TokenImageUrl")]
            internal string ERC20TokenImageUrl
            {
                get { return _ERC20TokenImageUrl; }
                set { _ERC20TokenImageUrl = value; SetUpToDateAsFalse(); }
            }

            [Column("ERC20TokenRate")]
            internal Money ERC20TokenRate
            {
                get { return _ERC20TokenRate; }
                set { _ERC20TokenRate = value; SetUpToDateAsFalse(); }
            }

            [Column("ERC20TokenEnabled")]
            internal bool IsERC20TokenEnabled
            {
                get { return _IsERC20TokenEnabled; }
                set { _IsERC20TokenEnabled = value; SetUpToDateAsFalse(); }
            }
            
            [Column("ParityUrl")]
            internal string ParityUrl
            {
                get { return _ParityUrl; }
                set { _ParityUrl = value; SetUpToDateAsFalse(); }
            }

            [Column("ERC20TokensFreezeSystemEnabled")]
            internal bool ERC20TokensFreezeSystemEnabled
            {
                get { return _ERC20TokensFreezeSystemEnabled; }
                set { _ERC20TokensFreezeSystemEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("ERC20TokensFreezeTimeDays")]
            internal int ERC20TokensFreezeTimeDays
            {
                get { return _ERC20TokensFreezeTimeDays; }
                set { _ERC20TokensFreezeTimeDays = value; SetUpToDateAsFalse(); }
            }

            [Column("ERC20StatsReservedSupply")]
            internal decimal ERC20StatsReservedSupply
            {
                get { return _ERC20StatsReservedSupply; }
                set { _ERC20StatsReservedSupply = value; SetUpToDateAsFalse(); }
            }

            [Column("ERC20StatsBurnedSupply")]
            internal decimal ERC20StatsBurnedSupply
            {
                get { return _ERC20StatsBurnedSupply; }
                set { _ERC20StatsBurnedSupply = value; SetUpToDateAsFalse(); }
            }

            [Column("ERC20StatsCirculatingSupply")]
            internal decimal ERC20StatsCirculatingSupply
            {
                get { return _ERC20StatsCirculatingSupply; }
                set { _ERC20StatsCirculatingSupply = value; SetUpToDateAsFalse(); }
            }

            [Column("ERC20StatsTotalSupply")]
            internal decimal ERC20StatsTotalSupply
            {
                get { return _ERC20StatsTotalSupply; }
                set { _ERC20StatsTotalSupply = value; SetUpToDateAsFalse(); }
            }

            int _DaysToActivateAddress, _ERC20TokenWithdrawalSourceBalance, _ERC20TokensFreezeTimeDays;
            Money _ERC20TokenRate, _WithdrawalFee, _MaxAutomaticPayoutValue, _MarketValueEnlargedBy, _CachedPrice;
            bool _IsERC20TokenEnabled, _IsERC20TokenDepositsEnabled, _IsERC20TokenWithdrawalsEnabled, _IsWithdrawalsEnabled, _ERC20TokensFreezeSystemEnabled;
            Decimal _WithdrawalFeePercent, _MarketValueMultipliedBy, _MinimumERC20TokenWithdrawal, _ERC20StatsReservedSupply, _ERC20StatsBurnedSupply, _ERC20StatsCirculatingSupply, _ERC20StatsTotalSupply;
            string _ERC20TokenName, _ERC20TokenTLA, _ERC20TokenContract, _ERC20TokenImageUrl, _ParityUrl;
            #endregion

            internal void SaveEthereum()
            {
                SavePartially(IsUpToDate, buildEthereumProperties());
            }
            internal void ReloadEthereum()
            {
                ReloadPartially(IsUpToDate, buildEthereumProperties());
            }

            private PropertyInfo[] buildEthereumProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.ERC20TokenContract)
                    .Append(x => x.ERC20TokenImageUrl)
                    .Append(x => x.ERC20TokenRate)
                    .Append(x => x.IsERC20TokenEnabled)
                    .Append(x => x.ParityUrl)
                    .Append(x => x.ERC20TokensFreezeSystemEnabled)
                    .Append(x => x.ERC20TokensFreezeTimeDays)
                    .Append(x => x.ERC20StatsReservedSupply)
                    .Append(x => x.ERC20StatsBurnedSupply)
                    .Append(x => x.ERC20StatsCirculatingSupply)
                    .Append(x => x.ERC20StatsTotalSupply);

                return exValues.Build();
            }
        }
    }

}
