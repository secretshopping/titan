using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{

    public class PaparaAccountDetails : PaymentAccountDetails
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public new static string TableName { get { return "PaparaGateways"; } }
        protected override string dbTable { get { return TableName; } }

        public new static class Columns
        {
            public const string Id = "Id";
            public const string ApiUserName = "APIName";
            public const string ApiKey = "APIKey";
            public const string WalletNumber = "WalletNumber";
            public const string SecretKey = "SecretKey";
            public const string ReferansId = "ReferansId";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ApiUserName)]
        public string ApiName { get { return _apiName; } set { _apiName = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ApiKey)]
        public string ApiKey { get { return _apiKey; } set { _apiKey = value; SetUpToDateAsFalse(); } }

        [Column(Columns.WalletNumber)]
        public string WalletNumber { get { return _walletNumber; } set { _walletNumber = value; SetUpToDateAsFalse(); } }

        [Column(Columns.SecretKey)]
        public string SecretKey { get { return _secretKey; } set { _secretKey = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ReferansId)]
        public int ReferansId { get { return _referansId; } set { _referansId = value; SetUpToDateAsFalse(); } }

        private int _id, _referansId;
        private string _apiName, _apiKey, _walletNumber, _secretKey;

        #endregion

        #region Constructors

        public PaparaAccountDetails() : base() { }
        public PaparaAccountDetails(int id) : base(id) { }
        public PaparaAccountDetails(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public override string AccountType { get { return "Papara"; } }
        public override PaymentAccount Account { get { return new PaparaAccount(this); } }
        public static bool Exists(string username)
        {
            var results = TableHelper.SelectAllRows<PaparaAccountDetails>();
            foreach (var result in results)
                if (result.Username.Replace(" ", "") == username)
                    return true;

            return false;
        }

        public override ButtonGenerationStrategy GetStrategy()
        {
            return new PaparaButtonGenerationStrategy(this);
        } 

        public static string GetSecretKey()
        {
            var firstGateway = PaparaAccountDetails.GetFirstIncomeGateway<PaparaAccountDetails>();
            if (firstGateway != null)
                return firstGateway.SecretKey;

            return String.Empty;
        }
    }
}