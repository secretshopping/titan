using Newtonsoft.Json.Linq;
using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using Titan.ICO;

public class CoinmarketcapHelper
{
    public static Money GetCurrentExchangeRate(string exchangeCodeId, string currencyCode)
    {
        //Usually we takie it right from the API
        //However it differs for ERC20 Token Because we don't have its value in the API

        #region Token value

        if (exchangeCodeId == "ERC20Token")
        {
            if (AppSettings.Site.CurrencyIsTokenCryptocurrency)
                return new Money(1);
            else
                return AppSettings.Ethereum.ERC20TokenRate;
        }

        //We have token cryptocurrency as our main website currency and we want to fetch other official cryptocurrency value
        //We need to get token value to BTC/USD/whatever and then lookup the value in the API
        if (currencyCode == AppSettings.Site.CurrencyCode && AppSettings.Site.CurrencyIsTokenCryptocurrency)
        {
            if (AppSettings.Payments.TokenCryptocurrencyValueType == TokenCryptocurrencyValue.Static)
            {
                if (AppSettings.Ethereum.ERC20TokenRate.IsZero)
                    return Money.Zero;

                //We have static value in USD (1 TOKEN = X USD)
                Money OneCryptoInUSD = CoinmarketcapHelper.GetCurrentExchangeRateFromAPI(exchangeCodeId, "USD");
                return new Money(OneCryptoInUSD.ToDecimal() / AppSettings.Ethereum.ERC20TokenRate.ToDecimal());
            }
            else if (AppSettings.Payments.TokenCryptocurrencyValueType == TokenCryptocurrencyValue.DynamicFromInternalExchange)
            {
                //We take value from Internal Exchange
                decimal OneStockValue = InternalExchangeTransaction.GetLastStockValue();
                if ((AppSettings.InternalExchange.InternalExchangeStockType == BalanceType.MainBalance ||
                    AppSettings.InternalExchange.InternalExchangeStockType == BalanceType.CashBalance) &&
                    AppSettings.InternalExchange.InternalExchangePurchaseVia == BalanceType.BTC)
                {
                    if (OneStockValue == 0)
                        return Money.Zero;

                    //Stock = our token currency
                    //Purchase via BTC
                    Money OneCryptoInBTC = CoinmarketcapHelper.GetCurrentExchangeRateFromAPI(exchangeCodeId, "BTC");
                    return new Money(OneCryptoInBTC.ToDecimal() / OneStockValue);
                }
            }

            return new Money(1);
        }

        #endregion

        return GetCurrentExchangeRateFromAPI(exchangeCodeId, currencyCode);
    }

    public static Money GetCurrentExchangeRateFromAPI(string exchangeCodeId, string currencyCode)
    {
        Money result = Money.Zero;

        try
        {
            using (WebClient client = new WebClient())
            {
                string data = client.DownloadJsonString(
                    String.Format("https://api.coinmarketcap.com/v1/ticker/{0}/?convert={1}", exchangeCodeId, currencyCode));

                return Money.Parse(Convert.ToString(JObject.Parse(data.Trim('[', ']'))[string.Format("price_{0}", currencyCode.ToLower())]));
            }
        }
        catch (Exception ex)
        { }

        return result;
    }
}