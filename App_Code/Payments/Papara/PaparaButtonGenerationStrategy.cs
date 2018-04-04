using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PaparaServiceReference;

namespace Prem.PTC.Payments
{
    public class PaparaButtonGenerationStrategy : ButtonGenerationStrategy
    {
        private const int ItemNameMaxLength = 255;
        private const int MaxArgsCount = 5;

        protected PaparaAccountDetails account;

        public PaparaButtonGenerationStrategy(PaparaAccountDetails papara)
        {
            account = papara;
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
            string url = String.Empty;

            var shooppingVoucher = new ShoppingVoucherEntity();
            shooppingVoucher.Amount = amount.ToDecimal();
            shooppingVoucher.ProductName = itemName;
            shooppingVoucher.CategoryName = HashingManager.Base64Encode(command);
            shooppingVoucher.TotalAmount = amount.ToDecimal();
            shooppingVoucher.Quantity = 1;

            int orderId = PaparaOrder.Create(amount, command, String.Join(ButtonGenerationStrategy.ArgsDelimeter.ToString(), args));

            var client = new ApiRequestSoapClient();
            var result = client.TransactionRequest(account.ApiName, account.ApiKey, account.Username, orderId.ToString(), amount.ToDecimal(), Money.Zero.ToDecimal(),
                new ShoppingVoucherEntity[1] { shooppingVoucher }, AppSettings.Site.Url + "Handlers/Payment/Papara.ashx", 2);
            client.Close();

            if (result.ResultStatus && !String.IsNullOrEmpty(result.ResultObject))
                url = result.ResultObject;

            return url;
        }

        public override string ToString()
        {
            return "Papara";
        }

    }
}