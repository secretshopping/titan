using ExtensionMethods;
using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using Titan.InternalExchange;

namespace Titan.ICO
{
    [Serializable]
    public class InternalExchangeBid : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "InternalExchangeBids"; } }
        protected override string dbTable { get { return TableName; } }

        #region Columns

        public static class Columns
        {
            public const string Id = "BidId";
            public const string BidUserId = "BidUserId";
            public const string BidAmount = "BidAmount";
            public const string BidValue = "BidValue";
            public const string BidDate = "BidDate";
            public const string OriginalAmount = "OriginalAmount";
            public const string BidStatus = "BidStatus";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return id; } protected set { id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.BidUserId)]
        public int BidUserId { get { return bidUserId; } set { bidUserId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.BidAmount)]
        public decimal BidAmount { get { return bidAmount; } set { bidAmount = value; SetUpToDateAsFalse(); } }

        [Column(Columns.BidValue)]
        public decimal BidValue { get { return bidValue; } set { bidValue = value; SetUpToDateAsFalse(); } }

        [Column(Columns.BidDate)]
        public DateTime BidDate { get { return bidDate; } set { bidDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.OriginalAmount)]
        public decimal OriginalAmount { get { return originalAmount; } set { originalAmount = value; SetUpToDateAsFalse(); } }

        [Column(Columns.BidStatus)]
        protected int BidStatus { get { return bidStatus; } set { bidStatus = value; SetUpToDateAsFalse(); } }

        public ExchangeOfferStatus Status
        {
            get { return (ExchangeOfferStatus)BidStatus; }
            set { BidStatus = (int)value; }
        }

        private int id, bidUserId, bidStatus;
        private decimal bidAmount, bidValue, originalAmount;
        private DateTime bidDate;

        #endregion

        public InternalExchangeBid() : base() { }
        public InternalExchangeBid(int id) : base(id) { }
        public InternalExchangeBid(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public static bool HaveUserAnyOffers(int userId)
        {
            if ((int)TableHelper.SelectScalar(String.Format("SELECT COUNT(*) FROM InternalExchangeBids WHERE BidUserId={0}", userId)) > 0)
                return true;
            return false;
        }

        public void Withdraw(bool forceAdmin = false)
        {
            if (forceAdmin)
            {
                InternalExchangeManager.WithdrawOffer(Id, BidUserId, forceAdmin, false);
            }
            else
            {
                InternalExchangeManager.WithdrawOffer(Id, false);
            }
        }
    }
}