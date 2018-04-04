using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Prem.PTC;

public class InternalExchangeStatistics : BaseTableObject
{
    #region model

    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return String.Empty; } }
    protected override string dbTable { get { return TableName; } }

    public static class Columns
    {
        public const string Id = "Id";
        public const string LastValue = "LastValue";
        public const string LastBid = "LastBid";
        public const string LastAsk = "LastAsk";
        public const string Max24 = "Max24";
        public const string Min24 = "Min24";
        public const string Change24 = "Change24";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return 1; } protected set { } }

    [Column(Columns.LastValue)]
    public decimal LastValue { get { return _LastValue; } protected set { _LastValue = value; } }

    [Column(Columns.LastBid)]
    public decimal LastBid { get { return _LastBid; } protected set { _LastBid = value; } }

    [Column(Columns.LastAsk)]
    public decimal LastAsk { get { return _LastAsk; } protected set { _LastAsk = value; } }

    [Column(Columns.Max24)]
    public decimal Max24 { get { return _Max24; } protected set { _Max24 = value; } }

    [Column(Columns.Min24)]
    public decimal Min24 { get { return _Min24; } protected set { _Min24 = value; } }

    [Column(Columns.Change24)]
    public decimal Change24 { get { return _Change24; } protected set { _Change24 = value; } }

    private decimal _LastValue, _LastBid, _LastAsk, _Max24, _Min24, _Change24;

    #endregion

    public InternalExchangeStatistics() : base()
    {
        LastValue = 0m;
        LastBid = 0m;
        LastAsk = 0m;
        Max24 = 0m;
        Min24 = 0m;
        Change24 = 0m;
    }
    //public InternalExchangeOfferResponse(int id) : base(id) { }
    public InternalExchangeStatistics(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

    public static string SelectCommand
    {
        get
        {
            return
                @"SELECT TOP(1)
					1 AS Id
					, ISNULL(LastValue, 0.00000001) AS LastValue
					, ISNULL(LastBid, 0) AS LastBid
					, ISNULL(LastAsk, 0) AS LastAsk
					, ISNULL(Max24, 0) AS Max24
					, ISNULL(Min24, 0) AS Min24
					, 
					CASE
						WHEN LastValue > LastValue24 THEN (LastValue - LastValue24) / LastValue * 100.0
						WHEN LastValue < LastValue24 THEN (LastValue - LastValue24) / LastValue24 * 100.0
						ELSE 0.0
					END AS Change24
				FROM
				(SELECT TOP(1) TransactionValue AS LastValue FROM InternalExchangeTransactions ORDER BY TransactionDate DESC
				) lastValue
				LEFT JOIN (SELECT TOP(1) TransactionValue AS LastBid FROM InternalExchangeTransactions WHERE IsAsk=0 ORDER BY TransactionDate DESC
				) lastBid ON 1=1						 
				LEFT JOIN (SELECT TOP(1) TransactionValue AS LastAsk FROM InternalExchangeTransactions WHERE IsAsk=1 ORDER BY TransactionDate DESC
				) lastAsk ON 1=1
				LEFT JOIN (SELECT MAX(TransactionValue) AS Max24 FROM InternalExchangeTransactions WHERE TransactionDate>GETDATE()-1
				) max24 ON 1=1
				LEFT JOIN (SELECT MIN(TransactionValue) AS Min24 FROM InternalExchangeTransactions WHERE TransactionDate>GETDATE()-1
				) min24 ON 1=1
				LEFT JOIN (SELECT TOP(1) TransactionValue AS LastValue24 FROM InternalExchangeTransactions WHERE TransactionDate<=GETDATE()-1 ORDER BY TransactionDate DESC
				) last24 ON 1=1";
        }
    }

    public static string SelectCommandForOld
    {
        get
        {
            return
                @"SELECT TOP(1)
                    1 AS Id
				    , *
				FROM
                (
                SELECT TransactionValue AS LastValue FROM InternalExchangeTransactions ORDER BY TransactionDate DESC OFFSET 1 ROW FETCH NEXT 1 ROW ONLY
                ) lastValue
                , (SELECT ISNULL(TransactionValue, 0) AS LastBid FROM InternalExchangeTransactions WHERE IsAsk=0 ORDER BY TransactionDate DESC OFFSET 1 ROW FETCH NEXT 1 ROW ONLY
                ) lastBid
                , (SELECT ISNULL(TransactionValue, 0) AS LastAsk FROM InternalExchangeTransactions WHERE IsAsk=1 ORDER BY TransactionDate DESC OFFSET 1 ROW FETCH NEXT 1 ROW ONLY
                ) lastAsk
                , (SELECT ISNULL(MAX(TransactionValue), 0) AS Max24 FROM InternalExchangeTransactions WHERE TransactionDate<GETDATE()-1
                ) max24
                , (SELECT ISNULL(MIN(TransactionValue), 0) AS Min24 FROM InternalExchangeTransactions WHERE TransactionDate<GETDATE()-1
                ) min24
                , (SELECT ISNULL(SUM(TransactionAmount), 0) AS Sum24 FROM InternalExchangeTransactions WHERE TransactionDate>GETDATE()-2 AND TransactionDate<GETDATE()-1
                ) sum24";
        }
    }
}