using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
/// <summary>
/// Summary description for NetellerAccountDetails
/// </summary>
/// 

namespace Prem.PTC.Payments
{
    public class NetellerAccountDetails : PaymentAccountDetails
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "NetellerGateways"; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "Id";  
            public const string ClientId = "ClientId";
            public const string ClientSecret = "ClientSecret";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("ClientId")]
        public string ClientId { get { return _clientId; } set { _clientId = value; SetUpToDateAsFalse(); } }

        [Column("ClientSecret")]
        public string ClientSecret { get { return _clientSecret; } set { _clientSecret = value; SetUpToDateAsFalse(); } }

        private int _id;
        private string _accountId, _clientId, _clientSecret;

        #endregion

        #region Constructors

        public NetellerAccountDetails() : base() { }
        public NetellerAccountDetails(int id) : base(id) { }
        public NetellerAccountDetails(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public override string AccountType { get { return "Neteller"; } }
        public override PaymentAccount Account
        {
            get { return new NetellerAccount(this); }
        }

        public static bool Exists(string username)
        {
            return TableHelper.CountOf<NetellerAccountDetails>(TableHelper.MakeDictionary(PaymentAccountDetails.Columns.Username, username)) > 0;
        }

        public override ButtonGenerationStrategy GetStrategy()
        {
            return new NetellerButtonGenerationStrategy(this);
        }
    }
}