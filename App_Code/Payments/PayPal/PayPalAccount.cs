using Newtonsoft.Json.Linq;
using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using PayPal.Api;

namespace Prem.PTC.Payments
{
    public class PayPalAccount : PaymentAccount<PayPalAccountDetails>
    {
        private const bool IsSandboxMode = false;
        private string CurrencyCode = AppSettings.Site.CurrencyCode;

        #region API URLs

        private string getBalanceApi
        {
            get
            {
                const string GetBalanceApiUrl = "https://api-3t.paypal.com/nvp";
                const string GetBalanceSandboxApiUrl = "https://api-3t.sandbox.paypal.com/nvp";

                return IsSandboxMode ? GetBalanceSandboxApiUrl : GetBalanceApiUrl;
            }
        }

        private string singlePaymentApi
        {
            get
            {
                const string SinglePaymentApiUrl = " https://svcs.paypal.com/AdaptivePayments/Pay";
                const string SinglePaymentSandboxApiUrl = "https://svcs.sandbox.paypal.com/AdaptivePayments/Pay";

                return IsSandboxMode ? SinglePaymentSandboxApiUrl : SinglePaymentApiUrl;
            }
        }

        private string singlePaymentApiMassPay
        {
            get
            {
                const string SinglePaymentApiUrl = "https://api-3t.paypal.com/nvp";
                const string SinglePaymentSandboxApiUrl = "https://api-3t.sandbox.paypal.com/nvp";

                return IsSandboxMode ? SinglePaymentSandboxApiUrl : SinglePaymentApiUrl;
            }
        }

        private string paypalApi
        {
            get
            {
                const string payoutApi = "https://api.paypal.com";
                const string payoutApiSandbox = "https://api.sandbox.paypal.com";

                return IsSandboxMode ? payoutApi : payoutApiSandbox;
            }
        }

        #endregion


        public PayPalAccount(PayPalAccountDetails details)
            : base(details) { }

        public override Money Balance
        {
            get
            {
                string dataToSend = buildBalanceRequestString();

                var request = new NVPWebRequest(getBalanceApi);
                request.SendRequest(dataToSend);
                var response = new NVPWebResponse(request.Response).Content;

                for (int i = 0; ; ++i)
                {
                    string currencyCode = string.Empty;

                    if (!response.TryGetValue("L_CURRENCYCODE" + i, out currencyCode))
                        return Money.Zero;

                    if (CurrencyCode.Equals(currencyCode, StringComparison.OrdinalIgnoreCase))
                        return Money.Parse(response["L_AMT" + i]);
                }
            }
        }

        private string buildBalanceRequestString()
        {
            return new NVPStringBuilder()
                .Append("USER", accountDetails.ApiUsername)
                .Append("PWD", accountDetails.ApiPassword)
                .Append("SIGNATURE", accountDetails.ApiSignature)
                .Append("METHOD", "GetBalance")
                .Append("VERSION", "51.0")
                .Append("RETURNALLCURRENCIES", 1)
                .Build();
        }


        public override TransactionResponse CommitTransaction(TransactionRequest request)
        {
            if (accountDetails.IsMassPayEnabled)
                return CommitTransactionMassPay(request);

            if (accountDetails.UsesDepreciatedApi)
                return CommitTransactionNormal(request);

            return CommitTransactionUsingLatestApi(request);
        }

        private TransactionResponse CommitTransactionUsingLatestApi(TransactionRequest request)
        {
            Random random = new Random();

            int batchID1 = 1000000 + random.Next(1, 1000000);
            int batchID2 = 1000000 + random.Next(1, 1000000);

            var config = new Dictionary<string, string>();
            config["mode"] = "live";
            config["clientId"] = accountDetails.ClientID;
            config["clientSecret"] = accountDetails.ClientSecret;

            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);

            var payout = new Payout
            {
                sender_batch_header = new PayoutSenderBatchHeader
                {
                    sender_batch_id = batchID1.ToString(),
                    email_subject = AppSettings.Payments.TransactionNote,
                    recipient_type = PayoutRecipientType.EMAIL
                },
                items = new List<PayoutItem>() {new PayoutItem
                {
                    recipient_type = PayoutRecipientType.EMAIL,
                    amount = new Currency
                    {
                        value = request.Payment.ToShortClearString(),
                        currency = AppSettings.Site.CurrencyCode
                    },
                    note = AppSettings.Payments.TransactionNote,
                    sender_item_id = batchID2.ToString(),
                    receiver = request.PayeeId
                }}
            };

