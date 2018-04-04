using Newtonsoft.Json.Linq;
using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prem.PTC.Payments
{
    public class PaparaTransactionResponse : TransactionResponse
    {
        internal PaparaTransactionResponse(PaparaAccount usedAccount, string rawResponse)
        {
            RawResponse = rawResponse;
            UsedAccount = usedAccount;
            IsSuccess = true;
            Note = String.Empty;

            try
            {
                var responseJson = JObject.Parse(rawResponse);
                Note = responseJson["ResultMessage"].ToString();
                if ((int)responseJson["ResultCode"] != 100)
                    IsSuccess = false;

            }
            catch (Exception ex) { }
            
        }
    }
}