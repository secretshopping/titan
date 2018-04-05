using ExtensionMethods;
using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;

namespace Titan.ICO
{
    [Serializable]
    public class ICOStage : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "ICOStages"; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "Id";
            public const string StartDate = "StartDate";
            public const string EndDate = "EndDate";
            public const string TotalAvailableTokens = "TotalAvailableTokens";
            public const string TokenPrice = "TokenPrice";
            public const string Name = "Name";
            public const string StatusInt = "StatusInt";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return id; } protected set { id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Name)]
        public string Name { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column(Columns.StartDate)]
        public DateTime StartDate { get { return startDate; } set { startDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.EndDate)]
        public DateTime EndDate { get { return endDate; } set { endDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TotalAvailableTokens)]
        public int TotalAvailableTokens { get { return totalAvailableTokens; } set { totalAvailableTokens = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TokenPrice)]
        public Money TokenPrice { get { return tokenPrice; } set { tokenPrice = value; SetUpToDateAsFalse(); } }

        [Column(Columns.StatusInt)]
        protected int StatusInt { get { return statusInt; } set { statusInt = value; SetUpToDateAsFalse(); } }

        public StageStatus Status
        {
            get { return (StageStatus)StatusInt; }
            set { StatusInt = (int)value; }
        }

        private int id, totalAvailableTokens, statusInt;
        private DateTime startDate, endDate;
        private Money tokenPrice;
        private string name;

        public ICOStage() : base() { }
        public ICOStage(int id) : base(id) { }
        public ICOStage(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public void Update(string name, DateTime startDate, DateTime endDate, int totalTokens, Money tokenPrice)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            TotalAvailableTokens = totalTokens;
            TokenPrice = tokenPrice;
            Save();
        }

        public override void Delete()
        {
            Status = StageStatus.Deleted;
            Save();
        }

        public static bool CheckDataAvailability(DateTime startDate, DateTime endDate)
        {
            //TODO
            return true;
        }

        public int GetAvailableTokens()
        {
            return this.TotalAvailableTokens - ICOPurchase.GetPurchasedTokensCount(this.Id);
        }

        public static ICOStage GetCurrentStage()
        {
            var query = string.Format("SELECT * FROM ICOStages WHERE StartDate < '{0}' AND EndDate > '{0}' AND StatusInt = {1}",
                                    AppSettings.ServerTime.ToDBString(), (int)StageStatus.Active);
            var stages = TableHelper.GetListFromRawQuery<ICOStage>(query);

            ICOStage stage = null;

            if (stages.Count > 0)
                stage = stages[0];

            if (stages.Count == 0 && AppSettings.ICO.ICOStartNewStageIfPreviousEndedEarlierEnabled)
            {
                var nextStage = GetNextStage();
                if (nextStage != null)
                    stage = nextStage;
            }

            //Lets see if it should be finished
            if (stage != null && stage.GetAvailableTokens() == 0)
            {
                stage.Status = StageStatus.Finished;
                stage.Save();
                return null;
            }

            return stage;
        }

        public static ICOStage GetNextStage()
        {
            var query = string.Format("SELECT * FROM ICOStages WHERE StartDate > '{0}' AND StatusInt = {1}",
                                     AppSettings.ServerTime.ToDBString(), (int)StageStatus.Active);
            var stages = TableHelper.GetListFromRawQuery<ICOStage>(query);

            return stages.Count > 0 ? stages[0] : null;
        }

        public static List<ICOStage> GetAllNoDeleted()
        {
            return TableHelper.GetListFromRawQuery<ICOStage>("SELECT * FROM ICOStages WHERE StatusInt <> 3 ORDER BY StartDate ASC");
        }
    }
}