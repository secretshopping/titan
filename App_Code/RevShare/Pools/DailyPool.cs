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

[Serializable]
public class DailyPool : BaseTableObject
{
    #region Columns
    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "DailyPools"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("SumAmount")]
    public Money SumAmount { get { return amount; } set { amount = value; SetUpToDateAsFalse(); } }

    [Column("DateDay")]
    public DateTime DateDay { get { return date; } set { date = value; SetUpToDateAsFalse(); } }

    [Column("DailyPoolType")]
    public int DailyPoolType { get { return _DailyPoolType; } set { _DailyPoolType = value; SetUpToDateAsFalse(); } }

    private int _id;
    private int _DailyPoolType;
    private Money amount;
    private DateTime date;


    public DailyPool()
        : base() { }

    public DailyPool(int id) : base(id) { }

    public DailyPool(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }

    #endregion Columns

    public void MoveMoneyForTomorrow()
    {
        DailyPool tomorrowPool = Get(this.DailyPoolType, AppSettings.ServerTime.AddDays(1).Date);
        tomorrowPool.SumAmount += this.SumAmount;
        tomorrowPool.Save();
        this.SumAmount = Money.Zero;
    }

    /// <summary>
    /// Gets the pool basing on the type. If pool is not present in the DB, it is being created
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static DailyPool Get(int poolId, DateTime date)
    {
        var where = TableHelper.MakeDictionary("DailyPoolType", poolId);
        where.Add("DateDay", date.Date);

        var results = TableHelper.SelectRows<DailyPool>(where);

        if (results.Count > 0)
            return results[0];

        DailyPool NewPool = new DailyPool();
        NewPool.DailyPoolType = poolId;
        NewPool.SumAmount = new Money(0);
        NewPool.DateDay = date.Date;
        NewPool.Save();

        return NewPool;
    }

    public static DailyPool GetTodayPool(int poolId)
    {
        return Get(poolId, AppSettings.ServerTime.Date);
    }

    public static DailyPool GetYesterdayPool(int poolId)
    {
        //YESTERDAY pool is used for distribution
        return Get(poolId, AppSettings.ServerTime.AddDays(-1).Date);
    }

}
