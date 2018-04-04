using System.Data;
using Prem.PTC;

namespace Titan.PaidToPromote
{
    public class PaidToPromoteTemporaryIP : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "PaidToPromoteTemporaryIPs"; } }
        protected override string dbTable { get { return TableName; } }

        #region Columns
        public static class Columns
        {
            public const string Id = "Id";
            public const string IP = "IP";
            public const string AdvertId = "AdvertId";
            public const string CreditedUser = "CreditedUser";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IP)]
        public string IP { get { return _ip; } set { _ip = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AdvertId)]
        public int AdvertId { get { return _advertId; } set { _advertId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CreditedUser)]
        public int? CreditedUser { get { return _creditedUser; } set { _creditedUser = value; SetUpToDateAsFalse(); } }

        private string _ip;
        private int _id, _advertId;
        private int? _creditedUser;
        #endregion

        public PaidToPromoteTemporaryIP() : base() { }

        public PaidToPromoteTemporaryIP(int id) : base(id) { }

        public PaidToPromoteTemporaryIP(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }
    }
}