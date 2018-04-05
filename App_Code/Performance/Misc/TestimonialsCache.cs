using Prem.PTC;
using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class TestimonialsCache : CacheBase
{
    protected override string Name { get { return "ActiveTestimonials"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(5); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.BelowNormal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }
    protected override object GetDataFromSource()
    {
        return Testimonial.Get(TestimonialStatus.Active);
    }
}