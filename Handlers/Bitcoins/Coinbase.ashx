<%@ WebHandler Language="C#" Class="CoinbaseHandlerAshx" %>

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

public class CoinbaseHandlerAshx : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        CoinbaseHandler handler = new CoinbaseHandler(context, PaymentProcessor.Coinbase);
        handler.Process();
    }

public bool IsReusable { get { return false; } }
}