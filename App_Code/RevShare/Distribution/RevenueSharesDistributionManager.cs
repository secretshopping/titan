using System;
using Prem.PTC;

public class RevenueSharesDistributionManager
{
    public static void CRON()
    {
        if (AppSettings.RevShare.IsRevShareEnabled && !TitanFeatures.IsEpadilla &&
            (AppSettings.RevShare.DistributionTime != DistributionTimePolicy.EveryWeek 
            || AppSettings.ServerTime.DayOfWeek == AppSettings.RevShare.DayOfWeekDistribution))
        {
            AppSettings.RevShare.Reload();

            if (AppSettings.Misc.SpilloverEnabled)
                AdPackManager.FinishAdPacksIfNotUpgradedFor(7);

            if (TitanFeatures.StringSawSundayPool.HasValue && AppSettings.ServerTime.DayOfWeek == DayOfWeek.Sunday)
                StringSawSundayPool.CreditFromSundayPool();

            DailyDistribution(false, AppSettings.RevShare.DistributionTime == DistributionTimePolicy.EveryHour);

            AppSettings.RevShare.HourlyDistributionsMadeToday = 0;
            AppSettings.RevShare.Save();

            TableHelper.ExecuteRawCommandNonQuery("UPDATE AdPackTypes SET FixedDistributionValuePercent = FixedDistributionValuePercent2;");
            RevShareManager.RemoveOldPercentageValuesFromDatabase();

            if (TitanFeatures.IsTradeOwnSystem)
            {
                string AdPackConditionForTradeOwnSystem = @"
                    WITH CTE AS(
                        SELECT
                            UserId AS UserId, purchasedate AS purchasedate, ROW_NUMBER()OVER(PARTITION BY userId ORDER BY purchasedate) AS rownumber
                        FROM AdPacks
                        WHERE
                            MoneyToReturn > MoneyReturned
                    )
                    UPDATE Users SET FirstActiveDayOfAdPacks = GETDATE(), AdPackViewedCounter = 0 WHERE Users.UserId NOT IN (SELECT UserId from CTE);";

                TableHelper.ExecuteRawCommandNonQuery(AdPackConditionForTradeOwnSystem); 
            }

            //RevShare Income Statistics
            //WEEKLY
            if(AppSettings.ServerTime.DayOfWeek == DayOfWeek.Sunday)
                TableHelper.ExecuteRawCommandNonQuery("UPDATE Users SET StatsRevShareLastWeekIncome = StatsRevShareCurrentWeekIncome, StatsRevShareCurrentWeekIncome = 0");
            //MONTHLY
            if (AppSettings.ServerTime.Day == DateTime.DaysInMonth(AppSettings.ServerTime.Year, AppSettings.ServerTime.Month)) 
                TableHelper.ExecuteRawCommandNonQuery("UPDATE Users SET StatsRevShareLastMonthIncome = StatsRevShareCurrentMonthIncome, StatsRevShareCurrentMonthIncome = 0");
        }
    }

    /// <summary>
    /// When money is distributed every hour. RUN 23 times a day, the last run should be normal CRON.
    /// </summary>
    public static void CRON_EVERY_HOUR()
    {
        DailyDistribution(true, false);
    }

