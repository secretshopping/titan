using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

/// <summary>
/// Summary description for UserBets
/// </summary>
public class SurfAdsPack : BaseTableObject
{

    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "SurfAdsPacks"; } }
    protected override string dbTable { get { return TableName; } }
    public static class Columns
    {
        public const string Id = "Id";
        public const string Price = "Price";
        public const string Clicks = "Clicks";
        public const string DisplayTime = "DisplayTime";
        public const string Status = "Status";
    }


    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Price)]
    public Money Price { get { return _Price; } set { _Price = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Clicks)]
    public int Clicks { get { return _Clicks; } set { _Clicks = value; SetUpToDateAsFalse(); } }

    [Column(Columns.DisplayTime)]
    public int DisplayTime { get { return _DisplayTime; } set { _DisplayTime = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Status)]
    protected int StatusInt { get { return _Status; } set { _Status = value; SetUpToDateAsFalse(); } }
    #endregion

    private int _id, _Clicks, _Status, _DisplayTime;
    Money _Price;

    public SurfAdsPackStatus Status
    {
        get { return (SurfAdsPackStatus)StatusInt; }
        set { StatusInt = (int)value; }
    }

    public SurfAdsPack()
            : base()
    { }

    public SurfAdsPack(int id) : base(id) { }

    public SurfAdsPack(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    { }

    public static List<SurfAdsPack> GetAllActivePacks()
    {
        return TableHelper.SelectRows<SurfAdsPack>(TableHelper.MakeDictionary("Status", (int)SurfAdsPackStatus.Active));
    }
}