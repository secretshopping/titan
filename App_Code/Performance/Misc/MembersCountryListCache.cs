using Prem.PTC.Members;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Web.Caching;

class MembersCountryListCache : CacheBase
{
    protected override string Name { get { return "MembersCountryListCache"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(10); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.Normal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override bool CachedInAdminPanel { get { return true; } }

    protected override object GetDataFromSource()
    {
        ResourceSet resourceSet = Resources.Countries.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        SortedDictionary<string, int> data = new SortedDictionary<string, int>();

        foreach (DictionaryEntry entry in resourceSet)
        {
            data.Add(entry.Value.ToString(), 0);
        }

        var query = String.Format(@"SELECT [Country], COUNT([UserId]) AS [Count] FROM Users WHERE {0} GROUP BY [Country]", MemberStatusHelper.PotentiallyActiveStatusesSQLHelper);
        var list = TableHelper.GetListFromRawQuery<GeolocationDictionary>(query);        

        foreach(var item in list)
        {
            data[item.Country] = item.Count;
        }

        return data;
    }
}