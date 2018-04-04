using System;
using System.Web;
using System.Text;
using System.Data;
using Prem.PTC;
using Prem.PTC.Referrals;
using Prem.PTC.Statistics;
using Prem.PTC.Security;
using Titan;
using Titan.CLP;
using Titan.Advertising;
using Titan.Matrix;
using Titan.InvestmentPlatform;
using Titan.Marketplace;
using Titan.MiniVideos;
using Titan.PaidToPromote;
using Titan.RollDiceLottery;
using SocialNetwork;
using Titan.ICO;
using MarchewkaOne.Titan.Balances;
using Titan.News;

public class CRON
{
    private static Parser parser;
    private static StringBuilder log;
    private static DateTime stime = DateTime.Now;

    public static void ProceedDailyTasks()
    {
        log = new StringBuilder();

        var Entry = new CronEntry();
        Entry.Date = DateTime.Now;

        try
        {
            Log("Starting CRON procedure");

            //Make sure it runs only once
            if (CanRun())
            {
                AppSettings.IsOffline = true;
                AppSettings.Misc.LastCRONRun = DateTime.Now;
                AppSettings.Misc.Save();
                stime = DateTime.Now;

                AntiCheatSystem.CRON();
                Log("OK [1] AntiCheatSystem. Elapsed time: " + GetTimeString(stime));

                RentReferralsSystem.CRON();
                Log("OK [2] RentedReferralsSystem. Elapsed time: " + GetTimeString(stime));

                StatisticsManager.CRON();
                Log("OK [3a] StatisticsManager. Elapsed time: " + GetTimeString(stime));

                //MUST BE RUN BEFORE MEMBERMANAGER.CRON() !!!
                CustomFeaturesManager.CRON();
                Log("OK [3b] CustomFeaturesManager. Elapsed time: " + GetTimeString(stime));

                MemberManager.CRON();
                Log("OK [4] Member. Elapsed time: " + GetTimeString(stime));

                SyncedNetworkFactory.SynchronizeAll();
                Log("OK [5] CPA Offers. Elapsed time: " + GetTimeString(stime));

                AppSettings.Payments.CRON();
                Log("OK [6] Payments. Elapsed time: " + GetTimeString(stime));

                ShoutboxManager.CRON();
                Log("OK [7] Shoutbox. Elapsed time: " + GetTimeString(stime));

                CLPManager.CRON();
                Log("OK [8] CLPManager. Elapsed time: " + GetTimeString(stime));

                BannerAuctionManager.CRON();
                Log("OK [10] BannerAuctionManager. Elapsed time: " + GetTimeString(stime));

                PointsLockingManager.CRON();
                Log("OK [11] PointsLockingManager. Elapsed time: " + GetTimeString(stime));

                DBArchiver.CRON();
                Log("OK [12] DBArchiver. Elapsed time: " + GetTimeString(stime));

                RevenueSharesDistributionManager.CRON();
                Log("OK [13] RevenueSharesDistributionManager. Elapsed time: " + GetTimeString(stime));

                CreditLineManager.CRON();
                Log("OK [14] CreditLineManager. Elapsed time: " + GetTimeString(stime));

                PoolRotatorManager.CRON();
                Log("OK [15] PoolRotatorManager. Elapsed time: " + GetTimeString(stime));

                JackpotManager.CRON();
                Log("OK [16] JackpotManager. Elapsed time: " + GetTimeString(stime));

                TrafficExchangeManager.CRON();
                Log("OK [17] TrafficExchangeManager. Elapsed time: " + GetTimeString(stime));

                DailyPoolManager.CRON();
                Log("OK [18] DailyPoolManager. Elapsed time: " + GetTimeString(stime));

                //Matrix
                MatrixBase matrix = MatrixFactory.GetMatrix();
                if (matrix != null) matrix.CRON();
                Log("OK [19] MatrixBase. Elapsed time: " + GetTimeString(stime));

                ApiAccessToken.CRON();
                Log("OK [20] ApiAccessToken. Elapsed time: " + GetTimeString(stime));

                InvestmentPlatformManager.CRON();
                Log("OK [21] InvestmentPlatformManager. Elapsed time: " + GetTimeString(stime));

                MarketplaceBalanceLogManager.CRON();
                Log("OK [22] MarketplaceBalanceLogManager. Elapsed time: " + GetTimeString(stime));

                MiniVideoManager.CRON();
                Log("OK [23] MiniVideoManager. Elapsed time: " + GetTimeString(stime));

                PaidToPromoteManager.CRON();
                Log("OK [24] PaidToPromoteManager. Elapsed time: " + GetTimeString(stime));

                RollDiceLotteryManager.CRON();
                Log("OK [25] RollDiceLotteryManager. Elapsed time: " + GetTimeString(stime));

                WalletManager.CRON();
                Log("OK [26] WalletManager. Elapsed time: " + GetTimeString(stime));

                NewsManager.CRON();
                Log("OK [27] NewsManager. Elapsed time: " + GetTimeString(stime));


                Entry.Type = 1;
                Entry.Text = "Procedure completed successfully (27/27 100%) after " + GetTimeString(stime);
            }
            else
            {
                Entry.Type = 2;
                Entry.Text = "Procedure prevented from multiple run";
            }

        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            Log("Fatal error (exception thrown)..");
            Entry.Type = 3;
            Entry.Text = "Fatal error during procedure execution. Check logs for more information";
            Mailer.SendCRONFailureMessage();
        }
        finally
        {
            ErrorLogger.Log(log.ToString(), LogType.CRON);
            AppSettings.IsOffline = false;
            Entry.Save();
            HttpContext.Current.Response.Write(Entry.Text);
        }
    }

