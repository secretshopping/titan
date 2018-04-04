using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Prem.PTC.Payments
{
    /// <summary>
    /// Tries to commit transaction using consecutive gateways.
    /// Ends on first successful transaction response or when
    /// all given gateways fail.
    /// </summary>
    public class MultiGatewayTransaction : Transaction
    {
        private IList<PaymentAccount> _accounts;

        public MultiGatewayTransaction(TransactionRequest request, IList<PaymentAccount> accounts)
        {
            Request = request;
            _accounts = accounts;
        }

        public override TransactionResponse Commit()
        {
            if (_accounts == null || _accounts.Count == 0) 
                return (Response = new TransactionResponse(false, "No gateways provided", null));

            foreach (PaymentAccount account in _accounts)
            {
                base.Response = account.CommitTransaction(Request);
                if (Response.IsSuccess) break;
            }

            return Response;
        }
    }

}