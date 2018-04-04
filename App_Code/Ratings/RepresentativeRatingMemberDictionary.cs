using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

public static class RepresentativeRatingMemberDictionary
{
    private static readonly string Name = "RepresentativeRatingMemberDictionary";

    public static RatingMemberInfo Get(int userId)
    {
        if (AppSettings.Side == ScriptSide.Client)
        {
            Dictionary<int, RatingMemberInfo> dictionary;
            if (HttpContext.Current.Cache[Name] != null)
                dictionary = (Dictionary<int, RatingMemberInfo>)HttpContext.Current.Cache[Name];
            else
                dictionary = new Dictionary<int, RatingMemberInfo>();

            if (dictionary.ContainsKey(userId))
                return dictionary[userId];
            else
            {
                dictionary[userId] = Representative.LoadNewUserData(userId);
                HttpContext.Current.Cache.Insert(Name, dictionary, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration,
                CacheItemPriority.Normal, null);

                return dictionary[userId];
            }
        }
        else
            return Representative.LoadNewUserData(userId);
    }
}