using System;
using Prem.PTC;
using System.Data;
using ExtensionMethods;

/// <summary>
/// Summary description for PoolDistributionHistory
/// Updates table with daily amount that goes into revenue distribution pool
/// </summary>
public class AdPackProfitDistributionHistory : BaseTableObject
{

    #region Columns
    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "AdPackProfitDistributionHistory"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("Amount")]
    public Money SumAmount { get { return amount; } set { amount = value; SetUpToDateAsFalse(); } }

    [Column("Date")]
    public DateTime DateDay { get { return date; } set { date = value; SetUpToDateAsFalse(); } }

    private int _id;
    private Money amount;
    private DateTime date;
    public AdPackProfitDistributionHistory()
            : base() { }

    public AdPackProfitDistributionHistory(int id) : base(id) { }

    public AdPackProfitDistributionHistory(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }
    #endregion Columns

    public static void Add(Money amount)
    {
        AdPackProfitDistributionHistory newRule = null;

        DateTime Now = AppSettings.ServerTime;

        var historyEntries = TableHelper.GetListFromRawQuery<AdPackProfitDistributionHistory>("SELECT * FROM AdPackProfitDistributionHistory WHERE [Date] = '" + Now.Date.ToDBString() + "'");

        if (historyEntries.Count > 0)
        {
            newRule = historyEntries[0];
            newRule.SumAmount += amount;
        }
        else
        {
            newRule = new AdPackProfitDistributionHistory();
            newRule.SumAmount = amount;
            newRule.date = Now.Date;
        }
        
        newRule.Save();
    }
}