using System;
using System.Data;

namespace Prem.PTC.Advertising
{
    [Serializable]
    public class BannerAdvertGeolocation : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "BannerAdvertGeolocations"; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "BannerAdvertGeolocationId";
            public const string AdvertId = "BannerAdvertId";
            public const string BannedCounty = "BannedCountry";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AdvertId)]
        protected int AdvertId { get { return _advertId; } set { _advertId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.BannedCounty)]
        public string BannedCountry { get { return _bannedCountry; } set { _bannedCountry = value; SetUpToDateAsFalse(); } }

        private int _id, _advertId;
        private string _bannedCountry;

        #endregion


        #region Constructors

        public BannerAdvertGeolocation(int advertId, string bannedCountry) : base()
        {
            AdvertId = advertId;
            BannedCountry = bannedCountry;
        }
        public BannerAdvertGeolocation() : base() { }
        public BannerAdvertGeolocation(int id) : base() { }
        public BannerAdvertGeolocation(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion
    }
}