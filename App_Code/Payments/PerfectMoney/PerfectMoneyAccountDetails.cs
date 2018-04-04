using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{

    public class PerfectMoneyAccountDetails : PaymentAccountDetails
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "PerfectMoneyGateways"; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "Id";
            public const string AccountNumber = "Number";
            public const string DisplayName = "Name";
            public const string AlternatePassphrase = "AlternateMerchantPassphrase";
            public const string Password = "LoginPassword";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// U3490354
        /// </summary>
        [Column(Columns.AccountNumber)]
        public string AccountNumber { get { return _apiUsername; } set { _apiUsername = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// UseTitan Official Payments Inc.
        /// </summary>
        [Column(Columns.DisplayName)]
        public string DisplayName { get { return _apiPassword; } set { _apiPassword = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AlternatePassphrase)]
        public string AlternatePassphrase { get { return _apiSignature; } set { _apiSignature = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Password)]
        public string Password { get { return _AppId; } set { _AppId = value; SetUpToDateAsFalse(); } }

        private int _id;
        private string _apiUsername, _apiPassword, _apiSignature, _AppId;

        #endregion

        #region Constructors

        public PerfectMoneyAccountDetails() : base() { }
        public PerfectMoneyAccountDetails(int id) : base(id) { }
        public PerfectMoneyAccountDetails(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public override string AccountType { get { return "PerfectMoney"; } }
        public override PaymentAccount Account { get { return new PerfectMoneyAccount(this); } }

        public static bool Exists(string AccountNumber)
        {
            return TableHelper.CountOf<PerfectMoneyAccountDetails>(TableHelper.MakeDictionary(PerfectMoneyAccountDetails.Columns.AccountNumber, AccountNumber)) > 0;
        }

        public override ButtonGenerationStrategy GetStrategy()
        {
            return new PerfectMoneyButtonGenerationStrategy(this);
        }
    }
}