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

    public class PortfolioShareTransation : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "PortfolioShareTransations"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("PortfolioProductId")]
        public int PortfolioProductId { get { return _PortfolioShareId; } set { _PortfolioShareId = value; SetUpToDateAsFalse(); } }

        [Column("Units")]
        public int Units { get { return _Units; } set { _Units = value; SetUpToDateAsFalse(); } }

        [Column("SellerUsername")]
        public string SellerUsername { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("BuyerUsername")]
        public string BuyerUsername { get { return name2; } set { name2 = value; SetUpToDateAsFalse(); } }

        [Column("SoldAmount")]
        public Money SoldAmount { get { return _SoldAmount; } set { _SoldAmount = value; SetUpToDateAsFalse(); } }

        [Column("SoldFee")]
        public Money SoldFee { get { return _SoldFee; } set { _SoldFee = value; SetUpToDateAsFalse(); } }

        [Column("DateSold")]
        public DateTime DateSold { get { return _DateSold; } set { _DateSold = value; SetUpToDateAsFalse(); } }

        private int _id, _PortfolioShareId, _BannerAdvertId, _Units;
        private string name, name2;
        private Money _SoldAmount, _SoldFee;
        private DateTime _DateSold;

        #endregion Columns

        public PortfolioShareTransation()
            : base() { }

        public PortfolioShareTransation(int id) : base(id) { }

        public PortfolioShareTransation(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    }
}