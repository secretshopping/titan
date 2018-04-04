using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Prem.PTC.Advertising;
using Prem.PTC.Utils.NVP;
using System.Collections.Specialized;
using Titan.Cryptocurrencies;

namespace Prem.PTC.Payments
{
    #region General
    /// <summary>
    /// Encapsulates the way of creation generic payment button
    /// </summary>
    /// <seealso cref="IButtonGenerator"/>
    /// <seealso cref="BaseButtonGenerator"/>
    public abstract class ButtonGenerationStrategy
    {
        public static readonly char ArgsDelimeter = ',';
        public static string SUCCESS_URL { get { return AppSettings.Site.Url + "status.aspx?type=paymentok"; } }
        public static string FAILURE_URL { get { return AppSettings.Site.Url + "status.aspx?type=fail"; } }
        /// <summary>
        /// Generates payment button.
        /// </summary>
        /// <param name="itemName">Name describing the item, displayed to customer in PayPal/Payza</param>
        /// <param name="amount">How much customer must pay.</param>
        /// <param name="handlerCommand">Some subclass of IIpnHandler
        /// - definition of action that will be invoked by IpnHandler</param>
        /// <param name="args">Additional args that will be passed to handler
        /// - for example membername and membershippackId when member buys upgrade</param>
        /// <returns>should return ready to use http link redirecting to Payza/PayPal website 
        /// where user can finish payment. Optionally can return html fragment 
        /// (for example html form) to create buttton.</returns>
        public string Generate(string itemName, Money amount, Type handlerCommand, params object[] args)
        {
            if (!typeof(IIpnHandler).IsAssignableFrom(handlerCommand))
                throw new ArgumentOutOfRangeException("Incorrect command: " + handlerCommand.FullName);

            StringBuilder ProcessorButton = new StringBuilder();
            ProcessorButton.Append(String.Format("<a class=\"btn btn-lg btn-white m-5\" style=\"width: 250px;\" href=\"{0}\">", generate(itemName, amount, handlerCommand.FullName, args)));

            if (TitanFeatures.IsClickmyad && args[2].ToString() == CryptocurrencyAPIProvider.CoinPayments.ToString())
                ProcessorButton.Append(String.Format("<img src=\"{0}{1}{2}.png\" alt=\"Pay\" /></a>", AppSettings.Site.Url, "Images/OneSite/TransferMoney/", string.Format("{0}Button", args[2])));            
            else
            {
                ProcessorButton.Append(String.Format("<img src=\"{0}{1}{2}.png\" alt=\"Pay\" />", AppSettings.Site.Url, "Images/OneSite/TransferMoney/", args[2]));
                ProcessorButton.Append(String.Format("{0}</a>", args[2]));
            }

            return ProcessorButton.ToString();
        }

        protected abstract string generate(string itemName, Money amount, string command, object[] args);
    }
    #endregion General

    #region PayPal

    /// <summary>
    /// Paypal button generation
    /// </summary>
    public class PayPalButtonGenerationStrategy : ButtonGenerationStrategy
    {
        public static readonly char ArgsDelimeter = ',';

        private const bool IsSandboxMode = false;
        private const string HandlerUrl = "https://www.paypal.com/cgi-bin/webscr";
        private const string HandlerSandboxUrl = "https://www.sandbox.paypal.com/cgi-bin/webscr";
        private const int ItemNameMaxLength = 127;

        private string Handler { get { return IsSandboxMode ? HandlerSandboxUrl : HandlerUrl; } }

        protected PayPalAccountDetails account;

        public PayPalButtonGenerationStrategy(PayPalAccountDetails paypal)
        {
            account = paypal;
        }

        /// <summary>
        /// Creates some general paypal button
        /// </summary>
        /// <param name="itemName">Name describing the item or service. Max Length: 127 characters</param>
        /// <param name="amount">The price or cost of the product or service.</param>
        /// <param name="command">Describes how handler should handle IPN request. 
        /// Stored in field custom. Max Length: 100 characters</param>
        /// <param name="args"></param>
        /// <returns>Http address where members should be redirected after clicking button</returns>
        protected override string generate(string itemName, Money amount, string command, object[] args)
        {
            NVPStringBuilder nvps = new NVPStringBuilder();

            nvps.Append("cmd", "_xclick")
                .Append("business", account.Username)
                .Append("item_name", itemName.Substring(0, Math.Min(itemName.Length, ItemNameMaxLength)))
                .Append("custom", command)
                .Append("invoice", String.Join(ArgsDelimeter + "", args))
                .Append("amount", amount.ToShortClearString())
                .Append("currency_code", AppSettings.Site.CurrencyCode)
                .Append("button_subtype", "services")
                .Append("no_shipping", "1")
                .Append("notify_url", AppSettings.Site.Url + "Handlers/PayPal.ashx");

            return Handler + '?' + nvps.Build();
        }

        public override string ToString()
        {
            return "PayPal";
        }
    }

    #endregion PayPal

    #region Payza

