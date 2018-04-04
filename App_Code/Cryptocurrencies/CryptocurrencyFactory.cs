using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Titan.Cryptocurrencies
{
    public class CryptocurrencyFactory
    {
        public static Type[] GetArrayOfSupportedTypes()
        {
            return new Type[] {
                typeof(BitcoinCryptocurrency),
                typeof(RippleCryptocurrency),
                typeof(EthereumCryptocurrency),
                typeof(ERC20TokenCryptocurrency) };
        }

        public static Cryptocurrency Get(CryptocurrencyType cryptocurrencyType)
        {
            var cache = new CryptocurrenciesCache();
            var dictionary = (Dictionary<CryptocurrencyType, Cryptocurrency>)cache.Get();

            //ERCFreezed is basically ERC20Token when it comes to token properties
            if (cryptocurrencyType == CryptocurrencyType.ERCFreezed)
                cryptocurrencyType = CryptocurrencyType.ERC20Token;

            return dictionary[cryptocurrencyType];
        }

        public static Cryptocurrency Get<T>() where T : Cryptocurrency
        {
            Type cryptocurrencyInterface = typeof(T);
            FieldInfo prop = cryptocurrencyInterface.GetField("cryptocurrencyType");
            var cryptocurrencyType = (CryptocurrencyType)prop.GetValue(null);
            return Get(cryptocurrencyType);
        }

        public static Cryptocurrency Get(string cryptocurrencyCode)
        {
            return Get(GetCryptocurrencyType(cryptocurrencyCode));
        }

        public static List<Cryptocurrency> GetAllAvailable()
        {
            var cache = new CryptocurrenciesCache();
            var dictionary = (Dictionary<CryptocurrencyType, Cryptocurrency>)cache.Get();
            return dictionary.Values.Where(elem => elem.Available).ToList();
        }

        public static CryptocurrencyType GetCryptocurrencyType(string cryptocurrencyCode)
        {
            CryptocurrencyType result = CryptocurrencyType.ERC20Token;
            try
            {
                result = (CryptocurrencyType)Enum.Parse(typeof(CryptocurrencyType), cryptocurrencyCode);
            }
            catch (Exception ex) { }
            return result;
        }
    }
}