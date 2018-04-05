using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;

public abstract class MatrixBase
{
    public int Length { get { return AppSettings.Matrix.MaxChildrenInMatrix; } }
    public int Depth { get { return AppSettings.Matrix.MatrixMaxCreditedLevels; } }
    //Is Autoallocate Enabled
    public bool IsConst { get { return AppSettings.Matrix.AutolocateMembersInBinaryMatrix; } }

    public int MaxUsers { get { return (int)(Length - Math.Pow((double)Length, (double)(Depth + 1))) / (1 - Length); } }
    public static int UsersCount
    {
        get
        {
            string query = string.Format(@"SELECT COUNT(1) FROM Users WHERE MatrixId IS NOT NULL");
            return (int)TableHelper.SelectScalar(query);
        }
    }
    public static bool HasUsers
    {
        get
        {
            if (AppSettings.Matrix.Type == MatrixType.None) return false;

            return MatrixBase.UsersCount > 1;
        }
    }

    public static MatrixType Type { get { return AppSettings.Matrix.Type; } }
    
    public MatrixCrediterBase Crediter { get; set; }

    public abstract void CRON();

    public void TryAddMember()
    {
        TryAddMember(Member.Current);
    }

    public void TryAddMember(int userId)
    {
        TryAddMember(new Member(userId));
    }

    public bool TryAddMember(Member user)
    {
        if (AppSettings.TitanModules.HasModule(33) || AppSettings.TitanModules.HasModule(39))        
            if (UserIsEligible(user))
                return AddMember(user);        

        return false;
    }

