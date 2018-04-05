using Prem.PTC.Members;
using System;

namespace Prem.PTC.Payments
{
    public class RevolutAccount : PaymentAccount<RevolutAccountDetails>
    {
        protected RevolutAPI RevolutAPI;
        protected RevolutAccountDetails account;

        public RevolutAccount(RevolutAccountDetails details) : base(details)
        {
            account = details;
            RevolutAPI = new RevolutAPI(account.APIKey);
        }        

        public override Money Balance
        {
            get
            {
                try
                {
                    return new Money(RevolutAPI.GetAccount().Balance);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                    return new Money(0);
                }

            }
        }

        public override TransactionResponse CommitTransaction(TransactionRequest request)
        {
            var phone = new Member(request.MemberName).PhoneNumber;
             
            return new RevolutTransactionResponse(this, RevolutAPI.CreatePayment(request.MemberName, phone, request.Payment.ToDecimal() ));
        }
    }
}