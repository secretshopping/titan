using Prem.PTC.Members;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Prem.PTC.Payments
{
    public class TransactionRequest
    {
        public string MemberName { get; set; }
        public string PayeeId { get; set; }
        public Money Payment { get; set; }
        public String Note { get; set; }

        #region Constructors

        public TransactionRequest() { }

        public TransactionRequest(Member member, Money amount, string transactionNote = "")
        {
            MemberName = member.Name;
            PayeeId = member.Email;
            Payment = amount;
            Note = transactionNote;
        }

        public TransactionRequest(string memberName, string payeeId, Money amount, string transactionNote = "")
        {
            MemberName = memberName;
            PayeeId = payeeId;
            Payment = amount;
            Note = transactionNote;
        }

        #endregion Constructors
    }
}