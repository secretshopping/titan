using System;
using System.Collections.Generic;
using System.Data;
using Prem.PTC.Members;

namespace Prem.PTC.Advertising
{
    [Serializable]
    public class TrafficExchangeAdvert : Advert<IPtcAdvertPack>
    {
        #region Columns
        public static new string TableName { get { return "TrafficExchangeAdverts"; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "TrafficExchangeAdvertId";
            public const string AdvertPackId = "TrafficExchangeAdvertPackId";
            public const string Title = "Title";
            public const string Description = "Description";
            public const string DisplayTime = "DisplayTimeSeconds";
            public const string MinMembershipId = "MinMembershipId"; 
            public const string HasBoldTitle = "HasBoldTitle";
            public const string TimeBetweenViewsInMinutes = "TimeBetweenViewsInMinutes";

        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AdvertPackId)]
        protected override int? AdvertPackId { get { return _advertPackId; } set { _advertPackId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Title)]
        public string Title { get { return _title; } set { _title = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Description)]
        public string Description { get { return _description; } set { _description = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DisplayTime)]
        protected int DisplayTimeSeconds { get { return _displayTimeSeconds; } set { _displayTimeSeconds = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TimeBetweenViewsInMinutes)]
        public int TimeBetweenViewsInMinutes { get { return _TimeBetweenViewsInMinutes; } set { _TimeBetweenViewsInMinutes = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MinMembershipId)]
        public int MinMembershipId { get { return _minMembershipId; } set { _minMembershipId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.HasBoldTitle)]
        public bool HasBoldTitle { get { return _hasBoldTitle; } set { _hasBoldTitle = value; SetUpToDateAsFalse(); } }

        [Column("BannedCountries")]
        public string GeolocatedCountries { get { return _BannedCountries; } set { _BannedCountries = value; SetUpToDateAsFalse(); } }

        public bool IsGeolocated { get { return !String.IsNullOrEmpty(GeolocatedCountries); } protected set { _isGeolocated = value; SetUpToDateAsFalse(); } }

        private int _id, _categoryId, _displayTimeSeconds, _minMembershipId, _TimeBetweenViewsInMinutes;
        private int? _advertPackId;
        private string _title, _description, _BannedCountries;
        private bool _isGeolocated, _hasBoldTitle;
        #endregion

        #region Geolocation
        public static readonly char Delimiter = ','; //Default delimiter for country codes & cities

        public List<string> GetGeolocatedCountries()
        {
            List<string> countriesList = new List<string>();
            if (string.IsNullOrWhiteSpace(GeolocatedCountries))
                return countriesList;

            var countries = GeolocatedCountries.Split(Delimiter);

            foreach (var cc in countries)
                countriesList.Add(CountryManager.GetCountryName(cc));

            return countriesList;
        }

        public bool AreGeolocatedCountriesTheSame(TrafficExchangeAdvert compare)
        {
            if (this.GeolocatedCountries == compare.GeolocatedCountries)
                return true;
            return false;
        }

        public void ClearCountryGeolocation()
        {
            GeolocatedCountries = "";
        }

        public void AddGeolocation(string country)
        {
            if (string.IsNullOrWhiteSpace(GeolocatedCountries))
                GeolocatedCountries += CountryManager.GetCountryCode(country);
            else
                GeolocatedCountries += Delimiter + CountryManager.GetCountryCode(country);
        }

        public void RemoveGeolocation(string country)
        {
            var currentlyGeolocatedCountries = GetGeolocatedCountries();
            var countryCodeToRemove = CountryManager.GetCountryCode(country);

            if (currentlyGeolocatedCountries.Contains(countryCodeToRemove))
            {
                currentlyGeolocatedCountries.Remove(countryCodeToRemove);
                ClearCountryGeolocation();
                foreach (var elem in currentlyGeolocatedCountries)
                {
                    AddGeolocation(CountryManager.GetCountryCode(elem));
                }
            }
        }

        public void AddMultipleGeolocations(List<string> countries)
        {
            ClearCountryGeolocation();
            foreach (var country in countries)
            {
                AddGeolocation(country);
            }
        }

        public bool IsGeolocatedCountry
        {
            get
            {
                if (String.IsNullOrWhiteSpace(GeolocatedCountries))
                    return false;
                return true;
            }
        }
        #endregion Geolocation

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

                if (DisplayTimeSeconds <= 15)
                    return AdExposure.Mini;

                if (DisplayTimeSeconds <= 30)
                    return AdExposure.Standard;

                return AdExposure.Extended;
            }
        }

        #region Constructors
        public TrafficExchangeAdvert() : base() { TargetBalance = PurchaseBalances.Traffic; }
        public TrafficExchangeAdvert(int id) : base(id) { }
        public TrafficExchangeAdvert(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }
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

        /// <summary>
        /// Saves advert's content including geolocation
        /// </summary>
        /// <exception cref="DbException" />
        public override void Save(bool forceSave = false)
        {
            base.Save(forceSave);
        }

        /// <summary>
        /// Reloads advert's content including geolocation
        /// </summary>
        public override void Reload()
        {
            base.Reload();
        }

        public override void Click()
        {
            base.Click();
        }

        /// <summary>
        /// Deletes advert's content including geolocation
        /// </summary>
        public override void Delete()
        {
            base.Delete();
        }

        /// <exception cref="DbException" />
        public static List<TrafficExchangeAdvert> SelectAdvertsWith(AdvertStatus status)
        {
            var whereStatus = TableHelper.MakeDictionary(Advert.Columns.Status, (int)status);
            return TableHelper.SelectRows<TrafficExchangeAdvert>(whereStatus);
        }

        /// <param name="id">id of PtcAdvert</param>
        /// <exception cref="DbException" />
        public static void Delete(int id)
        {
            TableHelper.DeleteRows<TrafficExchangeAdvert>(TrafficExchangeAdvert.Columns.Id, id);
        }

        /// <param name="id">id of PtcAdvert</param>
        /// <exception cref="DbException" />
        public static void Update(Dictionary<string, object> what, int id)
        {
            Update(what, TableHelper.MakeDictionary(TrafficExchangeAdvert.Columns.Id, id));
        }

        /// <exception cref="DbException" />
        public static void Update(Dictionary<string, object> what, Dictionary<string, object> where)
        {
            TableHelper.UpdateRows(TrafficExchangeAdvert.TableName, what, where);
        }

        private static Money calculateExtendedEarnings(Money earnings, int DisplayTime)
        {
            //Now we nned to calculate second multiplier

            decimal percent = (decimal)DisplayTime / (decimal)30;
            decimal.Round(percent, 4, MidpointRounding.ToEven);

            Money temp = new Money(percent);

            Money result = earnings * temp;
            return result;
        }

        public static Money CalculateEarningsFromDirectReferralTE(Member User, TrafficExchangeAdvert Ad)
        {
            if (AppSettings.TrafficExchange.CreditBasedOnDurationEnabled)
                return calculateExtendedEarnings(User.Membership.DRTrafficExchangeClickEarnings, Ad.DisplayTimeSeconds);
            else
                return User.Membership.DRTrafficExchangeClickEarnings;
        }

        public static Money CalculateNormalMemberEarningsTE(Member User, TrafficExchangeAdvert Ad)
        {
            if (AppSettings.TrafficExchange.CreditBasedOnDurationEnabled)
                return calculateExtendedEarnings(User.Membership.TrafficExchangeClickEarnings, Ad.DisplayTimeSeconds);
            else
                return User.Membership.TrafficExchangeClickEarnings;
        }
    }
}