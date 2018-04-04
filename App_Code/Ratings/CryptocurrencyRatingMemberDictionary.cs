using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

public static class CryptocurrencyRatingMemberDictionary
{
    private static readonly string Name = "CryptocurrencyRatingMemberDictionaryCache";

    public static RatingMemberInfo Get(int userId)
    {
        Dictionary<int, RatingMemberInfo> dictionary;

        if(AppSettings.Side == ScriptSide.Client)
        {
            if (HttpContext.Current.Cache[Name] != null)
                dictionary = (Dictionary<int, RatingMemberInfo>)HttpContext.Current.Cache[Name];
            else
                dictionary = new Dictionary<int, RatingMemberInfo>();

            if (dictionary.ContainsKey(userId))
                return dictionary[userId];
            else
            {
                dictionary[userId] = CryptocurrencyPlatformManager.LoadNewUserData(userId); ;
                HttpContext.Current.Cache.Insert(Name, dictionary, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration,
                CacheItemPriority.Normal, null);

                return dictionary[userId];
            }
        }
        else
            return CryptocurrencyPlatformManager.LoadNewUserData(userId);

    }
}