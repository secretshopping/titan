using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExtensionMethods;
using Prem.PTC.Members;
using Prem.PTC;

/// <summary>
/// Summary description for LoginManager
/// </summary>
public class LoginManager
{
    public static bool LoginAdWatched
    {
        get
        {
            if (HttpContext.Current.Session == null || HttpContext.Current.Session["_LoginAdWatched"] == null)
                return false;
            return (bool)HttpContext.Current.Session["_LoginAdWatched"];
        }
        set
        {
            HttpContext.Current.Session["_LoginAdWatched"] = value;
        }
    }


    /// <summary>
    /// Returns a login ad that should be watched now
    /// </summary>
    /// <returns></returns>
    public static LoginAd GetLoginAd(Member user)
    {
        List<LoginAd> adslist;
        adslist = TableHelper.GetListFromRawQuery<LoginAd>(string.Format(@"SELECT * from LoginAds 
            WHERE Status = {0} 
            AND  CAST(floor(cast(DisplayDate as float)) as datetime) =  CAST(floor(cast({{fn NOW()}} as float)) as datetime)", (int)AdvertStatus.Active));



        List<LoginAd> geolocatedAds = (from elem in adslist
                                       where (!elem.IsGeolocated || (elem.IsGeolocated && elem.IsGeolocationMeet(user)))
                                       select elem).ToList();

        if (geolocatedAds.Count <= 0)
            return null;

        Random random = new Random();

        List<LoginAd> adToReturn = new List<LoginAd>();
        adToReturn.Add(geolocatedAds[random.Next(0, geolocatedAds.Count)]);

        return adToReturn[0];
    }

    public static int GetNumberOfAdsPurchasedForDay(DateTime date)
    {
        var ads = TableHelper.SelectScalar(string.Format(@"SELECT COUNT(*) FROM LoginAds WHERE CAST(floor(cast(DisplayDate as float)) as datetime) = '{0}' AND Status != {1}",
            date.ToShortDateDBString(), (int)AdvertStatus.Rejected));

        return Convert.ToInt32(ads);
    }

    public static void TryDisplayLoginAd(Member user)
    {
        //if watched or login ads disabled 
        if ((!AppSettings.TitanFeatures.AdvertLoginAdsEnabled && !AppSettings.LoginAds.IsAdflyEnabled) || LoginAdWatched)
            return;

        var adToDisplay = GetLoginAd(user);

        //if no ads available
        if (adToDisplay == null)
        {
            if (AppSettings.LoginAds.IsAdflyEnabled)
            {
                //Try AdFly
                LoginAdWatched = true;
                HttpContext.Current.Response.Redirect(String.Format("http://adf.ly/{0}/{1}", 
                    AppSettings.LoginAds.AdflyUserId, AppSettings.Site.Url + "user/"));
            }
            else
                return;
        }
        else
            HttpContext.Current.Response.Redirect("~/user/earn/surf.aspx?f=5");
    }

}
