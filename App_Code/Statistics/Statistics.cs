using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Prem.PTC.Payments;
using Titan.Cryptocurrencies;

namespace Prem.PTC.Statistics
{
    public class Statistics : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "[Statistics]"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Type")]
        protected int IntType { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

        [Column("NumberOfDays")]
        public int NumberOfDays { get { return quantity; } set { quantity = value; SetUpToDateAsFalse(); } }

        [Column("Data1")]
        public string Data1 { get { return data1; } set { data1 = value; SetUpToDateAsFalse(); } }

        [Column("Data2")]
        public string Data2 { get { return data2; } set { data2 = value; SetUpToDateAsFalse(); } }

        private int _id, quantity, type;
        private string data1, data2;

        #endregion Columns

        public Statistics()
            : base() { }

        public Statistics(int id) : base(id) { }

        public Statistics(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public Statistics(StatisticsType type)
            : this(TableHelper.SelectRows<Statistics>(TableHelper.MakeDictionary("Type", (int)type))[0].Id)
        {
            //There is only one of type (not always :( )
        }

        public StatisticsType Type
        {
            get
            {
                return (StatisticsType)IntType;
            }

            set
            {
                IntType = (int)value;
            }
        }

        public void AddToData1(int amount)
        {
            Data1 = handleIntAdd(Data1, amount);
        }

        public void AddToData2(int amount)
        {
            Data2 = handleIntAdd(Data2, amount);
        }

        public void AddToData1(Money amount)
        {
            Data1 = handleMoneyAdd(Data1, amount);
        }

        public void AddToData2(Money amount)
        {
            Data2 = handleMoneyAdd(Data2, amount);
        }

        private string handleIntAdd(string source, int amount)
        {
            var list = TableHelper.GetIntListFromString(source);
            list[0] = list[0] + amount;
            return TableHelper.GetStringFromIntList(list);
        }

        private string handleMoneyAdd(string source, Money amount)
        {
            var list = TableHelper.GetMoneyListFromString(source);
            list[0] = list[0] + amount;
            return TableHelper.GetStringFromMoneyList(list);
        }

        public static void DeletePaymentAccount(PaymentAccountDetails account)
        {
            TableHelper.DeleteRows<Statistics>(TableHelper.MakeDictionary("Data1", StatisticsManager.GetPaymentAccountName(account)));
        }

        public static void DeletePaymentAccount(CryptocurrencyAPIProvider processorName)
        {
            TableHelper.DeleteRows<Statistics>(TableHelper.MakeDictionary("Data1", processorName.ToString()));
        }

        public static void AddPaymentAccount(PaymentAccountDetails account)
        {
            Statistics stat = new Statistics();
            stat.Data1 = StatisticsManager.GetPaymentAccountName(account);
            stat.data2 = "0.000#0.000#0.000#0.000#0.000#0.000#0.000#0.000#0.000#0.000#0.000#0.000#0.000#0.000";
            stat.Type = StatisticsType.AvailableFunds;
            stat.NumberOfDays = 14;
            stat.Save();
        }

        public static void UpdatePaymentAccount(string oldUserName, PaymentAccountDetails account)
        {
            if (oldUserName == account.Username)
                return;

            var id = TableHelper.SelectScalar(string.Format("SELECT Id FROM [Statistics] WHERE Data1 LIKE '{0}:{1}'", account.AccountType, oldUserName));
            Statistics stat = new Statistics((int)id);
            stat.Data1 = StatisticsManager.GetPaymentAccountName(account);
            stat.Save();
        }

        public static void AddPaymentAccount(CryptocurrencyAPIProvider processorName)
        {
            var stat = new Statistics
            {
                Data1 = processorName.ToString(),
                data2 = "0.000#0.000#0.000#0.000#0.000#0.000#0.000#0.000#0.000#0.000#0.000#0.000#0.000#0.000",
                Type = StatisticsType.AvailableFunds,
                NumberOfDays = 14
            };
            stat.Save();
        }

        public void SetData2(Money amount)
        {
            var list = TableHelper.GetMoneyListFromString(Data2);
            list[0] = amount;
            Data2 = TableHelper.GetStringFromMoneyList(list);
        }

        public static void AddToCashflow(Money amount)
        {
            var stats = new Statistics(StatisticsType.Cashflow);
            stats.AddToData1(amount);
            stats.Save();
        }
    }
}