<%@ WebHandler Language="C#" Class="CPACreator" %>

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
using Titan;
using ExtensionMethods;

public class CPACreator : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            string LogMessage = "CPACREATOR.ASHX: \r\n" + context.Request.Params.ToRawString();
            ErrorLogger.Log(LogMessage, LogType.HandlerHit);

            //Force settings refresh
            AppSettings.Offerwalls.Reload();

            string SourceIP = IP.Current;
            string password = context.Request.QueryString["p"].ToString();
            string hash = context.Request.QueryString["h"].ToString();

            //Verify the source
            if (password == HashingManager.GenerateSHA256(AppSettings.Offerwalls.UniversalHandlerPassword))
            {
                //All OK, proceed
                if (context.Request.QueryString["r"] != null &&
                    context.Request.QueryString["r"] == "1")
                {
                    //Remove
                    CPAFileManager.RemoveHandlerIAmOnClientSideNow(context, hash);
                }
                else
                {
                    //Create
                    CPAFileManager.CreateHandlerIAmOnClientSideNow(context, hash);
                }
                context.Response.Write(CPAFileManager.RESPONSE_OK_CODE);
            }
            else
            {
                context.Response.Write(CPAFileManager.RESPONSE_FRAUD_CODE);
            }
        }
        catch (Exception ex)
        {
            Prem.PTC.ErrorLogger.Log(ex);
            context.Response.Write(CPAFileManager.RESPONSE_ERROR_CODE);
        }
    }

    public bool IsReusable { get { return false; } }
}