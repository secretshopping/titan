using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Titan;
using Titan.Advertising;
using Titan.Shares;

public class GlobalPoolManager
{
    private Money amount;
    private Member user;

    public GlobalPoolManager(Money amount, Member user)
    {
        this.amount = amount;
        this.user = user;
    }

    public static void AddToPool(int poolId, Money amount)
    {
        var pool = GlobalPool.Get(poolId);
        pool.SumAmount += amount;
        pool.Save();
    }

    public static void SubtractFromPool(int poolId, Money amount)
    {
        AddToPool(poolId, amount * -1);
    }
}