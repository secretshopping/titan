using Prem.PTC;
using Prem.PTC.Utils;
using Resources;
using System;

namespace Titan.Cryptocurrencies
{
    [Serializable]
    public class BitcoinCryptocurrency : Cryptocurrency
    {
        protected static string CoinmarketcapID = "bitcoin";
        public override bool Available { get { return AppSettings.TitanModules.HasModule(3); } }
        public static CryptocurrencyType cryptocurrencyType = CryptocurrencyType.BTC;

        #region Display

        public override string CurrencySign { get { return "฿"; } }
        public override string CurrencyDisplaySignBefore { get { return "฿"; } }
        public override string CurrencyDisplaySignAfter { get { return String.Empty; } }

        #endregion

        public BitcoinCryptocurrency() : base((int)cryptocurrencyType, CoinmarketcapID) { }
        public override CryptocurrencyType CryptocurrencyType { get { return cryptocurrencyType; } }


    }
}