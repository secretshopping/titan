using Prem.PTC.Members;
using System;
using System.Web;

public class ReferrerUtils
{
    public static string GetReferrerName()
    {
        try
        {
            var cookies = HttpContext.Current.Request.Cookies;
            var myCookie = cookies["ReferrerCookie"];

            if (myCookie == null || string.IsNullOrEmpty(myCookie.Value))
                return null;

            var cookieValue = myCookie.Value;
            int userId;
            var isNumber = Int32.TryParse(cookieValue, out userId);

            if (!isNumber)
                return cookieValue;

            return new Member(userId).Name;
        }
        catch(Exception ex)
        {
            return String.Empty;
        }
    }

    public static void SetReferrer(int userId)
    {
        SetCookieReferrer(userId.ToString());
    }

    public static void SetReferrer(string userName)
    {
        SetCookieReferrer(userName);
    }

    public static void SetReferrer(Member user)
    {
        SetCookieReferrer(user.Name);
    }

    private static void SetCookieReferrer(string refCookieValue)
    {
        var context = HttpContext.Current;
        var referrerCookie = new HttpCookie("ReferrerCookie", refCookieValue)
        {
            Expires = DateTime.Now.AddDays(7)
        };
        context.Response.Cookies.Add(referrerCookie);
    }
}