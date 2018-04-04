using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Prem.PTC.Members
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class VMemberGeolocation : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "VMemberGeolocation"; } }
        protected override string dbTable { get { return TableName; } }

        #region Columns

        public static class Columns
        {
            public const string Id = "UserId";
            public const string Username = "Username";
            public const string AvatarUrl = "AvatarUrl";
            public const string Country = "Country";
            public const string CountryCode = "CountryCode";
            public const string RegisterDate = "RegisterDate";
            public const string RegisteredLatitude = "RegisteredLatitude";
            public const string RegisteredLongitude = "RegisteredLongitude";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Username)]
        public string Username { get { return _username; } set { _username = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AvatarUrl)]
        public string AvatarUrl { get { return _avatarUrl; } set { _avatarUrl = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Country)]
        public string Country { get { return _country; } set { _country = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CountryCode)]
        public string CountryCode { get { return _countryCode; } set { _countryCode = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RegisterDate)]
        public DateTime RegisterDate { get { return _registerDate; } set { _registerDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RegisteredLatitude)]
        public Decimal? RegisteredLatitude { get { return _RegisteredLatitude; } set { _RegisteredLatitude = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RegisteredLongitude)]
        public Decimal? RegisteredLongitude { get { return _RegisteredLongitude; } set { _RegisteredLongitude = value; SetUpToDateAsFalse(); } }
        
        private int _id;
        private string _avatarUrl, _country, _countryCode, _username;
        private DateTime _registerDate;
        private Decimal? _RegisteredLatitude, _RegisteredLongitude;

        #endregion

        #region Constructors

        public VMemberGeolocation()
            : base()
        { }
        public VMemberGeolocation(int id)
            : base(id)
        { }
        public VMemberGeolocation(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        { }

        public VMemberGeolocation(string username) : this(Member.GetMemberId(username))
        {

        }

        #endregion Constructors

        #region GetList

        public static List<VMemberGeolocation> GetAll()
        {
            return TableHelper.SelectAllRows<VMemberGeolocation>();
        }

        public static List<VMemberGeolocation> GetRegisteredLast(int top = 50)
        {
            string query = string.Format("SELECT TOP {2} * FROM {0} WHERE {3} != 0 AND {3} IS NOT NULL ORDER BY {1} DESC",
                TableName, Columns.RegisterDate, top, Columns.RegisteredLatitude);

            return TableHelper.GetListFromRawQuery<VMemberGeolocation>(query);
        }

        public static List<VMemberGeolocation> GetRegisteredFromDate(DateTime from)
        {
            string where = String.Format("WHERE {0} >= TRY_CONVERT(DATETIME, '{1}', 102)", Columns.RegisterDate, from);

            return TableHelper.GetListFromQuery<VMemberGeolocation>(where);
        }

        public static List<VMemberGeolocation> GetRegisteredFromLastWeek()
        {
            return GetRegisteredFromDate(DateTime.Now.AddDays(-7));
        }

        #endregion

        #region GetMapJson

        private class VMemberGeolocationMapJson
        {
            [JsonProperty("latLng")]
            public string[] RegisteredCoordinates{ get; set; }

            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("avatar")]
            public string AvatarUrl { get; set; }

            [JsonProperty("flag")]
            public string Flag { get; set; }

            public VMemberGeolocationMapJson(VMemberGeolocation info)
            {
                RegisteredCoordinates = new string[]
                {
                    (info.RegisteredLatitude.HasValue ? info.RegisteredLatitude.Value : 0.0m).ToString(),
                    (info.RegisteredLongitude.HasValue ? info.RegisteredLongitude.Value : 0.0m).ToString()
                };
                Country = info.Country;
                Username = info.Username;



                AvatarUrl = info.AvatarUrl.StartsWith("http") ?
                    info.AvatarUrl :
                    info.AvatarUrl.Replace("~", AppSettings.Site.Url);

                Flag = String.Format("{0}/Images/Flags/{1}.png", AppSettings.Site.Url, info.CountryCode.ToLower());
            }
        }

        public string GetMapJson()
        {
            return GetMapJson(this);
        }

        public static string GetMapJson(VMemberGeolocation input)
        {
            VMemberGeolocationMapJson[] array = new VMemberGeolocationMapJson[]
            {
                new VMemberGeolocationMapJson(input)
            };

            return GetMapJson(array);
        }

        public static string GetMapJson(List<VMemberGeolocation> info)
        {
            List<VMemberGeolocationMapJson> list = info.Select(x => new VMemberGeolocationMapJson(x)).ToList();
            return GetMapJson(list.ToArray());
        }

        private static string GetMapJson(VMemberGeolocationMapJson[] info)
        {
            JsonSerializer serializer = new JsonSerializer();
            StringWriter stringWriter = new StringWriter();
            
            using (var writer = new JsonTextWriter(stringWriter))
            {
                writer.QuoteName = false;
                writer.QuoteChar = '\'';
                serializer.Serialize(writer, info);
            }

            return  stringWriter.ToString();
        }

        public static string GetRegisteredFromDateMapJson(DateTime from)
        {
            return GetMapJson(GetRegisteredFromDate(from));
        }

        public static string GetRegisteredFromLastWeekMapJson()
        {
            return GetMapJson(GetRegisteredFromLastWeek());
        }

        public static string GetRegisteredLastMapJson(int top = 50)
        {
            return GetMapJson(GetRegisteredLast(top));
        }

        #endregion
    }
}