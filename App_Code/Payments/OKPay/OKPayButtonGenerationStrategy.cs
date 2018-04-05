using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{
    public class OKPayButtonGenerationStrategy : ButtonGenerationStrategy
    {
        private string Handler { get { return "https://checkout.okpay.com/"; } }

        private const int ItemNameMaxLength = 255;
        private const int MaxArgsCount = 5;

        protected OKPayAccountDetails account;

        public OKPayButtonGenerationStrategy(OKPayAccountDetails okPay)
        {
            account = okPay;
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
            string orderId = HashingManager.GenerateMD5(AppSettings.Site.Name + DateTime.Now).ToLower();

            NVPStringBuilder nvps = new NVPStringBuilder();

            nvps.Append("ok_receiver", account.Username)
                .Append("ok_item_1_name", itemName)
                .Append("ok_item_1_price", amount.ToShortClearString())
                .Append("ok_currency", AppSettings.Site.CurrencyCode)
                .Append("ok_fees", "1")
                .Append("ok_invoice", orderId)
                .Append("ok_item_1_custom_1_title", "command")
                .Append("ok_item_1_custom_1_value", command)
                .Append("ok_item_1_custom_2_title", "args")
                .Append("ok_item_1_custom_2_value", HashingManager.Base64Encode(String.Join(ArgsDelimeter + "", args)));

            return Handler + '?' + nvps.Build();
        }

        public override string ToString()
        {
            return "OKPay";
        }

    }
}