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
using System.Text;

public class Report : BaseTableObject
{
    private StringBuilder sb;

    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "Reports"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("DateDay")]
    public DateTime Time { get { return date; } set { date = value; SetUpToDateAsFalse(); } }

    [Column("ActiveAdPacks")]
    public int ActiveAdPacks { get { return _ActiveAdPacks; } set { _ActiveAdPacks = value; SetUpToDateAsFalse(); } }

    [Column("AdPackTypeId")]
    public int AdPackTypeId { get { return _AdPackTypeId; } set { _AdPackTypeId = value; SetUpToDateAsFalse(); } }

    [Column("MoneyInPool")]
    public Money MoneyInPool { get { return _MoneyInPool; } set { _MoneyInPool = value; SetUpToDateAsFalse(); } }

    [Column("MoneyDistributed")]
    public Money MoneyDistributed { get { return _MoneyDistributed; } set { _MoneyDistributed = value; SetUpToDateAsFalse(); } }

    [Column("Report1")]
    public string Details { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

    private int _id, _ActiveAdPacks, _AdPackTypeId;
    private string name;
    private Money _MoneyInPool, _MoneyDistributed;
    private DateTime date;

    #endregion Columns

    public Report()
        : base() {
            sb = new StringBuilder();
        }

    public Report(int id) : base(id) { }

    public Report(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }

    public void AddDetails(string s)
    {
        sb.Append(s);
    }

    public void SaveDetails()
    {
        Details = sb.ToString();
    }

    public static void Add(int activeAdPacks, Money inPool, Money distributed, string details, int adPackTypeId = 0)
    {
        Report report = new Report();
        report.Time = DateTime.Now;
        report.MoneyInPool = inPool;
        report.MoneyDistributed = distributed;
        report.ActiveAdPacks = activeAdPacks;
        report.Details = details;
        report.AdPackTypeId = adPackTypeId;
        report.Save();
    }

}

