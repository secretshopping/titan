using Prem.PTC;
using Prem.PTC.Utils;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Titan.Cryptocurrencies;

public enum BalanceType
{
    [Description("Main Balance")]
    MainBalance = 1,
    [Description("Purchase Balance")]
    PurchaseBalance = 2,
    [Description("Traffic Balance")]
    TrafficBalance = 3,
    [Description("Points Balance")]
    PointsBalance = 4,
    [Description("Commission Balance")]
    CommissionBalance = 6,
    [Description("PTC Credits")]
    PTCCredits = 7,
    [Description("Cash Balance")]
    CashBalance = 8,
    [Description("Investment Balance")]
    InvestmentBalance = 9,
    [Description("Marketplace Balance")]
    MarketplaceBalance = 10,

    //Cryptocurrency-Wallet balances
    [Description("BTC")]
    BTC = 11,
    [Description("ETH")]
    ETH = 12,
    [Description("XRP")]
    XRP = 13,
    [Description("Coin")]
    Token = 14,
    [Description("FreezedCoin")]
    FreezedToken = 15,

    [Description("Login Ads Credits")]
    LoginAdsCredits = 16,

    InvestmentLevels = 17
}

public static class BalanceTypeHelper
{
    public static bool IsCryptoBalance(BalanceType type)
    {
        switch (type)
        {
            case BalanceType.BTC:
            case BalanceType.ETH:
            case BalanceType.Token:
            case BalanceType.XRP:
            case BalanceType.FreezedToken:
                return true;
        }

        return false;
    }

    public static bool CheckItem(BalanceType type)
    {
        switch (type)
        {
            case BalanceType.MainBalance:
            case BalanceType.PurchaseBalance:
            case BalanceType.PointsBalance:
            case BalanceType.CommissionBalance:
            case BalanceType.CashBalance:
                return true;
            case BalanceType.TrafficBalance:
            case BalanceType.PTCCredits:
                return AppSettings.TitanModules.HasProduct(2);
            case BalanceType.InvestmentBalance:
                return AppSettings.TitanModules.HasProduct(5) && false;
            case BalanceType.MarketplaceBalance:
                return AppSettings.TitanModules.HasModule(9);
            case BalanceType.BTC:
                return AppSettings.TitanModules.HasModule(3);
            case BalanceType.ETH:
            case BalanceType.Token:
            case BalanceType.FreezedToken:
            case BalanceType.XRP:
                return AppSettings.TitanModules.HasModule(51);
            case BalanceType.LoginAdsCredits:
                return AppSettings.TitanModules.HasModule(14);
        }

        return false;
    }

    public static String GetName(BalanceType balanceType)
    {
        switch (balanceType)
        {
            case BalanceType.MainBalance:
                if (TitanFeatures.IsTrafficThunder)
                    return String.Format(U6012.WALLET, String.Format("{0} {1}", AppSettings.Site.CurrencyCode, U5004.MAIN));
                return L1.MAINBALANCE;

            case BalanceType.PurchaseBalance:
                if (TitanFeatures.IsTrafficThunder)
                    return String.Format(U6012.WALLET, AppSettings.Site.CurrencyCode);
                return U6012.PURCHASEBALANCE;

            case BalanceType.TrafficBalance:
                return U4200.TRAFFICBALANCE;

            case BalanceType.PointsBalance:
                return String.Format("{0}", AppSettings.PointsName);

            case BalanceType.CommissionBalance:
                return U5004.COMMISSIONBALANCE;

            case BalanceType.PTCCredits:
                return String.Format("{0} {1}", U6003.PTC, U6012.CREDITS);

            case BalanceType.CashBalance:
                return U5008.CASHBALANCE;

            case BalanceType.InvestmentBalance:
                return U6006.INVESTMENTBALANCE;

            case BalanceType.MarketplaceBalance:
                return U6008.MARKETPLACEBALANCE;

            case BalanceType.BTC:
            case BalanceType.ETH:
            case BalanceType.XRP:
                return String.Format(U6012.WALLET, balanceType.ToString());

            case BalanceType.Token:
                return String.Format(U6012.WALLET, CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token).Code);

            case BalanceType.FreezedToken:
                return String.Format(U6012.WALLET, CryptocurrencyFactory.Get(CryptocurrencyType.ERCFreezed).Code);

            case BalanceType.LoginAdsCredits:
                return U5008.LOGINADSCREDITS;

            default:
                return String.Empty;
        }
    }

    public static String GetDisplayValue(BalanceType balanceType, decimal input)
    {
        Money ValueInMoney = new Money(input);

        if (balanceType == BalanceType.PointsBalance)
            return ValueInMoney.AsPoints().ToString();

        else if (balanceType == BalanceType.PTCCredits)
            return ValueInMoney.ToClearString();

        else if (BalanceTypeHelper.IsCryptoBalance(balanceType))
            return (new CryptocurrencyMoney(CryptocurrencyTypeHelper.ConvertToCryptocurrencyType(balanceType), input)).ToString();


        return ValueInMoney.ToString();
    }

    /// <summary>
    /// Returns list control source with all statuses' short descriptions as text 
    /// and int status ordinal as value
    /// </summary>
    public static ListItem[] ListItems
    {
        get
        {
            var items = new List<ListItem>();

            foreach (BalanceType type in Enum.GetValues(typeof(BalanceType)))
                if (CheckItem(type))
                    items.Add(new ListItem(type.GetDescription(), (int)type + ""));

            return items.ToArray();
        }
    }

}