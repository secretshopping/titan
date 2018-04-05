using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Resources;
using SocialNetwork;
using Titan.Cryptocurrencies;

public partial class UserDeposit : System.Web.UI.Page
{
    public Member user;
    public Cryptocurrency BtcCryptocurrency { get; set; }
    public Cryptocurrency EthCryptocurrency { get; set; }
    public Cryptocurrency XrpCryptocurrency { get; set; }
    public Cryptocurrency TokenCryptocurrency { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.MoneyTransferEnabled);

        user = Member.Current;
        AppSettings.Reload();

        BtcCryptocurrency = CryptocurrencyFactory.Get<BitcoinCryptocurrency>();
        EthCryptocurrency = CryptocurrencyFactory.Get<EthereumCryptocurrency>();
        XrpCryptocurrency = CryptocurrencyFactory.Get<RippleCryptocurrency>();
        TokenCryptocurrency = CryptocurrencyFactory.Get<RippleCryptocurrency>();

        DepositButton.Visible = AppSettings.Representatives.RepresentativesHelpDepositEnabled;

        if (!Page.IsPostBack)
        {

            if (BtcCryptocurrency.DepositEnabled)
            {
                BTCButton.Visible = !CryptocurrencyApiFactory.GetDepositApi(BtcCryptocurrency).AllowToUsePaymentButtons();

                if (TitanFeatures.IsRofriqueWorkMines)
                    BTCButton.Text = "BTC Deposits";
                else
                    BTCButton.Text = "BTC";
            }

            if (TitanFeatures.IsFlotrading)
            {
                BalanceButton.CssClass = "";
                BTCButton.CssClass = "ViewSelected";
                MenuMultiView.ActiveViewIndex = 1;
            }

            if (TitanFeatures.IsTrafficThunder)
                UserBalancesPlaceHolder.Visible = false;

            AppSettings.Reload();

            LangAdder.Add(DepositButton, U6010.DEPOSITVIAREPRESENTATIVE);
            LangAdder.Add(btnTransfer, L1.TRANSFER);
            LangAdder.Add(DepositViaRepresentativeButton, U6010.SENDTRANSFERMESSAGE);
            LangAdder.Add(MPesaConfirmButton, L1.CONFIRM);
            LangAdder.Add(BTCAmountRequiredFieldValidator, U2502.INVALIDMONEYFORMAT);
            LangAdder.Add(CalculatePointsValueButton, U6007.CALCULATE);

            //MPesa?
            if (Request.QueryString["mpesa"] != null)
            {
                StandardTransferPlaceHolder.Visible = false;
                MPesaTransferPlaceHolder2.Visible = true;
                var gateway = PaymentAccountDetails.GetFirstIncomeGateway<MPesaSapamaAccountDetails>();
                MPesaAmount.Text = String.Format(U6005.TODEPOSITVIAMPESA, gateway.Username, gateway.Username,
                    Money.Parse(Request.QueryString["mpesa"]).ToString());
            }

            //Pre-selected tab
            if (Request.QueryString["button"] != null)
            {
                var button = (Button)MenuButtonPlaceHolder.FindControl("Button" + Request.QueryString["button"]);
                MenuButton_Click(button, null);
            }

            RadioFrom.Items.AddRange(GenerateHTMLButtons.DepositBalanceFromItems);

            if (TitanFeatures.IsRofriqueWorkMines && !CryptocurrencyApiFactory.GetDepositApi(BtcCryptocurrency).AllowToUsePaymentButtons())
                RadioFrom.Items.Add(new ListItem("", "BTC"));

            BTCTo.Items.AddRange(GenerateHTMLButtons.BTCToItems);

            if (BtcCryptocurrency.DepositEnabled)
            {
                BTCValueLabel.Text = ClassicBTCValueLabel.Text = string.Format("<p class='alert alert-info'>{0}: <b>{1}</b></p>", U5003.ESTIMATEDBTCVALUE, BtcCryptocurrency.GetValue().ToString());
                BTCValueLabel.Visible = ClassicBTCValueLabel.Visible = AppSettings.Site.CurrencyCode != "BTC"; //We don't want to show 1B = 1B

                BTCPointsFrom.Items.AddRange(GenerateHTMLButtons.BTCFromItems);

                BTCPointsFrom.SelectedIndex = 0;

                if (BTCPointsFrom.SelectedValue == "Wallet")
                {
                    btnDepositBTC.Visible = true;

                    if (BtcCryptocurrency.DepositTarget == DepositTargetBalance.PurchaseBalance)
                        BTCTo.SelectedValue = "AdBalance";
                    else if (BtcCryptocurrency.DepositTarget == DepositTargetBalance.CashBalance)
                        BTCTo.SelectedValue = "CashBalance";
                    else if (BtcCryptocurrency.DepositTarget == DepositTargetBalance.Wallet)
                        BTCTo.SelectedValue = "BTCWallet";
                }

                BTCButtonPanel.Visible = CryptocurrencyApiFactory.GetDepositApi(BtcCryptocurrency).AllowToUsePaymentButtons();
                ClassicBTCPanel.Visible = !CryptocurrencyApiFactory.GetDepositApi(BtcCryptocurrency).AllowToUsePaymentButtons();
            }

            if (((AppSettings.Payments.CommissionToMainBalanceEnabled && !TitanFeatures.UserCommissionToMainBalanceEnabled)
                || (TitanFeatures.UserCommissionToMainBalanceEnabled && user.CommissionToMainBalanceEnabled)
                || TitanFeatures.IsRevolca) && user.CheckAccessCustomizeTradeOwnSystem)
            {
                RadioTo.Items.Add(new ListItem("", "Main balance"));
            }

            if (!AppSettings.Payments.CashBalanceEnabled)
            {
                ListItem cb = RadioTo.Items.FindByValue("Cash balance");
                if (cb != null)
                    RadioTo.Items.Remove(cb);
            }

            if (!AppSettings.Payments.MarketplaceBalanceEnabled)
            {
                ListItem cb = RadioTo.Items.FindByValue("Marketplace balance");
                if (cb != null)
                    RadioTo.Items.Remove(cb);
            }

            SetRadioToValues();
            SetProcessorValues();
            SetupSpecialTransfersProperties();

            if (RadioFrom.Items.Count == 0 || RadioTo.Items.Count == 0)
            {
                //No transfers available
                transferInputRow.Visible = false;
                TransferSameCommissionToMainLiteral.Visible = true;
                TransferSameCommissionToMainLiteral.Text = U5006.NOTRANSFEROPTIONS;
                transfertable.Visible = false;
            }

            if (TitanFeatures.IsTradeOwnSystem)
            {
                //Checking condition to display appropriate message
                if (!user.CheckAccessCustomizeTradeOwnSystem && user.CommissionToMainBalanceRequiredViewsMessageInt > 0)
                {
                    CommissionTransferInfoDiv.Visible = true;
                    CommissionTransferInfo.Text = String.Format(U6010.COMMISSIONBALANCETRANSFERINFO, user.CommissionToMainBalanceRequiredViewsMessageInt, AppSettings.RevShare.AdPack.AdPackName);
                }
                else if (!user.CheckAccessCustomizeTradeOwnSystem && user.CommissionToMainBalanceRequiredViewsMessageInt == 0)
                {
                    CommissionTransferInfoDiv.Visible = true;
                    CommissionTransferInfo.Text = String.Format(U6010.COMMISSIONBALANCETRANSFERNOACTIVEADPACKINFO, AppSettings.RevShare.AdPack.AdPackName);
                }
                else
                    CommissionTransferInfoDiv.Visible = false;
            }

            if (BtcCryptocurrency.DepositEnabled && BtcCryptocurrency.DepositMinimum > 0)
            {
                AdditionalInfoPlaceHolder.Visible = true;
                AdditionalInfoLiteral.Text = String.Format(U6012.WARNINGMINBTCDEPOSIT, CryptocurrencyMoney.Parse(BtcCryptocurrency.DepositMinimum.ToString(), CryptocurrencyType.BTC).ToString());
            }        
        }

