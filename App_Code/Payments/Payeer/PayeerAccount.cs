using Newtonsoft.Json.Linq;
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
    public class PayeerAccount : PaymentAccount<PayeerAccountDetails>
    {
        private string CurrencyCode = AppSettings.Site.CurrencyCode;

        private string apiUrl
        {
            get
            {
                return "https://payeer.com/ajax/api/api.php";
            }
        }

        private string ApiUsername
        {
            get
            {
                if (AppSettings.Side == ScriptSide.Client)
                    return accountDetails.ApiUsername;
                
                return accountDetails.AdminApiUsername;
            }
        }

        public PayeerAccount(PayeerAccountDetails details)
            : base(details) { }

        public override Money Balance
        {
            get
            {
                string dataToSend = buildBalanceRequestString();

                try
                {
                    var request = new NVPWebRequest(apiUrl);
                    request.SendRequest(dataToSend);
                    var webResponse = new NVPWebResponse(request.Response).RawContent;

                    JObject response = JObject.Parse(webResponse);
                    JArray errors = (JArray)response["errors"];

                    if (errors.Count > 0)
                    {
                        //Error
                        ErrorLogger.Log("Payeer API error: " + response["errors"].ToString());
                        return Money.Zero;
                    }

                    return Money.Parse(response["balance"][AppSettings.Site.CurrencyCode]["BUDGET"].ToString());
                }
                catch(Exception ex)
                {
                    ErrorLogger.Log(ex);
                    return Money.Zero;
                }
            }
        }

        private string buildBalanceRequestString()
        {
            return new NVPStringBuilder()
                .Append("account", accountDetails.Username)
                .Append("apiId", ApiUsername)
                .Append("apiPass", accountDetails.ApiPassword)
                .Append("language", "en")
                .Append("action", "balance")
                .Build();
        }

        public override TransactionResponse CommitTransaction(TransactionRequest request)
        {
            try
            {
                string dataToSend = buildTransactionRequestString(request);

                var webRequest = new NVPWebRequest(apiUrl);
                webRequest.SendRequest(dataToSend);

                var webResponse = new NVPWebResponse(webRequest.Response).RawContent;

                JObject response = JObject.Parse(webResponse);                
                return new PayeerTransactionResponse(this, response);
            }
            catch(Exception ex)
            {
                ErrorLogger.Log(ex);
                return new PayeerTransactionResponse(this, null, true);
            }
        }

        private string buildTransactionRequestString(TransactionRequest request)
        {
            return new NVPStringBuilder()
                .Append("account", accountDetails.Username)
                .Append("apiId", ApiUsername)
                .Append("apiPass", accountDetails.ApiPassword)
                .Append("language", "en")
                .Append("action", "transfer")
                .Append("curOut", CurrencyCode)
                .Append("curIn", CurrencyCode)
                .Append("sum", request.Payment.ToShortClearString())
                .Append("to", request.PayeeId)
                .Build();
        }

    }
}