using Newtonsoft.Json.Linq;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;

public class RevShareManager
{
    #region Instant accurals
    public static bool CheckInstantAccrualsRequirement()
    {
        var query = string.Format("SELECT COUNT(*) FROM {0} WHERE {1} = {2}",
                AdPackType.TableName, AdPackType.Columns.Status, (int)AdPackTypeStatus.Active);
        var activeTypesCount = Convert.ToInt32(TableHelper.SelectScalar(query));

        if (!AppSettings.RevShare.IsRevShareEnabled ||
            AppSettings.RevShare.AdPack.DistributionPolicy != DistributionPolicy.Fixed ||
            AppSettings.RevShare.DistributionTime != DistributionTimePolicy.EveryHour ||
            activeTypesCount != 1)
        {
            AppSettings.RevShare.AdPack.InstantAccrualsEnabled = false;
            AppSettings.RevShare.AdPack.Save();
            return false;
        }

        return true;
    }

    public static void AddAdPackTypePercentageHistory(int adPackTypeId, decimal percentage)
    {
        //prevent adding duplicates
        var checkQuery = string.Format("SELECT COUNT(*) FROM {0} WHERE {1} = {2} AND CAST({3} AS DATE) = TRY_CONVERT(DATETIME, '{4}', 102)",
            AdPackTypesDailyReturnedPercentValue.TableName, AdPackTypesDailyReturnedPercentValue.Columns.AdPackTypeId, adPackTypeId,
            AdPackTypesDailyReturnedPercentValue.Columns.Date, AppSettings.ServerTime.Date);

        if ((int)TableHelper.SelectScalar(checkQuery) > 0)
            return;

        var newRecord = new AdPackTypesDailyReturnedPercentValue
        {
            AdPackTypeId = adPackTypeId,
            Date = AppSettings.ServerTime.Date,
            PercentageValue = percentage
        };

        newRecord.Save();
    }

    /// <summary>
    /// Fired at Daily CRON
    /// </summary>
    public static void RemoveOldPercentageValuesFromDatabase()
    {
        try
        {
            //We want to have records from last 10 days
            var query = string.Format("DELETE FROM {0} WHERE {1} < TRY_CONVERT(DATETIME, '{2}', 102)",
                AdPackTypesDailyReturnedPercentValue.TableName, AdPackTypesDailyReturnedPercentValue.Columns.Date, AppSettings.ServerTime.AddDays(-10));

            TableHelper.ExecuteRawCommandNonQuery(query);
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    public static Money GetEarningFromDate(int userId, DateTime date)
    {
        if (!AppSettings.RevShare.AdPack.InstantAccrualsEnabled)
            return Money.Zero;

        var query = string.Format("SELECT SUM(AMOUNT) FROM {0} WHERE [BalanceLogType] = {1} AND [UserId] = {2} AND CAST([DateOccured] AS DATE) = TRY_CONVERT(DATETIME, '{3}', 102)",
            BalanceLog.TableName, (int)BalanceLogType.AdPackROI, userId, date.Date);

        var result = TableHelper.SelectScalar(query);
        
        return result == DBNull.Value ? Money.Zero : new Money(Convert.ToDecimal(TableHelper.SelectScalar(query)));
    }

    public static decimal GetAdPackDailyReturnedPercentage(int adPacktypeId, DateTime date)
    {
        if (!AppSettings.RevShare.AdPack.InstantAccrualsEnabled)
            return 0.0m;

        var query = string.Format("SELECT {0} FROM {1} WHERE {2} = {3} AND CAST([{4}] AS DATE) = TRY_CONVERT(DATETIME, '{5}', 102)",
            AdPackTypesDailyReturnedPercentValue.Columns.PercentageValue, AdPackTypesDailyReturnedPercentValue.TableName, AdPackTypesDailyReturnedPercentValue.Columns.AdPackTypeId,
            adPacktypeId, AdPackTypesDailyReturnedPercentValue.Columns.Date, date.Date);

        var result = TableHelper.SelectScalar(query);
        return result == DBNull.Value ? 0.0m : Convert.ToDecimal(TableHelper.SelectScalar(query));
    }

    private static int GetAdPackTypeId()
    {
        var query = string.Format("SELECT {0} FROM {1} WHERE {2} = {3}",
            AdPackType.Columns.Id, AdPackType.TableName, AdPackType.Columns.Status, (int)AdPackTypeStatus.Active);

        return Convert.ToInt32(TableHelper.SelectScalar(query));
    }

    private static int GetAdpacksCount(int userId, int adPackTypeId)
    {
        var query = string.Format("SELECT COUNT(Id) FROM {0} WHERE MoneyReturned < MoneyToReturn AND UserId = {1} AND AdPackTypeId = {2}",
            AdPack.TableName, userId, adPackTypeId);

        return Convert.ToInt32(TableHelper.SelectScalar(query));
    }

    public static string GetJsonWithInstantAccrualsValues(int userId, int fromDays = 9)
    {
        CheckInstantAccrualsRequirement();
        if (!AppSettings.RevShare.AdPack.InstantAccrualsEnabled)
            return string.Empty;

        //max 10 days = 9 + today
        if (fromDays > 9)
            fromDays = 9;

        var adPackTypeId = GetAdPackTypeId();
        var array = new JArray();
        var myDate = AppSettings.ServerTime.AddDays(-fromDays);
        var commissionPerSeconds = 0m;

        while (AppSettings.ServerTime.Date > myDate.Date)
        {
            var date = myDate.Date;
            var earnings = GetEarningFromDate(userId, date);
            var percentage = GetAdPackDailyReturnedPercentage(adPackTypeId, date);
            array.Add(GetJObject(date, earnings, percentage));
            myDate = myDate.AddDays(1);
        }

        //Current Day
        var user = new Member(userId);
        var adpacksCount = GetAdpacksCount(user.Id, adPackTypeId);
        if (user.RevenueShareAdsWatchedYesterday >= user.Membership.AdPackDailyRequiredClicks && adpacksCount > 0)
        {
            AdPackType adPackType;
            var cache = new AdPackTypesCache();
            var adPackTypes = (Dictionary<int, AdPackType>)cache.Get();
            adPackTypes.TryGetValue(adPackTypeId, out adPackType);
            

            var date = myDate.Date;
            var percentage = GetAdPackDailyReturnedPercentage(adPackTypeId, date);
            commissionPerSeconds = (adPackType.Price.ToDecimal() * (percentage / 100) / 24 / 3600) * adpacksCount;
            var time = (AppSettings.ServerTime.Minute * 60) + AppSettings.ServerTime.Second;
            var earnings = GetEarningFromDate(userId, date);

            array.Add(GetJObject(date, earnings, percentage));            
        }
        else        
            array.Add(GetJObject(myDate.Date, Money.Zero, 0.0m));
        
        array.Add(new JObject(new JProperty("earningPerSecond", commissionPerSeconds)));
        return array.ToString();
    }

    private static JObject GetJObject(DateTime date, Money money, decimal percentage)
    {
        return new JObject(new JProperty("date", String.Format("{0:yyyy-MM-dd}", date)), new JProperty("money", money.ToDecimal()), new JProperty("percentage", percentage));
    }
    #endregion
}