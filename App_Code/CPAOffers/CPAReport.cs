using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using Prem.PTC.Members;
using System.Collections.Generic;
using System.Net;
using Prem.PTC.Referrals;

namespace Prem.PTC.Offers
{

    public class CPAReport : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "CPAReports"; } }
        protected override string dbTable { get { return TableName; } }


        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("ReportText")]
        public string ReportText { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("ReportingUsername")]
        public string ReportingUsername { get { return username; } set { username = value; SetUpToDateAsFalse(); } }

        [Column("DateReported")]
        public DateTime DateReported { get { return _DateStart; } set { _DateStart = value; SetUpToDateAsFalse(); } }

        [Column("OfferId")]
        public int OfferId { get { return _OfferId; } set { _OfferId = value; SetUpToDateAsFalse(); } }

        [Column("IsDeleted")]
        public bool IsDeleted { get { return _IsDeleted; } set { _IsDeleted = value; SetUpToDateAsFalse(); } }


        private int _id, _OfferId;
        private string name, username;
        private DateTime _DateStart;
        private bool _IsDeleted;

        #endregion Columns

        public CPAReport()
            : base() { }

        public CPAReport(int id) : base(id) { }

        public CPAReport(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }
    }
}