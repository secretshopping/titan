using LocalBitcoins;
using Newtonsoft.Json.Linq;
using System;

namespace Prem.PTC.Payments
{
    public class LocalBitcoinsButtonGenerationStrategy : ButtonGenerationStrategy
    {
        public const char joinSymbol = '#';
        protected LocalBitcoinsAccountDetails account;

        public LocalBitcoinsButtonGenerationStrategy(LocalBitcoinsAccountDetails localBitcoins)
        {
            account = localBitcoins;
        }

        protected override string generate(string itemName, Money amount, string command, object[] args)
        {
            //FOR BTC MIN AMOUNT IS 0.001
            try
            {
                var ApiKey = account.APIKey;
                var ApiSecret = account.APISecret;
                var clientAPI = new LocalBitcoinsAPI(ApiKey, ApiSecret);

                var currencyCode = AppSettings.Site.CurrencyCode;
                //command#args
                var customFields = string.Join(ArgsDelimeter.ToString(), args);
                var hashingValues = HashingManager.Base64Encode(command + joinSymbol + customFields);
                decimal finalAmount;
                if (AppSettings.Site.CurrencyCode != "BTC")
                    finalAmount = Convert.ToDecimal(amount.ToShortClearString());
                else
                    finalAmount = Convert.ToDecimal(amount.ToClearString());

                var createdInvoiceDetails = clientAPI.NewInvoice(currencyCode, finalAmount, hashingValues, SUCCESS_URL).ToString();
                var data = JObject.Parse(createdInvoiceDetails);

                return data.data.invoice.url;
            }
            catch (Exception e)
            {
                throw new MsgException(e.Message);
            }
        }

        public override string ToString()
        {
            return "LocalBitcoins";
        }

    }
}