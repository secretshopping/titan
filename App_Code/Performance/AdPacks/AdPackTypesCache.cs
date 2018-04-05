using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class AdPackTypesCache : CacheBase
{
    protected override string Name { get { return "AdPackTypesCache"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(30); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.High; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }
    protected override object GetDataFromSource()
    {
        var types = AdPackTypeManager.GetAllTypes();
        var dictionary = new Dictionary<int, AdPackType>();

        foreach (var type in types)
            dictionary.Add(type.Id, type);

        return dictionary;
    }
}