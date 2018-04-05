using Newtonsoft.Json.Linq;
using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prem.PTC.Payments
{
    public class PayeerTransactionResponse : TransactionResponse
    {
        internal PayeerTransactionResponse(PayeerAccount usedAccount, JObject response, bool IsException = false)
        {
            if (IsException)
            {
                UsedAccount = usedAccount;
                RawResponse = String.Empty;
                Note = String.Empty;
                IsSuccess = true;
            }
            else
            {
                RawResponse = response.ToString();
                UsedAccount = usedAccount;
                Note = string.Empty;

                var errors = response["errors"];

                if (errors == null || (errors != null && errors.ToObject<bool>() == false))
                {
                    IsSuccess = true;
                    ReferenceNumber = TryGetValue(response["historyId"]);
                    Note = TryGetValue(response["historyId"]);
                }
                else
                {
                    IsSuccess = false;
                    try
                    {
                        Note = TryGetValue(response["errors"]);
                    }
                    catch (Exception ex) { }
                    ReferenceNumber = string.Empty;
                }
            }
        }

        private string TryGetValue(object input)
        {
            try
            {
                return input.ToString();
            }
            catch(Exception ex)
            {
                return String.Empty;
            }
        }
    }
}