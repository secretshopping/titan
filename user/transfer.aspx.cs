using System;
using System.Web.UI.WebControls;
using Resources;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using MarchewkaOne.Titan.Balances;
using Titan;
using SocialNetwork;
using System.Web.UI;
using Titan.Cryptocurrencies;

public partial class About : System.Web.UI.Page
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

        PointsButton.Text = AppSettings.PointsName;
        PointsButton.Visible = ShowPointsView();
        PointsToSpinsButton.Visible = AppSettings.TitanFeatures.SlotMachineEnabled;
        RofriquePlaceHolder.Visible = TitanFeatures.IsRofrique;

        if (AppSettings.Payments.TransferMode != TransferFundsMode.DenyAll)
        {
            TransferToOthersButton.Visible = true;
            TransferToOthersButton.Text = U3500.TRANSFERTOOTHER;
        }

        if (!Page.IsPostBack)
        {
            if (TitanFeatures.IsJ5WalterOffersFromHome)
            {
                BalanceButton.Visible = false;
                PointsButton.CssClass = "ViewSelected";
                MenuMultiView.ActiveViewIndex = 1;
            }

            if (TitanFeatures.isBoazorSite)
            {
                BalanceButton.Visible = false;
                TransferToOthersButton.Visible = false;
                MenuMultiView.ActiveViewIndex = 1;
            }
            

            if (TitanFeatures.IsTrafficThunder)
                UserBalancesPlaceHolder.Visible = false;
            
            AppSettings.Reload();
            
            LangAdder.Add(btnTransfer, L1.TRANSFER);
            LangAdder.Add(btnTransferPoints, L1.TRANSFER);
            LangAdder.Add(MPesaConfirmButton, L1.CONFIRM);

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

            RadioFrom.Items.AddRange(GenerateHTMLButtons.TransferBalanceFromItems);
            
            //Points -> Main Balance (on/off)
            if (!AppSettings.Misc.IsTransferPointsToMainBalanceEnabled || !AppSettings.Points.PointsEnabled)
                PointsTo.Items.RemoveAt(2);

            if (!AppSettings.Payments.PointsToAdBalanceEnabled)
                PointsTo.Items.Remove("Purchase balance");

            if (AppSettings.Payments.TransferMode == TransferFundsMode.AllowMainBalanceOnly
                || AppSettings.Payments.TransferMode == TransferFundsMode.AllowMainToPurchaseBalance)
            {
                MemberFrom.Items[1].Selected = true;
                MemberFrom.Items.RemoveAt(0);
            }
            else if (AppSettings.Payments.TransferMode == TransferFundsMode.AllowPointsOnly)
                MemberFrom.Items.RemoveAt(1);

            if (((AppSettings.Payments.CommissionToMainBalanceEnabled && !TitanFeatures.UserCommissionToMainBalanceEnabled)
                || (TitanFeatures.UserCommissionToMainBalanceEnabled && user.CommissionToMainBalanceEnabled)
                || TitanFeatures.IsRevolca) && user.CheckAccessCustomizeTradeOwnSystem)
            {
                RadioTo.Items.Add(new ListItem("", "Main balance"));
                TransferSameCommissionToMainLiteral.Visible = true;
                TransferSameCommissionToMainLiteral.Text = U5004.TRANSFERCOMMISSIONTOMAINFEE + ": <b>"
                    + user.Membership.SameUserCommissionToMainTransferFee.ToString() + "%</b><br />";

                if (user.Membership.SameUserCommissionToMainTransferFee == 0)
                    TransferSameCommissionToMainP.Visible = false;
            }

            if (AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled == false)
            {
                ListItem a = PointsTo.Items.FindByValue("Traffic balance");
                ListItem b = RadioTo.Items.FindByValue("Traffic balance");
                if (a != null)
                    PointsTo.Items.Remove(a);
                if (b != null)
                    RadioTo.Items.Remove(b);
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

            #region TransferFee
            var pointsFee = user.Membership.OtherUserPointsToPointsTransferFee;
            var mainToMainFee = user.Membership.OtherUserMainToMainTransferFee;
            var mainToAdFee = user.Membership.OtherUserMainToAdTransferFee;

            if (AppSettings.Payments.TransferMode == TransferFundsMode.AllowPointsAndMainBalance)
            {
                TransferOthersMainToMainLiteral.Visible = true;
                TransferOthersMainToMainLiteral.Text = U5004.THEFEE.Replace("%a%", "<b>" + L1.MAINBALANCE + "</b>")
                    .Replace("%n%", "<b>" + mainToMainFee.ToString() + "%</b>");

                TransferOthersPointsLiteral.Visible = true;
                TransferOthersPointsLiteral.Text = U5004.THEFEE.Replace("%a%", "<b>" + AppSettings.PointsName + "</b>")
                        .Replace("%n%", "<b>" + pointsFee.ToString() + "%</b>");
            }
            else if (AppSettings.Payments.TransferMode == TransferFundsMode.AllowMainBalanceOnly)
            {
                TransferOthersMainToMainLiteral.Visible = true;
                TransferOthersMainToMainLiteral.Text = U5004.THEFEE.Replace("%a%", "<b>" + L1.MAINBALANCE + "</b>")
                    .Replace("%n%", "<b>" + mainToMainFee.ToString() + "%</b>");
            }
            else if (AppSettings.Payments.TransferMode == TransferFundsMode.AllowPointsOnly)
            {
                TransferOthersPointsLiteral.Visible = true;
                TransferOthersPointsLiteral.Text = U5004.THEFEE.Replace("%a%", "<b>" + AppSettings.PointsName + "</b>")
                        .Replace("%n%", "<b>" + pointsFee.ToString() + "%</b>");
            }
            else if (AppSettings.Payments.TransferMode == TransferFundsMode.AllowMainToPurchaseBalance)
            {
                TransferOthersMainToAdLiteral.Visible = true;
                TransferOthersMainToAdLiteral.Text = U5004.THEFEE.Replace("%a%", "<b>" + L1.MAINBALANCE + "</b>")
                    .Replace("%n%", "<b>" + mainToAdFee.ToString() + "%</b>");
            }
            #endregion

            SetRadioToValues();
            //CalculatePointsValue();
            SetProcessorValues();
            SetupSpecialTransfersProperties();

            if (RadioFrom.Items.Count == 0 || RadioTo.Items.Count == 0)
            {
                //No transfers available
                transferInputRow.Visible = false;
                TransferSameCommissionToMainLiteral.Visible = true;
                TransferSameCommissionToMainLiteral.Text = "<br/><br/>" + U5006.NOTRANSFEROPTIONS;
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
        }

        SetRadioItemsValues(RadioFrom);
        SetRadioItemsValues(RadioTo);
        SetRadioItemsValues(PointsTo);

        RemoveDuplicatesFromList();
        
        PointsFrom.Items[0].Attributes.Add("data-content", "<img src = '../Images/OneSite/TransferMoney/Points.png' /> " + AppSettings.PointsName);

        if (!TitanFeatures.IsRofriqueWorkMines)
        {
            if(TitanFeatures.IsAhmed)
                LangAdder.Add(BalanceButton, string.Format("{0}/{1}", U6012.DEPOSITCOINS, L1.TRANSFER));
            else
                LangAdder.Add(BalanceButton, L1.TRANSFER);
        }
        else
            LangAdder.Add(BalanceButton, "Cash Deposits");

        LangAdder.Add(btnTransferMember, L1.TRANSFER);
        LangAdder.Add(CalculatePointsValueButton, U6007.CALCULATE);
    }

    private void SetRadioItemsValues(DropDownList dropDownList)
    {
        var TokenCryptocurrency = CryptocurrencyFactory.Get<ERC20TokenCryptocurrency>();

        foreach (ListItem item in dropDownList.Items)
        {
            item.Text = item.Value;

            string ImageIconName = String.Format("../Images/OneSite/TransferMoney/{0}.png", item.Value);

            if (item.Value == (String.Format("{0} Wallet", TokenCryptocurrency.Code)))
                ImageIconName = AppSettings.Ethereum.ERC20TokenImageUrl;

            if (TitanFeatures.IsClickmyad && item.Text == CryptocurrencyAPIProvider.CoinPayments.ToString())
                item.Attributes.Add("data-content", String.Format("<img src='{0}' />", ImageIconName));
            else
                item.Attributes.Add("data-content", String.Format("<img src='{0}' style='height:40px' /> {1}", ImageIconName, item.Text));
        }
    }

    protected void CalculatePointsValue()
    {
        try
        {
            int points = Int32.Parse(SliderSinglePointsTextBox.Text);
            Money Amount = PointsConverter.ToMoney(points);

            PointsNumberLabel.Text = points.ToString();
            PointsValueLabel.Text = Amount.ToString();
        }
        catch (Exception ex) { }
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

        string amount = TransferFromTextBox.Text.Trim().Replace(",",".");
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
                    Response.Redirect("~/user/transfer.aspx?mpesa=" + Amount.ToClearString());

                PaymentAmountLabel.Text = L1.AMOUNT + ": <b>" + Amount.ToString() + "</b>";

                if (RadioFrom.SelectedValue == BtcCryptocurrency.WithdrawalApiProcessor.ToString())
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

    protected void btnTransferPoints_Click(object sender, EventArgs e)
    {
        string amount = SliderSinglePointsTextBox.Text;
        int Points;
        try
        {
            AppSettings.DemoCheck();

            Points = Int32.Parse(amount);
            Member User = Member.Current;

            Money Amount = Titan.PointsConverter.ToMoney(Points);

            //Anti-Injection Fix
            if (Amount <= new Money(0))
                throw new MsgException(L1.ERROR);

            if (PointsFrom.SelectedValue == "Points")
            {
                if (Points > User.PointsBalance)
                    throw new MsgException(L1.NOTENOUGHFUNDS);

                User.SubtractFromPointsBalance(Points, "Transfer from " + AppSettings.PointsName + " balance", BalanceLogType.Other, setMinPoints: false);

                if (PointsTo.SelectedValue == "Traffic balance")
                    User.AddToTrafficBalance(Amount, "Transfer from  " + AppSettings.PointsName + "  balance");
                else if (PointsTo.SelectedValue == "Purchase balance")
                    User.AddToPurchaseBalance(Amount, "Transfer from  " + AppSettings.PointsName + "  balance");
                else if (PointsTo.SelectedValue == "Main balance")
                {
                    User.AddToMainBalance(Amount, "Transfer from  " + AppSettings.PointsName + "  balance");
                    User.TotalEarned += Amount;
                }

                else throw new MsgException("You must select your target account");

                User.SaveBalances();
                Response.Redirect("../status.aspx?type=transferok");
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

    protected void btnTransferSpins_Click(object sender, EventArgs e)
    {
        string amount = Request.Form["pricePoints"].ToString();
        int Points;
        try
        {
            AppSettings.DemoCheck();

            Points = Int32.Parse(amount);
            Member User = Member.Current;

            int spins = Points / 10;

            //Anti-Injection Fix
            if (spins <= 0)
                throw new MsgException(L1.ERROR);

            if (PointsFrom.SelectedValue == "Points")
            {
                if (Points > User.PointsBalance)
                    throw new MsgException(L1.NOTENOUGHFUNDS);

                User.SubtractFromPointsBalance(Points, "Transfer from " + AppSettings.PointsName + " balance", BalanceLogType.Other, setMinPoints: false);
                User.SlotMachineChances += spins;
                User.Save();
                Response.Redirect("../status.aspx?type=transferok");
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

    protected void btnTransferMember_Click(object sender, EventArgs e)
    {
        try
        {
            AppSettings.DemoCheck();

            if (AppSettings.Payments.TransferMode == TransferFundsMode.DenyAll)
                throw new MsgException("Forbidden");

            Member TransferTo = new Member(MemberTo.Text);
            TransferManager TransferHandler = new TransferManager(Member.Current, TransferTo);

            if (MemberFrom.SelectedValue == "Points")
            {
                int Points = Int32.Parse(MemberHowMuch.Text);
                TransferHandler.TryTransfer(Points);
            }
            else if (MemberFrom.SelectedValue == "Main Balance")
            {
                Money Amount = Money.Parse(MemberHowMuch.Text.Replace(",", "."));
                TransferHandler.TryTransfer(Amount);
            }

            Response.Redirect("../status.aspx?type=transferok");

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
        RemoveDuplicatesFromList();
        SetRadioToValues();
        SetProcessorValues();
        SetupSpecialTransfersProperties();
    }

    protected void SetupSpecialTransfersProperties()
    {
        AdditionalInformationLiteralPlaceHolder.Visible = false;
        RequestedBTCCryptocurrencyTransfer = false;
        CurrencyTransferSignLiteral.Text = AppSettings.Site.MulticurrencySign;
        TransferFromTextBox.Text = "100.00";

        if (RadioFrom.SelectedValue == "Main balance" && RadioTo.SelectedValue == TokenCryptocurrency.Code + " Wallet")
        {
            AdditionalInformationLiteralPlaceHolder.Visible = true;
            AdditionalInformationLiteral.Text = string.Format("1 {0} = <b>{1}</b>",
                TokenCryptocurrency.Code, TokenCryptocurrency.GetValue().ToString());

            if (TitanFeatures.IsTrafficThunder)
            {
                AdditionalInformationLiteral.Text = string.Format("1 {0} = <b>${1}</b>",
                    TokenCryptocurrency.Code, TokenCryptocurrency.GetValue().ToShortClearString());
            }
        }

        if(RadioTo.SelectedValue == "BTC Wallet" || RadioTo.SelectedValue == "BTC" || 
           RadioFrom.SelectedValue == "CoinPayments" || RadioFrom.SelectedValue == "CoinBase" || RadioFrom.SelectedValue == "Blocktrail")
        {
            AdditionalInformationLiteralPlaceHolder.Visible = true;
            AdditionalInformationLiteral.Text = string.Format("1 BTC = <b>{0}</b>", CryptocurrencyFactory.Get(CryptocurrencyType.BTC).GetValue().ToString());

            if(BtcCryptocurrency.DepositEnabled && BtcCryptocurrency.DepositMinimum > 0)
                AdditionalInformationLiteral.Text += "<br /><br />" + String.Format(U6012.WARNINGMINBTCDEPOSIT, CryptocurrencyMoney.Parse(BtcCryptocurrency.DepositMinimum.ToString(), CryptocurrencyType.BTC).ToString());

            if (RadioFrom.SelectedValue == "CoinPayments")
                AdditionalInformationLiteral.Text += " " + U6012.BTCDEPOSITLIMITINFO;
        }

        if (RadioTo.SelectedValue == "BTC Wallet")
        {
            RequestedBTCCryptocurrencyTransfer = true;
            CurrencyTransferSignLiteral.Text = "฿";
            TransferFromTextBox.Text = "0.1";
        }
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

        if (PointsTo.SelectedItem != null)
            PointsTo.SelectedItem.Attributes.Add("disabled", "true");
    }

    private bool ShowPointsView()
    {
        return AppSettings.Points.PointsEnabled && (AppSettings.Payments.PointsToAdBalanceEnabled || AppSettings.Misc.IsTransferPointsToMainBalanceEnabled
            || AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled);
    }

    protected void CalculatePointsValueButton_Click(object sender, EventArgs e)
    {
        CalculatePointsValue();
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

    protected UserControl GetPaymentHTML(string name, string logoPath, string info)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/Representatives/RepresentativePaymentRow.ascx");
        var parsedControl = objControl as IRepresentativePaymentRow;

        parsedControl.ProcessorName = name;
        parsedControl.LogoImagePath = logoPath;
        parsedControl.Info = info;

        return (UserControl)parsedControl;
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
