using ExtensionMethods;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;

namespace Titan.PaidToPromote
{
    public class PaidToPromoteManager
    {
        public static bool IsInPool(int userId)
        {
            int count = (int)TableHelper.SelectScalar(
                string.Format("SELECT COUNT(*) FROM PaidToPromoteUsers WHERE UserId = {0}", userId));

            return count > 0;
        }

        public static string GetUserClicks(int userId)
        {
            var query = string.Format("SELECT * FROM {0} WHERE [UserId] = {1}", PaidToPromoteUser.TableName, userId);
            return TableHelper.GetListFromRawQuery<PaidToPromoteUser>(query)[0].ClicksDelivered.ToString();
        }

        public static string GetUserLink(int userId)
        {
            return string.Format("{0}user/earn/surf.aspx?ref={1}", AppSettings.Site.Url, userId);
        }

        public static void AddUserToPool(int userId)
        {
            var newUser = new PaidToPromoteUser
            {
                UserId = userId,
                ClicksDelivered = 0,
                CreditedCPM = 0,
                ReferralsDelivered = 0
            };            
            newUser.Save();            
        }

        public static List<PaidToPromotePack> GetAllActivePacks
        {
            get
            {
                var query = string.Format("SELECT * FROM {0} WHERE [Status] != {1} AND [Status] != {2}", PaidToPromotePack.TableName, (int)UniversalStatus.Paused, (int)UniversalStatus.Deleted);
                return TableHelper.GetListFromRawQuery<PaidToPromotePack>(query);
            }
        }

        public static int PackComparision(PaidToPromotePack x, PaidToPromotePack y)
        {
            if ((int)x.Ends.EndMode < (int)y.Ends.EndMode)
                return 1;
            else if ((int)x.Ends.EndMode == (int)y.Ends.EndMode)
            {
                if (x.Price > y.Price)
                    return 1;
                if (x.Price == y.Price)
                    return 0;

                return -1;
            }
            return -1;
        }

        public static bool IsEmptySlotInRotation
        {
            get
            {
                return GetAdsCountInRotation < AppSettings.PaidToPromote.RotationSlotsCount;
            }            
        }

        public static int GetAdsCountInRotation
        {
            get
            {
                var query = string.Format("SELECT COUNT(*) FROM PaidToPromoteAdverts WHERE [Status] = {0}", (int)AdvertStatus.Active);
                return (int)TableHelper.SelectScalar(query);
            }
        }

        public static List<PaidToPromoteAdvert> GetActiveAdsForCurrentIP(string ip)
        {
            var advertsQuery = string.Format("SELECT * FROM {0} WHERE [Status] = {1} AND [CreatorId] != 1005", PaidToPromoteAdvert.TableName, (int)AdvertStatus.Active);
            var adminAdsQuery = string.Format("SELECT * FROM {0} WHERE [Status] = {1} AND [CreatorId] = 1005", PaidToPromoteAdvert.TableName, (int)AdvertStatus.Active);
            var IPsQuery = string.Format("SELECT * FROM {0} WHERE [IP] = '{1}'", PaidToPromoteTemporaryIP.TableName, ip);

            var adsList = TableHelper.GetListFromRawQuery<PaidToPromoteAdvert>(advertsQuery);
            var adminList = TableHelper.GetListFromRawQuery<PaidToPromoteAdvert>(adminAdsQuery);
            var IpsList = TableHelper.GetListFromRawQuery<PaidToPromoteTemporaryIP>(IPsQuery);
            var bannedAdsList = new List<int>();
            var resultList = new List<PaidToPromoteAdvert>();
            var userCountryCode = CountryManager.LookupCountryCode(ip);

            foreach (var item in IpsList)            
                bannedAdsList.Add(item.AdvertId);            

            foreach(var ad in adsList)            
                if (!bannedAdsList.Contains(ad.Id) && ad.CheckGeo(userCountryCode))
                    resultList.Add(ad);            

            resultList.Shuffle();
            adminList.Shuffle();
            resultList.AddRange(adminList);            

            return resultList;
        }        

        public static void TryToCreditAdAndReferrer(int adId, string IP, int referrerId)
        {
            var advert = new PaidToPromoteAdvert(adId);
            var pack = new PaidToPromotePack(advert.PackId);

            if (pack.Ends.EndMode == End.Mode.Clicks)
                advert.CreditClicks();
            else
                advert.CheckDays();

            try
            {
                if (advert.CreatorId != 1005)
                {
                    var checkIPQuery = string.Format("SELECT COUNT(*) FROM {0} WHERE [IP] = '{1}' AND [CreditedUser] = {2}", PaidToPromoteTemporaryIP.TableName, IP, referrerId);
                    var sessionCount = (int)TableHelper.SelectScalar(checkIPQuery);
                    var newIPSession = new PaidToPromoteTemporaryIP
                    {
                        IP = IP,
                        AdvertId = advert.Id
                    };

                    if (sessionCount == 0)
                    {
                        CrfeditPTPUser(referrerId);
                        newIPSession.CreditedUser = referrerId;
                    }
                    else
                        newIPSession.CreditedUser = null;

                    newIPSession.Save();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);                
            }
        }

        private static void CrfeditPTPUser(int userId)
        {
            var selectQuery = string.Format("SELECT * FROM {0} WHERE [UserId] = {1}", PaidToPromoteUser.TableName, userId);
            var users = TableHelper.GetListFromRawQuery<PaidToPromoteUser>(selectQuery);

            users[0].InceaseClicks();            
        }

        public static void CRON()
        {
            try
            {
                var query = string.Format("DELETE FROM {0}", PaidToPromoteTemporaryIP.TableName);
                TableHelper.ExecuteRawCommandNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);                
            }
        }
    }
}