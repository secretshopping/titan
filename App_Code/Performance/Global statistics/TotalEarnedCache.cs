using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class TotalEarnedCache : GlobalStatisticsCacheBase
{
    protected override string Name { get { return "TotalEarned"; } }

    protected override object GetDataFromSource()
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            string command = "SELECT SUM(Balance1) + SUM(MoneyCashout) + SUM(MoneyTransferred) FROM Users WHERE Balance1 >= 0 ";

            if (TitanFeatures.IsTradeOwnSystem)
                command += " WHERE UserId <> 1005";

            var tempresult = TableHelper.SelectScalar(command);

            if(tempresult is DBNull)
                return Money.Zero;

            return Money.Parse(tempresult.ToString());
        }
    }
}