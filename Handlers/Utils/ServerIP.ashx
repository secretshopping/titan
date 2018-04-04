<%@ WebHandler Language="C#" Class="ServerTimeHandler" %>

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

public class ServerTimeHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.Write(System.Web.HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"]);
    }

    public bool IsReusable { get { return false; } }
}