using Prem.PTC;
using Prem.PTC.Members;
using System.Collections.Generic;
using System.Data;
using Titan;

public class LastJoinedUserCache : GlobalStatisticsCacheBase
{
    protected override string Name { get { return "LastJoinedUser"; } }

    protected override object GetDataFromSource()
    {
        using(var bridge = ParserPool.Acquire(Database.Client))
        {
            string command = "SELECT TOP 1 [Username] FROM [Users] ORDER BY [RegisterDate] DESC";
            return bridge.Instance.ExecuteRawCommandScalar(command).ToString();
        }
    }
}