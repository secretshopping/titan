using Prem.PTC;
using Prem.PTC.Utils;
using Resources;
using System;

namespace Titan.Cryptocurrencies
{
    [Serializable]
    public class ERC20TokenCryptocurrency : Cryptocurrency
    {
        protected static string CoinmarketcapID = "ERC20Token";
        public override bool Available { get { return AppSettings.TitanModules.HasProduct(7); } }
        public static CryptocurrencyType cryptocurrencyType = CryptocurrencyType.ERC20Token;

        public ERC20TokenCryptocurrency() : base((int)cryptocurrencyType, CoinmarketcapID) { }
        public override CryptocurrencyType CryptocurrencyType { get { return cryptocurrencyType; } }

        public override string GetImageUrl()
        {
            return AppSettings.Ethereum.ERC20TokenImageUrl;
        }

    }
}