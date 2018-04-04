using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class TotalViewOfPTCAdsCache : GlobalStatisticsCacheBase
{
    protected override string Name { get { return "TotalViewOfPTCAds"; } }

    protected override object GetDataFromSource()
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            string command = "SELECT SUM(TotalClicks) FROM Users";
            return Convert.ToInt32(bridge.Instance.ExecuteRawCommandScalar(command));
        }
    }
}