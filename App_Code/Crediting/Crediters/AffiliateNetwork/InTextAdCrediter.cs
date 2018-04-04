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
using Prem.PTC.Offers;
using Titan.Publisher.InTextAds;

namespace Titan
{
    public class InTextAdCrediter : Crediter
    {
        Money amount;
        public InTextAdCrediter(Money amount, int publisherId)
            : base(new Member(publisherId))
        {
            this.amount = amount;
        }

        public Money Credit()
        {
            var amountToCredit = Money.MultiplyPercent(amount, User.Membership.PublishersInTextAdClickProfitPercentage);
            base.CreditMainBalance(amountToCredit, "InText Ad Click", BalanceLogType.Other);
  
            return amount - amountToCredit;
        }

        protected override Money CalculateRefEarnings(Member user, Money input, int tier)
        {
            //if (tier > 1)
            //    return Money.Zero;

            //return Money.MultiplyPercent(input, 100);
            return Money.Zero;
        }
    }
}