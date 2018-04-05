using Prem.PTC;
using System.Data;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Prem.PTC.Members;

namespace Titan.Publisher
{
    public class GlobalPostback : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "GlobalPostbacks"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("Url")]
        public string Url { get { return _Url; } set { _Url = value; SetUpToDateAsFalse(); } }

        [Column("OfferId")]
        public string OfferId { get { return _OfferId; } set { _OfferId = value; SetUpToDateAsFalse(); } }

        [Column("OfferName")]
        public string OfferName { get { return _OfferName; } set { _OfferName = value; SetUpToDateAsFalse(); } }

        [Column("Ip")]
        public string Ip { get { return _Ip; } set { _Ip = value; SetUpToDateAsFalse(); } }

        [Column("CountryCode")]
        public string CountryCode { get { return _CountryCode; } set { _CountryCode = value; SetUpToDateAsFalse(); } }

        [Column("SubId")]
        public string SubId { get { return _SubId; } set { _SubId = value; SetUpToDateAsFalse(); } }

        [Column("Payout")]
        protected Money Payout { get { return _Payout; } set { _Payout = value; SetUpToDateAsFalse(); } }

        [Column("CurrencyCode")]
        public string CurrencyCode { get { return _CurrencyCode; } set { _CurrencyCode = value; SetUpToDateAsFalse(); } }

        [Column("SubId2")]
        public string SubId2 { get { return _SubId2; } set { _SubId2 = value; SetUpToDateAsFalse(); } }

        [Column("SubId3")]
        public string SubId3 { get { return _SubId3; } set { _SubId3 = value; SetUpToDateAsFalse(); } }

        [Column("Age")]
        public int Age { get { return _Age; } set { _Age = value; SetUpToDateAsFalse(); } }

        [Column("Gender")]
        protected int GenderInt { get { return _GenderInt; } set { _GenderInt = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

        [Column("SendAttempts")]
        public int SendAttempts { get { return _SendAttempts; } set { _SendAttempts = value; SetUpToDateAsFalse(); } }

        [Column("PublishersWebsiteId")]
        public int PublishersWebsiteId { get { return _PublishersWebsiteId; } set { _PublishersWebsiteId = value; SetUpToDateAsFalse(); } }

        [Column("GlobalPostbackType")]
        protected int GlobalPostbackTypeInt { get { return _GlobalPostbackTypeInt; } set { _GlobalPostbackTypeInt = value; SetUpToDateAsFalse(); } }

        public GlobalPostbackType GlobalPostbackType
        {
            get
            {
                return (GlobalPostbackType)GlobalPostbackTypeInt;
            }
            set
            {
                GlobalPostbackTypeInt = (int)value;
            }
        }

        public PostbackStatus Status
        {
            get
            {
                return (PostbackStatus)StatusInt;
            }
            set
            {
                StatusInt = (int)value;
            }
        }

        public Gender Gender
        {
            get
            {
                return (Gender)GenderInt;
            }
            set
            {
                GenderInt = (int)value;
            }
        }

        int _Id, _StatusInt, _SendAttempts, _PublishersWebsiteId, _Age, _GlobalPostbackTypeInt, _GenderInt;
        string _Url, _SubId2, _SubId3, _Ip, _CountryCode, _OfferName, _SubId, _CurrencyCode, _OfferId;
        Money _Payout;

        public GlobalPostback(int id)
            : base(id)
        {
        }

        public GlobalPostback(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        {
        }

        private GlobalPostback(GlobalPostbackType globalPostbackType, int offerId, string offerTitle, int publishersWebsiteId, string externalUsername, string subId2,
        string subId3, string ip, string countryCode, Money payout, Gender gender = Prem.PTC.Members.Gender.Null, int age = -1)
        {
            Url = new PublishersWebsite(publishersWebsiteId).PostbackUrl;
            OfferId = offerId.ToString();
            OfferName = offerTitle;
            Ip = ip;
            CountryCode = countryCode;
            SubId = externalUsername;
            Payout = payout;
            CurrencyCode = AppSettings.Site.CurrencyCode;
            SubId2 = subId2;
            SubId3 = subId3;
            Status = PostbackStatus.Notsent;
            SendAttempts = 0;
            PublishersWebsiteId = publishersWebsiteId;
            Age = age;
            Gender = gender;
            GlobalPostbackType = globalPostbackType;
        }

        public static GlobalPostback Create(GlobalPostbackType globalPostbackType, int offerId, string offerTitle, int publishersWebsiteId, string externalUsername, string subId2,
        string subId3, string ip, string countryCode, Money payout, Gender gender = Prem.PTC.Members.Gender.Null, int age = -1)
        {
            var postback = new GlobalPostback(globalPostbackType, offerId, offerTitle, publishersWebsiteId, externalUsername, subId2,
                subId3, ip, countryCode, payout, gender, age);
            postback.Save();
            return postback;
        }
        #region Test Postback
        public static string GetTestResponse(string url, string subId, Money payout, GlobalPostbackType globalPostbackType = GlobalPostbackType.Cpa)
        {
            using (var wb = new WebClient())
            {
                var data = GetTestPostData(subId, payout, globalPostbackType);

                var response = wb.UploadValues(url, "POST", data);
                var responseString = Encoding.ASCII.GetString(response);

                return responseString.Trim().ToLower();
            }
        }

        private static NameValueCollection GetTestPostData(string subId, Money payout, GlobalPostbackType globalPostbackType = GlobalPostbackType.Cpa)
        {
            var data = new NameValueCollection();
            data[Parameters.OfferId] = "TestOfferId";
            data[Parameters.OfferName] = "TestOfferName";
            data[Parameters.IpAddress] = "TestIpAddress";
            data[Parameters.CountryCode] = "TestCountryCode";
            data[Parameters.Age] = "TestAge";
            data[Parameters.Gender] = Prem.PTC.Members.Gender.Null.ToString();
            data[Parameters.SubId] = subId;
            data[Parameters.Payout] = payout.ToClearString();
            data[Parameters.CurrencyCode] = AppSettings.Site.CurrencyCode;
            data[Parameters.GlobalPostbackType] = globalPostbackType.ToString();
            return data;
        }
        #endregion

        public void Send()
        {

            if (this.Status == PostbackStatus.Ok)
                return;
            try
            {
                using (var wb = new WebClient())
                {
                    var data = GetPostData();

                    var response = wb.UploadValues(Url, "POST", data);
                    var responseString = Encoding.ASCII.GetString(response);

                    if (responseString.Trim().ToLower() == GlobalPostback.Parameters.SuccessfulResponse)
                        this.Status = PostbackStatus.Ok;
                    else
                        this.Status = PostbackStatus.Error;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                this.Status = PostbackStatus.Error;
            }
            finally
            {
                this.SendAttempts++;
                this.Save();
            }

        }


        protected NameValueCollection GetPostData()
        {
            var data = new NameValueCollection();
            data[Parameters.GlobalPostbackType] = GlobalPostbackType.ToString();
            data[Parameters.OfferId] = OfferId.ToString();
            data[Parameters.OfferName] = OfferName;
            data[Parameters.IpAddress] = Ip;
            data[Parameters.CountryCode] = CountryCode;
            data[Parameters.SubId] = SubId;
            data[Parameters.Payout] = Payout.ToClearString();
            data[Parameters.CurrencyCode] = CurrencyCode;

            if (Age != -1)
                data[Parameters.Age] = Age.ToString();

            data[Parameters.Gender] = Gender.ToString();
            if (!string.IsNullOrEmpty(SubId2))
            {
                data[Parameters.SubId2] = SubId2;
            }
            if (!string.IsNullOrEmpty(SubId3))
            {
                data[Parameters.SubId3] = SubId3;
            }

            return data;
        }


        public static class Parameters
        {
            public static string OfferId = "offer_id";
            public static string OfferName = "offer_name";
            public static string IpAddress = "ip_address";
            public static string CountryCode = "country_code";
            public static string SubId = "sub_id";
            public static string Payout = "payout";
            public static string CurrencyCode = "currency_code";
            public static string SubId2 = "sub_id2";
            public static string SubId3 = "sub_id3";
            public static string Age = "age";
            public static string Gender = "gender";
            public static string PublishersWebsiteId = "publisher_website_id";
            public static string SuccessfulResponse = "ok";
            public static string GlobalPostbackType = "action_type";
        }
    }

    public enum PostbackStatus
    {
        Notsent = 1,
        Ok = 2,
        Error = 3
    }

    public enum GlobalPostbackType
    {
        Cpa = 1,
        PtcOfferWall = 2
    }
}