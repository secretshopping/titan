using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;

namespace Titan.Shares
{
    public class SharesMarketManager
    {
        public static int GetSharesAvailableForSale(PortfolioShare share)
        {
            int result = share.Shares;

            //Check the market
            var sharesOnMarket = TableHelper.SelectRows<ShareOnMarket>(TableHelper.MakeDictionary("PortfolioShareId", share.Id));
            foreach (var shareOnMarket in sharesOnMarket)
            {
                result = result - shareOnMarket.SharesToSell;
            }

            return result;
        }

        public static void AddShareToMarket(PortfolioShare share, int units, Money price)
        {
            ShareOnMarket MarketShare = new ShareOnMarket();
            MarketShare.SharesToSell = units;
            MarketShare.Username = share.OwnerUsername;
            MarketShare.Price = price;
            MarketShare.PortfolioShareId = share.Id;
            MarketShare.PortfolioProductId = share.PortfolioProductId;
            MarketShare.DateStarted = DateTime.Now;
            MarketShare.Save();
        }

        public static void SellShare(ShareOnMarket share, Member user)
        {
            //Add transaction
            PortfolioShareTransation transaction = new PortfolioShareTransation();
            transaction.BuyerUsername = user.Name;
            transaction.DateSold = DateTime.Now;
            transaction.PortfolioProductId = share.PortfolioProductId;
            transaction.SellerUsername = share.Username;
            transaction.SoldAmount = share.Price;
            transaction.SoldFee = CalculateFee(share.Price);
            transaction.Units = share.SharesToSell;
            transaction.Save();

            //Remove old share
            PortfolioShare Old = new PortfolioShare(share.PortfolioShareId);
            Old.Shares -= share.SharesToSell;
            Old.Save();

            //Add new share
            PortfolioShare New = GetProperShare(share, user);
            New.Shares += share.SharesToSell;
            New.Save();

            //Remove from market
            share.Delete();

            //Earnings stats
            EarningsStatsManager.Add(EarningsStatsType.PortfolioUnits, transaction.SoldFee);
        }

        public static Money CalculatePriceWithFee(Money amount)
        {
            return amount + CalculateFee(amount);
        }

        public static Money CalculateFee(Money amount)
        {
            return Money.MultiplyPercent(amount, AppSettings.Shares.SharesMarketSaleCommission);
        }

        //
      
        private static PortfolioShare GetProperShare(ShareOnMarket share, Member user)
        {
            PortfolioShare New;

            //Check if we already dont have this kind of shares
            var where = TableHelper.MakeDictionary("OwnerUsername", user.Name);
            where.Add("PortfolioProductId", share.PortfolioProductId);
            var current = TableHelper.SelectRows<PortfolioShare>(where);

            if (current.Count > 0)
                New = current[0];
            else
            {
                New = new PortfolioShare();
                New.PortfolioProductId = share.PortfolioProductId;
                New.OwnerUsername = user.Name;
                New.Shares = 0;
            }

            return New;
        }
    }
}