using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prem.PTC.Payments
{
    public class SolidTrustPayTransactionResponse : TransactionResponse
    {
        internal SolidTrustPayTransactionResponse(SolidTrustPayAccount usedAccount, NameValuePairs response, string rawResponse)
        {
            RawResponse = response.ToString();
            UsedAccount = usedAccount;
            Note = string.Empty;

            //Get success indicator
            IsSuccess = rawResponse.Contains("ACCEPTED");
            Note = rawResponse;

            //Get reference number
            if (IsSuccess)
            {
                try
                {
                    int beginIndex = rawResponse.IndexOf("=") + 1;
                    int endIndex = rawResponse.IndexOf("Status") - 1;
                    ReferenceNumber = rawResponse.Substring(beginIndex, endIndex);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                }
            }

        }
    }
}