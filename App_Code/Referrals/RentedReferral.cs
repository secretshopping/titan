using Prem.PTC.Memberships;
using Prem.PTC.Utils;
using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;

namespace Prem.PTC.Referrals
{
    /// <summary>
    /// Summary description for Bot
    /// </summary>
    public class RentedReferral : BaseTableObject
    {
        #region Columns
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "RentedReferrals"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("RefId", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("OwnerUsername")]
        public string OwnerUsername { get { return _username1; } set { _username1 = value; SetUpToDateAsFalse(); } }

        [Column("FiredBy")]
        public string FiredByUsername { get { return _username2; } set { _username2 = value; SetUpToDateAsFalse(); } }

        [Column("BotClass")]
        public int BotClass { get { return _bc; } set { _bc = value; SetUpToDateAsFalse(); } }

        [Column("ReferralSince")]
        public DateTime ReferralSince { get { return _sentDate; } set { _sentDate = value; SetUpToDateAsFalse(); } }

        [Column("HasAutoPay")]
        public bool HasAutoPay { get { return _isRead; } set { _isRead = value; SetUpToDateAsFalse(); } }

        [Column("ExpireDate")]
        public DateTime ExpireDate { get { return _sentDate2; } set { _sentDate2 = value; SetUpToDateAsFalse(); } }

        [Column("ClicksStats")]
        public string ClicksStats { get { return _text; } set { _text = value; SetUpToDateAsFalse(); } }

        [Column("LastClick")]
        public DateTime? LastClick { get { return _sentDate3; } set { _sentDate3 = value; SetUpToDateAsFalse(); } }

        [Column("TotalClicks")]
        public int TotalClicks { get { return _totalClicks; } set { _totalClicks = value; SetUpToDateAsFalse(); } }

        [Column("PointsEarnedToReferer")]
        public int PointsEarnedToReferer { get { return _totalClicks2; } set { _totalClicks2 = value; SetUpToDateAsFalse(); } }

        [Column("LastPointableActivity")]
        public DateTime? LastPointableActivity { get { return _sentDate35; } set { _sentDate35 = value; SetUpToDateAsFalse(); } }


        private int _id, _bc, _totalClicks, _totalClicks2;
        private string _username1, _username2, _text;
        private DateTime _sentDate, _sentDate2;
        private DateTime? _sentDate3, _sentDate35;
        private bool _isRead;

        #endregion Columns

        public List<int> ClicksStatsList
        {
            get
            {
                return TableHelper.GetIntListFromString(ClicksStats);
            }
            set
            {
                ClicksStats = TableHelper.GetStringFromIntList(value);
            }
        }

        public bool IsBot
        {
            get
            {
                if (BotClass == -1)
                    return false;
                return true;
            }
        }

        public RentedReferral(int id) :base(id){ }

        public RentedReferral()
            : base() { }

        public RentedReferral(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }
    }
}