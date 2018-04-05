using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Prem.PTC.Advertising;
using System.Web;
using System.Web.UI;

namespace Titan.Shares
{

    public class ShareOnMarket : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "SharesOnMarket"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("PortfolioShareId")]
        public int PortfolioShareId { get { return _PortfolioShareId; } set { _PortfolioShareId = value; SetUpToDateAsFalse(); } }

        [Column("PortfolioProductId")]
        public int PortfolioProductId { get { return _PortfolioProductId; } set { _PortfolioProductId = value; SetUpToDateAsFalse(); } }

        [Column("Username")]
        public string Username { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("Price")]
        public Money Price { get { return _Price; } set { _Price = value; SetUpToDateAsFalse(); } }

        [Column("SharesToSell")]
        public int SharesToSell { get { return _SharesToSell; } set { _SharesToSell = value; SetUpToDateAsFalse(); } }

        [Column("DateStarted")]
        public DateTime DateStarted { get { return _DateStarted; } set { _DateStarted = value; SetUpToDateAsFalse(); } }

        private int _id, _PortfolioShareId, _SharesToSell, _PortfolioProductId;
        private string name;
        private Money _Price;
        private DateTime _DateStarted;

        #endregion Columns

        public ShareOnMarket()
            : base() { }

        public ShareOnMarket(int id) : base(id) { }

        public ShareOnMarket(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }



    }
}