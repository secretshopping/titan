using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Titan;
using Prem.PTC.Offers;


public class PointsLockingManager
{
    public static void CRON()
    {
        AppSettings.Points.Reload();
        CRONForOfferwallsLog();
        CRONForCPAGPTLog();
    }

    private static void CRONForOfferwallsLog()
    {
        var lockedLogs = TableHelper.GetListFromQuery<OfferwallsLog>("WHERE OfferStatus = " + (int)OfferwallsLogStatus.CreditedAndPointsLocked);
        foreach (var lockedLog in lockedLogs)
        {
            if (lockedLog.Date.AddDays(AppSettings.Points.LockDays) < DateTime.Now)
            {
                //Should be released
                Release(lockedLog);
            }
        }
    }

    private static void CRONForCPAGPTLog()
    {
        var lockedLogs = TableHelper.GetListFromQuery<CPAPostbackLog>("WHERE Statusek = " + (int)CPAPostBackLogStatus.CreditedAndPointsLocked);
        foreach (var lockedLog in lockedLogs)
        {
            if (lockedLog.DateHappened.AddDays(AppSettings.Points.LockDays) < DateTime.Now)
            {
                //Should be released
                ReleaseCPA(lockedLog);
            }
        }
    }                                        
    public static bool OfferwallShouldBeLocked(Money money, Member member, Offerwall Wall)
    {
        money = OfferwallCrediter.CalculatedAndConversion(money, member, Wall);
        return PointsLockingManager.ShouldBeLocked(money, Wall.CreditAs, member, Wall.RequiresConversion);
    }

    public static bool CPAGPTShouldBeLocked(Money money, CreditAs creditAs, Member member, bool RequiresConversion)
    {
        money = CPAGPTCrediter.CalculatePostback(money, RequiresConversion, member, creditAs);
        return PointsLockingManager.ShouldBeLocked(money, creditAs, member, RequiresConversion);
    }

    private static bool ShouldBeLocked(Money money, CreditAs creditAs, Member member, bool RequiresConversion)
    {

        int Points = 0;

        if (creditAs == CreditAs.MainBalance)
            Points = money.ConvertToPoints();
        else
            Points = money.AsPoints();

        if (Points > AppSettings.Points.LockPoints && AppSettings.Points.IsLock)
            return true;

        return false;
    }

    public static void Release(OfferwallsLog log)
    {
        try
        {
            Offerwall Wall = TableHelper.SelectRows<Offerwall>(TableHelper.MakeDictionary("DisplayName", log.NetworkName))[0];

            //Credit
            OfferwallCrediter Crediter = new OfferwallCrediter(new Member(log.Username), Wall);
            Crediter.CreditMember(log.SentBalanceMoney, Wall);

            //Update log
            log.Status = OfferwallsLogStatus.CreditedByOfferwallPointsUnlocked;
            log.Save();
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    public static void ReleaseCPA(CPAPostbackLog log)
    {
        try
        {
            var where = TableHelper.MakeDictionary("NetworkOfferIdInt", log.OfferId);
            where.Add("AdvertiserUsername", log.NetworkName);
            CPAOffer OurOffer = TableHelper.SelectRows<CPAOffer>(where)[0]; //TODO (What if offer has been deleted in the meantime?)

            //Credit
            CPAGPTCrediter Crediter = (CPAGPTCrediter)CrediterFactory.Acquire(log.Username, CreditType.CPAGPTOffer);
            var conditions = TableHelper.MakeDictionary("Username", log.Username);
            conditions.Add("OfferId", OurOffer.Id);
            conditions.Add("OfferStatus", (int)OfferStatus.Pending);

            OfferRegisterEntry entry = TableHelper.SelectRows<OfferRegisterEntry>(conditions)[0];

            CPAManager.AcceptEntryManually(entry, new Member(entry.Username));

            //Update log
            log.Status = CPAPostBackLogStatus.CreditedByNetworkPointsUnlocked;
            log.Save();

        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

}