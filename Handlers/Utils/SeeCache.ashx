<%@ WebHandler Language="C#" Class="SeeCacheHandler" %>

using System;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Reflection;
using Prem.PTC.Utils.NVP;
using Prem.PTC.Payments;
using Prem.PTC.Members;
using Prem.PTC;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;

public class SeeCacheHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        string result = String.Empty;

        var r = new Dictionary<string, string>();

        using (var pc = new PerformanceCounter("ASP.NET Applications", "Cache % Process Memory Limit Used", true))
        {
            pc.InstanceName = "__Total__";
            r.Add("Total_ProcessMemoryUsed", String.Concat(pc.NextValue().ToString("N1"), "%"));
        }
        using (var pc = new PerformanceCounter("ASP.NET Applications", "Cache API Entries", true))
        {
            pc.InstanceName = "__Total__";
            r.Add("Total_Entries", pc.NextValue().ToString("N0"));
        }
        using (var pc = new PerformanceCounter("ASP.NET Applications", "Cache API Misses", true))
        {
            pc.InstanceName = "__Total__";
            r.Add("Total_Misses", pc.NextValue().ToString("N0"));
        }
        using (var pc = new PerformanceCounter("ASP.NET Applications", "Cache API Hit Ratio", true))
        {
            pc.InstanceName = "__Total__";
            r.Add("Total_HitRatio", String.Concat(pc.NextValue().ToString("N1"), "%"));
        }
        using (var pc = new PerformanceCounter("ASP.NET Applications", "Cache API Trims", true))
        {
            pc.InstanceName = "__Total__";
            r.Add("Total_Trims", pc.NextValue().ToString());
        }

        foreach (var elem in r)
        {
            result += elem.Key + ": " + elem.Value + "<br/>";
        }

        context.Response.Write(result);
    }

    public bool IsReusable { get { return false; } }
}