<%@ WebHandler Language="C#" Class="ThemeActivatorHandler" %>

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

public class ThemeActivatorHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            string LogMessage = "THEMEACTIVATOR.ASHX: \r\n" + context.Request.Params.ToRawString();
            ErrorLogger.Log(LogMessage, LogType.HandlerHit);

            //Force settings refresh
            AppSettings.Reload();

            string SourceIP = IP.Current;
            string password = context.Request.QueryString["p"].ToString();
            string theme = HttpUtility.UrlDecode(context.Request.QueryString["s"].ToString());

            //Verify the source
            if (password == HashingManager.SHA256(AppSettings.Offerwalls.UniversalHandlerPassword))
            {
                context.Response.Write(TryApplyNewTheme(theme));
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

    private bool TryApplyNewTheme(string theme)
    {
        bool IsOK = false;

        try
        {
            if (AppSettings.Site.Theme != theme)
            {
                //We are changing themes

                //1. Lets move defaut.aspx & Site.master of current theme to its folder
                FileMoveAndReplaceIfExists(HttpContext.Current.Server.MapPath("~/default.aspx"),
                    HttpContext.Current.Server.MapPath("~/Themes/" + AppSettings.Site.Theme + "/default.aspx"));

                FileMoveAndReplaceIfExists(HttpContext.Current.Server.MapPath("~/Sites.master"),
                    HttpContext.Current.Server.MapPath("~/Themes/" + AppSettings.Site.Theme + "/Sites.master"));

                //2. Lets update current theme to the new one
                AppSettings.Site.Theme = theme;
                AppSettings.Site.Save();

                //3. Lets move new theme files to official website
                FileMoveAndReplaceIfExists( HttpContext.Current.Server.MapPath("~/Themes/" + AppSettings.Site.Theme + "/default.aspx"),
                    HttpContext.Current.Server.MapPath("~/default.aspx"));

                FileMoveAndReplaceIfExists(HttpContext.Current.Server.MapPath("~/Themes/" + AppSettings.Site.Theme + "/Sites.master"),
                    HttpContext.Current.Server.MapPath("~/Sites.master"));

                IsOK = true;
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }

        return IsOK;
    }

    private void FileMoveAndReplaceIfExists(string fromPath, string toPath)
    {
        if (File.Exists(toPath))
            File.Delete(toPath);

        File.Move(fromPath, toPath);
    }

    public bool IsReusable { get { return false; } }
}