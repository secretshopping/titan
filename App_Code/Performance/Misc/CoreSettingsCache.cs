using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Prem.PTC.Memberships;

public class CoreSettingsCache : CacheBase
{
    protected override string Name { get { return "CoreSettings"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(5); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.Normal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        return TableHelper.SelectAllRows<CoreSettings>().FirstOrDefault();
    }
    public override object Get()
    {
        if (AppSettings.Side == ScriptSide.AdminPanel)
            return GetFromSession();
        else
            return GetFromCache();
    }
    
    private object GetFromCache()
    {
        if (ForceUpdate && HttpContext.Current.Cache[Name] != null)
            HttpContext.Current.Cache.Remove(Name);

        if (HttpContext.Current.Cache[Name] != null)
            return (object)HttpContext.Current.Cache[Name];

        object result = GetDataFromSource();

        HttpContext.Current.Cache.Insert(Name, result, null, DateTime.Now.Add(KeepCacheFor), Cache.NoSlidingExpiration,
            Priority, RemovedCallback);

        return result;
    }

    private object GetFromSession()
    {
        if (HttpContext.Current.Session["CoreSettings"] != null)
            return HttpContext.Current.Session["CoreSettings"];
        else
        {
            var settings = GetDataFromSource();
            HttpContext.Current.Session["CoreSettings"] = settings;
            return settings;
        }
    }
}