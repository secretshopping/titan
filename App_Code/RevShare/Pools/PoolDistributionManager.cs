using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Titan;

public class PoolDistributionManager
{
    /// <summary>
    /// Distributes the money from the Profit Source properly
    /// </summary>
    /// <param name="source"></param>
    /// <param name="amount"></param>
    public static bool AddProfit(ProfitSource source, Money amount)
    {
        if (amount != Money.Zero)
        {
            var where = TableHelper.MakeDictionary(ProfitPoolDistribution.Columns.ProfitSource, (int)source);
            var targetPools = TableHelper.SelectRows<ProfitPoolDistribution>(where);

            Money distributed = new Money(0);

            for (int i = 0; i < targetPools.Count; i++)
            {
                var poolRule = targetPools[i];
                Money toDistribute = new Money(0);

                //Last pool rule
                if (i == (targetPools.Count - 1))
                    toDistribute = (amount - distributed);
                else
                {
                    toDistribute = Money.MultiplyPercent(amount, poolRule.ProfitPoolPercent);
                    distributed += toDistribute;
                }

                if (poolRule.Pool == PoolsHelper.GetBuiltInProfitPoolId(Pools.AdPackRevenueReturn))
                {
                    //Daily pool
                    DailyPoolManager.AddToPool(poolRule.Pool, AppSettings.ServerTime, toDistribute);
                    AdPackProfitDistributionHistory.Add(toDistribute);
                }
                else
                {
                    //Global pool
                    GlobalPoolManager.AddToPool(poolRule.Pool, toDistribute);
                }
            }
            return distributed != Money.Zero;
        }
        return false;
    }

    public static void AddProfitToSundayPool(Money amount)
    {
        try
        {
            var sundayPoolId = PoolsHelper.GetSundayPoolId();
            GlobalPoolManager.AddToPool(sundayPoolId, amount);
        }
        catch(Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    public static void SubtractProfit(ProfitSource source, Money amount)
    {
        AddProfit(source, amount.Negatify());
    }

    public static void AddProfit(ProfitSource source, Money amount, ProfitPoolDistribution targetPool)
    {
        if (targetPool.Pool == PoolsHelper.GetBuiltInProfitPoolId(Pools.AdPackRevenueReturn))
        {
            //Daily pool
            DailyPoolManager.AddToPool(targetPool.Pool, DateTime.Now, amount);
            AdPackProfitDistributionHistory.Add(amount);
        }
        else
        {
            //Global pool
            GlobalPoolManager.AddToPool(targetPool.Pool, amount);
        }
    }

    private static String GetGlobalPoolSum(int poolId)
    {
        String Query = String.Format("SELECT SumAmount FROM GlobalPools WHERE GlobalPoolType={0}", poolId);
        return TableHelper.SelectScalar(Query).ToString();
    }

    public static Money GetGlobalPoolSumInMoney(int poolId)
    {
        return Money.Parse(GetGlobalPoolSum(poolId));
    }
}