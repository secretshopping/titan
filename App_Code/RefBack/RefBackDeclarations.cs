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
    public class RefBackDeclarations : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "RefbackDeclarations"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("RefbackSiteId")]
        public int RefbackSiteId { get { return quantity; } set { quantity = value; SetUpToDateAsFalse(); } }

        [Column("Username")]
        public string Username { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        private int _id, quantity;
        private string name, _Title;

        #endregion Columns

        public RefBackDeclarations()
            : base() { }

        public RefBackDeclarations(int id) : base(id) { }

        public RefBackDeclarations(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    }
