using Prem.PTC;
using Prem.PTC.Utils;
using System;

namespace Titan.Cryptocurrencies
{
    [Serializable]
    public class RippleCryptocurrency : Cryptocurrency
    {
        protected static string CoinmarketcapID = "ripple";
        public override bool Available { get { return AppSettings.TitanModules.HasModule(58); } }
        public static CryptocurrencyType cryptocurrencyType = CryptocurrencyType.XRP;

        public RippleCryptocurrency() : base((int)cryptocurrencyType, CoinmarketcapID) { }
        public override CryptocurrencyType CryptocurrencyType { get { return cryptocurrencyType; } }
    }
}