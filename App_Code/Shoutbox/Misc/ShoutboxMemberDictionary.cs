using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

public static class ShoutboxMemberDictionary
{
    private static readonly string Name = "ShoutboxMemberDictionaryCache";

    public static ShoutboxMemberInfo Get(string username)
    {
        Dictionary<string, ShoutboxMemberInfo> dictionary;

        if (HttpContext.Current.Cache[Name] != null)
            dictionary = (Dictionary<string, ShoutboxMemberInfo>)HttpContext.Current.Cache[Name];
        else
            dictionary = new Dictionary<string, ShoutboxMemberInfo>();

        if (dictionary.ContainsKey(username))
            return dictionary[username];
        else
        {
            Member user = new Member(username);
            var info = new ShoutboxMemberInfo(user.AvatarUrl, user.CountryCode, user.FormattedName, user.ShoutboxPrivacyPermission,
                user.IsForumAdministrator);
            dictionary[username] = info;

            HttpContext.Current.Cache.Insert(Name, dictionary, null, DateTime.Now.AddMinutes(15), Cache.NoSlidingExpiration,
            CacheItemPriority.Normal, null);

            return info;
        }
    }
}