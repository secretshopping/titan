using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using Prem.PTC.Payments;
using Prem.PTC.Utils;
using System.Net.Mail;
using Titan.Cryptocurrencies;

public partial class About : System.Web.UI.Page
{
    Member User;

    public Cryptocurrency BtcCryptocurrency { get; set; }
    public Cryptocurrency EthCryptocurrency { get; set; } 
    public Cryptocurrency XrpCryptocurrency { get; set; } 
    public Cryptocurrency TokenCryptocurrency { get; set; } 

    protected void Page_Load(object sender, EventArgs e)
    {
        //Add translations & hints
        LangAdder.Add(ChangeSettingsButton, L1.SAVE);
        LangAdder.Add(ChangeSettingsButton2, L1.SAVE);
        LangAdder.Add(SecuritySettingsSaveButton, L1.SAVE);
        LangAdder.Add(PreferencesSettingsSaveButton, L1.SAVE);
        LangAdder.Add(ChangeSettingsButton3, U4000.ENTERVAC);
        LangAdder.Add(EmailRequired, L1.REG_REQ_EMAIL, true);
        LangAdder.Add(CorrectEmailRequired, L1.ER_BADEMAILFORMAT, true);
        LangAdder.Add(RegularExpressionValidator2, L1.ER_INVALIDPASS, true);
        LangAdder.Add(CustomValidator2, U6000.PINCANTSTARTWITHZERO, true);
        LangAdder.Add(PasswordCompare, L1.ER_PASSDIFFER, true);
        LangAdder.Add(RegularExpressionValidator6, L1.ER_INVALIDPASS, true);
        LangAdder.Add(CompareValidator1, L1.ER_PASSDIFFER, true);
        LangAdder.Add(RegularExpressionValidator3, L1.ER_BADPIN, true);
        LangAdder.Add(RegularExpressionValidator4, L1.ER_BADPIN, true);
        LangAdder.Add(RegularExpressionValidator5, L1.ER_BADPIN, true);
        LangAdder.Add(RequiredFieldValidator4, L1.REG_REQ_PIN, true);
        LangAdder.Add(RequiredFieldValidator3, L1.REG_REQ_PIN, true);
        LangAdder.Add(RegularExpressionValidator1, L1.ER_BADPIN, true);
        LangAdder.Add(RequiredFieldValidator2, L1.REG_REQ_PIN, true);
        LangAdder.Add(RegularExpressionValidator7, L1.ER_BADPIN, true);
        LangAdder.Add(RequiredFieldValidator5, L1.REG_REQ_PIN, true);
        LangAdder.Add(RequiredFieldValidator1, L1.REG_REQ_PIN, true);
        LangAdder.Add(RegularExpressionValidator8, L1.ER_BADPIN, true);
        HintAdder.Add(Password, L1.REG_PASSWORD);
        HintAdder.Add(ConfirmPassword, L1.H_LEAVEBLANK);
        HintAdder.Add(PIN, L1.H_LEAVEBLANK);
        LangAdder.Add(MenuButtonGeneral, U4000.GENERAL);
        LangAdder.Add(MenuButtonPayment, U4000.PAYMENT);
        LangAdder.Add(MenuButtonVacationMode, U4000.VACATIONMODE);
        LangAdder.Add(AvatarUploadValidCustomValidator, U4200.INVALIDAVATAR + ". " + string.Format(U6012.AVATARVALIDATOR, ImagesHelper.AvatarImage.MaxWidth, ImagesHelper.AvatarImage.MaxWidth), true);
        LangAdder.Add(AccountTypeValidator, U6000.SELECTACCOUNTTYPE, true);
        LangAdder.Add(MenuButtonVerification, L1.VERIFICATION);
        LangAdder.Add(MenuButtonPreferences, U6000.PREFERENCES);
        LangAdder.Add(MenuButtonSecurity, U6004.AUTHENTICATION);
        LangAdder.Add(VerificationButton, L1.SEND);
        LangAdder.Add(Verification_BannerUploadSubmit, L1.SUBMIT);

        VacationLiteral.Text = U4200.VACATIONINFO2;
        if (AppSettings.TitanFeatures.AdvertAdPacksEnabled)
            VacationLiteral.Text += "<br/>" + U5002.VACATIONINFO3.Replace("%n%", AppSettings.RevShare.AdPack.AdPackName).Replace("%p%", AppSettings.RevShare.AdPack.AdPackNamePlural);

        User = Member.Current;

        BtcCryptocurrency = CryptocurrencyFactory.Get<BitcoinCryptocurrency>();
        EthCryptocurrency = CryptocurrencyFactory.Get<EthereumCryptocurrency>();
        XrpCryptocurrency = CryptocurrencyFactory.Get<RippleCryptocurrency>();
        TokenCryptocurrency = CryptocurrencyFactory.Get<RippleCryptocurrency>();

        EarnerCheckBoxPlaceHolder.Visible = AppSettings.TitanFeatures.EarnersRoleEnabled;
        AdvertiserCheckBoxPlaceHolder.Visible = AppSettings.TitanFeatures.AdvertisersRoleEnabled;
        PublisherCheckBoxPlaceHolder.Visible = AppSettings.TitanFeatures.PublishersRoleEnabled;
        PINDiv1.Visible = PINDiv2.Visible = PINDiv3.Visible = PINDiv4.Visible = PINDiv5.Visible = PINDiv6.Visible =
            AppSettings.Registration.IsPINEnabled;

        //Vacation mode
        if (!AppSettings.VacationAndInactivity.IsEnabled)
            MenuButtonVacationMode.Visible = false;

        if (User.Status == MemberStatus.VacationMode)
        {
            YouAreInVacationPanel.Visible = true;
            VacationPanel.Visible = false;
            LangAdder.Add(ChangeSettingsButton3, U4000.EXITVAC);
            ChangeSettingsButton3.OnClientClick = "return confirm('" + U4000.EXITVACSURE + "');";
            VacationEndsLiteral.Text = ((DateTime)User.VacationModeEnds).ToShortDateString();
        }

        Password.Attributes["title"] += " " + L1.H_LEAVEBLANK;

        XrpDestTagTextBox.Attributes["placeholder"] = U6011.DESTINATIONTAG;
        myonoffswitch.InputAttributes.Add("class", "onoffswitch-checkbox");
        myonoffswitch.ClientIDMode = System.Web.UI.ClientIDMode.Static;
        myonoffswitch2.InputAttributes.Add("class", "onoffswitch2-checkbox");
        myonoffswitch2.ClientIDMode = System.Web.UI.ClientIDMode.Static;
        AvatarImage.ImageUrl = string.Format("{0}?{1}", User.AvatarUrl, AppSettings.ServerTime);

        ClientScript.RegisterStartupScript(this.GetType(), "onloadscript", "password2change(true);", true);
        HadSecondaryPassword.Text = User.HasSecondaryPassword() ? "1" : "0";

        // Show/hide proper settings
        cpaNotifications.Visible = AppSettings.TitanFeatures.EarnCPAGPTEnabled;
        shoutboxPrivacy.Visible = !(AppSettings.Shoutbox.DisplayMode == ShoutboxDisplayMode.Disabled);
        captchaSettings.Visible = AppSettings.Captcha.AllowMembersToChooseCaptcha;

        if (!AppSettings.TitanFeatures.EarnCPAGPTEnabled && !AppSettings.Captcha.AllowMembersToChooseCaptcha && AppSettings.Shoutbox.DisplayMode == ShoutboxDisplayMode.Disabled)
            MenuButtonPlaceHolder.Controls.Remove(MenuButtonPreferences);

        if (!Page.IsPostBack || SuccMessagePanel.Visible)
        {
            if (SuccMessagePanel.Visible == false)
            {
                Email.Text = User.Email;
                myonoffswitch.Checked = User.MessageSystemTurnedOn;
                PIN.Text = "";
                myonoffswitch2.Checked = User.HasSecondaryPassword();
            }

            ShoutboxPrivacyList.SelectedValue = ((int)User.ShoutboxPrivacyPermission).ToString();
            CPACompletedPermissionsList.SelectedValue = ((int)User.CPAOfferCompletedBehavior).ToString();
            CaptchaRB.SelectedValue = ((int)User.SelectedCaptchaType).ToString();

            FirstNameTextBox.Text = User.FirstName;
            SecondNameTextBox.Text = User.SecondName;
            AddressTextBox.Text = User.Address;
            CityTextBox.Text = User.City;
            StateProvinceTextBox.Text = User.StateProvince;
            ZipCodeTextBox.Text = User.ZipCode;

            EarnerCheckBox.Checked = User.IsEarner;
            EarnerCheckBoxImage.Attributes.Add("class", User.IsEarner ? "icon fa fa-money fa-fw fa-2x img-thumbnail img-check text-primary p-10 check" : "icon fa fa-money fa-fw fa-2x img-thumbnail img-check text-primary p-10");
            AdvertiserCheckBox.Checked = User.IsAdvertiser;
            AdvertiserCheckBoxImage.Attributes.Add("class", User.IsAdvertiser ? "icon fa fa-bullhorn fa-fw fa-2x img-thumbnail img-check text-primary p-10 check" : "icon fa fa-bullhorn fa-fw fa-2x img-thumbnail img-check text-primary p-10");
            PublisherCheckBox.Checked = User.IsPublisher;
            PublisherCheckBoxImage.Attributes.Add("class", User.IsPublisher ? "icon fa fa-globe fa-fw fa-2x img-thumbnail img-check text-primary p-10 check" : "icon fa fa-globe fa-fw fa-2x img-thumbnail img-check text-primary p-10");
        }

        //Loading Basic and Custom Processors
        LoadBasePayoutProcesssorsControls();
        LoadCustomProcessorsControls();

        btcSettings.Visible = BtcCryptocurrency.WithdrawalEnabled && !TitanFeatures.IsClickmyad;
        rippleSettings.Visible = XrpCryptocurrency.WithdrawalEnabled;
        ethereumSettings.Visible = EthCryptocurrency.WithdrawalEnabled;
        tokenSettings.Visible = TokenCryptocurrency.WithdrawalEnabled;

        if (!BtcCryptocurrency.WithdrawalEnabled
                && BasicPayoutProcessorsPlaceHolder.Controls.Count == 0
                && CustomPayoutProcessorsPlaceHolder.Controls.Count == 0
                && !XrpCryptocurrency.WithdrawalEnabled
                && !TokenCryptocurrency.WithdrawalEnabled
                && !EthCryptocurrency.WithdrawalEnabled)
            MenuButtonPlaceHolder.Controls.Remove(MenuButtonPayment);

        if (btcSettings.Visible && BtcCryptocurrency.WithdrawalApiProcessor == CryptocurrencyAPIProvider.Coinbase && AppSettings.Cryptocurrencies.CoinbaseAddressesPolicy == CoinbaseAddressesPolicy.CoinbaseEmail)
            btcSettings.Visible = false;

        if (Request.QueryString["ref"] != null)
        {
            //We have target reference
            string TargetReference = Request.QueryString["ref"];

            if (TargetReference == "payment")
            {
                MenuMultiView.ActiveViewIndex = 1;
                foreach (Button b in MenuButtonPlaceHolder.Controls)
                    b.CssClass = "";
                MenuButtonPayment.CssClass = "ViewSelected";
            }
            else if (TargetReference == "vacation")
            {
                MenuMultiView.ActiveViewIndex = 2;
                foreach (Button b in MenuButtonPlaceHolder.Controls)
                    b.CssClass = "";
                MenuButtonVacationMode.CssClass = "ViewSelected";
            }
            else if (TargetReference == "verification")
            {
                MenuMultiView.ActiveViewIndex = 3;
                foreach (Button b in MenuButtonPlaceHolder.Controls)
                    b.CssClass = "";
                MenuButtonVacationMode.CssClass = "ViewSelected";
            }
        }

        //Verification
        if (!AppSettings.Authentication.IsDocumentVerificationEnabled)
            MenuButtonVerification.Visible = false;

        if (User.VerificationStatus == VerificationStatus.Pending)
        {
            AccountVerificationStatus.Text = L1.PENDING;
            AccountVerificationStatus.ForeColor = System.Drawing.Color.Blue;
            documentsUpload.Visible = false;
            LockVerificationFields();
        }

        if (User.VerificationStatus == VerificationStatus.Verified)
        {
            AccountVerificationStatus.ForeColor = System.Drawing.Color.Green;
            AccountVerificationStatus.Text = U5006.VERIFIED;
            LockVerificationFields();
        }

        if (User.VerificationStatus == VerificationStatus.NotVerified)
        {
            documentsUpload.Visible = true;
            AccountVerificationStatus.ForeColor = System.Drawing.Color.DarkRed;
            AccountVerificationStatus.Text = U5006.NOTVERIFIED;
        }

        DetailedInfoPlaceHolder.Visible = AppSettings.Authentication.DetailedRegisterFields;

        ScriptManager.GetCurrent(this).RegisterPostBackControl(Verification_BannerUploadSubmit);
        ScriptManager.GetCurrent(this).RegisterPostBackControl(VerificationButton);

        EarnerCheckBoxImage.Attributes.Add("class", EarnerCheckBox.Checked ? "icon fa fa-money fa-fw fa-2x img-thumbnail img-check text-primary p-10 check" : "icon fa fa-money fa-fw fa-2x img-thumbnail img-check text-primary p-10");
        AdvertiserCheckBoxImage.Attributes.Add("class", AdvertiserCheckBox.Checked ? "icon fa fa-bullhorn fa-fw fa-2x img-thumbnail img-check text-primary p-10 check" : "icon fa fa-bullhorn fa-fw fa-2x img-thumbnail img-check text-primary p-10");
        PublisherCheckBoxImage.Attributes.Add("class", PublisherCheckBox.Checked ? "icon fa fa-globe fa-fw fa-2x img-thumbnail img-check text-primary p-10 check" : "icon fa fa-globe fa-fw fa-2x img-thumbnail img-check text-primary p-10");

        if (TitanFeatures.IsRofriqueWorkMines)
            ButtonsPlaceHolder.Visible = false;

        if (AppSettings.Registration.IsDefaultRegistrationStatusEnabled)
            EarnerCheckBoxPlaceHolder.Visible = AdvertiserCheckBoxPlaceHolder.Visible = PublisherCheckBoxPlaceHolder.Visible = false;
    }

