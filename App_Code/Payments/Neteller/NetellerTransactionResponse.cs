using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC.Payments;
using Newtonsoft.Json.Linq;
using Prem.PTC;
using System.IO;
using Newtonsoft.Json;

public class NetellerTransactionResponse : TransactionResponse
{
    internal NetellerTransactionResponse(NetellerAccount usedAccount, JObject jsonResponse, WebException outerException = null)
    {
        UsedAccount = usedAccount;
        Note = string.Empty;

        if (outerException != null)
        {
            IsSuccess = false;
            Note = outerException.Message;
            try
            {
                var resp = new StreamReader(outerException.Response.GetResponseStream()).ReadToEnd();

                dynamic obj = JsonConvert.DeserializeObject(resp);
                Note = obj.error.message;
            }
            catch (Exception ex) { }
        }
        else
        {
            try
            {
                RawResponse = jsonResponse.ToString();

                if (jsonResponse["transaction"]["status"].ToString() == "accepted")
                {
                    IsSuccess = true;
                    ReferenceNumber = jsonResponse["transaction"]["id"].ToString();
                    Note = ReferenceNumber;
                }
                else if (jsonResponse["transaction"]["status"].ToString() == "pending")
                {
                    IsSuccess = true;
                    ReferenceNumber = jsonResponse["transaction"]["id"].ToString();
                    Note = "Status is 'pending'. Probably Neteller account did not exist and the member got the money on email.";
                }
                else
                {
                    IsSuccess = false;
                    Note = "Error sending the payment";
                }
            }
            catch (Exception ex)
            {
                IsSuccess = true;
                Note = ex.Message;
                ErrorLogger.Log(ex);
            }
        }
    }
}

