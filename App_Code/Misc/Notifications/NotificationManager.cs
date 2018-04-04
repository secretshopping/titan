using System;
using System.Collections.Generic;
using System.Linq;
using Prem.PTC.Members;
using Prem.PTC.Advertising;
using Prem.PTC;
using Prem.PTC.Offers;
using System.Web;
using Titan;
using SocialNetwork;

public class NotificationManager
{
    private static readonly int RefreshEveryMinutes = 15;
    public static readonly string CookieName = "Notifications";

    #region Public Fields

    public static int Get(NotificationType type)
    {
        if (!Member.IsLogged)
            return 0;

        switch (type)
        {
            case NotificationType.NewAds:
                if (AppSettings.TitanFeatures.EarnAdsEnabled)
                    return GetNotificationValue(type);
                return 0;
                
            case NotificationType.NewAdPacksAds:
                if (AppSettings.TitanFeatures.EarnAdPacksEnabled)
                    return GetNotificationValue(type);
                return 0;

            case NotificationType.NewMessages:
                if (AppSettings.TitanFeatures.PeopleMessagesEnabled)
                    return GetNotificationValue(type);
                return 0;

            case NotificationType.NewDirectReferrals:
                if (AppSettings.TitanFeatures.ReferralsDirectEnabled)
                    return GetNotificationValue(type);
                return 0;

            case NotificationType.NewAchievements:
                if (AppSettings.TitanFeatures.TrophiesEnabled)
                    return GetNotificationValue(type);
                return 0;

            case NotificationType.NewSummaryInfos:
                return GetNotificationValue(type);

            case NotificationType.NewCPAOffers:
                if (AppSettings.TitanFeatures.EarnCPAGPTEnabled)
                    return GetNotificationValue(type);
                return 0;

            case NotificationType.UnreadSupportTickets:
                return GetNotificationValue(type);

            case NotificationType.NewFacebookAds:
                if (AppSettings.TitanFeatures.EarnLikesEnabled)
                    return GetNotificationValue(type);
                return 0;

            case NotificationType.UnassignedMatrixMembers:
                if (AppSettings.TitanFeatures.ReferralMatrixEnabled && AppSettings.Matrix.Type == MatrixType.Referral)
                    return GetNotificationValue(type);
                return 0;

            case NotificationType.PendingRepresentativePaymentRequest:
                if(AppSettings.TitanFeatures.IsRepresentativesEnabled && (AppSettings.Representatives.RepresentativesHelpDepositEnabled || AppSettings.Representatives.RepresentativesHelpWithdrawalEnabled))
                    return GetNotificationValue(type);
                return 0;

            default:
                return 0;
        }
    }

    public static int GetAccountSum()
    {
        return Get(NotificationType.NewDirectReferrals) + Get(NotificationType.NewMessages)
            + Get(NotificationType.NewSummaryInfos) + Get(NotificationType.NewAchievements);
    }

    public static int GetEarnSum()
    {
        return Get(NotificationType.NewFacebookAds) + Get(NotificationType.NewAds) + Get(NotificationType.NewCPAOffers);
    }

    public static void Refresh(NotificationType type)
    {
        RefreshWithMember(type, null);
    }

    public static void RefreshWithMember(NotificationType type, Member user)
    {
        string cookieName = CookieName + (int)type;

        int result = 0;

        result = RefreshAndGetValue(type, user);

        HttpContext.Current.Response.Cookies[cookieName].Value = result.ToString();
        HttpContext.Current.Response.Cookies[cookieName].Expires = DateTime.Now.AddMinutes(RefreshEveryMinutes);
    }

    public static void SpotAllDirectReferrals()
    {
        TableHelper.ExecuteRawCommandNonQuery("UPDATE Users SET IsSpotted = 1 WHERE ReferrerId = " + Member.CurrentId);
    }

    #endregion

    #region Logic

    private static int GetNotificationValue(NotificationType type)
    {
        string cookieName = CookieName + (int)type;

        if (HttpContext.Current.Request.Cookies[cookieName] != null)
            return Convert.ToInt32(HttpContext.Current.Request.Cookies[cookieName].Value);

        int result = RefreshAndGetValue(type);

        HttpContext.Current.Response.Cookies[cookieName].Value = result.ToString();
        HttpContext.Current.Response.Cookies[cookieName].Expires = DateTime.Now.AddMinutes(RefreshEveryMinutes);

        return result;
    }

    private static int RefreshAndGetValue(NotificationType type, Member user = null)
    {
        if (!Member.IsLogged)
            return 0;

        if (user == null)
            user = Member.CurrentInCache;

        switch (type)
        {
            case NotificationType.NewAds:

                List<PtcAdvert> AvailableAdList = PtcAdvert.GetActiveAdsForUser(user);
                int newAdsCounter = 0;

                foreach (PtcAdvert Ad in AvailableAdList)
                {
                    if (!user.AdsViewed.Contains(Ad.Id))
                        newAdsCounter++;
                }
                return newAdsCounter;

            case NotificationType.NewMessages:
                return ConversationMessage.GetNumberOfUnreadMessages(Member.CurrentId);

            case NotificationType.NewDirectReferrals:

                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    return (int)bridge.Instance.ExecuteRawCommandScalar("SELECT COUNT (*) FROM Users WHERE IsSpotted = 0 AND ReferrerId = "
                        + Member.CurrentId);
                }

            case NotificationType.NewAchievements:

                return user.UnspottedAchievements;

            case NotificationType.NewCPAOffers:
                //CPAOFfers
                //User is already loaded
                var OM = new OffersManager(user);
                return OM.AllActiveOffersForMember.Count;

            case NotificationType.UnreadSupportTickets:

                    return (int)TableHelper.SelectScalar(String.Format("SELECT COUNT(*) FROM SupportTickets WHERE IsRead = 0 AND FromUsername = '{0}'",
                        Member.CurrentName));
                

            case NotificationType.NewAdPacksAds:

                List<AdPacksAdvert> AdPacksAdList = AdPackManager.GetAdPacksAdvertsForUsers(user.Membership.AdPackDailyRequiredClicks);
                int numberofAdsViewed = user.RSAPTCAdsViewed.Count();

                int adsRequiredToWatch = user.Membership.AdPackDailyRequiredClicks - numberofAdsViewed;

                if (adsRequiredToWatch <= 0)
                    return 0;

                if (adsRequiredToWatch > AdPacksAdList.Count)
                {
                    int adCount = 0;
                    foreach (AdPacksAdvert ad in AdPacksAdList)
                    {
                        if (!user.RSAPTCAdsViewed.Contains(ad.Id))
                            adCount++;
                    }
                    return adCount;
                }
                return adsRequiredToWatch;

            case NotificationType.UnassignedMatrixMembers:
                return user.GetUnassignedMatrixMembersCount();

            case NotificationType.PendingRepresentativePaymentRequest:
                return ConversationMessage.GetPendingRequestForRepresentativeCount(user.Id);

            default:
                return 0;
        }
    }

    [Obsolete]
    public static void ForceRefresh()
    {
        //For backwards compatibility. It does nothing.
    }

    [Obsolete]
    public static void RemoveAllCookies(HttpResponse response)
    {
        //For backwards compatibility. It does nothing.
    }

    #endregion

}