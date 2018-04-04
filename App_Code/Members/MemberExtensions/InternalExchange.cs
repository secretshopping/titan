using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.InternalExchange;

namespace Prem.PTC.Members
{
    public partial class Member : BaseTableObject
    {
        public static decimal GetCurrentUserInternalExchangeAskAmount()
        {
            return InternalExchangeManager.GetUserInternalExchangeAskAmount(Member.CurrentId);
        }

        public decimal GetInternalExchangeDayAskAmount()
        {
            return InternalExchangeManager.GetUserInternalExchangeAskAmount(this.Id);
        }

        public decimal GetInternalExchangeFreezedStock()
        {
            var query = String.Format("SELECT SUM(AskAmount) FROM InternalExchangeAsks WHERE AskUserId = {0} AND AskStatus = 1;",
                this.Id);

            var result = TableHelper.SelectScalar(query);
            return (result is DBNull) ? new decimal(0) : Convert.ToDecimal(result);
        }

        public decimal GetInternalExchangeFreezedMoney()
        {
            var query = String.Format("SELECT SUM(BidAmount * BidValue) FROM InternalExchangeBids WHERE BidUserId = {0} AND BidStatus = 1;",
                this.Id);

            var result = TableHelper.SelectScalar(query);
            return (result is DBNull) ? new decimal(0) : Convert.ToDecimal(result);
        }
    }
}