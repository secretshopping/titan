using Prem.PTC;
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
public class AutomaticGroup : BaseTableObject
{

    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "AutomaticGroups"; } }
    protected override string dbTable { get { return TableName; } }
    public static class Columns
    {       
        public const string Id = "Id";
        public const string Number = "Number";
        public const string AdPacksLimit = "AdPacksLimit"; 
        public const string AcceleratedProfitPercentage = "AcceleratedProfitPercentage";
        public const string Color = "Color";
        public const string IsHighestGroup = "IsHighestGroup";
    }


    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Number)]
    public int Number { get { return _Number; } set { _Number = value; SetUpToDateAsFalse(); } }

    [Column(Columns.AdPacksLimit)]
    public int AdPacksLimit { get { return _AdPacksLimit; }  set { _AdPacksLimit = value; SetUpToDateAsFalse(); } }

    [Column(Columns.AcceleratedProfitPercentage)]
    public int AcceleratedProfitPercentage { get { return _AcceleratedProfitPercentage; }  set { _AcceleratedProfitPercentage = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Color)]
    public string Color { get { return _Color; } set { _Color = value; SetUpToDateAsFalse(); } }

    [Column(Columns.IsHighestGroup)]
    public bool IsHighestGroup { get { return _IsHighestGroup; } set { _IsHighestGroup = value; SetUpToDateAsFalse(); } }
    #endregion

    private int _id, _Number, _AdPacksLimit, _AcceleratedProfitPercentage;
    string _Color;
    bool _IsHighestGroup;
    public AutomaticGroup()
            : base() { }

    public AutomaticGroup(int id) : base(id) { }

    public AutomaticGroup(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }
}