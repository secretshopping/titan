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

namespace Prem.PTC.Payments
{
    public class PayeerHandler : PaymentHandler
    {
        string TransactionID, PaymentMethod, SentHash, Amount, MerchantAccount, Status, Args, OrderID, Currency, CommandName;

        public PayeerHandler(HttpContext context, PaymentProcessor processor)
            : base(context, processor)
        { }

        public override void ProcessRequest()
        {
            //Set variables
            TransactionID = context.Request["m_operation_id"];
            OrderID = context.Request["m_orderid"];
            PaymentMethod = context.Request["m_operation_ps"];
            SentHash = context.Request["m_sign"];
            Amount = context.Request["m_amount"];
            Currency = context.Request["m_curr"];
            MerchantAccount = context.Request["m_shop"];
            Status = context.Request["m_status"];
            Args = context.Request["m_desc"];

            try
            {
                //Parse Args
                Args = HashingManager.Base64Decode(Args);
                Args = Args.Substring(Args.IndexOf("**") + 2);
                CommandName = Args.Substring(0, Args.IndexOf("#"));
                Args = Args.Substring(Args.IndexOf("#") + 1);

                //Check IPs
                CheckIP("185.71.65.92,185.71.65.189,149.202.17.210");

                //Check security hash
                CheckIncomeHash();

                //Check duplicated transactions
                CheckIfNotDoneYet(TransactionID);

                //Check if we are the merchant
                CheckMerchant(PayeerAccountDetails.Exists(MerchantAccount));

                //Check currency
                CheckCurrency(Currency);

                //Check status
                CheckStatus(Status, "success");

                //All OK, let's proceed
                Assembly assembly = Assembly.GetAssembly(typeof(IIpnHandler));
                var type = assembly.GetType(CommandName, true, true);
                IIpnHandler command = Activator.CreateInstance(type) as IIpnHandler;

                command.HandlePayeer(Args, TransactionID, Amount);

                context.Response.Write(OrderID + "|success");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                context.Response.Write(OrderID + "|error");
            }
        }

        #region Misc

        protected void CheckIncomeHash()
        {
            List<PayeerAccountDetails> gateWays = TableHelper.SelectRows<PayeerAccountDetails>(TableHelper.MakeDictionary("IsActive", true));

            bool isOk = false;

            foreach (var gateway in gateWays)
            {
                try
                {
                    string toHash = TransactionID + ":" + PaymentMethod + ":" + context.Request["m_operation_date"] + ":" + context.Request["m_operation_pay_date"]
                    + ":" + MerchantAccount + ":" + OrderID + ":" + Amount + ":" + Currency + ":" + Args + ":" + Status + ":"
                    + gateway.SecretKey;

                    string hash = HashingManager.SHA256(toHash).ToUpper();

                    CheckHash(SentHash, hash);
                    isOk = true;
                }
                catch (Exception ex) { }
            }

            //if (!isOk)
            //    throw new MsgException("Bad income hash");
        }

        #endregion Misc
    }

}