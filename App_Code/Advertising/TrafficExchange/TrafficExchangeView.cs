using System;
using System.Data;

namespace Prem.PTC.Advertising
{
    public class TrafficExchangeView : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "TrafficExchangeViews"; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "Id";
            public const string TrafficExchangeAdvertId = "TrafficExchangeAdvertId";
            public const string UserId = "UserId";
            public const string DisplayDate = "LastDisplayDate";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TrafficExchangeAdvertId)]
        public int TrafficExchangeAdvertId { get { return _TrafficExchangeAdvertId; } set { _TrafficExchangeAdvertId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.UserId)]
        public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DisplayDate)]
        public DateTime DisplayDate { get { return _DisplayDate; } set { _DisplayDate = value; SetUpToDateAsFalse(); } }


        private int _id, _TrafficExchangeAdvertId, _UserId;
        DateTime _DisplayDate;
        #endregion

        #region Constructors

        public TrafficExchangeView()
            : base()
        {

        }
        public TrafficExchangeView(int id)
            : base(id)
        {
        }
        public TrafficExchangeView(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        {

        }


        #endregion

       
    }
}