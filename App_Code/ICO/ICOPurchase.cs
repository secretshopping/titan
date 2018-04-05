using ExtensionMethods;
using Prem.PTC;
using System;
using System.Data;

namespace Titan.ICO
{
    public class ICOPurchase : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "ICOPurchases"; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "Id";
            public const string UserId = "UserId";
            public const string NumberOfTokens = "NumberOfTokens";
            public const string PurchaseTime = "PurchaseTime";
            public const string NumberOfTokensIncludingReferrers = "NumberOfTokensIncludingReferrers";
            public const string ICOStageId = "ICOStageId";            
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return id; } protected set { id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.UserId)]
        public int UserId { get { return userId; } set { userId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.NumberOfTokens)]
        public int NumberOfTokens { get { return numberOfTokens; } set { numberOfTokens = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PurchaseTime)]
        public DateTime PurchaseTime { get { return purchaseTime; } set { purchaseTime = value; SetUpToDateAsFalse(); } }       
       
        [Column(Columns.NumberOfTokensIncludingReferrers)]
        public decimal NumberOfTokensIncludingReferrers { get { return numberOfTokensIncludingReferrers; } set { numberOfTokensIncludingReferrers = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ICOStageId)]
        public int ICOStageId { get { return icoStageId; } set { icoStageId = value; SetUpToDateAsFalse(); } }

        private int id, userId, numberOfTokens, icoStageId;
        private DateTime purchaseTime;
        private decimal numberOfTokensIncludingReferrers;

        public ICOPurchase() : base() { }
        public ICOPurchase(int id) : base(id) { }
        public ICOPurchase(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }       


        public static int GetUserPurchasesInLast15Min(int stageId, int userId)
        {
            var query = string.Format("SELECT SUM(NumberOfTokens) FROM ICOPurchases WHERE UserId = {0} AND ICOStageId = {1} AND PurchaseTime >= '{2}'", 
                userId, stageId, AppSettings.ServerTime.AddMinutes(-15).ToDBString());

            var result = TableHelper.SelectScalar(query);

            if (result is DBNull)
                return 0;

            return Convert.ToInt32(result);
        }

        public static int GetPurchasedTokensCount(int stageId)
        {
            var query = string.Format("SELECT SUM(NumberOfTokens) FROM ICOPurchases WHERE ICOStageId = {0}", stageId);

            var result = TableHelper.SelectScalar(query);

            if (result is DBNull)
                return 0;

            return Convert.ToInt32(result);
        }

        public static void Add(int userId, int numberOfTokens, decimal numberOfTokensIncludingReferrers, int stageId)
        {
            var NewPurchase = new ICOPurchase();
            NewPurchase.UserId = userId;
            NewPurchase.NumberOfTokens = numberOfTokens;
            NewPurchase.PurchaseTime = AppSettings.ServerTime;
            NewPurchase.NumberOfTokensIncludingReferrers = numberOfTokensIncludingReferrers;
            NewPurchase.ICOStageId = stageId;
            NewPurchase.Save();
        }
    }
}