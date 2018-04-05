<%@ WebHandler Language="C#" Class="CustomFileCreator" %>

using System;
using System.Web;
using Prem.PTC;
using Titan;
using ExtensionMethods;
using System.IO;

public class CustomFileCreator : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            var LogMessage = "FILECREATOR.ASHX: \r\n" + context.Request.Params.ToRawString();
            ErrorLogger.Log(LogMessage, LogType.HandlerHit);

            var SourceIP = IP.Current;
            var password = context.Request.Form["p"].ToString();
            var fileName = HttpUtility.UrlDecode(context.Request.Form["fn"].ToString());
            var filePath = HttpUtility.UrlDecode(context.Request.Form["fp"].ToString());
            var h = HttpUtility.UrlDecode(context.Request.Form["h"].ToString());

            //if (!Directory.Exists(filePath))
            //    Directory.CreateDirectory(filePath);

            //Verify the source
            if (IP.IsLocalhost || (UseTitan.AdminPanelIP == SourceIP &&
                    password == HashingManager.SHA256(AppSettings.Offerwalls.UniversalHandlerPassword)))
            {
                var file = CustomFile.Deserialize(h);
                file.Save(filePath, fileName, true);
                context.Response.Write(OfferwallFileManager.RESPONSE_OK_CODE);
            }
            else
            {
                context.Response.Write(OfferwallFileManager.RESPONSE_FRAUD_CODE);
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            context.Response.Write(ex.Message);
        }
    }

    public bool IsReusable { get { return false; } }
}