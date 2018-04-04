using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Prem.PTC.Advertising;
using Prem.PTC.Referrals;
using Resources;
using System.Text;
using System.Security.Cryptography;
using Titan;

/// <summary>
/// Handles all actions connected with TrafficGrid
/// </summary>
public static class TrafficGridManager
{
    private static Random random = new Random();
    private static string CLEANUP_COMMAND_1 = "DELETE FROM TrafficGridLatestWinners WHERE Id NOT IN (SELECT TOP 10 Id FROM TrafficGridLatestWinners ORDER BY Date DESC)";
    private static string CLEANUP_COMMAND_2 = "DELETE FROM TrafficGridTopWinners WHERE Id NOT IN (SELECT TOP 10 Id FROM TrafficGridTopWinners ORDER BY Value DESC)";
    /// <summary>
    /// Handles all action required to do with the prize
    /// Throws MsgException as an information
    /// if MsgException contains [OK] = user won (what won passed)
    /// if not, it win nothing (error info passed)
    /// </summary>
    /// <param name="username"></param>
    public static void GetPrizeAndSaveIt(Member User)
    {
        int RandomBetween1_100 = random.Next(1, 101);

        if (User.Membership.TrafficGridChances >= RandomBetween1_100)
        {
            //He won something
            //Now we are randoming the prize
            RandomBetween1_100 = random.Next(1, 101);
            int HelpingSum = 0;

            if (RandomBetween1_100 <= HelpingSum + AppSettings.TrafficGrid.P_AdBalance)
            {
                //He won AdBalance
                HandleAdBalance(User);
            }
            HelpingSum += AppSettings.TrafficGrid.P_AdBalance;

            if (RandomBetween1_100 <= HelpingSum + AppSettings.TrafficGrid.P_DirectReferralLimit)
            {
                //He won DR Limit+1
                HandleDRLimit(User);
            }
            HelpingSum += AppSettings.TrafficGrid.P_DirectReferralLimit;

            if (RandomBetween1_100 <= HelpingSum + AppSettings.TrafficGrid.P_MainBalance)
            {
                //He won Main Balance
                HandleMainBalance(User);
            }
            HelpingSum += AppSettings.TrafficGrid.P_MainBalance;

            if (RandomBetween1_100 <= HelpingSum + AppSettings.TrafficGrid.P_Points)
            {
                //He won Points
                HandlePoints(User);
            }
            HelpingSum += AppSettings.TrafficGrid.P_Points;

            if (RandomBetween1_100 <= HelpingSum + AppSettings.TrafficGrid.P_RentalBalance)
            {
                //He won RentalBalance
                HandleRentalBalance(User);
            }
            HelpingSum += AppSettings.TrafficGrid.P_RentalBalance;

            if (RandomBetween1_100 <= HelpingSum + AppSettings.TrafficGrid.P_RentedReferral)
            {
                //He won RentedReferral
                HandleNewRentedReferrals(User);
            }

        }
        else
        {
            //He won nothing
            throw new MsgException(L1.WONNOTHING);
        }

    }

    private static void AddWinToTrafficGridTables(string what, Money value, string username)
    {
        try
        {
            AppSettings.TrafficGrid.TrafficGridDailyMoneyLeft -= value;
            if (AppSettings.TrafficGrid.TrafficGridDailyMoneyLeft < new Money(0))
                AppSettings.TrafficGrid.TrafficGridDailyMoneyLeft = new Money(0);

            AppSettings.TrafficGrid.Save();

            //1. Latest winners
            var data1 = new TrafficGridLatestWinners();
            data1.Date = DateTime.Now;
            data1.Username = username;
            data1.What = what;
            data1.Save();

            //2. Top winners
            var data2 = new TrafficGridTopWinners();
            data2.Date = DateTime.Now;
            data2.Username = username;
            data2.What = what;
            data2.Value = value;
            data2.Save();

            //Check if not too many records, if so, delete them
            //We want to keep the DB clean :-)
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                bridge.Instance.ExecuteRawCommandNonQuery(CLEANUP_COMMAND_1);
                bridge.Instance.ExecuteRawCommandNonQuery(CLEANUP_COMMAND_2);
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    private static Money WonMoney()
    {
        int myRandom = random.Next(1, Money.Zero.GetMultiplier() + 1);

        Money realRandom = new Money(Convert.ToDecimal(decimal.Divide(myRandom, Money.Zero.GetMultiplier())));

        return new Money(realRandom.ToDecimal() * AppSettings.TrafficGrid.Limit.ToDecimal());
    }

    /// <summary>
    /// It throws MsgException when we have funds problems
    /// </summary>
    /// <returns></returns>
    private static int WonPoints(out Money Pay)
    {
        Pay = WonMoney();

        if (Pay > AppSettings.TrafficGrid.TrafficGridDailyMoneyLeft || Pay <= Money.Zero)
            throw new MsgException(L1.WONNOTHING);

        int Points = Pay.ConvertToPoints();
        return Points;
    }

    public static int GetMinPointsReward()
    {
        Money minMoney = new Money(0.01);
        return minMoney.ConvertToPoints();
    }

    public static int GetMaxPointsReward()
    {
        Money maxMoney = AppSettings.TrafficGrid.Limit;
        return maxMoney.ConvertToPoints();
    }

    public static string GenerateGridHTML()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<div class=\"traffic-grid\">");
        for (int i = 0; i < 600; ++i)
        {
            sb.Append("<div class=\"traffic-cell\" onclick=\"doSubmit();\"><div class=\"traffic-ccell\"></div><div class=\"traffic-cframe\"></div></div>");
        }
        sb.Append("</div>");

        return sb.ToString();
    }

