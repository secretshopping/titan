using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace Prem.PTC.Payments
{
    public class PayzaAccountDetails : PaymentAccountDetails
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return AppSettings.TableNames.PayzaGateways; } }
        protected override string dbTable { get { return TableName; } }

        public static readonly int ApiPasswordLength = 16;


        #region Columns

        public static new class Columns
        {
            public const string Id = "PayzaGatewayId";
            public const string APIPassword = "APIPassword";
            public const string SenderEmail = "SenderEmail";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        /// <exception cref="ArgumentException" />
        [Column("APIPassword")]
        public string APIPassword
        {
            get { return _apiPassword; }
            set
            {
                _apiPassword = value; SetUpToDateAsFalse();
            }
        }

        [Column("SenderEmail")]
        public string SenderEmail { get { return _senderEmail; } set { _senderEmail = value; SetUpToDateAsFalse(); } }

        private int _id;
        private string _apiPassword, _senderEmail;

        #endregion

        #region Constructors

        public PayzaAccountDetails() : base() { }
        public PayzaAccountDetails(int id) : base(id) { }
        public PayzaAccountDetails(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public override string AccountType { get { return "Payza"; } }
        public override PaymentAccount Account
        {
            get { return new PayzaAccount(this); }
        }

        public static bool Exists(string username)
        {
            int e1 = TableHelper.CountOf<PayzaAccountDetails>(TableHelper.MakeDictionary(PaymentAccountDetails.Columns.Username, username));
            int e2 = TableHelper.CountOf<PayzaAccountDetails>(TableHelper.MakeDictionary(PayzaAccountDetails.Columns.SenderEmail, username));

            if (e1 + e2 > 0)
                return true;

            return false;
        }

        public override ButtonGenerationStrategy GetStrategy()
        {
            return new PayzaButtonGenerationStrategy(this);
        }
    }
}