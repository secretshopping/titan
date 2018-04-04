using LocalBitcoins;
using Newtonsoft.Json.Linq;
using System;

namespace Prem.PTC.Payments
{
    public class LocalBitcoinsAccount : PaymentAccount<LocalBitcoinsAccountDetails>
    {
        public LocalBitcoinsAccount(LocalBitcoinsAccountDetails details) : base(details) { account = details; }
        protected LocalBitcoinsAccountDetails account;

        public override Money Balance
        {
            get
            {
                try
                {
                    var ApiKey = account.APIKey;
                    var ApiSecret = account.APISecret;
                    var clientAPI = new LocalBitcoinsAPI(ApiKey, ApiSecret);                   
                    var data = JObject.Parse(clientAPI.GetWallet());

                    return new Money(Convert.ToDecimal(data.total.balance));
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                    return Money.Zero;
                }
            }
        }

        public override TransactionResponse CommitTransaction(TransactionRequest request)
        {
            throw new MsgException("Not implemented");
        }
    }
}