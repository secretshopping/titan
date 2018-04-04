<%@ WebHandler Language="C#" Class="PermissionCheckerHandler" %>

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

public class PermissionCheckerHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            string LogMessage = "PERMISSIONCHECKER.ASHX: \r\n" + context.Request.Params.ToRawString();
            ErrorLogger.Log(LogMessage, LogType.HandlerHit);

            //Force settings refresh
            AppSettings.Offerwalls.Reload();

            string SourceIP = IP.Current;
            string password = context.Request.QueryString["p"].ToString();
            string source = HttpUtility.UrlDecode(context.Request.QueryString["s"].ToString());

            //Verify the source
            if (password == HashingManager.SHA256(AppSettings.Offerwalls.UniversalHandlerPassword))
            {
                context.Response.Write(FileCreateCheck(source));
            }
            else
            {
                context.Response.Write("REQUEST NOT ALLOWED");
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    private bool FileCreateCheck(string path)
    {
        bool IsOK = false;
        try
        {
            File.Create(HttpContext.Current.Server.MapPath(path + "testfile.txt")).Close();
            File.Delete(HttpContext.Current.Server.MapPath(path + "testfile.txt"));
            IsOK = true;
        }
        catch (Exception ex)
        { }

        return IsOK;
    }

    public bool IsReusable { get { return false; } }
}