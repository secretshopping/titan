using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Titan;

public class OfferwallsLog : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "OfferwallsLogs"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("OfferStatus")]
    protected int IntStatus { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

    [Column("IsDeleted")]
    public bool IsDeleted { get { return _isDeleted; } set { _isDeleted = value; SetUpToDateAsFalse(); } }

    [Column("DateAdded")]
    public DateTime Date { get { return date; } set { date = value; SetUpToDateAsFalse(); } }

    [Column("Username")]
    public string Username { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

    [Column("NetworkName")]
    public string NetworkName { get { return name1; } set { name1 = value; SetUpToDateAsFalse(); } }

    [Column("TrackingID")]
    public string TrackingID
    {
        get { return name2; }
        set
        {
            name2 = value;

            if (value != null)
            {
                if (value.Length > 49)
                    name2 = name2.Substring(0, 49);
                else
                    name2 = value;
            }
            SetUpToDateAsFalse();
        }
    }

    [Column("SentBalance")]
    public string SentBalance { get { return e1; } set { e1 = value; SetUpToDateAsFalse(); } }

    [Column("CalculatedBalance")]
    public string CalculatedBalance { get { return e2; } set { e2 = value; SetUpToDateAsFalse(); } }

    [Column("CalculatedBalanceMoney")]
    public Money SentBalanceMoney { get { return e22; } set { e22 = value; SetUpToDateAsFalse(); } }

    private bool _isLocked, _isDeleted;
    private int _id, points, cpoints, type;
    private string name, name1, name2, name3, name4, e1, e2;
    private DateTime date;
    private Money e22;

    #endregion Columns

    public OfferwallsLog()
        : base() { }

    public OfferwallsLog(int id) : base(id) { }

    public OfferwallsLog(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }


    public OfferwallsLogStatus Status
    {
        get
        {
            return (OfferwallsLogStatus)IntStatus;
        }
        set
        {
            IntStatus = (int)value;
        }
    }

    public static OfferwallsLog Create(Offerwall Wall, string Username, Money SentBalance, Money CalculatedBalance, string TrackingInfo, OfferwallsLogStatus Status)
    {
        if (TrackingInfo == null)
            TrackingInfo = "?";

        string sent = "-";
        string credited = "-";

        if (Wall.CreditAs == CreditAs.Points)
        {
            sent = SentBalance.GetRealTotals().ToString();

            if (CalculatedBalance != null)
                credited = CalculatedBalance.GetRealTotals().ToString() + " " + AppSettings.PointsName;
        }

        if (Wall.CreditAs == CreditAs.MainBalance)
        {
            sent = SentBalance.ToClearString();

            if (CalculatedBalance != null)
                credited = CalculatedBalance.ToString();
        }

        OfferwallsLog ol = new OfferwallsLog();
        ol.Date = DateTime.Now;
        ol.NetworkName = Wall.DisplayName;
        ol.SentBalance = sent;
        ol.Status = Status;
        ol.SentBalanceMoney = SentBalance;
        ol.TrackingID = TrackingInfo;
        ol.Username = Username;
        ol.CalculatedBalance = credited;
        ol.Save();

        return ol;
    }

    public static void CreateCrowdflower(string Username, Money SentBalance, Money CalculatedBalance, string TrackingInfo, OfferwallsLogStatus Status)
    {
        Offerwall Temp = new Offerwall();
        Temp.DisplayName = "CrowdFlower";
        Temp.CreditAs = CreditAs.Points;

        if (AppSettings.Offerwalls.ConvertCrowdflowerToMainBalance)
            Temp.CreditAs = CreditAs.MainBalance;

        Create(Temp, Username, SentBalance, CalculatedBalance, TrackingInfo, Status); //TODO Crowdflower Points Locking

    }
}
