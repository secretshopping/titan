using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;

namespace Prem.PTC.Referrals
{

    public class ReferralPackage : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "ReferralPackages"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("PercentValue")]
        public int PercentValue { get { return imageid; } set { imageid = value; SetUpToDateAsFalse(); } }

        [Column("RefsInPackage")]
        public int RefsInPackage { get { return _Points; } set { _Points = value; SetUpToDateAsFalse(); } }

        private int _id, quantity, type, imageid, _Points;

        #endregion Columns

        public ReferralPackage()
            : base() { }

        public ReferralPackage(int id) : base(id) { }

        public ReferralPackage(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    }
}