using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class EarningsStatsManager
{
	public static Money GetEarnings(EarningsStatsType type, DateTime day)
	{
        DailyEarning Earning = DailyEarning.GetProperObject(type, day.Date);
        return Earning.SumAmount;
	}

    public static void Add(EarningsStatsType type, Money amount)
    {
        DailyEarning Earning = DailyEarning.GetProperObject(type, DateTime.Now.Date);

        //Add & save
        Earning.SumAmount += amount;
        Earning.Save();
    }

    public static void Subtract(EarningsStatsType type, Money amount)
    {
        Add(type, amount * -1);
    }
}

