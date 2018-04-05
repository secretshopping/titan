using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using ExtensionMethods;

namespace Prem.PTC.Advertising
{
    [Serializable]
    public class PtcAdvert : Advert<IPtcAdvertPack>
    {
        #region Columns

        public static new string TableName { get { return AppSettings.TableNames.PtcAdverts; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "PtcAdvertId";
            public const string CategoryId = "PtcCategoryId";
            public const string AdvertPackId = "PtcAdvertPackId";
            public const string Title = "Title";
            public const string Description = "Description";
            public const string DisplayTime = "DisplayTimeSeconds";
            public const string ClickValue = "ClickValue";
            public const string DirectRefClickValue = "DirectReferralClickValue";
            public const string RentedRefClickValue = "RentedReferralClickValue";
            public const string MinMembershipId = "MinMembershipId";
            public const string HasBoldTitle = "HasBoldTitle";
            // niepotrzebna kolumna, zostawiłem na razie żeby się nie bawić w zmiany w bazie
            [Obsolete]
            public const string IsGeolocated = "IsGeolocated";
            public const string GeolocatedCC = "GeolocatedCC";
            public const string BackgroundColor = "BackgroundColor";
            public const string PtcAdvertDimensionsId = "PtcAdvertDimensionsId";
            public const string StartPageDate = "StartPageDate";
            public const string CaptchaQuestion = "CaptchaQuestion";
            public const string CaptchaYesAnswers = "CaptchaYesAnswers";
            public const string CaptchaNoAnswers = "CaptchaNoAnswers";

            public const string GeolocatedAgeMin = "GeolocatedAgeMin";
            public const string GeolocatedAgeMax = "GeolocatedAgeMax";
            public const string GeolocatedGender = "GeolocatedGender";
            public const string PointsEarnedFromViews = "PointsEarnedFromViews";
            public const string ExtraViews = "ExtraViews";
            public const string PackPrice = "PackPrice";
            public const string ImagePath = "ImagePath";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        // Zostawiam w celu kompatybilności jeśli gdziekolwiek ktoś tego używa
        [Column(Columns.CategoryId)]
        public int CategoryId { get { return _categoryId; } set { _categoryId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AdvertPackId)]
        protected override int? AdvertPackId { get { return _advertPackId; } set { _advertPackId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Title)]
        public string Title { get { return _title; } set { _title = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Description)]
        public string Description { get { return _description; } set { _description = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DisplayTime)]
        protected int DisplayTimeSeconds { get { return _displayTimeSeconds; } set { _displayTimeSeconds = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// Default value 100
        /// </summary>
        [Column(Columns.ClickValue)]
        public int ClickValue { get { return _clickValue; } set { _clickValue = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// Default value 100
        /// </summary>
        [Column(Columns.DirectRefClickValue)]
        public int DirectReferralClickValue { get { return _directReferralClickValue; } set { _directReferralClickValue = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// Default value 100
        /// </summary>
        [Column(Columns.RentedRefClickValue)]
        public int RentedReferralClickValue { get { return _rentedReferralClickValue; } set { _rentedReferralClickValue = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MinMembershipId)]
        public int MinMembershipId { get { return _minMembershipId; } set { _minMembershipId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.HasBoldTitle)]
        public bool HasBoldTitle { get { return _hasBoldTitle; } set { _hasBoldTitle = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsGeolocated)]
        public bool IsGeolocated
        {
            get
            {
                if (String.IsNullOrWhiteSpace(GeolocatedCC)
                    && GeolocatedAgeMin <= 0 && GeolocatedAgeMax <= 0 && GeolocatedGender == Gender.Null)
                    return false;
                return true;
            }
            set { _isGeolocated = value; SetUpToDateAsFalse(); }
        }

        [Column(Columns.GeolocatedCC)]
        public string GeolocatedCC { get { return _GeolocatedCC; } set { _GeolocatedCC = value; SetUpToDateAsFalse(); } }

        [Column(Columns.BackgroundColor)]
        public string BackgroundColor { get { return _BackgroundColor; } set { _BackgroundColor = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PtcAdvertDimensionsId)]
        public int PtcAdvertDimensionsId { get { return _PtcAdvertDimensionsId; } set { _PtcAdvertDimensionsId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.StartPageDate)]
        public DateTime? StartPageDate { get { return _StartPageDate; } set { _StartPageDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CaptchaQuestion)]
        public string CaptchaQuestion { get { return _CaptchaQuestion; } set { _CaptchaQuestion = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CaptchaYesAnswers)]
        public int CaptchaYesAnswers { get { return _CaptchaYesAnswers; } set { _CaptchaYesAnswers = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CaptchaNoAnswers)]
        public int CaptchaNoAnswers { get { return _CaptchaNoAnswers; } set { _CaptchaNoAnswers = value; SetUpToDateAsFalse(); } }

        [Column(Columns.GeolocatedAgeMin)]
        public int GeolocatedAgeMin { get { return _GeolocatedAgeMin; } set { _GeolocatedAgeMin = value; SetUpToDateAsFalse(); } }

        [Column(Columns.GeolocatedAgeMax)]
        public int GeolocatedAgeMax { get { return _GeolocatedAgeMax; } set { _GeolocatedAgeMax = value; SetUpToDateAsFalse(); } }

        [Column(Columns.GeolocatedGender)]
        protected int GeolocatedGenderInt { get { return _Gender; } set { _Gender = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PointsEarnedFromViews)]
        public int PointsEarnedFromViews { get { return _PointsEarnedFromViews; } set { _PointsEarnedFromViews = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ImagePath)]
        public string ImagePath { get { return _imagePath; } set { _imagePath = value;  SetUpToDateAsFalse(); } }

        public Gender GeolocatedGender
        {
            get { return (Gender)GeolocatedGenderInt; }
            set { GeolocatedGenderInt = (int)value; }
        }

        [Column("IsStarredAd")]
        public bool IsStarredAd { get { return _IsStarredAd; } set { _IsStarredAd = value; SetUpToDateAsFalse(); } }

        [Column("AdvertiserUserId")]
        public int AdvertiserUserId { get { return _AdvertiserUserId; } set { _AdvertiserUserId = value; SetUpToDateAsFalse(); } }

        [Column("ExtraViews")]
        public int ExtraViews { get { return _ExtraViews; } set { _ExtraViews = value; SetUpToDateAsFalse(); } }

        [Column("PackPrice")]
        public Money PackPrice { get { return _PackPrice; } set { _PackPrice = value; SetUpToDateAsFalse(); } }

        [Column("IsEdited")]
        public bool IsEdited { get { return _IsEdited; } set { _IsEdited = value; SetUpToDateAsFalse(); } }       

        private int _id, _categoryId, _displayTimeSeconds, _clickValue, _directReferralClickValue, _rentedReferralClickValue, _minMembershipId, _PtcAdvertDimensionsId,
            _CaptchaYesAnswers, _CaptchaNoAnswers, _Gender, _GeolocatedAgeMax, _GeolocatedAgeMin, _AdvertiserUserId, _PointsEarnedFromViews, _ExtraViews;
        private int? _advertPackId;
        private string _title, _description, _GeolocatedCC, _BackgroundColor, _CaptchaQuestion, _imagePath;
        private bool _isGeolocated, _hasBoldTitle, _IsStarredAd, _IsEdited;
        private DateTime? _StartPageDate;
        private Money _PackPrice;

        #endregion

        #region GEOLOCATION

        public bool IsGeolocatedByCountry
        {
            get
            {
                if (String.IsNullOrWhiteSpace(GeolocatedCC))
                    return false;
                return true;
            }
        }

        public bool IsGeolocatedByGender
        {
            get
            {
                if (GeolocatedGender == Gender.Null)
                    return false;
                return true;
            }
        }

        public bool IsGeolocatedByAge
        {
            get
            {
                if (GeolocatedAgeMax == 0 && GeolocatedAgeMin == 0)
                    return false;
                return true;
            }
        }

        #endregion

        public TimeSpan DisplayTime
        {
            get { return TimeSpan.FromSeconds(DisplayTimeSeconds); }
            set { DisplayTimeSeconds = (int)value.TotalSeconds; }
        }

        /// <summary>
        /// Sets all properties of pack except from Price
        /// </summary>
        public override IPtcAdvertPack Pack
        {
            get { return AdvertPackId != null ? new PtcAdvertPackIdProxy(AdvertPackId.Value) : null; }
            set
            {
                AdvertPackId = value.Id;
                Ends = value.Ends;
                DisplayTime = value.DisplayTime;
            }
        }

        public AdExposure ExposureType
        {
            get
            {
                //Here you can modify the exposure settings

                if (DisplayTimeSeconds <= 5)
                    return AdExposure.Micro;

                if (DisplayTimeSeconds <= 10)
                    return AdExposure.Mini;

                if (DisplayTimeSeconds <= 15)
                    return AdExposure.Fixed;

                if (DisplayTimeSeconds <= 30)
                    return AdExposure.Standard;

                return AdExposure.Extended;
            }
        }


        #region Constructors

        public PtcAdvert()
            : base()
        {
            ClickValue = DirectReferralClickValue = RentedReferralClickValue = 100;
        }
        public PtcAdvert(int id)
            : base(id)
        {
        }
        public PtcAdvert(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        {
            if (!row.Table.Columns.Contains(ElName(() => ClickValue))) ClickValue = 100;
            if (!row.Table.Columns.Contains(ElName(() => DirectReferralClickValue))) DirectReferralClickValue = 100;
            if (!row.Table.Columns.Contains(ElName(() => RentedReferralClickValue))) RentedReferralClickValue = 100;

        }


        #endregion


        public override void Prolong(IPtcAdvertPack more, Money prolongationCost)
        {
            base.Prolong(more, prolongationCost);
            DisplayTime = more.DisplayTime;
        }

        public override void Prolong(IPtcAdvertPack more)
        {
            base.Prolong(more);
            DisplayTime = more.DisplayTime;
        }

        public Money MoneyToClaimAsCashLink
        {
            get
            {
                try
                {
                    //Pack.Ends is always clicks for Cash Links
                    Decimal PaidPerEachClick = this.Price.ToDecimal() / Convert.ToDecimal(this.Ends.Value);

                    //50/50
                    return new Money(PaidPerEachClick / (Decimal)2);
                }
                catch (Exception ex)
                {
                    return new Money(0);
                }
            }
        }

        public bool IsOKWithGeolocation(Member user)
        {
            if (!IsGeolocated)
                return true;

            if (!String.IsNullOrWhiteSpace(GeolocatedCC) && !GeolocatedCC.Contains(user.CountryCode.Trim()))
                return false;

            if (GeolocatedAgeMin > 0 && user.Age < GeolocatedAgeMin)
                return false;

            if (GeolocatedAgeMax > 0 && user.Age > GeolocatedAgeMax)
                return false;

            if (GeolocatedGender != Gender.Null && GeolocatedGender != user.Gender)
                return false;

            return true;
        }


        /// <summary>
        /// Saves advert's content including geolocation
        /// </summary>
        /// <exception cref="DbException" />
        public override void Save(bool forceSave = false)
        {
            base.Save(forceSave);
        }

        public override void Click()
        {
            TableHelper.ExecuteRawCommandNonQuery("EXEC AddClickToPtcAdvert @AdvertId = " + Id);
        }

        public void ClickWithFeedbackCaptcha(int yes, int no)
        {
            TableHelper.ExecuteRawCommandNonQuery("EXEC AddClickToPtcAdvertWithFeedback @AdvertId = " + Id + ", @yes = " + yes + ", @no = " + no);
        }

        /// <exception cref="DbException" />
        public static List<PtcAdvert> SelectAdvertsWith(AdvertStatus status)
        {
            var whereStatus = TableHelper.MakeDictionary(Advert.Columns.Status, (int)status);

            return TableHelper.SelectRows<PtcAdvert>(whereStatus);
        }

        /// <param name="title">Name of PtcAdvert</param>
        /// <exception cref="DbException" />
        public static bool Exists(string title)
        {
            return TableHelper.RowExists(AppSettings.TableNames.PtcAdverts, PtcAdvert.Columns.Title, title);
        }

        /// <param name="title">Name of PtcAdvert</param>
        /// <exception cref="DbException" />
        public static void Delete(string title)
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                var advertIdCol = Parser.Columns(PtcAdvert.Columns.Id);
                var whereName = TableHelper.MakeDictionary(PtcAdvert.Columns.Title, title);
                var ptcAdvertIds = bridge.Instance.Select(PtcAdvert.TableName, advertIdCol, whereName);

                // delete ptc adverts
                bridge.Instance.Delete(PtcAdvert.TableName, whereName);
            }

        }

        /// <param name="id">id of PtcAdvert</param>
        /// <exception cref="DbException" />
        public static void Delete(int id)
        {
            TableHelper.DeleteRows<PtcAdvert>(PtcAdvert.Columns.Id, id);
        }

        /// <param name="id">id of PtcAdvert</param>
        /// <exception cref="DbException" />
        public static void Update(Dictionary<string, object> what, int id)
        {
            Update(what, TableHelper.MakeDictionary(PtcAdvert.Columns.Id, id));
        }

        /// <exception cref="DbException" />
        public static void Update(Dictionary<string, object> what, Dictionary<string, object> where)
        {
            TableHelper.UpdateRows(PtcAdvert.TableName, what, where);
        }

        /// <summary>
        /// Returns ads which user should be able to view (active, egnible for membership AND GEOLOCATION)
        /// </summary>
        /// <param name="MembershipId"></param>
        /// <returns></returns>
        public static List<PtcAdvert> GetActiveAdsForUser(Member User)
        {
            List<PtcAdvert> adslist = GetAllActiveAds();
            IEnumerable<int> notAllowedMinMembershipIds;

            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                var what = Parser.Columns(Membership.Columns.Id, Membership.Columns.DisplayOrder);
                var datatable = bridge.Instance.Select(Membership.TableName, what, null);

                int DisplayOrder = (from DataRow row in datatable.Rows
                                    where row.Field<int>(Membership.Columns.Id) == User.Membership.Id
                                    select row.Field<int>(Membership.Columns.DisplayOrder)).FirstOrDefault();

                notAllowedMinMembershipIds = from DataRow row in datatable.Rows
                                             where row.Field<int>(Membership.Columns.DisplayOrder) > DisplayOrder
                                             select row.Field<int>(Membership.Columns.Id);
            }

            return (from elem in adslist
                    where !notAllowedMinMembershipIds.Contains(elem.MinMembershipId) && elem.IsOKWithGeolocation(User)
                    select elem).ToList();
        }

        public static List<PtcAdvert> GetUnwatchedAdsForUser(Member user)
        {
            var activeAds = GetActiveAdsForUser(user);

            List<PtcAdvert> unwatchedAds = new List<PtcAdvert>();

            if ((AppSettings.Points.LevelMembershipPolicyEnabled && user.Membership.AutosurfViewLimitMonth > user.PtcAutoSurfClicksThisMonth) 
                || !AppSettings.Points.LevelMembershipPolicyEnabled)
            {
                unwatchedAds = activeAds.Where(a => !user.AdsViewed.Contains(a.Id)).ToList();
            }

            return unwatchedAds;
        }

        public static List<PtcAdvert> GetActiveCashLinksForUser(Member User)
        {
            List<PtcAdvert> adslist = GetAllActiveAds();

            return (from elem in adslist
                    where (elem.IsOKWithGeolocation(User))
                    select elem).ToList();
        }


        /// <summary>
        /// Returns all active ads
        /// </summary>
        /// <returns></returns>
        public static List<PtcAdvert> GetAllActiveAds()
        {
            var cache = new PtcAdvertCache();
            return (List<PtcAdvert>)cache.Get();
        }

        public static int GetUserActiveCampaignsCount(int userId)
        {
            return (int)TableHelper.SelectScalar("SELECT COUNT(*) FROM PtcAdverts WHERE Status = " + (int)AdvertStatus.Active +
                    " AND AdvertiserUserId = " + userId);
        }

        private static Money calculateEarnings(Money earnings, int clickvalue)
        {
            if (clickvalue > 100)
                return earnings;

            return Money.MultiplyPercent(earnings, clickvalue);
        }

        private static Money calculateExtendedEarnings(Money earnings, int clickvalue, int DisplayTime, bool ForcePositiveValue = false)
        {
            if (clickvalue > 100)
                return earnings;

            //Now we nned to calculate second multiplier

            Decimal percent = (Decimal)DisplayTime / (Decimal)30;
            Decimal.Round(percent, 4, MidpointRounding.ToEven);

            Money temp = Money.MultiplyPercent(earnings, clickvalue);
            Money temp2 = new Money(percent);

            Money result = temp * temp2;

            if (ForcePositiveValue && result == new Money(0))
            {
                return Money.MinPositiveValue;
            }
            return result;
        }

        //For normal mode

        public static Money CalculateNormalMemberEarnings(Member User, PtcAdvert Ad)
        {
            return CalculateNormalMemberEarnings(User.Membership, Ad);
        }

        public static Money CalculateEarningsFromRentedReferral(Member User, PtcAdvert Ad)
        {
            return CalculateEarningsFromRentedReferral(User.Membership, Ad);
        }

        public static Money CalculateEarningsFromDirectReferral(Member User, PtcAdvert Ad)
        {
            return CalculateEarningsFromDirectReferral(User.Membership, Ad);
        }

        public static Money CalculateEarningsFromDirectReferralTE(Member User, PtcAdvert Ad)
        {
            return CalculateEarningsFromDirectReferralTE(User.Membership, Ad);
        }

        public static Money CalculateNormalMemberEarningsTE(Member User, PtcAdvert Ad)
        {
            return CalculateNormalMemberEarningsTE(User.Membership, Ad);
        }

        public static Money CalculateNormalMemberEarnings(IMembership UserMembership, PtcAdvert Ad)
        {
            if (AppSettings.PtcAdverts.ExposureCategoriesEnabled)
                return CalculateExposureNormalEarnings(UserMembership, Ad);


            return calculateEarnings(UserMembership.AdvertClickEarnings, Ad.ClickValue);
        }

        private static Money CalculateExposureNormalEarnings(IMembership userMembership, PtcAdvert ad)
        {
            var exposureEarnings = Money.Zero;

            switch (ad.ExposureType)
            {
                case AdExposure.Mini:
                    exposureEarnings = userMembership.ExposureMiniClickEarnings;
                    break;
                case AdExposure.Micro:
                    exposureEarnings = userMembership.ExposureMicroClickEarnings;
                    break;
                case AdExposure.Fixed:
                    exposureEarnings = userMembership.ExposureFixedClickEarnings;
                    break;
                case AdExposure.Standard:
                    exposureEarnings = userMembership.ExposureStandardClickEarnings;
                    break;
                case AdExposure.Extended:
                    exposureEarnings = userMembership.ExposureExtendedClickEarnings;
                    break;
                default:
                    break;
            }
            exposureEarnings = Money.MultiplyPercent(exposureEarnings, ad.ClickValue);
            return exposureEarnings;
        }

        public static Money CalculateEarningsFromRentedReferral(IMembership UserMembership, PtcAdvert Ad)
        { 
            if (AppSettings.PtcAdverts.ExposureCategoriesEnabled)
                return CalculateExposureRentedEarnings(UserMembership, Ad);

            return calculateEarnings(UserMembership.RentedReferralAdvertClickEarnings, Ad.RentedReferralClickValue);
        }

        private static Money CalculateExposureRentedEarnings(IMembership userMembership, PtcAdvert ad)
        {
            var exposureEarnings = Money.Zero;

            switch (ad.ExposureType)
            {
                case AdExposure.Mini:
                    exposureEarnings =  userMembership.ExposureMiniRentedClickEarnings;
                    break;
                case AdExposure.Micro:
                    exposureEarnings =  userMembership.ExposureMicroRentedClickEarnings;
                    break;
                case AdExposure.Fixed:
                    exposureEarnings =  userMembership.ExposureFixedRentedClickEarnings;
                    break;
                case AdExposure.Standard:
                    exposureEarnings =  userMembership.ExposureStandardRentedClickEarnings;
                    break;
                case AdExposure.Extended:
                    exposureEarnings =  userMembership.ExposureExtendedRentedClickEarnings;
                    break;
                default:
                    break;
            }

            exposureEarnings = Money.MultiplyPercent(exposureEarnings, ad.RentedReferralClickValue);
            return exposureEarnings;
        }

        public static Money CalculateEarningsFromDirectReferral(IMembership UserMembership, PtcAdvert Ad)
        {
            if (AppSettings.PtcAdverts.ExposureCategoriesEnabled)
                return CalculateExposureDirectEarnings(UserMembership, Ad);

            return calculateEarnings(UserMembership.DirectReferralAdvertClickEarnings, Ad.DirectReferralClickValue);
        }

        private static Money CalculateExposureDirectEarnings(IMembership userMembership, PtcAdvert ad)
        {
            var exposureEarnings = Money.Zero;

            switch (ad.ExposureType)
            {
                case AdExposure.Mini:
                    exposureEarnings =  userMembership.ExposureMiniDirectClickEarnings;
                    break;
                case AdExposure.Micro:
                    exposureEarnings =  userMembership.ExposureMicroDirectClickEarnings;
                    break;
                case AdExposure.Fixed:
                    exposureEarnings =  userMembership.ExposureFixedDirectClickEarnings;
                    break;
                case AdExposure.Standard:
                    exposureEarnings =  userMembership.ExposureStandardDirectClickEarnings;
                    break;
                case AdExposure.Extended:
                    exposureEarnings =  userMembership.ExposureExtendedDirectClickEarnings;
                    break;
                default:
                   break;
            }

            exposureEarnings = Money.MultiplyPercent(exposureEarnings, ad.DirectReferralClickValue);
            return exposureEarnings;
        }

        public static Money CalculateEarningsFromDirectReferralTE(IMembership UserMembership, PtcAdvert Ad)
        {
            //For traffic exchange
            return calculateExtendedEarnings(UserMembership.DirectReferralAdvertClickEarnings, Ad.DirectReferralClickValue, Ad.DisplayTimeSeconds);
        }

        public static Money CalculateNormalMemberEarningsTE(IMembership UserMembership, PtcAdvert Ad)
        {
            //For traffic exchange
            return calculateExtendedEarnings(UserMembership.AdvertClickEarnings, Ad.ClickValue, Ad.DisplayTimeSeconds);
        }

        public static int GetNumberOfUsersWithinBalanceRange(PtcAdvertPack pack)
        {
            var numberOfUsers = TableHelper.SelectScalar(string.Format("SELECT COUNT(*) FROM Users WHERE Balance3 >= {0} AND Balance3 <= {1}",
                pack.MinUserBalance.ToClearString(), pack.MaxUserBalance.ToClearString()));
            return Convert.ToInt32(numberOfUsers);
        }

        public static int GetNumberOfStartPagesPurchasedForDay(DateTime date)
        {
            try
            {
                var ads = TableHelper.SelectScalar(string.Format(@"SELECT COUNT(*) FROM PtcAdverts
                                                                   WHERE StartPageDate is not null
                                                                   AND CAST(floor(cast(StartPageDate as float)) as datetime) = '{0}'", date.ToShortDateDBString()));

                return Convert.ToInt32(ads);
            }
            catch (Exception ex)
            {
                return 1000;
            }
        }
    }
}