using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Titan.Leadership;
using Titan.Matrix;
using MemberExtentionMethods;

public class BinaryReferralMatrixCrediter : MatrixCrediterBase
{
    public override void Credit(Member user, Money servicePrice)
    {
        var maxLevels = AppSettings.Matrix.MatrixMaxCreditedLevels;

        for (int i = 1; i <= maxLevels; i++)
        {
            var ancestor = user.GetAncestor(1);

            if (ancestor == null)
                break;

            var nodeId = user.MatrixId.ToString().Substring(user.MatrixId.ToString().Length - 2); //Can be /1 or /2
            var leg = nodeId == "1/" ? MatrixLeg.Left : MatrixLeg.Right;

            CreditAncestor(ancestor, servicePrice, 100, leg); //100% of AdPack price credited to leg

            user = ancestor;
        }
    }

    private void CreditAncestor(Member ancestor, Money servicePrice, decimal percentage, MatrixLeg leg)
    {
        try
        {
            if (percentage <= 0)
                return;

            Money bonus = Money.MultiplyPercent(servicePrice, percentage);

            //We are adding to left or right leg
            if (leg == MatrixLeg.Left)
                ancestor.MatrixBonusMoneyFromLeftLeg += bonus;
            else
                ancestor.MatrixBonusMoneyFromRightLeg += bonus;

            if (ancestor.MatrixBonusMoneyFromLeftLeg > AppSettings.Matrix.MatrixCyclesBonusMoneyFromLeg &&
                ancestor.MatrixBonusMoneyFromRightLeg > AppSettings.Matrix.MatrixCyclesBonusMoneyFromLeg)
            {
                int cyclesPerDay = -1;

                if(AppSettings.Matrix.MatrixCyclesFromRank)
                {
                    LeadershipRank rank = ancestor.GetRank();

                    cyclesPerDay = rank != null ? rank.MatrixCyclesPerDay : cyclesPerDay;
                }
                else
                {
                    cyclesPerDay = AppSettings.Matrix.MatrixCyclesPerDay;
                }
                
                //We cycle or flush out
                if (ancestor.MatrixBonusMoneyCyclesToday < cyclesPerDay)
                {
                    ancestor.MatrixBonusMoneyCyclesToday++;
                    ancestor.MatrixBonusMoneyFromLeftLeg -= AppSettings.Matrix.MatrixCyclesBonusMoneyFromLeg;
                    ancestor.MatrixBonusMoneyFromRightLeg -= AppSettings.Matrix.MatrixCyclesBonusMoneyFromLeg;

                    if(TitanFeatures.IsTrafficThunder)
                        ancestor.AddToCommissionBalance(AppSettings.Matrix.MatrixCyclesPrizeMoney, "Matrix Cycle bonus");
                    else
                        ancestor.AddToMainBalance(AppSettings.Matrix.MatrixCyclesPrizeMoney, "Matrix Cycle bonus");

                    ancestor.IncreaseMetrixBonusEarnings(AppSettings.Matrix.MatrixCyclesPrizeMoney);
                    ancestor.Save();
                }
                else
                {
                    //We flush
                    ancestor.MatrixBonusMoneyFromLeftLeg = Money.Zero;
                    ancestor.MatrixBonusMoneyFromRightLeg = Money.Zero;
                    ancestor.SaveMatrixId();
                }
            }
            else
            {
                ancestor.SaveMatrixId();
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }
}