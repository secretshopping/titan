<%@ WebHandler Language="C#" Class="BlocktrailHandlerAshx" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using Prem.PTC.Utils;
using Resources;
using Prem.PTC.Payments;
using System.IO;
using ExtensionMethods;
using Coinbase;
using Coinbase.ObjectModel;

public class BlocktrailHandlerAshx : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        BlocktrailHandler handler = new BlocktrailHandler(context, PaymentProcessor.Blocktrail);
        handler.Process();
    }

public bool IsReusable { get { return false; } }
}