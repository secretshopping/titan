using ExtensionMethods;
using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Prem.PTC.Offers;
using Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.Publish.PTCOfferWalls
{
    public class PTCOfferWallView : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "PTCOfferWallViews"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("Username")]
        public string Username { get { return _Username; } protected set { _Username = value; SetUpToDateAsFalse(); } }

        [Column("DateTimeViewed")]
        public DateTime DateTimeViewed { get { return _DateTimeViewed; } protected set { _DateTimeViewed = value; SetUpToDateAsFalse(); } }

        [Column("PTCOfferWallId")]
        public int PTCOfferWallId { get { return _PTCOfferWallId; } protected set { _PTCOfferWallId = value; SetUpToDateAsFalse(); } }

        [Column("PublishersWebsiteId")]
        public int PublishersWebsiteId { get { return _PublishersWebsiteId; } protected set { _PublishersWebsiteId = value; SetUpToDateAsFalse(); } }

        [Column("PublisherPayout")]
        public Money PublisherPayout { get { return _PublisherPayout; } protected set { _PublisherPayout = value; SetUpToDateAsFalse(); } }

        #endregion Columns


        int _Id, _PTCOfferWallId, _PublishersWebsiteId;
        string _Username;
        DateTime _DateTimeViewed;
        Money _PublisherPayout;

        public PTCOfferWallView(int id) : base(id) { }

        public PTCOfferWallView(DataRow row, bool isUpToDate = true)
                    : base(row, isUpToDate) { }

        private PTCOfferWallView(string username, int ptcOfferWallId, int publishersWebsiteId, Money publisherPayout)
        {
            Username = username;
            PTCOfferWallId = ptcOfferWallId;
            PublishersWebsiteId = publishersWebsiteId;
            DateTimeViewed = AppSettings.ServerTime;
            PublisherPayout = publisherPayout;
        }
       
        public static void Add(string username, int ptcOfferWallId, int publishersWebsiteId, Money publisherPayout)
        {
            var ptcOfferWallView = new PTCOfferWallView(username, ptcOfferWallId, publishersWebsiteId, publisherPayout);
            ptcOfferWallView.Save();
        }

        public static List<PTCOfferWallView> GetWatchedToday(string userName, int publishersWebsiteId)
        {
            string query = string.Format(@"SELECT * FROM PTCOfferWallViews WHERE Username = '{0}'
                                        AND  PublishersWebsiteId = {1}
                                        AND DateTimeViewed > '{2}'",
                                        userName, publishersWebsiteId, AppSettings.ServerTime.AddHours(-24).ToDBString());

            return TableHelper.GetListFromRawQuery<PTCOfferWallView>(query);
        }
    }
}