using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExtensionMethods;


public class PoolRotatorManager
{
    public static bool IsInPool(int userId)
    {
        int count = (int)TableHelper.SelectScalar(
            String.Format("SELECT COUNT(*) FROM PoolRotatorLinkUsers WHERE IsActive = 1 AND UserId = {0}", userId));

        return count > 0;
    }

    public static PoolRotatorLinkUser Get(int userId)
    {
        return TableHelper.GetListFromRawQuery<PoolRotatorLinkUser>(
            String.Format("SELECT * FROM PoolRotatorLinkUsers WHERE IsActive = 1 AND UserId = {0}", userId))[0];
    }

    public static int GetUserIdFromPool()
    {
        var membersInPool = TableHelper.GetListFromRawQuery<PoolRotatorLinkUser>("SELECT * FROM PoolRotatorLinkUsers WHERE IsActive = 1");
        Random random = new Random();

        if (membersInPool.Count > 0)
            return membersInPool[random.Next(0, membersInPool.Count)].UserId;

        return -1;
    }

    public static string TryGetUserNameFromPool(string refUsername)
    {
        // With referral link for Pool Rotator
        if (refUsername.StartsWith("*"))
        {
            int userId = PoolRotatorManager.GetUserIdFromPool();
            if (userId != -1)
                HttpContext.Current.Session["ReferralFrom"] = "Rotator Link";

            refUsername = userId == -1 ? String.Empty : new Member(userId).Name;
        }

        return refUsername;
    }

    public static void TryAddLinkView(string input)
    {
        if (AppSettings.TitanFeatures.ReferralPoolRotatorEnabled)
        {
            try
            {
                if (!String.IsNullOrEmpty(input) && input.StartsWith("*"))
                {
                    int linkId = Convert.ToInt32(input.Substring(1));
                    PoolRotatorLinkUser link = new PoolRotatorLinkUser(linkId);
                    link.ClicksDelivered++;

                    if (link.ClicksDelivered % 1000 == 0)
                    {
                        Member owner = new Member(link.UserId);
                        owner.AddToPointsBalance(owner.Membership.PointsPer1000viewsDeliveredToPoolRotator, "Rotator Link bonus");
                        owner.SaveBalances();
                    }

                    link.Save();
                }
            }
            catch (Exception ex) { }
        }
    }

    public static void AddToPool(int userId)
    {
        PoolRotatorLinkUser linkUser = new PoolRotatorLinkUser();
        linkUser.Expires = DateTime.Now.AddDays(30);
        linkUser.UserId = userId;
        linkUser.ReferralsDelivered = 0;
        linkUser.ClicksDelivered = 0;
        linkUser.IsActive = true;
        linkUser.Save();
    }

    public static void CRON()
    {
        try
        {
            string Command = String.Format("UPDATE PoolRotatorLinkUsers SET IsActive = 0 WHERE IsActive = 1 AND Expires <= '{0}'", AppSettings.ServerTime.ToDBString());
            TableHelper.ExecuteRawCommandNonQuery(Command);
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }
}