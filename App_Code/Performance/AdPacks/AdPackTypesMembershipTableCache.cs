using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class AdPackTypesMembershipTableCache : CacheBase
{
    protected override string Name { get { return "AdPackTypesMembershipTableCache"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(30); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.Low; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }
    protected override object GetDataFromSource()
    {
        return AdPackTypeMembershipMapper.Mapper.GetHtml();
    }
}