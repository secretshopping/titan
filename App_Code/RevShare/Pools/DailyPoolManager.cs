using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using ExtensionMethods;

public class DailyPoolManager
{
    public DailyPoolManager()
    {

    }

    /// <summary>
    /// Add to N next pools, amount/n, starting from date
    /// </summary>
    /// <param name="type"></param>
    /// <param name="date"></param>
    /// <param name="amount"></param>
    /// <param name="parts"></param>
    public static void AddToPool(int poolId, DateTime date, Money amount)
    {
        int n = 1; //default value

        if (poolId == PoolsHelper.GetBuiltInProfitPoolId(Pools.AdPackRevenueReturn))
            n = AppSettings.RevShare.DailyPoolDurationTime;


        Money divided = new Money(amount.ToDecimal() / (Decimal)n);
        Money distributed = Money.Zero;

        DateTime currentDate = date.Date;

        for (int i = 0; i < n; i++)
        {
            var pool = DailyPool.Get(poolId, currentDate.Date);

            if (i == n - 1) //Last pool rule
                pool.SumAmount += (amount - distributed);
            else
                pool.SumAmount += divided;

            pool.Save();
            currentDate = currentDate.AddDays(1);
            distributed += divided;
        }
    }

    public static void CRON()
    {
        int poolId = PoolsHelper.GetBuiltInProfitPoolId(Pools.AdPackRevenueReturn);

        //Delete pools older then 2 days and move their money to current pools

        List<DailyPool> oldPools = TableHelper.GetListFromRawQuery<DailyPool>(String.Format(
            "SELECT * FROM DailyPools WHERE DateDay <= '{0}' AND DailyPoolType = {1}", AppSettings.ServerTime.AddDays(-2).ToDBString(), poolId));

        Money left = Money.Zero;

        foreach (var oldPool in oldPools)
            left += oldPool.SumAmount;

        if (left > Money.Zero)
            DailyPoolManager.AddToPool(poolId, AppSettings.ServerTime, left);

        TableHelper.ExecuteRawCommandNonQuery(String.Format(
            "DELETE FROM DailyPools WHERE DateDay <= '{0}' AND DailyPoolType = {1}", AppSettings.ServerTime.AddDays(-2).ToDBString(), poolId));
    }
}