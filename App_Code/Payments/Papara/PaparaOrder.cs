using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{

    public class PaparaOrder : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public new static string TableName { get { return "PaparaOrders"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Amount")]
        public Money Amount { get { return _Amount; } set { _Amount = value; SetUpToDateAsFalse(); } }

        [Column("CommandName")]
        public string CommandName { get { return _apiKey; } set { _apiKey = value; SetUpToDateAsFalse(); } }

        [Column("Args")]
        public string Args { get { return _walletNumber; } set { _walletNumber = value; SetUpToDateAsFalse(); } }

        [Column("DateAdded")]
        public DateTime DateAdded { get { return _DateAdded; } set { _DateAdded = value; SetUpToDateAsFalse(); } }

        [Column("IsPaid")]
        public bool IsPaid { get { return _IsPaid; } set { _IsPaid = value; SetUpToDateAsFalse(); } }


        private int _id, _UserId;
        private string _apiName, _apiKey, _walletNumber, _secretKey;
        private bool _IsPaid;
        private DateTime _DateAdded;
        private Money _Amount;

        #endregion

        #region Constructors

        public PaparaOrder() : base() { }
        public PaparaOrder(int id) : base(id) { }
        public PaparaOrder(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public static int Create(Money amount, string command, string args)
        {
            PaparaOrder order = new PaparaOrder();
            order.Amount = amount;
            order.CommandName = command;
            order.Args = args;
            order.DateAdded = AppSettings.ServerTime;
            order.IsPaid = false;
            order.Save();

            return order.Id;
        }

        public static PaparaOrder Get(int id)
        {
            var result = new PaparaOrder(id);
            if (!result.IsPaid)
                return result;

            return null;
        }
    }
}