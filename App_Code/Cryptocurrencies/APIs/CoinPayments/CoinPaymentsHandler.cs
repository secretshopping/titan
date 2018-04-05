using System;
using System.Web;
using System.Reflection;
using ExtensionMethods;
using System.IO;
using Prem.PTC.Members;
using Titan.Cryptocurrencies;

namespace Prem.PTC.Payments
{
    public class CoinPaymentsHandler : PaymentHandler
    {
        public CoinPaymentsHandler(HttpContext context, PaymentProcessor processor)
            : base(context, processor)
        { }

        public override void ProcessRequest()
        {
            string strRequest = context.Request.GetFromBodyString();

            var jsonString = string.Empty;
            var operationType = string.Empty;

            try
            {
                HttpContext.Current.Request.InputStream.Position = 0;
                using (StreamReader inputStream = new StreamReader(HttpContext.Current.Request.InputStream))
                    jsonString = inputStream.ReadToEnd();

                var o = HttpContext.Current.Request.Params;
                var merchant_id = AppSettings.Cryptocurrencies.CoinPaymentsYourMerchantID;
                var secret = AppSettings.Cryptocurrencies.CoinPaymentsIPNSecret;
                var hmac = HttpContext.Current.Request.ServerVariables["HTTP_HMAC"];
                var merchant = o["merchant"].ToString();
                var myhmac = HashingManager.SHA512HMAC(secret, jsonString);
                var transactionId = o["txn_id"];
                var originalCurrency = o["currency1"]; //The original currency/coin submitted in your button. Note: Make sure you check this, a malicious user could have changed it manually.
                var args = o["invoice"];
                

                if (string.IsNullOrEmpty(hmac))
                    throw new MsgException("No HMAC signature sent");

                if (string.IsNullOrEmpty(jsonString))
                    throw new MsgException("Error reading POST data");

                if (string.IsNullOrEmpty(merchant))
                    throw new MsgException("No Merchant ID passed");

                if (merchant != merchant_id)
                    throw new MsgException("Invalid Merchant ID");

                if (myhmac.ToUpper() != hmac.ToUpper())
                    throw new MsgException("HMAC signature does not match");

                CheckIfNotDoneYet(transactionId);

                //IPN process
                operationType = o["ipn_type"].ToString();

                if (operationType == "simple")
                {
                    var commandName = o["custom"];
                    var assembly = Assembly.GetAssembly(typeof(IIpnHandler));
                    var type = assembly.GetType(commandName, true, true);
                    var command = Activator.CreateInstance(type) as IIpnHandler;
                    var splitedArgs = args.Split(ButtonGenerationStrategy.ArgsDelimeter);
                    var user = new Member(splitedArgs[0]);

                    var amount = o["amount1"]; //The total amount of the payment in your original currency/coin.                    
                    var intStatus = Convert.ToInt32(o["status"]);
                    var status = intStatus >= 100 ? true : false;
                    var buyerCurrency = o["currency2"];
                    var buyerAmount = o["amount2"];
                    var currencyInfo = string.Format("{0}: {1}", buyerCurrency, buyerAmount);

                    //Wallet deposit
                    if (command is WalletDepositCryptocurrencyIpnHandler && Money.IsCryptoCurrency(originalCurrency) && AppSettings.Site.CurrencyCode != originalCurrency && status)
                    {                       
                        var Cryptocurrency = CryptocurrencyFactory.Get(originalCurrency);
                        
                        if (Cryptocurrency.DepositTarget == DepositTargetBalance.Wallet)
                            Cryptocurrency.TryDepositCryptocurrency(user, Decimal.Parse(amount), transactionId, currencyInfo);
                    }

                    if (status)
                    {
                        CheckCurrency(originalCurrency);
                        command.HandleCoinPayments(args, transactionId, amount, currencyInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }
    }
}