using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;


    /// <summary>
    /// Handling RefBack
    /// </summary>
    public class RefBackSites : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "RefbackSites"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("UsersDeclatered")]
        public int UsersDeclared { get { return quantity; } set { quantity = value; SetUpToDateAsFalse(); } }

        [Column("SiteName")]
        public string SiteName { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("ReferralLink")]
        public string ReferralLink { get { return _Title; } set { _Title = value; SetUpToDateAsFalse(); } }

        [Column("IsActive")]
        public bool IsActive { get { return _IsActive; } set { _IsActive = value; SetUpToDateAsFalse(); } }

        private bool _IsActive;
        private int _id, quantity;
        private string name, _Title;

        #endregion Columns

        public RefBackSites()
            : base() { }

        public RefBackSites(int id) : base(id) { }

        public RefBackSites(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    }
