using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Data;
using System.Text;
using Titan.CustomFeatures;

namespace Titan.InvestmentPlatform
{
    [Serializable]
    public class InvestmentUsersPlans : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "InvestmentUsersPlans"; } }

        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "Id";
            public const string PlanId = "PlanId";
            public const string UserId = "UserId";
            public const string Status = "Status";
            public const string BalanceBoughtType = "BalanceBoughtType";
            public const string PurchaseDate = "PurchasedDate";
            public const string MoneyReturned = "MoneyReturned";
            public const string MoneyToReturn = "MoneyToReturn";
            public const string MoneyInSystem = "MoneyInSystem";
            public const string FinishDate = "FinishDate";
            public const string CurrentMonthPayout = "CurrentMonthPayout";
            public const string LastWithdrawalDate = "LastWithdrawalDate";
            public const string Price = "Price";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return id; } protected set { id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PlanId)]
        public int PlanId { get { return planId; } set { planId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.UserId)]
        public int UserId { get { return userId; } set { userId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Status)]
        protected int _Status { get { return status; } set { status = value; SetUpToDateAsFalse(); } }

        public PlanStatus Status
        {
            get { return (PlanStatus)_Status; }
            set { _Status = (int)value; }
        }

        [Column(Columns.BalanceBoughtType)]
        protected int _BalanceBoughtType { get { return balanceBoughtType; } set { balanceBoughtType = value; SetUpToDateAsFalse(); } }

        public PurchaseBalances BalanceBoughtType
        {
            get { return (PurchaseBalances)_BalanceBoughtType; }
            set { _BalanceBoughtType = (int)value; }
        }

        [Column(Columns.PurchaseDate)]
        public DateTime PurchaseDate { get { return purchaseDate; } set { purchaseDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.FinishDate)]
        public DateTime? FinishDate { get { return finishDate; } set { finishDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.LastWithdrawalDate)]
        public DateTime? LastWithdrawalDate { get { return lastWithdrawalDate; } set { lastWithdrawalDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MoneyReturned)]
        public Money MoneyReturned { get { return moneyReturned; } set { moneyReturned = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MoneyToReturn)]
        public Money MoneyToReturn { get { return moneyToReturn; } set { moneyToReturn = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MoneyInSystem)]
        public Money MoneyInSystem { get { return moneyInSystem; } set { moneyInSystem = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CurrentMonthPayout)]
        public Money CurrentMonthPayout { get { return currentMonthPayout; } set { currentMonthPayout = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Price)]
        public Money Price { get { return price; } set { price = value; SetUpToDateAsFalse(); } }

        private DateTime? finishDate, lastWithdrawalDate;
        private DateTime purchaseDate;
        private int id, planId, userId, balanceBoughtType, status;
        private Money moneyReturned, moneyToReturn, moneyInSystem, currentMonthPayout, price;

        public InvestmentUsersPlans() : base()
        {
            MoneyInSystem = Money.Zero;
        }

        public InvestmentUsersPlans(int id) : base(id) { }

        public InvestmentUsersPlans(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public void TakeMoneyFromFinishedPlans()
        {
            var query = string.Format(@"SELECT SUM({0}) FROM {1} WHERE {2} = {3} AND status = {4} ",
                Columns.MoneyInSystem, TableName, Columns.UserId, UserId, (int)PlanStatus.Finished);

            try
            {
                var scalar = TableHelper.SelectScalar(query);
                var result = scalar.ToString() != string.Empty ? scalar : 0m;
                var overAmount = new Money((decimal)result);

                if (overAmount > Money.Zero)
                {
                    MoneyInSystem += overAmount;
                    MoneyReturned += overAmount;

                    this.Save();

                    var resetQuery = string.Format(@"UPDATE {0} SET {1} = 0 WHERE {2} = {3} AND status = {4} ",
                        TableName, Columns.MoneyInSystem, Columns.UserId, UserId, (int)PlanStatus.Finished);

                    TableHelper.ExecuteRawCommandNonQuery(resetQuery);
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e);
            }
        }

        public static Money GetMoneyInSystemFromFinishedPlans(int userId)
        {
            var query = string.Format(@"SELECT SUM({0}) FROM {1} WHERE {2} = {3}",
                Columns.MoneyInSystem, TableName, Columns.UserId, userId);

            try
            {
                return new Money((decimal)TableHelper.SelectScalar(query));
            }
            catch (Exception e)
            {
                return Money.Zero;
            }
        }

        public void TryToWidthdrawlMoneyFromSystem(bool multipleTransfer = false)
        {
            var user = Member.CurrentInCache;

            if (AppSettings.InvestmentPlatform.InvestmentPlatformDailyLimitsEnabled)
                if (multipleTransfer)
                    return;
                else
                    throw new MsgException(U6010.INVPLATFORMCANTTRANSFER);

            if (MoneyInSystem == Money.Zero)
                if (multipleTransfer)
                    return;
                else
                    throw new MsgException(U6010.INVPLATFORMNOTHINGTOTRANSFER);

            if (MoneyInSystem < user.Membership.InvestmentPlatformMinAmountToCredited)
                if (multipleTransfer)
                    return;
                else
                    throw new MsgException(U6006.MINAMOUNTTOPAYOUT + ": " + user.Membership.InvestmentPlatformMinAmountToCredited);


            var note = string.Format("Widthdraw from {0}", new InvestmentPlatformPlan(PlanId).Name);
            var crediter = new InvestmentPlanCrediter(user);
            crediter.CreditPlan(MoneyInSystem, BalanceType.MainBalance, note, BalanceLogType.InvestmentPlatformWithdrawal);

            MoneyInSystem = Money.Zero;
            LastWithdrawalDate = AppSettings.ServerTime;
            Save();
        }

        public void Finish(bool isSave = true)
        {
            FinishDate = AppSettings.ServerTime;
            Status = PlanStatus.Finished;

            if (!AppSettings.InvestmentPlatform.InvestmentPlatformDailyLimitsEnabled)
            {
                var user = Member.CurrentInCache;
                var crediter = new InvestmentPlanCrediter(user);
                var platformPlan = new InvestmentPlatformPlan(PlanId);
                var note = string.Format("Finished plan: {0}", platformPlan.Name);

                if (TitanFeatures.IsRetireYoung)
                    crediter.CreditPlan(MoneyInSystem + platformPlan.Price, BalanceType.MainBalance, note, BalanceLogType.InvestmentPlatformWithdrawal);
                else
                    crediter.CreditPlan(MoneyInSystem, BalanceType.MainBalance, note, BalanceLogType.InvestmentPlatformWithdrawal);

                //BONUS
                if (MoneyInSystem >= MoneyToReturn && LastWithdrawalDate == null)
                {
                    var bonus = platformPlan.EndBonus;

                    if (bonus > Money.Zero)
                    {
                        note = string.Format(U6010.BONUSFORFINISHEDPLAN, platformPlan.Name);
                        crediter.CreditPlan(bonus, BalanceType.MainBalance, note, BalanceLogType.InvestmentPlatformWithdrawal);

                        var historyNote = string.Format("{0} ({1}/{2})", note, bonus.ToString(), RetireyoungManager.GetAggregate(user.Id));
                        History.AddEntry(user.Name, HistoryType.InvestmentPlatformBonus, historyNote);
                    }
                }

                MoneyInSystem = Money.Zero;
            }

            if (isSave)
                Save();
        }

        public void Remove()
        {
            Status = PlanStatus.Removed;
            Save();
        }

        public string GeneratePlanNumber()
        {
            var sb = new StringBuilder();

            sb.Append(string.Format("P{0}-", FormText(new InvestmentPlatformPlan(PlanId).Id, 4)));
            sb.Append(string.Format("{0}-{1}-{2}-", FormText(PurchaseDate.AddYears(-2000).Year, 2), FormText(PurchaseDate.Month, 2), FormText(PurchaseDate.Day, 2)));
            sb.Append(string.Format("{0}", FormText(Id, 4)));

            return sb.ToString();
        }

        private string FormText(int number, int maxLength)
        {
            return number.ToString().PadLeft(maxLength, '0');
        }
    }
}