using System.Data;

namespace Prem.PTC.Payments
{
    public class LocalBitcoinsAccountDetails : PaymentAccountDetails
    {
        #region Columns
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "LocalBitcoinsGateways"; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "Id";
            public const string APISecret = "APISecret";
            public const string APIKey = "APIKey";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.APISecret)]
        public string APISecret { get { return _APISecret; } set { _APISecret = value; SetUpToDateAsFalse(); } }

        [Column(Columns.APIKey)]
        public string APIKey { get { return _APIKey; } set { _APIKey = value; SetUpToDateAsFalse(); } }

        private int _id;
        private string _APISecret, _APIKey;
        #endregion

        #region Constructors
        public LocalBitcoinsAccountDetails() : base() { }
        public LocalBitcoinsAccountDetails(int id) : base(id) { }
        public LocalBitcoinsAccountDetails(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }
        #endregion Constructors

        public override string AccountType { get { return "LocalBitcoins"; } }
        public override PaymentAccount Account { get { return new LocalBitcoinsAccount(this); } }

        public static bool Exists(string walletID)
        {
            return TableHelper.CountOf<LocalBitcoinsAccountDetails>(TableHelper.MakeDictionary(PaymentAccountDetails.Columns.Username, walletID)) > 0;
        }

        public override ButtonGenerationStrategy GetStrategy()
        {
            return new LocalBitcoinsButtonGenerationStrategy(this);
        }
    }

}