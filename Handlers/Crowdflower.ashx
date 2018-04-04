<%@ WebHandler Language="C#" Class="Crowdflower" %>

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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Linq;
using System.Collections;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

public class Crowdflower : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        CrowdflowerHandler.ProcessRequest(context);
    }

    public bool IsReusable { get { return false; } }
}