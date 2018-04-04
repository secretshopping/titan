<%@ WebHandler Language="C#" Class="ImageCreator" %>

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

public class ImageCreator : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            string LogMessage = "IMAGECREATOR.ASHX: \r\n" + context.Request.Params.ToRawString();
            ErrorLogger.Log(LogMessage, LogType.HandlerHit);

            string SourceIP = IP.Current;
            string password = context.Request.Form["p"].ToString();
            string fileName = HttpUtility.UrlDecode(context.Request.Form["fn"].ToString());
            string filePath = HttpUtility.UrlDecode(context.Request.Form["fp"].ToString());
            string h = HttpUtility.UrlDecode(context.Request.Form["h"].ToString());

            //Verify the source
            if (IP.IsLocalhost || (UseTitan.AdminPanelIP == SourceIP &&
                    password == HashingManager.SHA256(AppSettings.Offerwalls.UniversalHandlerPassword)))
            {
                Banner image = Banner.Deserialize(h);
                image.Save(filePath, fileName, true);
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
            context.Response.Write(ex.Message);
        }
    }

    public bool IsReusable { get { return false; } }
}