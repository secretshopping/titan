using Prem.PTC;
using Prem.PTC.Memberships;
using Prem.PTC.Payments;
using System;
using System.Data;

namespace Titan.InvestmentPlatform
{
    [Serializable]
    public class InvestmentPlatformPlan : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "InvestmentPlatformPlans"; } }

        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "Id";
            public const string Number = "Number";
            public const string Name = "Name";
            public const string Price = "Price";
            public const string Status = "Status";
            public const string Color = "Color";
            public const string Roi = "Roi";
            public const string Time = "Time";
            public const string BinaryEarning = "BinaryEarning";
            public const string DailyLimit = "DailyLimit";
            public const string MonthlyLimit = "MonthlyLimit";
            public const string RequiredMembershipId = "RequiredMembershipId";
            public const string EarningDaysDelay = "EarningDaysDelay";
            public const string EndBonus = "EndBonus";
            public const string LevelFee = "LevelFee";
            public const string LevelMaxDepositPerDay = "LevelMaxPurchasePerDay";
            public const string MaxPrice = "MaxPrice";
            public const string AvailableFromDate = "AvailableFromDate";
            public const string PaymentProcessorInt = "PaymentProcessorInt";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return id; } protected set { id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Number)]
        public int Number { get { return number; } set { number = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Name)]
        public string Name { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Price)]
        public Money Price { get { return price; } set { price = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Status)]
        protected int _Status { get { return status; } set { status = value; SetUpToDateAsFalse(); } }

        public UniversalStatus Status
        {
            get { return (UniversalStatus)_Status; }
            set { _Status = (int)value; }
        }

        [Column(Columns.Color)]
        public string Color { get { return color; } set { color = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Roi)]
        public int Roi { get { return roi; } set { roi = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Time)]
        public int Time { get { return time; } set { time = value; SetUpToDateAsFalse(); } }

        [Column(Columns.BinaryEarning)]
        public int BinaryEarning { get { return binaryEarning; } set { binaryEarning = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DailyLimit)]
        public Money DailyLimit { get { return dailyLimit; } set { dailyLimit = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MonthlyLimit)]
        public Money MonthlyLimit { get { return monthlyLimit; } set { monthlyLimit = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RequiredMembershipId)]
        public int RequiredMembershipId { get { return requiredMembershipId; } set { requiredMembershipId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.EarningDaysDelay)]
        public int EarningDaysDelay { get { return earningDaysDelay; } set { earningDaysDelay = value; SetUpToDateAsFalse(); } }

        [Column(Columns.EndBonus)]
        public Money EndBonus { get { return endBonus; } set { endBonus = value; SetUpToDateAsFalse(); } }

        [Column(Columns.LevelFee)]
        public Money LevelFee { get { return levelFee; } set { levelFee = value; SetUpToDateAsFalse(); } }

        [Column(Columns.LevelMaxDepositPerDay)]
        public int LevelMaxDepositPerDay { get { return levelMaxDepositPerDay; } set { levelMaxDepositPerDay = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MaxPrice)]
        public Money MaxPrice { get { return maxPrice; } set { maxPrice = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AvailableFromDate)]
        public DateTime? AvailableFromDate { get { return availableDate; } set { availableDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PaymentProcessorInt)]
        protected int? PaymentProcessorInt { get { return paymentProcessorInt; } set { paymentProcessorInt = value; SetUpToDateAsFalse(); } }

        public PaymentProcessor PaymentProcessor
        {
            get
            {
                if (PaymentProcessorInt == null)
                    return 0;

                return (PaymentProcessor)PaymentProcessorInt;
            }
            set { PaymentProcessorInt = (int)value; }
        }

        private string name, color;
        private int? paymentProcessorInt;
        private int id, roi, time, number, status, binaryEarning, requiredMembershipId, earningDaysDelay, levelMaxDepositPerDay;
        private Money price, dailyLimit, monthlyLimit, endBonus, levelFee, maxPrice;
        private DateTime? availableDate;

        public InvestmentPlatformPlan() : base() { }
        public InvestmentPlatformPlan(int id) : base(id) { }
        public InvestmentPlatformPlan(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public static void UpdateTableAfterDeletingMembership(int membershipId)
        {
            var querry = string.Format(@"UPDATE {0} SET {1} = {2} WHERE {1} = {3}", TableName, Columns.RequiredMembershipId, Membership.Standard.Id, membershipId);
            TableHelper.ExecuteRawCommandNonQuery(querry);
        }

        public void Activate()
        {
            Status = UniversalStatus.Active;
            Save();
        }

        public void Pause()
        {
            Status = UniversalStatus.Paused;
            Save();
        }

        public override void Delete()
        {
            Status = UniversalStatus.Deleted;
            Save();
            InvestmentPlatformManager.UpdatePlansNumbers();
        }

        public Money TotalMinDefaultEarning()
        {
            try
            {
                if (TitanFeatures.IsRetireYoung)
                    return Price * Roi / 100 + Price;
                return Price * Roi / 100;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                return Money.Zero;
            }
        }

        public Money TotalMaxEarning()
        {
            try
            {
                if (TitanFeatures.IsRetireYoung)
                    return MaxPrice * Roi / 100 + MaxPrice;
                return MaxPrice * Roi / 100;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                return Money.Zero;
            }
        }

        public static string GetNameByLevel(int level)
        {
            var query = string.Format("SELECT {0} FROM {1} WHERE {2} = {3}", Columns.Name, TableName, Columns.Number, level);

            return TableHelper.SelectScalar(query).ToString();
        }

        public string GetPriceText()
        {
            if (MaxPrice == Money.Zero)
                return Price.ToString();

            return string.Format("{0} - {1}", price.ToString(), MaxPrice.ToString());
        }

        public bool CheckPlanPrice(Money price)
        {
            if (MaxPrice != Money.Zero && price <= MaxPrice && price >= Price)
                return true;            

            if(price == Price)
                return true;

            return false;
        }
    }
}