using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using Prem.PTC.Members;
using System.Collections.Generic;
using System.Net;
using Prem.PTC.Referrals;
using ExtensionMethods;

namespace Prem.PTC.Offers
{

    public class OfferRegisterEntry : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "OfferRegisterEntries"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("OfferId")]
        public int _OfferId { get { return oid; } set { oid = value; SetUpToDateAsFalse(); } }

        [Column("Username")]
        public string Username { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("PostDate")]
        public DateTime DateAdded { get { return _DateStart; } set { _DateStart = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// If null that offer has not been completed
        /// </summary>
        [Column("CompletedDate")]
        public DateTime DateCompleted { get { return _DateStart2; } set { _DateStart2 = value; SetUpToDateAsFalse(); } }

        [Column("OfferStatus")]
        protected int _Status { get { return os; } set { os = value; SetUpToDateAsFalse(); } }

        [Column("LoginID")]
        public string LoginID { get { return _LoginID; } set { _LoginID = value; SetUpToDateAsFalse(); } }

        [Column("EmailID")]
        public string EmailID { get { return _EmailID; } set { _EmailID = value; SetUpToDateAsFalse(); } }

        [Column("HasBeenLocked")]
        public bool HasBeenLocked { get { return _HasBeenLocked; } set { _HasBeenLocked = value; SetUpToDateAsFalse(); } }

        [Column("IsDaily")]
        public bool IsDaily { get { return _IsDaily; } set { _IsDaily = value; SetUpToDateAsFalse(); } }

        [Column("OfferLevel")]
        public int OfferLevel { get { return _OfferLevel; } set { _OfferLevel = value; SetUpToDateAsFalse(); } }

        [Column("DeviceTypeInt")]
        protected int DeviceTypeInt { get { return _DeviceTypeInt; } set { _DeviceTypeInt = value; SetUpToDateAsFalse(); } }

        private int _id, oid, os, _OfferLevel, _DeviceTypeInt;
        private string name, name2, _LoginID, _EmailID;
        private DateTime _DateStart, _DateStart2;
        private Money _baseValue;
        private bool _HasBeenLocked, _IsDaily;

        #endregion Columns

        public OfferRegisterEntry()
            : base() { }

        public OfferRegisterEntry(int id) : base(id) { }

        public OfferRegisterEntry(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }


        private CPAOffer _offer;

        public CPAOffer Offer
        {
            get
            {
                if (_offer == null)
                    _offer = new CPAOffer(_OfferId);
                return _offer;
            }
        }

        public DeviceType DeviceType
        {
            get
            {
                return (DeviceType)DeviceTypeInt;
            }
            set
            {
                DeviceTypeInt = (int)value;
            }
        }

        public OfferStatus Status
        {
            get
            {
                return (OfferStatus)_Status;
            }
            set
            {
                _Status = (int)value;
            }
        }

        /// <summary>
        /// Before adding, verify that the record does not exists!
        /// </summary>
        /// <param name="offerId"></param>
        /// <param name="username"></param>
        /// <param name="firststatus"></param>
        /// <param name="?"></param>
        public static OfferRegisterEntry AddNew(int offerId, string username, OfferStatus firststatus, bool isDaily, DeviceType deviceType, string loginID = null, string EmailID = null, int OfferLevel = 0)
        {
            var or = new OfferRegisterEntry();
            or._OfferId = offerId;
            or.Username = username;
            or.DateAdded = DateTime.Now;

            if (firststatus == OfferStatus.Completed)
                or.DateCompleted = DateTime.Now;
            else
                or.DateCompleted = OffersManager.DateTimeZero; //We know that it's not completed

            or.Status = firststatus;
            or.LoginID = loginID;
            or.EmailID = EmailID;
            or.IsDaily = isDaily;
            or.DeviceType = deviceType;
            or.Save();

            return or;
        }

        public static OfferRegisterEntry AddNew(CPAOffer offer, string username, OfferStatus firststatus, string loginID = null, string EmailID = null, int OfferLevel = 0)
        {
            return AddNew(offer.Id, username, firststatus, offer.IsDaily, offer.DeviceType, loginID, EmailID, OfferLevel);
        }

        public static List<OfferRegisterEntry> GetAllSubmissionsForOffer(string Username, CPAOffer ThisOffer)
        {
            var dict = TableHelper.MakeDictionary("Username", Username);
            dict.Add("OfferId", ThisOffer.Id);

            return TableHelper.SelectRows<OfferRegisterEntry>(dict);
        }

        public static int GetAllTodaysSubmissionsCountForOffer(int offerId)
        {
            var query = String.Format(
                "DECLARE @CreationDate Date = '{1}'; SELECT COUNT(Id) FROM OfferRegisterEntries WHERE OfferId = {0} AND CONVERT(Date, [PostDate]) = @CreationDate",
                offerId, AppSettings.ServerTime.Date.ToDBString());

            return (int)TableHelper.SelectScalar(query);
        }

        /// <summary>
        /// Throws MsgException with corresponding outut message
        /// Performs check on duplicate & status (Igored, Reported, Denied)
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="theOffer"></param>
        /// <returns></returns>
        public static void CheckDuplicateAndStatus(string Username, CPAOffer theOffer, bool isPostBack = false)
        {
            var list = GetAllSubmissionsForOffer(Username, theOffer);

            //No point in checking submissions, when there are no submisions other then 'Report'
            if (list.Count > 0 && !AppSettings.CPAGPT.ReadOnlyModeEnabled) 
            {
                //Daily
                int howManyWasSubmitedToday = 0;

                foreach (var submission in list)
                {
                    if (submission.DateAdded.Date == AppSettings.ServerTime.Date)
                        howManyWasSubmitedToday++;

                    if (!isPostBack)
                    {
                        if (submission.Status == OfferStatus.Ignored || submission.Status == OfferStatus.Reported
                           || submission.Status == OfferStatus.Denied)
                                throw new MsgException("This offer has been " + submission.Status);
                    }
                }

                if (!theOffer.IsDaily)
                    throw new MsgException(Resources.U3900.ALREADYSUBMITED);

                if (howManyWasSubmitedToday >= theOffer.MaxDailyCredits)
                    throw new MsgException(Resources.U3900.DAILYBUTABOVETHELIMIT.Replace("%n1%", howManyWasSubmitedToday.ToString())
                        .Replace("%n2%", theOffer.MaxDailyCredits.ToString()));

            }
        }

    }
}