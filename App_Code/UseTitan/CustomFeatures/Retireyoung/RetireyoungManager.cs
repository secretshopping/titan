using Prem.PTC;
using System;
using Titan.InvestmentPlatform;

namespace Titan.CustomFeatures
{
    public enum Fridays { ThisFriday, NextFriday }

    [Serializable]
    public class ReportRecord
    {
        public string Email;
        public DateTime RequestDate;
        public Fridays PaymentDate;
        public decimal AmountBTC;
        public decimal AmountUSD;
        public decimal MainBalance;
    }

    public class RetireyoungManager
    {
        public static Money GetAggregate(int userId)
        {
            var query = string.Format("SELECT SUM(MoneyInsystem) FROM InvestmentUsersPlans WHERE [UserId] = {0} and [Status] = {1}", userId, (int)PlanStatus.Active);
            return Money.Parse(TableHelper.SelectScalar(query).ToString()) ?? Money.Zero;
        }
    }
}