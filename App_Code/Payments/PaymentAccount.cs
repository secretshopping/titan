using System;

namespace Prem.PTC.Payments
{
    public abstract class PaymentAccount
    {
        public abstract Money Balance { get; }
        public abstract TransactionResponse CommitTransaction(TransactionRequest Request);
        public abstract PaymentAccountDetails Details { get; }
    }

    public abstract class PaymentAccount<T> : PaymentAccount
        where T : PaymentAccountDetails
    {
        protected T accountDetails;

        public PaymentAccount(T accountDetails)
        {
            this.accountDetails = accountDetails;
        }

        public override PaymentAccountDetails Details { get { return accountDetails; } }
    }
}