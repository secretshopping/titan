using System;
using System.Data;

namespace Prem.PTC.Payments
{
    public class UsersPaymentProcessorsAddress : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "UsersPaymentProcessorsAddresses"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("UserId")]
        public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

        [Column("CustomPayoutProcessorId")]
        public int CustomPayoutProcessorId { get { return _CustomPayoutProcessorId; } set { _CustomPayoutProcessorId = value; SetUpToDateAsFalse(); } }

        [Column("ProcessorTypeInt")]
        public int ProcessorTypeInt { get { return _ProcessorTypeInt; } set { _ProcessorTypeInt = value; SetUpToDateAsFalse(); } }

        [Column("PaymentAddress")]
        public string PaymentAddress { get { return _PaymentAddress; } set { _PaymentAddress = value; SetUpToDateAsFalse(); } }

        [Column("LastChanged")]
        public DateTime LastChanged { get { return _LastChanged; } set { _LastChanged = value; SetUpToDateAsFalse(); } }

        private int _id, _UserId, _CustomPayoutProcessorId, _ProcessorTypeInt;
        private string _PaymentAddress;
        private DateTime _LastChanged;

        #endregion

        #region Constructors

        public UsersPaymentProcessorsAddress() : base() { }
        public UsersPaymentProcessorsAddress(int id) : base(id) { }
        public UsersPaymentProcessorsAddress(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors


        public static bool HasAddress(int userId, PaymentProcessorInfo processor)
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM UsersPaymentProcessorsAddresses WHERE UserId = {0} AND ProcessorTypeInt = {1}";

            if (processor.IsAutomaticProcessor)
                count = (int)TableHelper.SelectScalar(String.Format(query, userId, (int)processor.ProcessorType));
            else
                count = (int)TableHelper.SelectScalar(
                    String.Format(query + " AND CustomPayoutProcessorId = {2}", userId, (int)processor.ProcessorType, processor.CustomPayoutProcessorId));

            return count > 0;
        }

        public static UsersPaymentProcessorsAddress GetAddress(int userId, PaymentProcessorInfo processor)
        {
            UsersPaymentProcessorsAddress address;
            string query = "SELECT * FROM UsersPaymentProcessorsAddresses WHERE UserId = {0} AND ProcessorTypeInt = {1}";

            if (!HasAddress(userId, processor))
                return null;

            if (processor.IsAutomaticProcessor)
                address = TableHelper.GetListFromRawQuery<UsersPaymentProcessorsAddress>(String.Format(query, userId, (int)processor.ProcessorType))[0];
            else
                address = TableHelper.GetListFromRawQuery<UsersPaymentProcessorsAddress>(
                    String.Format(query + " AND CustomPayoutProcessorId = {2}", userId, (int)processor.ProcessorType, processor.CustomPayoutProcessorId))[0];

            return address;
        }

        public static void TrySetAddress(int userId, PaymentProcessorInfo processor, string address)
        {
            var CurrentAddress = GetAddress(userId, processor);

            if (CurrentAddress != null && CurrentAddress.PaymentAddress != address)
            {
                //Update
                CurrentAddress.PaymentAddress = address;
                CurrentAddress.LastChanged = AppSettings.ServerTime;
                CurrentAddress.Save();
            }
            else
            {
                //Create new
                UsersPaymentProcessorsAddress NewAddress = new UsersPaymentProcessorsAddress();
                NewAddress.LastChanged = AppSettings.ServerTime.AddYears(-10);
                NewAddress.UserId = userId;
                NewAddress.CustomPayoutProcessorId = processor.CustomPayoutProcessorId;
                NewAddress.PaymentAddress = address;
                NewAddress.ProcessorTypeInt = (int)processor.ProcessorType;
                NewAddress.Save();
            }
        }

    }
}