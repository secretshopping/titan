using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using MarchewkaOne.Titan.Balances;

namespace Titan
{
    public class CrowdflowerCrediter : Crediter
    {
        public CrowdflowerCrediter(Member User)
            : base(User)
        {
        }

        /// <summary>
        /// Fired when credited
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="As"></param>
        /// <param name="OfferwallName"></param>
        /// <param name="RequiresConversion"></param>
        /// <returns></returns>
        public Money Credit(Money Input, CreditAs As, string OfferwallName, bool RequiresConversion = false)
        {
            Money Calculated = CreditGeneral(Input, As, OfferwallName, RequiresConversion);
            User.Save();

            //Contests
            Prem.PTC.Contests.ContestManager.IMadeAnAction(Prem.PTC.Contests.ContestType.CrowdFlower, User.Name, null, 1);

            return Calculated;
        }

        /// <summary>
        /// Fired when reversed
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="As"></param>
        /// <param name="OfferwallName"></param>
        /// <param name="RequiresConversion"></param>
        /// <returns></returns>
        public Money Reverse(Money Input, CreditAs As, string OfferwallName, bool RequiresConversion = false)
        {
            OfferwallName = OfferwallName + " - REVERSAL";

            Money Calculated = CreditGeneral(Input.Negatify(), As, OfferwallName, RequiresConversion);
            User.Save();

            //Contests
            Prem.PTC.Contests.ContestManager.IMadeAnAction(Prem.PTC.Contests.ContestType.CrowdFlower, User.Name, null, -1);

            return Calculated;
        }

        private Money CreditGeneral(Money Input, CreditAs As, string OfferwallName, bool RequiresConversion = false, bool IsReversal = false)
        {
            Money Calculated = Money.MultiplyPercent(Input, base.User.Membership.OfferwallsProfitPercent);

            //Conversion?
            if (RequiresConversion)
            {
                if (As == CreditAs.Points)
                    Calculated = new Money(PointsConverter.ToPoints(Calculated));
                if (As == CreditAs.MainBalance)
                    Calculated = PointsConverter.ToMoney(Calculated.ToDecimal());
            }

            if (As == CreditAs.Points)
            {
                base.CreditPoints(Calculated.GetRealTotals(), "CrowdFlower", BalanceLogType.Other);
                base.CreditReferersPoints(Calculated.GetRealTotals(), "CrowdFlower /ref/ " + User.Name, BalanceLogType.Other);
            }
            if (As == CreditAs.MainBalance)
            {
                base.CreditMainBalance(Calculated, "CrowdFlower", BalanceLogType.Other);
                base.CreditReferersMainBalance(Calculated, "CrowdFlower /ref/ " + User.Name, BalanceLogType.Other);
            }

            History.AddOfferwalCompleted(User.Name, OfferwallName, Calculated, As);

            return Calculated;
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            List<Commission> commissions = (List<Commission>)new CommissionsCache().Get();
            var commission = commissions.FirstOrDefault(c => c.MembershipId == user.Membership.Id && c.RefLevel == tier);

            if (commission == null)
                return Money.Zero;

            return Money.MultiplyPercent(amount, commission.CPAOfferPercent);
        }
    }
}