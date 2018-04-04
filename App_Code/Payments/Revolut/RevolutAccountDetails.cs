using System.Data;

namespace Prem.PTC.Payments
{

    public class RevolutAccountDetails : PaymentAccountDetails
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public new static string TableName { get { return "RevolutGateways"; } }
        protected override string dbTable { get { return TableName; } }

        public new static class Columns
        {
            public const string Id = "Id";
            public const string APIKey = "APIKey";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }
        
        [Column(Columns.APIKey)]
        public string APIKey { get { return _ApiKey; } set { _ApiKey = value; SetUpToDateAsFalse(); } }

        private int _id;
        private string _ApiKey;

        #endregion

        #region Constructors

        public RevolutAccountDetails() : base() { }
        public RevolutAccountDetails(int id) : base(id) { }
        public RevolutAccountDetails(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public override string AccountType { get { return "Revolut"; } }
        public override PaymentAccount Account { get { return new RevolutAccount(this); } }

        public override ButtonGenerationStrategy GetStrategy()
        {
            return new RevolutButtonGenerationStrategy(this);
        } 
    }
}