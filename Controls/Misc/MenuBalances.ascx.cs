using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using Titan.Cryptocurrencies;

public partial class Controls_Misc_MenuBalances : System.Web.UI.UserControl
{
    public Cryptocurrency TokenCryptocurrency { get; set; }
    public Cryptocurrency BtcCryptocurrency { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Member.IsLogged)
        {
            var user = Member.CurrentInCache;

            TokenCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token);
            BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

            if (TitanFeatures.PurchaseBalanceDisabled)
                AdBalancePlaceHolder.Visible = false;

            PointsPlaceHolder.Visible = AppSettings.Points.PointsEnabled;
            CashBalancePlaceHolder.Visible = AppSettings.Payments.CashBalanceEnabled;
            CommissionBalancePlaceHolder.Visible = AppSettings.Payments.CommissionBalanceEnabled;
            TrafficBalancePlaceHolder.Visible = AppSettings.TitanFeatures.EarnTrafficExchangeEnabled;
            MarketPlaceBalancePlaceHolder.Visible = AppSettings.Payments.MarketplaceBalanceEnabled;
            InvestmentBalancePlaceHolder.Visible = AppSettings.InvestmentPlatform.InvestmentBalanceEnabled;
            BTCWalletPlaceHolder.Visible = BtcCryptocurrency.WalletEnabled;
            ERC20TokenPlaceHolder.Visible = TokenCryptocurrency.WalletEnabled;
            ERC20FreezedWalletPlaceHolder.Visible = TokenCryptocurrency.WalletEnabled && AppSettings.Ethereum.ERC20TokensFreezeSystemEnabled;

            MainBalanceLiteral.Text = string.Format("<td class='text-success'>{0}:</td><td class='text-success'><b>{1}</b></td>", U5004.MAIN, user.MainBalance.ToString());
            AdBalanceLiteral.Text = string.Format("<td>{0}:</td><td><b>{1}</b></td>", U6012.PURCHASE, user.PurchaseBalance.ToString());
            PointsLiteral.Text = string.Format("<td>{0}:</td><td><b>{1}</b></td>", AppSettings.PointsName, user.PointsBalance.ToString());
            CashBalanceLiteral.Text = string.Format("<td>{0}:</td><td><b>{1}</b></td>", !TitanFeatures.IsRofriqueWorkMines ? U6002.CASH : "Funds Deposited", user.CashBalance.ToString());
            CommissionBalanceLiteral.Text = string.Format("<td>{0}:</td><td><b>{1}</b></td>", U5004.COMMISSION, user.CommissionBalance.ToString());
            TrafficBalanceLiteral.Text = string.Format("<td>{0}:</td><td><b>{1}</b></td>", U5004.TRAFFIC, user.TrafficBalance.ToString());
            MarketPlaceBalanceLiteral.Text = string.Format("<td>{0}:</td><td><b>{1}</b></td>", U5006.MARKETPLACE, user.MarketplaceBalance.ToString());
            InvestmentBalanceLiteral.Text = string.Format("<td>{0}:</td><td><b>{1}</b></td>", U6006.INVESTMENT, user.InvestmentBalance.ToString());

            if (BtcCryptocurrency.WalletEnabled)
                BTCWalletLiteral.Text = string.Format("<td class='text-warning'>{0}:</td><td class='text-warning'><b>{1}</b></td>", String.Format(U6012.WALLET, "BTC"),
                    user.GetCryptocurrencyBalance(CryptocurrencyType.BTC));

            if (TokenCryptocurrency.WalletEnabled)
                ERC20WalletLiteral.Text = string.Format("<td>{0}:</td><td><b>{1}</b></td>", String.Format(U6012.WALLET, TokenCryptocurrency.Code),
                    user.GetCryptocurrencyBalance(CryptocurrencyType.ERC20Token));

            if(TokenCryptocurrency.WalletEnabled && AppSettings.Ethereum.ERC20TokensFreezeSystemEnabled)
                ERC20FreezedTokensWalletLiteral.Text = string.Format("<td>{0}:</td><td><b>{1}</b></td>",
                    TokenCryptocurrency.Code + " Freezed", 
                    user.GetCryptocurrencyBalance(CryptocurrencyType.ERCFreezed));
        }
    }
}