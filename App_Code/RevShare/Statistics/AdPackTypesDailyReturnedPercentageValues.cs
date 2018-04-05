using System;
using Prem.PTC;
using System.Data;

public class AdPackTypesDailyReturnedPercentValue : BaseTableObject
{
    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "AdPackTypesDailyReturnedPercentageValues"; } }
    protected override string dbTable { get { return TableName; } }

    public static class Columns
    {
        public const string Id = "Id";
        public const string AdPackTypeId = "AdPackTypeId";
        public const string Date = "Date";
        public const string PercentageValue = "PercentageValue";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.AdPackTypeId)]
    public int AdPackTypeId { get { return _adPackTypeId; } set { _adPackTypeId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Date)]
    public DateTime Date { get { return _date; } set { _date = value; SetUpToDateAsFalse(); } }

    [Column(Columns.PercentageValue)]
    public decimal PercentageValue { get { return _percentageValue; } set { _percentageValue = value; SetUpToDateAsFalse(); } }

    private int _id, _adPackTypeId;
    private DateTime _date;
    private decimal _percentageValue;

    public AdPackTypesDailyReturnedPercentValue() : base() { }
    public AdPackTypesDailyReturnedPercentValue(int id) : base(id) { }
    public AdPackTypesDailyReturnedPercentValue(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }
    #endregion Columns    
}
