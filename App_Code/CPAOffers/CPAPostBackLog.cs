using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using Prem.PTC.Members;
using System.Collections.Generic;
using System.Net;
using Prem.PTC.Members;
using System.Web.UI.WebControls;
using System.Web;
using Resources;
using Prem.PTC.Advertising;
using System.Linq;
using Prem.PTC.Referrals;
using Titan;

namespace Prem.PTC.Offers
{

    public class CPAPostbackLog : BaseTableObject
    {
        public CPAPostbackLog()
            : base() { }

        public CPAPostbackLog(int id) : base(id) { }

        public CPAPostbackLog(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "CPAPostBackLogs"; } }
        protected override string dbTable { get { return TableName; } }


        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        //Strings
        [Column("NetworkName")]
        public string NetworkName { get { return a1; } set { a1 = parseLength(value, 50); SetUpToDateAsFalse(); } }

        [Column("Username")]
        public string Username { get { return a2; } set { a2 = parseLength(value, 50); SetUpToDateAsFalse(); } }

        [Column("WebsiteId")]
        public string WebsiteId { get { return a3; } set { a3 = parseLength(value, 10); SetUpToDateAsFalse(); } }

        [Column("CampaignId")]
        public string OfferId { get { return a4; } set { a4 = parseLength(value, 10); SetUpToDateAsFalse(); } }

        [Column("IPFrom")]
        public string IPFrom { get { return a5; } set { a5 = parseLength(value, 50); SetUpToDateAsFalse(); } }

        [Column("OfferTitle")]
        public string OfferTitle { get { return a6; } set { a6 = parseLength(value, 50); SetUpToDateAsFalse(); } }

        [Column("Rate")]
        public string Rate { get { return a7; } set { a7 = parseLength(value, 50); SetUpToDateAsFalse(); } }

        [Column("CreditType")]
        public string CreditType { get { return a8; } set { a8 = parseLength(value, 5); SetUpToDateAsFalse(); } }

        [Column("UserCreditedMoney")]
        public string UserCreditedMoney { get { return a9; } set { a9 = parseLength(value, 10); SetUpToDateAsFalse(); } }

        [Column("DateHappened")]
        public DateTime DateHappened { get { return dt; } set { dt = value; SetUpToDateAsFalse(); } }

        [Column("Statusek")]
        protected int IntStatus { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

        [Column("IsDeleted")]
        public bool IsDeleted { get { return _isDeleted; } set { _isDeleted = value; SetUpToDateAsFalse(); } }

        [Column("SentBalance")]
        public string SentBalance { get { return e1; } set { e1 = parseLength(value, 10); SetUpToDateAsFalse(); } }

        [Column("CalculatedBalance")]
        public string CalculatedBalance { get { return e2; } set { e2 = parseLength(value, 10); SetUpToDateAsFalse(); } }

        [Column("CalculatedBalanceMoney")]
        public Money SentBalanceMoney { get { return e22; } set { e22 = value; SetUpToDateAsFalse(); } }



        private bool q1, q2, q3, q4, q5, q6, _isLocked, _isDeleted;
        private int _id, type;
        private string a1, a2, a3, a4, a5, a6, a7, a8, a9, e1, e2;
        private DateTime dt;
        private Money e22;

        #endregion Columns

        public CPAPostBackLogStatus Status
        {
            get
            {
                return (CPAPostBackLogStatus)IntStatus;
            }
            set
            {
                IntStatus = (int)value;
            }
        }

        private string parseLength(string Input, int Allowed)
        {
            if (Input == null)
                return Input;

            if (Input.Length > Allowed)
                return Input.Substring(0, Allowed - 1);
            return Input;
        }

        public static void Create(AffiliateNetwork Network, string Username, Money SentBalance, Money CalculatedBalance, string OfferId, CPAPostBackLogStatus Status)
        {
            if (OfferId == null)
                OfferId = "?";

            string sent = "-";
            string credited = "-";

            if (Network.CreditAs == CreditAs.Points)
            {
                sent = SentBalance.GetRealTotals().ToString();

                if (CalculatedBalance != null)
                    credited = CalculatedBalance.GetRealTotals().ToString() + " " + AppSettings.PointsName;
            }

            if (Network.CreditAs == CreditAs.MainBalance)
            {
                sent = SentBalance.ToClearString();

                if (CalculatedBalance != null)
                    credited = CalculatedBalance.ToString();
            }

            CPAPostbackLog ol = new CPAPostbackLog();

            ol.DateHappened = DateTime.Now;
            ol.NetworkName = Network.Name;
            ol.SentBalance = sent;
            ol.CalculatedBalance = credited;
            ol.Status = Status;
            ol.SentBalanceMoney = SentBalance;
            ol.OfferId = OfferId;
            ol.Username = Username;
            ol.Save();

        }
    }


    public enum CPAPostBackLogStatus
    {
        Null = 0,

        CreditedByNetwork = 1, 
        ReversedByNetwork = 4,

        //3000
        WrongSignature = 5,
        SentFromUnallowedIP = 6,
        NetworkInactive = 7,
        OfferNotFoundByOfferId = 8,

        SubmissionNotFound = 9,

        //4000
        CreditedAndPointsLocked = 10,
        CreditedByNetworkPointsUnlocked = 11,

        MemberNotFoundByUsername = 12,
        CreditedLockedButRejectedByAdmin = 13,

        ExceededSubmissionLimitForThisOffer = 14
    }

    public static class CPAPostBackLogStatusHelper
    {
        /// <summary>
        /// Returns list control source with all statuses' short descriptions as text 
        /// and int status ordinal as value
        /// </summary>
        public static ListItem[] ListItems
        {
            get
            {
                var query = from CPAPostBackLogStatus status in Enum.GetValues(typeof(CPAPostBackLogStatus))
                            where status != CPAPostBackLogStatus.Null
                            orderby (int)status
                            select new ListItem(status.ShortDescription(), (int)status + "");

                var array = query.ToArray();

                return array;
            }
        }
    }

    public static class CPAPostBackLogExtensions
    {
        /// <summary>
        /// Provides human readable, short description for each status.
        /// </summary>
        public static string ShortDescription(this CPAPostBackLogStatus status)
        {
            if (status == CPAPostBackLogStatus.Null) return "Unknown";

            return Enum.GetName(typeof(CPAPostBackLogStatus), status);
        }
    }
}