    public void TryAddMember(Member user, AdvertType adType = AdvertType.Null)
    {
        try
        {
            if (AppSettings.TitanModules.HasModule(33) || AppSettings.TitanModules.HasModule(39))            
                if (UserIsEligible(user) && CheckCampaing(adType))
                {
                    AddMember(user);
                    user.ReloadMatrixId();
                }           
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    protected abstract bool UserIsEligible(Member user);

    protected bool AddMember(Member user, Member referer = null)
    {
        if(IsConst)
        {
            return AutoallocateUserInMatrix(user);
        }
        else
        {
            return AddUserToMatrix(user);
        }

        return false;
    }

    public void AllocateUnallocatedMembers()
    {
        //Let check if there are some unalocated members in the matrix. If so, let's allocate them.
        //This situation will only happen when administrator enabled Autoallocate option
        var unallocatedMembers = GetUnassignedMembers();

        for (int i = 0; i < unallocatedMembers.Count; i++)
            TryAddMember(unallocatedMembers[i]);
    }

    public static List<Member> GetAllUnassignedMembers()
    {
        string query = string.Format(@"SELECT UserId, Username, MatrixId, ReferrerId FROM Users WHERE MatrixId IS NULL AND ReferrerId <> -1 ORDER BY RegisterDate ASC");
        return TableHelper.GetListFromRawQuery<Member>(query);
    }

    protected bool AutoallocateUserInMatrix(Member user)
    {
        Member Referrer = new Member(user.ReferrerId);

        if (Referrer.MatrixId.IsNull) //If referrer is not in the matrix, we can't place the user
            return false;
        
        string query = string.Format(@"
        DECLARE @ParentMatrixId hierarchyid;
        DECLARE @ReferrerMatrixId hierarchyid;
        DECLARE @LastChildMatrixId hierarchyid;

        SELECT @ReferrerMatrixId = CAST(MatrixId AS hierarchyid) FROM Users WHERE UserId = {1};

        SELECT @ParentMatrixId = (SELECT TOP 1 CAST(A.MatrixId AS hierarchyid)
                        FROM Users AS A 
                        LEFT OUTER JOIN Users AS B
                            ON CAST(A.MatrixId AS hierarchyid) = CAST(B.MatrixId AS hierarchyid).GetAncestor(1)
                        WHERE A.MatrixId IS NOT NULL AND CAST(A.MatrixId AS hierarchyid).IsDescendantOf(@ReferrerMatrixId) = 1
                        GROUP BY A.MatrixId
                        HAVING COUNT(B.UserId) < {2}
                        ORDER BY CAST(A.MatrixId AS hierarchyid).GetLevel() ASC, CAST(A.MatrixId AS hierarchyid) ASC);
				
        SELECT @LastChildMatrixId = Max(CAST(MatrixId AS hierarchyid)) FROM Users WHERE CAST(MatrixId AS hierarchyid).GetAncestor(1) = @ParentMatrixId;
            
        UPDATE Users SET MatrixId = @ParentMatrixId.GetDescendant(@LastChildMatrixId,NULL).ToString() WHERE UserId = {0};",
        user.Id, Referrer.Id, Depth);

        TableHelper.ExecuteRawCommandNonQuery(query);

        return true;
    }

    protected bool AddUserToMatrix(Member user) //For global matrix (Standard policy)
    {
        string query = string.Format(@"
            DECLARE @Parent hierarchyid;
            DECLARE @LastChild hierarchyid;
            
            SELECT @Parent = 
                (SELECT TOP 1 CAST(A.MatrixId AS hierarchyid)
                FROM Users AS A 
                LEFT OUTER JOIN Users AS B
                    ON CAST(A.MatrixId AS hierarchyid) = CAST(B.MatrixId AS hierarchyid).GetAncestor(1)
                WHERE A.MatrixId IS NOT NULL
                GROUP BY A.MatrixId
                HAVING COUNT(B.UserId) < {0}
                ORDER BY CAST(A.MatrixId AS hierarchyid).GetLevel(), CAST(A.MatrixId AS hierarchyid));
            
            SELECT @LastChild = Max(CAST(MatrixId AS hierarchyid)) FROM Users WHERE CAST(MatrixId AS hierarchyid).GetAncestor(1) = @Parent;
            
            UPDATE Users SET MatrixId = @Parent.GetDescendant(@LastChild,NULL).ToString() WHERE UserId = {1};",
        AppSettings.Matrix.MaxChildrenInMatrix, user.Id);

        TableHelper.ExecuteRawCommandNonQuery(query);

        return true;
    }

    public static void TryAddMemberAndCredit(Member user, Money servicePrice, AdvertType adType = AdvertType.Null)
    {
        if (AppSettings.TitanModules.HasModule(33) || AppSettings.TitanModules.HasModule(39))
        {
            MatrixBase matrix = MatrixFactory.GetMatrix();
            if (matrix != null)
            {
                matrix.TryAddMember(user, adType);
                matrix.Credit(user, servicePrice);
            }
        }
    }

    public void Credit(Member user, Money servicePrice)
    {
        if (user.MatrixId.IsNull)
            return;

        Crediter.Credit(user, servicePrice);
    }

    protected virtual bool CheckCampaing(AdvertType adType)
    {
        return true;
    }

    public static bool CanCreditReferer(Member user, Member referer)
    {
        if (!TitanFeatures.IsRevolca)
            return true;

        if (referer.RevenueShareAdsWatchedYesterday < referer.Membership.AdPackDailyRequiredClicks)
            return false;

        var directRefs = referer.GetDirectReferralsList()
            .Where(r => r.Status == MemberStatus.Active)
            .OrderBy(r => r.Id).ToList();

        if (directRefs.Count < 7)
            return false;

        if (directRefs.Skip(6).Any(r => r.Id == user.Id))
            return true;

        return false;
    }

    public static List<Member> GetUnassignedMembers()
    {
        string query = string.Format(@"SELECT UserId, Username, MatrixId, ReferrerId FROM Users WHERE MatrixId IS NULL AND ReferrerId <> -1 ORDER BY RegisterDate ASC");
        return TableHelper.GetListFromRawQuery<Member>(query);
    }

    public static List<AdvertType> GetAvailableAdvertTypes()
    {
        List<AdvertType> campaigns = new List<AdvertType>();
        if (AppSettings.Matrix.MatrixCheckPtc)
            campaigns.Add(AdvertType.PTC);

        if (AppSettings.Matrix.MatrixCheckBanner)
            campaigns.Add(AdvertType.Banner);

        if (AppSettings.Matrix.MatrixCheckFacebook)
            campaigns.Add(AdvertType.Facebook);

        if (AppSettings.Matrix.MatrixCheckLogin)
            campaigns.Add(AdvertType.Login);

        if (AppSettings.Matrix.MatrixCheckTrafficGrid)
            campaigns.Add(AdvertType.TrafficGrid);

        if (AppSettings.Matrix.MatrixCheckAdPack)
            campaigns.Add(AdvertType.AdPack);

        if (AppSettings.Matrix.MatrixCheckExternalBanner)
            campaigns.Add(AdvertType.ExternalBanner);

        if (AppSettings.Matrix.MatrixCheckInText)
            campaigns.Add(AdvertType.InText);

        if (AppSettings.Matrix.MatrixCheckPtcOfferWall)
            campaigns.Add(AdvertType.PTCOfferWall);

        if (AppSettings.Matrix.MatrixCheckInvestmentPlatform)
            campaigns.Add(AdvertType.InvestmentPlan);

        return campaigns;
    }

    private static int GetLastLevel()
    {
        string query = "SELECT MAX(Cast(MatrixId AS HierarchyId).GetLevel()) from Users;";
        int lastLevel = (Int16)TableHelper.SelectScalar(query);
        return lastLevel;
    }

    public static KeyValuePair<int, int> GetLastLevelWithChildrenCount()
    {
        var lastLevelChildrenCount = new KeyValuePair<int, int>();
        int lastLevel = GetLastLevel();
        string query = string.Format(@"
			    DECLARE @DDC Table(C INT NOT NULL)
			    INSERT INTO @DDC 
				    SELECT COUNT(B.UserId) AS DirectDescendantsCount
				    FROM Users AS A 
						    LEFT OUTER JOIN Users AS B
							    ON CAST(A.MatrixId AS HierarchyId) = CAST(B.MatrixId AS HierarchyId).GetAncestor(1)
							    WHERE CAST(A.MatrixId AS HierarchyId).GetLevel() = {0}
				    GROUP BY A.MatrixId

			    SELECT SUM(C) FROM @DDC", lastLevel == 0 ? 0 : lastLevel - 1);

        object childrenObj = TableHelper.SelectScalar(query);

        if (childrenObj is DBNull)
            lastLevelChildrenCount = new KeyValuePair<int, int>(lastLevel, 0);
        else
            lastLevelChildrenCount = new KeyValuePair<int, int>(lastLevel, (int)childrenObj);

        return lastLevelChildrenCount;
    }
}