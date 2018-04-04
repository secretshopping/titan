using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Text;
using Prem.PTC.Advertising;
using System.Text;
using Prem.PTC.Members;

namespace Titan.Advertising
{
    public class BannerAuctionManager
    {
        public static readonly int AuctionsGeneratedUpfrontDays = 100;

        /// <summary>
        /// Number of Constant Banners displayed on the same page = 
        /// number of banners winning particular auction =
        /// number of not refunded banners
        /// </summary>
        public static readonly int DisplayedNormalBannerNumber = 1;

        public static void CRON()
        {
            BannerAuctionSQLManager.ExecuteCRONSQL();
        }

        /// <summary>
        /// Return current banner that should be displayed, NULL otherwise
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position">if you want to get different position than 1st banner (e.g. you have multiple banner controls)</param>
        /// <returns></returns>
        public static BannerAdvert GetCurrentBanner(BannerAdvertDimensions dimensions, int position = 1)
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                var currentSql = bridge.Instance.ExecuteRawCommandToDataTable(BannerAuctionSQLManager.GetCurrentSQL(dimensions));
                var currentList = TableHelper.GetListFromDataTable<BannerAuction>(currentSql, 1);

                if (currentList.Count > 0)
                {
                    BannerAuction auction = currentList[0];
                    BannerBid bid = auction.GetHighestBid(position);
                    if (bid != null)
                    {
                        if (!bid.RefAndPoolsCredited)
                        {

                            BannerCrediter crediter = new BannerCrediter(new Member(bid.Username));
                            Money moneyLeftForPools = crediter.CreditReferer(bid.BidValue);
                            PoolDistributionManager.AddProfit(ProfitSource.Banners, moneyLeftForPools);

                            bid.RefAndPoolsCredited = true;
                            bid.Save();
                        }
                        return new BannerAdvert(bid.BannerAdvertId);
                    }
                }
                return null;
            }
        }

        public static BannerAuction GetFirstActiveAuction(BannerAdvertDimensions dimensions)
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                var currentSql = bridge.Instance.ExecuteRawCommandToDataTable(BannerAuctionSQLManager.GetFirstActiveAuctionSQL(dimensions));
                var currentList = TableHelper.GetListFromDataTable<BannerAuction>(currentSql, 1);

                if (currentList.Count > 0)
                    return currentList[0];
                else
                {
                    BannerAuctionManager.CRON(); //No auctions created yet, do it
                    return null;
                }
            }
        }

        public static void AddBid(BannerBid bid, BannerAuction auction, Member user)
        {
            //Add new bid
            bid.Save();

            //Make history
            History.AddBidPlaced(user.Name, bid.BidValue.ToString());

            //Refund previous bid (if needed)
            BannerBid bidToRefund = null;
            bidToRefund = auction.GetHighestBid(DisplayedNormalBannerNumber + 1);

            if (bidToRefund != null)
            {
                RefundBid(bidToRefund);

                //Earning statistics
                HandleEarningStats(bid, auction, bidToRefund);
            }
        }

        private static void RefundBid(BannerBid bid)
        {
            if (bid != null)
            {
                Member user = new Member(bid.Username);
                user.AddToPurchaseBalance(Money.MultiplyPercent(bid.BidValue, AppSettings.BannerAdverts.LostBidsReturnPercent), "Bid refund");
                user.SaveBalances();
            }
        }

        private static void HandleEarningStats(BannerBid bid, BannerAuction auction, BannerBid refundBid)
        {
            EarningsStatsManager.Add(EarningsStatsType.Banner, bid.BidValue);
            if (refundBid != null)
                EarningsStatsManager.Subtract(EarningsStatsType.Banner, refundBid.BidValue);
        }
    }
}