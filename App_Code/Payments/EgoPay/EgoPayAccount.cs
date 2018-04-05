using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Prem.PTC.Payments
{
    public class EgoPayAccount : PaymentAccount<EgoPayAccountDetails>
    {
        // If true all transactions are commmited with sandbox api url instead of ordirary api url
        private const bool IsSandboxMode = false;
        private string CurrencyCode = AppSettings.Site.CurrencyCode;

        private string getBalanceApi
        {
            get
            {
                const string GetBalanceApiUrl = "https://api-3t.EgoPay.com/nvp";
                const string GetBalanceSandboxApiUrl = "https://api-3t.sandbox.EgoPay.com/nvp";

                return IsSandboxMode ? GetBalanceSandboxApiUrl : GetBalanceApiUrl;
            }
        }

        private string singlePaymentApi
        {
            get
            {
                const string SinglePaymentApiUrl = " https://svcs.EgoPay.com/AdaptivePayments/Pay";
                const string SinglePaymentSandboxApiUrl = "https://svcs.sandbox.EgoPay.com/AdaptivePayments/Pay";

                return IsSandboxMode ? SinglePaymentSandboxApiUrl : SinglePaymentApiUrl;
            }
        }

        private string singlePaymentApiMassPay
        {
            get
            {
                const string SinglePaymentApiUrl = "https://api-3t.EgoPay.com/nvp";
                const string SinglePaymentSandboxApiUrl = "https://api-3t.sandbox.EgoPay.com/nvp";

                return IsSandboxMode ? SinglePaymentSandboxApiUrl : SinglePaymentApiUrl;
            }
        }


        public EgoPayAccount(EgoPayAccountDetails details)
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
            else
                return CommitTransactionNormal(request);         
        }

        private TransactionResponse CommitTransactionNormal(TransactionRequest request)
        {
            string dataToSend = buildTransactionRequestString(request);

            var req = (HttpWebRequest)WebRequest.Create(singlePaymentApi);

            req.Headers.Add("X-EgoPay-SECURITY-USERID", accountDetails.ApiUsername);
            req.Headers.Add("X-EgoPay-SECURITY-PASSWORD", accountDetails.ApiPassword);
            req.Headers.Add("X-EgoPay-SECURITY-SIGNATURE", accountDetails.ApiSignature);
            req.Headers.Add("X-EgoPay-APPLICATION-ID", accountDetails.AppId);
            req.Headers.Add("X-EgoPay-REQUEST-DATA-FORMAT", "NV");
            req.Headers.Add("X-EgoPay-RESPONSE-DATA-FORMAT", "NV");

            req.Headers.Add("X-EgoPay-DEVICE-IPADDRESS", "");
            req.Headers.Add("X-EgoPay-REQUEST-SOURCE", "merchant-php-sdk-2.0.96");

            //req.Headers.Add("X-EgoPay-SANDBOX-EMAIL-ADDRESS", "Platform.sdk.seller@gmail.com");

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

            return new EgoPayTransactionResponse(this, content);
        }

        private TransactionResponse CommitTransactionMassPay(TransactionRequest request)
        {
            string dataToSend = buildTransactionRequestString(request);

            var webRequest = new NVPWebRequest(singlePaymentApiMassPay);
            webRequest.SendRequest(dataToSend, Encoding.ASCII);

            var response = new NVPWebResponse(webRequest.Response);

            return new EgoPayTransactionResponse(this, response.Content);
        }


        private string buildTransactionRequestString(TransactionRequest request)
        {
            // NOTE: returnUrl, cancelUrl:
            // URL to redirect the sender's browser to after the sender
            // has logged into EgoPay and approved a payment; it is always 
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
            // has logged into EgoPay and approved a payment; it is always 
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