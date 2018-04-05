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
    public class CoinbaseHandler : PaymentHandler
    {
        public CoinbaseHandler(HttpContext context, PaymentProcessor processor)
            : base(context, processor)
        { }

        public override void ProcessRequest()
        {
            try
            {
                CoinbaseApi api = new CoinbaseApi(AppSettings.Cryptocurrencies.CoinbaseAPIKey, AppSettings.Cryptocurrencies.CoinbaseAPISecret, false);

                if (!IP.IsIpInRange(IP.Current, "54.175.255.192/27"))
                    throw new MsgException("Bad request IP.");

                var json = JObject.Parse(context.Request.GetFromBodyString());

                string merchant = json["account"]["id"].ToString();

                if (String.IsNullOrEmpty(merchant))
                    throw new MsgException("No Account ID passed");

                if (merchant != CoinbaseCryptocurrencyApi.GetAccountId(api))
                    throw new MsgException("Invalid account ID.");

                //IPN process
                string operationType = json["type"].ToString();

                if (AppSettings.Cryptocurrencies.IsCoinbaseMerchant)
                {
                    string transactionId = json["data"]["transaction"]["id"].ToString();
                    string currency = json["data"]["amount"]["currency"].ToString();
                    string amount = json["data"]["amount"]["amount"].ToString();
                    string args = json["data"]["metadata"]["all"].ToString();
                    string[] argsSplit = args.Split(ButtonGenerationStrategy.ArgsDelimeter);

                    CheckIfNotDoneYet(transactionId);

                    if (operationType == "wallet:orders:paid" || operationType == "wallet:orders:mispaid")
                    {
                        string commandName = json["data"]["description"].ToString();
                        string buyerCurrency = json["data"]["bitcoin_amount"]["currency"].ToString();
                        string buyerCurrencyAmount = json["data"]["bitcoin_amount"]["amount"].ToString();
                        string cryptocurrencyInfo = string.Format("{0}{1}", buyerCurrencyAmount, buyerCurrency);

                        var assembly = Assembly.GetAssembly(typeof(IIpnHandler));
                        var type = assembly.GetType(commandName, true, true);
                        var command = Activator.CreateInstance(type) as IIpnHandler;
                        var Cryptocurrency = CryptocurrencyFactory.Get(buyerCurrency);

                        //Wallet deposit, sent in BTC
                        if (command is WalletDepositCryptocurrencyIpnHandler && Cryptocurrency.DepositTarget == DepositTargetBalance.Wallet
                            && currency == "BTC" && AppSettings.Site.CurrencyCode != "BTC")
                            Cryptocurrency.TryDepositCryptocurrency(new Member(argsSplit[0]), Decimal.Parse(amount), transactionId, cryptocurrencyInfo);

                        CheckCurrency(currency);
                        command.HandleCoinbase(args, transactionId, amount, cryptocurrencyInfo);
                    }
                }
                else
                {
                    string transactionId = json["additional_data"]["transaction"]["id"].ToString();
                    string currency = json["additional_data"]["amount"]["currency"].ToString();
                    string amount = json["additional_data"]["amount"]["amount"].ToString();
                    string currencyInfo = string.Format("{0}{1}", amount, currency);
                    string address = json["data"]["address"].ToString();

                    CheckIfNotDoneYet(transactionId);

                    if (currency != "BTC")
                        throw new MsgException("This is not BTC.");

                    if (operationType == "wallet:addresses:new-payment")
                    {
                        var bitcoinAmount = Convert.ToDecimal(amount);
                        var TargetAddress = json["data"]["address"].ToString();
                        var Cryptocurrency = CryptocurrencyFactory.Get(currency);

                        Cryptocurrency.TryDepositCryptocurrency(CryptocurrencyAPIProvider.Coinbase, address, bitcoinAmount, transactionId, currencyInfo);
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