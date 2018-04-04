<%@ WebHandler Language="C#" Class="AdzbuzzOAuthHandler" %>

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
using ExtensionMethods;
using Titan.CustomFeatures;

public class AdzbuzzOAuthHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            //string LogMessage = "AdzbuzzOAuth.ASHX: \r\n" + context.Request.ToRawString();
            //ErrorLogger.Log(LogMessage, LogType.HandlerHit);

            if (context.Request.QueryString["code"] != null && context.Request.QueryString["state"] != null)
            {
                AdzbuzzOAuth.GetTokenAndLogin(context.Request.QueryString["code"].ToString(),
                    context.Request.QueryString["state"].ToString());
            }
            else
            {
                string key = HashingManager.SHA256(DateTime.Now + AppSettings.Offerwalls.UniversalHandlerPassword ).ToLower();
                var systemRedirectUri = AdzbuzzOAuth.GetRedirectUrl(key);  
                context.Response.Redirect(systemRedirectUri);
            }

        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            context.Response.Redirect("~/user/default.aspx?afterlogin=1");
        }
    }

    public bool IsReusable { get { return false; } }
}