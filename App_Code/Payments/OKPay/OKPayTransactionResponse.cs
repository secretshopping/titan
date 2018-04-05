using Newtonsoft.Json.Linq;
using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using OkPayAPI;
using System.Text;

namespace Prem.PTC.Payments
{
    public class OKPayTransactionResponse : TransactionResponse
    {
        internal OKPayTransactionResponse(OKPayAccount usedAccount, Exception ex)
        {
            UsedAccount = usedAccount;
            RawResponse = ex.Message;
            Note = ex.Message;
            ReferenceNumber = ex.Message;
            IsSuccess = false;
        }

        internal OKPayTransactionResponse(OKPayAccount usedAccount, TransactionInfo info)
        {
            RawResponse = info.ToString();
            UsedAccount = usedAccount;
            Note = info.Status.ToString();
            IsSuccess = (info.Status == OperationStatus.Completed || info.Status == OperationStatus.Hold || info.Status == OperationStatus.Pending);
            ReferenceNumber = info.ID.ToString();
        }
    }
}