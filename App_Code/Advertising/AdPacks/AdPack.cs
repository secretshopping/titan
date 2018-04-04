using System;
using System.Data;
using Prem.PTC.Utils;

namespace Prem.PTC.Advertising
{
    public class AdPack : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "AdPacks"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("AdPacksAdvertId")]
        public int AdPacksAdvertId { get { return _AdPacksAdvertId; } set { _AdPacksAdvertId = value; SetUpToDateAsFalse(); } }

        [Column("TotalClicks")]
        public int TotalClicks { get { return _TotalClicks; } set { _TotalClicks = value; SetUpToDateAsFalse(); } }

        [Column("TotalNormalBannerImpressions")]
        public int TotalNormalBannerImpressions { get { return _TotalNormalBannerImpressions; } set { _TotalNormalBannerImpressions = value; SetUpToDateAsFalse(); } }

        [Column("TotalConstantBannerImpressions")]
        public int TotalConstantBannerImpressions { get { return _TotalConstantBannerImpressions; } set { _TotalConstantBannerImpressions = value; SetUpToDateAsFalse(); } }

        [Column("ConstantBannerImpressionsBought")]
        public int ConstantBannerImpressionsBought { get { return _ConstantBannerImpressionsBought; } set { _ConstantBannerImpressionsBought = value; SetUpToDateAsFalse(); } }

        [Column("NormalBannerImpressionsBought")]
        public int NormalBannerImpressionsBought { get { return _NormalBannerImpressionsBought; } set { _NormalBannerImpressionsBought = value; SetUpToDateAsFalse(); } }

        [Column("ClicksBought")]
        public int ClicksBought { get { return _ClicksBought; } set { _ClicksBought = value; SetUpToDateAsFalse(); } }

        [Column("PurchaseDate")]
        public DateTime PurchaseDate { get { return _PurchaseDate; } set { _PurchaseDate = value; SetUpToDateAsFalse(); } }

        [Column("MoneyReturned")]
        public Money MoneyReturned { get { return _MoneyReturned; } set { _MoneyReturned = value; SetUpToDateAsFalse(); } }

        [Column("MoneyToReturn")]
        public Money MoneyToReturn { get { return _MoneyToReturn; } set { _MoneyToReturn = value; SetUpToDateAsFalse(); } }

        [Column("UserCustomGroupId")]
        public int UserCustomGroupId { get { return _UserCustomGroupId; } set { _UserCustomGroupId = value; SetUpToDateAsFalse(); } }

        [Column("UserId")]
        public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

        [Column("AdPackTypeId")]
        public int AdPackTypeId { get { return _AdPackTypeId; } set { _AdPackTypeId = value; SetUpToDateAsFalse(); } }

        [Column("DistributionPriority")]
        public Decimal DistributionPriority { get { return _DistributionPriority; } set { _DistributionPriority = value; SetUpToDateAsFalse(); } }

        [Column("DisplayTime")]
        public int DisplayTime { get { return _DisplayTime; } set { _DisplayTime = value; SetUpToDateAsFalse(); } }

        [Column("BalanceBoughtType")]
        protected int BalanceBoughtTypeInt { get { return _BalanceBoughtType; } set { _BalanceBoughtType = value; SetUpToDateAsFalse(); } }
        
        public PurchaseBalances BalanceBoughtType
        {
            get
            {
                return (PurchaseBalances)BalanceBoughtTypeInt;
            }
            set
            {
                BalanceBoughtTypeInt = (int)value;
            }
        }

        public AdPacksAdvert AdPackAdvert
        {
            get
            {
                if (_AdPackAdvert == null)
                    _AdPackAdvert = new AdPacksAdvert(AdPacksAdvertId);
                return _AdPackAdvert;
            }
            set
            {
                _AdPackAdvert = value;
                AdPacksAdvertId = value.Id;
            }
        }

        private int _Id, _AdPacksAdvertId, _TotalNormalBannerImpressions, _TotalClicks, _Packages, _TotalConstantBannerImpressions, _UserCustomGroupId, _AdPackTypeId, _BalanceBoughtType;
        private int _ConstantBannerImpressionsBought, _NormalBannerImpressionsBought, _ClicksBought, _UserId, _DisplayTime;
        private Money _MoneyReturned, _MoneyToReturn;
        private AdPacksAdvert _AdPackAdvert;
        private DateTime _PurchaseDate;
        private Decimal _DistributionPriority;


        #endregion

        #region Constructors

        /// <summary>
        /// Creates blank instance of Packs class
        /// </summary>
        public AdPack()
            : base()
        { }

        public AdPack(int id)
            : base(id)
        {
        }

        public AdPack(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        {
        }

        #endregion Constructors

        public void AddConstantImpression()
        {
            bool isUpToDate = IsUpToDate;

            //Work
            TotalConstantBannerImpressions++;

            SavePartially(isUpToDate, new PropertyBuilder<AdPack>()
                .Append(x => x.TotalConstantBannerImpressions)
                .Build());
        }

        public void AddNormalImpression()
        {
            bool isUpToDate = IsUpToDate;

            //Work
            TotalNormalBannerImpressions++;

            SavePartially(isUpToDate, new PropertyBuilder<AdPack>()
                .Append(x => x.TotalNormalBannerImpressions)
                .Build());
        }

        public void AddClick()
        {
            bool isUpToDate = IsUpToDate;

            //Work
            TotalClicks++;

            SavePartially(isUpToDate, new PropertyBuilder<AdPack>()
                .Append(x => x.TotalClicks)
                .Build());
        }

        public int GetNumberOfConstantImpressionsLeft()
        {
            return ConstantBannerImpressionsBought - TotalConstantBannerImpressions;
        }

        public int GetNumberOfNotmalImpressionsLeft()
        {
            return NormalBannerImpressionsBought - TotalNormalBannerImpressions;
        }

        public int GetNumberOfClicksLeft()
        {
            return ClicksBought - TotalClicks;
        }
        
    }
}