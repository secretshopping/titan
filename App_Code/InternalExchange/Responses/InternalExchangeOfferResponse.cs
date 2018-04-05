using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Prem.PTC;

/// <summary>
/// Summary description for InternalExchangeOfferResponse
/// </summary>
public class InternalExchangeOfferResponse : BaseTableObject
{
    #region model

    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return String.Empty; } }
    protected override string dbTable { get { return TableName; } }

    public static class Columns
    {
        public const string Id = "Id";
        public const string AmountTransferd = "AmountTransferd";
        public const string ValueTransferd = "ValueTransferd";
        public const string AmountPlaced = "AmountPlaced";
        public const string ValuePlaced = "ValuePlaced";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return 1; } protected set {  } }
    
    [Column(Columns.AmountTransferd)]
    public decimal AmountTransferd { get { return _AmountTransferd; } protected set { _AmountTransferd = value;} }

    [Column(Columns.ValueTransferd)]
    public decimal ValueTransferd { get { return _ValueTransferd; } protected set { _ValueTransferd = value;  } }

    [Column(Columns.AmountPlaced)]
    public decimal AmountPlaced { get { return _AmountPlaced; } protected set { _AmountPlaced = value; } }

    [Column(Columns.ValuePlaced)]
    public decimal ValuePlaced { get { return _ValuePlaced; } protected set { _ValuePlaced = value; } }
    
    private decimal _AmountTransferd, _ValueTransferd, _AmountPlaced, _ValuePlaced;

    #endregion

    public InternalExchangeOfferResponse() : base() { }
    //public InternalExchangeOfferResponse(int id) : base(id) { }
    public InternalExchangeOfferResponse(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }
}