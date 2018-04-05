using ExtensionMethods;
using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;

namespace Titan.ICO
{
    [Serializable]
    public class InternalExchangeTransaction : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "InternalExchangeTransactions"; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "TransactionId";
            public const string AskUserId = "AskUserId";
            public const string BidUserId = "BidUserId";
            public const string TransactionAmount = "TransactionAmount";
            public const string TransactionValue = "TransactionValue";
            public const string TransactionDate = "TransactionDate";
            public const string IsAsk = "IsAsk";
            public const string AskFee = "AskFee";
            public const string BidFee = "BidFee";
            public const string AskId = "AskId";
            public const string BidId = "BidId";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return id; } protected set { id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AskUserId)]
        public int AskUserId { get { return askUserId; } set { askUserId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.BidUserId)]
        public int BidUserId { get { return bidUserId; } set { bidUserId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TransactionAmount)]
        public decimal TransactionAmount { get { return transactionAmount; } set { transactionAmount = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TransactionValue)]
        public decimal TransactionValue { get { return transactionValue; } set { transactionValue = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TransactionDate)]
        public DateTime TransactionDate { get { return transactionDate; } set { transactionDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsAsk)]
        public bool IsAsk { get { return isAsk; } set { isAsk = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AskFee)]
        public decimal AskFee { get { return askFee; } set { askFee = value; SetUpToDateAsFalse(); } }

        [Column(Columns.BidFee)]
        public decimal BidFee { get { return bidFee; } set { bidFee = value; SetUpToDateAsFalse(); } }


        private int id, askUserId, bidUserId;
        private decimal transactionAmount, transactionValue, askFee, bidFee;
        private DateTime transactionDate;
        private bool isAsk;

        public InternalExchangeTransaction() : base() { }
        public InternalExchangeTransaction(int id) : base(id) { }
        public InternalExchangeTransaction(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #region Statistics
        public static decimal GetLastStockValue(bool currentData = true)
        {
            if (currentData)
            {
                InternalExchangeStatistics internalExchangeStatistics = (InternalExchangeStatistics)new InternalExchangeStatisticsCache().Get();
                return internalExchangeStatistics.LastValue;
            }
            else
            {
                InternalExchangeStatistics internalExchangeOldStatistics = (InternalExchangeStatistics)new InternalExchangeOldStatisticsCache().Get();
                return internalExchangeOldStatistics.LastValue;
            }
        }

        public static decimal GetLastBidValue(bool currentData = true)
        {
            if (currentData)
            {
                InternalExchangeStatistics internalExchangeStatistics = (InternalExchangeStatistics)new InternalExchangeStatisticsCache().Get();
                return internalExchangeStatistics.LastBid;
            }
            else
            {
                InternalExchangeStatistics internalExchangeOldStatistics = (InternalExchangeStatistics)new InternalExchangeOldStatisticsCache().Get();
                return internalExchangeOldStatistics.LastBid;
            }   
        }

        public static decimal GetLastAskValue(bool currentData = true)
        {
            if (currentData)
            {
                InternalExchangeStatistics internalExchangeStatistics = (InternalExchangeStatistics)new InternalExchangeStatisticsCache().Get();
                return internalExchangeStatistics.LastAsk;
            }
            else
            {
                InternalExchangeStatistics internalExchangeOldStatistics = (InternalExchangeStatistics)new InternalExchangeOldStatisticsCache().Get();
                return internalExchangeOldStatistics.LastAsk;
            }
                
        }

        public static decimal GetLast24hHigh(bool currentData = true)
        {
            if (currentData)
            {
                InternalExchangeStatistics internalExchangeStatistics = (InternalExchangeStatistics)new InternalExchangeStatisticsCache().Get();
                return internalExchangeStatistics.Max24;
            }
            else
            {
                InternalExchangeStatistics internalExchangeOldStatistics = (InternalExchangeStatistics)new InternalExchangeOldStatisticsCache().Get();
                return internalExchangeOldStatistics.Max24;
            }    
        }

        public static decimal GetLast24hLow(bool currentData = true)
        {
            if (currentData)
            {
                InternalExchangeStatistics internalExchangeStatistics = (InternalExchangeStatistics)new InternalExchangeStatisticsCache().Get();
                return internalExchangeStatistics.Min24;
            }
            else
            {
                InternalExchangeStatistics internalExchangeOldStatistics = (InternalExchangeStatistics)new InternalExchangeOldStatisticsCache().Get();
                return internalExchangeOldStatistics.Min24;
            }
        }

        public static decimal GetLast24hVolume(bool currentData = true)
        {
            if (currentData)
            {
                InternalExchangeStatistics internalExchangeStatistics = (InternalExchangeStatistics)new InternalExchangeStatisticsCache().Get();
                return internalExchangeStatistics.Change24;
            }
            else
            {
                InternalExchangeStatistics internalExchangeOldStatistics = (InternalExchangeStatistics)new InternalExchangeOldStatisticsCache().Get();
                return internalExchangeOldStatistics.Change24;
            }
        }
        
        public static decimal GetLastStockValueBefore24h()
        {
            var value = TableHelper.SelectScalar(String.Format("SELECT TOP(1) TransactionValue FROM InternalExchangeTransactions WHERE TransactionDate<'{0}' ORDER BY TransactionDate DESC", AppSettings.ServerTime.AddDays(-1).ToDBString()));
            if (value is DBNull || value == null)
                return 0;
            else
                return (decimal)value;
        }

        public static decimal GetValueOfOffers(bool askOffers)
        {
            String Query = askOffers ?  "SELECT SUM(AskAmount*AskValue) FROM InternalExchangeAsks WHERE AskStatus=1" : 
                                        "SELECT SUM(BidAmount*BidValue) FROM InternalExchangeBids WHERE BidStatus=1" ;
            try
            {
                return (decimal)TableHelper.SelectScalar(Query);
            }
            catch (Exception ex)
            {
                return 0m;
            }
        }

        public static decimal GetSumOfStockForOffers(bool askOffers)
        {
            String Query = askOffers ? "SELECT SUM(AskAmount) FROM InternalExchangeAsks WHERE AskStatus=1" :
                                        "SELECT SUM(BidAmount) FROM InternalExchangeBids WHERE BidStatus=1";
            try
            {
                return (decimal)TableHelper.SelectScalar(Query);
            }
            catch (Exception ex)
            {
                return 0m;
            }
        }

        #endregion

        #region Offer Detail

        public static List<InternalExchangeTransaction> GetAskDetail(int askId)
        {
            var where = TableHelper.MakeDictionary(Columns.AskId, askId);
            return TableHelper.SelectRows<InternalExchangeTransaction>(where);
        }

        public static List<InternalExchangeTransaction> GetBidDetail(int bidId)
        {
            var where = TableHelper.MakeDictionary(Columns.BidId, bidId);
            return TableHelper.SelectRows<InternalExchangeTransaction>(where);
        }

        #endregion
    }
}