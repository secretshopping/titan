using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SessionManager
/// </summary>
public static class SessionManager
{
    public static void ClearCookiesAfterLogout()
    {
        ClearCookie(ShoutboxCommands.cookieName);
        ClearCookiesStartingWith(NotificationManager.CookieName);
    }

    private static void ClearCookie(string cookieName)
    {
        if (HttpContext.Current.Request.Cookies[cookieName] != null)
            HttpContext.Current.Response.Cookies[cookieName].Expires = DateTime.Now.AddYears(-5);
    }

    private static void ClearCookiesStartingWith(string cookieName)
    {
        var cookies = HttpContext.Current.Request.Cookies.AllKeys;

        foreach(var cookieKey in cookies)
        {
            if (cookieKey.ToString().StartsWith(cookieName))
                HttpContext.Current.Response.Cookies[cookieKey.ToString()].Expires = DateTime.Now.AddYears(-5);
        }
    }
}