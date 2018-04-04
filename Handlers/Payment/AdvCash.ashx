﻿<%@ WebHandler Language="C#" Class="AdvCash" %>

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

public class AdvCash : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        PaymentHandler handler = new AdvCashHandler(context, PaymentProcessor.AdvCash);
        handler.Process();
    }

    public bool IsReusable { get { return false; } }
}