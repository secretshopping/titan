using LukeSkywalker.IPNetwork;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using Titan;

public class GoogleCaptchaJSCache : CacheBase
{
    protected override string Name { get { return "GoogleCaptchaJSCache"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(15); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.High; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    protected override object GetDataFromSource()
    {
        string result = String.Empty;

        using (var client = new MyWebClient())
        {
            try
            {
                result = client.DownloadString("https://www.google.com/recaptcha/api.js?hl=en");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }

        return result;
    }
}