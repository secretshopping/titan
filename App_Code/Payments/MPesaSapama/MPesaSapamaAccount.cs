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
using Prem.PTC.Members;

namespace Prem.PTC.Payments
{
    public class MPesaSapamaAccount : PaymentAccount<MPesaSapamaAccountDetails>
    {
        public MPesaSapamaAccount(MPesaSapamaAccountDetails details)
            : base(details)
        { }

        public override Money Balance
        {
            get
            {
                return Money.Zero; //No such method in the API
            }
        }

        public override TransactionResponse CommitTransaction(TransactionRequest request)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    string queryString = String.Empty;
                    string dataToSend = buildTransactionRequestString(request, ref queryString);

                    var webRequest = new NVPWebRequest("http://sapamacash.com/api/b2c?" + queryString);
                    webRequest.SendRequest(dataToSend);
                    var responseString = new NVPWebResponse(webRequest.Response).RawContent;

                    return new MPesaSapamaTransactionResponse(this, JObject.Parse(responseString));
                }
                catch (WebException ex)
                {
                    ErrorLogger.Log(ex);
                    var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    throw new MsgException(resp);
                }
            }
        }

        private string buildTransactionRequestString(TransactionRequest request, ref string queryString)
        {
            string transactionID = HashingManager.GenerateMD5(DateTime.Now + request.MemberName + AppSettings.Offerwalls.UniversalHandlerPassword);

            var arrayToHash = new string[] { accountDetails.APIKey, accountDetails.APISecret, "json", accountDetails.LocationId.ToString(), "live" };
            var DataToHash = String.Join(".", arrayToHash);
            var user = new Member(request.MemberName);

            string hash = HashingManager.SHA256(DataToHash).ToLower();

            //We need to convert money from our currency to KES
            var moneyToBeSent = CurrencyExchangeHelper.Calculate(request.Payment, "KES");

            queryString = String.Format("format=json&api_key={0}&hash={1}&location_id={2}&mode=live",
                accountDetails.APIKey, hash, accountDetails.LocationId.ToString());

            return new NVPStringBuilder()
                .Append("format", "json")
                .Append("api_key", accountDetails.APIKey)
                .Append("hash", hash)
                .Append("location_id", accountDetails.LocationId)
                .Append("mode", "live")
                .Append("beneficiaries[0][gateway]", "mpesa")
                .Append("beneficiaries[0][phone]", request.PayeeId)
                .Append("beneficiaries[0][trans_reference]", transactionID)
                .Append("beneficiaries[0][description]", request.Note)
                .Append("beneficiaries[0][first_name]", user.FirstName)
                .Append("beneficiaries[0][last_name]", user.SecondName)
                .Append("beneficiaries[0][amount]", moneyToBeSent.ToShortClearString())
                .Append("beneficiaries[0][callback_url]", AppSettings.Site.Url + "Handlers/Payment/MPesaSapamaX.ashx")
                .Append("beneficiaries[0][callback_http_method]", "GET")
                .Append("beneficiaries[0][message]", HttpUtility.UrlEncode(AppSettings.Payments.TransactionNote))
                .Append("beneficiaries[0][short_code]", accountDetails.Username)
                .Append("beneficiaries[0][currency]", "KES")
                .Build();
        }

    }
}