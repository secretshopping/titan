using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;

namespace Titan.Leadership
{
    public partial class RanksUsers : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "RanksUsers"; } }

        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "Id";
            public const string UserId = "UserId";
            public const string RankId = "RankId";
            public const string AquairedDate = "AquairedDate";
            public const string IsCurrent = "IsCurrent";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.UserId)]
        public int UserId { get { return _userId; } set { _userId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RankId)]
        public int RankId { get { return _rankId; } set { _rankId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AquairedDate)]
        public DateTime AquairedDate { get { return _aquairedDate; } set { _aquairedDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsCurrent)]
        public bool IsCurrent { get { return _isCurrent; } set { _isCurrent = value; SetUpToDateAsFalse(); } }

        private bool _isCurrent;
        private int _id, _userId, _rankId;

        private DateTime _aquairedDate;


        public static bool operator==(RanksUsers left, RanksUsers right)
        {
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                return object.ReferenceEquals(left, right);

            return left.Id == right.Id;
        }

        public static bool operator!=(RanksUsers left, RanksUsers right)
        {
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                return !object.ReferenceEquals(left, right);

            return left.Id != right.Id;
        }
    }
}