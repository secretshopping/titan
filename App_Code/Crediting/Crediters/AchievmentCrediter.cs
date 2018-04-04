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
    public class AchievmentCrediter : Crediter
    {
        public AchievmentCrediter(Member User)
            : base(User)
        {
        }

        public void CreditPoints(int points)
        {
            base.CreditPoints(points, "Achievement", BalanceLogType.Other);
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            return Money.Zero;
        }
    }
}