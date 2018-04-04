using System;
using System.Collections.Generic;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Titan.InvestmentPlatform;
using Titan.MemerModels;

public class CampaingMatrix : MatrixBase
{
    public override void CRON()
    {
        try
        {
            if (!AppSettings.TitanFeatures.ReferralMatrixEnabled)
                return;

            if (AppSettings.Matrix.Type == MatrixType.Campaing &&
                AppSettings.Matrix.LastMatrixRebuild.AddDays(AppSettings.Matrix.DaysBetweenMatrixRebuild).Date <= AppSettings.ServerTime.Date)
            {
                AppSettings.Matrix.Reload();
                ClearMatrix();
                RecreateMatrix();
                AppSettings.Matrix.LastMatrixRebuild = AppSettings.ServerTime;
                AppSettings.Matrix.Save();
            }
        }
        catch(Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    #region CRON Methods
    private void ClearMatrix()
    {
        string query = string.Format(@"
            BEGIN TRANSACTION
                UPDATE Users SET MatrixId = NULL
                WHERE UserId != {0}
            COMMIT TRANSACTION", 
        AppSettings.DirectReferrals.DefaultReferrerEnabled ? AppSettings.DirectReferrals.DefaultReferrerId : 1005);
        TableHelper.ExecuteRawCommandNonQuery(query);
    }

    private void RecreateMatrix()
    {
        var eligibleIds = GetEligibleUserIds();
        if (eligibleIds.Count > 0)
        {
            string query = string.Format(@"
                BEGIN TRANSACTION

                    DECLARE @UserIds Table (Id INT NOT NULL)
                    INSERT INTO @UserIds VALUES ({0})

                    DECLARE @Id int
                    WHILE EXISTS(SELECT * FROM @UserIds)
	                    BEGIN
		                    SELECT @Id = (SELECT TOP 1 Id FROM @UserIds ORDER BY NewId())
		                    DECLARE @Parent hierarchyid;
                            DECLARE @LastChild hierarchyid;
            
                            SELECT @Parent = 
                                (SELECT TOP 1 CAST(A.MatrixId AS hierarchyid)
                                FROM Users AS A 
                                LEFT OUTER JOIN Users AS B
                                    ON CAST(A.MatrixId AS hierarchyid) = CAST(B.MatrixId AS hierarchyid).GetAncestor(1)
                                WHERE A.MatrixId IS NOT NULL
                                GROUP BY A.MatrixId
                                HAVING COUNT(B.UserId) < {1}
                                ORDER BY CAST(A.MatrixId AS hierarchyid).GetLevel(), CAST(A.MatrixId AS hierarchyid));
            
                            SELECT @LastChild = Max(CAST(MatrixId AS hierarchyid)) FROM Users WHERE CAST(MatrixId AS hierarchyid).GetAncestor(1) = @Parent;
            
                            UPDATE Users SET MatrixId = @Parent.GetDescendant(@LastChild,NULL).ToString() 
                            WHERE UserId = @Id;

		                    DELETE FROM @UserIds WHERE Id = @Id
	                    END

                COMMIT TRANSACTION", string.Join("),(", eligibleIds), AppSettings.Matrix.MaxChildrenInMatrix);
            TableHelper.ExecuteRawCommandNonQuery(query);
        }
    }

    private List<int> GetEligibleUserIds()
    {
        int adminId = 1005;
        string requiredAdPackClicks = "0";

        if (AppSettings.DirectReferrals.DefaultReferrerEnabled)
            adminId = (int)AppSettings.DirectReferrals.DefaultReferrerId;

        string getMembersQuery = string.Format(@"SELECT UserId, Username FROM Users 
                                    WHERE AccountStatusInt = {0} AND UserId != {1} AND RevenueShareAdsWatchedYesterday >= {2}",
                                (int)MemberStatus.Active, adminId, requiredAdPackClicks);

        var users = TableHelper.GetListFromRawQuery<MatrixMemberModel>(getMembersQuery);

        var eligibleUserIds = new List<int>();

        foreach (var user in users)
        {
            if (!user.EligibleForMatrix && AppSettings.Matrix.MatrixCheckPtc)
                user.EligibleForMatrix = CheckEligibility(AdvertType.PTC, user);

            if (!user.EligibleForMatrix && AppSettings.Matrix.MatrixCheckBanner)
                user.EligibleForMatrix = CheckEligibility(AdvertType.Banner, user);

            if (!user.EligibleForMatrix && AppSettings.Matrix.MatrixCheckAdPack)
                user.EligibleForMatrix = CheckEligibility(AdvertType.AdPack, user);

            if (!user.EligibleForMatrix && AppSettings.Matrix.MatrixCheckExternalBanner)
                user.EligibleForMatrix = CheckEligibility(AdvertType.ExternalBanner, user);

            if (!user.EligibleForMatrix && AppSettings.Matrix.MatrixCheckFacebook)
                user.EligibleForMatrix = CheckEligibility(AdvertType.Facebook, user);

            if (!user.EligibleForMatrix && AppSettings.Matrix.MatrixCheckInText)
                user.EligibleForMatrix = CheckEligibility(AdvertType.InText, user);

            if (!user.EligibleForMatrix && AppSettings.Matrix.MatrixCheckLogin)
                user.EligibleForMatrix = CheckEligibility(AdvertType.Login, user);

            if (!user.EligibleForMatrix && AppSettings.Matrix.MatrixCheckPtcOfferWall)
                user.EligibleForMatrix = CheckEligibility(AdvertType.PTCOfferWall, user);

            if (!user.EligibleForMatrix && AppSettings.Matrix.MatrixCheckTrafficGrid)
                user.EligibleForMatrix = CheckEligibility(AdvertType.TrafficGrid, user);

            if (!user.EligibleForMatrix && AppSettings.Matrix.MatrixCheckInvestmentPlatform)
                user.EligibleForMatrix = CheckEligibility(AdvertType.InvestmentPlan, user);

            if (user.EligibleForMatrix)
                eligibleUserIds.Add(user.Id);
        }
        return eligibleUserIds;
    }

    private bool CheckEligibility(AdvertType adType, MatrixMemberModel user)
    {
        string query = string.Empty;
        bool eligible = false;
        switch (adType)
        {
            case AdvertType.PTC:
                query = string.Format(
                    @"SELECT CASE WHEN (
                                        SELECT COUNT(PtcAdvertId) FROM PtcAdverts WHERE AdvertiserUserId = {0} AND Status = {1}) = 0 
                                      THEN 0 
                                      ELSE 1 
                                 END;", user.Id, (int)AdvertStatus.Active);
                break;
            case AdvertType.Banner:
                query = string.Format(
                     @"SELECT CASE WHEN (
                                        SELECT COUNT(BannerAdvertId) FROM BannerAdverts WHERE CreatorUsername = '{0}' AND Status = {1}) = 0 
                                      THEN 0 
                                      ELSE 1 
                                 END;", user.Name, (int)AdvertStatus.Active);
                break;
            case AdvertType.Facebook:
                query = string.Format(
                     @"SELECT CASE WHEN (
                                        SELECT COUNT(FbAdvertId) FROM FacebookAdverts WHERE CreatorUsername = '{0}' AND Status = {1}) = 0 
                                      THEN 0 
                                      ELSE 1 
                                 END;", user.Name, (int)AdvertStatus.Active);
                break;
            case AdvertType.AdPack:
                query = string.Format(
                     @"SELECT CASE WHEN (
                                        SELECT COUNT(Id) FROM AdPacks WHERE UserId = {0} AND MoneyToReturn > MoneyReturned) = 0 
                                      THEN 0 
                                      ELSE 1 
                                 END;", user.Id, (int)AdvertStatus.Active);
                break;
            case AdvertType.ExternalBanner:
                query = string.Format(
                     @"SELECT CASE WHEN (
                                        SELECT COUNT(Id) FROM ExternalBannerAdverts WHERE UserId = {0} AND Status = {1}) = 0 
                                      THEN 0 
                                      ELSE 1 
                                 END;", user.Id, (int)AdvertStatus.Active);
                break;
            case AdvertType.InText:
                query = string.Format(
                     @"SELECT CASE WHEN (
                                        SELECT COUNT(Id) FROM InTextAdverts WHERE UserId = {0} AND Status = {1}) = 0 
                                      THEN 0 
                                      ELSE 1 
                                 END;", user.Id, (int)AdvertStatus.Active);
                break;
            case AdvertType.Login:
                query = string.Format(
                     @"SELECT CASE WHEN (
                                        SELECT COUNT(Id) FROM LoginAds WHERE CreatorUserId = {0} AND Status = {1}) = 0 
                                      THEN 0 
                                      ELSE 1 
                                 END;", user.Id, (int)AdvertStatus.Active);
                break;
            case AdvertType.PTCOfferWall:
                query = string.Format(
                     @"SELECT CASE WHEN (
                                        SELECT COUNT(Id) FROM PTCOfferWalls WHERE UserId = {0} AND Status = {1}) = 0 
                                      THEN 0 
                                      ELSE 1 
                                 END;", user.Id, (int)AdvertStatus.Active);
                break;
            case AdvertType.TrafficGrid:
                query = string.Format(
                     @"SELECT CASE WHEN (
                                        SELECT COUNT(PtcAdvertId) FROM TrafficGridAdverts WHERE CreatorUsername = '{0}' AND Status = {1}) = 0 
                                      THEN 0 
                                      ELSE 1 
                                 END;", user.Name, (int)AdvertStatus.Active);
                break;
            case AdvertType.InvestmentPlan:
                query = string.Format(
                     @"SELECT CASE WHEN (
                                        SELECT COUNT(Id) FROM InvestmentUsersPlans WHERE UserId = {0} AND Status = {1}) = 0 
                                      THEN 0 
                                      ELSE 1 
                                 END;", user.Id, (int)PlanStatus.Active);
                break;
            default:
                break;
        }

        if (!string.IsNullOrEmpty(query))
            eligible = Convert.ToBoolean(TableHelper.SelectScalar(query));

        return eligible;
    }
    #endregion

    protected override bool UserIsEligible(Member user)
    {
        return AppSettings.TitanFeatures.ReferralMatrixEnabled &&
                AppSettings.Matrix.Type == MatrixType.Campaing &&
                user.MatrixId.IsNull;
    }

    protected override bool CheckCampaing(AdvertType adType)
    {
        bool canAdd = false;
        switch (adType)
        {
            case AdvertType.PTC:
                canAdd = AppSettings.Matrix.MatrixCheckPtc;
                break;
            case AdvertType.Banner:
                canAdd = AppSettings.Matrix.MatrixCheckBanner;
                break;
            case AdvertType.Facebook:
                canAdd = AppSettings.Matrix.MatrixCheckFacebook;
                break;
            case AdvertType.CPA:
                canAdd = AppSettings.Matrix.MatrixCheckCpa;
                break;
            case AdvertType.AdPack:
                canAdd = AppSettings.Matrix.MatrixCheckAdPack;
                break;
            case AdvertType.ExternalBanner:
                canAdd = AppSettings.Matrix.MatrixCheckExternalBanner;
                break;
            case AdvertType.InText:
                canAdd = AppSettings.Matrix.MatrixCheckInText;
                break;
            case AdvertType.Login:
                canAdd = AppSettings.Matrix.MatrixCheckLogin;
                break;
            case AdvertType.PTCOfferWall:
                canAdd = AppSettings.Matrix.MatrixCheckPtcOfferWall;
                break;
            case AdvertType.TrafficGrid:
                canAdd = AppSettings.Matrix.MatrixCheckTrafficGrid;
                break;
            case AdvertType.InvestmentPlan:
                canAdd = AppSettings.Matrix.MatrixCheckInvestmentPlatform;
                break;
            default:
                break;
        }
        return canAdd;
    }
}