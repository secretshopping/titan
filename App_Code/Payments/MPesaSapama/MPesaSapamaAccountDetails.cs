using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{

    public class MPesaSapamaAccountDetails : PaymentAccountDetails
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public new static string TableName { get { return "MPesaSapamaGateways"; } }
        protected override string dbTable { get { return TableName; } }

        public new static class Columns
        {
            public const string Id = "Id";
            public const string LocationId = "LocationId";
            public const string APIKey = "APIKey";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.LocationId)]
        public int LocationId { get { return _LocationId; } set { _LocationId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.APIKey)]
        public string APIKey { get { return _ApiKey; } set { _ApiKey = value; SetUpToDateAsFalse(); } }

        [Column("APISecret")]
        public string APISecret { get { return _APISecret; } set { _APISecret = value; SetUpToDateAsFalse(); } }

        private int _id, _LocationId;
        private string _ApiKey, _APISecret;

        #endregion

        #region Constructors

        public MPesaSapamaAccountDetails() : base() { }
        public MPesaSapamaAccountDetails(int id) : base(id) { }
        public MPesaSapamaAccountDetails(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public override string AccountType { get { return "MPesaAgent"; } }
        public override PaymentAccount Account { get { return new MPesaSapamaAccount(this); } }

        public static bool Exists(int locationId)
        {
            var results = TableHelper.SelectAllRows<MPesaSapamaAccountDetails>();
            foreach (var result in results)
                if (result.LocationId == locationId)
                    return true;
            return false;
        }

        public override ButtonGenerationStrategy GetStrategy()
        {
            return new MPesaSapamaButtonGenerationStrategy(this);
        } 
    }
}