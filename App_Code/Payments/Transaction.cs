using Prem.PTC.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Transaction
{
    public TransactionRequest Request { get; protected set; }
    public TransactionResponse Response { get; protected set; }
    private PaymentAccount _account;

    internal Transaction() { }

    internal Transaction(TransactionRequest request, PaymentAccount account)
    {
        Request = request;
        _account = account;
    }

    public virtual TransactionResponse Commit()
    {
        return Response = _account.CommitTransaction(Request);
    }
}