    private void LoadCustomProcessorsControls()
    {
        var customPayoutProcessorsList = CustomPayoutProcessor.GetAllActiveProcessors();

        foreach (var customPayoutProcessor in customPayoutProcessorsList)
        {
            var objControl = (UserControl)Page.LoadControl("~/Controls/Payment/ProcessorSettings.ascx");
            var parsedControl = objControl as IProcessorSettingsObjectControl;

            parsedControl.Processor = customPayoutProcessor;
            parsedControl.UserId = User.Id;
            parsedControl.DataBind();

            CustomPayoutProcessorsPlaceHolder.Controls.Add(objControl);
        }
    }

    private void LoadBasePayoutProcesssorsControls()
    {
        var basePayoutProcessorsList = PaymentAccountDetails.AvailableUniquePayoutGateways;

        foreach (var basePayoutProcessor in basePayoutProcessorsList)
        {
            var objControl = (UserControl)Page.LoadControl("~/Controls/Payment/ProcessorSettings.ascx");
            var parsedControl = objControl as IProcessorSettingsObjectControl;

            parsedControl.BasicProcessor = basePayoutProcessor;
            parsedControl.UserId = User.Id;
            parsedControl.DataBind();

            BasicPayoutProcessorsPlaceHolder.Controls.Add(objControl);
        }
    }

