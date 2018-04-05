using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class OnlineUsersCache : GlobalStatisticsCacheBase
{
    protected override string Name { get { return "OnlineUsers"; } }
    protected override object GetDataFromSource()
    {
        try
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                string command = "SELECT COUNT(*) FROM Users WHERE LastLoginDate > @DATE ";
                object data = bridge.Instance.ExecuteRawCommandScalar(TableHelper.GetSqlCommand(command, DateTime.Now.AddHours(-3)));

                return Convert.ToInt32(data);
            }
        }
        catch (Exception ex)
        {
            return 0;
        }
    }
}