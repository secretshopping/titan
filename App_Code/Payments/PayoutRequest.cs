using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Prem.PTC.Payments
{
    public class PayoutRequest : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "PayoutRequests"; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "PayoutRequestId";
            public const string Username = "Username";
            public const string RequestDate = "RequestDate"; // Note: When is a Keyword
            public const string IsPaid = "IsPaid";
            public const string PaymentProcessor = "PaymentProcessor";
            public const string Amount = "Amount";
            public const string PaymentAddress = "PaymentAddress";
            public const string IsRequest = "IsRequest";
            public const string BalanceType = "BalanceType";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Username)]
        public string Username { get { return _username; } set { _username = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RequestDate)]
        public DateTime RequestDate { get { return _when; } set { _when = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsPaid)]
        public bool IsPaid { get { return _isPaid; } set { _isPaid = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PaymentProcessor)]
        public string PaymentProcessor { get { return _paymentProcessor; } set { _paymentProcessor = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Amount)]
        public Money Amount { get { return _amount; } set { _amount = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PaymentAddress)]
        public string PaymentAddress { get { return _paymentAddress; } set { _paymentAddress = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsRequest)]
        public bool IsRequest { get { return _isRequest; } set { _isRequest = value; SetUpToDateAsFalse(); } }

        [Column(Columns.BalanceType)]
        protected int BalanceTypeInt { get { return _BalanceTypeInt; } set { _BalanceTypeInt = value; SetUpToDateAsFalse(); } }

        public BalanceType BalanceType
        {
            get
            {
                return (BalanceType)BalanceTypeInt;
            }
            set
            {
                BalanceTypeInt = (int)value;
            }
        }


        private int _id, _BalanceTypeInt;
        private string _username, _paymentProcessor, _paymentAddress;
        private DateTime _when;
        private Money _amount;
        private bool _isPaid;
        private bool _isRequest;

        #endregion

        #region Constructors

        public PayoutRequest() : base() { }
        public PayoutRequest(int id) : base(id) { }
        public PayoutRequest(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public static implicit operator TransactionRequest(PayoutRequest request)
        {
            return new TransactionRequest()
            {
                MemberName = request.Username,
                PayeeId = request.PaymentAddress,
                Payment = request.Amount
            };
        }

        public static string GetPayoutRequestsSQLQuery(bool showOnlyBannedMembers = false)
        {
            return @"SELECT pr.* FROM [PayoutRequests] AS pr " + GetPayoutRequestsSQLConditions(showOnlyBannedMembers) + " ORDER BY pr.[RequestDate] DESC";           
        }

        public static Money GetSumOfAllPendingPayoutRequests()
        {
            var result = TableHelper.SelectScalar(
                @"SELECT SUM(pr.Amount) FROM [PayoutRequests] AS pr " + GetPayoutRequestsSQLConditions());

            if (!(result is DBNull))
                return new Money((Decimal)result);

            return Money.Zero;
        }


        public static void GeneratePendingRequestsCSV_MarkAsPaid_AndReturn(GridView gridview, bool markAsPaid = true)
        {
            string fileName = "PendingPayoutsRequestsExport_" + AppSettings.ServerTime.ToString("yyyy_MM_dd") + ".csv";

            try
            {
                string content = DBArchiverAPI.ExportToCSVString(
                    @"SELECT pr.PaymentAddress, pr.Amount, pr.Username, FORMAT(pr.RequestDate,'MM/dd/yyyy') AS [RequestDate], pr.PaymentProcessor FROM [PayoutRequests] AS pr " + GetPayoutRequestsSQLConditions() + " ORDER BY pr.[RequestDate] DESC");

                if (markAsPaid)
                    MarkAllRequestsAsPaid();

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.ContentType = "application/text";
                HttpContext.Current.Response.Output.Write(content);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log("Error while exporting CSV to file: " + ex.Message);
                throw new MsgException(ex.Message);
            }
        }

        public static void MarkAllRequestsAsPaid()
        {
            List<PayoutRequest> pendingRequests = TableHelper.GetListFromRawQuery<PayoutRequest>(GetPayoutRequestsSQLQuery());

            foreach (var request in pendingRequests)
                PayoutManager.MarkAsPaid(request);
        }

        public static string GetPayoutRequestsSQLConditions(bool showOnlyBannedMembers = false)
        {
            string SQLKeyWord = String.Empty;

            if (showOnlyBannedMembers)
                SQLKeyWord = "NOT ";

            return String.Format(@"LEFT JOIN [Users] AS m ON pr.[Username] = m.[Username]
                               WHERE (pr.[IsPaid] = 0) AND (m.[AccountStatusInt] {0}IN (1,10)) AND (PaymentProcessor <> 'REJECTED')
                               AND (pr.[IsRequest] = 1)", SQLKeyWord);
            
        }

        public static int GetTransactionsAmount()
        {
            return Convert.ToInt32(TableHelper.SelectScalar("SELECT COUNT(*) FROM PayoutRequests WHERE IsRequest = 'False'").ToString());
        }

        public static Money GetTransactionsValue()
        {
            return Money.Parse(TableHelper.SelectScalar("SELECT SUM(Amount) FROM PayoutRequests WHERE IsRequest = 'False'").ToString());
        }

        public static Money GetTotalPaidOutAmount()
        {
            var query = "SELECT SUM(Amount) FROM PayoutRequests WHERE IsPaid = 'True' AND IsRequest = 'True'";
            var scalar = TableHelper.SelectScalar(query);
            var result = scalar.ToString() != string.Empty ? scalar : 0m;

            return new Money((decimal)result);
        }
    }
}