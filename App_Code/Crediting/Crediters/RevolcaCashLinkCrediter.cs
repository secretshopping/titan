using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for RevolcaCashLinkCrediter
/// </summary>
public class RevolcaCashLinkCrediter
{
    private readonly Member user;

    public RevolcaCashLinkCrediter(Member user)
    {
        this.user = user;
    }

    public Money Credit(PtcAdvert ad)
    {
        Money calculated = ad.MoneyToClaimAsCashLink;

        user.AddToMainBalance(calculated, "CashLink", BalanceLogType.CashLinkClick);
        user.IncreaseEarnings(calculated);

        //OK mark as watched and credit
        List<int> av = user.AdsViewed;
        av.Add(ad.Id);
        user.AdsViewed = av;
        user.LastActive = DateTime.Now;

        Money NewTotalEarned = (user.TotalEarned + calculated);

        user.TryToAddAchievements(
            Prem.PTC.Achievements.Achievement.GetProperAchievements(
            Prem.PTC.Achievements.AchievementType.AfterEarning, NewTotalEarned.GetRealTotals()));

        user.IncreaseUserStatClicks(1);
        user.IncreaseStatClicks(1);
        user.IncreaseCashLinksEarnings(calculated);

        user.Save();

        DailyPoolManager.AddToPool(PoolsHelper.GetBuiltInProfitPoolId(Pools.AdPackRevenueReturn), DateTime.Now, calculated);
        AdPackProfitDistributionHistory.Add(calculated);

        return calculated;
    }
}