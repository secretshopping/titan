using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.IO;
using System.Diagnostics;

public class ApplicationRestarter
{
    public static bool RestartAppPool()
    {
        ErrorLogger.Log("RESTARTING THE APPLICATION");

        //First try killing your worker process
        try
        {
            //Get the current process
            Process process = Process.GetCurrentProcess();
            // Kill the current process
            process.Kill();
            // if your application have no rights issue then it will restart your app pool
            return true;
        }
        catch (Exception ex)
        {
            //if exception occoured then log exception
            ErrorLogger.Log("Restart Request Failed. Exception details :-" + ex);
        }

        //Try unloading appdomain
        try
        {
            //note that UnloadAppDomain requires full trust
            HttpRuntime.UnloadAppDomain();
            return true;
        }
        catch (Exception ex)
        {
            //if exception occoured then log exception
            ErrorLogger.Log("Restart Request Failed. Exception details :-" + ex);
        }

        //Finally automating the dirtiest way to restart your application pool

        //get the path of web.config
        string webConfigPath = HttpContext.Current.Request.PhysicalApplicationPath + "\\web.config";
        try
        {
            //Change the last modified time and it will restart pool
            File.SetLastWriteTimeUtc(webConfigPath, DateTime.UtcNow);
            return true;
        }
        catch (Exception ex)
        {
            //if exception occoured then log exception
            ErrorLogger.Log("Restart Request Failed. Exception details :-" + ex);
        }

        //Still no hope, you have to do something else.
        return false;

    }
}