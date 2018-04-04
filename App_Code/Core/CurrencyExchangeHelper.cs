using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls;
using Prem.PTC;
using Titan.Cryptocurrencies;

public static class CurrencyExchangeHelper
{
    public static string CurrencyCacheName { get { return "CurrenciesRatesCache"; } }

    public static Money GetRate(string currencyCode)
    {
        return Calculate(new Money(1), currencyCode);
    }

    public static Money Calculate(Money value, string currencyCode)
    {
        if (currencyCode == AppSettings.Site.CurrencyCode)
            return value;

        var rates = GetRates(currencyCode);

        return value * rates[currencyCode];
    }

    public static Money FromCalculate(Money value, string currencyCode)
    {
        if (currencyCode == AppSettings.Site.CurrencyCode)
            return value;

        var rates = GetRates(currencyCode);

        return value / rates[currencyCode];
    }

    public static decimal TryCalculate(decimal value)
    {
        if (!MulticurrencyHelper.ShowMulticurrency)
            return value;

        return Calculate(new Money(value), MulticurrencyHelper.GetCurrency()).ToDecimal();
    }

    public static Money TryCalculate(Money value)
    {
        if (!MulticurrencyHelper.ShowMulticurrency)
            return value;

        return Calculate(value, MulticurrencyHelper.GetCurrency());
    }

    public static Money TryFromCalculate(Money value)
    {
        if (!MulticurrencyHelper.ShowMulticurrency)
            return value;

        return FromCalculate(value, MulticurrencyHelper.GetCurrency());
    }

    private static decimal GetCurrentRate(string currencyCode)
    {
        if (Money.IsCryptoCurrency(currencyCode))
            return CryptocurrencyFactory.Get(currencyCode).GetOriginalValue(AppSettings.Site.CurrencyCode).ToDecimal(); //1BTC = X USD
        
        return ExchangeRatesFactory.Get().GetRate(currencyCode).ToDecimal(); //1USD = X PLN
    }

    private static Dictionary<string, decimal> GetRates(string currencyCode)
    {
        Dictionary<string, decimal> rates = null;

        if (HttpContext.Current.Cache[CurrencyCacheName] != null)
            rates = (Dictionary<string, decimal>)HttpContext.Current.Cache[CurrencyCacheName];
        else
            rates = new Dictionary<string, decimal>();

        if (!rates.ContainsKey(currencyCode))
        {
            DateTime duration = Money.IsCryptoCurrency(currencyCode) ? DateTime.Now.AddMinutes(3) : DateTime.Now.AddHours(3);

            rates.Add(currencyCode, GetCurrentRate(currencyCode));
            HttpContext.Current.Cache.Insert(CurrencyCacheName, rates, null, duration, Cache.NoSlidingExpiration,
                CacheItemPriority.AboveNormal, null);
        }

        return rates;
    }
}