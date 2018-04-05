using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;

public class ReferralMatrix : MatrixBase
{
    public override void CRON()
    {
        try
        {
            if (IsConst) AllocateUnallocatedMembers();
        }
        catch(Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    protected override bool UserIsEligible(Member user)
    {
        return AppSettings.TitanFeatures.ReferralMatrixEnabled &&
                AppSettings.Matrix.Type == MatrixType.Referral &&
                user.MatrixId.IsNull && user.HasReferer;
    }
}