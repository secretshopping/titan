using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using Prem.PTC.Members;
using System.Collections.Generic;
using System.Net;
using Prem.PTC.Referrals;
using MarchewkaOne.Titan.Contests;

namespace Prem.PTC.Contests
{

    public class ForumContest : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Forum; } }

        public static new string TableName { get { return "ForumContests"; } }
        protected override string dbTable { get { return TableName; } }


        public static class Columns
        {
            public static string Id = "Id";
            public static string ForumId = "ForumId";
            public static string ContestId = "ContestId";
        }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("ForumId")]
        public int ForumId { get { return _ForumId; } set { _ForumId = value; SetUpToDateAsFalse(); } }

        [Column("ContestId")]
        public int ContestId { get { return _ContestId; } set { _ContestId = value; SetUpToDateAsFalse(); } }

        private int _Id, _ForumId, _ContestId;
        private Contest _contest;
        #endregion Columns

        public ForumContest()
            : base()
        { }

        public ForumContest(int id) : base(id) { }

        public ForumContest(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        { }


        public Contest Contest
        {
            get
            {
                if (_contest == null)
                    _contest = new Contest(ContestId);
                return _contest;
            }
            set
            {
                _contest = value;
                ContestId = value.Id;
            }
        }
    }
}