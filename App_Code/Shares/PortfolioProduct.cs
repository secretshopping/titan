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

    public class PortfolioProduct : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "PortfolioProducts"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Name")]
        public string Name { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("Description")]
        public string Description { get { return name2; } set { name2 = value; SetUpToDateAsFalse(); } }

        [Column("CostToBuild")]
        public Money CostToBuild { get { return _CostToBuild; } set { _CostToBuild = value; SetUpToDateAsFalse(); } }

        [Column("TotalShares")]
        public int TotalShares { get { return _TotalShares; } set { _TotalShares = value; SetUpToDateAsFalse(); } }

        [Column("DateStarted")]
        public DateTime DateStarted { get { return _DateStarted; } set { _DateStarted = value; SetUpToDateAsFalse(); } }

        private int _id, _TotalShares;
        private string name, name2;
        private Money _CostToBuild;
        private DateTime _DateStarted;

        #endregion Columns

        public PortfolioProduct()
            : base() { }

        public PortfolioProduct(int id) : base(id) { }

        public PortfolioProduct(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    }
}