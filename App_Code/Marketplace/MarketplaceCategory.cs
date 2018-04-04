using System.Data;
using System.Collections.Generic;
using Prem.PTC;

namespace Titan.Marketplace
{
    public class MarketplaceCategory : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "MarketplaceCategories"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("Title")]
        public string Title { get { return _Title; } set { _Title = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int StatusInt { get { return _Status; } set { _Status = value; SetUpToDateAsFalse(); } }

        public UniversalStatus Status
        {
            get
            {
                return (UniversalStatus)StatusInt;
            }
            set { StatusInt = (int)value; }
        }

        private int _Id, _Status;
        private string _Title;

        public MarketplaceCategory()
            : base() { }

        public MarketplaceCategory(int id) : base(id) { }

        public MarketplaceCategory(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate) { }

        #endregion Columns

        public static List<MarketplaceCategory> AllActiveCategories()
        {
            return TableHelper.GetListFromRawQuery<MarketplaceCategory>(string.Format("SELECT * FROM MarketplaceCategories WHERE Status = {0}", (int)UniversalStatus.Active));
        }
    }
}