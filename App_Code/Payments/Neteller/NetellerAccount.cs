using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

/// <summary>
/// Summary description for NetellerAccount
/// </summary>
/// 

namespace Prem.PTC.Payments
{
    public class NetellerAccount : PaymentAccount<NetellerAccountDetails>
    {
        private const bool IsSandboxMode = false;
        private string CurrencyCode = AppSettings.Site.CurrencyCode;

        private string EndPointUrl
        {
            get
            {
                const string EndPointApiUrl = "https://api.neteller.com";
                const string EndPointSandboxApiUrl = "https://test.api.neteller.com";

                return IsSandboxMode ? EndPointSandboxApiUrl : EndPointApiUrl;
            }
        }

        public NetellerAccount(NetellerAccountDetails details)
            : base(details)
        { }

        public override Money Balance
        {

            get
            {
                return Money.Zero; //Operation not supported by Neteller API
            }
        }

        public string GetToken()
        {
            string EndUrl = String.Format("{0}/v1/oauth2/token?grant_type=client_credentials", EndPointUrl);
            string valuesToCreateToken = String.Format("{0}:{1}", accountDetails.ClientId, accountDetails.ClientSecret);
            string PreAccessToken = HashingManager.Base64Encode(valuesToCreateToken);

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Authorization", "Basic " + PreAccessToken);
                client.Headers.Add("Content-Type", "application/json");
                JObject TokenJson = JObject.Parse(client.UploadString(EndUrl, ""));
                return TokenJson["accessToken"].ToString();
            }
        }

        public string CreateOrder(string name, Money amount, string command, string args)
        {
            string PaymentUrl = String.Format("{0}/v1/orders", EndPointUrl);
            string merchantRefId = Titan.OfferwallParser.MD5(DateTime.Now.ToString() + args);
            JObject PaymentJson = null;

            using (WebClient client = new WebClient())
            {

                JObject DataRequest = new JObject(
                        new JProperty("order",
                           new JObject(
                              new JProperty("merchantRefId", merchantRefId),
                              new JProperty("totalAmount", amount.ToShortClearString().Replace(".", "")),
                              new JProperty("currency", this.CurrencyCode),
                              new JProperty("lang", "en_US"),
                              new JProperty("items", new JArray( 
                                  new JObject(
                                    new JProperty("amount", amount.ToShortClearString().Replace(".", "")),
                                    new JProperty("quantity", 1),
                                    new JProperty("sku", command.Replace("Prem.PTC.Payments.", "")),
                                    new JProperty("name", name.Substring(0, Math.Min(name.Length, 14))),
                                    new JProperty("description", args)

                                  ))),
                              new JProperty("redirects", new JArray(

                              new JObject(
                                  new JProperty("rel", "on_success"),
                                  new JProperty("uri", ButtonGenerationStrategy.SUCCESS_URL)),
                              new JObject(
                                  new JProperty("rel", "on_cancel"),
                                  new JProperty("uri", ButtonGenerationStrategy.FAILURE_URL))

                                  )))));

                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(DataRequest.ToString());

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PaymentUrl);
                    request.Headers.Add("Authorization", " Bearer " + GetToken());
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.ContentLength = data.Length;

                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse)request.GetResponse();
                    String responseString = String.Empty;

                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        responseString = reader.ReadToEnd();
                    }

                    PaymentJson = JObject.Parse(responseString);
                    response.Close();

                    string url = PaymentJson["links"][0]["url"].ToString();
                    string orderId = PaymentJson["orderId"].ToString();

                    return url;
                }
                catch (WebException ex)
                {
                    ErrorLogger.Log(ex);
                    var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    throw new MsgException(resp);
                }

                return String.Empty;
            }

        }


        public override TransactionResponse CommitTransaction(TransactionRequest Request)
        {
            string PaymentUrl = String.Format("{0}/v1/transferOut", EndPointUrl);
            string merchantRefId = Titan.OfferwallParser.MD5(DateTime.Now.ToString() + Request.PayeeId);
            JObject PaymentJson = null;

            using (WebClient client = new WebClient())
            {

                JObject DataRequest = new JObject
                    (
                        new JProperty("payeeProfile",
                        new JObject(
                           new JProperty("email", Request.PayeeId))),
                        new JProperty("transaction",
                                new JObject(new JProperty("amount", Request.Payment.ToShortClearString().Replace(".", "")), new JProperty("currency", this.CurrencyCode),
                                        new JProperty("merchantRefId", merchantRefId))));

                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(DataRequest.ToString());

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PaymentUrl);
                    request.Headers.Add("Authorization", " Bearer " + GetToken());
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.ContentLength = data.Length;

                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse)request.GetResponse();
                    String responseString = String.Empty;

                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        responseString = reader.ReadToEnd();
                    }

                    PaymentJson = JObject.Parse(responseString);
                    response.Close();
                    return new NetellerTransactionResponse(this, PaymentJson);
                }
                catch (WebException ex)
                {
                    return new NetellerTransactionResponse(this, PaymentJson, ex);
                }
            }
        }
    }

}