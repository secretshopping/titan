using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using ExtensionMethods;
using Prem.PTC.Payments;

public class MemberManager
{
    public static void CRON()
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            //0. RevenueShareAdsWatched yesterday
            string RevenueShareAdsWatchedCommand = @"
            UPDATE Users 
            SET RevenueShareAdsWatchedYesterday = (
            CASE WHEN Users.RSAAdsViewed = '-1'
            THEN -1
            ELSE len(Users.RSAAdsViewed) - len(replace(Users.RSAAdsViewed, '#', ''))
            END
            ) + 1
            ";
            bridge.Instance.ExecuteRawCommandNonQuery(RevenueShareAdsWatchedCommand);
        }

        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            //1. Credit new Referral Reward
            CreditForNewReferrals();

            //2. Delete watched ads & TrafficGrid and other
            string resetDailyStatsCommand = "UPDATE Users SET " + Member.Columns.ViewedAds + " = '-1', "
                + Member.Columns.TrafficGridHitsToday + " = 0, PointsToday = 0, PointsCreditedForVideoToday = 0, PointsCreditedForSearchToday = 0, CPACompletedToday = 0, CompletedDailyCPAOffersToday = '-1', CompletedOffersFromOfferwallsToday = 0, MatrixBonusMoneyCyclesToday = 0, CompletedOffersMoreThan100pFromOfferwallsToday = 0, FbLikesToday = 0";

            if (AppSettings.RevShare.DistributionTime != DistributionTimePolicy.EveryWeek ||
                 AppSettings.ServerTime.DayOfWeek == AppSettings.RevShare.DayOfWeekDistribution)
            {
                resetDailyStatsCommand += ", RSAAdsViewed = '-1'";
            }

            bridge.Instance.ExecuteRawCommandNonQuery(resetDailyStatsCommand);

        }

        //3. Recalculate Statistics
        RecalculateStatistics(Member.Columns.StatsEarned, true);
        RecalculateStatistics(Member.Columns.StatsClicks, false);
        RecalculateStatistics(Member.Columns.UserClicksStats, false);
        RecalculateStatistics("RawDirectReferralsClicks", false);
        RecalculateStatistics("RawRentedReferralsClicks", false);
        RecalculateStatistics("StatsDirectReferralsEarned", true);
        RecalculateStatistics("StatsPointsEarned", false);
        RecalculateStatistics("StatsDirectReferralsPointsEarned", false);
        RecalculateStatistics("StatsDRAdPacksEarned", true);
        RecalculateStatistics("StatsCashLinksEarned", true);
        RecalculateStatistics("StatsDRCashLinksEarned", true);

        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            //4. Mark inactive ones
            string Command = string.Format(@"UPDATE Users SET {0} = {2}, AccountStatus = 'Expired'
            WHERE Username <> 'admin'
            AND {0} = {3} 
            AND (
		            (
			            (LastLoginDate IS NOT NULL 
			            AND DATEADD (day, {1}, LastLoginDate) < GETDATE()) 
		            OR 
			            (LastLoginDate IS NULL 
			            AND DATEADD (day, {1}, RegisterDate) < GETDATE())
		            )
	            AND (
			            (VacationModeEnds IS NULL) 
		            OR 
			            (DATEADD (day, {1}, VacationModeEnds) < GETDATE())
		            )
            )", "AccountStatusInt", AppSettings.Misc.DaysToInactivity, (int)MemberStatus.Expired, (int)MemberStatus.Active);

            bridge.Instance.ExecuteRawCommandNonQuery(Command);


            //5. Upgrade expiration
            var upgradeExpiredMembersQuery = "SELECT * FROM Users WHERE UpgradeExpires IS NOT NULL AND UpgradeExpires < GETDATE()";
            var upgradeExpiredMembers = TableHelper.GetListFromRawQuery<Member>(upgradeExpiredMembersQuery);
            foreach (Member user in upgradeExpiredMembers)
            {
                DateTime membershipExpires = (DateTime)user.MembershipExpires;
                var dateWhenReferralsWillBeResolved = membershipExpires.AddDays(AppSettings.Referrals.ResolveReferralsAfterSpecifiedDays);

                if (dateWhenReferralsWillBeResolved <= DateTime.Now)
                {
                    user.ResolveReferralsLimitDate = null;
                    user.Downgrade();
                }
                else
                {
                    user.ResolveReferralsLimitDate = dateWhenReferralsWillBeResolved;
                    user.Downgrade(false);
                }
                History.AddUpgradeExpiration(user.Name, user.MembershipName);
            }

            var usersWhoCouldHaveTooManyReferralsQuery = string.Format("SELECT * FROM {0} WHERE {1} IS NOT NULL AND {1} <= GETDATE()",
                Member.TableName, Member.Columns.ResolveReferralsLimitDate, AppSettings.Referrals.ResolveReferralsAfterSpecifiedDays);
            var usersWhoCouldHaveTooManyReferrals = TableHelper.GetListFromRawQuery<Member>(usersWhoCouldHaveTooManyReferralsQuery);
            foreach (var user in usersWhoCouldHaveTooManyReferrals)
            {
                user.ResolveReferralsLimitDate = null;
                user.SaveMembership();
                user.ResolveReferralLimits(user.Membership);
            }

            DateTime DateNow = DateTime.Now;

            //6 Vacation mode expiration
            string VacationModeExpiredCommand = "UPDATE Users SET LastActivityTime = GETDATE(), AccountStatusInt = " + (int)MemberStatus.Active + ", AccountStatus = 'Active' WHERE VacationModeEnds IS NOT NULL AND VacationModeEnds < GETDATE() AND AccountStatusInt = " + (int)MemberStatus.VacationMode;
            bridge.Instance.ExecuteRawCommandNonQuery(VacationModeExpiredCommand);

            //7. RemovalReferrals
            Member.DeleteReferralsCRON();

            //8. Inactivity fee: 
            if (AppSettings.VacationAndInactivity.InactivityChargePerDay > Money.Zero)
            {
                try
                {
                    string InactivityUpdateCommand = string.Format("UPDATE Users SET Balance1 = Balance1 - {0}", AppSettings.VacationAndInactivity.InactivityChargePerDay.ToClearString());
                    string InactivitySelectCommand = "SELECT * FROM Users";
                    string InactivityCondition = string.Format(" WHERE (LastActivityTime IS NOT NULL AND LastActivityTime < '{0}') AND AccountStatusInt = {1}",
                        DateNow.AddDays(-AppSettings.VacationAndInactivity.DaysToInactivityCharge).ToDBString(), (int)MemberStatus.Active);
                    BalanceLogManager.GlobalMemberAdjustHelper(bridge.Instance, InactivitySelectCommand, InactivityUpdateCommand, InactivityCondition, "Inactivity fee", AppSettings.VacationAndInactivity.InactivityChargePerDay, BalanceLogType.Other);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                }
            }

            if (DateNow.Day == 1)
                bridge.Instance.ExecuteRawCommandNonQuery("UPDATE Users SET PtcAutoSurfClicksThisMonth = 0;");

            DowngradeMembersLevels();

            //Automatically reject all payout requests from banned members
            //We are doing it to properly calculate 'CheckMaxValueOfPendingRequestsPerDay' for payment processors
            var PayoutRequestsToReject = TableHelper.GetListFromRawQuery<PayoutRequest>(PayoutRequest.GetPayoutRequestsSQLQuery(true));
            foreach (var request in PayoutRequestsToReject)
                PayoutManager.RejectRequest(request);
        }

        //9. Commissions Income Statistics
        //WEEKLY
        if (AppSettings.ServerTime.DayOfWeek == DayOfWeek.Sunday)
            TableHelper.ExecuteRawCommandNonQuery("UPDATE Users SET StatsCommissionsLastWeekIncome = StatsCommissionsCurrentWeekIncome, StatsCommissionsCurrentWeekIncome = 0");
        //MONTHLY
        if (AppSettings.ServerTime.Day == DateTime.DaysInMonth(AppSettings.ServerTime.Year, AppSettings.ServerTime.Month))
            TableHelper.ExecuteRawCommandNonQuery("UPDATE Users SET StatsCommissionsLastMonthIncome = StatsCommissionsCurrentMonthIncome, StatsCommissionsCurrentMonthIncome = 0");
    }

    private static void RecalculateStatistics(string columnName, bool isMoney)
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            bridge.Instance.ExecuteRawCommandNonQuery(TableHelper.GetRawRecalculateCommand(Member.TableName, columnName, isMoney));
        }
    }

    private static void CreditForNewReferrals()
    {
        var command = string.Format(@"
        BEGIN TRANSACTION
	        DECLARE @UsersToCredit TABLE (UserIdToCredit INT NOT NULL, ReferralId INT NOT NULL, MembershipId INT NOT NULL);
	        INSERT INTO @UsersToCredit 
		        SELECT A.UserId, B.UserId, A.UpgradeId AS Id 
		        FROM Users A 
		        JOIN Users B ON A.Userid = B.ReferrerId 
		        WHERE B.CreditedRefererReward = 0
			        AND B.IsRented = 0
			        AND (A.AccountStatusInt = {0} OR A.AccountStatusInt = {1})
			        AND B.TotalEarned >= (SELECT MinReferralEarningsToCreditReward FROM Memberships WHERE MembershipId = A.UpgradeId)
                    AND (SELECT NewReferralReward FROM Memberships WHERE MembershipId = A.UpgradeId) > 0;


	        DECLARE @IdToCredit INT;
	        DECLARE @ReferralId INT;
	        DECLARE @MembershipId INT;
	        DECLARE @SingleRow TABLE (UserIdToCredit INT NOT NULL, ReferralId INT NOT NULL, MembershipId INT NOT NULL);
	        WHILE EXISTS(SELECT * FROM @UsersToCredit)
		        BEGIN
			        INSERT INTO @SingleRow SELECT TOP 1 * FROM @UsersToCredit;
			        SELECT @IdToCredit = (SELECT TOP 1 UserIdToCredit FROM @SingleRow);
			        SELECT @ReferralId = (SELECT TOP 1 ReferralId FROM @SingleRow);
			        SELECT @MembershipId = (SELECT TOP 1 MembershipId FROM @SingleRow);
			        DECLARE @Reward DECIMAL(19,8);
			        SELECT @Reward = (SELECT NewReferralReward FROM Memberships WHERE MembershipId = @MembershipId);
			        print 'IdToCredit: ' + Convert(nvarchar, @IdToCredit);
			        print 'ReferralId: ' + Convert(nvarchar, @ReferralId);
			        print 'Reward: ' + Convert(nvarchar, @Reward);

			        DECLARE @AccountState DECIMAL(19,8);
			        SELECT @AccountState= (SELECT Balance1 FROM Users WHERE UserId = @IdToCredit);

			        UPDATE Users SET Balance1 = Balance1 + @Reward, TotalEarned = TotalEarned + @Reward, TotalDirectReferralsEarned = TotalDirectReferralsEarned + @Reward WHERE UserId = @IdToCredit;
			        UPDATE Users SET CreditedRefererReward = 1, TotalEarnedToDReferer = TotalEarnedToDReferer + @Reward WHERE UserId = @ReferralId;
			
			        INSERT INTO BalanceLogs VALUES(@IdToCredit, GetDate(), 'New Ref Reward', @Reward, {2}, @AccountState, {3})
			        DELETE FROM @UsersToCredit WHERE ReferralId = @ReferralId;
			        DELETE FROM @SingleRow;
		        END
        COMMIT TRANSACTION",
        (int)MemberStatus.Active, (int)MemberStatus.VacationMode, (int)BalanceType.MainBalance, (int)BalanceLogType.Other);
        TableHelper.ExecuteRawCommandNonQuery(command);
    }

    public static string getUsersProfileURL(string userName)
    {
        return string.Format("<a href=sites/profile.aspx?u={0}>{0}</a>", userName);
    }

    private static List<Member> GetUsersToDowngradeLevel()
    {
        string query = string.Format(@"
SELECT u.* FROM Users u 
JOIN Memberships m ON u.UpgradeId = m.MembershipId 
WHERE u.PtcSurfClicksThisMonth < m.MinAdsWatchedMonthlyToKeepYourLevel");

        var users = TableHelper.GetListFromRawQuery<Member>(query);

        return users;
    }

    /// <summary>
    /// Downgrades users who didn't watch enough PTC ads this month to the previous membership.
    /// Only if LevelMembershipPolicyEnabled
    /// </summary>
    private static void DowngradeMembersLevels()
    {
        if (AppSettings.Points.LevelMembershipPolicyEnabled)
        {
            foreach (var member in GetUsersToDowngradeLevel())
            {
                member.Downgrade(member.GetPreviousMembership());
                member.Save();
            }
        }
    }

    public static void AddReferrerToAllMembersWithoutReferrer(int referrerId)
    {
        var referrerName = Member.GetMemberUsername(referrerId);
        var query = string.Format("UPDATE {0} SET {1} = {2}, {3} = '{4}', ReferralSince = '{5}' WHERE {1} = -1 AND {6} != '{4}'",
            Member.TableName, Member.Columns.ReferrerId, referrerId, Member.Columns.Referer, referrerName, AppSettings.ServerTime.ToDBString(), Member.Columns.Username);

        var adminQuery = string.Format("UPDATE {0} SET {1} = -1, {2} = '' WHERE {3} = '{4}'",
            Member.TableName, Member.Columns.ReferrerId, Member.Columns.Referer, Member.Columns.Username, referrerName);

        TableHelper.ExecuteRawCommandNonQuery(query);
        TableHelper.ExecuteRawCommandNonQuery(adminQuery);
    }
}