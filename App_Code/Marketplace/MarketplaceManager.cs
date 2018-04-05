using MarchewkaOne.Titan.Balances;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.Marketplace
{
    public static class MarketplaceManager
    {
        private static void ConfirmIPN(MarketplaceIPN ipn)
        {
            MarketplaceProduct product = new MarketplaceProduct(ipn.ProductId);
            Member seller = new Member(product.SellerId);

            if (ipn.Status == MarketplaceIPNStatus.Pending)
            {
                ipn.Status = MarketplaceIPNStatus.Confirmed;
                ipn.Save();

                seller.AddToPurchaseBalance(ipn.ProductQuantity * product.Price, "Marketplace sale", BalanceLogType.MarketplaceSale);
                seller.SaveBalances();
            }
        }

        public static void TryConfirmIPN(int ipnId)
        {
            MarketplaceIPN ipn = new MarketplaceIPN(ipnId);
            ConfirmIPN(ipn);
        }
    }
}