using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Prem.PTC.Advertising;
using System.Web;
using System.Web.UI;

namespace Titan.Advertising
{

    public class BannerBid : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "BannerBids"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("BannerAuctionId")]
        public int BannerAuctionId { get { return _BannerAuctionId; } set { _BannerAuctionId = value; SetUpToDateAsFalse(); } }

        [Column("Username")]
        public string Username { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("BidValue")]
        public Money BidValue { get { return _BidValue; } set { _BidValue = value; SetUpToDateAsFalse(); } }

        [Column("BannerAdvertId")]
        public int BannerAdvertId { get { return _BannerAdvertId; } set { _BannerAdvertId = value; SetUpToDateAsFalse(); } }

        [Column("RefAndPoolsCredited")]
        public bool RefAndPoolsCredited { get { return _RefAndPoolsCredited; } set { _RefAndPoolsCredited = value; SetUpToDateAsFalse(); } }


        private int _id, _BannerAuctionId, _BannerAdvertId;
        private string name;
        private Money _BidValue;
        private bool _RefAndPoolsCredited;

        #endregion Columns

        public BannerBid()
            : base() { }

        public BannerBid(int id) : base(id) { }

        public BannerBid(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }


        public string ToString(int place, bool isPaid = false)
        {
            //Generate HTML
            StringBuilder sb = new StringBuilder();

              //<div class="bannerBid">
              //          <h3>1: $4.54</h3>
              //          <div class="bannerBidPaid">PAID: 101%</div><br />
              //          <img src="dupa" /> <br />
              //          Username: asdasdfsdf<br />
              //          http://onet.pl
              //      </div>
            BannerAdvert TargetBanner = this.Banner;
            string url = (HttpContext.Current.Handler as Page).ResolveUrl(TargetBanner.ImagePath);

            sb.Append("<div class=\"bannerBid\"><h3>")
              .Append(place)
              .Append(": ")
              .Append(BidValue.ToString())
              .Append("</h3>");

            if (isPaid)
            {
                sb.Append("<div class=\"bannerBidPaid\">");
                sb.Append(Resources.U4000.PAID);
                sb.Append(": ");
                sb.Append(AppSettings.BannerAdverts.LostBidsReturnPercent);
                sb.Append("%</div><br />");
            }

            sb.Append("<img src=\"");
            sb.Append(url);
            sb.Append("\" class=\"bannerBidImage\" /> <br/>");
            sb.Append(Resources.L1.USERNAME);
            sb.Append(": ");
            sb.Append(Username)
              .Append("<br/><i>")
              .Append(TargetBanner.TargetUrl)
              .Append("</i></div>");

            return sb.ToString();
        }

        public BannerAdvert Banner
        {
            get
            {
                return TableHelper.SelectRows<BannerAdvert>(TableHelper.MakeDictionary("BannerAdvertId", BannerAdvertId))[0];
            }
        }
    }
}