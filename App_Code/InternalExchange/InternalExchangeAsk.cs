using ExtensionMethods;
using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using Titan.InternalExchange;

namespace Titan.ICO
{
    [Serializable]
    public class InternalExchangeAsk : BaseTableObject
    {

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "InternalExchangeAsks"; } }
        protected override string dbTable { get { return TableName; } }

        #region Columns

        public static class Columns
        {
            public const string Id = "AskId";
            public const string AskUserId = "AskUserId";
            public const string AskAmount = "AskAmount";
            public const string AskValue = "AskValue";
            public const string AskDate = "AskDate";
            public const string OriginalAmount = "OriginalAmount";
            public const string AskStatus = "AskStatus";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return id; } protected set { id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AskUserId)]
        public int AskUserId { get { return askUserId; } set { askUserId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AskAmount)]
        public decimal AskAmount { get { return askAmount; } set { askAmount = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AskValue)]
        public decimal AskValue { get { return askValue; } set { askValue = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AskDate)]
        public DateTime AskDate { get { return askDate; } set { askDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.OriginalAmount)]
        public decimal OriginalAmount { get { return originalAmount; } set { originalAmount = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AskStatus)]
        protected int AskStatus { get { return askStatus; } set { askStatus = value; SetUpToDateAsFalse(); } }

        public ExchangeOfferStatus Status
        {
            get { return (ExchangeOfferStatus)AskStatus; }
            set { AskStatus = (int)value; }
        }

        private int id, askUserId, askStatus;
        private decimal askAmount, askValue, originalAmount;
        private DateTime askDate;

        #endregion

        public InternalExchangeAsk() : base() { }
        public InternalExchangeAsk(int id) : base(id) { }
        public InternalExchangeAsk(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public static bool HaveUserAnyOffers(int userId)
        {
            if ((int)TableHelper.SelectScalar(String.Format("SELECT COUNT(*) FROM InternalExchangeAsks WHERE AskUserId={0}", userId)) > 0)
                return true;
            return false;
        }
        
        public void Withdraw(bool forceAdmin = false)
        {
            if (forceAdmin)
            {
                InternalExchangeManager.WithdrawOffer(Id, AskUserId, forceAdmin, true);
            }
            else
            {
                InternalExchangeManager.WithdrawOffer(Id, true);
            }
        }
    }
}