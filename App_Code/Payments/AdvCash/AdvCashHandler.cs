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
    public class AdvCashHandler : PaymentHandler
    {
        string TransactionID, PaymentMethod, SentHash, Amount, MerchantAccount, Status, Args, OrderID, Currency, CommandName;

        public AdvCashHandler(HttpContext context, PaymentProcessor processor)
            : base(context, processor)
        { }

        public override void ProcessRequest()
        {
            //Set variables
            TransactionID = context.Request.QueryString["ac_transfer"];
            OrderID = context.Request.QueryString["ac_order_id"];
            PaymentMethod = context.Request.QueryString["ac_ps"];
            SentHash = context.Request.QueryString["ac_hash"];
            Amount = context.Request.QueryString["ac_merchant_amount"];
            Currency = context.Request.QueryString["ac_merchant_currency"];
            MerchantAccount = context.Request.QueryString["ac_dest_wallet"];
            Status = context.Request.QueryString["ac_transaction_status"];
            Args = context.Request.QueryString["acm_args"];
            CommandName = context.Request.QueryString["acm_command"];

            try
            {
                //Check IPs
                //

                //Check security hash
                CheckIncomeHash();

                //Check duplicated transactions
                CheckIfNotDoneYet(TransactionID);

                //Check if we are the merchant
                CheckMerchant(AdvCashAccountDetails.Exists(MerchantAccount.Replace(" ", "")));

                //Check currency
                CheckCurrency(Currency);

                //Check status
                CheckStatus(Status, "COMPLETED");

                //All OK, let's proceed
                Assembly assembly = Assembly.GetAssembly(typeof(IIpnHandler));
                var type = assembly.GetType(CommandName, true, true);
                IIpnHandler command = Activator.CreateInstance(type) as IIpnHandler;

                command.HandleAdvCash(Args, TransactionID, Amount);

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
            string toHash = TransactionID + ":" + context.Request.QueryString["ac_start_date"] + ":" + context.Request.QueryString["ac_sci_name"] +
                ":" + context.Request.QueryString["ac_src_wallet"]
            + ":" + MerchantAccount + ":" + OrderID + ":" + context.Request.QueryString["ac_amount"] + ":" + Currency + ":" + AdvCashAccount.APIPassword;

            string hash = HashingManager.SHA256(toHash);

            CheckHash(SentHash, hash);
        }

        #endregion Misc
    }

}