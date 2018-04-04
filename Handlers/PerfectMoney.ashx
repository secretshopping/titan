<%@ WebHandler Language="C#" Class="PerfectMoney" %>

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

public class PerfectMoney : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        PerfectMoneyHandler.ProcessRequest(context);
    }

    public bool IsReusable { get { return false; } }
}