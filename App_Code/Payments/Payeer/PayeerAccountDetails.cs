using System.Data;

namespace Prem.PTC.Payments
{

    public class PayeerAccountDetails : PaymentAccountDetails
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return"PayeerGateways"; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "Id";
            public const string ApiUsername = "ApiUsername";
            public const string AdminApiUsername = "AdminApiUsername";
            public const string ApiPassword = "ApiPassword";
            public const string MerchantID = "MerchantID";
            public const string SecretKey = "SecretKey";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ApiUsername)]
        public string ApiUsername { get { return _apiUsername; } set { _apiUsername = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AdminApiUsername)]
        public string AdminApiUsername { get { return _AdminApiUsername; } set { _AdminApiUsername = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ApiPassword)]
        public string ApiPassword { get { return _apiPassword; } set { _apiPassword = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MerchantID)]
        public string MerchantID { get { return _MerchantID; } set { _MerchantID = value; SetUpToDateAsFalse(); } }

        [Column(Columns.SecretKey)]
        public string SecretKey { get { return _SecretKey; } set { _SecretKey = value; SetUpToDateAsFalse(); } }

        private int _id;
        private string _apiUsername, _apiPassword, _MerchantID, _SecretKey, _AdminApiUsername;

        #endregion

        #region Constructors

        public PayeerAccountDetails() : base() { }
        public PayeerAccountDetails(int id) : base(id) { }
        public PayeerAccountDetails(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public override string AccountType { get { return "Payeer"; } }
        public override PaymentAccount Account { get { return new PayeerAccount(this); } }
        public static bool Exists(string username)
        {
            int e1 = TableHelper.CountOf<PayeerAccountDetails>(TableHelper.MakeDictionary(PaymentAccountDetails.Columns.Username, username));
            int e2 = TableHelper.CountOf<PayeerAccountDetails>(TableHelper.MakeDictionary(PayeerAccountDetails.Columns.MerchantID, username));

            if (e1 + e2 > 0)
                return true;

            return false;
        }

        public override ButtonGenerationStrategy GetStrategy()
        {
            return new PayeerButtonGenerationStrategy(this);
        }
    }
}