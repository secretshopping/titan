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

namespace Prem.PTC.Payments
{
    public class PaparaHandler : PaymentHandler
    {
        string TransactionID, PaymentMethod, SentHash, Amount, WalletId, Status, Args, OrderID, Currency, CommandName;

        public PaparaHandler(HttpContext context, PaymentProcessor processor)
            : base(context, processor)
        { }

        public override void ProcessRequest()
        {
            string pre1 = context.Request["pre1"];
            string pre2 = context.Request["pre2"];
            pre1 = HashingManager.Base64Decode(pre1);
            string orderId = pre1.Split('+')[0];
            string statusCode = pre1.Split('+')[1];

            string key = orderId + "+" + statusCode;
            key = HashingManager.Base64Encode(key);
            string md5String = HashingManager.GenerateMD5(key + PaparaAccountDetails.GetSecretKey());

            //Papara transaction OK check
            if (md5String == pre2)
            {
                //Set variables
                OrderID = orderId;

                try
                {
                    PaparaOrder Order = PaparaOrder.Get(Convert.ToInt32(OrderID));

                    Args = Order.Args;
                    TransactionID = OrderID;
                    Amount = Order.Amount.ToShortClearString();
                    CommandName = Order.CommandName;

                    //Check duplicated transactions
                    CheckIfNotDoneYet(OrderID);

                    //All OK, let's proceed
                    Assembly assembly = Assembly.GetAssembly(typeof(IIpnHandler));
                    var type = assembly.GetType(CommandName, true, true);
                    IIpnHandler command = Activator.CreateInstance(type) as IIpnHandler;

                    command.HandlePapara(Args, TransactionID, Amount);

                    Order.IsPaid = true;
                    Order.Save();

                    context.Response.Redirect(ButtonGenerationStrategy.SUCCESS_URL);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                    context.Response.Redirect(ButtonGenerationStrategy.FAILURE_URL);
                }
            }
            else
            {
                context.Response.Redirect(ButtonGenerationStrategy.FAILURE_URL);
            }
        }
    }

}