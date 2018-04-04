using System;
using Prem.PTC.Members;
using System.Data;

namespace Prem.PTC.Payments
{
    public class PaymentProof : BaseTableObject
    {
        #region Columns
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "PaymentProofs"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Type")]
        protected int IntType { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

        [Column("Amount")]
        public Money Amount { get { return quantity; } set { quantity = value; SetUpToDateAsFalse(); } }

        [Column("Date")]
        public DateTime Date { get { return _Date; } set { _Date = value; SetUpToDateAsFalse(); } }

        [Column("UserId")]
        public int UserId { get { return _CC; } set { _CC = value; SetUpToDateAsFalse(); } }

        [Column("Processor")]
        protected int IntProcessor { get { return _CC1; } set { _CC1 = value; SetUpToDateAsFalse(); } }

        [Column("ProcessorName")]
        public string ProcessorName { get { return pn; } set { pn = value; SetUpToDateAsFalse(); } }

        private int _id, type, _CC, _CC1;
        private DateTime _Date;
        private Money quantity;
        private string pn;
        #endregion Columns

        public PaymentProof()
            : base() { }

        public PaymentProof(int id) : base(id) { }

        public PaymentProof(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public PaymentType Type
        {
            get
            {
                return (PaymentType)IntType;
            }

            set
            {
                IntType = (int)value;
            }
        }

        public PaymentProcessor Processor
        {
            get
            {
                return (PaymentProcessor)IntProcessor;
            }

            set
            {
                IntProcessor = (int)value;
            }
        }


        public static void Add(int userId, Money amount, PaymentType type, PaymentProcessor processor, string processorName = "")
        {
            var proof = new PaymentProof();
            proof.Type = type;
            proof.Amount = amount;
            proof.Date = DateTime.Now;
            proof.UserId = userId;
            proof.Processor = processor;
            proof.ProcessorName = processorName;
            proof.Save();

            //Sending Payout Confirmation, Fired for all PP, BTC, CommissionPayout
            try
            {
                var user = new Member(userId);
                if (AppSettings.Emails.PayoutEmailMessageEnabled)
                    Mailer.SendPayoutConfirmationMessage(user.Name, amount, user.Email);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }

        public static void Add(Member User, PayoutRequest request, PaymentType PaymentTypeT = PaymentType.Manual)
        {
            //Add payment proof
            if (request.PaymentProcessor == "PayPal")
                PaymentProof.Add(User.Id, request.Amount, PaymentTypeT, PaymentProcessor.PayPal);
            else if (request.PaymentProcessor == "Payza")
                PaymentProof.Add(User.Id, request.Amount, PaymentTypeT, PaymentProcessor.Payza);
            else if (request.PaymentProcessor == "PerfectMoney")
                PaymentProof.Add(User.Id, request.Amount, PaymentTypeT, PaymentProcessor.PerfectMoney);
            else if (request.PaymentProcessor == "OKPay")
                PaymentProof.Add(User.Id, request.Amount, PaymentTypeT, PaymentProcessor.OKPay);
            else if (request.PaymentProcessor == "EgoPay")
                PaymentProof.Add(User.Id, request.Amount, PaymentTypeT, PaymentProcessor.EgoPay);
            else if (request.PaymentProcessor == "Neteller")
                PaymentProof.Add(User.Id, request.Amount, PaymentTypeT, PaymentProcessor.Neteller);
            else if (request.PaymentProcessor == "SolidTrustPay")
                PaymentProof.Add(User.Id, request.Amount, PaymentTypeT, PaymentProcessor.SolidTrustPay);
            else if (request.PaymentProcessor == "Payeer")
                PaymentProof.Add(User.Id, request.Amount, PaymentTypeT, PaymentProcessor.Payeer);
            else if (request.PaymentProcessor == "Papara")
                PaymentProof.Add(User.Id, request.Amount, PaymentTypeT, PaymentProcessor.Papara);
            else if (request.PaymentProcessor == "MPesa")
                PaymentProof.Add(User.Id, request.Amount, PaymentTypeT, PaymentProcessor.MPesa);
            else if (request.PaymentProcessor == "MPesaAgent")
                PaymentProof.Add(User.Id, request.Amount, PaymentTypeT, PaymentProcessor.MPesaAgent);
            else if (request.PaymentProcessor == "LocalBitcoins")
                PaymentProof.Add(User.Id, request.Amount, PaymentTypeT, PaymentProcessor.LocalBitcoins);
            else if (request.PaymentProcessor == "Revolut")
                PaymentProof.Add(User.Id, request.Amount, PaymentTypeT, PaymentProcessor.Revolut);
            else
            {
                //Custom PP
                PaymentProof.Add(User.Id, request.Amount, PaymentTypeT, PaymentProcessor.CustomPayoutProcessor, request.PaymentProcessor);
            }
        }
    }


    public enum PaymentType
    {
        Null = 0,
        Instant = 1,
        Manual = 2
    }


    // VERY IMPORTANT !!!
    // IF YOU ADD A BTC PROCESSOR, MAKE SURE TO ADD IT TO: BitcoinFactory.AllBTCProcessors LIST
    public enum PaymentProcessor
    {
        Null = 0,
        PayPal = 1,
        Payza = 2,
        PerfectMoney = 3,
        OKPay = 4,
        EgoPay = 5,
        Neteller = 6,
        SolidTrustPay = 7,
        Payeer = 8,
        BTC = 9,
        AdvCash = 10,
        Papara = 11,
        MPesa = 12,
        LocalBitcoins = 13,
        Coinbase = 14,
        MPesaAgent = 15,
        CoinPayments = 16,
        Revolut = 17,
        Blocktrail = 18,
        XRP = 19,
        ETH = 20,

        ViaRepresentative = 99, //Payment made via representative
        CustomPayoutProcessor = 100 //CUstom Payout Processor
    }
}