    private static void DailyDistribution(bool HourDistribution = false, bool FinalDistributionButHourly = false)
    {
        DailyPool Pool = DailyPool.GetYesterdayPool(PoolsHelper.GetBuiltInProfitPoolId(Pools.AdPackRevenueReturn));

        String RaportMessage = String.Empty;
        Int32 ActiveAdPacks = 0;
        Money TotalDistributed = Money.Zero;
        Money PerUnit = Money.Zero;
        Money InThePool = Pool.SumAmount;

        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            DistributionSQLHelper DistributionHelper = new DistributionSQLHelper(bridge.Instance);
            DistributionHelper.SetStartingDistributionPriority();
            ActiveAdPacks = DistributionHelper.GetSumOfActiveAdPacks();

            try
            {
                if (AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.Classic)
                {
                    //Classic
                    //Nothing to change
                }
                if (AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.CustomGroups ||
                    AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticAndCustomGroups)
                {
                    //CustomGrups 
                    if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Accelerate)
                        DistributionHelper.UpdatePrioritiesCustomGroups();
                }
                if (AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticGroups ||
                    AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticAndCustomGroups)
                {
                    //AutomaticGroups 
                    DistributionHelper.UpdatePrioritiesAutomaticGroups();
                }

                Decimal priorities = DistributionHelper.GetSumOfPriorities();

                if (ActiveAdPacks == 0)
                    throw new MsgException("No active AdPacks with active members. " + GetNoDistributionMessage(HourDistribution));

                //Make the distribution
                var adPackTypes = AdPackTypeManager.GetAllTypes();

                foreach (var adPackType in adPackTypes)
                {
                    var returnedPercentage = 0.0m;

                    if (AppSettings.RevShare.AdPack.DistributionPolicy == DistributionPolicy.Fixed)
                    {
                        PerUnit = GetMoneyPerUnit(GetMoneyPerUnitFixed(adPackType), HourDistribution, FinalDistributionButHourly, adPackType);
                        returnedPercentage = adPackType.FixedDistributionValuePercent;
                    }
                    else if (AppSettings.RevShare.AdPack.DistributionPolicy == DistributionPolicy.Pools)
                    {
                        PerUnit = GetMoneyPerUnit(GetMoneyPerUnitPools(InThePool, priorities), HourDistribution, FinalDistributionButHourly, adPackType);
                        returnedPercentage = PerUnit.ToDecimal() / adPackType.Price.ToDecimal();
                    }

                    RaportMessage += "<b>" + adPackType.Name + "</b> for priority 1.00 (no acceleration): <b>" + PerUnit.ToClearString() + "</b>. <br/>";

                    if (PerUnit > Money.Zero)
                        TotalDistributed += DistributionHelper.DistributeUsingPriority(PerUnit, adPackType.Id);

                    RevShareManager.AddAdPackTypePercentageHistory(adPackType.Id, returnedPercentage);
                }
                if (TitanFeatures.isAri)
                    AriRevShareDistribution.CreditAriRevShareDistribution();
            }
            catch (MsgException ex)
            {
                RaportMessage += ex.Message;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }

            Pool.SumAmount -= TotalDistributed;

            if (HourDistribution == false)
            {
                if (AppSettings.RevShare.AdPack.DistributionPolicy == DistributionPolicy.Pools)
                    RaportMessage += "Money moved to the next day pool: " + Pool.SumAmount.ToClearString() + ". ";

                Pool.MoveMoneyForTomorrow();
            }

            Pool.Save();

            Report.Add(ActiveAdPacks, InThePool, TotalDistributed, RaportMessage);

            CustomGroupManager.TrySetGroupsAsExpired();
        }
    }

    private static string GetNoDistributionMessage(bool HourDistribution)
    {
        if (HourDistribution)
            return "No distribution made this hour. ";

        return "Money moved to the next pool and no distribution made this day. ";
    }

    private static Money GetMoneyPerUnit(Money PerUnit, bool HourDistribution, bool FinalDistributionButHourly, AdPackType adPackType)
    {
        if (AppSettings.RevShare.AdPack.DistributionPolicy == DistributionPolicy.Pools)
        {
            var maxRoi = Money.MultiplyPercent(Money.MultiplyPercent(adPackType.Price, adPackType.PackReturnValuePercentage),
                            AppSettings.RevShare.AdPack.MaxDailyROIPercent);

            if (PerUnit > maxRoi)
                PerUnit = maxRoi;
        }

        //Hour distribution?
        if (HourDistribution)
            PerUnit = new Money(PerUnit.ToDecimal() / (Decimal)(24));

        //Hour but final
        if (FinalDistributionButHourly)
            PerUnit = PerUnit - (AppSettings.RevShare.HourlyDistributionsMadeToday * (new Money(PerUnit.ToDecimal() / (Decimal)(24))));

        return PerUnit;
    }

    private static Money GetMoneyPerUnitPools(Money InThePool, Decimal priorities)
    {
        return new Money(InThePool.ToDecimal() / priorities);
    }

    private static Money GetMoneyPerUnitFixed(AdPackType type)
    {
        return new Money(type.Price.ToDecimal() * (type.FixedDistributionValuePercent / (Decimal)100));
    }

}