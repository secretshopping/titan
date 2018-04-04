using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class TotalInTrafficBalanceCache : GlobalStatisticsCacheBase
{
    protected override string Name { get { return "TotalInTrafficBalance"; } }

    protected override object GetDataFromSource()
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            string command = "SELECT SUM(Balance2) FROM Users";
            string tempresult = bridge.Instance.ExecuteRawCommandScalar(command).ToString();
            return Money.Parse(tempresult);
        }
    }
}