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
    public class SolidTrustPayAccount : PaymentAccount<SolidTrustPayAccountDetails>
    {
        private string CurrencyCode = AppSettings.Site.CurrencyCode;
        public SolidTrustPayAccount(SolidTrustPayAccountDetails details)
            : base(details)
        { }

        private string paymentApi
        {
            get
            {
                return "https://solidtrustpay.com/accapi/process.php";
            }
        }

        public override Money Balance
        {
            get
            {
                return new Money(10000); //GetBalance API is not supported by SolidTrustPay
            }
        }

        public override TransactionResponse CommitTransaction(TransactionRequest request)
        {
            string dataToSend = buildTransactionRequestString(request);

            var req = (HttpWebRequest)WebRequest.Create(paymentApi);

            req.Method = "POST";
            req.ContentLength = dataToSend.Length;
            req.ContentType = "application/x-www-form-urlencoded";

            var streamWriter = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
            streamWriter.Write(dataToSend);
            streamWriter.Close();

            NameValuePairs content = null;
            string result = String.Empty;
            using (var streamReader = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
                content = NameValuePairs.Parse(result);
            }

            return new SolidTrustPayTransactionResponse(this, content, result);
        }

        private string buildTransactionRequestString(TransactionRequest request)
        {
            return new NVPStringBuilder()
                .Append("api_id", accountDetails.ApiUsername)
                .Append("api_pwd", HashingManager.GenerateMD5(accountDetails.ApiPassword + "s+E_a*"))
                .Append("currency", CurrencyCode)
                .Append("item_id", AppSettings.Payments.TransactionNote)
                .Append("amount", request.Payment.ToShortClearString())
                .Append("user", request.PayeeId)
                .Build();
        }
    }
}