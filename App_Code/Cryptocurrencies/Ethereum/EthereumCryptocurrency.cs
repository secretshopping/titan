using Prem.PTC;
using Prem.PTC.Utils;
using System;

namespace Titan.Cryptocurrencies
{
    [Serializable]
    public class EthereumCryptocurrency : Cryptocurrency
    {
        protected static string CoinmarketcapID = "ethereum";
        public override bool Available { get { return AppSettings.TitanModules.HasModule(51); } }
        public static CryptocurrencyType cryptocurrencyType = CryptocurrencyType.ETH;

        public EthereumCryptocurrency() : base((int)cryptocurrencyType, CoinmarketcapID) { }
        public override CryptocurrencyType CryptocurrencyType { get { return cryptocurrencyType; } }

    }
}