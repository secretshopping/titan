using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace Titan.Cryptocurrencies
{
    public class CryptocurrencyApiFactory
    {
        public static Type[] GetArrayOfSupportedTypes()
        {
            return new Type[] {
                typeof(BlocktrailCryptocurrencyApi),
                typeof(CoinbaseCryptocurrencyApi),
                typeof(CoinPaymentsCryptocurrencyApi),
                typeof(ParityCryptocurrencyApi)
            };
        }

        public static CryptocurrencyApi Get(CryptocurrencyAPIProvider aPIProvider)
        {
            var cache = new CryptocurrenciesApiCache();
            var dictionary = (Dictionary<CryptocurrencyAPIProvider, CryptocurrencyApi>)cache.Get();
            return dictionary[aPIProvider];
        }

        public static CryptocurrencyApi GetDepositApi(Cryptocurrency cryptocurrency)
        {
            return Get((CryptocurrencyAPIProvider)cryptocurrency.DepositApiProcessor);
        }

        public static CryptocurrencyApi GetWithdrawalApi(Cryptocurrency cryptocurrency)
        {
            return Get((CryptocurrencyAPIProvider)cryptocurrency.WithdrawalApiProcessor);
        }

        public static List<CryptocurrencyApi> GetEnabledApisForDeposits(CryptocurrencyType cryptocurrencyType)
        {
            return GetEnabledApis(cryptocurrencyType, "DepositsAvailable");
        }

        public static List<CryptocurrencyApi> GetEnabledApisForWithdrawals(CryptocurrencyType cryptocurrencyType)
        {
            return GetEnabledApis(cryptocurrencyType, "WithdrawalsAvailable");
        }

        private static List<CryptocurrencyApi> GetEnabledApis(CryptocurrencyType cryptocurrencyType, string columnToCheck)
        {
            var query = String.Format("SELECT TypeInt FROM CryptocurrencyApis WHERE Enabled = 1 AND TypeInt IN " +
                "(SELECT TypeInt FROM CryptocurrencyApiOperations WHERE {1} = 1 AND CryptocurrencyTypeId = {0})", (int)cryptocurrencyType, columnToCheck);
            var list = TableHelper.GetStringListFromRawQuery(query);

            var result = new List<CryptocurrencyApi>();
            foreach (var api in list)
                result.Add(CryptocurrencyApiFactory.Get((CryptocurrencyAPIProvider)Convert.ToInt32(api)));

            return result;
        }

        private static List<CryptocurrencyApi> GetAllApis()
        {
            var cache = new CryptocurrenciesApiCache();
            var dictionary = (Dictionary<CryptocurrencyAPIProvider, CryptocurrencyApi>)cache.Get();
            return dictionary.Values.ToList();
        }
    }
}