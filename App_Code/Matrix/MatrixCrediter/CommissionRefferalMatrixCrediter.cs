using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Titan.Leadership;
using Titan.Matrix;
using MemberExtentionMethods;

public class CommissionReferralMatrixCrediter : MatrixCrediterBase
{
    public override void Credit(Member user, Money servicePrice)
    {
        var maxLevels = AppSettings.Matrix.MatrixMaxCreditedLevels;

        var levelCommissions = MatrixCommissionReferralLevel.GetActive();

        if (levelCommissions.Count == 0)
            return;

        for (int i = 1; i <= maxLevels; i++)
        {
            var ancestor = user.GetAncestor(i);

            if (ancestor == null)
                break;

            var directRefCount = ancestor.GetActiveDirectReferralsCount();

            var relativeLevel = GetRelativeLevel(user, ancestor);

            if (directRefCount == 0 || relativeLevel == 0 || relativeLevel > maxLevels)
                continue;

            if (directRefCount >= relativeLevel)
            {
                var commission = levelCommissions
                                   .Where(l => l.DirectRefCount <= directRefCount)
                                   .OrderByDescending(l => l.DirectRefCount)
                                   .FirstOrDefault();

                if (commission != null)
                    CreditAncestor(ancestor, servicePrice, commission.CommissionPercent);
            }
        }
    }

    private void CreditAncestor(Member ancestor, Money servicePrice, decimal percentage)
    {
        if (percentage <= 0)
            return;

        if (!TitanFeatures.IsRevolca || ancestor.RevenueShareAdsWatchedYesterday >= ancestor.Membership.AdPackDailyRequiredClicks)
        {
            ancestor.AddToMainBalance(Money.MultiplyPercent(servicePrice, percentage), "Matrix Reward");
            ancestor.SaveBalances();
        }
    }

    private int GetRelativeLevel(Member descendant, Member ancestor)
    {
        try
        {
            return (descendant.MatrixId.GetLevel() - ancestor.MatrixId.GetLevel()).Value;
        }
        catch
        {
            return 0;
        }
    }
}