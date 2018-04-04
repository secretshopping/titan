using Prem.PTC;
using System;
using System.Data;

namespace Titan.CustomFeatures
{
    public class RetireyoungHistoryPay : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "RetireyoungHistoryPays"; } }

        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("UserId")]
        public int UserId { get { return _userId; } set { _userId = value; SetUpToDateAsFalse(); } }

        [Column("DateOccured")]
        public DateTime Date { get { return _date; } set { _date = value; SetUpToDateAsFalse(); } }

        [Column("Amount")]
        public Money Amount { get { return _amount; } set { _amount = value; SetUpToDateAsFalse(); } }

        [Column("Aggregate")]
        public Money Aggregate { get { return _aggregate; } set { _aggregate = value; SetUpToDateAsFalse(); } }

        [Column("Note")]
        public string Note { get { return _note; } set { _note = value; SetUpToDateAsFalse(); } }

        private int _id, _userId;
        private Money _amount, _aggregate;
        private DateTime _date;
        private string _note;

        public RetireyoungHistoryPay() : base() { }

        public RetireyoungHistoryPay(int id) : base(id) { }

        public RetireyoungHistoryPay(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }
    }
}