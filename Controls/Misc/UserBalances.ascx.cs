using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Web;
using Titan.Cryptocurrencies;

public partial class Controls_Misc_UserBalances : System.Web.UI.UserControl
{
    public Cryptocurrency TokenCryptocurrency { get; set; }
    public Cryptocurrency BtcCryptocurrency { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        TokenCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token);
        BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

        CommissionBalanceContainer.Visible = AppSettings.Payments.CommissionBalanceEnabled;
        PointsBalanceContainer.Visible = AppSettings.Points.PointsEnabled;
        TrafficBalanceContainer.Visible = AppSettings.TitanFeatures.EarnTrafficExchangeEnabled;
        CashBalanceContainer.Visible = AppSettings.Payments.CashBalanceEnabled;
        InvestmentBalanceContainer.Visible = AppSettings.InvestmentPlatform.InvestmentBalanceEnabled;
        MarketplaceBalanceContainer.Visible = AppSettings.Payments.MarketplaceBalanceEnabled;

        BTCWalletContainer.Visible = BtcCryptocurrency.WalletEnabled;
        ERC20TokenWalletContainer.Visible = TokenCryptocurrency.WalletEnabled;
        ERC20FreezedTokensContainer.Visible = TokenCryptocurrency.WalletEnabled && AppSettings.Ethereum.ERC20TokensFreezeSystemEnabled;

        #region Customizations
        if (TitanFeatures.IsAhmed && HttpContext.Current.Request.Url.AbsolutePath.Contains("user/default.aspx"))
            DailyTaskPlaceHolder.Visible = true;

        if (TitanFeatures.PurchaseBalanceDisabled)
            AdBalancePlaceHolder.Visible = false;

        if(BtcCryptocurrency.WalletEnabled || TokenCryptocurrency.WalletEnabled)
        {
            //placeholder for custom balance design
        }

        if (BtcCryptocurrency.WalletEnabled)
        {
            CryptocurrencyMoney BTCWallet = Member.CurrentInCache.GetCryptocurrencyBalance(CryptocurrencyType.BTC);
            Money BTCValue = CryptocurrencyFactory.Get(CryptocurrencyType.BTC).GetValue();
            EstimatedBTCWalletValueLiteral.Text = ((Money)(BTCWallet*BTCValue)).ToString();
            EstimatedBTCWalletValueLiteral.Visible = true;
        }

        if (TokenCryptocurrency.WalletEnabled)
        {
            CryptocurrencyMoney ERC20Wallet = Member.CurrentInCache.GetCryptocurrencyBalance(CryptocurrencyType.ERC20Token);
            Money ERC20TokenValue = AppSettings.Ethereum.ERC20TokenRate;
            EstimatedERC20WalletValueLiteral.Text = ((Money)ERC20Wallet * ERC20TokenValue).ToString();
        }

        if(TokenCryptocurrency.WalletEnabled && AppSettings.Ethereum.ERC20TokensFreezeSystemEnabled)
        {
            CryptocurrencyMoney ERC20FreezedTokens = Member.CurrentInCache.GetCryptocurrencyBalance(CryptocurrencyType.ERCFreezed);
            Money ERC20TokenValue = AppSettings.Ethereum.ERC20TokenRate;
            EstimatedERC20FreezedTokensValueLiteral.Text = ((Money)ERC20FreezedTokens * ERC20TokenValue).ToString();
        }

        if (AppSettings.Points.PointsEnabled)
            EstimatedPointsValueLiteral.Text = new Money((decimal)Member.CurrentInCache.PointsBalance / (decimal)Points.GetPointsPer1d()).ToString();

        #endregion
    }
}