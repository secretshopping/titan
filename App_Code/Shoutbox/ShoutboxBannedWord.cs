using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;

namespace Titan
{
    public class ShoutboxBannedWord : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "ShoutboxBannedWords"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("WordName")]
        public string WordName { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        private int _id;
        private string name;

        #endregion Columns

        public ShoutboxBannedWord()
            : base() { }

        public ShoutboxBannedWord(int id) : base(id) { }

        public ShoutboxBannedWord(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    }
}