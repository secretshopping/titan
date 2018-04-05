using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Prem.PTC;
using Resources;

public partial class Controls_Register : System.Web.UI.UserControl
{
    public string FacebookAuthHidden
    {
        get
        {
            if (IsFromFacebookOAuth)
                return "style=\"display:none;\"";
            return "";
        }
    }

    public bool IsFromFacebookOAuth
    {
        get
        {
            if (Request.QueryString["fb"] != null && Session["accessToken"] != null)
                return true;
            return false;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        GlobalMasterHelper.PageLoad();
        AdBlockManager.CheckDenyForAll();

        RegistrationCaptchaPlaceHolder.Visible = AppSettings.Registration.IsRegistrationCaptchaEnabled;
        PINSectionPlaceHolder.Visible = AppSettings.Registration.IsPINEnabled;

        if (!Page.IsPostBack)
        {
            AppSettings.Authentication.Reload();

            if (Request.QueryString["e"] != null)
                Email.Text = Request.QueryString["e"];

            if (TitanFeatures.IsRetireYoung)
                ReferrerUtils.SetReferrer("RetireYoung");

            if (Request.QueryString["u"] != null || ReferrerUtils.GetReferrerName() != null)
            {
                string refUsername;
                if (Request.QueryString["u"] != null)
                    ReferrerUtils.SetReferrer(Request.QueryString["u"]);

                refUsername = ReferrerUtils.GetReferrerName();

                if (!Page.IsPostBack)
                    PoolRotatorManager.TryAddLinkView(refUsername);

                // With referral link for Pool Rotator
                refUsername = PoolRotatorManager.TryGetUserNameFromPool(refUsername);

                Referer.Text = refUsername;
            }
            else if (Session["PaidToPromoteReferer"] != null)
            {
                var refId = Convert.ToInt32(Session["PaidToPromoteReferer"].ToString());
                var refUsername = new Member(refId).Name;

                Referer.Text = refUsername;
            }
        }
        FeatureManager Manager = null;

        //Facebook OAuth
        if (IsFromFacebookOAuth)
        {
            //Check if country is eligible for Facebook Register
            Manager = new FeatureManager(GeolocatedFeatureType.FacebookRegistration);

            //Hide unnecessary fields
            FacebookMember fbMember = new FacebookMember(Session["accessToken"].ToString());

            Email.Text = fbMember.Email;

            //Passwords validators
            RegularExpressionValidator2.Enabled = false;
            PasswordRequired.Enabled = false;
            ConfirmPasswordRequired.Enabled = false;
            PasswordCompare.Enabled = false;
            CustomValidator1.Enabled = false;
            TitanCaptcha.Visible = false;

            FirstName.Text = fbMember.FirstName;
            SecondName.Text = fbMember.LastName;
        }
        else
        {
            //Check if country is eligible for Standard Register
            Manager = new FeatureManager(GeolocatedFeatureType.Registration);
        }

        if (!Manager.IsAllowed)
        {
            RegistrationPanel.Visible = false;
            CreateUserButton.Visible = false;
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = U4000.SORRYCOUNTRY;
        }

        //Set up textbox hints
        HintAdder.Add(Username, L1.REG_USERNAME);
        HintAdder.Add(Password, U3501.REG_PASSWORD);
        HintAdder.Add(Email, L1.REG_EMAIL);
        HintAdder.Add(PIN, L1.REG_PIN);
        HintAdder.Add(Referer, L1.REG_REFERER);

        Username.Attributes.Add("placeholder", L1.USERNAME);
        Email.Attributes.Add("placeholder", "Email");
        Password.Attributes.Add("placeholder", L1.PASSWORD);
        ConfirmPassword.Attributes.Add("placeholder", L1.CONFIRMPASSWORD);
        PIN.Attributes.Add("placeholder", L1.DESIREDPIN);
        BirthYear.Attributes.Add("placeholder", L1.BIRTHYEAR);
        FirstName.Attributes.Add("placeholder", L1.FIRSTNAME);
        SecondName.Attributes.Add("placeholder", L1.SECONDNAME);
        City.Attributes.Add("placeholder", L1.CITY);
        StateProvince.Attributes.Add("placeholder", L1.STATEPROVINCE);
        ZipCode.Attributes.Add("placeholder", L1.ZIPCODE);

        Address.Attributes.Add("placeholder", L1.ADDRESS);

        //Add translations
        LangAdder.Add(CreateUserButton, L1.REGISTER);
        LangAdder.Add(RegularExpressionValidator1, L1.ER_INVALIDUSERNAME, true);
        LangAdder.Add(UserNameRequired, L1.REG_REQ_USERNAME, true);
        LangAdder.Add(EmailRequired, L1.REG_REQ_EMAIL, true);
        LangAdder.Add(CorrectEmailRequired, L1.ER_BADEMAILFORMAT, true);
        LangAdder.Add(PasswordRequired, L1.REG_REQ_PASS, true);
        LangAdder.Add(RegularExpressionValidator2, L1.ER_INVALIDPASS, true);
        LangAdder.Add(PasswordCompare, L1.ER_PASSDIFFER, true);
        LangAdder.Add(ConfirmPasswordRequired, L1.REG_REQ_CONFIRM, true);
        LangAdder.Add(RegularExpressionValidator3, L1.ER_BADPIN, true);
        LangAdder.Add(RequiredFieldValidator2, L1.REG_REQ_PIN, true);
        LangAdder.Add(RegularExpressionValidator4, L1.ER_BADYEAR, true);
        LangAdder.Add(RequiredFieldValidator1, L1.REG_REQ_YEAR, true);
        LangAdder.Add(RefererValidator, L1.ER_BADREF, true);
        LangAdder.Add(CustomValidator1, L1.ER_BADCAPTCHA, true);
        LangAdder.Add(CustomValidator4, L1.REG_REQ_TOS, true);

        EarnerCheckBox.Text = U6000.EARNER;
        AdvertiserCheckBox.Text = L1.ADVERTISER;
        PublisherCheckBox.Text = U6000.PUBLISHER;

        //Detailed info
        LangAdder.Add(RE_1, L1.DETAILEDNOSPECIAL + " " + L1.FIRSTNAME, true);
        LangAdder.Add(RF_1, L1.FIRSTNAME + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(RE_2, L1.DETAILEDNOSPECIAL + " " + L1.SECONDNAME, true);
        LangAdder.Add(RF_2, L1.SECONDNAME + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(RE_3, L1.DETAILEDNOSPECIAL + " " + L1.ADDRESS, true);
        LangAdder.Add(RF_3, L1.ADDRESS + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(RE_4, L1.DETAILEDNOSPECIAL + " " + L1.CITY, true);
        LangAdder.Add(RF_4, L1.CITY + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(RE_5, L1.DETAILEDNOSPECIAL + " " + L1.STATEPROVINCE, true);
        LangAdder.Add(RF_5, L1.STATEPROVINCE + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(RE_6, L1.DETAILEDNOSPECIAL + " " + L1.ZIPCODE, true);
        LangAdder.Add(RF_6, L1.ZIPCODE + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(AccountTypeValidator, U6000.SELECTACCOUNTTYPE, true);

        //Check detailed info
        if (AppSettings.Authentication.DetailedRegisterFields)
            DetailedPanel.Visible = true;

        AvailableRolesPlaceHolder.Visible = !AppSettings.Registration.IsDefaultRegistrationStatusEnabled;
        EarnerCheckBoxPlaceHolder.Visible = AppSettings.TitanFeatures.EarnersRoleEnabled;
        AdvertiserCheckBoxPlaceHolder.Visible = AppSettings.TitanFeatures.AdvertisersRoleEnabled;
        PublisherCheckBoxPlaceHolder.Visible = AppSettings.TitanFeatures.PublishersRoleEnabled;

        if (AppSettings.Registration.IsDefaultRegistrationStatusEnabled)
        {
            EarnerCheckBox.Checked = AppSettings.Registration.IsDefaultEarnerStatus;
            AdvertiserCheckBox.Checked = AppSettings.Registration.IsDefaultAdvertiserStatus;
            PublisherCheckBox.Checked = AppSettings.Registration.IsDefaultPublisherStatus;
        }

        //Custom field
        CustomFields.Controls.Add(RegistrationFieldCreator.Generate());

        CountryInformation CIService = new CountryInformation(IP.Current);
        CountryName.Text = CIService.CountryName;
        Flag.ImageUrl = "~/Images/Flags/" + CIService.CountryCode.ToLower() + ".png";
    }

    protected void Validate_Referer(object source, ServerValidateEventArgs args)
    {
        args.IsValid = false;

        //Validate if exist and below the limit
        string InReferer = Referer.Text.Trim();

        if (!string.IsNullOrEmpty(InReferer) && Member.Exists(InReferer))
        {
            Member UserReferer = new Member(InReferer);

            if (UserReferer.GetDirectReferralsCount() < UserReferer.DirectReferralLimit)
                args.IsValid = true;
        }
    }

    protected void Validate_Tos(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (checkedTerms.Checked == true);
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
    protected void Validate_AccountType(object source, ServerValidateEventArgs args)
    {
        if (AppSettings.AffiliateNetwork.AffiliateNetworkEnabled)
            args.IsValid = EarnerCheckBox.Checked || AdvertiserCheckBox.Checked || PublisherCheckBox.Checked;
        else
            args.IsValid = EarnerCheckBox.Checked || AdvertiserCheckBox.Checked;
    }

    protected void CreateUserButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;

        if (Page.IsValid)
        {
            try
            {
                DateTime InBirthYear = new DateTime(Int32.Parse(BirthYear.Text.Trim()), 1, 1);
                Gender InGender = GenderList.SelectedValue == "1" ? Gender.Male : Gender.Female;
                Panel CustomFields = this.CustomFields;
                int InPIN = AppSettings.Registration.IsPINEnabled ? Int32.Parse(PIN.Text.Trim()) : 9999;

                if (IsFromFacebookOAuth)
                {
                    //Facebook register procedure
                    string accessToken = Session["accessToken"].ToString();

                    FacebookMember fbMember = new FacebookMember(accessToken);
                    TitanRegisterService.Register(Username.Text.Trim(), Email.Text.Trim(), InPIN, InBirthYear, accessToken,
                        Referer.Text.Trim(), fbMember.Gender, CustomFields, FirstName.Text, SecondName.Text, Address.Text,
                        City.Text, StateProvince.Text, ZipCode.Text, EarnerCheckBox.Checked, AdvertiserCheckBox.Checked,
                        PublisherCheckBox.Checked, fbMember.FacebookId);
                }
                else
                {
                    //Standard register procedure
                    TitanRegisterService.Register(Username.Text.Trim(), Email.Text.Trim(), InPIN, InBirthYear, Password.Text.Trim(),
                        Referer.Text.Trim(), InGender, CustomFields, FirstName.Text, SecondName.Text, Address.Text, City.Text,
                        StateProvince.Text, ZipCode.Text, EarnerCheckBox.Checked, AdvertiserCheckBox.Checked, PublisherCheckBox.Checked);
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
}