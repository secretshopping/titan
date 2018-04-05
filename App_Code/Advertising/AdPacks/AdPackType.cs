using Prem.PTC;
using Prem.PTC.Advertising;
using System;
using System.Data;

[Serializable]
public class AdPackType : BaseTableObject
{

    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "AdPackTypes"; } }
    protected override string dbTable { get { return TableName; } }
    public static class Columns
    {
        public const string Id = "Id";
        public const string Price = "Price";
        public const string AdBalanceReturnPercentage = "AdBalanceReturnPercentage";
        public const string PackReturnValuePercentage = "PackReturnValuePercentage";
        public const string MaxInstances = "MaxInstances";
        public const string Name = "Name";
        public const string Number = "Number";
        public const string Color = "Color";
        public const string MinNumberOfPreviousType = "MinNumberOfPreviousType";
        public const string Status = "Status";
        public const string RequiredMembership = "RequiredMembership";
        public const string Clicks = "Clicks";
        public const string DisplayTime = "DisplayTime";
        public const string NormalBannerImpressions = "NormalBannerImpressions";
        public const string ConstantBannerImpressions = "ConstantBannerImpressions";
        public const string ValueOf1SecondInClicks = "ValueOf1SecondInClicks";
        public const string CustomGroupsEnabled = "CustomGroupsEnabled";
        public const string FixedDistributionValuePercent = "FixedDistributionValuePercent";
        public const string FixedDistributionValuePercent2 = "FixedDistributionValuePercent2";
        public const string LoginAdsCredits = "LoginAdsCredits";
        public const string WithdrawLimitPercentage = "WithdrawLimitPercentage";
        public const string TrafficExchangeSurfCredits = "TrafficExchangeSurfCredits";
        public const string MaxInstancesOfAllAdpacks = "MaxInstancesOfAllAdpacks";
        public const string MarketplaceBalanceReturnPercentage = "MarketplaceBalanceReturnPercentage";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Price)]
    public Money Price { get { return _Price; } set { _Price = value; SetUpToDateAsFalse(); } }

    [Column(Columns.AdBalanceReturnPercentage)]
    public int AdBalanceReturnPercentage { get { return _AdBalanceReturnPercentage; } set { _AdBalanceReturnPercentage = value; SetUpToDateAsFalse(); } }

    [Column(Columns.PackReturnValuePercentage)]
    public int PackReturnValuePercentage { get { return _PackReturnValuePercentage; } set { _PackReturnValuePercentage = value; SetUpToDateAsFalse(); } }

    [Column(Columns.MaxInstances)]
    public int MaxInstances { get { return _MaxInstances; } set { _MaxInstances = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Number)]
    public int Number { get { return _Number; } set { _Number = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Color)]
    public string Color { get { return _Color; } set { _Color = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Name)]
    public string Name { get { return _Name; } set { _Name = value; SetUpToDateAsFalse(); } }

    [Column(Columns.MinNumberOfPreviousType)]
    public int MinNumberOfPreviousType { get { return Number == 0 ? 0 : _MinNumberOfPreviousType; } set { _MinNumberOfPreviousType = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Status)]
    protected int StatusInt { get { return _Status; } set { _Status = value; SetUpToDateAsFalse(); } }

    //5006

    [Column(Columns.RequiredMembership)]
    public int RequiredMembership { get { return _RequiredMembership; } set { _RequiredMembership = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Clicks)]
    public int Clicks { get { return _Clicks; } set { _Clicks = value; SetUpToDateAsFalse(); } }

    [Column(Columns.DisplayTime)]
    public int DisplayTime { get { return _DisplayTime; } set { _DisplayTime = value; SetUpToDateAsFalse(); } }

    [Column(Columns.NormalBannerImpressions)]
    public int NormalBannerImpressions { get { return _NormalBannerImpressions; } set { _NormalBannerImpressions = value; SetUpToDateAsFalse(); } }

    [Column(Columns.ConstantBannerImpressions)]
    public int ConstantBannerImpressions { get { return _ConstantBannerImpressions; } set { _ConstantBannerImpressions = value; SetUpToDateAsFalse(); } }

    [Column(Columns.ValueOf1SecondInClicks)]
    public int ValueOf1SecondInClicks { get { return _ValueOf1SecondInClicks; } set { _ValueOf1SecondInClicks = value; SetUpToDateAsFalse(); } }

    [Column(Columns.CustomGroupsEnabled)]
    public bool CustomGroupsEnabled { get { return _CustomGroupsEnabled; } set { _CustomGroupsEnabled = value; SetUpToDateAsFalse(); } }

    [Column(Columns.FixedDistributionValuePercent)]
    public Decimal FixedDistributionValuePercent { get { return _FixedDistributionValuePercent; } set { _FixedDistributionValuePercent = value; SetUpToDateAsFalse(); } }

    [Column(Columns.FixedDistributionValuePercent2)]
    public Decimal FixedDistributionValuePercent2 { get { return _FixedDistributionValuePercent2; } set { _FixedDistributionValuePercent2 = value; SetUpToDateAsFalse(); } }

    [Column(Columns.LoginAdsCredits)]
    public int LoginAdsCredits { get { return _LoginAdsCredits; } set { _LoginAdsCredits = value; SetUpToDateAsFalse(); } }

    [Column(Columns.WithdrawLimitPercentage)]
    public int WithdrawLimitPercentage { get { return _WithdrawLimitPercentage; } set { _WithdrawLimitPercentage = value; SetUpToDateAsFalse(); } }

    [Column(Columns.TrafficExchangeSurfCredits)]
    public Money TrafficExchangeSurfCredits { get { return _TrafficExchangeSurfCredits; } set { _TrafficExchangeSurfCredits = value; SetUpToDateAsFalse(); } }

    [Column(Columns.MaxInstancesOfAllAdpacks)]
    public int MaxInstancesOfAllAdpacks { get { return _MaxInstancesOfAllAdpacks; } set { _MaxInstancesOfAllAdpacks = value; SetUpToDateAsFalse(); } }

    [Column(Columns.MarketplaceBalanceReturnPercentage)]
    public int MarketplaceBalanceReturnPercentage { get { return _MarketplaceBalanceReturnPercentage; } set { _MarketplaceBalanceReturnPercentage = value; SetUpToDateAsFalse(); } }

    #endregion

    private int _id, _Number, _AdBalanceReturnPercentage, _PackReturnValuePercentage, _MaxInstances, _MinNumberOfPreviousType, _Status,
        _RequiredMembership, _Clicks, _DisplayTime, _NormalBannerImpressions, _ConstantBannerImpressions, _ValueOf1SecondInClicks,
        _LoginAdsCredits, _WithdrawLimitPercentage, _MaxInstancesOfAllAdpacks, _MarketplaceBalanceReturnPercentage;
    Money _Price, _TrafficExchangeSurfCredits;
    string _Color, _Name;
    bool _CustomGroupsEnabled;
    Decimal _FixedDistributionValuePercent, _FixedDistributionValuePercent2;

    public AdPackTypeStatus Status
    {
        get { return (AdPackTypeStatus)StatusInt; }
        set { StatusInt = (int)value; }
    }

    public AdPackType()
            : base()
    { }

    public AdPackType(int id) : base(id) { }

    public AdPackType(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    { }


    public static AdPackType Standard
    {
        get
        {
            return TableHelper.SelectRows<AdPackType>(TableHelper.MakeDictionary("Number", 0))[0];
        }
    }
}