using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;
using Prem.PTC.Memberships;

/// <summary>
/// Summary description for TrafficExchangeManager
/// </summary>
public static class TrafficExchangeManager
{
    public static List<TrafficExchangeAdvert> GetAdsAvailableForUser(Member user)
    {
        var watchedLongAgoEnoughAds = TableHelper.GetListFromRawQuery<TrafficExchangeAdvert>(string.Format(@"SELECT * FROM TrafficExchangeAdverts tea 
            JOIN TrafficExchangeViews tev ON tea.TrafficExchangeAdvertId = tev.TrafficExchangeAdvertId
            LEFT JOIN TrafficExchangeAdvertPacks teap ON tea.TrafficExchangeAdvertPackId = teap.TrafficExchangeAdvertPackId 
            WHERE tev.LastDisplayDate < (
	            CASE WHEN teap.TrafficExchangeAdvertPackId IS NULL 
	            THEN DATEADD (mi,-tea.TimeBetweenViewsInMinutes,{{fn NOW()}}) 
	            ELSE DATEADD (mi,-teap.TimeBetweenViewsInMinutes,{{fn NOW()}}) END)
            AND tev.UserId = {0}
            AND tea.Status = {1}
            AND (tea.MinMembershipID IN ({2}) OR tea.MinMembershipID = 0)
            ORDER BY tev.LastDisplayDate", user.Id, (int)AdvertStatus.Active, Membership.GetSqlQuerryForMembershipsIdListUnderCurrentMembership()));

        var neverWatchedAds = TableHelper.GetListFromRawQuery<TrafficExchangeAdvert>(string.Format(@"SELECT * FROM TrafficExchangeAdverts tea 
            WHERE TrafficExchangeAdvertId NOT IN (SELECT TrafficExchangeViews.TrafficExchangeAdvertId FROM TrafficExchangeViews WHERE TrafficExchangeViews.UserId = {0}) 
            AND tea.Status = {1} AND (tea.MinMembershipID IN ({2}) OR tea.MinMembershipId = 0)", user.Id, (int)AdvertStatus.Active, Membership.GetSqlQuerryForMembershipsIdListUnderCurrentMembership()));

        neverWatchedAds.AddRange(watchedLongAgoEnoughAds);

        List<TrafficExchangeAdvert> geolocatedAds = (from elem in neverWatchedAds
                                                     where (!elem.IsGeolocated || (elem.IsGeolocated && elem.GeolocatedCountries.Contains(user.CountryCode)))
                                                     select elem).ToList();

        geolocatedAds.Shuffle();

        return geolocatedAds;
    }

    public static void AddAdvertToRecentlyWatched(Member user, int adId)
    {
        //Should only return 1 record
        var oldViews = GetViewedAdvert(user.Id, adId);
        TrafficExchangeView teView;
        if (oldViews.Count == 1)
            teView = new TrafficExchangeView(oldViews[0].Id);
        else
            teView = new TrafficExchangeView();

        teView.UserId = user.Id;
        teView.DisplayDate = DateTime.Now;
        teView.TrafficExchangeAdvertId = adId;
        teView.Save();
    }

    public static List<TrafficExchangeView> GetViewedAdvert(int userId, int adId)
    {
        List<TrafficExchangeView> userViews;

        userViews = TableHelper.GetListFromRawQuery<TrafficExchangeView>(string.Format(@"SELECT * FROM TrafficExchangeViews tev 
            WHERE tev.UserId = {0}
            AND tev.TrafficExchangeAdvertId = {1}", userId, adId));

        return userViews;
    }

    public static void CRON()
    {
        var maxTimeBetweenViews = TableHelper.SelectScalar("SELECT MAX(TimeBetweenViewsInMinutes) FROM TrafficExchangeAdvertPacks");
        if (maxTimeBetweenViews is DBNull)
            return;
        
        string deleteQuery = string.Format("DELETE FROM TrafficExchangeViews WHERE LastDisplayDate < DATEADD (mi,-{0},GETDATE())", (int)maxTimeBetweenViews);
        TableHelper.ExecuteRawCommandNonQuery(deleteQuery);

        String ClearUsers = String.Format("UPDATE Users SET AmountOfWatchedTrafficAdsToday=0");
        TableHelper.ExecuteRawCommandNonQuery(ClearUsers);
    }

}