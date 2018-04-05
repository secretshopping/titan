using System;
using System.Configuration;
using Prem.PTC.Advertising;
using Prem.PTC.Utils;
using System.Reflection;
using System.Web;
using System.Diagnostics;
using System.Net;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        /// <summary>
        /// Get total members of the website
        /// </summary>
        public static int TotalMembers
        {
            get
            {
                var cache = new TotalMembersCache();
                return (int)cache.Get();
            }
        }

        //Gets current server time of client website
        public static DateTime ServerTime
        {
            get
            {
                if (Side == ScriptSide.Client)
                    return DateTime.Now;

                return DateTime.Now.AddHours(AppSettings.Misc.ServerTimeDifference);
            }
        }

        /// <summary>
        /// Gets Server IP address (of client website)
        /// </summary>
        public static string ServerIP
        {
            get
            {
                if (AppSettings.Side == ScriptSide.Client)
                    return HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];
                else
                {
                    try
                    {
                        using (WebClient client = new MyWebClient())
                        {
                            return client.DownloadString(AppSettings.Site.Url + "Handlers/Utils/ServerIP.ashx");
                        }
                    }
                    catch (Exception ex)
                    {
                        return "unknown";
                    }
                }
            }
        }

        /// <summary>
        /// Get number of online users
        /// </summary>
        public static int OnlineUsers
        {
            get
            {
                var cache = new OnlineUsersCache();
                return (int)cache.Get();
            }
        }

        public static string LastJoinedUser
        {
            get
            {
                var cache = new LastJoinedUserCache();
                return cache.Get().ToString();
            }
        }


        public static Money TotalCashout
        {
            get
            {
                var cache = new TotalCashoutCache();
                return (Money)cache.Get();
            }
        }

        /// <summary>
        /// Get total members registered today 
        /// </summary>
        public static int TotalMembersRegisteredToday
        {
            get
            {
                var cache = new TotalMembersRegisteredTodayCache();
                return (int)cache.Get();
            }
        }

        /// <summary>
        /// Get total earned money (not cashout) from the website
        /// </summary>
        public static Money TotalEarned
        {
            get
            {
                var cache = new TotalEarnedCache();
                return (Money)cache.Get();
            }
        }

        /// <summary>
        /// Get total main balance from users
        /// </summary>
        public static Money TotalInMainBalance
        {
            get
            {
                var cache = new TotalInMainBalanceCache();
                return (Money)cache.Get();
            }
        }

        /// <summary>
        /// Get total advertising balance from users
        /// </summary>
        public static Money TotalInAdBalance
        {
            get
            {
                var cache = new TotalInAdBalanceCache();
                return (Money)cache.Get();
            }
        }

        /// <summary>
        /// Get total traffic balance from users
        /// </summary>
        public static Money TotalInTrafficBalance
        {
            get
            {
                var cache = new TotalInTrafficBalanceCache();
                return (Money)cache.Get();
            }
        }

        public static int TotalViewOfPTCAds
        {
            get
            {
                var cache = new TotalViewOfPTCAdsCache();
                return (int)cache.Get();
            }
        }


        public static int TotalAdPackAdsViews
        {
            get
            {
                var cache = new TotalAdPackAdsViewsCache();
                return (int)cache.Get();
            }
        }

        public static int AllActiveCustomGroups
        {
            get
            {
                var cache = new AllActiveCustomGroupsCache();
                return (int)cache.Get();
            }
        }


        /// <summary>
        /// Including PTC & Banners
        /// </summary>
        public static int ExposuresServedYesterday
        {
            get
            {
                var cache = new ExposuresServedYesterdayCache();
                return (int)cache.Get();
            }
        }

        public static Money InvestmentPlatformTotalPaidIn
        {
            get
            {
                var cache = new InvestmentPlatformTotalPaidInCache();
                return (Money)cache.Get();
            }
        }
    }
}