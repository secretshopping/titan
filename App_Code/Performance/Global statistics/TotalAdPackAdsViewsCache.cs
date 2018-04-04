using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class TotalAdPackAdsViewsCache : GlobalStatisticsCacheBase
{
    protected override string Name { get { return "TotalAdPackAdsViews"; } }
    protected override object GetDataFromSource()
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            string command = "SELECT SUM(TotalClicks) FROM AdPacks";
            object tempresult = bridge.Instance.ExecuteRawCommandScalar(command);

            if (tempresult is DBNull)
                return 0;

            return Convert.ToInt32(tempresult);
        }
    }
}