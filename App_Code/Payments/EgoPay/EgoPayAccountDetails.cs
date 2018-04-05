using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{

    public class EgoPayAccountDetails : PaymentAccountDetails
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return"EgoPayGateways"; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "EgoPayGatewayId";
            public const string ApiUsername = "ApiUsername";
            public const string ApiPassword = "ApiPassword";
            public const string ApiSignature = "ApiSignature";
            public const string AppId = "AppId";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ApiUsername)]
        public string ApiUsername { get { return _apiUsername; } set { _apiUsername = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ApiPassword)]
        public string ApiPassword { get { return _apiPassword; } set { _apiPassword = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ApiSignature)]
        public string ApiSignature { get { return _apiSignature; } set { _apiSignature = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AppId)]
        public string AppId { get { return _AppId; } set { _AppId = value; SetUpToDateAsFalse(); } }

        public bool IsMassPayEnabled
        {
            get
            {
                if (AppId == "MASSPAY")
                    return true;
                return false;
            }
            set
            {
                if (value)
                    AppId = "MASSPAY";
                else
                    AppId = "blank";
            }
        }


        private int _id;
        private string _apiUsername, _apiPassword, _apiSignature, _AppId;

        #endregion

        #region Constructors

        public EgoPayAccountDetails() : base() { }
        public EgoPayAccountDetails(int id) : base(id) { }
        public EgoPayAccountDetails(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public override string AccountType { get { return "EgoPay"; } }
        public override PaymentAccount Account { get { return new EgoPayAccount(this); } }

        public static bool Exists(string username)
        {
            return TableHelper.CountOf<EgoPayAccountDetails>(TableHelper.MakeDictionary(PaymentAccountDetails.Columns.Username, username)) > 0;
        }

        public override ButtonGenerationStrategy GetStrategy()
        {
            return null;
        }
    }
}