using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;

namespace Titan
{
    public class ShoutboxBannedUsernames : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "ShoutboxBannedUsernames"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Username")]
        public string Username { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("BannedSince")]
        public DateTime BannedSince { get { return d1; } set { d1 = value; SetUpToDateAsFalse(); } }

        [Column("BannedUntil")]
        public DateTime BannedUntil { get { return d2; } set { d2 = value; SetUpToDateAsFalse(); } }

        private int _id;
        private string name;
        private DateTime d1, d2;

        #endregion Columns

        public ShoutboxBannedUsernames()
            : base() { }

        public ShoutboxBannedUsernames(int id) : base(id) { }

        public ShoutboxBannedUsernames(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    }
}