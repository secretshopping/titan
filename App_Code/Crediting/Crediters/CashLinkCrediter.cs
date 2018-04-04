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
    public class CashLinkCrediter : Crediter
    {
        public CashLinkCrediter(Member User)
            : base(User)
        {
        }

        public Money Credit(PtcAdvert Ad)
        {
            Money Calculated = Ad.MoneyToClaimAsCashLink;

            base.CreditMainBalance(Calculated, "CashLink", BalanceLogType.CashLinkClick, false);
  
            //OK mark as watched and credit
            List<int> av = User.AdsViewed;
            av.Add(Ad.Id);
            User.AdsViewed = av;
            User.LastActive = DateTime.Now;

            Money NewTotalEarned = (User.TotalEarned + Calculated);

            User.TryToAddAchievements(
                Prem.PTC.Achievements.Achievement.GetProperAchievements(
                Prem.PTC.Achievements.AchievementType.AfterEarning, NewTotalEarned.GetRealTotals()));

            User.IncreaseUserStatClicks(1);
            User.IncreaseStatClicks(1);
            User.IncreaseCashLinksEarnings(Calculated);

            //Now let's credit the referrer
            CreditReferersMainBalance(Ad.MoneyToClaimAsCashLink, "CashLink /ref/" + User.Name, BalanceLogType.CashLinkRefClick, 1, null, false);

            User.Save();

            return Calculated;
        }

        protected override Money CalculateRefEarnings(Member User, Money Input, int tier)
        {
            if (tier > 1)
                return Money.Zero;

            return Money.MultiplyPercent(Input, 100);
        }
    }
}