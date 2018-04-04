using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;

public class CommissionMatrixCrediter : MatrixCrediterBase
{
    public override void Credit(Member user, Money servicePrice)
    {
        var maxLevels = AppSettings.Matrix.MatrixMaxCreditedLevels;

        for (int i = 1; i <= maxLevels; i++)
        {
            Member ancestor = user.GetAncestor(1);
            MatrixCommissionLevel commissionLevel = MatrixCommissionLevel.GetByLevel(i);

            if (ancestor == null)
                break;

            if(commissionLevel.Commission != Decimal.Zero)
            {
                Money commission = servicePrice * commissionLevel.Commission / 100.00m;

                ancestor.AddToCashBalance(commission,
                    String.Format("Matrix level {0} commission reward", commissionLevel.CommissionLevel));
            }

            user = ancestor;
        }
    }
}