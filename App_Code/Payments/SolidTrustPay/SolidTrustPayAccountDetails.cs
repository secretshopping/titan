using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{

    public class SolidTrustPayAccountDetails : PaymentAccountDetails
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return"SolidTrustPayGateways"; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "Id";
            public const string ApiUsername = "ApiUsername";
            public const string ApiPassword = "ApiPassword";
            public const string PaymentButtonsPassword = "PaymentButtonsPassword";
            public const string PaymentButtonName = "PaymentButtonName";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ApiUsername)]
        public string ApiUsername { get { return _apiUsername; } set { _apiUsername = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ApiPassword)]
        public string ApiPassword { get { return _apiPassword; } set { _apiPassword = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PaymentButtonsPassword)]
        public string PaymentButtonsPassword { get { return _PaymentButtonsPassword; } set { _PaymentButtonsPassword = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PaymentButtonName)]
        public string PaymentButtonName { get { return _PaymentButtonName; } set { _PaymentButtonName = value; SetUpToDateAsFalse(); } }

        private int _id;
        private string _apiUsername, _apiPassword, _PaymentButtonsPassword, _PaymentButtonName;

        #endregion

        #region Constructors

        public SolidTrustPayAccountDetails() : base() { }
        public SolidTrustPayAccountDetails(int id) : base(id) { }
        public SolidTrustPayAccountDetails(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public override string AccountType { get { return "SolidTrustPay"; } }
        public override PaymentAccount Account { get { return new SolidTrustPayAccount(this); } }

        public static bool Exists(string username)
        {
            return TableHelper.CountOf<SolidTrustPayAccountDetails>(TableHelper.MakeDictionary(PaymentAccountDetails.Columns.Username, username)) > 0;
        }

        public override ButtonGenerationStrategy GetStrategy()
        {
            return new SolidTrustPayButtonGenerationStrategy(this);
        }
    }
}