            var result = payout.Create(apiContext);

            string PayoutBatchID = result.batch_header.payout_batch_id;

            return new PayPalTransactionResponse(this, Payout.Get(apiContext, PayoutBatchID));
        }

        private TransactionResponse CommitTransactionNormal(TransactionRequest request)
        {
            string dataToSend = buildTransactionRequestString(request);

            var req = (HttpWebRequest)WebRequest.Create(singlePaymentApi);

            req.Headers.Add("X-PAYPAL-SECURITY-USERID", accountDetails.ApiUsername);
            req.Headers.Add("X-PAYPAL-SECURITY-PASSWORD", accountDetails.ApiPassword);
            req.Headers.Add("X-PAYPAL-SECURITY-SIGNATURE", accountDetails.ApiSignature);
            req.Headers.Add("X-PAYPAL-APPLICATION-ID", accountDetails.AppId);
            req.Headers.Add("X-PAYPAL-REQUEST-DATA-FORMAT", "NV");
            req.Headers.Add("X-PAYPAL-RESPONSE-DATA-FORMAT", "NV");
            req.Headers.Add("X-PAYPAL-DEVICE-IPADDRESS", "");
            req.Headers.Add("X-PAYPAL-REQUEST-SOURCE", "merchant-php-sdk-2.0.96");


            req.Method = "POST";
            req.ContentLength = dataToSend.Length;
            req.ContentType = "application/x-www-form-urlencoded";

            var streamWriter = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
            streamWriter.Write(dataToSend);
            streamWriter.Close();

            NameValuePairs content = null;
            using (var streamReader = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                content = NameValuePairs.Parse(result);
            }

            return new PayPalTransactionResponse(this, content);
        }

        private TransactionResponse CommitTransactionMassPay(TransactionRequest request)
        {
            string dataToSend = buildTransactionRequestStringForMassPay(request);

            var webRequest = new NVPWebRequest(singlePaymentApiMassPay);
            webRequest.SendRequest(dataToSend, Encoding.ASCII);

            var response = new NVPWebResponse(webRequest.Response);

            return new PayPalTransactionResponseForMassPay(this, response.Content);
        }


        private string buildTransactionRequestString(TransactionRequest request)
        {
            // NOTE: returnUrl, cancelUrl:
            // URL to redirect the sender's browser to after the sender
            // has logged into PayPal and approved a payment; it is always 
            // required but only used if a payment requires explicit approval 

            return new NVPStringBuilder()
                .Append("actionType", "PAY")
                .Append("cancelUrl", AppSettings.Site.Url)
                .Append("currencyCode", CurrencyCode)
                .Append("returnUrl", AppSettings.Site.Url)
                .Append("requestEnvelope.errorLanguage", "en_US")
                .Append("receiverList.receiver(0).amount", request.Payment.ToShortClearString())
                .Append("receiverList.receiver(0).email", request.PayeeId)
                .Append("senderEmail", accountDetails.Username)
                .Build();
        }

        private string buildTransactionRequestStringForMassPay(TransactionRequest request)
        {
            // NOTE: returnUrl, cancelUrl:
            // URL to redirect the sender's browser to after the sender
            // has logged into PayPal and approved a payment; it is always 
            // required but only used if a payment requires explicit approval 

            return new NVPStringBuilder()
                     .Append("USER", accountDetails.ApiUsername)
                     .Append("PWD", accountDetails.ApiPassword)
                     .Append("SIGNATURE", accountDetails.ApiSignature)
                     .Append("METHOD", "MassPay")
                     .Append("VERSION", "2.3")
                     .Append("CURRENCYCODE", CurrencyCode)
                     .Append("RECEIVERTYPE", "EmailAddress")
                     .Append("L_EMAIL0", request.PayeeId)
                     .Append("L_AMT0", request.Payment.ToShortClearString())
                     .Append("memo", request.Note)
                     .Build();
        }

    }
}