using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Data;
using Prem.PTC.Members;
using Prem.PTC.Payments;

namespace Titan
{
    public class IncomingPayment : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "IncomingPayments"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int IntStatus { get { return status; } set { status = value; SetUpToDateAsFalse(); } }

        [Column("Username")]
        public string Username { get { return _ClientHTML; } set { _ClientHTML = value; SetUpToDateAsFalse(); } }

        [Column("WhenMade")]
        public DateTime WhenMade { get { return _DisplayName; } set { _DisplayName = value; SetUpToDateAsFalse(); } }

        [Column("Amount")]
        public Money Amount { get { return _Hash; } set { _Hash = value; SetUpToDateAsFalse(); } }

        [Column("PaymentProcessor")]
        protected int IntPaymentProcessor { get { return _CreditAs; } set { _CreditAs = value; SetUpToDateAsFalse(); } }


        private int _id, status, _CreditAs, _IntWhatIsSent;
        private string _ClientHTML;
        private Money _Hash;
        private DateTime _DisplayName;

        #endregion Columns

        #region Constructors

        public IncomingPayment()
            : base() { }

        public IncomingPayment(int id) : base(id) { }

        public IncomingPayment(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        #endregion Constructors

        #region Properties


        public PaymentProcessor PaymentProcessor
        {
            get
            {
                return (PaymentProcessor)IntPaymentProcessor;
            }

            set
            {
                IntPaymentProcessor = (int)value;
            }
        }

        public PaymentFor PaymentFor
        {
            get
            {
                return (PaymentFor)IntStatus;
            }

            set
            {
                IntStatus = (int)value;
            }
        }


        #endregion Properties

    }

    public enum PaymentFor
    {
        Null = 0,

        TransferMoney = 1,
        Advertisement = 2
    }
}