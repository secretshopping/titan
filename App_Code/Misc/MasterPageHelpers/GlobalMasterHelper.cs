using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using System.Web.Security;
using System.Web.UI;

public class GlobalMasterHelper
{
    public static void PageLoad()
    {
        HttpContext context = HttpContext.Current;
        Page page = (Page)context.Handler;

        //If cron runs, deny all actions
        if (AppSettings.IsOffline)
            context.Response.Redirect("~/offline.html");

        if (DateTime.Now < AppSettings.Site.MaintenanceModeEndsDate)
        {
            AppSettings.Site.Reload();
            context.Response.Redirect("~/maintenance.aspx");
        }

        if (!page.IsPostBack)
        {
            //Refreshing AppSettings once/3 minutes
            if (LastAppSettingsRefreshTime < DateTime.Now.AddMinutes(-3) || AppSettings.Site.DeveloperModeEnabled)
            {
                LastAppSettingsRefreshTime = DateTime.Now;
                AppSettings.Reload();
            }

            //Maybe apply referer?
            //FOR CORRECT REFERER SOURCE
            //Applies to all members
            if (context.Session["ReferralFrom"] == null)
            {
                if (context.Request.UrlReferrer != null)
                    context.Session["ReferralFrom"] = context.Request.UrlReferrer.Host;
                else
                    context.Session["ReferralFrom"] = "Unknown";
            }

        }
    }

    public static void LogoutIfBanned()
    {
        //Check if member is not banned meantime
        if (Member.IsLogged && Member.CurrentInCache.IsBanned)
        {
            HttpContext context = HttpContext.Current;
            Member.CurrentInCache.Logout(HttpContext.Current.Response);
            FormsAuthentication.SignOut();
            context.Session.Abandon();
            context.Response.Redirect("status.aspx?type=logoutsuspended&id=logoutsus");
        }
    }


    private const string Application_Name = "AppSettingsRefreshed";

    public static DateTime LastAppSettingsRefreshTime
    {
        get
        {
            if (HttpContext.Current.Application[Application_Name] == null)
                HttpContext.Current.Application[Application_Name] = DateTime.Now.AddDays(-10); //Never

            return (DateTime)HttpContext.Current.Application[Application_Name];
        }
        set
        {
            HttpContext.Current.Application[Application_Name] = value;
        }
    }
}