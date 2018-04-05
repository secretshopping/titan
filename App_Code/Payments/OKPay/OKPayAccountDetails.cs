using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{

    public class OKPayAccountDetails : PaymentAccountDetails
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "OKPayGateways"; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "Id";

        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }


        [Column("APISecret")]
        public string APISecret { get { return _APISecret; }  set { _APISecret = value; SetUpToDateAsFalse(); } }


        private int _id;
        private string _APISecret;

        #endregion

        #region Constructors

        public OKPayAccountDetails() : base() { }
        public OKPayAccountDetails(int id) : base(id) { }
        public OKPayAccountDetails(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public override string AccountType { get { return "OKPay"; } }
        public override PaymentAccount Account { get { return new OKPayAccount(this); } }

        public static bool Exists(string walletID)
        {
            return TableHelper.CountOf<OKPayAccountDetails>(TableHelper.MakeDictionary(PaymentAccountDetails.Columns.Username, walletID)) > 0;
        }

        public override ButtonGenerationStrategy GetStrategy()
        {
            return new OKPayButtonGenerationStrategy(this);
        }
    }

}