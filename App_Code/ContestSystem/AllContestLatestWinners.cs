using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;

namespace Prem.PTC.Contests
{

    public class AllContestLatestWinners : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "AllContestLatestWinners2"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("ContestId")]
        public int ContestId { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

        [Column("Winner1")]
        public string Winner1 { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("Winner2")]
        public string Winner2 { get { return aname; } set { aname = value; SetUpToDateAsFalse(); } }

        [Column("Winner3")]
        public string Winner3 { get { return bname; } set { bname = value; SetUpToDateAsFalse(); } }

        [Column("NumberOfParticipants")]
        public int NumberOfParticipants { get { return type2; } set { type2 = value; SetUpToDateAsFalse(); } }

        [Column("Participants")]
        public string Participants { get { return bname2; } set { bname2 = value; SetUpToDateAsFalse(); } }


        private int _id, quantity, type, type2;
        private string name, aname, bname, bname2;


        #endregion Columns

        public AllContestLatestWinners()
            : base() { }

        public AllContestLatestWinners(int id) : base(id) { }

        public AllContestLatestWinners(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }
    }
}