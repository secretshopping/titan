using System.Data;

namespace Prem.PTC.Advertising
{
    public class PtcAdvertGeolocation : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return AppSettings.TableNames.PtcAdvertGeolocations; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "PtcAdvertGeolocationId";
            public const string AdvertId = "AdvertId";
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

        public PtcAdvertGeolocation(int advertId, string bannedCountry)
        {
            AdvertId = advertId;
            BannedCountry = bannedCountry;
        }
        public PtcAdvertGeolocation() : base() { }
        public PtcAdvertGeolocation(int id) : base() { }
        public PtcAdvertGeolocation(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion
    }
}