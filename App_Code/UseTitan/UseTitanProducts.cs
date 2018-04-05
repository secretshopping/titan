using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UseTitanHelpers
{
    public class UseTitanProducts
    {
        public static string GetHTML(TitanProduct product)
        {
            return String.Format("<img src='https://usetitan.com/Images/ProductsModules/{0}' style='width:16px; vertical-align:middle'/><b>{1}</b>",
                GetImage(product), GetName(product));
        }

        public static string GetName(TitanProduct product)
        {
            switch (product)
            {
                case TitanProduct.GPTPTC:
                    return "TITAN GPT+PTC+TE";
                case TitanProduct.RevenueSharing:
                    return "TITAN Revenue Sharing";
                case TitanProduct.AffiliateNetwork:
                    return "TITAN Affiliate Network";
                case TitanProduct.InvestmentPlatform:
                    return "TITAN Investment Platform";
                case TitanProduct.CryptocurrencyTrading:
                    return "TITAN Cryptocurrency Trading";
                default:
                    return String.Empty;
            }
        }

        public static string GetImage(TitanProduct product)
        {
            switch (product)
            {
                case TitanProduct.GPTPTC:
                    return "logo-1.png";
                case TitanProduct.RevenueSharing:
                    return "logo-2.png";
                case TitanProduct.AffiliateNetwork:
                    return "logo-green.png";
                case TitanProduct.InvestmentPlatform:
                    return "logo-3.png";
                case TitanProduct.CryptocurrencyTrading:
                    return "logo-4.png";
                default:
                    return String.Empty;
            }
        }
    }
}