using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class AllActiveCustomGroupsCache : GlobalStatisticsCacheBase
{
    protected override string Name { get { return "AllActiveCustomGroups"; } }

    protected override object GetDataFromSource()
    {
        string query = string.Format(@"SELECT COUNT(*)  FROM UserCustomGroups ucg 
            WHERE ucg.Status = {0};", (int)CustomGroupStatus.Active);
        return (int)TableHelper.SelectScalar(query);
    }
}