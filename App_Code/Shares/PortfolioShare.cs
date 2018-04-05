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

    public class PortfolioShare : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "PortfolioShares"; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "Id";
            public const string OwnerUsername = "OwnerUsername";
            public const string Shares = "Shares";
        }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("PortfolioProductId")]
        public int PortfolioProductId { get { return _PortfolioProductId; } set { _PortfolioProductId = value; SetUpToDateAsFalse(); } }

        [Column("OwnerUsername")]
        public string OwnerUsername { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("Shares")]
        public int Shares { get { return _Shares; } set { _Shares = value; SetUpToDateAsFalse(); } }

        private int _id, _PortfolioProductId, _Shares;
        private string name;

        public PortfolioProduct Product
        {
            get
            {
                return new PortfolioProduct(PortfolioProductId);
            }
        }

        #endregion Columns

        public PortfolioShare()
            : base() { }

        public PortfolioShare(int id) : base(id) { }

        public PortfolioShare(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public override void Save(bool forceSave = false)
        {
            if (Shares > 0)
                base.Save(forceSave);
            else
                base.Delete(); //If no shares left, just remove the object
        }

    }
}