    /// <summary>
    /// Creates some general payza button
    /// </summary>
    public class PayzaButtonGenerationStrategy : ButtonGenerationStrategy
    {
        private const bool IsSandboxMode = false;
        private const string HandlerUrl = "https://secure.payza.eu/checkout";
        private const string HandlerSandboxUrl = "https://sandbox.Payza.eu/sandbox/payprocess.aspx";

        private string Handler { get { return IsSandboxMode ? HandlerSandboxUrl : HandlerUrl; } }

        private const int ItemNameMaxLength = 50;
        private const int MaxArgsCount = 6;

        protected PayzaAccountDetails account;

        public PayzaButtonGenerationStrategy(PayzaAccountDetails payza)
        {
            account = payza;
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
            if (args.Length > MaxArgsCount)
                throw new ArgumentException("MaxArgsCount exceeded. Expected max " + MaxArgsCount + ", found " + args.Length);

            NVPStringBuilder nvps = new NVPStringBuilder();

            nvps.Append("ap_merchant", account.Username)
                .Append("ap_purchasetype", "service")
                .Append("ap_itemname", itemName.Substring(0, Math.Min(itemName.Length, ItemNameMaxLength)))
                .Append("ap_amount", amount.ToShortClearString())
                .Append("ap_currency", AppSettings.Site.CurrencyCode)
                .Append("ap_alerturl", AppSettings.Site.Url + "Handlers/Payza.ashx")
                .Append("ap_returnurl", ButtonGenerationStrategy.SUCCESS_URL)
                .Append("ap_cancelurl", ButtonGenerationStrategy.FAILURE_URL)
                .Append("ap_ipnversion", 2)
                .Append("apc_1", command);

            for (int i = 0; i < args.Length; ++i)
                nvps.Append("apc_" + (i + 2), Convert.ToString(args[i]));
            
            return Handler + '?' + nvps.Build();
        }

        public override string ToString()
        {
            return "Payza";
        }

    }

    #endregion Payza

    #region PerfectMoney

    /// <summary>
    /// PerfectMoney button generation
    /// </summary>
    public class PerfectMoneyButtonGenerationStrategy : ButtonGenerationStrategy
    {
        public static readonly char ArgsDelimeter = ',';

        private string HandlerUrl = AppSettings.Site.Url + "status.aspx";
        private const int ItemNameMaxLength = 127;

        private string Handler { get { return HandlerUrl; } }

        protected PerfectMoneyAccountDetails account;

        public PerfectMoneyButtonGenerationStrategy(PerfectMoneyAccountDetails pm)
        {
            account = pm;
        }

        protected override string generate(string itemName, Money amount, string command, object[] args)
        {
            NVPStringBuilder nvps = new NVPStringBuilder();

            nvps.Append("PAYEE_ACCOUNT", account.AccountNumber)
                .Append("PAYEE_NAME", account.DisplayName)
                .Append("PAYMENT_METHOD", "PerfectMoney account")
                .Append("BAGGAGE_FIELDS", "ITEM_NAME ITEM_COMMAND ITEM_ARGS")
                .Append("ITEM_NAME", itemName.Substring(0, Math.Min(itemName.Length, ItemNameMaxLength)))
                .Append("ITEM_COMMAND", command)
                .Append("ITEM_ARGS", String.Join(ArgsDelimeter + "", args))
                .Append("PAYMENT_AMOUNT", amount.ToShortClearString())
                .Append("PAYMENT_UNITS", AppSettings.Site.CurrencyCode)
                .Append("PAYMENT_URL", ButtonGenerationStrategy.SUCCESS_URL)
                .Append("NOPAYMENT_URL", ButtonGenerationStrategy.FAILURE_URL)
                .Append("STATUS_URL", AppSettings.Site.Url + "Handlers/PerfectMoney.ashx")
                .Append("pp", "PerfectMoney"); //Automatic submit on status.aspx

            return Handler + '?' + nvps.Build();
        }

        public override string ToString()
        {
            return "PerfectMoney";
        }

        public static string GenerateInputLiteral(NameValueCollection nvc)
        {
            string temp = "";
            foreach (string s in nvc.Keys)
                temp += "<input type=\"hidden\" name=\"" + s + "\" value=\"" + nvc[s] + "\">";
            return temp;
        }
    }

    #endregion PerfectMoney

    #region Neteller

    public class NetellerButtonGenerationStrategy : ButtonGenerationStrategy
    {
        public static readonly char ArgsDelimeter = ',';

        private const bool IsSandboxMode = false;
        private string HandlerUrl = AppSettings.Site.Url + "status.aspx";
        private const int ItemNameMaxLength = 149;

        private string Handler { get { return HandlerUrl; } }

        protected NetellerAccountDetails account;

        public NetellerButtonGenerationStrategy(NetellerAccountDetails neteller)
        {
            account = neteller;
        }

