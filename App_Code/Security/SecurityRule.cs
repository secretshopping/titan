using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;

namespace Prem.PTC.Security
{
    /// <summary>
    /// Handling SecurityRules
    /// </summary>
    public class SecurityRule : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "SecurityRules"; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "SecurityRuleId";
            public const string Type = "Type";
            public const string Value = "Value";
            public const string Comment = "Comment";
            public const string Date = "Date";            
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Type)]
        protected string IntType { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Value)]
        public string Value { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Comment)]
        public string Comment { get { return comment; } set { comment = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Date)]
        public DateTime Date { get { return when; } set { when = value; SetUpToDateAsFalse(); } }

        private int _id;
        private string name, type, comment;
        private DateTime when;

        #endregion Columns


        #region Constructors

        public SecurityRule()
            : base() { }

        public SecurityRule(int id) : base(id) { }

        public SecurityRule(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        #endregion Constructors


        public SecurityRuleType Type
        {
            get
            {
                if (IntType == "IP")
                    return SecurityRuleType.IP;
                if (IntType == "Country")
                    return SecurityRuleType.Country;
                if (IntType == "Username")
                    return SecurityRuleType.Username;
                if (IntType == "IPRange")
                    return SecurityRuleType.IPRange;

                return SecurityRuleType.Null;
            }

            set
            {
                IntType = value.ToString();
            }
        }

    }
}