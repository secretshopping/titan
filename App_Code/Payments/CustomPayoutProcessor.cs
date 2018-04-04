using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC.Members;
using Prem.PTC;
using System.Data;
using Resources;
using ExtensionMethods;

namespace Prem.PTC.Payments
{
    public class CustomPayoutProcessor : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "CustomPayoutProcessors"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("StatusInt")]
        protected int StatusInt { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

        [Column("MoneyFee")]
        public Money MoneyFee { get { return m1; } set { m1 = value; SetUpToDateAsFalse(); } }

        [Column("PercentFee")]
        public Money PercentageFee { get { return m2; } set { m2 = value; SetUpToDateAsFalse(); } }

        [Column("Name")]
        public String Name { get { return s1; } set { s1 = value; SetUpToDateAsFalse(); } }

        [Column("Description")]
        public String Description { get { return s2; } set { s2 = value; SetUpToDateAsFalse(); } }

        [Column("ImageURL")]
        public String ImageURL { get { return s3; } set { s3 = value; SetUpToDateAsFalse(); } }

        [Column("OverrideGlobalLimit")]
        public bool OverrideGlobalLimit { get { return _OverrideGlobalLimit; } set { _OverrideGlobalLimit = value; SetUpToDateAsFalse(); } }

        [Column("CashoutLimit")]
        public Money CashoutLimit { get { return _CashoutLimit; } set { _CashoutLimit = value; SetUpToDateAsFalse(); } }

        [Column("MAxValueOfPendingRequestsPerDay")]
        public Money MaxValueOfPendingRequestsPerDay { get { return _MaxValueOfPendingRequestsPerDay; } set { _MaxValueOfPendingRequestsPerDay = value; SetUpToDateAsFalse(); } }

        [Column("DaysToBlockWithdrawalsAfterAccounChange")]
        public int DaysToBlockWithdrawalsAfterAccounChange { get { return _blockWithdrawals; } set { _blockWithdrawals = value; SetUpToDateAsFalse(); } }

        private int _id, type, _blockWithdrawals;
        private Money m1, m2, _CashoutLimit, _MaxValueOfPendingRequestsPerDay;
        private String s1, s2, s3;
        private bool _OverrideGlobalLimit;

        #endregion Columns

        public CustomPayoutProcessor()
            : base()
        { }

        public CustomPayoutProcessor(int id) : base(id) { }

        public CustomPayoutProcessor(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        { }

        public UniversalStatus Status
        {
            get
            {
                return (UniversalStatus)StatusInt;
            }
            set
            {
                StatusInt = (int)value;
            }
        }

        public string FeesToString()
        {
            return MoneyFee.ToString() + " + " + PercentageFee.ToShortClearString() + "%";
        }

        public void CheckMaxValueOfPendingRequestsPerDay(Money amount)
        {
            if (this.MaxValueOfPendingRequestsPerDay < new Money(2000000000))
            {
                var sum = TableHelper.SelectScalar(String.Format(
                    @"WITH 
                    AllRequests AS
                    (
                    SELECT CAST(RequestDate AS DATE) AS DateDay FROM PayoutRequests where PaymentProcessor = '{0}'
                    AND IsRequest = 1 AND IsPaid = 0 GROUP BY CAST(RequestDate AS DATE)
                    )
                    SELECT SUM(Amount) FROM PayoutRequests WHERE PaymentProcessor = '{0}' AND IsRequest = 1 AND CAST(RequestDate AS DATE) IN(SELECT DateDay FROM AllRequests)",
                    this.Name));

                if (sum != null && !(sum is DBNull))
                {
                    Money PendingRequestsValue = new Money((Decimal)sum);

                    if (PendingRequestsValue + amount > this.MaxValueOfPendingRequestsPerDay)
                        throw new MsgException(U6006.PROCESSORLIMIT);
                }
            }
        }

        public static List<CustomPayoutProcessor> GetAllActiveProcessors()
        {
            var query = string.Format("SELECT * FROM {0} WHERE StatusInt = {1}", TableName, (int)UniversalStatus.Active);
            return TableHelper.GetListFromRawQuery<CustomPayoutProcessor>(query);
        }

        public static List<CustomPayoutProcessor> GetAllProcessors()
        {
            var query = string.Format("SELECT * FROM {0}", TableName, (int)UniversalStatus.Active);
            return TableHelper.GetListFromRawQuery<CustomPayoutProcessor>(query);
        }

        public static List<CustomPayoutProcessor> AllUniqueProcessors
        {
            get
            {
                return GetAllProcessors()
                    .GroupBy(procesor => procesor.Name)
                    .Select(procesor => procesor.First())
                    .ToList();
            }
        }


        public static bool IsCustomPayoutProcessor(string value)
        {
            bool isCustom = false;
            try
            {
                int a = Int32.Parse(value);
                isCustom = true;
            }
            catch { }

            return isCustom;
        }

        public static int GetCustomPayoutProcessorId(string value)
        {
            int isCustom = 0;
            try
            {
                isCustom = Int32.Parse(value);
            }
            catch { }

            return isCustom;
        }
    }
}