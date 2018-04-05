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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Prem.PTC.Payments
{
    public class NetellerHandler : PaymentHandler
    {
        string TransactionID, PaymentMethod, SentHash, Amount, MerchantAccount, Status, Args, OrderID, Currency, CommandName;

        public NetellerHandler(HttpContext context, PaymentProcessor processor)
            : base(context, processor)
        { }

        public override void ProcessRequest()
        {

            try
            {
                string secretKey = HashingManager.GenerateMD5(AppSettings.Offerwalls.UniversalHandlerPassword + "Neteller");

                string documentContents;
                using (Stream receiveStream = context.Request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }

                JObject obj = JObject.Parse(documentContents);

                //Parse Args
                Status = obj["eventType"].ToString();
                SentHash = obj["key"].ToString();
                TransactionID = obj["id"].ToString();
                //Currency
                //Amount
                //Args?
                //CommandName

                Args = HashingManager.Base64Decode(Args);
                Args = Args.Substring(Args.IndexOf("**") + 2);
                CommandName = "Prem.PTC.Payments." + Args.Substring(0, Args.IndexOf("#"));
                Args = Args.Substring(Args.IndexOf("#") + 1);

                //Check IPs
                //Not available in Neteller

                //Check hash
                CheckHash(secretKey, SentHash);

                //Check duplicated transactions
                CheckIfNotDoneYet(TransactionID);

                //Check if we are the merchant
                //Not available in Neteller

                //Check currency
                CheckCurrency(Currency);

                //Check status
                CheckStatus(Status, "order_payment_succeeded");

                //All OK, let's proceed
                Assembly assembly = Assembly.GetAssembly(typeof(IIpnHandler));
                var type = assembly.GetType(CommandName, true, true);
                IIpnHandler command = Activator.CreateInstance(type) as IIpnHandler;

                command.HandleNeteller(Args, TransactionID, Amount);

                context.Response.Write("OK");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                context.Response.Write("Error");
            }
        }

    }

}