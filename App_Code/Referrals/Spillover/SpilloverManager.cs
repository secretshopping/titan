using Prem.PTC.Members;
using MemberExtentionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;

/// <summary>
/// Summary description for SpilloverManager
/// </summary>
public class SpilloverManager
{
    private readonly Member userToResolve;
    private readonly Member oldReferer;
    public SpilloverManager(Member userToResolve)
    {
        this.userToResolve = userToResolve;
        this.oldReferer = new Member(userToResolve.ReferrerId);
    }
    public void ResolveReferrals()
    {
        try
        {
            if (userToResolve.ReferrerId == AppSettings.RevShare.AdminUserId)
                return;

            if (oldReferer.GetEverUpgradedReferralsList().Count > oldReferer.Membership.MaxUpgradedDirectRefs)
                MoveToDownline();
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }
    private void MoveToDownline()
    {
        var newReferer = FindNewRefererId(oldReferer, 1);

        if (newReferer == null || newReferer.Id == oldReferer.Id)
            newReferer = new Member(AppSettings.RevShare.AdminUserId);

        userToResolve.TryAddReferer(newReferer);
        userToResolve.Save();
    }

    private Member FindNewRefererId(Member user, int tier)
    {
        Member newRefererHelper = null;

        if (tier > AppSettings.Referrals.ReferralEarningsUpToTier)
            return oldReferer;

        var referrals = user.GetEverUpgradedReferralsList().Where(r => r.Id != userToResolve.Id).ToList();

        for (int i = 0; i < referrals.Count && newRefererHelper == null; i++)
        {
            if (referrals[i].GetEverUpgradedReferralsList().Count < referrals[i].Membership.MaxUpgradedDirectRefs)
                newRefererHelper = referrals[i];
        }

        if (newRefererHelper != null)
            return newRefererHelper;

        for (int i = 0; i < referrals.Count && newRefererHelper == null; i++)
        {
            newRefererHelper = FindNewRefererId(referrals[i], tier + 1);
        }

        if (newRefererHelper != null)
            return newRefererHelper;

        return oldReferer;
    }

    public static void AddMoneyToRevenuePool(Money amount)
    {
        var poolId = PoolsHelper.GetBuiltInProfitPoolId(Pools.AdPackRevenueReturn);
        var pool = TableHelper.GetListFromRawQuery<ProfitPoolDistribution>(string.Format("SELECT * FROM ProfitPoolDistribution WHERE Pool = {0}", poolId)).FirstOrDefault();
        if (pool != null)
            PoolDistributionManager.AddProfit(ProfitSource.Memberships, amount, pool);
    }
}
