using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Prem.PTC.Advertising;
using Resources;

namespace Titan.Advertising
{

    public class BannerAuction : BaseTableObject
    {
        public static readonly TimeSpan DeleteOldAfter = TimeSpan.FromDays(3);
        //public static readonly TimeSpan CloseAuctionBefore = AuctionTime;

        #region Columns & Constructors

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "BannerAuctions"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("BannerType")]
        protected int IntBannerType { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

        [Column("DateStart")]
        public DateTime DateStart { get { return _DateStart; } set { _DateStart = value; SetUpToDateAsFalse(); } }

        private int _id, type;
        private DateTime _DateStart;

        public BannerAdvertDimensions BannerType
        {
            get
            {
                return new BannerAdvertDimensions(IntBannerType);
            }

            set
            {
                IntBannerType = value.Id;
            }
        }

        public BannerAuction()
            : base()
        { }

        public BannerAuction(int id) : base(id) { }

        public BannerAuction(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        { }


        #endregion Columns & Constructors

        public DateTime DateEnd
        {
            get
            {
                return DateStart.Add(AppSettings.BannerAdverts.AuctionTime);
            }
        }

        public BannerAuctionStatus Status
        {
            get
            {
                if (DateTime.Now > DateEnd.Add(DeleteOldAfter))
                    return BannerAuctionStatus.EndedToRemove;

                if (DateTime.Now >= DateEnd)
                    return BannerAuctionStatus.Ended;

                if (DateTime.Now >= DateStart)
                    return BannerAuctionStatus.OnDisplay;

                if (DateTime.Now.Add(AppSettings.BannerAdverts.AuctionTime) > DateStart)
                    return BannerAuctionStatus.Closed;

                return BannerAuctionStatus.Opened;
            }
        }

        public string Closes
        {
            get
            {
                switch (Status)
                {
                    case BannerAuctionStatus.Opened:
                        TimeSpan result = DateEnd.Subtract(DateTime.Now).Add(-AppSettings.BannerAdverts.AuctionTime).Add(-AppSettings.BannerAdverts.AuctionTime);
                        if (result.Days > 0)
                            return result.ToString("d'd 'h'h'");
                        if (result.Hours > 0)
                            return result.ToString("h'h 'm'm'");
                        if (result.Minutes > 0)
                            return result.ToString("m'm 's's'");
                        return result.ToString("s's'");

                    case BannerAuctionStatus.Closed:
                        return U4000.CLOSED;
                    case BannerAuctionStatus.OnDisplay:
                        return U4000.BANNERONDISPLAY;
                }
                return String.Empty;
            }
        }

        public string DisplayTime
        {
            get
            {
                switch (Status)
                {
                    case BannerAuctionStatus.Opened:
                        return DateStart.Hour + ":00";
                    case BannerAuctionStatus.Closed:
                        return DateStart.Hour + ":00(" + U4000.NEXT + ")";
                    case BannerAuctionStatus.OnDisplay:
                        return DateStart.Hour + ":00(" + U4000.NOW + ")";
                }
                return String.Empty;
            }
        }

        public string HighestBidString
        {
            get
            {
                BannerBid Highest = this.HighestBid;
                if (Highest == null)
                    return "<i>" + U4000.NOBIDSYET + "</i>";

                return Highest.BidValue.ToString();
            }
        }

        /// <summary>
        /// Returns NULL if no bids placed
        /// </summary>
        public BannerBid HighestBid
        {
            get
            {
                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    var highestSql = bridge.Instance.ExecuteRawCommandToDataTable("SELECT TOP 1 * FROM BannerBids WHERE BannerAuctionId = " + this.Id + " ORDER BY BidValue DESC");
                    var highestList = TableHelper.GetListFromDataTable<BannerBid>(highestSql, 1);

                    return highestList.Count > 0 ? highestList[0] : null;
                }
            }
        }

        public BannerBid GetHighestBid(int position)
        {
            if (position == 1)
                return HighestBid;
            else
            {
                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    var highestSql = bridge.Instance.ExecuteRawCommandToDataTable("SELECT TOP " + position +
                        " * FROM BannerBids WHERE BannerAuctionId = " + this.Id + " ORDER BY BidValue DESC");

                    var highestList = TableHelper.GetListFromDataTable<BannerBid>(highestSql, position);

                    return highestList.Count > (position - 1) ? highestList[position - 1] : null;

                }
            }
        }

        public Money NextMinBidValue
        {
            get
            {
                BannerBid Highest = HighestBid;

                if (Highest == null)
                {
                    return AppSettings.BannerAdverts.StartingAmount;

                }
                return Highest.BidValue + AppSettings.BannerAdverts.BidAmount;
            }
        }

        public List<BannerBid> GetBids()
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                var highestSql = bridge.Instance.ExecuteRawCommandToDataTable("SELECT * FROM BannerBids WHERE BannerAuctionId = " + this.Id + " ORDER BY BidValue DESC");
                return TableHelper.GetListFromDataTable<BannerBid>(highestSql, 100, true);
            }
        }


    }
}