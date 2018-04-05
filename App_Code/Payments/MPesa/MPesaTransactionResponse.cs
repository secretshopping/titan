using Newtonsoft.Json.Linq;
using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prem.PTC.Payments
{
    public class MPesaTransactionResponse : TransactionResponse
    {
        internal MPesaTransactionResponse(MPesaAccount usedAccount, Exception ex)
        {
            UsedAccount = usedAccount;
            RawResponse = ex.Message;
            Note = ex.Message;
            ReferenceNumber = ex.Message;
            IsSuccess = false;
        }

        internal MPesaTransactionResponse(MPesaAccount usedAccount, dynamic response)
        {
            try
            {
                UsedAccount = usedAccount;
                IsSuccess = response.entries[0].status == "Queued";

                if (IsSuccess)
                    ReferenceNumber = response.entries[0].transactionId;
                else
                    Note = response.entries[0].errorMessage;
            }
            catch(Exception ex)
            {
                IsSuccess = true;
            }
        }
    }
}