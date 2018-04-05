using System;
using System.Data;
using System.Collections.Generic;
using Prem.PTC.Advertising;
using Titan;

namespace Prem.PTC.Offers
{
    [Serializable]
    public abstract class CPAOfferBase : GeolocationBase
    {
        #region Columns
        public override Database Database { get { return Database.Client; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Title")]
        public string Title { get { return name; } set { name = parseLength(value, 500); SetUpToDateAsFalse(); } }

        [Column("Description")]
        public string Description { get { return name2; } set { name2 = value; SetUpToDateAsFalse(); } }

        [Column("DateAdded")]
        public DateTime DateAdded { get { return _DateStart; } set { _DateStart = value; SetUpToDateAsFalse(); } }

        [Column("BaseValue")]
        public Money BaseValue { get { return _baseValue; } set { _baseValue = value; SetUpToDateAsFalse(); } }

        [Column("CategoryId")]
        public int IntCategory { get { return _IntCategory; } set { _IntCategory = value; SetUpToDateAsFalse(); } }

        public DateTime LastCredited { get { return OffersManager.DateTimeZero; } set { } }

        [Column("IsDaily")]
        public bool IsDaily { get { return _IsDaily; } set { _IsDaily = value; SetUpToDateAsFalse(); } }

        [Column("IsSyncWithNetwork")]
        public bool IsSyncWithNetwork { get { return _IsDailyDD; } set { _IsDailyDD = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int IntStatus { get { return _IntStatus; } set { _IntStatus = value; SetUpToDateAsFalse(); } }

        [Column("ImageURL")]
        public string ImageURL { get { return _ImageURL; } set { _ImageURL = parseLength(value, 150); SetUpToDateAsFalse(); } }

        [Column("NetworkRate")]
        public string NetworkRate { get { return _NetworkRate; } set { _NetworkRate = parseLength(value, 10); SetUpToDateAsFalse(); } }

        [Column("LoginBoxRequired")]
        public bool LoginBoxRequired { get { return lbr1; } set { lbr1 = value; SetUpToDateAsFalse(); } }

        [Column("EmailBoxRequired")]
        public bool EmailBoxRequired { get { return lbr2; } set { lbr2 = value; SetUpToDateAsFalse(); } }

        [Column("AdvertiserUsername")]
        public string AdvertiserUsername { get { return n3; } set { n3 = parseLength(value, 50); SetUpToDateAsFalse(); } }

        [Column("CreditsBought")]
        public int CreditsBought { get { return _CreditsBought; } set { _CreditsBought = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// Daily offer can be completed max this number of times (daily)
        /// </summary>
        [Column("MaxDailyCredits")]
        public int MaxDailyCredits { get { return _MaxDailyCredits; } set { _MaxDailyCredits = value; SetUpToDateAsFalse(); } }

        [Column("TargetURL")]
        public string TargetURL { get { return _TargetURL; } set { _TargetURL = value; SetUpToDateAsFalse(); } }

        [Column("NetworkName")]
        public string NetworkName { get { return q1; } set { q1 = parseLength(value, 80); SetUpToDateAsFalse(); } }

        [Column("NetworkOfferIdInt")]
        public string NetworkOfferId { get { return _NetworkOfferId; } set { _NetworkOfferId = value; SetUpToDateAsFalse(); } }

        [Column("IsFromAutomaticNetwork")]
        public bool IsFromAutomaticNetwork { get { return w1; } set { w1 = value; SetUpToDateAsFalse(); } }

        [Column("IsIgnored")]
        public bool IsIgnored { get { return w1AAA; } set { w1AAA = value; SetUpToDateAsFalse(); } }

        [Column("CreditOfferAs")]
        protected int CreditOfferAsInt { get { return _CreditOfferAs; } set { _CreditOfferAs = value; SetUpToDateAsFalse(); } }

        [Column("RequiredMembership")]
        public int RequiredMembership { get { return _RequiredMembership; } set { _RequiredMembership = value; SetUpToDateAsFalse(); } }

        [Column("OfferLevel")]
        public string OfferLevel { get { return _OfferLevel; } set { _OfferLevel = value; SetUpToDateAsFalse(); } }

        [Column("DeviceType")]
        protected int DeviceTypeInt { get { return _DeviceTypeInt; } set { _DeviceTypeInt = value; SetUpToDateAsFalse(); } }

        [Column("MaxGlobalDailySubmits")]
        public int MaxGlobalDailySubmits { get { return _MaxGlobalDailySubmits; } set { _MaxGlobalDailySubmits = value; SetUpToDateAsFalse(); } }

        [Column("TargetBalance")]
        protected int TargetBalanceInt { get { return _TargetBalance; } set { _TargetBalance = value; SetUpToDateAsFalse(); } }

        public PurchaseBalances TargetBalance { get { return (PurchaseBalances)TargetBalanceInt; } set { TargetBalanceInt = (int)value; } }

        private int _id, _IntCategory, _IntStatus, _CreditsBought, q2QE, _CreditOfferAs, _MaxDailyCredits, geo3, geo4, geo5, _RequiredMembership, _DeviceTypeInt,
            _MaxGlobalDailySubmits, _TargetBalance;
        private bool _IsDaily, lbr1, lbr2, w1, _IsDailyDD, w1AAA;
        private string name, name2, _ImageURL, n3, _TargetURL, q1, q2, q3, _NetworkRate, geo, geo1, geo2, _OfferLevel,
            _NetworkOfferId;
        protected DateTime _DateStart, _DateStart2, _DateLastCredited;
        private Money _baseValue;
        protected List<OfferRegisterEntry> _entries = null;
        protected int _rating = -1;
        protected int _completed = -1;
        protected string avgcredit = "-1";
        private int _completedClicks = -1;

        public string Temp_EmailID { get; set; }
        public string Temp_LoginID { get; set; }
        public int Temp { get; set; }
        public DateTime Temp_Time { get; set; }

        public bool GlobalDailySubmitsRestricted
        {
            get
            {
                return IsDaily && MaxGlobalDailySubmits < 2000000000;
            }
            set
            {
                MaxGlobalDailySubmits = value ? 1000000000 : 2000000000;
            }
        }

        public CreditOfferAs CreditOfferAs
        {
            get
            {
                return (CreditOfferAs)CreditOfferAsInt;
            }
            set
            {
                CreditOfferAsInt = (int)value;
            }
        }

        public CPACategory Category
        {
            get
            {
                return new CPACategory(IntCategory);
            }
            set
            {
                IntCategory = (int)value.Id;
            }
        }

        public AdvertStatus Status
        {
            get
            {
                return (AdvertStatus)IntStatus;
            }
            set
            {
                IntStatus = (int)value;
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

        public override bool IsUpToDate
        {
            get
            {
                return base.IsUpToDate;
            }
        }

        protected Banner _bannerImage;
        public Banner BannerImage
        {
            get { return _bannerImage; }
            set
            {
                _bannerImage = value;
                SetUpToDateAsFalse();
            }
        }
        #endregion Columns

        #region Constructors
        public CPAOfferBase() : base()
        {
            NetworkOfferId = "-1";
        }

        public CPAOfferBase(int id) : base(id) { }

        public CPAOfferBase(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }
        #endregion Constructors

        #region Help
        public int GetProgressPercent()
        {
            return Convert.ToInt32(((double)CompletedClicks / (double)CreditsBought) * 100);
        }

        public int CompletedClicks
        {
            get
            {
                if (_completedClicks == -1)
                {
                    var where = TableHelper.MakeDictionary("OfferId", Id);
                    where.Add("OfferStatus", (int)Offers.OfferStatus.Completed);
                    _completedClicks = TableHelper.CountOf<OfferRegisterEntry>(where);
                }
                return _completedClicks;
            }
        }

        public void Save(bool forceSave = false)
        {
            base.Save(forceSave);
        }

        public static void Delete(int id)
        {
            TableHelper.DeleteRows<CPAOffer>("id", id);
        }

        protected string parseLength(string Input, int Allowed)
        {
            if (Input == null)
                return Input;

            if (Input.Length > Allowed)
                return Input.Substring(0, Allowed - 1);
            return Input;
        }

        public Money GetAmount(int CPAOffersPercent)
        {
            return Money.MultiplyPercent(BaseValue, CPAOffersPercent);
        }

        public string GetOfferLevel()
        {
            return OfferLevel;
        }
        #endregion Help
    }
}