    public static void ProceedHourlyDistribution()
    {
        AppSettings.RevShare.Reload();

        if (AppSettings.RevShare.IsRevShareEnabled)
        {
            log = new StringBuilder();

            var Entry = new CronEntry();
            Entry.Date = DateTime.Now;

            try
            {
                stime = DateTime.Now;
                AppSettings.IsOffline = true;

                if (AppSettings.RevShare.DistributionTime != DistributionTimePolicy.EveryHour)
                {
                    Entry.Type = 2;
                    Entry.Text = "CRON has been triggered, but Hourly Distribution is turned off. No action has been made.";
                }
                else
                {
                    if (AppSettings.RevShare.HourlyDistributionsMadeToday < 23)
                    {
                        RevenueSharesDistributionManager.CRON_EVERY_HOUR();

                        AppSettings.RevShare.HourlyDistributionsMadeToday++;
                        AppSettings.RevShare.Save();

                        Entry.Type = 1;
                        Entry.Text = "Hourly distribution proceed successfully after " + GetTimeString(stime);
                    }
                    else
                    {
                        Entry.Type = 2;
                        Entry.Text = "Hourly distribution prevented from running more than 23 times a day";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                Log("Fatal error (exception thrown)..");
                Entry.Type = 3;
                Entry.Text = "Fatal error during procedure execution. Check logs for more information";
                //Mailer.SendCRONFailureMessage();
            }
            finally
            {
                ErrorLogger.Log(log.ToString(), LogType.CRON);
                Entry.Save();
                AppSettings.IsOffline = false;
            }
        }
    }

    public static void ProceedFrequentDistribution()
    {
        try
        {
            log = new StringBuilder();

            // [1]Escrow for Representatives Actions 
            ConversationManager.CRON();

            // [2]Escrow for Cryptocurrency Trading
            CryptocurrencyPlatformManager.CRON();

        }
        catch(Exception ex)
        {
            ErrorLogger.Log(ex.Message, LogType.CRON);
            Log("FrequentCRON error: " + ex.Message);
        }
    }

    private static bool CanRun()
    {
        AppSettings.Misc.Reload();

        if (AppSettings.Misc.LastCRONRun.Date != DateTime.Now.Date)
            return true;
        return false;
    }

    private static void Log(string text)
    {
        log.Append("[");
        log.Append(DateTime.Now.ToString());
        log.Append("] ");
        log.Append(text);
        log.Append(Environment.NewLine);
    }

    private static string GetTimeString(DateTime time)
    {
        TimeSpan dt = DateTime.Now.Subtract(time);
        return  dt.Minutes + "m " + dt.Seconds + "s";
    }
}

public class CronEntry : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "CronEntries"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("Type1")]
    public int Type { get { return quantity; } set { quantity = value; SetUpToDateAsFalse(); } }

    [Column("Text1")]
    public string Text { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

    [Column("Date1")]
    public DateTime Date { get { return _Date; } set { _Date = value; SetUpToDateAsFalse(); } }


    private int _id, quantity;
    private string name, _Title;
    private DateTime _Date;

    #endregion Columns

    public CronEntry()
        : base() { }

    public CronEntry(int id) : base(id) { }

    public CronEntry(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }

}
