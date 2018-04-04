using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Members;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Titan
{
    public class JackpotPvpCrediter : Crediter
    {
        public JackpotPvpCrediter(Member User)
            : base(User)
        {
        }

        public void BuyStage(Money costOfStage)
        {
            Money moneySpent = base.CreditReferersMainBalance(costOfStage, "JackpotPvp Stage buy /ref/" + User.Name, BalanceLogType.PvpJackpotBuy);
            Money moneyLeft  = costOfStage - moneySpent;

            User.SubtractFromCashBalance(costOfStage, "Pvp Jackpot stage buy");
            User.Save();

            PoolDistributionManager.AddProfit(ProfitSource.JackpotPvp, moneyLeft);
        }

        public void CreditWin(Money cashWon)
        {
            base.CreditMainBalance(cashWon, "PvpJackpot Win", BalanceLogType.PvpJackpotWin);
            User.Save();


            if (TitanFeatures.IsNightwolf && PoolDistributionManager.GetGlobalPoolSumInMoney(PoolsHelper.GetBuiltInProfitPoolId(Pools.PvpJackpotGamePool)) < cashWon)
                GlobalPoolManager.SubtractFromPool(PoolsHelper.GetBuiltInProfitPoolId(Pools.AdministratorProfit), cashWon);
            else
                GlobalPoolManager.SubtractFromPool(PoolsHelper.GetBuiltInProfitPoolId(Pools.PvpJackpotGamePool), cashWon);
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            List<Commission> commissions = (List<Commission>)new CommissionsCache().Get();
            var commission = commissions.FirstOrDefault(c => c.MembershipId == user.Membership.Id && c.RefLevel == tier);

            if (commission == null)
                return Money.Zero;

            return Money.MultiplyPercent(amount, commission.JackpotPvpStageBuyFeePercent);
        }
    }
}