    public static TrafficGridAdvert GetRandomAdvertisement()
    {
        var list = TrafficGridAdvert.GetAllActiveAds();
        Random rand = new Random();
        int index = rand.Next(0, list.Count);
        return list[index];
    }

    public static bool IsOn
    {
        get
        {
            var list = TrafficGridAdvert.GetAllActiveAds();
            if (list.Count > 0)
                return true;
            return false;
        }
    }

    private static void HandleAdBalance(Member User)
    {
        Money Won = WonMoney();
        if (Won > AppSettings.TrafficGrid.TrafficGridDailyMoneyLeft || AppSettings.TrafficGrid.TrafficGridDailyMoneyLeft == Money.Zero)
            throw new MsgException(L1.WONNOTHING);

        string what = Won.ToString() + " on Purchase Balance";

        TrafficGridCrediter crediter = new TrafficGridCrediter(User);
        crediter.CreditUser(Won, what, TrafficGridPrizeType.PurchaseBalance);

        AddWinToTrafficGridTables(what, Won, User.Name);

        throw new MsgException("[OK]" + what);
    }

    private static void HandleMainBalance(Member User)
    {
        Money Won = WonMoney();
        if (Won > AppSettings.TrafficGrid.TrafficGridDailyMoneyLeft || AppSettings.TrafficGrid.TrafficGridDailyMoneyLeft == Money.Zero)
            throw new MsgException(L1.WONNOTHING);

        string what = Won.ToString() + " on Main Balance";

        TrafficGridCrediter crediter = new TrafficGridCrediter(User);
        crediter.CreditUser(Won, what, TrafficGridPrizeType.MainBalance);

        AddWinToTrafficGridTables(what, Won, User.Name);

        throw new MsgException("[OK]" + what);
    }

    private static void HandleRentalBalance(Member User)
    {
        Money Won = WonMoney();
        if (Won > AppSettings.TrafficGrid.TrafficGridDailyMoneyLeft || AppSettings.TrafficGrid.TrafficGridDailyMoneyLeft == Money.Zero)
            throw new MsgException(L1.WONNOTHING);

        string what = Won.ToString() + " on Traffic Balance";

        TrafficGridCrediter crediter = new TrafficGridCrediter(User);
        crediter.CreditUser(Won, what, TrafficGridPrizeType.TrafficBalance);

        AddWinToTrafficGridTables(what, Won, User.Name);

        throw new MsgException("[OK]" + what);
    }

    private static void HandleDRLimit(Member User)
    {
        if (AppSettings.TrafficGrid.TrafficGridDailyMoneyLeft == Money.Zero)
            throw new MsgException(L1.WONNOTHING);

        Money Won = new Money(0.1);
        string what = "Direct ref limit +1";

        TrafficGridCrediter crediter = new TrafficGridCrediter(User);
        crediter.CreditUser(Won, what, TrafficGridPrizeType.DirectReferralLimit);

        AddWinToTrafficGridTables(what, Won, User.Name);

        throw new MsgException("[OK]" + what);
    }

    private static void HandleNewRentedReferrals(Member User)
    {
        Money Won = User.Membership.ReferralRentCost;

        if (Won > AppSettings.TrafficGrid.TrafficGridDailyMoneyLeft || AppSettings.TrafficGrid.TrafficGridDailyMoneyLeft == Money.Zero)
            throw new MsgException(L1.WONNOTHING);

        string what = "1 rented referral";
        TrafficGridCrediter crediter = new TrafficGridCrediter(User);
        crediter.CreditUser(Won, what, TrafficGridPrizeType.RentedReferrals);

        AddWinToTrafficGridTables(what, Won, User.Name);

        throw new MsgException("[OK]" + what);
    }

    private static void HandlePoints(Member User)
    {
        Money Pay;
        int HowMany = WonPoints(out Pay);
        string what = HowMany.ToString() + " " + AppSettings.PointsName;

        TrafficGridCrediter crediter = new TrafficGridCrediter(User);
        crediter.CreditUser(Pay, what, TrafficGridPrizeType.Points, HowMany);

        AddWinToTrafficGridTables(what, Pay, User.Name);

        throw new MsgException("[OK]" + what);
    }

    
}
public enum TrafficGridPrizeType
{
    PurchaseBalance = 1,
    DirectReferralLimit = 2,
    MainBalance = 3,
    Points = 4,
    TrafficBalance = 5,
    RentedReferrals = 6
}