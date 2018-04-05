using System;
using System.Collections.Generic;
using Prem.PTC.Utils.NVP;

namespace Prem.PTC.Payments
{
    public class TransactionResponse
    {
        public bool IsSuccess { get; protected set; }
        public string Note { get; protected set; }
        public PaymentAccount UsedAccount { get; protected set; }
        public string RawResponse; // dodane w celu łatwiejszego logowania
        /// <summary>
        /// Transaction reference number. For PayPal it is CORRELATIONID, for Payza 
        /// REFERENCENUMBER
        /// </summary>
        public string ReferenceNumber { get; protected set; }

        public TransactionResponse() { }

        public TransactionResponse(bool isSuccess, string responseNote, PaymentAccount usedAccount)
        {
            IsSuccess = isSuccess;
            Note = responseNote;
            UsedAccount = usedAccount;
        }
    }
}