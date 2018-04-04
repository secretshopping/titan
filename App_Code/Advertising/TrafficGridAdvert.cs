using System;
using System.Collections.Generic;
using System.Data;

namespace Prem.PTC.Advertising
{
    [Serializable]
    public class TrafficGridAdvert : Advert<ITrafficGridAdvertPack>
    {
        #region Columns

        public static new string TableName { get { return "TrafficGridAdverts"; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "PtcAdvertId";
            // categories are not used in present but might be used in future
            [Obsolete]
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
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        // Zostawiam w celu kompatybilności jeśli gdziekolwiek ktoś tego używa
        [Column(Columns.CategoryId)]
        public int CategoryId { get { return -1; } set { /*_categoryId = value;*/ SetUpToDateAsFalse(); } }

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
        public bool IsGeolocated { get { return false; } set { } }

        private int _id, _categoryId, _displayTimeSeconds, _clickValue, _directReferralClickValue, _rentedReferralClickValue, _minMembershipId;
        private int? _advertPackId;
        private string _title, _description;
        private bool _isGeolocated, _hasBoldTitle;

        #endregion


        public TimeSpan DisplayTime
        {
            get { return TimeSpan.FromSeconds(DisplayTimeSeconds); }
            set { DisplayTimeSeconds = (int)value.TotalSeconds; }
        }

        /// <summary>
        /// Sets all properties of pack except from Price
        /// </summary>
        public override ITrafficGridAdvertPack Pack
        {
            get { return AdvertPackId != null ? new TrafficGridAdvertPackIdProxy(AdvertPackId.Value) : null; }
            set
            {
                AdvertPackId = value.Id;
                Ends = value.Ends;
                DisplayTime = value.DisplayTime;
            }
        }


        #region Constructors

        public TrafficGridAdvert()
            : base()
        {

            ClickValue = DirectReferralClickValue = RentedReferralClickValue = 100;
        }
        public TrafficGridAdvert(int id)
            : base(id)
        {

        }
        public TrafficGridAdvert(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        {
            if (!row.Table.Columns.Contains(ElName(() => ClickValue))) ClickValue = 100;
            if (!row.Table.Columns.Contains(ElName(() => DirectReferralClickValue))) DirectReferralClickValue = 100;
            if (!row.Table.Columns.Contains(ElName(() => RentedReferralClickValue))) RentedReferralClickValue = 100;

        }


        #endregion

        public override bool IsUpToDate
        {
            get
            {
                return base.IsUpToDate;
            }
        }

        public override void Prolong(ITrafficGridAdvertPack more, Money prolongationCost)
        {
            base.Prolong(more, prolongationCost);
            DisplayTime = more.DisplayTime;
        }

        public override void Prolong(ITrafficGridAdvertPack more)
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

            //Increase global statistics
            //var stat = new Prem.PTC.Statistics.Statistics(Prem.PTC.Statistics.StatisticsType.PTCClicks);
            //stat.AddToData1(1);
            //stat.Save();
        }

        /// <summary>
        /// Deletes advert's content including geolocation
        /// </summary>
        public override void Delete()
        {
            base.Delete();
        }

        /// <exception cref="DbException" />
        public static List<TrafficGridAdvert> SelectAdvertsWith(AdvertStatus status)
        {
            var whereStatus = TableHelper.MakeDictionary(Advert.Columns.Status, (int)status);

            return TableHelper.SelectRows<TrafficGridAdvert>(whereStatus);
        }

        /// <param name="title">Name of TrafficGridAdvert</param>
        /// <exception cref="DbException" />
        public static bool Exists(string title)
        {
            return TableHelper.RowExists(TrafficGridAdvert.TableName, TrafficGridAdvert.Columns.Title, title);
        }

        /// <param name="title">Name of TrafficGridAdvert</param>
        /// <exception cref="DbException" />
        public static void Delete(string title)
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                var advertIdCol = Parser.Columns(TrafficGridAdvert.Columns.Id);
                var whereName = TableHelper.MakeDictionary(TrafficGridAdvert.Columns.Title, title);
                var ptcAdvertIds = bridge.Instance.Select(TrafficGridAdvert.TableName, advertIdCol, whereName);

                // delete ptc adverts
                bridge.Instance.Delete(TrafficGridAdvert.TableName, whereName);
            }

        }

        /// <param name="id">id of TrafficGridAdvert</param>
        /// <exception cref="DbException" />
        public static void Delete(int id)
        {
            TableHelper.DeleteRows<TrafficGridAdvert>(TrafficGridAdvert.Columns.Id, id);
        }

        /// <param name="id">id of TrafficGridAdvert</param>
        /// <exception cref="DbException" />
        public static void Update(Dictionary<string, object> what, int id)
        {
            Update(what, TableHelper.MakeDictionary(TrafficGridAdvert.Columns.Id, id));
        }

        /// <exception cref="DbException" />
        public static void Update(Dictionary<string, object> what, Dictionary<string, object> where)
        {
            TableHelper.UpdateRows(TrafficGridAdvert.TableName, what, where);
        }

        /// <summary>
        /// Returns all active ads
        /// </summary>
        /// <returns></returns>
        public static List<TrafficGridAdvert> GetAllActiveAds()
        {
            List<TrafficGridAdvert> adslist;
            adslist = TableHelper.SelectRows<TrafficGridAdvert>(TableHelper.MakeDictionary(Advert.Columns.Status, (int)AdvertStatus.Active));

            return adslist;
        }
    }
}