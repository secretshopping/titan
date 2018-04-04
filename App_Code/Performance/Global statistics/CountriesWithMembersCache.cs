using Prem.PTC.Members;
using System;

public class CountriesWithMembersCache : GlobalStatisticsCacheBase
{
    protected override string Name { get { return "CountriesWithMembersCache"; } }
    protected override object GetDataFromSource()
    {     
        var query = string.Format("SELECT DISTINCT [Code] FROM Users WHERE [Code] != '-'");
        return TableHelper.GetStringListFromRawQuery(query);
    }
}