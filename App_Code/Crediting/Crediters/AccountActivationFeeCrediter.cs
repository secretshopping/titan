using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Members;
using System.Collections.Generic;
using System.Linq;

namespace Titan
{
    public class AccountActivationFeeCrediter : Crediter
    {
        public AccountActivationFeeCrediter(Member User)
            : base(User)
        {
        }

        /// <summary>
        /// Returns Money amount after referer credit
        /// </summary>
        /// <param name="activationFee"></param>
        /// <returns></returns>
        public Money CreditReferer(Money activationFee)
        {
            Money moneySpent = base.CreditReferersMainBalance(activationFee, "Account activation fee /ref/" + User.Name, BalanceLogType.Other);
            Money moneyLeftForPools = activationFee - moneySpent;
            User.Save();
            return moneyLeftForPools;
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            List<Commission> commissions = (List<Commission>)new CommissionsCache().Get();
            var commission = commissions.FirstOrDefault(c => c.MembershipId == user.Membership.Id && c.RefLevel == tier);

            if (commission == null)
                return Money.Zero;

            return Money.MultiplyPercent(amount, commission.AccountActivationFeePercent);
        }

    }
}