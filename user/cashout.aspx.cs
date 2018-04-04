using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Prem.PTC.Utils;
using SocialNetwork;
using Titan.Cryptocurrencies;

public partial class About : System.Web.UI.Page
{
    Member User;
    CryptocurrencyWithdrawalAddress cryptocurrencyAddress;

    public CryptocurrencyType SelectedCryptocurrency
    {
        get
        {
            if (ViewState["SelectedCryptocurrency"] != null)
                return (CryptocurrencyType)ViewState["SelectedCryptocurrency"];

            return CryptocurrencyType.BTC;
        }
        set
        {
            ViewState["SelectedCryptocurrency"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.MoneyPayoutEnabled);
        User = Member.Current;

        if (CreditLineManager.UserHasUnpaidLoans(User.Id))
        {
            CashoutButton.Enabled = CashoutButtonConfirm.Enabled =
            WithdrawCryptocurrencyButton.Enabled = WithdrawCryptocurrencyConfirmButton.Enabled =
            CommissionCashoutButton.Enabled = false;
            UnpaidCreditLineInfo.Visible = true;
        }

        if (!Page.IsPostBack)
        {
            AppSettings.Reload();

            #region Langs & Texts

            WithdrawCryptocurrencyButton.Text = L1.CASHOUT;
            WithdrawCryptocurrencyConfirmButton.Text = L1.CONFIRM;
            CashoutButtonConfirm.Text = L1.CONFIRM;
            SendWithdrawViaRepresentativeButton.Text = U6010.SENDTRANSFERMESSAGE;
            ProportionsGridView.EmptyDataText = U5004.NOPAYOUTHISTORY;
            MainBalanceButton.Text = L1.MAINBALANCE;
            MaxWithdrawalsButton.Text = U5004.MAXIMUMWITHDRAWAL;
            CommissionButton.Text = U5004.COMMISSIONBALANCE;
            WithdrawHistoryButton.Text = L1.HISTORY;
            CashoutButton.Text = CommissionCashoutButton.Text = L1.CASHOUT;
            MainBalanceLiteral.Text = User.MainBalance.ToString();
            CommissionBalanceLiteral.Text = User.CommissionBalance.ToString();
            WithdrawHistoryGridView.EmptyDataText = L1.NODATA;

            LangAdder.Add(RegularExpressionValidator3, L1.ER_BADPIN, true);
            LangAdder.Add(RegularExpressionValidator1, L1.ER_BADPIN, true);
            LangAdder.Add(RegularExpressionValidator2, L1.ER_BADPIN, true);
            LangAdder.Add(WithdrawViaRepresentativeButton, U6010.WITHDRAWVIAREPRESENTATIVE);
            LangAdder.Add(RequiredFieldValidator2, L1.REG_REQ_PIN, true);
            LangAdder.Add(RequiredFieldValidator4, L1.REG_REQ_PIN, true);
            LangAdder.Add(RequiredFieldValidator6, L1.REG_REQ_PIN, true);
            LangAdder.Add(RequiredFieldValidator1, L1.REQ_PP, true);
            LangAdder.Add(RequiredFieldValidator3, L1.REQ_PP, true);
            LangAdder.Add(RequiredFieldValidator7, U2502.INVALIDMONEYFORMAT, true);
            LangAdder.Add(BtcCodeValidator, U6000.INVALIDSECURITYCODE, true);
            LangAdder.Add(FiatCodeValidator, U6000.INVALIDSECURITYCODE, true);
            LangAdder.Add(REValidator, U3500.ILLEGALCHARS);
            #endregion

            PINDiv1.Visible = PINDiv2.Visible = BtcPinDiv.Visible = AppSettings.Registration.IsPINEnabled;

            var BtcCryptocurrency = CryptocurrencyFactory.Get<BitcoinCryptocurrency>();
            var XrpCryptocurrency = CryptocurrencyFactory.Get<RippleCryptocurrency>();
            var TokenCryptocurrency = CryptocurrencyFactory.Get<ERC20TokenCryptocurrency>();

            //Check if user has some manual payouts waiting
            var UnpaidRequests = User.GetUnpaidPayoutRequests();
            WarningPanel.Visible = UnpaidRequests.exists;
            WarningLiteral.Text = UnpaidRequests.text;

            //Generate proper Cashout options
            RadioFrom.Items.AddRange(GenerateHTMLButtons.CashoutFromItems);

            if (TitanFeatures.IsRofriqueWorkMines && !CryptocurrencyApiFactory.GetDepositApi(BtcCryptocurrency).AllowToUsePaymentButtons())
                RadioFrom.Items.Add(new ListItem("", "BTC"));

            CommissionRadioFrom.Items.AddRange(GenerateHTMLButtons.CashoutFromItems);

            if (RadioFrom.Items.Count < 1)
            {
                PayoutPlaceHolder.Visible = false;
                WarningPanel.Visible = true;
                WarningLiteral.Text = U5006.PAYOUTUNAVAILABLE;
            }

            if (CommissionRadioFrom.Items.Count < 1)
            {
                CommissionPayoutPlaceHolder.Visible = false;
                CommissionWarningPanel.Visible = true;
                CommissionWarningLiteral.Text = U5006.PAYOUTUNAVAILABLE;
            }

            CommissionButton.Visible = AppSettings.Payments.CommissionBalanceWithdrawalEnabled;
            WithdrawViaRepresentativeButton.Visible = AppSettings.Representatives.RepresentativesHelpWithdrawalEnabled;

            //Lang
            CashoutButton.Text = CommissionCashoutButton.Text = L1.CASHOUT;

            LangAdder.Add(RegularExpressionValidator3, L1.ER_BADPIN, true);
            LangAdder.Add(RegularExpressionValidator1, L1.ER_BADPIN, true);
            LangAdder.Add(RegularExpressionValidator2, L1.ER_BADPIN, true);
            LangAdder.Add(WithdrawViaRepresentativeButton, U6010.WITHDRAWVIAREPRESENTATIVE);
            LangAdder.Add(RequiredFieldValidator2, L1.REG_REQ_PIN, true);
            LangAdder.Add(RequiredFieldValidator4, L1.REG_REQ_PIN, true);
            LangAdder.Add(RequiredFieldValidator6, L1.REG_REQ_PIN, true);
            LangAdder.Add(RequiredFieldValidator1, L1.REQ_PP, true);
            LangAdder.Add(RequiredFieldValidator3, L1.REQ_PP, true);
            LangAdder.Add(RequiredFieldValidator7, U2502.INVALIDMONEYFORMAT, true);
            LangAdder.Add(BtcCodeValidator, U6000.INVALIDSECURITYCODE, true);
            LangAdder.Add(FiatCodeValidator, U6000.INVALIDSECURITYCODE, true);

            //Pre-selected tab
            if (Request.QueryString["b"] != null)
            {
                var button = (Button)MenuButtonPlaceHolder.FindControl(Request.QueryString["b"] + "Button");
                MenuButton_Click(button, null);
            }

            bool PercentOfInvestmentWIthdrawalEnabled = User.Membership.MaxWithdrawalAllowedPerInvestmentPercent < 1000000000;

            MaxWithdrawalsButton.Visible = AppSettings.Payments.ProportionalPayoutLimitsEnabled || PercentOfInvestmentWIthdrawalEnabled;

            if (PercentOfInvestmentWIthdrawalEnabled)
            {
                MaxWithdrawalAllowedPerInvestmentPercentPlaceHolder.Visible = true;
                PaymentProportionsManager ppm = new PaymentProportionsManager(User);
                Money invested = ppm.TotalPaidIn;
                Money withdrawn = ppm.TotalPaidOut;
                Money canwithdraw = Money.MultiplyPercent(invested, User.Membership.MaxWithdrawalAllowedPerInvestmentPercent);
                TotalPaidInLiteral.Text = invested.ToString();
                TotalCashoutLiteral.Text = withdrawn.ToString();
                HowmuchMoreCanBeWithdrawnLiteral.Text = String.Format(U6005.YOUCANWITHDRAWOFINVESTED,
                    NumberUtils.FormatPercents(User.Membership.MaxWithdrawalAllowedPerInvestmentPercent), canwithdraw.ToString(),
                    "<b>" + (canwithdraw - withdrawn).ToString() + "</b>");
            }

            //Pre-selected tab
            if (Request.QueryString["b"] != null)
            {
                var button = (Button)MenuButtonPlaceHolder.FindControl(Request.QueryString["b"] + "Button");
                MenuButton_Click(button, null);
            }

            BTCWithdrawalButton.Visible = BtcCryptocurrency.WithdrawalEnabled;
            XRPWithdrawalButton.Visible = XrpCryptocurrency.WithdrawalEnabled;
            ERC20TokenButton.Visible = TokenCryptocurrency.WithdrawalEnabled;

            BTCWithdrawalButton.Text = "BTC";
            ERC20TokenButton.Text = String.Format("{0} ({1})", TokenCryptocurrency.Name, TokenCryptocurrency.Code);
            XRPWithdrawalButton.Text = "XRP";

            if (RadioFrom.Items.Count < 1)//No payment processors, let's move to BTC/XRP/ETH tab on start            
            {
                MainBalanceButton.Visible = false;

                if (BtcCryptocurrency.WithdrawalEnabled)
                    MenuButton_Click(BTCWithdrawalButton, null);
                else if (XrpCryptocurrency.WithdrawalEnabled)
                    MenuButton_Click(XRPWithdrawalButton, null);
                else if (TokenCryptocurrency.WithdrawalEnabled)
                    MenuButton_Click(ERC20TokenButton, null);
                else
                    MainBalanceButton.Visible = true;

                if (TitanFeatures.IsClickmyad)
                    MenuButton_Click(XRPWithdrawalButton, null);
            }

            RadioFrom.SelectedIndex = 0;
            SetProcessorValues();
        }

        SetDropdownItemsValues(RadioFrom);

        if (TitanFeatures.IsClickmyad)
            BTCWithdrawalButton.Visible = false;

        if (TitanFeatures.IsTrafficThunder)
        {
            MainBalanceButton.Visible = false;
            MaxWithdrawalsButton.Visible = false;
        }
    }

    private void SetDropdownItemsValues(DropDownList dropDownList)
    {
        if (dropDownList.Items.Count > 0)
        {
            foreach (ListItem item in dropDownList.Items)
            {
                if (item.Value.ToNullableInt() != null)
                {
                    item.Attributes.Add("data-content", String.Format("{0}", item.Text));
                }
                else
                {
                    item.Text = item.Value;
                    item.Attributes.Add("data-content", String.Format("<img src='../Images/OneSite/TransferMoney/{0}.png' /> {1}",
                        item.Value, item.Text));
                }

            }
            RadioFrom.SelectedItem.Attributes.Add("disabled", "true");
        }
    }

    private PayoutRequest _editedPayoutRequest;
    private PayoutRequest EditedPayoutRequest
    {
        get
        {
            if (_editedPayoutRequest == null)
            {
                if (Session["EditedPayoutRequest"] is PayoutRequest)
                    _editedPayoutRequest = Session["EditedPayoutRequest"] as PayoutRequest;
                else
                    throw new InvalidOperationException("No payout request set");
            }

            return _editedPayoutRequest;
        }
        set { Session["EditedPayoutRequest"] = _editedPayoutRequest = value; }
    }

    protected void BtcCodeValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = VerificationCode.IsCodeValid(Member.CurrentId, BtcConfirmationCodeTextBox.Text);
    }

