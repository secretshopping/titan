using AfricasTalking;
using Prem.PTC.Members;
using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{
    public class MPesaSapamaButtonGenerationStrategy : ButtonGenerationStrategy
    {
        protected MPesaSapamaAccountDetails account;

        public MPesaSapamaButtonGenerationStrategy(MPesaSapamaAccountDetails mPesa)
        {
            account = mPesa;
        }

        /// <summary>
        /// Creates some general payza button
        /// </summary>
        /// <param name="itemName">Name describing the item or service. Max Length: 50 characters</param>
        /// <param name="amount">The price or cost of the product or service. The value for 
        /// amount must be positive. Null or negative numbers are not allowed.</param>
        /// <param name="command">Describes how handler should handle IPN request. 
        /// Stored in field apc_1. Max Length: 100 characters</param>
        /// <param name="args">
        /// Custom values you can pass along with the payment button. 
        /// The values are not displayed to the payer on our Pay Process page.
        /// Payza returns these fields back in the IPN.
        /// Used by IIpnHandler ('command')
        /// Stored in fields apc_2 - apc_6. Max Length: 100 characters
        /// </param>
        /// <returns>Http address where member should be redirected after clicking button.</returns>
        protected override string generate(string itemName, Money amount, string command, object[] args)
        {
            //MPesaAccountDetails accountDetails = PaymentAccountDetails.GetFirstIncomeGateway<MPesaAccountDetails>();

            //if (Member.IsLogged)
            //{
            //    AfricasTalkingGateway gateway = new AfricasTalkingGateway("sandbox", "fddf83dcfd24dbc98277f2bc5fd49869728ba372664ada30afe372d40f209ce7", "sandbox");
            //    //AfricasTalkingGateway gateway = new AfricasTalkingGateway(accountDetails.Username, accountDetails.ApiKey);

            //    Dictionary<string, string> metadata = new Dictionary<string, string>();
            //    metadata.Add("itemName", itemName);
            //    metadata.Add("command", command);
            //    metadata.Add("args", String.Join(ButtonGenerationStrategy.ArgsDelimeter.ToString(), args));

            //    var reply = gateway.initiateMobilePaymentCheckout(accountDetails.ProductName, Member.CurrentInCache.GetPaymentAddress(PaymentProcessor.MPesa), AppSettings.Site.CurrencyCode,
            //        amount.ToDecimal(), metadata);
            //}

            string url = String.Empty;
            return url;
        }

        public override string ToString()
        {
            return "MPesaAgent ";
        }

    }
}