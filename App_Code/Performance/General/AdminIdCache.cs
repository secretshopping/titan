using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Web.Caching;

public class AdminIdCache : CacheBase
{
    protected override string Name { get { return "AdminId"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(60); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.High; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        Member admin = new Member(AppSettings.RevShare.AdminUsername);
        return admin.Id;
    }
}