using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class ExchangeRatesFactory
{
    public static ExchangeRates Get()
    {  
        if (AppSettings.Payments.MultiCurrencyProvider == MultiCurrencyProvider.OpenExchangeRates) 
            return new OpenExchangeRates(AppSettings.Payments.ExchangeRateAppIDCode);
        else
            return new FixerExchangeRates();
    }
}