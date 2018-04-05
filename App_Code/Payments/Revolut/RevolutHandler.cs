using System;
using System.Web;
using ExtensionMethods;
using Newtonsoft.Json.Linq;

namespace Prem.PTC.Payments
{
    public class RevolutHandler : PaymentHandler
    {
        public RevolutHandler(HttpContext context, PaymentProcessor processor) : base(context, processor) { }
        
        public override void ProcessRequest()
        {
            var strRequest = context.Request.GetFromBodyString();
            var request = JObject.Parse(strRequest);
           
            try
            {
                //Check IPs
                //CheckIP("192.241.213.216");

                //Check duplicated transactions
                //CheckIfNotDoneYet(TransactionID);

                //Check if we are the merchant
                //CheckMerchant(MPesaSapamaAccountDetails.Exists(Convert.ToInt32(Merchant)));

                //Amount from KES to USD
                //Money money = Money.Parse(Amount); //in KES
                //var ConvertedMoney = CurrencyExchangeHelper.FromCalculate(money, "KES");

                //All OK, let's proceed
                //MPesaSapamaCode.Create(ConvertedMoney, Code, PhoneNumber);

                context.Response.Write("OK");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                context.Response.Write("ERROR");
            } 
        }
    }
}