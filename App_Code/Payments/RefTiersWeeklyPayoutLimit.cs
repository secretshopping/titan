using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for RefTiersWeeklyPayoutLimit
/// </summary>
public class RefTiersWeeklyPayoutLimit :BaseTableObject
{
    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "RefTiersWeeklyPayoutLimits"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("Amount")]
    public Money Amount { get { return _Amount; } set { _Amount = value; SetUpToDateAsFalse(); } }

    [Column("Tier")]
    public int Tier { get { return _Tier; } set { _Tier = value; SetUpToDateAsFalse(); } }

    int _id, _Tier;
    Money _Amount;
    #endregion

    public RefTiersWeeklyPayoutLimit()
            : base() { }

    public RefTiersWeeklyPayoutLimit(int id) : base(id) { }

    public RefTiersWeeklyPayoutLimit(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }
}

public static class RefTiersWeeklyPayoutLimitHelper
{
    public static Money GetUserLimit(Member user)
    {
        var rules = TableHelper.GetListFromRawQuery<RefTiersWeeklyPayoutLimit>("SELECT * FROM RefTiersWeeklyPayoutLimits");

        if (rules.Count <= 0)
            return null;

        IndirectReferralsHelper iRHelper = new IndirectReferralsHelper(user);
        var indirectRefs = iRHelper.HowManyEverUpgradedOnEachTier;

        // tier 0 = no refs
        int maxTier = 0;
        for (int i = 1; i <= AppSettings.Referrals.ReferralEarningsUpToTier; i++)
        {
            if (indirectRefs[i] <= 0)
                break;

            maxTier = i;
        }

        var rule = rules.Where(r => r.Tier == maxTier).FirstOrDefault();

        if (rule == null)
            return null;

        return rule.Amount;
    }
}