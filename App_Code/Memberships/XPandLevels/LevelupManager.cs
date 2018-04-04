using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using System.Text;
using Resources;

public class LevelupManager
{
    //Settings section for LevelUP Manager

    Member user;
    IMembership membership;
    public LevelupManager(Member user)
    {
        this.user = user;
        membership = user.Membership;
    }

    public void AddPrize(ref LevelNotification notification)
    {
        Random random = new Random();
        int RandomBetween1100 = random.Next(1, 101);

        if (RandomBetween1100 <= membership.LevelChanceToWinAnyAward)
        {
            RandomBetween1100 = random.Next(1, 101);
            int PercentHelper = 0;

            if (membership.LevelPointsPrizeChance > 0 && RandomBetween1100 <= PercentHelper + membership.LevelPointsPrizeChance)
            {
                int amount = random.Next(membership.LevelPointsPrizeMin, membership.LevelPointsPrizeMax);
                if (amount > 0)
                {
                    History.Add(user.Name, "Levelup Reward: " + amount + " " + AppSettings.PointsName);
                    user.AddToPointsBalance(amount, "Levelup Reward", triggerActions: false);
                    user.SaveBalances();
                    notification.PointsReward = amount;                  
                }

            }
            PercentHelper += membership.LevelPointsPrizeChance;

            if (membership.LevelAdCreditsChance> 0 && RandomBetween1100 <= PercentHelper + membership.LevelAdCreditsChance)
            {
                int amount = random.Next(membership.LevelAdCreditsMin, membership.LevelAdCreditsMax);
                if (amount > 0)
                {
                    History.Add(user.Name, "Levelup Reward: " + amount + " " + U5006.ADCREDITS);
                    user.AddToPTCCredits((decimal)amount, "Levelup Reward");
                    user.SaveBalances();
                    notification.PTCCreditsReward = amount;
                }
            }
            PercentHelper += membership.LevelAdCreditsChance;

            if (membership.LevelDRLimitIncreasedChance > 0 && RandomBetween1100 <= PercentHelper + membership.LevelDRLimitIncreasedChance)
            {
                int amount = random.Next(membership.LevelDRLimitIncreasedMin, membership.LevelDRLimitIncreasedMax);
                if (amount > 0)
                {
                    History.Add(user.Name, "Levelup Reward: " + U5008.DRLIMITENLARGEDBY + " " + amount);
                    user.DirectReferralLimitEnlargedBy += amount;
                    user.Save();
                    notification.DRLimitReward = amount;                  
                }
            }
        }
    }
}
