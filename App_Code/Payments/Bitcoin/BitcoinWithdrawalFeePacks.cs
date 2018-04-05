using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Data;
using System.Text;

public class BitcoinWithdrawalFeePacks : BaseTableObject
{
    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "BitcoinWithdrawalFeePacks"; } }

    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("Fee")]
    public decimal Fee { get { return _fee; } set { _fee = value; SetUpToDateAsFalse(); } }

    [Column("WeeksDelay")]
    public int WeeksDelay { get { return _weeksDelay; } set { _weeksDelay = value; SetUpToDateAsFalse(); } }
   
    private int _id, _weeksDelay;
    private decimal _fee;

    public BitcoinWithdrawalFeePacks() : base() { }

    public BitcoinWithdrawalFeePacks(int id): base(id){ }

    public BitcoinWithdrawalFeePacks(DataRow row, bool isUpToDate = true) : base(row, isUpToDate){ }

    public static bool IsWeeksPackExist(int weekDealy)
    {
        var query = string.Format("SELECT COUNT(*) FROM {0} WHERE [WeeksDelay] = {1}", TableName, weekDealy);

        return (int)TableHelper.SelectScalar(query) == 1;
    }    

    public static string GetPacksText(int userId)
    {
        var query = "SELECT * FROM BitcoinWithdrawalFeePacks ORDER BY WeeksDelay";
        var packsList = TableHelper.GetListFromRawQuery<BitcoinWithdrawalFeePacks>(query);
        var result = new StringBuilder();
        var userPack = GetFeePackForUser(userId);

        foreach (var pack in packsList)
        {
            var text = "";
            if (pack.WeeksDelay == userPack.WeeksDelay)
                text = string.Format("<b>{0}: {1}%, {2}: {3}</b><br />", U3500.CASHOUT_FEES, pack.Fee, U6010.WEEKSFROMLASTWITHDRAWAL, pack.WeeksDelay);
            else
                text = string.Format("{0}: {1}%, {2}: {3}<br />", U3500.CASHOUT_FEES, pack.Fee, U6010.WEEKSFROMLASTWITHDRAWAL, pack.WeeksDelay);
            result.Append(text);
        }

        return result.ToString();
    }

    public static BitcoinWithdrawalFeePacks GetFeePackForUser(int userId)
    {
        var daysFromLastWithdrawal = GetDaysFromLastWithdrawal(userId);

        try
        {
            int weeks = daysFromLastWithdrawal / 7;
            var query = string.Format("SELECT TOP 1 * FROM BitcoinWithdrawalFeePacks WHERE WeeksDelay <= {0} ORDER BY WeeksDelay DESC", weeks);
            return TableHelper.GetListFromRawQuery<BitcoinWithdrawalFeePacks>(query)[0];
        }
        catch(Exception e)
        {
            ErrorLogger.Log(e);
            return new BitcoinWithdrawalFeePacks();
        }
    }

    private static int GetDaysFromLastWithdrawal(int userId)
    {
        DateTime myDate;
        var user = new Member(userId);
        var btcQuery = string.Format("SELECT TOP 1 RequestDate FROM CryptocurrencyWithdrawRequests where UserId = {0} ORDER BY RequestDate DESC", userId);
        var ppQuery = string.Format("SELECT TOP 1 RequestDate FROM PayoutRequests where username = '{0}' ORDER BY RequestDate DESC", user.Name);

        DateTime? btcDate = (DateTime?)TableHelper.SelectScalar(btcQuery);
        DateTime? ppDate = (DateTime?)TableHelper.SelectScalar(ppQuery);

        if (btcDate == null && ppDate == null)
            myDate = user.Registered;
        else if (btcDate == null)
            myDate = (DateTime)ppDate;
        else if (ppDate == null)
            myDate = (DateTime)btcDate;
        else
            myDate = ppDate < btcDate ? (DateTime)btcDate : (DateTime)ppDate;

        return (AppSettings.ServerTime - myDate).Days;
    }
}