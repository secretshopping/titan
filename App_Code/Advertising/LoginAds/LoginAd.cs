using System;
using System.Data;
using Prem.PTC.Offers;

namespace Prem.PTC.Advertising
{
    [Serializable]
    public class LoginAd : GeolocationBase
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "LoginAds"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("CreatorUserId")]
        public int CreatorUserId { get { return _CreatorUserId; } set { _CreatorUserId = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int StatusInt { get { return _Status; } set { _Status = value; SetUpToDateAsFalse(); } }

        public AdvertStatus Status
        {
            get { return (AdvertStatus)StatusInt; }
            set { StatusInt = (int)value; }
        }

        [Column("TargetUrl")]
        public string TargetUrl { get { return UrlCreator.ParseUrl(_TargetUrl); } set { _TargetUrl = value; SetUpToDateAsFalse(); } }

        [Column("DisplayDate")]
        public DateTime DisplayDate { get { return _DisplayDate; } set { _DisplayDate = value; SetUpToDateAsFalse(); } }

        [Column("PurchaseDate")]
        public DateTime PurchaseDate { get { return _PurchaseDate; } set { _PurchaseDate = value; SetUpToDateAsFalse(); } }

        [Column("TotalViews")]
        public int TotalViews { get { return _TotalViews; } set { _TotalViews = value; SetUpToDateAsFalse(); } }

        [Column("PricePaid")]
        public Money PricePaid { get { return _PricePaid; } set { _PricePaid = value; SetUpToDateAsFalse(); } }

        [Column("TargetBalance")]
        protected int TargetBalanceInt { get { return _TargetBalance; } set { _TargetBalance = value; SetUpToDateAsFalse(); } }

        public PurchaseBalances TargetBalance { get { return (PurchaseBalances)TargetBalanceInt; } set { TargetBalanceInt = (int)value; } }

        private int _Id, _Status, _CreatorUserId, _TotalViews, _TargetBalance;
        private string _TargetUrl;
        private DateTime _DisplayDate, _PurchaseDate;
        private Money _PricePaid;
        #endregion

        #region Constructors
        public LoginAd() : base() { }

        public LoginAd(int id) : base(id) { }

        public LoginAd(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }
        #endregion
    }
}