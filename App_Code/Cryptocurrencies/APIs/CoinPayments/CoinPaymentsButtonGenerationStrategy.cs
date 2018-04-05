using Prem.PTC;
using Prem.PTC.Payments;
using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Specialized;


public class CoinPaymentsButtonGenerationStrategy : ButtonGenerationStrategy
{
    private string HandlerUrl = "https://www.coinpayments.net/index.php";
    private string Handler { get { return HandlerUrl; } }

    protected override string generate(string itemName, Money amount, string command, object[] args)
    {
        string AmountString = amount.ToClearString();
        string CurrencyCode = AppSettings.Site.CurrencyCode;

        if (amount is CryptocurrencyMoney)
        {
            //We have direct BTC -> BTC Wallet transfer
            AmountString = ((CryptocurrencyMoney)amount).ToClearString();
            CurrencyCode = ((CryptocurrencyMoney)amount).cryptocurrencyType.ToString();
        }

        NVPStringBuilder nvps = new NVPStringBuilder();

        nvps.Append("cmd", "_pay_simple")
            .Append("reset", "1")
            .Append("merchant", AppSettings.Cryptocurrencies.CoinPaymentsYourMerchantID)
            .Append("currency", CurrencyCode)
            .Append("amountf", AmountString)
            .Append("item_name", itemName)
            .Append("invoice", String.Join(ArgsDelimeter + "", args)) //invoice	- This is a passthru variable for your own use. [not visible to buyer]
            .Append("custom", command) //custom - This is a 2nd passthru variable for your own use. [not visible to buyer]
            .Append("success_url", SUCCESS_URL)
            .Append("cancel_url", FAILURE_URL);
            //.Append("ipn_url", AppSettings.Site.Url + "Handlers/CoinPayments.ashx") //If not set or blank defaults to the IPN URL in your settings.

        return Handler + '?' + nvps.Build();
    }

    public override string ToString()
    {
        return "CoinPayments";
    }

    public static string GenerateInputLiteral(NameValueCollection nvc)
    {
        string temp = "";
        foreach (string s in nvc.Keys)
            temp += "<input type=\"hidden\" name=\"" + s + "\" value=\"" + nvc[s] + "\">";
        return temp;
    }
}
