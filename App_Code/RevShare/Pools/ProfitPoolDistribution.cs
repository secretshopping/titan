using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using ExtensionMethods;


public class ProfitPoolDistribution : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "ProfitPoolDistribution"; } }
    protected override string dbTable { get { return TableName; } }
    public static class Columns
    {
        public const string Id = "Id";
        public const string ProfitSource = "ProfitSource";
        public const string Pool = "Pool";
        public const string ProfitPoolPercent = "ProfitPoolPercent";

    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.ProfitSource)]
    private int ProfitSourceInt { get { return _ProfitSource; } set { _ProfitSource = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Pool)]
    public int Pool { get { return _Pool; } set { _Pool = value; SetUpToDateAsFalse(); } }

    [Column(Columns.ProfitPoolPercent)]
    public decimal ProfitPoolPercent { get { return _ProfitPoolPercent; } set { _ProfitPoolPercent = value; SetUpToDateAsFalse(); } }

    int _id, _ProfitSource, _Pool;
    decimal _ProfitPoolPercent;

    public ProfitSource ProfitSource
    {
        get
        {
            return (ProfitSource)ProfitSourceInt;
        }
        set
        {
            ProfitSourceInt = (int)value;
        }
    }

    //public Pools TargetPool
    //{
    //    get
    //    {
    //        return (Pools)PoolInt;
    //    }
    //    set
    //    {
    //        PoolInt = (int)value;
    //    }
    //}

    public ProfitPoolDistribution()
            : base() { }

    public ProfitPoolDistribution(int id) : base(id) { }

    public ProfitPoolDistribution(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    #endregion

    public static void Delete(int profitPoolDistributionRuleId)
    {
        ProfitPoolDistribution rule = new ProfitPoolDistribution(profitPoolDistributionRuleId);

        if (new Pool(rule.Pool).Name != "Administrator Profit")
        {
            var adminRule = GetAdministratorProfitRule(rule.ProfitSource);
            if (adminRule.ProfitPoolPercent + rule.ProfitPoolPercent > 100)
                adminRule.ProfitPoolPercent = 100;
            else
                adminRule.ProfitPoolPercent += rule.ProfitPoolPercent;
            adminRule.Save();
            rule.Delete();
        }
    }

    public static void Add(ProfitSource source, Pool targetPool, decimal percent)
    {
        var adminRule = GetAdministratorProfitRule(source);

        if (targetPool.Name != "Administrator Profit")
        {
            decimal difference = 0;

            ProfitPoolDistribution newRule = null;

            var where = TableHelper.MakeDictionary(Columns.ProfitSource, (int)source);
            where.Add(Columns.Pool, targetPool.Id);
            var whereRule = TableHelper.SelectRows<ProfitPoolDistribution>(where);

            if (whereRule.Count > 0)
            {
                newRule = whereRule[0];
                difference = newRule.ProfitPoolPercent - percent;
            }
            else
            {
                newRule = new ProfitPoolDistribution();
                difference = -percent;
            }

            newRule.ProfitPoolPercent = percent;
            newRule.ProfitSource = source;
            newRule.Pool = targetPool.Id;
            newRule.Save();

            adminRule.ProfitPoolPercent += difference;
            adminRule.Save();
        }
    }

    public static ProfitPoolDistribution GetAdministratorProfitRule(ProfitSource source)
    {
        var query = string.Format("SELECT TOP 1 * FROM ProfitPoolDistribution WHERE Pool = {0} AND ProfitSource = {1}",
            PoolsHelper.GetBuiltInProfitPoolId(Pools.AdministratorProfit), (int)source);

        var adminRule = TableHelper.GetListFromRawQuery<ProfitPoolDistribution>(query).SingleOrDefault();

        if (adminRule == null)
        {
            adminRule = new ProfitPoolDistribution()
            {
                ProfitPoolPercent = 100,
                ProfitSource = source,
                Pool = PoolsHelper.GetBuiltInProfitPoolId(Pools.AdministratorProfit)
            };
            adminRule.Save();
        }
        return adminRule;
    }

    public static bool DistributionForPvpJackpotExists()
    {
        if (Convert.ToInt32(TableHelper.SelectScalar(String.Format("SELECT COUNT(*) FROM ProfitPoolDistribution WHERE ProfitSource={0}", (int)ProfitSource.JackpotPvp))) > 0)
            return true;

        return false;
    }
}