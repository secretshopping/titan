using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class DatabaseConnectionPersister
{
    [Obsolete]
    public readonly static string DBConnectionErrorName = "DBConnectionErrorName";

    private readonly static string LastSuccessfulConnnection = "LastSuccessfulConnnection";
    private readonly static int ConnectionIdleInSeconds = 60;

    public static DateTime GetLastSuccessfulDatabaseConnection()
    {
        if (HttpContext.Current != null && HttpContext.Current.Application != null &&
            HttpContext.Current.Application[LastSuccessfulConnnection] != null)
            return (DateTime)HttpContext.Current.Application[LastSuccessfulConnnection];

        return DateTime.Now.AddMonths(-1);
    }

    public static void SetLastSuccessfulDatabaseConnection(DateTime dateTime)
    {
        if (AppSettings.Side == ScriptSide.AdminPanel)
            return;

        if (HttpContext.Current != null && HttpContext.Current.Application != null)
            HttpContext.Current.Application[LastSuccessfulConnnection] = dateTime;
    }

    public static void TryRestartApplication()
    {
        if (AppSettings.Side == ScriptSide.Client &&
            DatabaseConnectionPersister.GetLastSuccessfulDatabaseConnection().AddSeconds(ConnectionIdleInSeconds) < DateTime.Now)
            ApplicationRestarter.RestartAppPool();
    }
}