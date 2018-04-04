using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Jackpot
/// </summary>
public class Jackpot : BaseTableObject
{
    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "Jackpots"; } }
    protected override string dbTable { get { return TableName; } }
    public static class Columns
    {

        public const string Id = "Id";
        public const string StartDate = "StartDate";
        public const string EndDate = "EndDate";
        public const string TicketPrice = "TicketPrice";
        public const string Status = "Status";
        public const string Name = "Name";
        public const string NumberOfTickets = "NumberOfTickets";
        public const string NumberOfParticipants = "NumberOfParticipants";
        public const string NumberOfWinningTickets = "NumberOfWinningTickets";

        public const string MainBalancePrize = "Prize";
        public const string AdBalancePrize = "AdBalancePrize";
        public const string LoginAdsCreditsPrize = "LoginAdsCreditsPrize";
        public const string UpgradeIdPrize = "UpgradeIdPrize";
        public const string UpgradeDaysPrize = "UpgradeDaysPrize";

        public const string MainBalancePrizeEnabled = "MainBalancePrizeEnabled";
        public const string AdBalancePrizeEnabled = "AdBalancePrizeEnabled";
        public const string UpgradePrizeEnabled = "UpgradePrizeEnabled";
        public const string LoginAdsCreditsPrizeEnabled = "LoginAdsCreditsPrizeEnabled";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.MainBalancePrize)]
    public Money MainBalancePrize { get { return _MainBalancePrize; } set { _MainBalancePrize = value; SetUpToDateAsFalse(); } }

    [Column(Columns.StartDate)]
    public DateTime StartDate { get { return _StartDate; } set { _StartDate = value; SetUpToDateAsFalse(); } }

    [Column(Columns.EndDate)]
    public DateTime EndDate { get { return _EndDate; } set { _EndDate = value; SetUpToDateAsFalse(); } }

    [Column(Columns.TicketPrice)]
    public Money TicketPrice { get { return _TicketPrice; } set { _TicketPrice = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Status)]
    protected int JackpotStatusInt { get { return _JackpotStatusInt; } set { _JackpotStatusInt = value; SetUpToDateAsFalse(); } }

    [Column(Columns.NumberOfParticipants)]
    public int? NumberOfParticipants { get { return _NumberOfParticipants; } set { _NumberOfParticipants = value; SetUpToDateAsFalse(); } }

    [Column(Columns.NumberOfTickets)]
    public int? NumberOfTickets { get { return _NumberOfTickets; } set { _NumberOfTickets = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Name)]
    public string Name { get { return _Name; } set { _Name = value; SetUpToDateAsFalse(); } }

    [Column(Columns.AdBalancePrize)]
    public Money AdBalancePrize { get { return _AdBalancePrize; } set { _AdBalancePrize = value; SetUpToDateAsFalse(); } }

    [Column(Columns.LoginAdsCreditsPrize)]
    public int LoginAdsCreditsPrize { get { return _LoginAdsCreditsPrize; } set { _LoginAdsCreditsPrize = value; SetUpToDateAsFalse(); } }

    [Column(Columns.UpgradeIdPrize)]
    public int UpgradeIdPrize { get { return _UpgradeIdPrize; } set { _UpgradeIdPrize = value; SetUpToDateAsFalse(); } }

    [Column(Columns.UpgradeDaysPrize)]
    public int UpgradeDaysPrize { get { return _UpgradeDaysPrize; } set { _UpgradeDaysPrize = value; SetUpToDateAsFalse(); } }

    [Column(Columns.UpgradePrizeEnabled)]
    public bool UpgradePrizeEnabled { get { return _UpgradePrizeEnabled; } set { _UpgradePrizeEnabled = value; SetUpToDateAsFalse(); } }

    [Column(Columns.MainBalancePrizeEnabled)]
    public bool MainBalancePrizeEnabled { get { return _MainBalancePrizeEnabled; } set { _MainBalancePrizeEnabled = value; SetUpToDateAsFalse(); } }

    [Column(Columns.AdBalancePrizeEnabled)]
    public bool AdBalancePrizeEnabled { get { return _AdBalancePrizeEnabled; } set { _AdBalancePrizeEnabled = value; SetUpToDateAsFalse(); } }

    [Column(Columns.LoginAdsCreditsPrizeEnabled)]
    public bool LoginAdsCreditsPrizeEnabled { get { return _LoginAdsCreditsPrizeEnabled; } set { _LoginAdsCreditsPrizeEnabled = value; SetUpToDateAsFalse(); } }

    [Column(Columns.NumberOfWinningTickets)]
    public int NumberOfWinningTickets { get { return _NumberOfWinningTickets; } set { _NumberOfWinningTickets = value; SetUpToDateAsFalse(); } }

    public JackpotStatus Status
    {
        get
        {
            return (JackpotStatus)JackpotStatusInt;
        }

        set
        {
            JackpotStatusInt = (int)value;
        }
    }
    #endregion

    private Money _MainBalancePrize, _TicketPrice, _AdBalancePrize;
    private int _id, _JackpotStatusInt, _LoginAdsCreditsPrize, _UpgradeIdPrize, _UpgradeDaysPrize, _NumberOfWinningTickets;
    private DateTime _StartDate, _EndDate;
    private string _Name;
    private int? _NumberOfParticipants, _NumberOfTickets;
    private bool _UpgradePrizeEnabled, _MainBalancePrizeEnabled, _AdBalancePrizeEnabled, _LoginAdsCreditsPrizeEnabled;

    public Jackpot()
            : base()
    { }

    public Jackpot(int id) : base(id) { }

    public Jackpot(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    { }

    public List<int> GetDistinctUserWinnerIds()
    {
        return TableHelper.GetListFromRawQuery(string.Format("SELECT DISTINCT UserWinnerId FROM JackpotWinningTickets WHERE JackpotId = {0}", Id));
    }

    public List<int>GetWinningTicketNumbers()
    {
        return TableHelper.GetListFromRawQuery(string.Format("SELECT WinningTicketNumber FROM JackpotWinningTickets WHERE JackpotId = {0}", Id));
    }
}

public enum JackpotStatus
{
    Paused = 1,
    Active = 2,
    Finished = 3,
    Deleted = 4
}

public enum JackpotPrize
{
    MainBalance = 0,
    AdBalance = 1,
    LoginAdsCredits = 2,
    Upgrade = 3
}