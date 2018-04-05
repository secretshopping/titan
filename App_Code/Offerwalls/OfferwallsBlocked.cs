using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Titan;
using Prem.PTC.Members;
using System.Web;

namespace MarchewkaOne.Titan.Offerwalls
{

    public class OfferwallsBlocked : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "OfferwallsBlocked"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("OfferwallId")]
        public int OfferwallId { get { return a1; } set { a1 = value; SetUpToDateAsFalse(); } }

        [Column("UserId")]
        public int UserId { get { return a2; } set { a2 = value; SetUpToDateAsFalse(); } }

        private int _id, a1, a2;

        #endregion Columns

        public OfferwallsBlocked() : base() { }
        public OfferwallsBlocked(int id) : base(id) { }
        public OfferwallsBlocked(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    }
}
