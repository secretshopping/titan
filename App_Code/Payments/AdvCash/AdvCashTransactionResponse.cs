using Newtonsoft.Json.Linq;
using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prem.PTC.Payments
{
    public class AdvCashTransactionResponse : TransactionResponse
    {
        internal AdvCashTransactionResponse(AdvCashAccount usedAccount, Exception ex)
        {
            UsedAccount = usedAccount;
            RawResponse = ex.Message;
            Note = ex.Message;
            ReferenceNumber = ex.Message;
            IsSuccess = false;
        }

        internal AdvCashTransactionResponse(AdvCashAccount usedAccount, string id)
        {
            RawResponse = id;
            UsedAccount = usedAccount;
            Note = id;
            IsSuccess = true;
            ReferenceNumber = id;
        }
    }
}