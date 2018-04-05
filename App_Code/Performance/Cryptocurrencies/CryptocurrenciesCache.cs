using Prem.PTC.Members;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Web.Caching;
using Titan.Cryptocurrencies;

class CryptocurrenciesCache : CacheBase
{
    protected override string Name { get { return "CryptocurrenciesCache"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromSeconds(30); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.High; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        Dictionary<CryptocurrencyType, Cryptocurrency> data = new Dictionary<CryptocurrencyType, Cryptocurrency>();

        foreach (Type cryptocurrencyType in CryptocurrencyFactory.GetArrayOfSupportedTypes())
        {
            Cryptocurrency cryptocurrency =  (Cryptocurrency)Activator.CreateInstance(cryptocurrencyType);
            data.Add(cryptocurrency.CryptocurrencyType, cryptocurrency);
        }

        return data;
    }
}