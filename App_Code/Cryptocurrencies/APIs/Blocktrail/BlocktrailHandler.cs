using System;
using System.Web;
using System.Reflection;
using ExtensionMethods;
using System.IO;
using Prem.PTC.Members;
using Coinbase;
using Newtonsoft.Json.Linq;
using System.Text;
using Prem.PTC.Utils;
using Titan.Cryptocurrencies;

namespace Prem.PTC.Payments
{
    public class BlocktrailHandler : PaymentHandler
    {
        public BlocktrailHandler(HttpContext context, PaymentProcessor processor)
            : base(context, processor)
        { }

        public override void ProcessRequest()
        {
            try
            {
                var originalJson = JObject.Parse(context.Request.GetFromBodyString());

                string transactionHash = originalJson["data"]["hash"].ToString();
                string operationType = originalJson["event_type"].ToString();
                string address = ((JProperty)originalJson["addresses"].First).Name.ToString();
                string currency = originalJson["network"].ToString();
                int retryCount = Int32.Parse(originalJson["retry_count"].ToString());

                //Get the info about TX from Blocktrail anyway
                BlocktrailAPI api = new BlocktrailAPI(AppSettings.Cryptocurrencies.BlocktrailAPIKey, AppSettings.Cryptocurrencies.BlocktrailAPIKeySecret);
                var json = api.GetTransactionInformation(transactionHash);

                //Get the value
                var transactionFound = false;
                var amountInSatoshis = "0";
                var outputs = (JArray)json["outputs"];

                foreach (var output in outputs)
                    if (output["address"].ToString() == address)
                    {
                        amountInSatoshis = output["value"].ToString();
                        transactionFound = true;
                    }

                if (!transactionFound)
                    throw new MsgException("We can't find such transaction.");

                if (currency != "BTC")
                    throw new MsgException("This is not BTC.");

                if (retryCount > 0)
                    throw new MsgException("Retry count = " + retryCount);

                var confirmations = Convert.ToInt32(json["confirmations"].ToString());
                var IsDepositOperation = (operationType == "address-transactions");
                var bitcoinAmountInSatoshis = Convert.ToInt64(amountInSatoshis);
                var amountInCryptocurrency = bitcoinAmountInSatoshis / 100000000;
                var currencyInfo = string.Format("{0}{1}", amountInCryptocurrency, currency);

                var Cryptocurrency = CryptocurrencyFactory.Get(currency);

                //End of API fetch data operations, now proceeding with our methods
                if (IsDepositOperation)
                    Cryptocurrency.TryDepositCryptocurrency(CryptocurrencyAPIProvider.Blocktrail, address, amountInCryptocurrency, transactionHash, currencyInfo, confirmations);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }
    }
}