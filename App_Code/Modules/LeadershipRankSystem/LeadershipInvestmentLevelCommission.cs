using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using Prem.PTC;
using Prem.PTC.Utils;

public class LeadershipInvestmentLevelCommission : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "LeadershipInvestmentLevelCommission"; } }
    protected override string dbTable { get { return TableName; } }

    public static class Columns
    {
        public const string Id = "Id";
        public const string CommissionLevel = "CommissionLevel";
        public const string Commission = "Commission";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }
    
    [Column(Columns.CommissionLevel)]
    public int CommissionLevel{ get { return _commissionLevel; } set { _commissionLevel = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Commission)]
    public decimal Commission { get { return _commission; } set { _commission = value; SetUpToDateAsFalse(); } }

    int _id, _commissionLevel;
    decimal _commission;

    public LeadershipInvestmentLevelCommission() : base() { }

    public LeadershipInvestmentLevelCommission(int id) : base(id) { }

    public LeadershipInvestmentLevelCommission(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

    #endregion

    #region Helpers

    public void SaveInvestmentPlans()
    {
        PropertyInfo[] propertiesToSave = BuildLevelCommission();

        SavePartially(IsUpToDate, propertiesToSave);
    }

    public void ReloadInvestmentPlans()
    {
        PropertyInfo[] propertiesToReload = BuildLevelCommission();

        ReloadPartially(IsUpToDate, propertiesToReload);
    }

    private PropertyInfo[] BuildLevelCommission()
    {
        PropertyBuilder<LeadershipInvestmentLevelCommission> builder = new PropertyBuilder<LeadershipInvestmentLevelCommission>();

        builder.Append(x => x.CommissionLevel)
               .Append(x => x.Commission);

        return builder.Build();
    }

    public static LeadershipInvestmentLevelCommission GetByLevel(int commissionLevel)
    {
        var result = TableHelper.SelectRows<LeadershipInvestmentLevelCommission>(TableHelper.MakeDictionary(Columns.CommissionLevel, commissionLevel)).FirstOrDefault();
        
        if(result == null)
        {
            result = new LeadershipInvestmentLevelCommission()
            {
                CommissionLevel = commissionLevel,
                Commission = 0.0m
            };
        }

        return result;
    }

    public static List<LeadershipInvestmentLevelCommission> GetAvailableLevels()
    {
        List<LeadershipInvestmentLevelCommission> list = TableHelper.SelectAllRows<LeadershipInvestmentLevelCommission>();

        for (int i = 0; i <= AppSettings.Referrals.ReferralEarningsUpToTier; i++)
        {
            if (!list.Exists(x => x.CommissionLevel == i))
            {
                list.Add(new LeadershipInvestmentLevelCommission()
                {
                    CommissionLevel = i,
                    Commission = i == 0 ? 100.00m : 0.00m
                });
            }
        }

        list.RemoveAll(x => x.CommissionLevel > AppSettings.Referrals.ReferralEarningsUpToTier);
        list = list.OrderBy(x => x.CommissionLevel).ToList();
        
        return list;
    }

    #endregion
}