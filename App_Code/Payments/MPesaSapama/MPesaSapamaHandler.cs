using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Reflection;
using Prem.PTC.Utils.NVP;
using Prem.PTC.Payments;
using Prem.PTC.Members;
using Prem.PTC;
using System.Net;
using ExtensionMethods;
using Newtonsoft.Json.Linq;

namespace Prem.PTC.Payments
{
    public class MPesaSapamaHandler : PaymentHandler
    {
        string TransactionID, Merchant, SentHash, Amount, Value, Code, Category, PhoneNumber;

        public MPesaSapamaHandler(HttpContext context, PaymentProcessor processor)
            : base(context, processor)
        { }

        public override void ProcessRequest()
        {
            var request = context.Request.QueryString;

            //Set variables
            TransactionID = HttpUtility.UrlDecode(request["proxy_trans_id"]);

            Amount = HttpUtility.UrlDecode(request["trans_amount"]);
            Merchant = HttpUtility.UrlDecode(request["location_id"]);

            PhoneNumber = HttpUtility.UrlDecode(request["phone"]);
            Code = HttpUtility.UrlDecode(request["trans_id"]);

            try
            {
                //Check IPs
                CheckIP("192.241.213.216");

                //Check duplicated transactions
                CheckIfNotDoneYet(TransactionID);

                //Check if we are the merchant
                CheckMerchant(MPesaSapamaAccountDetails.Exists(Convert.ToInt32(Merchant)));

                //Amount from KES to USD
                Money money = Money.Parse(Amount); //in KES
                var ConvertedMoney = CurrencyExchangeHelper.FromCalculate(money, "KES");

                //All OK, let's proceed
                MPesaSapamaCode.Create(ConvertedMoney, Code, PhoneNumber);

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