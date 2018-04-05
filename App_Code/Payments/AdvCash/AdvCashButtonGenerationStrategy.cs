using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{
    public class AdvCashButtonGenerationStrategy : ButtonGenerationStrategy
    {
        private string Handler { get { return "https://wallet.advcash.com/sci/"; } }

        private const int ItemNameMaxLength = 255;
        private const int MaxArgsCount = 5;

        protected AdvCashAccountDetails account;

        public AdvCashButtonGenerationStrategy(AdvCashAccountDetails advcash)
        {
            account = advcash;
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

            string signature = String.Format("{0}:{1}:{2}:{3}:{4}:{5}", account.AccountEmail,
                account.SCIName, amount.ToShortClearString(), AppSettings.Site.CurrencyCode,
                AdvCashAccount.APIPassword, orderId);

            signature = HashingManager.SHA256(signature).ToLower();

            NVPStringBuilder nvps = new NVPStringBuilder();

            nvps.Append("ac_account_email", account.AccountEmail)
                .Append("ac_sci_name", account.SCIName)
                .Append("ac_comments", itemName.Substring(0, Math.Min(itemName.Length, ItemNameMaxLength)))
                .Append("ac_amount", amount.ToShortClearString())
                .Append("ac_currency", AppSettings.Site.CurrencyCode)

                .Append("ac_status_url", AppSettings.Site.Url + "Handlers/Payment/AdvCash.ashx")
                .Append("ac_success_url ", ButtonGenerationStrategy.SUCCESS_URL)
                .Append("ac_fail_url", ButtonGenerationStrategy.FAILURE_URL)

                .Append("ac_sign", signature)
                .Append("ac_order_id", orderId)
                .Append("acm_command", command)
                .Append("acm_args", String.Join(ArgsDelimeter + "", args));

            return Handler + '?' + nvps.Build();
        }

        public override string ToString()
        {
            return "AdvCash";
        }

    }
}