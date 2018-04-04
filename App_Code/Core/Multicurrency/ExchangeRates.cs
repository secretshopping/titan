using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public abstract class ExchangeRates
{
    /// <summary>
    /// Returns current from currency rate. E.g. 1from = Xto
    /// </summary>
    /// <param name="currency"></param>
    /// <returns></returns>
    public abstract Money GetRate(string from, string to);

    public Money GetRate(string currency)
    {
        Money rate = new Money(1);

        try
        {
            return GetRate(AppSettings.Site.CurrencyCode, currency);
        }
        catch (Exception ex)
        { }

        return rate;
    }
}