using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Prem.PTC;

//The only class, which uses cache and do not derive from CacheBase
//is Member (Member.CurrentInCache)

public abstract class CacheBase
{
    protected abstract string Name { get; }
    protected abstract TimeSpan KeepCacheFor { get; }
    protected abstract CacheItemPriority Priority { get; }
    protected abstract CacheItemRemovedCallback RemovedCallback { get; }
    protected virtual bool CachedInAdminPanel { get { return false; } }

    public bool ForceUpdate { get; set; }

    public virtual object Get()
    {
        if (AppSettings.Side == ScriptSide.AdminPanel && CachedInAdminPanel && HttpContext.Current.Session != null)
        {
            if (HttpContext.Current.Session[Name] == null)
                HttpContext.Current.Session[Name] = GetDataFromSource();

            return HttpContext.Current.Session[Name];
        }

        if (AppSettings.Side == ScriptSide.AdminPanel || AppSettings.Site.DeveloperModeEnabled)
            return GetDataFromSource(); // No caching on Admin Panel side

        if (ForceUpdate && HttpContext.Current.Cache[Name] != null)
            HttpContext.Current.Cache.Remove(Name);

        if (HttpContext.Current.Cache[Name] != null)
            return (object)HttpContext.Current.Cache[Name];

        object result = GetDataFromSource();

        HttpContext.Current.Cache.Insert(Name, result, null, DateTime.Now.Add(KeepCacheFor), Cache.NoSlidingExpiration,
            Priority, RemovedCallback);

        return result;
    }

    protected abstract object GetDataFromSource();
}