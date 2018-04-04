using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Diagnostics;

/// <summary>
/// For performance tests
/// Written fast and bad, but works perfectly :-)
/// </summary>
public class ClassTimer
{
    private static string getLogger(HttpSessionState Session)
    {
        return (string)Session["theMFWatch223"];
    }

    private static void setLogger(HttpSessionState Session, string logger)
    {
        Session["theMFWatch223"] = logger;
    }

    private static Stopwatch getWatch(HttpSessionState Session)
    {
        return (Stopwatch)Session["theMFWatch22"];
    }

    private static void setWatch(HttpSessionState Session, Stopwatch watch)
    {
        Session["theMFWatch22"] = watch;
    }

    public static void Start()
    {
        setWatch(HttpContext.Current.Session, Stopwatch.StartNew());
        setLogger(HttpContext.Current.Session, "");
    }

    public static void Stop()
    {
        Stopwatch watch = getWatch(HttpContext.Current.Session);
        watch.Stop();
        setWatch(HttpContext.Current.Session, watch);
        AddPoint("END");
        Prem.PTC.ErrorLogger.Log(getLogger(HttpContext.Current.Session));
    }

    public static void AddPoint(string name)
    {
        string toLogger = getLogger(HttpContext.Current.Session);
        Stopwatch watch = getWatch(HttpContext.Current.Session);
        //toLogger += ;
        Prem.PTC.ErrorLogger.Log("Reached: " + name + " with time: " + watch.ElapsedMilliseconds + "ms \n");
        setLogger(HttpContext.Current.Session, toLogger);
    }
}