using System.Data;
using System.Collections.Generic;
using Prem.PTC;
using System;
using System.Linq;

namespace Titan.Marketplace
{
    /// <summary>
    /// This class is only used for calculations. Do not display these values to users.
    /// </summary>
    public class MarketplaceBalanceLog : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "MarketplaceBalanceLogs"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("UserId")]
        public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

        [Column("DateAdded")]
        protected DateTime DateAdded { get { return _DateAdded; } set { _DateAdded = value; SetUpToDateAsFalse(); } }

        [Column("Amount")]
        public Money Amount { get { return _Amount; } set { _Amount = value; SetUpToDateAsFalse(); } }

        int _Id, _UserId;
        Money _Amount;
        DateTime _DateAdded;

        private MarketplaceBalanceLog(int userId, Money amount)
        {
            UserId = UserId;
            Amount = amount;
            DateAdded = AppSettings.ServerTime;
        }

        public MarketplaceBalanceLog(int id) : base(id) { }

        public MarketplaceBalanceLog(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate) { }

        #endregion Columns

        public static void Add(int userId, Money amount)
        {
            var record = new MarketplaceBalanceLog(userId, amount);
            record.Save();
        }
    }
}