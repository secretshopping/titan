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
    public class MPesaHandler : PaymentHandler
    {
        string TransactionID, ProductName, SentHash, Amount, Value, Status, Args, Category, Currency, CommandName;

        public MPesaHandler(HttpContext context, PaymentProcessor processor)
            : base(context, processor)
        { }

        public override void ProcessRequest()
        {
            string strRequest = context.Request.GetFromBodyString();
            JObject request = JObject.Parse(strRequest);

            //Set variables

            Category = request["category"].ToString();
            ProductName = request["productName"].ToString();
            Value = request["value"].ToString();
            Status = request["status"].ToString();
            var Metadata = request["requestMetadata"];

            CommandName = Metadata["command"].ToString();
            Args = Metadata["args"].ToString();
            Amount = Value.Split(' ')[1];
            Currency = Value.Split(' ')[0];

            if (Category == "MobileCheckout")
            {
                TransactionID = request["providerRefId"].ToString();

                try
                {
                    //Check IPs
                    //CheckIP();

                    //Check duplicated transactions
                    CheckIfNotDoneYet(TransactionID);

                    //Check if we are the merchant
                    CheckMerchant(MPesaAccountDetails.Exists(ProductName));

                    //Check currency
                    CheckCurrency(Currency);

                    //Check status
                    CheckStatus(Status, "Success");

                    //All OK, let's proceed
                    Assembly assembly = Assembly.GetAssembly(typeof(IIpnHandler));
                    var type = assembly.GetType(CommandName, true, true);
                    IIpnHandler command = Activator.CreateInstance(type) as IIpnHandler;

                    command.HandleMPesa(Args, TransactionID, Amount);

                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                }
            }
        }
    }

}