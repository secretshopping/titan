using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class ReferralLeadershipManager
{
    static List<Member> Members;
    public static void CRON()
    {
        try
        {
            if (!AppSettings.TitanFeatures.ReferralsLeadershipEnabled)
                return;

            Members = TableHelper.GetListFromRawQuery<Member>(EligibleMembersQuery);
            RewardMembers();
            ResetMembers();
        }
        catch { }
    }

    private static string EligibleMembersQuery
    {
        get
        {
            return string.Format(@"SELECT * FROM Users 
WHERE AccountStatusInt = {0} 
AND UpgradeId = 
    (SELECT MembershipId FROM Memberships 
    WHERE DisplayOrder = (SELECT MAX(DisplayOrder) FROM Memberships) AND Status = {1})", (int)MemberStatus.Active, (int)MembershipStatus.Active);
        }
    }

    private static string GetUserIdsToReward()
    {
        List<int> userIdsToReward = new List<int>();
        List<Member> usersToRemove = new List<Member>();
        foreach(Member user in Members)
        {
            if (user.ShouldBeRewarded)
            {
                userIdsToReward.Add(user.Id);
                usersToRemove.Add(user);
            }
            Members = Members.Except(usersToRemove).ToList();
        }

        return string.Join(",", userIdsToReward);
    }

    private static string GetUserIdsToReset()
    {
        List<int> userIdsToReset = new List<int>();
        foreach (Member user in Members)
        {
            if (user.ShouldBeReset)
                userIdsToReset.Add(user.Id);
        }
        return string.Join(",", userIdsToReset);
    }

    private static void ResetMembers()
    {
        string idsToReset = GetUserIdsToReset();

        if (string.IsNullOrWhiteSpace(idsToReset))
            return;

        string resetQuery = string.Format(@"UPDATE Users SET LeadershipResetTime = GETDATE() WHERE UserId IN ({0});", idsToReset);

        TableHelper.ExecuteRawCommandNonQuery(resetQuery);
    }

    private static void RewardMembers()
    {
        string idsToReward = GetUserIdsToReward();

        if (string.IsNullOrWhiteSpace(idsToReward))
            return;

        string amountQuery = @"(SELECT Reward FROM ReferralLeaderShipLevels WHERE Number = (CASE Users.LeadershipLevelId WHEN -1 THEN 1 ELSE (SELECT Number + 1 FROM ReferralLeadershipLevels WHERE Id = Users.LeadershipLevelId) END ))";

        string rewardQuery = string.Format(@"UPDATE 
                Users 
            SET 
                Users.Balance1 = Users.Balance1 + {0},  
                Users.TotalEarned = Users.TotalEarned + {0},            
                Users.StatsEarned = cast(({0}+CONVERT(decimal(10,4),(CAST(SUBSTRING(Users.StatsEarned,0, CHARINDEX('#', Users.StatsEarned)) AS varchar(10))))) as varchar(1000)) + 
                    SUBSTRING(Users.StatsEarned,CHARINDEX('#', Users.StatsEarned), 1000)
            WHERE 
                Users.UserId IN ({1})", amountQuery, idsToReward);

        TableHelper.ExecuteRawCommandNonQuery(rewardQuery);

        string balanceLogHelperQuery = string.Format(@"SELECT 
			Users.UserId AS userId,
			{0} as amount,
			Users.Balance1 AS state
			FROM Users
            WHERE Users.UserId IN ({1})", amountQuery, idsToReward);

        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            BalanceLogManager.AddRange(bridge.Instance, balanceLogHelperQuery, "Leadership Reward", BalanceType.MainBalance, BalanceLogType.LeadershipReward);
        }

        string updateLeadershipIdQuery = string.Format(@"
        UPDATE Users 
        SET 
        Users.LeadershipLevelId = 
		        (SELECT Id FROM ReferralLeaderShipLevels 
		        WHERE Number = CASE Users.LeadershipLevelId WHEN -1 THEN 1 ELSE (SELECT NextNumber FROM ReferralLeadershipLevels WHERE Id = Users.LeadershipLevelId) END),
        Users.LeadershipResetTime = GETDATE()
        WHERE UserId IN ({0})", idsToReward);

        TableHelper.ExecuteRawCommandNonQuery(updateLeadershipIdQuery);
    }
}