        protected override string generate(string itemName, Money amount, string command, object[] args)
        {
            NVPStringBuilder nvps = new NVPStringBuilder();

            nvps.Append("netellerItemName", itemName.Substring(0, Math.Min(itemName.Length, ItemNameMaxLength)))
                .Append("netellerAmount", amount.ToShortClearString())
                .Append("netellerCommand", command)
                .Append("netellerArgs", String.Join(ArgsDelimeter + "", args));

            return Handler + '?' + nvps.Build();
        }

        public override string ToString()
        {
            return "Neteller";
        }

        public static void TryInvokeTransfer(NameValueCollection nvc)
        {
            //Create order
            NetellerAccountDetails accountDetails = PaymentAccountDetails.GetFirstIncomeGateway<NetellerAccountDetails>();
            NetellerAccount account = new NetellerAccount(accountDetails);

            string redirectUrl = account.CreateOrder(nvc["netellerItemName"], Money.Parse(nvc["netellerAmount"]), nvc["netellerCommand"], nvc["netellerArgs"]);

            HttpContext.Current.Response.Redirect(redirectUrl);
        }

    }

    #endregion

    #region SolidTrustPay

    /// <summary>
    /// SolidTrustPayButtonGenerationStrategy 
    /// </summary>
    public class SolidTrustPayButtonGenerationStrategy : ButtonGenerationStrategy
    {
        protected SolidTrustPayAccountDetails account;
        public static readonly char ArgsDelimeter = '#';
        public static string Handler { get { return AppSettings.Site.Url + "status.aspx"; } }
        public SolidTrustPayButtonGenerationStrategy(SolidTrustPayAccountDetails pm)
        {
            account = pm;
        }

        protected override string generate(string itemName, Money amount, string command, object[] args)
        {
            NVPStringBuilder nvps = new NVPStringBuilder();

            nvps.Append("merchantAccount", account.Username)
                .Append("sci_name", account.PaymentButtonName)
                .Append("item_id", itemName)
                .Append("amount", amount.ToShortClearString())
                .Append("user1", command)
                .Append("user2", String.Join(ArgsDelimeter.ToString(), args))
                .Append("PAYMENT_AMOUNT", amount.ToShortClearString())
                .Append("PAYMENT_UNITS", AppSettings.Site.CurrencyCode)
                .Append("return_url", ButtonGenerationStrategy.SUCCESS_URL)
                .Append("cancel_url", ButtonGenerationStrategy.FAILURE_URL)
                .Append("notify_url", AppSettings.Site.Url + "Handlers/SolidTrustPay.ashx")
                .Append("pp", "SolidTrustPay"); //Automatic submit on status.aspx

            return Handler + '?' + nvps.Build();
        }

        public override string ToString()
        {
            return "SolidTrustPay";
        }

        public static string GenerateInputLiteral(NameValueCollection nvc)
        {
            string temp = "";
            foreach (string s in nvc.Keys)
                temp += "<input type=\"hidden\" name=\"" + s + "\" value=\"" + nvc[s] + "\">";
            return temp;
        }
    }

    #endregion SolidTrustPay

    #region Payeer

    /// <summary>
    /// PayeerButtonGenerationStrategy 
    /// </summary>
    public class PayeerButtonGenerationStrategy : ButtonGenerationStrategy
    {
        public static readonly char ArgsDelimeter = ',';
        private string HandlerUrl = "https://payeer.com/merchant/";

        protected PayeerAccountDetails account;

        public PayeerButtonGenerationStrategy(PayeerAccountDetails pm)
        {
            account = pm;
        }

        protected override string generate(string itemName, Money amount, string command, object[] args)
        {
            NVPStringBuilder nvps = new NVPStringBuilder();

            string orderId = HashingManager.GenerateMD5(AppSettings.Site.Name + DateTime.Now).ToLower();
            orderId = orderId.Substring(0, Math.Min(orderId.Length, 31));

            string description = command + "#" + String.Join(ArgsDelimeter + "", args);
            description = itemName + " **" + description;

            string toHash = account.MerchantID + ":" + orderId + ":" + amount.ToShortClearString() + ":" + AppSettings.Site.CurrencyCode
                + ":" + HashingManager.Base64Encode(description) + ":" + account.SecretKey;
            string hash = HashingManager.SHA256(toHash).ToUpper();

            nvps.Append("m_shop", account.MerchantID)
                .Append("m_orderid", orderId)
                .Append("m_amount", amount.ToShortClearString())
                .Append("m_curr", AppSettings.Site.CurrencyCode)
                .Append("m_desc", HashingManager.Base64Encode(description))
                .Append("m_sign", hash);

            return HandlerUrl + '?' + nvps.Build();
        }

        public override string ToString()
        {
            return "Payeer";
        }

        public static string GenerateInputLiteral(NameValueCollection nvc)
        {
            string temp = "";
            foreach (string s in nvc.Keys)
                temp += "<input type=\"hidden\" name=\"" + s + "\" value=\"" + nvc[s] + "\">";
            return temp;
        }
    }

    #endregion Payeer
}