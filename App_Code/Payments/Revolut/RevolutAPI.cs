using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prem.PTC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

public class RevolutAPI
{
    public const string BaseApiUrl = "https://sandbox-b2b.revolut.com/api/1.0";
    private string APIKey;
    private List<AccountInfo> AccountsList;
    private List<string> CounterpartiesIds;

    public class AccountInfo
    {
        public string Id;
        public string Name;
        public decimal Balance;
        public string CurrencyCode; //	3-letter ISO currency code
        public string State; // the account state, one of active, inactive
        public bool Public; // determines if the account is visible to other Businesses on Revolut	B
        public DateTime CreationDate;
        public DateTime UpdatedDate;
        public string Type; // the type of the account, one of pocket, beneficiary
    }

    public RevolutAPI(string APIKey)
    {
        this.APIKey = APIKey;
        AccountsList = GetAccounts();
        DeleteAllCounterparties();
    }

    public dynamic CreatePayment(string username, string phoneNumber, decimal amount)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                var url = CreateURL("pay");
                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                var responseString = string.Empty;
                //requestId - max 40 chars
                var requestId = HashingManager.Base64Encode(AppSettings.ServerTime.TimeOfDay.ToString());
                var counterpartyId = AddCounterparty(username, phoneNumber);
                var description = "test";
                var handlerUrl = string.Format("{0}Handlers/Revolut.ashx", AppSettings.Site.Url);

                dynamic receiverInfo = new JObject
                {
                    { "counterparty_id", counterpartyId },
                    { "account_id", GetAccount().Id }
                };

                var dataToSend = new JObject
                {
                    { "request_id", requestId },
                    { "account_id",  GetAccount().Id }, // the ID of the account to pay from
                    { "receiver", receiverInfo },
                    { "amount", amount },
                    { "currency", AppSettings.Site.CurrencyCode },
                    { "description", description }
                    //{ "webhook", handlerUrl }
                };

                ErrorLogger.Log(dataToSend.ToString());

                webRequest.Method = "POST";
                webRequest.Headers.Add("Authorization", " Bearer " + APIKey);

                using (var stream = new StreamWriter(webRequest.GetRequestStream()))
                {
                    stream.Write(dataToSend);
                }

                using (var response = (HttpWebResponse)webRequest.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    responseString = reader.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<dynamic>(responseString);
            }
            catch (WebException ex)
            {
                ErrorLogger.Log(ex);
                var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                throw new MsgException(resp);
            }
        }
    }


    /// <summary>
    /// profile_type == personal => provide phone
    /// profile_type == business => provide email
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="email"></param>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public string AddCounterparty(string userName, string phoneNumber)
    {
        string counterpartyId = null;

        if (!phoneNumber.StartsWith("+"))
            phoneNumber = string.Format("+{0}", phoneNumber);

        using (WebClient client = new WebClient())
        {
            try
            {
                var url = CreateURL("counterparty");
                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                var responseString = string.Empty;
                var dataToSend = new JObject
                {
                    { "name", userName },
                    { "profile_type", "personal" },
                    { "phone", phoneNumber },
                };

                ErrorLogger.Log(dataToSend.ToString());

                webRequest.Method = "POST";
                webRequest.Headers.Add("Authorization", " Bearer " + APIKey);

                using (var stream = new StreamWriter(webRequest.GetRequestStream()))
                {
                    stream.Write(dataToSend);
                }

                using (var response = (HttpWebResponse)webRequest.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    responseString = reader.ReadToEnd();
                }

                var json = JsonConvert.DeserializeObject<dynamic>(responseString);

                //throw new MsgException(responseString);
                counterpartyId = json.id;
            }
            catch (WebException ex)
            {
                ErrorLogger.Log(ex);
                var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                throw new MsgException(resp);
            }
        }

        return counterpartyId;
    }

    public AccountInfo GetAccount(string currencyCode = null)
    {
        if (currencyCode == null)
            currencyCode = AppSettings.Site.CurrencyCode;

        return AccountsList.FindAll(x => x.CurrencyCode == currencyCode).OrderByDescending(x => x.Balance).First();
    }

    private List<AccountInfo> GetAccounts()
    {
        var list = new List<AccountInfo>();

        using (WebClient client = new WebClient())
        {
            try
            {
                var url = CreateURL("accounts");
                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                var responseString = string.Empty;

                webRequest.Method = "GET";
                webRequest.Headers.Add("Authorization", " Bearer " + APIKey);

                using (var response = (HttpWebResponse)webRequest.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    responseString = reader.ReadToEnd();
                }

                var json = JsonConvert.DeserializeObject<dynamic>(responseString);

                foreach (var item in json)
                {
                    var accountInfo = new AccountInfo
                    {
                        Id = item.id,
                        Name = item.name,
                        Balance = item.balance,
                        CurrencyCode = item.currency,
                        State = item.state,
                        Public = item.@public,
                        CreationDate = item.created_at,
                        UpdatedDate = item.updated_at,
                        Type = item.type
                    };
                    list.Add(accountInfo);
                }
            }
            catch (WebException ex)
            {
                ErrorLogger.Log(ex);
                var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                throw new MsgException(resp);
            }
        }

        return list;
    }

    private void DeleteAllCounterparties()
    {
        CounterpartiesIds = GetCounterparties();

        using (WebClient client = new WebClient())
        {
            try
            {
                foreach (var counterpartyId in CounterpartiesIds)
                {
                    var url = CreateURL("counterparty", counterpartyId);
                    var webRequest = (HttpWebRequest)WebRequest.Create(url);
                    var responseString = string.Empty;

                    webRequest.Method = "DELETE";
                    webRequest.Headers.Add("Authorization", " Bearer " + APIKey);

                    using (var response = (HttpWebResponse)webRequest.GetResponse())
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        responseString = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                ErrorLogger.Log(ex);
                var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                throw new MsgException(resp);
            }
        }
    }

    private List<string> GetCounterparties()
    {
        var list = new List<string>();

        using (WebClient client = new WebClient())
        {
            try
            {
                var url = CreateURL("counterparties");
                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                var responseString = string.Empty;

                webRequest.Method = "GET";
                webRequest.Headers.Add("Authorization", " Bearer " + APIKey);

                using (HttpWebResponse responseX = (HttpWebResponse)webRequest.GetResponse())
                using (Stream stream = responseX.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseString = reader.ReadToEnd();
                }

                var json = JsonConvert.DeserializeObject<dynamic>(responseString);

                foreach (var item in json)
                    list.Add(item.id.ToString());
            }
            catch (WebException ex)
            {
                ErrorLogger.Log(ex);
                var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                throw new MsgException(resp);
            }
        }
        return list;
    }

    private string CreateURL(string request, string parameter = null)
    {
        if (parameter == null)
            return string.Format("{0}/{1}", BaseApiUrl, request);

        return string.Format("{0}/{1}/{2}", BaseApiUrl, request, parameter);
    }
}