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


public class AntiCheatSystemRule : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "AntiCheatSystemRules"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("RuleTypeInt")]
    public int RuleTypeInt { get { return _RuleTypeInt; } set { _RuleTypeInt = value; SetUpToDateAsFalse(); } }

    [Column("RuleText")]
    public string RuleText { get { return _RuleText; } set { _RuleText = value; SetUpToDateAsFalse(); } }

    [Column("Enabled")]
    public bool Enabled { get { return _Enabled; } set { _Enabled = value; SetUpToDateAsFalse(); } }

    private int _id, _RuleTypeInt;
    private string _RuleText;
    private bool _Enabled;

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

    public AntiCheatSystemRule()
        : base()
    { }

    public AntiCheatSystemRule(int id) : base(id) { }

    public AntiCheatSystemRule(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate)
    { }


}
