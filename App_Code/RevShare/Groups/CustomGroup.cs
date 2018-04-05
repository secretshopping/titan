using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

[Serializable]
public class CustomGroup : BaseTableObject
{

    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "CustomGroups"; } }
    protected override string dbTable { get { return TableName; } }
    public static class Columns
    {
        
        public const string Id = "Id";
        public const string Number = "Number";
        public const string AdPacksLimit = "AdPacksLimit"; 
        public const string AcceleratedProfitPercentage = "AcceleratedProfitPercentage";
        public const string NumberOfGroupsLimit = "NumberOfGroupsLimit";
        public const string CreatorsMinNumberOfAdPacks = "CreatorsMinNumberOfAdPacks";
        public const string Color = "Color";
        public const string EnterLeaveAdPackMaxFillPercent = "EnterLeaveAdPackMaxFillPercent";
        public const string CreatorsMaxNumberOfAdPacks = "CreatorsMaxNumberOfAdPacks";
        public const string UsersMaxNumberOfAdPacks = "UsersMaxNumberOfAdPacks";
        public const string CreatorRewardBonusPercent = "CreatorRewardBonusPercent";
        public const string FirstRewardPercent = "FirstRewardPercent";
        public const string SecondRewardPercent = "SecondRewardPercent";
    }


    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Number)]
    public int Number { get { return _Number; } set { _Number = value; SetUpToDateAsFalse(); } }

    [Column(Columns.AdPacksLimit)]
    public int AdPacksLimit { get { return _AdPacksLimit; }  set { _AdPacksLimit = value; SetUpToDateAsFalse(); } }

    [Column(Columns.AcceleratedProfitPercentage)]
    public int AcceleratedProfitPercentage { get { return _AcceleratedProfitPercentage; }  set { _AcceleratedProfitPercentage = value; SetUpToDateAsFalse(); } }

    [Column(Columns.NumberOfGroupsLimit)]
    public int NumberOfGroupsLimit { get { return _NumberOfGroupsLimit; } set { _NumberOfGroupsLimit = value; SetUpToDateAsFalse(); } }

    [Column(Columns.CreatorsMinNumberOfAdPacks)]
    public int CreatorsMinNumberOfAdPacks { get { return _CreatorsMinNumberOfAdPacks; } set { _CreatorsMinNumberOfAdPacks = value; SetUpToDateAsFalse(); } }

    [Column(Columns.EnterLeaveAdPackMaxFillPercent)]
    public int EnterLeaveAdPackMaxFillPercent { get { return _EnterLeaveAdPackMaxFillPercent; } set { _EnterLeaveAdPackMaxFillPercent = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Color)]
    public string Color { get { return _Color; } set { _Color = value; SetUpToDateAsFalse(); } }

    [Column(Columns.CreatorsMaxNumberOfAdPacks)]
    public int CreatorsMaxNumberOfAdPacks { get { return _CreatorsMaxNumberOfAdPacks; } set { _CreatorsMaxNumberOfAdPacks = value; SetUpToDateAsFalse(); } }

    [Column(Columns.UsersMaxNumberOfAdPacks)]
    public int UsersMaxNumberOfAdPacks { get { return _UsersMaxNumberOfAdPacks; } set { _UsersMaxNumberOfAdPacks = value; SetUpToDateAsFalse(); } }

    [Column(Columns.CreatorRewardBonusPercent)]
    public int CreatorRewardBonusPercent { get { return _CreatorRewardBonusPercent; } set { _CreatorRewardBonusPercent = value; SetUpToDateAsFalse(); } }

    [Column(Columns.FirstRewardPercent)]
    public int FirstRewardPercent { get { return _FirstRewardPercent; } set { _FirstRewardPercent = value; SetUpToDateAsFalse(); } }

    [Column(Columns.SecondRewardPercent)]
    public int SecondRewardPercent { get { return _SecondRewardPercent; } set { _SecondRewardPercent = value; SetUpToDateAsFalse(); } }

    #endregion

    private int _id, _Number, _AdPacksLimit, _AcceleratedProfitPercentage, _NumberOfGroupsLimit, _CreatorsMinNumberOfAdPacks, _EnterLeaveAdPackMaxFillPercent,
        _CreatorsMaxNumberOfAdPacks, _UsersMaxNumberOfAdPacks, _CreatorRewardBonusPercent, _FirstRewardPercent, _SecondRewardPercent;
    string _Color;
    public CustomGroup()
            : base() { }

    public CustomGroup(int id) : base(id) { }

    public CustomGroup(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }
}