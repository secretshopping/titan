using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{

    public class MPesaAccountDetails : PaymentAccountDetails
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public new static string TableName { get { return "MPesaGateways"; } }
        protected override string dbTable { get { return TableName; } }

        public new static class Columns
        {
            public const string Id = "Id";
            public const string ProductName = "ProductName";
            public const string ApiKey = "ApiKey";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ProductName)]
        public string ProductName { get { return _ProductName; } set { _ProductName = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ApiKey)]
        public string ApiKey { get { return _ApiKey; } set { _ApiKey = value; SetUpToDateAsFalse(); } }

        private int _id;
        private string _ProductName, _ApiKey;

        #endregion

        #region Constructors

        public MPesaAccountDetails() : base() { }
        public MPesaAccountDetails(int id) : base(id) { }
        public MPesaAccountDetails(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public override string AccountType { get { return "MPesa"; } }
        public override PaymentAccount Account { get { return new MPesaAccount(this); } }
        public static bool Exists(string username)
        {
            var results = TableHelper.SelectAllRows<MPesaAccountDetails>();
            foreach (var result in results)
                if (result.Username.Replace(" ", "") == username)
                    return true;

            return false;
        }

        public override ButtonGenerationStrategy GetStrategy()
        {
            return new MPesaButtonGenerationStrategy(this);
        } 
    }
}