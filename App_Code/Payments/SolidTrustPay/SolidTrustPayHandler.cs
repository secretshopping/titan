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
    public class SolidTrustPayHandler : PaymentHandler
    {
        string TransactionID, SentHash,  Amount,  PayerAccount,  MerchantAccount,  Status, CommandName, Args;

        public SolidTrustPayHandler(HttpContext context, PaymentProcessor processor)
            : base(context, processor)
        { }

        public override void ProcessRequest()
        {
            if (context.Request["card_transact_status"] != null &&
                context.Request["gateway_result"] != null)
                ProcessCreditCardRequest();
            else
            {
                //Set variables
                TransactionID = context.Request["tr_id"];
                SentHash = context.Request["hash"];
                Amount = context.Request["amount"];
                PayerAccount = context.Request["payerAccount"];
                MerchantAccount = context.Request["merchantAccount"];
                Status = context.Request["status"];
                CommandName = context.Request["user1"];
                Args = context.Request["user2"];

                //Check IPs
                // --> No check available for SolidTrustPay

                //Check security hash
                CheckIncomeHash();

                //Check duplicated transactions
                CheckIfNotDoneYet(TransactionID);

                //Check if we are the merchant
                CheckMerchant(SolidTrustPayAccountDetails.Exists(MerchantAccount));

                //Check currency
                // --> No check available for SolidTrustPay

                //Check status
                CheckStatus(Status, "COMPLETE");

                //All OK, let's proceed
                Assembly assembly = Assembly.GetAssembly(typeof(IIpnHandler));
                var type = assembly.GetType(CommandName, true, true);
                IIpnHandler command = Activator.CreateInstance(type) as IIpnHandler;

                command.HandleSolidTrustPay(Args, TransactionID, Amount);
            }
        }

        private void ProcessCreditCardRequest()
        {
            //Set variables
            TransactionID = context.Request["tr_id"];
            SentHash = context.Request["hash"];
            Amount = context.Request["amount"];
            Status = context.Request["card_transact_status"];
            CommandName = context.Request["udf1"];
            Args = context.Request["udf2"];

            string gatewayResult = context.Request["gateway_result"];

            //Check IPs
            // --> No check available for SolidTrustPay

            //Check security hash
            CheckIncomeHash();

            //Check duplicated transactions
            CheckIfNotDoneYet(TransactionID);

            //Check currency
            // --> No check available for SolidTrustPay

            //Check status
            CheckStatus(Status, "ACCEPTED");

            //All OK, let's proceed
            Assembly assembly = Assembly.GetAssembly(typeof(IIpnHandler));
            var type = assembly.GetType(CommandName, true, true);
            IIpnHandler command = Activator.CreateInstance(type) as IIpnHandler;

            command.HandleSolidTrustPay(Args, TransactionID, Amount);
        }

        #region Misc

        protected bool CheckIncomeHash()
        {
            List<SolidTrustPayAccountDetails> gateWays = TableHelper.SelectRows<SolidTrustPayAccountDetails>(TableHelper.MakeDictionary("IsActive", true));

            bool isOk = false;

            foreach (var gateway in gateWays)
            {
                try
                {
                    string toHash = TransactionID + ":" + HashingManager.GenerateMD5(gateway.PaymentButtonsPassword) + ":" + Amount + ":" +
                                            MerchantAccount + ":" + PayerAccount;

                    CheckHash(SentHash, HashingManager.GenerateMD5(toHash));
                    isOk = true;
                }
                catch (Exception ex) { }
            }

            return isOk;
        }

        #endregion Misc
    }

}