using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Prem.PTC.Advertising;
using Prem.PTC.Contests;
using Prem.PTC.Referrals;
using MarchewkaOne.Titan.Balances;

namespace Titan
{
    public class TrafficExchangeCrediter : Crediter
    {
        public TrafficExchangeCrediter(Member User)
            : base(User)
        {
        }

        public Money CreditMember(TrafficExchangeAdvert Ad)
        {
            try
            {
                var referer = new Member(User.ReferrerId);
                Money DRCaltulated = TrafficExchangeAdvert.CalculateEarningsFromDirectReferralTE(referer, Ad);
                referer.AddToTrafficBalance(DRCaltulated, "Referrals Traffic Exchange", BalanceLogType.Other);
                referer.SaveBalances();
            }
            catch (Exception e) { }

            Money Calculated = TrafficExchangeAdvert.CalculateNormalMemberEarningsTE(User, Ad);

            User.AddToTrafficBalance(Calculated, "Traffic Exchange", BalanceLogType.Other);

            User.SaveBalances();

            return Calculated;
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            return Money.Zero;
        }
    }
}