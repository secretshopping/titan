using System.Collections.Generic;
using System.Linq;
using Prem.PTC;
using Prem.PTC.Members;
using MarchewkaOne.Titan.Balances;
using Titan.Leadership;

namespace Titan
{
    public class InvestmentPlanCrediter : Crediter
    {
        public InvestmentPlanCrediter(Member User) : base(User) { }

        public void CreditPlan(Money amount, BalanceType balancetype, string note, BalanceLogType logType)
        {
            switch (balancetype)
            {
                case BalanceType.MainBalance:
                    base.CreditMainBalance(amount, note, logType);
                    break;
                case BalanceType.InvestmentBalance:
                    //TO DO
                    break;
            }
        }

        public void CreditStructure(Money price)
        {
            User.InvestedIntoPlans += price;

            var list = new List<RestrictionKind>();
            list.Add(RestrictionKind.InvestedMoney);
            list.Add(RestrictionKind.InvestedMoneyStructure);
            RecalculateInvestmentStructure(User, price);

            LeadershipSystem.CheckSystem(list, User, InvestmentLevelCommision.Count - 1);
        }

        public Money CreditReferer(Money price)
        {
            Money moneySpent = base.CreditReferersMainBalance(price, "Investment Plan Purchase /ref/" + User.Name, BalanceLogType.InvestmentPlatformRefPlanPurchase);
            Money moneyLeftForPools = price - moneySpent;

            User.Save();
            return moneyLeftForPools;
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            List<Commission> commissions = (List<Commission>)new CommissionsCache().Get();
            var commission = commissions.FirstOrDefault(c => c.MembershipId == user.Membership.Id && c.RefLevel == tier);

            if (commission == null)
                return Money.Zero;

            return Money.MultiplyPercent(amount, commission.InvestmentPlanPurchasePercent);
        }

        protected void RecalculateInvestmentStructure(Member user, Money investment, int level = 0)
        {
            user.InvestedIntoPlansStructure += investment * InvestmentLevelCommision[level] / 100.0m;
            user.SaveInvestmentPlans();

            if(user.HasReferer && level <= AppSettings.Referrals.ReferralEarningsUpToTier)
            {
                RecalculateInvestmentStructure(new Member(user.ReferrerId), investment, ++level);
            }
        }

        public Dictionary<int, decimal> InvestmentLevelCommision
        {
            get
            {
                return LeadershipInvestmentLevelCommission.GetAvailableLevels().ToDictionary(x => x.CommissionLevel, y => y.Commission);
            }
        }
    }

}