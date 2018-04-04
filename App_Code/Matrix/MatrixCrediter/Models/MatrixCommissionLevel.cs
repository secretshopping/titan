using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using Prem.PTC;
using Prem.PTC.Utils;

public class MatrixCommissionLevel : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "MatrixCommissionLevels"; } }
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

    public MatrixCommissionLevel() : base() { }

    public MatrixCommissionLevel(int id) : base(id) { }

    public MatrixCommissionLevel(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

    #endregion

    #region Helpers

    public void SaveMatrixCommissionLevels()
    {
        PropertyInfo[] propertiesToSave = BuildLevelCommission();

        SavePartially(IsUpToDate, propertiesToSave);
    }

    public void ReloadMatrixCommissionLevels()
    {
        PropertyInfo[] propertiesToReload = BuildLevelCommission();

        ReloadPartially(IsUpToDate, propertiesToReload);
    }

    private PropertyInfo[] BuildLevelCommission()
    {
        PropertyBuilder<MatrixCommissionLevel> builder = new PropertyBuilder<MatrixCommissionLevel>();

        builder.Append(x => x.CommissionLevel)
               .Append(x => x.Commission);

        return builder.Build();
    }

    public static MatrixCommissionLevel GetByLevel(int commissionLevel)
    {
        var result = TableHelper.SelectRows<MatrixCommissionLevel>(TableHelper.MakeDictionary(Columns.CommissionLevel, commissionLevel)).FirstOrDefault();
        
        if(result == null)
        {
            result = new MatrixCommissionLevel()
            {
                CommissionLevel = commissionLevel,
                Commission = 0.0m
            };
        }

        return result;
    }

    public static List<MatrixCommissionLevel> GetAvailableLevels()
    {
        List<MatrixCommissionLevel> list = TableHelper.SelectAllRows<MatrixCommissionLevel>();

        for (int i = 1; i <= AppSettings.Matrix.MatrixMaxCreditedLevels; i++)
        {
            if (!list.Exists(x => x.CommissionLevel == i))
            {
                list.Add(new MatrixCommissionLevel()
                {
                    CommissionLevel = i,
                    Commission = 0.00m
                });
            }
        }

        list.RemoveAll(x => x.CommissionLevel > AppSettings.Matrix.MatrixMaxCreditedLevels);
        list = list.OrderBy(x => x.CommissionLevel).ToList();
        
        return list;
    }

    public void Update(int id, string commissionLevel, string commission)
    {
        if (commission != "0")
        {
            this.Id = id;
            this.CommissionLevel = Int32.Parse(commissionLevel);
            this.Commission = Decimal.Parse(commission);

            this.Save();
        }
    }

    #endregion
}