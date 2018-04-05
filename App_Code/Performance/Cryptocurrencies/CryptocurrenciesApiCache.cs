using Prem.PTC.Members;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Web.Caching;
using Titan.Cryptocurrencies;

class CryptocurrenciesApiCache : CacheBase
{
    protected override string Name { get { return "CryptocurrenciesApiCache"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromSeconds(60); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.AboveNormal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        Dictionary<CryptocurrencyAPIProvider, CryptocurrencyApi> data = new Dictionary<CryptocurrencyAPIProvider, CryptocurrencyApi>();

        foreach (Type cryptocurrencyApiType in CryptocurrencyApiFactory.GetArrayOfSupportedTypes())
        {
            CryptocurrencyApi cryptocurrencyApi =  (CryptocurrencyApi)Activator.CreateInstance(cryptocurrencyApiType);
            data.Add(cryptocurrencyApi.ApiType, cryptocurrencyApi);
        }

        return data;
    }
}