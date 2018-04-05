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


public class GlobalPool : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "GlobalPools"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("SumAmount")]
    public Money SumAmount { get { return amount; } set { amount = value; SetUpToDateAsFalse(); } }

    [Column("GlobalPoolType")]
    public int GlobalPoolType { get { return _GlobalPoolType; } set { _GlobalPoolType = value; SetUpToDateAsFalse(); } }

    private int _id;
    private int _GlobalPoolType;
    private Money amount;

    #endregion Columns

    public GlobalPool()
        : base() { }

    public GlobalPool(int id) : base(id) { }

    public GlobalPool(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }


    /// <summary>
    /// Gets the pool basing on the type. If pool is not present in the DB, it is being created
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static GlobalPool Get(int poolId)
    {
        var results = TableHelper.SelectRows<GlobalPool>(TableHelper.MakeDictionary("GlobalPoolType", poolId));

        if (results.Count > 0)
            return results[0];

        GlobalPool NewPool = new GlobalPool();
        NewPool.GlobalPoolType = poolId;
        NewPool.SumAmount = new Money(0);
        NewPool.Save();

        return NewPool;
    }
}
