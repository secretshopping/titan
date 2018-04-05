using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{
    public class PayzaTransactionResponse : TransactionResponse
    {
        public int ReturnCode { get; private set; }
        public bool IsTestMode { get; private set; }

        private static readonly int SuccessCode = 100;

        internal PayzaTransactionResponse(PayzaAccount usedAccount, NameValuePairs response)
        {
            RawResponse = response.ToString();
            UsedAccount = usedAccount;

            try
            {
                string refNo;
                response.TryGetValue("REFERENCENUMBER", out refNo);
                ReferenceNumber = refNo;

                ReturnCode = Convert.ToInt32(response["RETURNCODE"]);
                Note = response["DESCRIPTION"];
                IsTestMode = response["TESTMODE"] == "1";
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e);
            }

            IsSuccess = (ReturnCode == SuccessCode);
        }

        internal PayzaTransactionResponse(PayzaAccount usedAccount)
        {
            UsedAccount = usedAccount;

            Note = RawResponse = "Error getting response from Payza. Marked as successful.";
            ReferenceNumber = "X";
            IsSuccess = true;
        }
    }
}