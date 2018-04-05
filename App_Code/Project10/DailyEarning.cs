using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Prem.PTC.Advertising;
using System.Web;
using System.Web.UI;


public class DailyEarning : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "DailyEarnings"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("SumAmount")]
    public Money SumAmount { get { return amount; } set { amount = value; SetUpToDateAsFalse(); } }

    [Column("DateDay")]
    public DateTime DateDay { get { return date; } set { date = value; SetUpToDateAsFalse(); } }

    [Column("EarningsType")]
    public int EarningsType { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

    public EarningsStatsType Type
    {
        get
        {
            return (EarningsStatsType)EarningsType;
        }
        set
        {
            EarningsType = (int)value;
        }
    }

    private int _id;
    private int name;
    private Money amount;
    private DateTime date;

    public DailyEarning()
        : base() { }

    public DailyEarning(int id) : base(id) { }

    public DailyEarning(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }

    #endregion Columns

    public static DailyEarning GetProperObject(EarningsStatsType type, DateTime date)
    {
        DailyEarning Earning;

        var where = TableHelper.MakeDictionary("EarningsType", (int)type);
        where.Add("DateDay", date.Date);

        var result = TableHelper.SelectRows<DailyEarning>(where);
        if (result.Count == 0)
        {
            //Not present, we need to create it
            Earning = new DailyEarning();
            Earning.SumAmount = new Money(0);
            Earning.DateDay = date.Date;
            Earning.Type = type;
        }
        else
            Earning = result[0];

        return Earning;
    }

}

public enum EarningsStatsType
{
    Banner = 0,
    PortfolioUnits = 2,
    Promotion = 3,
    RevenueShareAds = 4,
    Charity = 5,
    CPAOffers = 6,
    Offerwalls = 7
}