using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AccessManager
/// </summary>
public static class AccessManager
{
    private static readonly string DefaultRedirectUrl = "~/user/default.aspx";

    public static void RedirectIfDisabled(bool enabled)
    {
        //Blocking access to Earn pages (daily task not finished - force ads to watch)
        if (AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin>0 && Member.IsLogged && AppSettings.TitanFeatures.EarnTrafficExchangeEnabled)
        {
            String RequestPath = HttpContext.Current.Request.Url.AbsolutePath;

            if (RequestPath.StartsWith("/user/earn/") &&
               (Member.CurrentInCache.NumberOfWatchedTrafficAdsToday < AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin) &&
               (RequestPath != "/user/earn/surf.aspx"))
                HttpContext.Current.Response.Redirect(DefaultRedirectUrl);
        }

        if (!enabled)
            HttpContext.Current.Response.Redirect(DefaultRedirectUrl);
    }
}