using Prem.PTC.Utils.NVP;
using System;
using System.Text;

namespace Prem.PTC.Payments
{
    public class PayzaAccount : PaymentAccount<PayzaAccountDetails>
    {
        // If true transactions all transactions are fake
        private const bool IsTestMode = false;
        // If true all transactions are commmited with sandbox api url instead of ordirary api url
        private const bool IsSandboxMode = false;

        private string CurrencyCode = AppSettings.Site.CurrencyCode;

        private string getBalanceApi
        {
            get
            {
                const string GetBalanceApiUrl = "https://api.payza.eu/svc/api.svc/GetBalance";
                const string GetBalanceSandboxApiUrl = "https://sandbox.Payza.eu/api/api.svc/GetBalance";

                return IsSandboxMode ? GetBalanceSandboxApiUrl : GetBalanceApiUrl;
            }
        }

        private string singlePaymentApi
        {
            get
            {
                const string SinglePaymentApiUrl = "https://api.payza.eu/svc/api.svc/sendmoney";
                const string SinglePaymentSandboxApiUrl = "https://sandbox.Payza.eu/api/api.svc/sendmoney";

                return IsSandboxMode ? SinglePaymentSandboxApiUrl : SinglePaymentApiUrl;
            }
        }


        public PayzaAccount(PayzaAccountDetails details)
            : base(details) { }


        public override Money Balance
        {
            get
            {
                try
                {
                    string dataToSend = buildBalanceRequestString();

                    var request = new NVPWebRequest(getBalanceApi);
                    request.SendRequest(dataToSend);
                    var response = new NVPWebResponse(request.Response).Content;

                    string balance = "";

                    return response.TryGetValue("AVAILABLEBALANCE_1", out balance) ? Money.Parse(balance) : Money.Zero;
                }
                catch(Exception ex)
                {
                    return Money.Zero;
                }
            }
        }

        private string buildBalanceRequestString()
        {
            return new NVPStringBuilder()
                                    .Append("USER", accountDetails.Username)
                                    .Append("PASSWORD", accountDetails.APIPassword)
                                    .Append("CURRENCY", CurrencyCode)
                                    .Build();
        }


        public override TransactionResponse CommitTransaction(TransactionRequest request)
        {
            string dataToSend = buildTransactionRequestString(request);

            var webRequest = new NVPWebRequest(singlePaymentApi);
            webRequest.SendRequest(dataToSend, Encoding.ASCII);

            NVPWebResponse response;

            try
            {
                response = new NVPWebResponse(webRequest.Response);
            }
            catch(Exception e)
            {
                ErrorLogger.Log(e);

                //502 bad gateway error fix (occuring sometimes)         
                PayzaTransactionResponse successfulResponse = new PayzaTransactionResponse(this);
                return successfulResponse;
            }

            return new PayzaTransactionResponse(this, response.Content);
        }

        private string buildTransactionRequestString(TransactionRequest request)
        {
            const int PurchaseTypeOther = 3;
            string senderEmail = string.IsNullOrEmpty(accountDetails.SenderEmail) ?
                accountDetails.Username : accountDetails.SenderEmail;

            return new NVPStringBuilder()
                        .Append("USER", accountDetails.Username)
                        .Append("PASSWORD", accountDetails.APIPassword)
                        .Append("AMOUNT", request.Payment.ToShortClearString())
                        .Append("CURRENCY", CurrencyCode)
                        .Append("RECEIVEREMAIL", request.PayeeId)
                        .Append("SENDEREMAIL", senderEmail)
                        .Append("PURCHASETYPE", PurchaseTypeOther)
                        .Append("NOTE", AppSettings.Payments.TransactionNote)
                        .Append("TESTMODE", IsTestMode ? 1 : 0)
                        .Build();
        }

    }
}