using Coinbase;
using Prem.PTC;
using Prem.PTC.Payments;
using System;

public class CoinbaseButtonGenerationStrategy : ButtonGenerationStrategy
{
    protected override string generate(string itemName, Money amount, string command, object[] args)
    {
        CoinbaseApi api = new CoinbaseApi(AppSettings.Cryptocurrencies.CoinbaseAPIKey, AppSettings.Cryptocurrencies.CoinbaseAPISecret, false);

        string description = String.Join(ArgsDelimeter + "", args);

        string AmountString = amount.ToClearString();
        string CurrencyCode = AppSettings.Site.CurrencyCode;

        if (amount is CryptocurrencyMoney)
        {
            //We have direct BTC -> BTC Wallet transfer
            AmountString = ((CryptocurrencyMoney)amount).ToClearString();
            CurrencyCode = ((CryptocurrencyMoney)amount).cryptocurrencyType.ToString();
        }

        var options = new
        {
            amount = AmountString,
            currency = CurrencyCode,
            name = itemName,
            description = command,
            customer_defined_amount = false,
            success_url = SUCCESS_URL,
            cancel_url = FAILURE_URL,
            //notifications_url = AppSettings.Site.Url + "Handlers/Bitcoins/Coinbase.ashx",
            collect_email = false,
            collect_country = false,
            metadata = new { all = description }
        };

        string data = string.Empty;

        try
        {
            var request = api.SendRequest(String.Format("/checkouts"),
                options, RestSharp.Method.POST);

            if (request.Errors != null)
            {
                ErrorLogger.Log(request.Errors[0].Message);
                return String.Empty;
            }

            data = request.Data["embed_code"].ToString();
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }

        return data;
    }

    public override string ToString()
    {
        return "Coinbase";
    }
}