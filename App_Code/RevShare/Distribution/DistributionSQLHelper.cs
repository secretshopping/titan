using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Data;
using Prem.PTC.Advertising;
using MarchewkaOne.Titan.Balances;
using System.Text;

public class DistributionSQLHelper
{
    private Parser parser;
    private static string MaxMultiplier
    {
        get
        {
            return Math.Pow(10, CoreSettings.GetMaxDecimalPlaces()).
                ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"))
                .Replace(",","");
        }
    }
    public DistributionSQLHelper(Parser bridge)
    {
        this.parser = bridge;
    }
    /// <summary>
    /// Condition for AdPack to being treated as eligible for distribution
    /// </summary>
    private static string DistributionCondition
    {
        get
        {
            StringBuilder condition = new StringBuilder();
            condition.Append(@"
             SELECT ap.Id FROM AdPacks ap 
             JOIN Users u ON ap.UserId = u.UserId
             JOIN Memberships m ON u.UpgradeId = m.MembershipId
             JOIN AdPackTypes apt ON apt.Id = ap.AdPackTypeId
             WHERE
                ( 
	                ap.MoneyToReturn > ap.MoneyReturned AND
                    ( 
		                (u.RevenueShareAdsWatchedYesterday >= m.AdPackDailyRequiredClicks AND 
		                (u.AccountStatusInt = 1 OR u.AccountStatusInt = 9))
	                OR
		                (u.AccountStatusInt = 10)
                    )
                ) ");
            if (AppSettings.RevShare.StopRoiAfterMembershipExpiration)
                condition.Append("AND m.DisplayOrder >= (SELECT DisplayOrder FROM Memberships WHERE MembershipId = apt.RequiredMembership)");

            return condition.ToString();
        }
    }

    /// <summary>
    /// 1 to all eligible AdPacks
    /// 0 to other
    /// </summary>
    public void SetStartingDistributionPriority()
    {
        //Updates all adpacks
        string AdPacksUpdateCommnad = String.Format(@"
            UPDATE 
	            AdPacks 
            SET 
	            DistributionPriority = (
		            SELECT CASE WHEN 
		            (AdPacks.Id IN ({0})) 
                    THEN 
                        1 
                    ELSE 
                        0 
                    END
            )", DistributionCondition);

        parser.ExecuteRawCommandNonQuery(AdPacksUpdateCommnad);
    }

    public Money DistributeUsingPriority(Money amount, int AdPackTypeId)
    {

        //Updates all adpacks
        string AdPacksUpdateCommnad = String.Format(@"
            UPDATE AdPacks 
            SET 
                MoneyReturned = MoneyReturned + (FLOOR({0} * ({1} * DistributionPriority)) / {0}) 
            WHERE 
                DistributionPriority > 0 AND AdPackTypeId = {2}", MaxMultiplier, amount.ToClearString(), AdPackTypeId);

        //Update all member balances 
        string MemberBalanceUpdateCommand = String.Format(@"
			WITH CTE AS (
                SELECT 
	                P.UserId AS userId, 
                    SUM(FLOOR({0} * ({1} * DistributionPriority)) / {0}) AS amount
                FROM AdPacks P 
                WHERE 
                    DistributionPriority > 0 AND AdPackTypeId = {4}             
                GROUP BY P.UserId
            )
            UPDATE 
                Users 
            SET 
				Users.Balance3 = Users.Balance3 + {3},
                Users.Balance1 = Users.Balance1 + ({2} - {3}) * (1 - {5}), 
                Users.MarketplaceBalance = Users.MarketplaceBalance + ({2} - {3}) * {5},
                Users.TotalEarned = Users.TotalEarned + ({2} - {3}) * (1 - {5}),            
                Users.StatsEarned = cast(({2}+CONVERT(decimal(19,8),(CAST(SUBSTRING(Users.StatsEarned,0, CHARINDEX('#', Users.StatsEarned)) AS varchar(30))))) as varchar(1000)) + 
                    SUBSTRING(Users.StatsEarned,CHARINDEX('#', Users.StatsEarned), 1000),
                Users.StatsRevShareCurrentWeekIncome = Users.StatsRevShareCurrentWeekIncome + {2},
                Users.StatsRevShareCurrentMonthIncome = Users.StatsRevShareCurrentMonthIncome + {2}
            WHERE 
                Users.UserId IN (SELECT DISTINCT UserID from CTE)", MaxMultiplier, 
                                                                    amount.ToClearString(), 
                                                                    AmountToEarn, 
                                                                    AmountToRepurchaseBalance(AdPackTypeId), 
                                                                    AdPackTypeId, 
                                                                    MarketplaceBalanceReturnPercentage(AdPackTypeId));

        //BalanceLogs, add fast /MAIN BALANCE/
        string dtCommand = String.Format(@"
            SELECT 
	            P.UserId AS userId, 
                (" + AmountCalculated + @" - " + AmountCalculatedForRepurchase(AdPackTypeId) + @") * (1 - " + MarketplaceBalanceReturnPercentage(AdPackTypeId) + @") AS amount, 
                U.Balance1 AS state
            FROM AdPacks P 
            JOIN Users U
	        ON P.UserID = U.UserID
            WHERE 
                DistributionPriority > 0 AND AdPackTypeId = " + AdPackTypeId + @"
            GROUP BY P.UserId, U.Balance1, U.UpgradeId", amount.ToClearString());

        //BalanceLogs, add fast /PURCHASE BALANCE/
        string dtCommandAdBalance = String.Format(@"
			SELECT 
	            P.UserId AS userId, 
                " + AmountCalculatedForRepurchase(AdPackTypeId) + @" AS amount, 
                U.Balance3 AS state
            FROM AdPacks P 
            JOIN Users U
	        ON P.UserID = U.UserID
            WHERE 
                DistributionPriority > 0 AND AdPackTypeId = " + AdPackTypeId + @" 
            GROUP BY P.UserId, U.Balance3, U.UpgradeId", amount.ToClearString());

        string dtCommandMarketplaceBalance = String.Format(@"
            SELECT 
	            P.UserId AS userId, 
                (" + AmountCalculated + @" - " + AmountCalculatedForRepurchase(AdPackTypeId) + @") * " + MarketplaceBalanceReturnPercentage(AdPackTypeId) + @" AS amount, 
                U.MarketplaceBalance AS state
            FROM AdPacks P 
            JOIN Users U
	        ON P.UserID = U.UserID
            WHERE 
                DistributionPriority > 0 AND AdPackTypeId = " + AdPackTypeId + @"
            GROUP BY P.UserId, U.MarketplaceBalance, U.UpgradeId", amount.ToClearString());

        parser.ExecuteRawCommandNonQuery(MemberBalanceUpdateCommand);

        BalanceLogManager.AddRange(parser, dtCommand, AppSettings.RevShare.AdPack.AdPackName + " revenue", BalanceType.MainBalance, BalanceLogType.AdPackROI);
        BalanceLogManager.AddRange(parser, dtCommandAdBalance, AppSettings.RevShare.AdPack.AdPackName + " revenue", BalanceType.PurchaseBalance, BalanceLogType.AdPackROI);
        if(AppSettings.Payments.MarketplaceBalanceEnabled)
            BalanceLogManager.AddRange(parser, dtCommandMarketplaceBalance, AppSettings.RevShare.AdPack.AdPackName + " revenue", BalanceType.MarketplaceBalance, BalanceLogType.AdPackROI);

        parser.ExecuteRawCommandNonQuery(AdPacksUpdateCommnad);

        //How much distributed
        string selectDistributionAmount = String.Format(@"
            SELECT SUM(FLOOR({0} * ({1} * DistributionPriority)) / {0}) FROM AdPacks
            WHERE 
                DistributionPriority > 0 AND AdPackTypeId = {2}", MaxMultiplier, amount.ToClearString(), AdPackTypeId);

        object result = parser.ExecuteRawCommandScalar(selectDistributionAmount);

        if (result is DBNull)
            return Money.Zero;

        return Money.Parse(result.ToString());
    }

    public Decimal GetSumOfPriorities()
    {
        string Command = String.Format(@"
        SELECT CASE WHEN x IS NOT NULL THEN x ELSE 0.00 END
        FROM
        (
            SELECT SUM(DistributionPriority) AS x
            FROM AdPacks 
            WHERE DistributionPriority > 0
        ) AS T");

        return (Decimal)parser.ExecuteRawCommandScalar(Command);
    }

    public int GetSumOfActiveAdPacks()
    {
        return Convert.ToInt32(GetSumOfPriorities());
    }

    #region Custom Groups

    public void UpdatePrioritiesCustomGroups()
    {
        string Command = String.Format(@"
            UPDATE AdPacks
            SET DistributionPriority = DistributionPriority + (
	            SELECT t.Amount FROM AdPacks ap
                LEFT OUTER JOIN UserCustomGroups UCG
                   ON ap.UserCustomGroupID = UCG.ID
                LEFT OUTER JOIN CustomGroups CG
                   ON UCG.CustomGroupID = CG.ID
                CROSS APPLY
                    (
                       SELECT CASE WHEN CG.ID IS NULL
                            THEN 0
                            ELSE (CG.AcceleratedProfitPercentage) / 100.0
                        END AS [Amount]
                    ) t
	            WHERE ap.Id = AdPacks.Id
            )
            WHERE DistributionPriority > 0");

        parser.ExecuteRawCommandNonQuery(Command);
    }

    #endregion

    #region Automatic Groups

    public void UpdatePrioritiesAutomaticGroups()
    {
        //First check if there is at least one group 
        if (TableHelper.CountOf<AutomaticGroup>() > 0)
        {
            string Command = String.Format(@"
            UPDATE AdPacks
            SET DistributionPriority = DistributionPriority + (
	            				SELECT TOP(1) (AcceleratedProfitPercentage-100) / 100.0 
								FROM AutomaticGroups
								WHERE 
									(SELECT COUNT(Id) 
									FROM AdPacks ap
									WHERE 
									ap.DistributionPriority > 0 AND
									ap.UserId = AdPacks.UserId) <= AdPacksLimit 
									OR IsHighestGroup = 1
								ORDER BY AdPacksLimit ASC
            )
            WHERE DistributionPriority > 0");

            parser.ExecuteRawCommandNonQuery(Command);
        }
    }

    #endregion


    #region Helpers

    private static string AmountToEarn
    {
        get
        {
            return @"
             (Select amount from CTE WHERE userId = Users.UserId)
            ";
        }
    }

    private static string AmountToRepurchaseBalance(int AdPackTypeId)
    {
        return String.Format(@"
            (Select FLOOR({0} * (((SELECT AdPackAdBalanceReturnPercentage FROM Memberships WHERE MembershipId = Users.UpgradeId)/100.0) * 
            ((SELECT AdBalanceReturnPercentage FROM AdPackTypes WHERE Id = {2})/100.0) * {1})) / {0})
            ", MaxMultiplier, AmountToEarn, AdPackTypeId);
    }

    private static string MarketplaceBalanceReturnPercentage(int adPackTypeId)
    {
        return string.Format("(SELECT MarketplaceBalanceReturnPercentage/100.0 FROM AdPackTypes WHERE Id = {0})", adPackTypeId);
    }

    private static string AmountCalculated
    {
        get
        {
            return @"
             (SUM(FLOOR(" + MaxMultiplier + @" * ({0} * DistributionPriority)) / " + MaxMultiplier + @"))
            ";
        }
    }

    private static string AmountCalculatedForRepurchase(int AdPackTypeId)
    {
        return @"
             ((Select FLOOR(" + MaxMultiplier + @" * (((SELECT AdPackAdBalanceReturnPercentage FROM Memberships WHERE MembershipId = U.UpgradeId)/100.0) * 
             ((SELECT AdBalanceReturnPercentage FROM AdPackTypes WHERE Id = " + AdPackTypeId + ")/100.0) * " + AmountCalculated + "))) / " + MaxMultiplier + @")";
    }

    #endregion

}