    private void LockVerificationFields()
    {
        FirstNameTextBox.Enabled = false;
        SecondNameTextBox.Enabled = false;
        AddressTextBox.Enabled = false;
        CityTextBox.Enabled = false;
        StateProvinceTextBox.Enabled = false;
        ZipCodeTextBox.Enabled = false;
    }

    private bool CanUpdateVerificationFields()
    {
        if (!AppSettings.Authentication.IsDocumentVerificationEnabled)
            return true;

        if (Member.CurrentInCache.VerificationStatus == VerificationStatus.NotVerified)
            return true;

        return false;
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("settings.aspx");

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";
    }

    protected void Validate_PIN(object source, ServerValidateEventArgs args)
    {
        int InPIN = Int32.Parse(PIN.Text.Trim());
        if (InPIN < 1000)
        {
            args.IsValid = false;
        }
        else
        {
            args.IsValid = true;
        }
    }

    #region AvatarUpload

    protected void changeSettings_AvatarUploadSubmit_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && changeSettings_AvatarUpload.HasFile)
        {
            newTemporaryBanner.Save("~/Images/Avatars/", HashingManager.GenerateSHA256(Member.CurrentName));

            User.AvatarUrl = newTemporaryBanner.Path;
            User.Save();

            changeSettings_AvatarUpload.Dispose();

            AvatarImage.ImageUrl = string.Format("{0}?{1}", User.AvatarUrl, AppSettings.ServerTime);

        }
    }

    Banner newTemporaryBanner;
    protected void createBannerAdvertisement_BannerUploadValidCustomValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid =
            Banner.TryFromStream(changeSettings_AvatarUpload.PostedFile.InputStream, out newTemporaryBanner)
            && ((newTemporaryBanner.Width <= ImagesHelper.AvatarImage.MaxWidth && newTemporaryBanner.Height <= ImagesHelper.AvatarImage.MaxHeight));
    }

    #endregion

    protected void ChangeSettingsButton_Click(object sender, EventArgs e)
    {
        SuccMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;

        if (Page.IsValid)
        {
            try
            {
                AppSettings.DemoCheck();           
                User.ValidatePIN(CurrentPIN.Text);

                //Email change
                string NewEmail = Email.Text.Trim();
                bool EmailChanged = false;

                if (User.Email != NewEmail)
                {
                    //Check if not taken
                    if (Member.ExistsWithEmail(Email.Text))
                        throw new MsgException(L1.ER_DUPLICATEEMAIL);

                    //Send activation email
                    try
                    {
                        Mailer.SendActivationLink(User.Name, NewEmail);
                        EmailChanged = true;
                        User.Temp = NewEmail;
                    }
                    catch (Exception ex)
                    {
                        throw new MsgException("Unable to send email");
                    }
                }

                User.MessageSystemTurnedOn = myonoffswitch.Checked;

                if (FirstNameTextBox.Text != User.FirstName && CanUpdateVerificationFields())
                {
                    User.FirstName = InputChecker.HtmlEncode(FirstNameTextBox.Text, FirstNameTextBox.MaxLength, L1.FIRSTNAME);
                }
                if (SecondNameTextBox.Text != User.SecondName && CanUpdateVerificationFields())
                {
                    User.SecondName = InputChecker.HtmlEncode(SecondNameTextBox.Text, SecondNameTextBox.MaxLength, L1.SECONDNAME);
                }
                if (AddressTextBox.Text != User.Address && CanUpdateVerificationFields())
                {
                    User.Address = InputChecker.HtmlEncode(AddressTextBox.Text, AddressTextBox.MaxLength, L1.ADDRESS);
                }
                if (StateProvinceTextBox.Text != User.StateProvince && CanUpdateVerificationFields())
                {
                    User.StateProvince = InputChecker.HtmlEncode(StateProvinceTextBox.Text, StateProvinceTextBox.MaxLength, L1.STATEPROVINCE);
                }
                if (CityTextBox.Text != User.City && CanUpdateVerificationFields())
                {
                    User.City = InputChecker.HtmlEncode(CityTextBox.Text, CityTextBox.MaxLength, L1.CITY);
                }
                if (ZipCodeTextBox.Text != User.ZipCode && CanUpdateVerificationFields())
                {
                    User.ZipCode = InputChecker.HtmlEncode(ZipCodeTextBox.Text, ZipCodeTextBox.MaxLength, L1.ZIPCODE);
                }

                User.IsEarner = EarnerCheckBox.Checked;
                User.IsAdvertiser = AdvertiserCheckBox.Checked;
                User.IsPublisher = PublisherCheckBox.Checked;

                User.Save();

                SuccMessagePanel.Visible = true;
                SuccMessage.Text = L1.SETTINGSSUCC;

                Master.ReloadSidebarMenu();

                if (EmailChanged)
                {
                    SuccMessage.Text += ". " + U4000.ACTEMAILSENT;
                }
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
    protected void Validate_AccountType(object source, ServerValidateEventArgs args)
    {
        if (!AppSettings.Registration.IsDefaultRegistrationStatusEnabled)
        {
            if (AppSettings.AffiliateNetwork.AffiliateNetworkEnabled)
                args.IsValid = EarnerCheckBox.Checked || AdvertiserCheckBox.Checked || PublisherCheckBox.Checked;
            else
                args.IsValid = EarnerCheckBox.Checked || AdvertiserCheckBox.Checked;
        }
        else
            args.IsValid = true;
    }

    protected void ChangeSettingsButton2_Click(object sender, EventArgs e)
    {
        SuccMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;

        if (Page.IsValid)
        {
            try
            {
                AppSettings.DemoCheck();
                User.ValidatePIN(CurrentPIN2.Text);

                foreach (var control in CustomPayoutProcessorsPlaceHolder.Controls)
                    (control as IProcessorSettingsObjectControl).Save();
                
                foreach (var control in BasicPayoutProcessorsPlaceHolder.Controls)
                    (control as IProcessorSettingsObjectControl).Save();

                if (CryptocurrencyFactory.Get<BitcoinCryptocurrency>().WithdrawalApiProcessor == CryptocurrencyAPIProvider.Coinbase)
                {
                    switch (AppSettings.Cryptocurrencies.CoinbaseAddressesPolicy)
                    {
                        case CoinbaseAddressesPolicy.BTCWallet:
                            CheckBTCWallet();
                            break;
                        case CoinbaseAddressesPolicy.CoinbaseEmail:
                            TryToSetEmail();
                            break;
                        case CoinbaseAddressesPolicy.CoinbaseEmailOrBTCWallet:
                            if (string.IsNullOrEmpty(BtcAddressTextBox.Text) && string.IsNullOrEmpty(CoinbaseEmailTextBox.Text))
                                throw new MsgException(U6010.YOUMUSTSETWALLETOREMAIL);

                            if (!string.IsNullOrEmpty(BtcAddressTextBox.Text))
                                CheckBTCWallet();

                            if (!string.IsNullOrEmpty(CoinbaseEmailTextBox.Text))
                                TryToSetEmail();
                            break;
                    }
                }
                else if (btcSettings.Visible)
                    CheckBTCWallet();

                CheckRippleWallet();
                CheckEthereumWallet();
                CheckTokenWallet();

                User.Save();
                SuccMessagePanel.Visible = true;
                SuccMessage.Text = L1.SETTINGSSUCC;
            }
            catch (Exception ex)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
        }
    }

    private void TryToSetEmail()
    {
        if (string.IsNullOrEmpty(CoinbaseEmailTextBox.Text))
            throw new MsgException(U6010.PLEASETYPECORRECTCOINBASEEMAIL);
        try
        {
            var address = new MailAddress(CoinbaseEmailTextBox.Text);
        }
        catch (Exception e)
        {
            throw new MsgException(U6010.PLEASETYPECORRECTCOINBASEEMAIL);
        }

        User.SetPaymentAddress(PaymentProcessor.Coinbase, CoinbaseEmailTextBox.Text);
    }

    private void CheckBTCWallet()
    {
        if (string.IsNullOrEmpty(BtcAddressTextBox.Text))
            return;

        BitcoinValidator.BitcoinValidator.ValidateBitcoinAddress(BtcAddressTextBox.Text);
        CryptocurrencyWithdrawalAddress.AddIfNotExists(
            User.Id,
            BtcAddressTextBox.Text.Replace(" ", String.Empty),
            CryptocurrencyType.BTC);
        BtcAddressWarning.Visible = true;
    }

    private void CheckRippleWallet()
    {
        if (String.IsNullOrWhiteSpace(XrpAddressTextBox.Text) || String.IsNullOrWhiteSpace(XrpDestTagTextBox.Text))
            return;

        if (XrpCryptocurrency.WithdrawalEnabled)
        {
            var rippleAddress = new RippleAddress(
                XrpAddressTextBox.Text.Replace(" ", String.Empty),
                XrpDestTagTextBox.Text.Replace(" ", String.Empty));

            CryptocurrencyWithdrawalAddress.AddIfNotExists(User.Id, rippleAddress.ToString(), CryptocurrencyType.XRP);
            XrpAddressWarning.Visible = true;
        }
    }

    private void CheckEthereumWallet()
    {
        if (String.IsNullOrWhiteSpace(EthAddressTextBox.Text))
            return;

        if (EthCryptocurrency.WithdrawalEnabled)
        {
            CryptocurrencyWithdrawalAddress.AddIfNotExists(
                User.Id,
                EthAddressTextBox.Text.Replace(" ", String.Empty),
                CryptocurrencyType.ETH);

            EthAddressWarning.Visible = true;
        }
    }

    private void CheckTokenWallet()
    {
        if (String.IsNullOrWhiteSpace(TokenAddressTextBox.Text))
            return;

        if (TokenCryptocurrency.WithdrawalEnabled)
        {
            CryptocurrencyWithdrawalAddress.AddIfNotExists(
                User.Id,
                TokenAddressTextBox.Text.Replace(" ", String.Empty),
                CryptocurrencyType.ERC20Token);

            TokenAddressWarning.Visible = true;
        }
    }

    protected void SecuritySettingsSaveButton_Click(object sender, EventArgs e)
    {
        SuccMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;

        if (Page.IsValid)
        {
            try
            {
                AppSettings.DemoCheck();

                User.ValidatePIN(CurrentPin5.Text);

                //Password1
                if (!string.IsNullOrEmpty(Password.Text) && Password.Text != ConfirmPassword.Text)
                    throw new MsgException(L1.ER_PASSDIFFER);

                if (!string.IsNullOrEmpty(Password.Text))
                    User.PrimaryPassword = MemberAuthenticationService.ComputeHash(Password.Text);

                //Password2
                if (!string.IsNullOrEmpty(Password2.Text) && Password2.Text != ConfirmPassword2.Text)
                    throw new MsgException(L1.ER_PASSDIFFER);

                if (!string.IsNullOrEmpty(Password2.Text))
                    User.SecondaryPassword = MemberAuthenticationService.ComputeHash(Password2.Text);

                //If he had seconary password and now dont want to
                if (User.HasSecondaryPassword() && myonoffswitch2.Checked == false)
                    User.SecondaryPassword = "";

                if (AppSettings.Registration.IsPINEnabled && !string.IsNullOrEmpty(PIN.Text))
                    User.PIN = Convert.ToInt32(PIN.Text);

                User.Save();
                SuccMessagePanel.Visible = true;
                SuccMessage.Text = L1.SETTINGSSUCC;
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
    protected void PreferencesSettingsSaveButton_Click(object sender, EventArgs e)
    {
        SuccMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;

        if (Page.IsValid)
        {
            try
            {
                AppSettings.DemoCheck();

                User.ValidatePIN(CurrentPin4.Text);

                //Permissions
                User.ShoutboxPrivacyPermission = (ShoutboxPermission)Convert.ToInt32(ShoutboxPrivacyList.SelectedValue);
                User.CPAOfferCompletedBehavior = (CPACompletedBehavior)Convert.ToInt32(CPACompletedPermissionsList.SelectedValue);

                //Captcha
                if (AppSettings.Captcha.AllowMembersToChooseCaptcha)
                    User.SelectedCaptchaType = (CaptchaType)Convert.ToInt32(CaptchaRB.SelectedValue);

                User.Save();
                SuccMessagePanel.Visible = true;
                SuccMessage.Text = L1.SETTINGSSUCC;
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
    protected void ChangeSettingsButton3_Click(object sender, EventArgs e)
    {
        SuccMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;

        if (Page.IsValid)
        {
            try
            {
                AppSettings.DemoCheck();

                User.ValidatePIN(CurrentPIN3.Text);

                //ENTERING OR EXITING?
                if (User.Status == MemberStatus.VacationMode)
                {
                    //Exiting
                    User.Status = MemberStatus.Active;
                    User.VacationModeEnds = DateTime.Now;
                }
                else
                {
                    //Entering
                    //Validate input
                    int days = 0;
                    try
                    {
                        days = Convert.ToInt32(VacationDays.Text);
                    }
                    catch (Exception ex)
                    {
                        throw new MsgException(U4000.BADFORMAT);
                    }

                    if (days <= 0)
                        throw new MsgException("The number of days must be > 0");

                    //Calculate cost
                    Money TotalCost = AppSettings.VacationAndInactivity.CostPerDay;
                    TotalCost *= days;

                    //Check balance & deduce
                    if (TotalCost > User.MainBalance)
                        throw new MsgException(L1.NOTENOUGHFUNDS);

                    User.SubtractFromMainBalance(TotalCost, "Vacation Mode for " + days + " day(s)");

                    //Activate
                    User.Status = MemberStatus.VacationMode;
                    User.VacationModeEnds = DateTime.Now.AddDays(days);
                }

                User.Save();
                SuccMessagePanel.Visible = true;
                SuccMessage.Text = L1.SETTINGSSUCC;
                Response.Redirect("settings.aspx");
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

    private ListItem[] _CreditAsses;
    public ListItem[] PrivacyElements
    {
        get { return _CreditAsses ?? (_CreditAsses = ShoutboxPermissionHelper.ListItems); }
    }

    private ListItem[] _CreditAsses2;
    public ListItem[] CPACompletedBehaviorElements
    {
        get { return _CreditAsses2 ?? (_CreditAsses2 = CPACompletedBehaviorHelper.ListItems); }
    }

    protected void RadioButtonList1_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ShoutboxPrivacyList.Items.AddRange(PrivacyElements);
            ShoutboxPrivacyList.SelectedValue = ((int)Member.CurrentInCache.ShoutboxPrivacyPermission).ToString();

        }
    }

    protected void CPACompletedPermissionsList_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            CPACompletedPermissionsList.Items.AddRange(CPACompletedBehaviorElements);
            CPACompletedPermissionsList.SelectedValue = ((int)Member.CurrentInCache.CPAOfferCompletedBehavior).ToString();
        }
    }

    #region Verification

    protected void Verification_BannerUploadSubmit_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && Verification_BannerUpload.HasFile)
        {
            VerificationTemporaryBanner.Save(AppSettings.FolderPaths.BannerAdvertImages);
            Verification_BannerUpload.Visible = false;
            Verification_BannerUploadSubmit.Visible = false;
            Verification_BannerImage.Visible = true;
            VerificationButton.Visible = true;
            Verification_BannerImage.ImageUrl = UrlUtils.ConvertTildePathToImgPath(VerificationTemporaryBanner.Path);
            Verification_BannerUpload.Dispose();
        }
        else if (!Page.IsValid)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = String.Format(U5008.MAXSIZE, 2500, 1500);
        }
    }

    Banner VerificationTemporaryBanner;
    protected void Verification_BannerUploadValidCustomValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid =
            Banner.TryFromStream(Verification_BannerUpload.PostedFile.InputStream, out VerificationTemporaryBanner)
            && (VerificationTemporaryBanner.Width <= 2500 && VerificationTemporaryBanner.Height <= 1500);
    }

    protected void VerificationButton_Click(object sender, EventArgs e)
    {
        try
        {
            Member User = Member.CurrentInCache;
            User.VerificationStatus = VerificationStatus.Pending;
            User.VerificationDocumentUrl = Verification_BannerImage.ImageUrl;
            User.Save();

            Verification_BannerImage.Visible = false;
            documentsUpload.Visible = false;

            AccountVerificationStatus.Text = L1.PENDING;
            AccountVerificationStatus.ForeColor = System.Drawing.Color.Blue;

            Response.Redirect("~/user/settings.aspx?ref=verification");
        }
        catch (Exception ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
    }

    #endregion

    protected void PaymentSettingsView_Activate(object sender, EventArgs e)
    {
        tokenImage.ImageUrl = AppSettings.Ethereum.ERC20TokenImageUrl;
        //Coinbase addresses policy
        if (BtcCryptocurrency.WithdrawalApiProcessor == CryptocurrencyAPIProvider.Coinbase)
        {
            switch (AppSettings.Cryptocurrencies.CoinbaseAddressesPolicy)
            {
                case CoinbaseAddressesPolicy.BTCWallet:
                    BtcAddressTextBox.Text = CoinbaseAddressHelper.GetAddress(User.Id);
                    break;
                case CoinbaseAddressesPolicy.CoinbaseEmail:
                    SetCoinbaseEmailControls();
                    btcSettings.Visible = false;
                    break;
                case CoinbaseAddressesPolicy.CoinbaseEmailOrBTCWallet:
                    SetCoinbaseEmailControls();
                    BtcAddressTextBox.Text = CoinbaseAddressHelper.GetAddress(User.Id, 2);
                    CoinbaseEmailTextBox.Text = CoinbaseAddressHelper.GetAddress(User.Id, 1);
                    break;
            }
        }
        else
        {
            var currentAddress = CryptocurrencyWithdrawalAddress.GetAddress(User.Id, CryptocurrencyType.BTC);

            if (currentAddress != null)
                BtcAddressTextBox.Text = currentAddress.Address;
            else
                BtcAddressWarning.Visible = false;

            //Ripple
            if (XrpCryptocurrency.WithdrawalEnabled)
            {
                var currentRippleAddress = CryptocurrencyWithdrawalAddress.GetAddress(User.Id, CryptocurrencyType.XRP);

                if (currentRippleAddress != null)
                {
                    var rippleAddress = RippleAddress.FromString(currentRippleAddress.Address);
                    XrpAddressTextBox.Text = rippleAddress.Address;
                    XrpDestTagTextBox.Text = rippleAddress.DestinationTag;
                }
                else
                    XrpAddressWarning.Visible = false;
            }
        }

        //Ethereum
        if (EthCryptocurrency.WithdrawalEnabled)
        {
            var currentEthereumAddress = CryptocurrencyWithdrawalAddress.GetAddress(User.Id, CryptocurrencyType.ETH);

            if (currentEthereumAddress != null)
                EthAddressTextBox.Text = currentEthereumAddress.Address;
            else
                EthAddressWarning.Visible = false;
        }
    }

    private void SetCoinbaseEmailControls()
    {
        CoinbaseEmailSettings.Visible = true;
        CoinbaseEmailTextBox.Visible = true;
        CoinbaseEmailTextBox_Watermark.WatermarkText = U6010.ENTERYOURCOINBASEEMAIL;
        CoinbaseEmailTextBox.Text = CoinbaseAddressHelper.GetAddress(User.Id);
    }
}
