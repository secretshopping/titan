using Newtonsoft.Json.Linq;
using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.ServiceModel;
using PaparaServiceReference;

namespace Prem.PTC.Payments
{
    public class PaparaAccount : PaymentAccount<PaparaAccountDetails>
    {
        private string CurrencyCode = AppSettings.Site.CurrencyCode;

        private string EndPointUrl
        {
            get
            {
                return "https://account.papara.com/webapi/EparaSettlementsService/Payment";
            }
        }

        public PaparaAccount(PaparaAccountDetails details)
            : base(details)
        { }

        public override Money Balance
        {
            get
            {
                return Money.Zero;
            }
        }

        public override TransactionResponse CommitTransaction(TransactionRequest request)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    string dataToSend = buildTransactionRequestString(request);
                    byte[] data = Encoding.UTF8.GetBytes(dataToSend);

                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(EndPointUrl);
                    webRequest.Method = "POST";
                    webRequest.ContentLength = data.Length;

                    using (Stream stream = webRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse)webRequest.GetResponse();
                    String responseString = String.Empty;

                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        responseString = reader.ReadToEnd();
                    }

                    response.Close();

                    return new PaparaTransactionResponse(this, responseString);
                }
                catch (WebException ex)
                {
                    ErrorLogger.Log(ex);
                    var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    throw new MsgException(resp);
                }
            }
        }

        private string buildTransactionRequestString(TransactionRequest request)
        {
            return new NVPStringBuilder()
                .Append("ApiName", accountDetails.ApiName)
                .Append("ApiKey", accountDetails.ApiKey)
                .Append("WalletTo", request.PayeeId)
                .Append("Amount1", request.Payment.GetRealTotals())
                .Append("Amount2", (request.Payment - new Money(request.Payment.GetRealTotals())).ToShortClearString().Replace("0.", ""))
                .Append("Description", request.Note)
                .Build();
        }
    }
}