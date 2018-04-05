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
    public class UniqueAdCrediter : Crediter
    {
        public UniqueAdCrediter(Member User)
            : base(User)
        {
        }

        public Money Credit(UniqueAd ad)
        {
            Money calculated = ad.RewardPerView;

            base.CreditMainBalance(calculated, "Unique Ad Click", BalanceLogType.UniqueAdClick);

            ad.ViewsReceived += 1;

            if (ad.ShouldBeFinished)
                ad.Status = AdvertStatus.Finished;

            ad.Save();
            UniqueAdClick adClick = new UniqueAdClick();
            adClick.UserId = User.Id;
            adClick.UniqueAdId = ad.Id;
            adClick.Save();
            User.LastActive = DateTime.Now;

            

            Money newTotalEarned = (User.TotalEarned + calculated);

            User.TryToAddAchievements(
                Prem.PTC.Achievements.Achievement.GetProperAchievements(
                Prem.PTC.Achievements.AchievementType.AfterEarning, newTotalEarned.GetRealTotals()));

            //Now let's credit the referrer
            //CreditReferersMainBalance(ad.RewardPerView, CalculateRefererEarnings, "Unique Ad /ref/" + User.Name, BalanceLogType.UniqueAdRefClick);

            User.Save();

            return calculated;
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            throw new NotImplementedException();
        }
    }
}