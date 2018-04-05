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

    public class CPAOfferContest : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "CPAOfferContests"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("CPACategoryId")]
        public int CPACategoryId { get { return _CPACategoryId; } set { _CPACategoryId = value; SetUpToDateAsFalse(); } }

        [Column("ContestId")]
        public int ContestId { get { return _ContestId; } set { _ContestId = value; SetUpToDateAsFalse(); } }

        private int _Id, _CPACategoryId, _ContestId;
        private Contest _contest;
        #endregion Columns

        public CPAOfferContest()
            : base()
        { }

        public CPAOfferContest(int id) : base(id) { }

        public CPAOfferContest(DataRow row, bool isUpToDate = true)
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