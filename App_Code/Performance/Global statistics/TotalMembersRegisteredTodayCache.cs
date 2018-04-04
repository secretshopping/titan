using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class TotalMembersRegisteredTodayCache : GlobalStatisticsCacheBase
{
    protected override string Name { get { return "TotalMembersRegisteredToday"; } }

    protected override object GetDataFromSource()
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            string command = "SELECT COUNT(*) FROM Users WHERE RegisterDate BETWEEN DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0)" +
                " AND DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 1)";
            return int.Parse(bridge.Instance.ExecuteRawCommandScalar(command).ToString());
        }
    }
}