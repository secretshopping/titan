using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{

    public class AdvCashAccountDetails : PaymentAccountDetails
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "AdvCashGateways"; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "Id";
            public const string SCIName = "SCIName";
            public const string APIName = "APIName";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.SCIName)]
        public string SCIName { get { return _SCIName; } set { _SCIName = value; SetUpToDateAsFalse(); } }

        [Column(Columns.APIName)]
        public string APIName { get { return _APIName; } set { _APIName = value; SetUpToDateAsFalse(); } }

        [Column("AccountEmail")]
        public string AccountEmail { get { return _AccountEmail; } set { _AccountEmail = value; SetUpToDateAsFalse(); } }


        private int _id;
        private string _SCIName, _APIName, _AccountEmail;

        #endregion

        #region Constructors

        public AdvCashAccountDetails() : base() { }
        public AdvCashAccountDetails(int id) : base(id) { }
        public AdvCashAccountDetails(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public override string AccountType { get { return "AdvCash"; } }
        public override PaymentAccount Account { get { return new AdvCashAccount(this); } }
        public static bool Exists(string username)
        {
            var results = TableHelper.SelectAllRows<AdvCashAccountDetails>();
            foreach (var result in results)
                if (result.Username.Replace(" ", "") == username)
                    return true;

            return false;
        }

        public override ButtonGenerationStrategy GetStrategy()
        {
            return new AdvCashButtonGenerationStrategy(this);
        }
    }
}