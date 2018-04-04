using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Collections;
using Prem.PTC.Members;

namespace Prem.PTC.Security
{
    public static class SecurityManager
    {

        /// <summary>
        /// Run after every successful login. May ban the user
        /// </summary>
        /// <param name="username"></param>
        public static void AfterLogin(Member User)
        {
            try
            {
                if (!IsOkWithRules(SecurityRuleType.IP, User.LastUsedIP))
                {
                    User.BanBlacklist(SecurityRuleType.IP.ToString());
                    User.SaveStatus();
                }
                if (!IsOkWithRules(SecurityRuleType.IPRange, User.LastUsedIP))
                {
                    User.BanBlacklist(SecurityRuleType.IPRange.ToString());
                    User.SaveStatus();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }

        /// <summary>
        /// Bans all users which match the security role
        /// </summary>
        /// <param name="Role"></param>
        /// <param name="Expression">Country name, IP address or username</param>
        public static void ActivateSecurityRole(SecurityRuleType Rule, string Expression)
        {
            List<Member> ToBeBanned = new List<Member>();
            var where = TableHelper.MakeDictionary("AccountStatusInt", (int)MemberStatus.Active);

            switch (Rule)
            {
                case SecurityRuleType.Country:
                    where.Add(Member.Columns.Country, Expression);
                    ToBeBanned = TableHelper.SelectRows<Member>(where);
                    break;

                case SecurityRuleType.IP:
                    where.Add(Member.Columns.LastUsedIP, Expression);
                    ToBeBanned = TableHelper.SelectRows<Member>(where);

                    where.Remove(Member.Columns.LastUsedIP);
                    where.Add(Member.Columns.RegisteredWithIP, Expression);
                    ToBeBanned.AddRange(TableHelper.SelectRows<Member>(where));
                    break;

                case SecurityRuleType.Username:
                    where.Add(Member.Columns.Username, Expression);
                    ToBeBanned = TableHelper.SelectRows<Member>(where);
                    break;

                case SecurityRuleType.IPRange:
                    var list = TableHelper.SelectRows<Member>(where);
                    foreach (var elem in list)
                    {
                        if (IPExtensions.IsInRange(elem.LastUsedIP, Expression) ||
                            IPExtensions.IsInRange(elem.RegisteredWithIP, Expression))
                            ToBeBanned.Add(elem);
                    }
                    break;
            }

            foreach (var user in ToBeBanned)
            {
                user.BanBlacklist(Rule.ToString());
                user.SaveStatus();
            }
        }

        public static bool IsOkWithRules(SecurityRuleType Rule, string Expression)
        {
            switch (Rule)
            {
                case SecurityRuleType.Country:
                    var list = TableHelper.SelectRows<SecurityRule>(TableHelper.MakeDictionary("Type", SecurityRuleType.Country.ToString()));
                    foreach (var rule in list)
                    {
                        if (rule.Value == Expression)
                            return false;
                    }
                    return true;

                case SecurityRuleType.IP:
                    list = TableHelper.SelectRows<SecurityRule>(TableHelper.MakeDictionary("Type", SecurityRuleType.IP.ToString()));
                    foreach (var rule in list)
                    {
                        if (rule.Value == Expression)
                            return false;
                    }
                    return true;

                case SecurityRuleType.Username:
                    list = TableHelper.SelectRows<SecurityRule>(TableHelper.MakeDictionary("Type", SecurityRuleType.Username.ToString()));
                    foreach (var rule in list)
                    {
                        if (rule.Value == Expression)
                            return false;
                    }
                    return true;

                case SecurityRuleType.IPRange:
                    list = TableHelper.SelectRows<SecurityRule>(TableHelper.MakeDictionary("Type", SecurityRuleType.IPRange.ToString()));
                    foreach (var rule in list)
                    {
                        if (IPExtensions.IsInRange(Expression, rule.Value))
                            return false;
                    }
                    return true;
            }
            return false;
        }

        public static void SetWatchingAdCookie(int seconds)
        {
            PTCSercurityManager.Lock(seconds);
        }

        public static bool IsWatchingAdCookieSet()
        {
            return PTCSercurityManager.IsLocked;
        }

        public static void SetWatchingAdSession(int seconds, int AdId)
        {
            string cookieName = "TIT_TEGE" + AdId;
            HttpContext.Current.Session.Add(cookieName, DateTime.Now.AddSeconds(seconds));
        }

        public static bool IsWatchingAdSessionSet(int AdId)
        {
            string cookieName = "TIT_TEGE" + AdId;

            if (HttpContext.Current.Session[cookieName] != null)
            {

                DateTime Finish = (DateTime)HttpContext.Current.Session[cookieName];
                if (Finish > DateTime.Now)
                    return true;
            }

            return false;
        }
    }

    public enum SecurityRuleType
    {
        Null = 0,
        Country = 1,
        IP = 2,
        Username = 3,

        IPRange = 4
    }



}