        SetRadioItemsValues(RadioFrom);
        SetRadioItemsValues(RadioTo);
        SetRadioItemsValues(BTCTo);

        RemoveDuplicatesFromList();

        foreach (ListItem item in BTCPointsFrom.Items)
        {
            item.Attributes.Add("data-content", "<img src='../Images/OneSite/TransferMoney/GoCoin.png' /> BTC");
        }
        
        if (!TitanFeatures.IsRofriqueWorkMines)
            LangAdder.Add(BalanceButton, U5009.BALANCE);
        else
            LangAdder.Add(BalanceButton, "Cash Deposits");

        LangAdder.Add(btnDepositBTC, U4200.DEPOSIT);
        LangAdder.Add(classicbtcDepositBTC, U4200.DEPOSIT);
    }

    private void SetRadioItemsValues(DropDownList dropDownList)
    {
        foreach (ListItem item in dropDownList.Items)
        {
            item.Text = item.Value;

            string ImageIconName = String.Empty;
            
            if (item.Value == (String.Format("{0} Wallet", TokenCryptocurrency.Code)))
            {
                ImageIconName = AppSettings.Ethereum.ERC20TokenImageUrl;
            }
            else
            {
                ImageIconName = String.Format("../Images/OneSite/TransferMoney/{0}.png", item.Value);
            }

            if (TitanFeatures.IsClickmyad && item.Text == CryptocurrencyAPIProvider.CoinPayments.ToString())
            {
                item.Attributes.Add("data-content", String.Format("<img src='{0}' />", ImageIconName));
            }
            else if (item.Text == TransferOptionConst.PointsTransfer)
            {
                item.Attributes.Add("data-content", String.Format("<img src='{0}' style='height:40px' /> {1}", ImageIconName, AppSettings.PointsName));
            }
            else
            {
                item.Attributes.Add("data-content", String.Format("<img src='{0}' style='height:40px' /> {1}", ImageIconName, item.Text));
            }
        }
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        ErrorMessage.Text = "";

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);
        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
            b.CssClass = "";

        TheButton.CssClass = "ViewSelected";
    }

    protected void btnTransfer_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        ErrorMessage.Text = "";
        MessageUpdatePanel.Update();

        string amount = TransferFromTextBox.Text.Trim().Replace(",", ".");
        Money Amount;
        try
        {
            Amount = Money.Parse(amount).FromMulticurrency();

            //Direct BTC transfer. BTC -> BTC Wallet
            if (RequestedBTCCryptocurrencyTransfer)
                Amount = new CryptocurrencyMoney(CryptocurrencyType.BTC, Decimal.Parse(amount));

            Member User = Member.Current;

            var HtmlResponse = false;
            var ResultMessage = TransferHelper.TryInvokeTransfer(RadioFrom.SelectedValue, RadioTo.SelectedValue, Amount, User, ref HtmlResponse);

            if (ResultMessage == U3501.TRANSFEROK)
                Response.Redirect("../status.aspx?type=transferok");
            else
            {
                //It was a payment processor transfer
                btnTransfer.Visible = false;

                //MPesa Sapama?
                if (RadioFrom.SelectedValue == "MPesaAgent")
                    Response.Redirect("~/user/deposit.aspx?mpesa=" + Amount.ToClearString());

                PaymentAmountLabel.Text = L1.AMOUNT + ": <b>" + Amount.ToString() + "</b>";

                if (RadioFrom.SelectedValue == BtcCryptocurrency.DepositApiProcessor.ToString())
                {
                    PaymentFeeLabel.Visible = false;
                    PaymentAmountWithFeeLabel.Visible = false;
                }
                else
                {
                    var gateway = PaymentAccountDetails.GetFirstGateway(RadioFrom.SelectedValue, true);

                    if (gateway == null)
                        throw new MsgException("No specified gateway installed.");

                    Amount = gateway.CalculateAmountWithFee(Amount);
                    PaymentFeeLabel.Text = U3500.CASHOUT_FEES + ": <b>" + gateway.StaticFee.ToString() + "</b> + <b>" + gateway.PercentFee + "%</b>";
                    PaymentAmountWithFeeLabel.Text = L1.AMOUNT + " + " + U3500.CASHOUT_FEES + " = <b>" + Amount.ToString() + "</b>";

                    PaymentFeeLabel.Visible = true;
                    PaymentAmountWithFeeLabel.Visible = true;
                }

                PaymentAmountLabel.Visible = true;
                transferInputRow.Visible = false;
                dropdownlistsRow.Visible = false;
                PaymentButtons.Text = ResultMessage;
            }
        }
        catch (MsgException ex)
        {
            ShowErrorMessage(ex.Message);
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            ShowErrorMessage(ex.Message);
        }
    }

    protected void BTCPointsFrom_SelectedIndexChanged(object sender, EventArgs e)
    {
        ErrorMessagePanelBTC.Visible = false;
        if (BTCPointsFrom.SelectedValue == "Wallet")
        {
            btnDepositBTC.Visible = true;

            if (BtcCryptocurrency.DepositTarget == DepositTargetBalance.PurchaseBalance)
                BTCTo.SelectedValue = "AdBalance";
            else if (BtcCryptocurrency.DepositTarget == DepositTargetBalance.CashBalance)
                BTCTo.SelectedValue = "CashBalance";
            else if (BtcCryptocurrency.DepositTarget == DepositTargetBalance.Wallet)
                BTCTo.SelectedValue = "BTCWallet";
        }
    }
    protected void btnDepositBTC_Click(object sender, EventArgs e)
    {
        try
        {
            if (BtcCryptocurrency.DepositEnabled)
            {
                if (CryptocurrencyApiFactory.GetDepositApi(BtcCryptocurrency).AllowToUsePaymentButtons())
                {
                    BTCDepositInfopanel.Visible = false;

                    var amount = Money.Parse(BTCAmountTextBox.Text).FromMulticurrency();
                    var bg = new DepositCryptocurrencyButtonGenerator(user, amount);
                    BTCPaymentButton.Text = GenerateHTMLButtons.GetBtcButton(bg);
                }

                else
                {
                    classicbtcDepositBTC.Visible = false;
                    BTCValuePanel.Visible = true;

                    string adminAddress = CryptocurrencyApiFactory.Get(BtcCryptocurrency.DepositApiProcessor).TryGetAdminAddress();

                    WalletToBTCPanel.Visible = true;
                    ClassicBTCPanel.Visible = true;

                    if (!string.IsNullOrWhiteSpace(adminAddress))
                    {
                        multipleDepositWarningLiteral.Visible = true;
                        multipleDepositWarningLiteral.Text = "<p class='alert alert-warning'>" + U5005.MUSTWAITTODEPOSIT + "</p>";

                        BitcoinQRCode.ImageUrl = "~/Handlers/Utils/BitcoinQRCode.ashx?address=" + adminAddress;

                        depositBTCLabel.Text = adminAddress;

                        DepositBTCInfoPanel.Visible = true;
                        DepositBTCInfoLabel.Text = string.Format(U4200.DEPOSITBTCDESCRIPTION, "<b>" + adminAddress + "</b>", "<br/>", BtcCryptocurrency.DepositMinimumConfirmations.ToString());
                    }
                    else
                        depositBTCLabel.Text = "<p class='alert alert-danger'>" + U4200.DEPOSITUNAVAILABLE + "</p>";
                }
            }
            else
                throw new MsgException(U4200.DEPOSITUNAVAILABLE);
        }
        catch (MsgException ex)
        {
            ErrorMessagePanelBTC.Visible = true;
            ErrorMessageBTC.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            ErrorMessagePanelBTC.Visible = true;
            ErrorMessageBTC.Text = ex.Message;
        }
    }

    protected void SetRadioToValues()
    {
        RadioTo.Items.Clear();

        if (!string.IsNullOrEmpty(RadioFrom.SelectedValue))
            RadioTo.Items.AddRange(TransferHelper.GetAvailableListItems(RadioFrom.SelectedValue));

        SetRadioItemsValues(RadioTo);
        RemoveDuplicatesFromList();
    }

    protected void RadioFrom_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (RadioFrom.SelectedValue == "BTC")
            Response.Redirect("/user/deposit.aspx?button=1");

        RemoveDuplicatesFromList();
        SetRadioToValues();
        SetProcessorValues();
        SetupSpecialTransfersProperties();
    }

    protected void SetupSpecialTransfersProperties()
    {
        AdditionalInformationLiteralPlaceHolder.Visible = false;
        PaymentConversionPlaceHolder.Visible = false;
        CalculatePlaceHolder.Visible = false;

        RequestedBTCCryptocurrencyTransfer = false;
        CurrencyTransferSignLiteral.Text = AppSettings.Site.MulticurrencySign;

        TransferFromTextBox.Text = AppSettings.Payments.CurrencyMode == CurrencyMode.Cryptocurrency? "0.1" : "100.00";

        if (RadioFrom.SelectedValue == "Main balance" && RadioTo.SelectedValue == TokenCryptocurrency.Code + " Wallet")
        {
            AdditionalInformationLiteral.Text = string.Format("1 {0} = <b>{1}</b>",
                TokenCryptocurrency.Code, TokenCryptocurrency.GetValue().ToString());

            if (TitanFeatures.IsTrafficThunder)
            {
                AdditionalInformationLiteral.Text = string.Format("1 {0} = <b>${1}</b>",
                    TokenCryptocurrency.Code, TokenCryptocurrency.GetValue().ToShortClearString());
            }

            if (!string.IsNullOrEmpty(AdditionalInformationLiteral.Text))
                AdditionalInformationLiteralPlaceHolder.Visible = true;
        }

        if (RadioTo.SelectedValue == "BTC Wallet" || RadioTo.SelectedValue == "BTC" ||
           RadioFrom.SelectedValue == "CoinPayments" || RadioFrom.SelectedValue == "CoinBase" || RadioFrom.SelectedValue == "Blocktrail")
        {
            if(AppSettings.Site.CurrencyCode != "BTC")
                AdditionalInformationLiteral.Text = string.Format("1 BTC = <b>{0}</b>", BtcCryptocurrency.GetValue().ToString());

            if (TitanFeatures.IsTrafficThunder)
                AdditionalInformationLiteral.Text = "1 BTC = <b>" + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:n2}", CryptocurrencyFactory.Get(CryptocurrencyType.BTC).GetOriginalValue("USD").ToDecimal()) + " USD</b>";

            if (BtcCryptocurrency.DepositEnabled && BtcCryptocurrency.DepositMinimum > 0)
            {
                if (!string.IsNullOrEmpty(AdditionalInformationLiteral.Text))
                    AdditionalInformationLiteral.Text += "<br /><br />";

                AdditionalInformationLiteral.Text += String.Format(U6012.WARNINGMINBTCDEPOSIT, CryptocurrencyMoney.Parse(BtcCryptocurrency.DepositMinimum.ToString(), CryptocurrencyType.BTC).ToString());

                if (RadioFrom.SelectedValue == "CoinPayments")
                    AdditionalInformationLiteral.Text += " " + U6012.BTCDEPOSITLIMITINFO;
            }

            if (!string.IsNullOrEmpty(AdditionalInformationLiteral.Text))
                AdditionalInformationLiteralPlaceHolder.Visible = true;
        }

        if (RadioTo.SelectedValue == TransferOptionConst.PointsTransfer)
        {
            SetupDepositToPoints();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "PointConversion", "pointConversion();", true);
        }

        if (RadioTo.SelectedValue == "BTC Wallet")
        {
            RequestedBTCCryptocurrencyTransfer = true;
            CurrencyTransferSignLiteral.Text = "฿";
            TransferFromTextBox.Text = "0.1";
        }
    }

    private void SetupDepositToPoints()
    {
        PaymentConversionPlaceHolder.Visible = true;
        CalculatePlaceHolder.Visible = true;

        AdditionalInformationLiteral.Text = Points.GetPointsConversionInfo();

        if (!string.IsNullOrEmpty(AdditionalInformationLiteral.Text))
            AdditionalInformationLiteralPlaceHolder.Visible = true;
    }

    public bool RequestedBTCCryptocurrencyTransfer
    {
        get
        {
            if (ViewState["RequestedBTCCryptocurrencyTransfer"] != null)
                return (bool)ViewState["RequestedBTCCryptocurrencyTransfer"];

            return false;
        }
        set
        {
            ViewState["RequestedBTCCryptocurrencyTransfer"] = value;
        }
    }

    protected void RadioTo_SelectedIndexChanged(object sender, EventArgs e)
    {
        RemoveDuplicatesFromList();
        SetupSpecialTransfersProperties();
    }

    protected void PointsTo_SelectedIndexChanged(object sender, EventArgs e)
    {
        RemoveDuplicatesFromList();
    }

    protected void RemoveDuplicatesFromList()
    {
        if (RadioFrom.SelectedItem != null)
            RadioFrom.SelectedItem.Attributes.Add("disabled", "true");

        if (RadioTo.SelectedItem != null)
            RadioTo.SelectedItem.Attributes.Add("disabled", "true");
    }

    private bool ShowPointsView()
    {
        return AppSettings.Points.PointsEnabled && (AppSettings.Payments.PointsToAdBalanceEnabled || AppSettings.Misc.IsTransferPointsToMainBalanceEnabled
            || AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled);
    }

    protected void ChangeAccountButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/user/settings.aspx?ref=payment");
    }

    //For MPesa
    protected void SetProcessorValues()
    {
        AccountInformationDiv.Visible = false;
        string selectedProcessor = RadioFrom.SelectedValue;

        if (selectedProcessor == "MPesa")
        {
            AccountInformationDiv.Visible = true;
            Account.Enabled = true;
            ChangeAccountButton.Visible = false;
            AddNewAccount.Visible = false;
            var userAccountAddress = "";

            PaymentProcessor targetprocessor = PaymentAccountDetails.GetFromStringType(selectedProcessor);
            userAccountAddress = user.GetPaymentAddress(targetprocessor);

            if (!string.IsNullOrWhiteSpace(userAccountAddress))
            {
                Account.Visible = true;
                Account.Enabled = false;
                Account.Text = userAccountAddress;
                ChangeAccountButton.Text = U6007.CHANGE;
                ChangeAccountButton.Visible = true;
            }
            else
            {
                Account.Visible = false;
                AddNewAccount.Text = L1.ADDNEW;
                AddNewAccount.Visible = true;
            }
        }

    }

    private void ShowErrorMessage(string message)
    {
        ErrorMessagePanel.Visible = true;
        ErrorMessage.Text = message;
        MessageUpdatePanel.Update();
    }

    protected void DepositViaRepresentativeButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        string amount = Request.Form["price"].ToString();
        Money Amount;

        try
        {
            Amount = Money.Parse(amount).FromMulticurrency();

            //Anti-Injection Fix
            if (Amount <= new Money(0))
                throw new MsgException(L1.ERROR);

            if (Amount < AppSettings.Payments.MinimumTransferAmount)
                throw new MsgException(U3000.ITSLOWERTHANMINIMUM);

            if (string.IsNullOrEmpty(RepresentativeMessage.Text) || String.IsNullOrWhiteSpace(RepresentativeMessage.Text))
                throw new MsgException(L1.REQ_TEXT);

            string Message = InputChecker.HtmlEncode(RepresentativeMessage.Text, RepresentativeMessage.MaxLength, U5004.MESSAGE);

            var SelectedRepresentative = new Representative(Convert.ToInt32(AvaibleRepresentativeList.SelectedValue));

            if (ConversationMessage.CheckIfThisUserHavePendingActions(user.Id))
                throw new MsgException(U6010.YOUHAVEPENDINGACTION);

            if (Amount > new Member(SelectedRepresentative.UserId).CashBalance)
                throw new MsgException(U6010.REPRESENTATIVENOFUNDS);

            RepresentativesTransferManager representativesTransferManager = new RepresentativesTransferManager(Member.CurrentId, SelectedRepresentative.UserId);
            representativesTransferManager.InvokeDeposit(Amount, Message);

            Response.Redirect("~/user/network/messenger.aspx");
        }
        catch (MsgException ex)
        {
            ShowErrorMessage(ex.Message);
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            ShowErrorMessage(ex.Message);
        }
    }

    protected void AvaibleRepresentativeList_SelectedIndexChanged(object sender, EventArgs e)
    {
        var representative = new Representative(Convert.ToInt32(AvaibleRepresentativeList.SelectedValue));
        var useId = representative.UserId;
        var optionsList = RepresentativesPaymentProcessor.GetAllPaymentOptions(useId);

        DepositOptionsPlaceHolder.Controls.Clear();

        foreach (var option in optionsList)
            if (!string.IsNullOrEmpty(option.DepositInfo))
                DepositOptionsPlaceHolder.Controls.Add(GetPaymentHTML(option.Name, option.LogoPath, option.DepositInfo));
    }

    protected UserControl GetPaymentHTML(string name, string logoPath, string info)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/Representatives/RepresentativePaymentRow.ascx");
        var parsedControl = objControl as IRepresentativePaymentRow;

        parsedControl.ProcessorName = name;
        parsedControl.LogoImagePath = logoPath;
        parsedControl.Info = info;

        return (UserControl)parsedControl;
    }

    protected void TransferViaRepresentativeView_Activate(object sender, EventArgs e)
    {
        var representativesList = Representative.GetAllActiveFromCountry(user.Country);

        if (representativesList.Count > 0)
        {
            NoRepresentativeInfoPlaceHolder.Visible = false;
            RepresentativeInfoContentPlaceHolder.Visible = true;
            AvaibleRepresentativeList.Items.Clear();

            String RadioButtonStyles = "style=\"float: left; padding-left: 5px; padding-right: 5px; \"";
            foreach (var representative in representativesList)
            {
                var item = new ListItem(String.Format("<p {0}>{1}</p> {2}", RadioButtonStyles, representative.Name, HtmlRatingGenerator.GenerateHtmlRating(RatingType.Representative, representative.UserId)), representative.Id.ToString());
                AvaibleRepresentativeList.Items.Add(item);
            }

            flagImage.ImageUrl = string.Format("~/Images/Flags/{0}.png", CountryManager.GetCountryCode(user.Country).ToLower());

            AvaibleRepresentativeList.SelectedIndex = 0;
            AvaibleRepresentativeList_SelectedIndexChanged(null, null);
        }
    }

    public void MPesaConfirmButton_Click(object sender, EventArgs e)
    {
        try
        {
            SuccessModal.Visible = MPesaSapamaCode.TryValidate(Member.CurrentName, MPesaCodeTextBox.Text, "254" + MPesaPhoneTextBox.Text);
        }
        catch (MsgException ex)
        {
            MPesaErrorPanel.Visible = true;
            MPesaErrorLiteral.Text = ex.Message;
        }
    }
}