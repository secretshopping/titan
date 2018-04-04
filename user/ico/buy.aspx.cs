using Prem.PTC;
using System;
using Resources;
using Titan.ICO;
using Titan.Cryptocurrencies;
using System.Web.UI.WebControls;
using Prem.PTC.Utils;
using Prem.PTC.Members;

public partial class user_ico_buy : System.Web.UI.Page
{
    public Cryptocurrency TokenCryptocurrency { get; set; }
    public Cryptocurrency BtcCryptocurrency { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.ICOBuyEnabled);
        TokenCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token);
        BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

        if (!Page.IsPostBack)
        {
            SetupLanguagesAndTexts();
        }
    }

    protected void SetupLanguagesAndTexts()
    {
        var currentStage = ICOStage.GetCurrentStage();
        var priceStage = currentStage;
        BuyFromBTCWalletButton.Visible = BtcCryptocurrency.WalletEnabled;

        if (currentStage == null)
        {
            priceStage = ICOStage.GetNextStage();
            BuyFromPurchaseBalanceButton.Visible = BuyFromBTCWalletButton.Visible = false;
        }

        Money tokenPrice = Money.Zero;

        if (priceStage == null) //No next stage coming
            tokenPrice = AppSettings.Ethereum.ERC20TokenRate;
        else
            tokenPrice = priceStage.TokenPrice;

        BTCValueLabel.Visible = BtcCryptocurrency.WalletEnabled;
        BTCValueLiteral.Text = String.Format("1 {0} = <b id='BTCPrice'>{1}</b> BTC", TokenCryptocurrency.Code,
            (tokenPrice.ToDecimal() / CryptocurrencyFactory.Get(CryptocurrencyType.BTC).GetValue().ToDecimal()).TruncateDecimals(8));

        USDValueLiteral.Text = String.Format("1 {0} = <b>{1}</b><span id='tokenPrice' style='display:none'>{2}</span>", TokenCryptocurrency.Code,
            tokenPrice.ToString(), tokenPrice.ToDecimal());

        MaxVolumeLiteral.Text = String.Format(U6012.MAXPER14MIN, "<b>" + AppSettings.ICO.ICOPurchaseLimitPerUserPer15mins + "</b>",
            TokenCryptocurrency.Code);

        LangAdder.Add(BuyFromPurchaseBalanceButton, U6012.PAYVIAPURCHASEBALANCE);
        LangAdder.Add(BuyFromBTCWalletButton, String.Format(U6012.PAYVIAWALLET, "BTC"));
        LangAdder.Add(RequiredFieldValidator4, L1.ER_ALLFIELDSREQUIRED);
        NumberOfTokensTextBox.Attributes["placeholder"] = String.Format("{0}", L1.AMOUNT);

        if (TitanFeatures.IsTrafficThunder)
            BuyFromPurchaseBalanceButton.Visible = false;
    }


    protected void Validate_Captcha(object source, ServerValidateEventArgs args)
    {
        if (TitanCaptcha.IsValid)
        {
            args.IsValid = true;
        }
        else
        {
            args.IsValid = false;
        }
    }

    protected void BuyFromPurchaseBalanceButton_Click(object sender, EventArgs e)
    {
        TryPurchaseTokens(BalanceType.PurchaseBalance);
    }

    protected void BuyFromBTCWalletButton_Click(object sender, EventArgs e)
    {
        if (!BtcCryptocurrency.WalletEnabled)
            throw new MsgException("BTC Wallet purchase is not enabled.");

        TryPurchaseTokens(BalanceType.BTC);
    }

    protected void TryPurchaseTokens(BalanceType balanceType)
    {
        SuccMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;

        try
        {
            int numberOfTokens = Convert.ToInt32(NumberOfTokensTextBox.Text);
            Member user = Member.Current;

            ICOManager.TryPurchaseTokens(user, ICOStage.GetCurrentStage(), numberOfTokens, balanceType);

            SuccMessagePanel.Visible = true;
            SuccMessage.Text = String.Format(U6012.SUCCTOKENSPURCHASE, numberOfTokens, TokenCryptocurrency.Code);
        }
        catch (MsgException ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            throw ex;
        }
    }
}