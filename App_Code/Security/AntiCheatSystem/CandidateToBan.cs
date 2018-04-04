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


public class CandidateToBan : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "CandidatesToBan"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("RuleTypeInt")]
    public int RuleTypeInt { get { return _RuleTypeInt; } set { _RuleTypeInt = value; SetUpToDateAsFalse(); } }

    [Column("Note")]
    public string Note { get { return _RuleText; } set { _RuleText = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("DateOccured")]
    public DateTime DateOccured { get { return _DateOccured; } set { _DateOccured = value; SetUpToDateAsFalse(); } }

    [Column("IsWhitelisted")]
    public bool IsWhitelisted { get { return _IsWhitelisted; } set { _IsWhitelisted = value; SetUpToDateAsFalse(); } }

    private int _id, _RuleTypeInt, _UserId;
    private string _RuleText;
    private bool _Enabled, _IsWhitelisted;
    private DateTime _DateOccured;

    public AntiCheatRuleType Type
    {
        get
        {
            return (AntiCheatRuleType)RuleTypeInt;
        }
        set
        {
            RuleTypeInt = (int)value;
        }
    }

    #endregion Columns

    public CandidateToBan()
        : base()
    { }

    public CandidateToBan(int id) : base(id) { }

    public CandidateToBan(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate)
    { }

  
}
