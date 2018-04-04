using System;
using System.Web;
using System.Reflection;
using ExtensionMethods;
using LocalBitcoins;
using Newtonsoft.Json.Linq;

namespace Prem.PTC.Payments
{
    public class LocalBitcoinsHandler : PaymentHandler
    {
        private string creditingAllowState = "PAID_AND_CONFIRMED";
        private string Amount, Currency, State, Id, CustomFields, CommandName, Args;
        private const char joinSymbol = LocalBitcoinsButtonGenerationStrategy.joinSymbol;

        public LocalBitcoinsHandler(HttpContext context, PaymentProcessor processor)
            : base(context, processor)
        { }

        public override void ProcessRequest()
        {
            string strRequest = context.Request.GetFromBodyString();
           
            //Set variables
            dynamic json = JObject.Parse(strRequest);
            Amount = json.data.invoice.amount;
            Currency = json.data.invoice.currency;
            State = json.data.invoice.state;
            Id = json.data.invoice.id;
            CustomFields = json.data.invoice.description;

            try
            {
                //Parse Args & CommandName
                var values = HashingManager.Base64Decode(CustomFields);
                var splitedValues = values.Split('#');
                CommandName = splitedValues[0];
                Args = splitedValues[1];

                //Check IPs
                //CheckIP();

                //Check security hash
                //CheckIncomeHash();

                //Check duplicated transactions
                CheckIfNotDoneYet(Id);

                //Check if we are the merchant
                //CheckMerchant(LocalBitcoinsAccountDetails.Exists(WalletId));

                //Check currency
                CheckCurrency(Currency);

                //Check status
                //CheckStatus(Status, "completed");

                if (!CheckInvoice())
                    return;

                //All OK, let's proceed
                Assembly assembly = Assembly.GetAssembly(typeof(IIpnHandler));
                var type = assembly.GetType(CommandName, true, true);
                IIpnHandler command = Activator.CreateInstance(type) as IIpnHandler;

                command.HandleLocalBitcoins(Args, Id, Amount);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                context.Response.Write(Id + "|error");
            }            
        }

        private bool CheckInvoice()
        {
            var gateway = PaymentAccountDetails.GetFirstIncomeGateway<LocalBitcoinsAccountDetails>();
            var clientAPI = new LocalBitcoinsAPI(gateway.APIKey, gateway.APISecret);
            var json = clientAPI.GetInvoiceInformations(Id);

            return json.data.invoice.state == creditingAllowState;
        }
    }
} 