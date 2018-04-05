using Prem.PTC.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Prem.PTC.Payments
{
    public sealed class TransactionFactory
    {
        private TransactionFactory() { }

 
        /// <exception cref="DbException"/>
        //public static Transaction CreateTransaction(TransactionRequest request)
        //{
        //    var gatewayDetails = PaymentAccountDetails.AllGateways;

        //    return createMultiGatewayTransaction(request, gatewayDetails);
        //}


        /// <exception cref="DbException"/>
        public static Transaction CreateTransaction<T>(TransactionRequest request) where T : PaymentAccountDetails
        {
            var gatewayDetails = TableHelper.SelectAllRows<T>();

            return createMultiGatewayTransaction(request, gatewayDetails);
        }


        /// <param name="accountType">Payza, PayPal etc</param>
        /// <exception cref="DbException"/>
        public static Transaction CreateTransaction(TransactionRequest request, string accountType)
        {
            Type t = PaymentAccountDetails.GetGatewayType(accountType);
            var instance = Activator.CreateInstance(t);

            MethodInfo method = typeof(TransactionFactory).GetMethod("CreateTransaction", new[] { typeof(TransactionRequest) });
            MethodInfo generic = method.MakeGenericMethod(t);

            return (Transaction)generic.Invoke(null, new object[] { request });

        }


        public static Transaction CreateTransaction(TransactionRequest request, PaymentAccountDetails account)
        {
            return new Transaction(request, account.Account);
        }

        private static Transaction createMultiGatewayTransaction<T>(TransactionRequest request, IList<T> gatewayDetails)
            where T : PaymentAccountDetails
        {
            var gatewaysOrdered = (from gateway in gatewayDetails
                                   where (gateway.Cashflow == GatewayCashflowDirection.FromGate ||
                                         gateway.Cashflow == GatewayCashflowDirection.Both)
                                         && gateway.IsActive
                                   orderby gateway.CashoutPriority ascending
                                   select gateway.Account);

            return new MultiGatewayTransaction(request, gatewaysOrdered.ToList());
        }
    }
}