    protected void FiatCodeValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = VerificationCode.IsCodeValid(Member.CurrentId, FiatConfirmationCodeTextBox.Text);
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        MenuMultiView.ActiveViewIndex = 0;
        SelectedCryptocurrency = CryptocurrencyType.BTC;

        if (TheButton == XRPWithdrawalButton)
            SelectedCryptocurrency = CryptocurrencyType.XRP;

        if (TheButton == ERC20TokenButton)
            SelectedCryptocurrency = CryptocurrencyType.ERC20Token;

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            if (b.Visible)
                b.CssClass = "";
        }

        TheButton.CssClass = "ViewSelected";
    }

    protected void CashoutButton_Click(object sender, EventArgs e)
    {
        SuccMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;

        Member User = Member.Current;

        if (Page.IsValid)
        {
            try
            {
                AppSettings.DemoCheck();

                User.ValidatePIN(PIN.Text);

                if (AppSettings.Payments.WithdrawalEmailEnabled)
                {
                    VerificationCode.Create(User.Id, Mailer.SendCodeVerificationEmail(Member.CurrentInCache.Email));
                    ConfirmationCodePlaceHolder2.Visible = true;
                    CashoutButtonConfirm.Visible = true;
                    SuccMessagePanel.Visible = true;
                    SuccMessage.Text = string.Format(U6000.VERIFICATIONEMAILSENT, AppSettings.Payments.WithdrawalVerificationCodeValidForMinutes);
                    CashoutButton.Visible = false;
                    PIN.Enabled = false;
                    AmountToCashoutTextBox.Enabled = false;
                    Account.Enabled = false;
                }
                else if ((AppSettings.Proxy.SMSType == ProxySMSType.FirstCashout && !User.IsPhoneVerified) ||
                         AppSettings.Proxy.SMSType == ProxySMSType.EveryCashout)
                {
                    if (!User.IsPhoneVerified)
                    {
                        User.RequireSMSPin();

                        System.Web.Security.FormsAuthentication.SignOut();
                        HttpContext.Current.Session.Abandon();
                    }
                    else
                    {
                        if (VerificationCode.ActiveCodeForUserExists(User.Id))
                        {
                            SuccMessage.Text = L1.ALREADYSENT + "(+" + User.PhoneCountryCode + " " + User.PhoneNumber +
                                               ")";
                        }
                        else
                        {
                            VerificationCode.Create(User.Id,
                                Convert.ToInt32(ProxStop.SendSMSWithPIN(User.PhoneCountryCode, User.PhoneNumber)));
                            SuccMessage.Text = string.Format(U6000.VERIFICATIONSMSSENT,
                                AppSettings.Payments.WithdrawalVerificationCodeValidForMinutes);
                            User.UnconfirmedSMSSent++;
                        }
                        ConfirmationCodePlaceHolder2.Visible = true;
                        CashoutButtonConfirm.Visible = true;
                        SuccMessagePanel.Visible = true;
                        User.IsPhoneVerifiedBeforeCashout = false;
                        User.Save();
                    }
                    CashoutButton.Visible = false;
                    PIN.Enabled = false;
                    AmountToCashoutTextBox.Enabled = false;
                    Account.Enabled = false;
                }
                else
                {
                    CashoutButtonConfirm_Click(this, null);
                }
            }
            catch (Exception ex)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
        }
    }

    protected void CashoutButtonConfirm_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            SuccMessagePanel.Visible = false;
            ErrorMessagePanel.Visible = false;
            CashoutButtonConfirm.Visible = false;

            try
            {

                //Parse amount
                Money Transferred;
                try
                {
                    Transferred = Money.Parse(AmountToCashoutTextBox.Text).FromMulticurrency();
                    Transferred = Money.Parse(Transferred.ToShortClearString());
                }
                catch (Exception ex)
                {
                    throw new MsgException(U2502.INVALIDMONEYFORMAT);
                }

                if (!AppSettings.Payments.WithdrawalEmailEnabled && AppSettings.Proxy.SMSType == ProxySMSType.EveryCashout)
                {
                    User.IsPhoneVerifiedBeforeCashout = true;
                    User.UnconfirmedSMSSent--;
                    User.Save();
                }

                //Lets process to cashout
                PayoutManager Manager = new PayoutManager(User, Transferred, RadioFrom.SelectedValue,
                    CustomPayoutProcessor.IsCustomPayoutProcessor(RadioFrom.SelectedValue), CustomPayoutProcessor.GetCustomPayoutProcessorId(RadioFrom.SelectedValue), Account.Text);

                SuccMessage.Text = Manager.TryMakePayout();
                SuccMessagePanel.Visible = true;
            }
            catch (Exception ex)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
            finally
            {
                Account.Enabled = true;
                PIN.Enabled = true;
                AmountToCashoutTextBox.Enabled = true;
                CashoutButton.Visible = true;
                ConfirmationCodePlaceHolder2.Visible = false;
                CashoutButtonConfirm.Visible = false;
            }
        }
    }

    protected void WithdrawCryptocurrencyButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            CryptocurrencySuccessMessagePanel.Visible = false;
            CryptocurrencyErrorMessagePanel.Visible = false;

            try
            {
                var Cryptocurrency = CryptocurrencyFactory.Get(SelectedCryptocurrency);

                User.ValidatePIN(CryptoPINTextBox.Text);

                string address = TryGetWithdrawalAddress(Cryptocurrency);

                Money moneyAmount = Money.Zero;
                Money moneyFee = Money.Zero;
                Money totalAmount = Money.Zero;
                decimal amountInCryptocurrency = Decimal.Zero;

                if (Cryptocurrency.WithdrawalFeePolicy == WithdrawalFeePolicy.Packs)
                {
                    WithdrawalPacksPlaceHolder.Visible = true;
                    WithdrawalPacksLiteral.Text = BitcoinWithdrawalFeePacks.GetPacksText(User.Id);
                }

                moneyAmount = new Money(Convert.ToDecimal(WithdrawCryptocurrencyAmountTextBox.Text)).FromMulticurrency();
                moneyFee = Cryptocurrency.EstimatedWithdrawalFee(amountInCryptocurrency, address, User.Id, Cryptocurrency.WithdrawalSource);
                totalAmount = moneyAmount - moneyFee;
                amountInCryptocurrency = Cryptocurrency.ConvertFromMoney(moneyAmount);

                if (Cryptocurrency.WithdrawalSource == WithdrawalSourceBalance.Wallet)
                {
                    amountInCryptocurrency = Convert.ToDecimal(WithdrawCryptocurrencyAmountTextBox.Text);
                    moneyAmount = new CryptocurrencyMoney(Cryptocurrency.Type, amountInCryptocurrency);
                    moneyFee = Cryptocurrency.EstimatedWithdrawalFee(amountInCryptocurrency, address, User.Id, Cryptocurrency.WithdrawalSource);
                    totalAmount = new CryptocurrencyMoney(SelectedCryptocurrency, moneyAmount.ToDecimal() - moneyFee.ToDecimal());
                }

                TryValidateCryptocurrencyWithdrawalByEmailOrPhone();

                CryptocurrencyFeeLiteral.Visible = WithdrawTotalCryptocurrencyLiteral.Visible = true;
                CryptocurrencyFeeLiteral.Text = "</br>" + U3500.CASHOUT_FEES + ": " + moneyFee.ToString() + "</br>";

                WithdrawTotalCryptocurrencyLiteral.Text = "<b>" + U5001.TOTAL + ": " + totalAmount + "</b>";
                WithdrawCryptocurrencyButton.Visible = false;
                WithdrawCryptocurrencyConfirmButton.Visible = true;
                WithdrawCryptocurrencyAmountTextBox.Enabled = false;
                CryptoPINTextBox.Enabled = false;
            }
            catch (Exception ex)
            {
                CryptocurrencyErrorMessagePanel.Visible = true;
                CryptocurrencyErrorMessageLiteral.Text = ex.Message;
            }
        }
    }

    protected void WithdrawCryptocurrencyConfirmButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            CryptocurrencySuccessMessagePanel.Visible = false;
            CryptocurrencyErrorMessagePanel.Visible = false;
            WithdrawCryptocurrencyConfirmButton.Visible = false;

            try
            {
                var Cryptocurrency = CryptocurrencyFactory.Get(SelectedCryptocurrency);
                string address = TryGetWithdrawalAddress(Cryptocurrency);
                User = Member.Current;

                decimal amount = Convert.ToDecimal(WithdrawCryptocurrencyAmountTextBox.Text);

                string successMessage = Cryptocurrency.TryMakeWithdrawal(User.Id, address, amount, Cryptocurrency.WithdrawalSource);

                CryptocurrencySuccessMessagePanel.Visible = true;
                CryptocurrencySuccessMessageLiteral.Text = successMessage;
                WithdrawCryptocurrencyAmountTextBox.Text = string.Empty;

                CryptocurrencyFeeLiteral.Visible = WithdrawTotalCryptocurrencyLiteral.Visible = false;

                if (!AppSettings.Payments.WithdrawalEmailEnabled && AppSettings.Proxy.SMSType == ProxySMSType.EveryCashout)
                {
                    User.IsPhoneVerifiedBeforeCashout = true;
                    User.UnconfirmedSMSSent--;
                    User.Save();
                }
            }
            catch (Exception ex)
            {
                CryptocurrencyErrorMessagePanel.Visible = true;
                CryptocurrencyErrorMessageLiteral.Text = ex.Message;
            }
            finally
            {
                WithdrawCryptocurrencyAmountTextBox.Enabled = true;
                WithdrawCryptocurrencyButton.Visible = true;
                ConfirmationCodePlaceHolder.Visible = false;
            }
        }
    }

    protected string TryGetWithdrawalAddress(Cryptocurrency cryptocurrency)
    {
        string address = String.Empty;

        if (cryptocurrency.WithdrawalApiProcessor == CryptocurrencyAPIProvider.Coinbase)
        {
            CoinbaseAddressesDDL.Enabled = false;
            if (AppSettings.Cryptocurrencies.CoinbaseAddressesPolicy == CoinbaseAddressesPolicy.CoinbaseEmailOrBTCWallet)
            {
                if (CoinbaseAddressesDDL.SelectedValue == CoinbaseAddressesPolicy.BTCWallet.ToString())
                    address = CoinbaseAddressHelper.TryToGetAndUseAddress(User.Id, 2);
                else
                    address = CoinbaseAddressHelper.TryToGetAndUseAddress(User.Id, 1);
            }
            else
            {
                address = CoinbaseAddressHelper.TryToGetAndUseAddress(User.Id);
            }
        }
        else
        {
            cryptocurrencyAddress = CryptocurrencyWithdrawalAddress.GetAddress(Member.CurrentId, SelectedCryptocurrency);

            if (cryptocurrencyAddress == null)
                throw new MsgException(U6000.ADDBTCADDRESSFIRST);

            if (!cryptocurrencyAddress.IsNew && cryptocurrencyAddress.DateAdded.AddDays(cryptocurrency.ActivateUserAddressAfterDays) > AppSettings.ServerTime)
                throw new MsgException(string.Format(U6000.CANTWITHDRAWBTCUNTIL, (cryptocurrencyAddress.DateAdded.AddDays(cryptocurrency.ActivateUserAddressAfterDays) - AppSettings.ServerTime).ToFriendlyDisplay(2)));

            address = cryptocurrencyAddress.Address.Replace(" ", String.Empty);
        }

        return address;
    }

    protected void TryValidateCryptocurrencyWithdrawalByEmailOrPhone()
    {
        if (AppSettings.Payments.WithdrawalEmailEnabled)
        {
            CryptoPINTextBox.Enabled = false;
            VerificationCode.Create(User.Id, Mailer.SendCodeVerificationEmail(Member.CurrentInCache.Email));
            ConfirmationCodePlaceHolder.Visible = true;
            CryptocurrencySuccessMessagePanel.Visible = true;
            CryptocurrencySuccessMessageLiteral.Text = string.Format(U6000.VERIFICATIONEMAILSENT, AppSettings.Payments.WithdrawalVerificationCodeValidForMinutes);
        }
        else if ((AppSettings.Proxy.SMSType == ProxySMSType.FirstCashout && !User.IsPhoneVerified) || AppSettings.Proxy.SMSType == ProxySMSType.EveryCashout)
        {
            if (!User.IsPhoneVerified)
            {
                User.RequireSMSPin();

                System.Web.Security.FormsAuthentication.SignOut();
                HttpContext.Current.Session.Abandon();
            }
            else
            {
                if (VerificationCode.ActiveCodeForUserExists(User.Id))
                {
                    CryptocurrencySuccessMessageLiteral.Text = L1.ALREADYSENT + "(+" + User.PhoneCountryCode + " " + User.PhoneNumber + ")";
                }
                else
                {
                    VerificationCode.Create(User.Id, Convert.ToInt32(ProxStop.SendSMSWithPIN(User.PhoneCountryCode, User.PhoneNumber)));
                    CryptocurrencySuccessMessageLiteral.Text = string.Format(U6000.VERIFICATIONSMSSENT, AppSettings.Payments.WithdrawalVerificationCodeValidForMinutes);
                    User.UnconfirmedSMSSent++;
                }
                ConfirmationCodePlaceHolder.Visible = true;
                CryptocurrencySuccessMessagePanel.Visible = true;
                User.IsPhoneVerifiedBeforeCashout = false;
                User.Save();
            }
        }
    }

    protected void ProportionsGridViewSqlDataSource_Init(object sender, EventArgs e)
    {
        ProportionsGridViewSqlDataSource.SelectCommand = string.Format(@"SELECT * FROM PaymentProportions WHERE UserId = {0}", Member.Current.Id);
    }

    protected void ProportionsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        PaymentProportionsManager ppm = new PaymentProportionsManager(User);
        ProportionsGridView.Columns[0].HeaderText = U5004.PAYMENTPROCESSOR;
        ProportionsGridView.Columns[1].HeaderText = U5004.PAIDIN;
        ProportionsGridView.Columns[2].HeaderText = U5004.PAIDIN + " (%)";
        ProportionsGridView.Columns[3].HeaderText = DEFAULT.TOTALCASHOUT;
        ProportionsGridView.Columns[4].HeaderText = U5004.MAXIMUMWITHDRAWAL + "*";

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            PaymentProcessor processor = (PaymentProcessor)Convert.ToInt32(e.Row.Cells[0].Text);

            int percentage = ppm.GetPercentage(processor);

            //Processor
            e.Row.Cells[0].Text = processor.ToString();

            //TotalIn
            e.Row.Cells[1].Text = Money.Parse(e.Row.Cells[1].Text).ToString();
            e.Row.Cells[2].Text = percentage.ToString() + "%";
            //TotalOut
            e.Row.Cells[3].Text = Money.Parse(e.Row.Cells[3].Text).ToString();

            Money maximum = ppm.GetMaximum(processor);
            if (maximum > User.MainBalance)
                maximum = User.MainBalance;
            e.Row.Cells[4].Text = maximum.ToString();


        }
    }

    protected void WithdrawHistoryGridViewDataSource_Init(object sender, EventArgs e)
    {
        WithdrawHistorySqlDataSource.SelectCommand = string.Format(@"SELECT * FROM PayoutRequests WHERE Username = '{0}' ORDER BY RequestDate DESC", Member.CurrentName);        
    }

    protected void WithdrawHistoryGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        WithdrawHistoryGridView.Columns[0].HeaderText = L1.DATE;
        WithdrawHistoryGridView.Columns[1].HeaderText = L1.AMOUNT;
        WithdrawHistoryGridView.Columns[2].HeaderText = U5004.PAYMENTPROCESSOR;
        WithdrawHistoryGridView.Columns[3].HeaderText = L1.ADDRESS;
        WithdrawHistoryGridView.Columns[4].HeaderText = L1.STATUS;
        WithdrawHistoryGridView.Columns[5].HeaderText = L1.TYPE;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[1].Text = Money.Parse(((DataBoundLiteralControl)e.Row.Cells[1].Controls[0]).Text.Trim()).ToString();

            string PaymentProcessorOrRejected = ((DataBoundLiteralControl)e.Row.Cells[2].Controls[0]).Text.Trim();

            if (PaymentProcessorOrRejected == "REJECTED")
                e.Row.Cells[2].Text = "-";

            bool IsPaid = Convert.ToBoolean(((DataBoundLiteralControl)e.Row.Cells[4].Controls[0]).Text.Trim());

            if (PaymentProcessorOrRejected == "REJECTED")
            {
                e.Row.Cells[4].Text = L1.REJECTED;
                e.Row.Cells[4].ForeColor = System.Drawing.Color.DarkRed;
                e.Row.Cells[6].Text = "";
            }
            else if (IsPaid)
            {
                e.Row.Cells[4].Text = L1.FINISHED;
                e.Row.Cells[4].ForeColor = System.Drawing.Color.Green;
                e.Row.Cells[6].Text = "";
            }
            else
            {
                e.Row.Cells[4].Text = L1.PENDING;
                e.Row.Cells[4].ForeColor = System.Drawing.Color.Purple;
            }

            BalanceType balance = (BalanceType)Convert.ToInt32(((DataBoundLiteralControl)e.Row.Cells[5].Controls[0]).Text.Trim());

            if (balance == BalanceType.MainBalance)
                e.Row.Cells[5].Text = L1.MAINBALANCE;
            else if (balance == BalanceType.CommissionBalance)
                e.Row.Cells[5].Text = U5004.COMMISSIONBALANCE;
        }
    }

    protected void WithdrawHistoryGridView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        int id = (int)WithdrawHistoryGridView.DataKeys[e.NewEditIndex].Value;
        EditedPayoutRequest = new PayoutRequest(id);

    }

    protected void WithdrawHistoryGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)

    {
        GridViewRow updateRow = WithdrawHistoryGridView.Rows[e.RowIndex];

        if (Page.IsValid)
        {

            var EditPaymentAddressTextBox_Edit = updateRow.FindControl("EditPaymentAddressTextBox") as TextBox;

            if (EditedPayoutRequest.PaymentProcessor != "REJECTED" && EditedPayoutRequest.IsRequest && !EditedPayoutRequest.IsPaid)
            {
                EditedPayoutRequest.PaymentAddress = EditPaymentAddressTextBox_Edit.Text;
                EditedPayoutRequest.Save();
            }
            WithdrawHistorySqlDataSource.DataBind();
        }
    }

    /// <summary>
    /// //BTC, XRP, TOKEN etc | all cryptocurrencies
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void CryptocurrencyPayout_Activate(object sender, EventArgs e)
    {
        CryptocurrencyErrorMessagePanel.Visible = false;
        CryptocurrencySuccessMessagePanel.Visible = false;
        WithdrawCryptocurrencyAmountTextBox.Text = string.Empty;
        CryptoPINTextBox.Enabled = true;
        CryptoPINTextBox.Text = string.Empty;
        CryptocurrencyFeeLiteral.Visible = WithdrawTotalCryptocurrencyLiteral.Visible = false;
        WithdrawCryptocurrencyAddressTextBox.Text = string.Empty;
        string errorMessage = String.Empty;

        Cryptocurrency Cryptocurrency = CryptocurrencyFactory.Get(SelectedCryptocurrency);

        CryptocurrencyImage.ImageUrl = Cryptocurrency.GetImageUrl();
        CryptocurrencyAddressLabel.Text = string.Format("{1} {0}", L1.ADDRESS, Cryptocurrency.Code);
        CryptocurrencyValueLiteral.Text = string.Format("1 {0} = <b>{1}</b>", Cryptocurrency.Code, Cryptocurrency.GetValue().ToString());

        if (TitanFeatures.IsTrafficThunder)
            CryptocurrencyValueLiteral.Text = "1 BTC = <b>" + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:n2}", CryptocurrencyFactory.Get(CryptocurrencyType.BTC).GetOriginalValue("USD").ToDecimal()) + " USD</b>";

        CryptocurrencyWithdrawalSourceLiteral.Text = string.Format(U6012.WITHDRAWALSOURCE, Cryptocurrency.GetWithdrawalSourceName());

        MinimumCryptocurrencyAmountLiteral.Text = string.Format("{0}: {1}<br />", L1.LIMIT, Cryptocurrency.GetMinimumWithdrawalAmount(User, Cryptocurrency.WithdrawalSource).ToString());
        MaximumCryptocurrencyAmountLiteral.Text = string.Format("{0}: {1}", U5004.MAXIMUM, Cryptocurrency.GetMaximumWithdrawalAmount(User, Cryptocurrency.WithdrawalSource, out errorMessage).ToString());

        //Currency sign
        CurrencySignLiteral.Text = AppSettings.Site.MulticurrencySign;
        if (Cryptocurrency.WithdrawalSource == WithdrawalSourceBalance.Wallet)
            CurrencySignLiteral.Text = Cryptocurrency.CurrencySign;

        if (Cryptocurrency.WithdrawalApiProcessor == CryptocurrencyAPIProvider.Coinbase)
        {
            switch (AppSettings.Cryptocurrencies.CoinbaseAddressesPolicy)
            {
                case CoinbaseAddressesPolicy.CoinbaseEmail:
                    CryptocurrencyAddressLabel.Text = U6010.COINBASEEMAIL;
                    WithdrawCryptocurrencyAddressTextBox.Text = CoinbaseAddressHelper.GetAddress(User.Id);

                    if (string.IsNullOrEmpty(WithdrawCryptocurrencyAddressTextBox.Text))
                        ChangeCryptocurrencyAddressButton.Text = L1.ADDNEW;
                    else
                        ChangeCryptocurrencyAddressButton.Text = U6007.CHANGE;
                    break;

                case CoinbaseAddressesPolicy.BTCWallet:
                    SetAndCheckCryptocurrencyAddress();
                    break;

                case CoinbaseAddressesPolicy.CoinbaseEmailOrBTCWallet:
                    CryptocurrencyAddressLabel.Text = U6010.COINBASEEMAILORBTCWALLET;
                    CoinbaseAddressesDDL.Visible = true;
                    WithdrawCryptocurrencyAddressTextBox.Visible = false;

                    var emailAddress = CoinbaseAddressHelper.GetAddress(User.Id, 1);
                    //coinbase Email first
                    if (string.IsNullOrEmpty(emailAddress))
                    {
                        CoinbaseAddressesDDL.Items.Add(L1.ADDNEW);
                        ChangeCryptocurrencyAddressButton.Text = L1.ADDNEW;
                    }
                    else
                    {
                        CoinbaseAddressesDDL.Items.Add(new ListItem(emailAddress, CoinbaseAddressesPolicy.CoinbaseEmail.ToString()));
                        ChangeCryptocurrencyAddressButton.Text = U6007.CHANGE;
                    }
                    //btc wallet
                    var btcAddress = CoinbaseAddressHelper.GetAddress(User.Id, 2);
                    if (string.IsNullOrEmpty(btcAddress))
                        CoinbaseAddressesDDL.Items.Add(U6010.ADDNEWBTCWALLET);
                    else
                        CoinbaseAddressesDDL.Items.Add(new ListItem(btcAddress, CoinbaseAddressesPolicy.BTCWallet.ToString()));

                    break;
            }
        }
        else
            SetAndCheckCryptocurrencyAddress();
    }

    private void SetAndCheckCryptocurrencyAddress()
    {
        var WalletAddressCryptocurrencyType = SelectedCryptocurrency;

        cryptocurrencyAddress = CryptocurrencyWithdrawalAddress.GetAddress(Member.CurrentId, WalletAddressCryptocurrencyType);

        if (cryptocurrencyAddress != null)
        {
            WithdrawCryptocurrencyAddressTextBox.Text = cryptocurrencyAddress.Address;

            if (SelectedCryptocurrency == CryptocurrencyType.XRP)
            {
                var rippleAddress = RippleAddress.FromString(WithdrawCryptocurrencyAddressTextBox.Text);
                WithdrawCryptocurrencyAddressTextBox.Text = rippleAddress.ToDisplayString();
            }

            ChangeCryptocurrencyAddressButton.Text = U6007.CHANGE;
        }
        else
            ChangeCryptocurrencyAddressButton.Text = L1.ADDNEW;
    }

    protected void CommissionCashoutButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            CommissionSuccMessagePanel.Visible = false;
            CommissionErrorMessagePanel.Visible = false;

            try
            {
                Money amountToPayout = Money.Parse(CommissionAmountToCashout.Text).FromMulticurrency();

                CommissionPayoutManager mgr = new CommissionPayoutManager(User, amountToPayout, CommissionRadioFrom.SelectedValue);

                CommissionSuccMessage.Text = mgr.TryMakePayout();
                CommissionSuccMessagePanel.Visible = true;
            }
            catch (MsgException ex)
            {
                CommissionErrorMessagePanel.Visible = true;
                CommissionErrorMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw ex;
            }
        }
    }

    protected void RadioFrom_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (RadioFrom.SelectedValue == "BTC")
            Response.Redirect("/user/cashout.aspx?b=BTCWithdrawal");

        SetProcessorValues();
    }

    protected void SetProcessorValues()
    {
        Account.Enabled = true;
        ChangeAccountButton.Visible = false;
        AddNewAccount.Visible = false;
        var userAccountAddress = "";
        Money GlobalLimit = PayoutLimit.GetGlobalLimitValue(User);
        var errorNote = "";

        try
        {
            //Custom Processor
            var CustomProcessor = new CustomPayoutProcessor(int.Parse(RadioFrom.SelectedValue));
            userAccountAddress = User.GetPaymentAddress(CustomProcessor.Id);
            Account.Text = userAccountAddress;

            //Limits
            string limit = (CustomProcessor.OverrideGlobalLimit ? CustomProcessor.CashoutLimit : GlobalLimit).ToShortString();
            Money maxPayout = PayoutManager.GetMaxPossiblePayout(PaymentProcessor.CustomPayoutProcessor, User, out errorNote);

            if (maxPayout > User.MainBalance)
                maxPayout = User.MainBalance;

            MinLimitLabel.Text = limit;
            MaxLimitLabel.Text = maxPayout.ToShortString();
            InfoLabel.Text = CustomProcessor.Description;
            FeesLabel.Text = CustomProcessor.FeesToString();
        }
        catch (Exception)
        {
            //Automatic processor
            var selectedProcessor = RadioFrom.SelectedValue;

            if (!String.IsNullOrEmpty(selectedProcessor))
            {
                PaymentProcessor targetprocessor = PaymentAccountDetails.GetFromStringType(selectedProcessor);
                userAccountAddress = User.GetPaymentAddress(targetprocessor);
                var gateway = PaymentAccountDetails.GetFirstGateway(selectedProcessor);

                //Limits
                string limit = (gateway.OverrideGlobalLimit ? gateway.CashoutLimit : GlobalLimit).ToShortString();
                Money maxPayout = PayoutManager.GetMaxPossiblePayout(PaymentAccountDetails.GetFromStringType(gateway.AccountType), User, out errorNote);

                if (maxPayout > User.MainBalance)
                    maxPayout = User.MainBalance;

                MinLimitLabel.Text = limit;
                MaxLimitLabel.Text = maxPayout.ToShortString();
                FeesLabel.Text = NumberUtils.FormatPercents(gateway.WithdrawalFeePercent.ToString());
            }
        }

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

    protected void ChangeAccountButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/user/settings.aspx?ref=payment");
    }

    protected void CoinbaseAddressesDDL_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (CoinbaseAddressesDDL.SelectedValue.ToString().Contains(L1.ADDNEW))
            ChangeCryptocurrencyAddressButton.Text = L1.ADDNEW;
        else
            ChangeCryptocurrencyAddressButton.Text = U6007.CHANGE;
    }

    protected void SendWithdrawViaRepresentativeButtonConfirm_Click(object sender, EventArgs e)
    {
        RepresentativeErrorMessagePanel.Visible = false;
        try
        {
            string amount = Request.Form["price"].ToString();

            Money Amount = Money.Parse(amount).FromMulticurrency();
            Member User = Member.Current;

            //Anti-Injection Fix
            if (Amount <= new Money(0))
                throw new MsgException(L1.ERROR);

            if (string.IsNullOrEmpty(RepresentativeMessage.Text))
                throw new MsgException(L1.REQ_TEXT);

            //Lets validate
            PayoutManager.ValidatePayout(User, Amount);
            PayoutManager.CheckMaxPayout(PaymentProcessor.ViaRepresentative, User, Amount);

            string Message = InputChecker.HtmlEncode(RepresentativeMessage.Text, RepresentativeMessage.MaxLength, U5004.MESSAGE);

            var SelectedRepresentative = new Representative(Convert.ToInt32(AvaibleRepresentativeList.SelectedValue));

            if (ConversationMessage.CheckIfThisUserHavePendingActions(User.Id))
                throw new MsgException(U6010.YOUHAVEPENDINGACTION);

            //All OK, let's proceed
            RepresentativesTransferManager representativesTransferManager = new RepresentativesTransferManager(Member.CurrentId, SelectedRepresentative.UserId);
            representativesTransferManager.InvokeWithdrawal(Amount, Message);

            Response.Redirect("~/user/network/messenger.aspx");
        }
        catch (MsgException ex)
        {
            RepresentativeErrorMessagePanel.Visible = true;
            RepresentativeErrorMessage.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            RepresentativeErrorMessagePanel.Visible = true;
            RepresentativeErrorMessage.Text = ex.Message;
        }
    }

    protected void AvaibleRepresentativeList_SelectedIndexChanged(object sender, EventArgs e)
    {
        var representative = new Representative(Convert.ToInt32(AvaibleRepresentativeList.SelectedValue));
        var useId = representative.UserId;
        var optionsList = RepresentativesPaymentProcessor.GetAllPaymentOptions(useId);

        WithdrawalOptionsPlaceHolder.Controls.Clear();

        foreach (var option in optionsList)
            if (!string.IsNullOrEmpty(option.WithdrawalInfo))
                WithdrawalOptionsPlaceHolder.Controls.Add(GetPaymentHTML(option.Name, option.LogoPath, option.WithdrawalInfo));
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

    protected void WithdrawViaRepresentativeView_Activate(object sender, EventArgs e)
    {
        var representativesList = Representative.GetAllActiveFromCountry(User.Country);

        if (representativesList.Count > 0)
        {
            SetFeeText();
            NoRepresentativeInfoPlaceHolder.Visible = false;
            RepresentativeInfoContentPlaceHolder.Visible = true;
            AvaibleRepresentativeList.Items.Clear();

            String RadioButtonStyles = "style=\"float: left; padding-left: 5px; padding-right: 5px; \"";
            foreach (var representative in representativesList)
            {
                var item = new ListItem(String.Format("<p {0}>{1}</p> {2}", RadioButtonStyles, representative.Name, HtmlRatingGenerator.GenerateHtmlRating(RatingType.Representative, representative.UserId)), representative.Id.ToString());
                AvaibleRepresentativeList.Items.Add(item);
            }

            flagImage.ImageUrl = string.Format("~/Images/Flags/{0}.png", CountryManager.GetCountryCode(User.Country).ToLower());

            AvaibleRepresentativeList.SelectedIndex = 0;
            AvaibleRepresentativeList_SelectedIndexChanged(null, null);
        }
    }

    private void SetFeeText()
    {
        var fee = AppSettings.Representatives.RepresentativesHelpWithdrawalFee;
        RepFeeLabel.Text = string.Format("{0}", NumberUtils.FormatPercents(fee));
    }
}
