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
    public class OKPayHandler : PaymentHandler
    {
        string TransactionID, PaymentMethod, SentHash, Amount, WalletId, Status, Args, OrderID, Currency, CommandName;

        public OKPayHandler(HttpContext context, PaymentProcessor processor)
            : base(context, processor)
        { }

        public override void ProcessRequest()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://checkout.okpay.com/ipn-verify");

            //Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            string strRequest = context.Request.GetFromBodyString();
            strRequest += "&ok_verify=true";
            req.ContentLength = Encoding.UTF8.GetByteCount(strRequest);

            //Send the request to OKPAY and get the response
            string strResponse = "";
            using (StreamWriter streamOut = new StreamWriter(req.GetRequestStream()))
            {
                streamOut.Write(strRequest);
                streamOut.Close();
                using (StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    strResponse = streamIn.ReadToEnd();
                    streamIn.Close();
                }
            }

            if (strResponse == "VERIFIED")
            {

                //Set variables
                TransactionID = context.Request["ok_ipn_id"];
                OrderID = context.Request["ok_invoice"];
                PaymentMethod = context.Request["ok_txn_payment_method"];
                Amount = context.Request["ok_txn_net"];
                Currency = context.Request["ok_txn_currency"];
                WalletId = context.Request["ok_receiver_wallet"];
                Status = context.Request["ok_txn_status"];
                Args = context.Request["ok_item_1_custom_2_value"];
                CommandName = context.Request["ok_item_1_custom_1_value"];

                try
                {

                    //Parse Args
                    Args = HashingManager.Base64Decode(Args);

                    //Check IPs
                    //CheckIP();

                    //Check security hash
                    //CheckIncomeHash();

                    //Check duplicated transactions
                    CheckIfNotDoneYet(TransactionID);

                    //Check if we are the merchant
                    CheckMerchant(OKPayAccountDetails.Exists(WalletId));

                    //Check currency
                    CheckCurrency(Currency);

                    //Check status
                    CheckStatus(Status, "completed");

                    //All OK, let's proceed
                    Assembly assembly = Assembly.GetAssembly(typeof(IIpnHandler));
                    var type = assembly.GetType(CommandName, true, true);
                    IIpnHandler command = Activator.CreateInstance(type) as IIpnHandler;

                    command.HandleOKPay(Args, TransactionID, Amount);

                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                    context.Response.Write(OrderID + "|error");
                }

            }
            else if (strResponse == "INVALID")
            {
                context.Response.Write(OrderID + "|invalid");
            }
            else
            {
                context.Response.Write(OrderID + "|other");
            }
        }
    }

}