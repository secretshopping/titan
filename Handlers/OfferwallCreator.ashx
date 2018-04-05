<%@ WebHandler Language="C#" Class="OfferwallCreator" %>

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

public class OfferwallCreator : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            string LogMessage = "OFFERWALLCREATOR.ASHX: \r\n" + context.Request.Params.ToRawString();
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
                    OfferwallFileManager.RemoveHandlerIAmOnClientSideNow(context, hash);
                }
                else
                {
                    //Create
                    OfferwallFileManager.CreateHandlerIAmOnClientSideNow(context, hash);
                }
                context.Response.Write(OfferwallFileManager.RESPONSE_OK_CODE);
            }
            else
            {
                context.Response.Write(OfferwallFileManager.RESPONSE_FRAUD_CODE);
            }
        }
        catch (Exception ex)
        {
            Prem.PTC.ErrorLogger.Log(ex);
            context.Response.Write(OfferwallFileManager.RESPONSE_ERROR_CODE);
        }
    }

    public bool IsReusable { get { return false; } }
}