using System.Data;

namespace Prem.PTC.Advertising
{
    /// <summary>
    /// Handling achievements
    /// </summary>
    public class TrafficExchangeSubpage : BaseTableObject
    {
        #region Columns
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "TrafficExchangeSubpages"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("PtcAdId")]
        public int AdId { get { return quantity; } set { quantity = value; SetUpToDateAsFalse(); } }

        [Column("SubPage")]
        public string SubPage { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        private int _id, quantity, type, imageid, _Points;
        private string name;
        #endregion Columns

        public TrafficExchangeSubpage() : base() { }
        public TrafficExchangeSubpage(int id) : base(id) { }
        public TrafficExchangeSubpage(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }       
    }
}