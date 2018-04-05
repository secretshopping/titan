using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using Resources;

public partial class sites_representatives : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.IsRepresentativesEnabled);

        LangAdders();
        BindControls();

        if (!IsPostBack)
        {
            ContactPanelPlaceHolder.Visible = false;

            if (Request.QueryString["tab"] != null)
            {
                //0 - Add New, 1 - Manage
                var tab = Convert.ToInt32(Request.QueryString["tab"]);

                SuccMessagePanel.Visible = false;
                ErrorMessagePanel.Visible = false;
                ContactPanelPlaceHolder.Visible = false;
                CountriesPlaceHolder.Visible = false;
               
                PaymentMethodsPlaceHolder.Visible = true;
                ManagePaymentMethodsPlaceHolder.Visible = false;
                AddNewPaymentMethodPlaceHolder.Visible = true;

                if (tab == 0)
                {
                    MenuAddNewPaymentMethodButton.CssClass += " active";
                }
                else if(tab == 1)
                {
                    MenuEditPaymentMethodsButton.CssClass += " active";
                    ManagePaymentMethodsPlaceHolder.Visible = true;
                    AddNewPaymentMethodPlaceHolder.Visible = false;                    
                }
                if (Request.QueryString["eid"] != null)
                {
                    var Id = Convert.ToInt32(Request.QueryString["eid"]);
                    EditedRepresentativesPaymentProcessor = new RepresentativesPaymentProcessor(Id);
                    RepresentativePaymentMethodForCurrentRep.RepresentativePaymentMethod = EditedRepresentativesPaymentProcessor;
                }
            }
        }        
    }

    private void LangAdders()
    {
        LangAdder.Add(NameTextBoxRequiredFieldValidator, L1.NAME + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(EmailTextBoxRequiredFieldValidator, "Email" + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(WhyRequiredFieldValidator, U6002.WHY + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(CityTextBoxRequiredFieldValidator, L1.CITY + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(CountryTextBoxRequiredFieldValidator, L1.COUNTRY + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(SkypeTextBoxRequiredFieldValidator, U4200.SKYPE + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(FacebookTextBoxRequiredFieldValidator, "Facebook " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(PhoneTextBoxRequiredFieldValidator, U4200.PHONE + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(LanguagesTextBoxRequiredFieldValidator, L1.LANGUAGE + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(CorrectEmailRequired, L1.ER_BADEMAILFORMAT);
        LangAdder.Add(PhoneNumberCustomValidator, L1.DETAILEDNOSPECIAL + " " + U4200.PHONE);
        LangAdder.Add(SendButton, L1.SEND);

        NameTextBox.Attributes.Add("placeholder", U6002.NAMEOFREPRESENTATION);
        EmailTextBox.Attributes.Add("placeholder", U6002.EMAILCONTACT);
        WhyTextBox.Attributes.Add("placeholder", U6002.WHYBECOMEREPRESENTATIVE);
        CityTextBox.Attributes.Add("placeholder", L1.CITY + " " + U6002.REPRESENTEDUNIT);
        CountryTextBox.Attributes.Add("placeholder", L1.COUNTRY + " " + U6002.REPRESENTEDUNIT);
        SkypeTextBox.Attributes.Add("placeholder", U6003.SKYPECONTACT);
        FacebookTextBox.Attributes.Add("placeholder", U4200.FACEBOOKPROFILE);
        PhoneTextBox.Attributes.Add("placeholder", U6002.PHONECONTACT);
        LanguagesTextBox.Attributes.Add("placeholder", U6002.LANGUAGES);

        RepPaymentProcessorsGridView.EmptyDataText = U6010.NOPAYMENTMETHODSADDED;       
    }

    private void BindControls()
    {
        var userIsRep = Representative.IsActiveRepresentative(Member.CurrentId);
        ForRepSubTitlePlaceHolder.Visible = userIsRep;
        DefaultSubTitlePlaceHolder.Visible = !userIsRep;

        var representatives = Representative.GetAllActive();
        var countries = representatives.Select(item => item.Country).Distinct().ToList();

        var userCountry = Member.IsLogged ? Member.CurrentInCache.Country : "???";

        if (countries.Contains(userCountry))
            BindRepresentatives(representatives, countries, -1, userCountry);

        for (int i = 0; i < countries.Count; i++)
        {
            if (countries[i] != userCountry)
                BindRepresentatives(representatives, countries, i);
        }

        titleLiteral.Text = U6002.REPRESENTATIVES;
        MenuAddNewPaymentMethodButton.Text = L1.ADDNEW;
        MenuEditPaymentMethodsButton.Text = L1.MANAGE;
        SavePaymentMethodButton.Text = U6010.ADD;

        if (Request.QueryString["tab"] != null)
        {
            titleLiteral.Text = U6010.EDITPAYMENTMETHODS;
            ForRepSubTitlePlaceHolder.Visible = false;
            if (Request.QueryString["eid"] != null)            
                SavePaymentMethodButton.Text = L1.SAVE;            
        }
    }

    protected void BindRepresentatives(List<Representative> representatives, List<string> countries, int iterator, string bindConcretCountry = null)
    {
        UserControl countryControl = (UserControl)Page.LoadControl("~/Controls/Representatives/RepresentativeCountry.ascx");

        PropertyInfo ctrl = countryControl.GetType().GetProperty("Representatives");

        if (String.IsNullOrEmpty(bindConcretCountry))
            ctrl.SetValue(countryControl, representatives.Where(item => item.Country == countries[iterator]).ToList(), null);
        else
            ctrl.SetValue(countryControl, representatives.Where(item => item.Country == bindConcretCountry).ToList(), null);

        countryControl.DataBind();
        CountriesPlaceHolder.Controls.Add(countryControl);
    }

    protected void ContactPanelLinkButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (!Member.IsLogged)
                Response.Redirect("/login.aspx");

            SuccMessagePanel.Visible = false;
            ErrorMessagePanel.Visible = false;

            ContactPanelPlaceHolder.Visible = true;
            TitlePlaceHolder.Visible = false;
            CountriesPlaceHolder.Visible = false;

            RepresentativePaymentMethod.Visible = AppSettings.Representatives.RepresentativesHelpWithdrawalEnabled || AppSettings.Representatives.RepresentativesHelpDepositEnabled;

            ClearFields();

            if (Representative.DidUserSendRequest(Member.CurrentId))
                throw new MsgException(U6003.ALREADYREPRESENTANT);

            if (AppSettings.Representatives.Policy == AppSettings.Representatives.RepresentativesPolicy.Automatic)
            {
                foreach (var item in CountryManager.countryName)
                    CountryDropDownList.Items.Add(item);

                CountryTextBox.Visible = false;
                CountryDropDownList.Visible = true;

                CountryTextBoxRequiredFieldValidator.Enabled = false;
            }
            else
            {
                CountryTextBox.Visible = true;
                CountryDropDownList.Visible = false;
            }

            UsernameTextBox.Text = new Member(Member.CurrentId).Name;
        }
        catch (MsgException ex)
        {
            ErrorMessagePanel.Visible = true;
            ContactPanelPlaceHolder.Visible = false;
            ErrorMessage.Text = ex.Message;
            TitlePlaceHolder.Visible = true;
        }
    }

    protected void SendButton_Click(object sender, EventArgs e)
    {
        SuccMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;

        if (!Member.IsLogged)
            Response.Redirect("/login.aspx");

        if (Page.IsValid)
        {
            try
            {
                var Representant = new Representative();

                Representant.Name = NameTextBox.Text;
                Representant.Email = EmailTextBox.Text;
                Representant.Why = WhyTextBox.Text;
                Representant.City = CityTextBox.Text;

                if (AppSettings.Representatives.Policy == AppSettings.Representatives.RepresentativesPolicy.Automatic)
                    Representant.Country = CountryDropDownList.SelectedItem.ToString();
                else
                    Representant.Country = CountryTextBox.Text.Trim();

                int allActiveFromCountry = Representative.GetAllActiveFromCountry(Representant.Country).Count;
                int maxNoOfRepresentatives = AppSettings.Representatives.NoOfRepresentatives;

                if (allActiveFromCountry >= maxNoOfRepresentatives)
                    throw new MsgException(string.Format(U6003.REPRESENTATIVESLIMIT, maxNoOfRepresentatives));

                Representant.Skype = SkypeTextBox.Text;
                Representant.Facebook = FacebookTextBox.Text;
                Representant.PhoneNumber = PhoneTextBox.Text;
                Representant.Languages = LanguagesTextBox.Text;
                Representant.UserId = Member.CurrentId;
                Representant.Status = UniversalStatus.Paused;

                if (Representative.DidUserSendRequest(Representant.UserId))
                    throw new MsgException(U6003.ALREADYREPRESENTANT);

                if (Representative.IsEmailInDatabase(Representant.Email))
                    throw new MsgException(U6002.EMAILREGISTRED);

                if (RepresentativePaymentMethod.Visible)
                {
                    if (string.IsNullOrEmpty(RepresentativePaymentMethod.LogoImagePath))
                        throw new MsgException(U5006.MUSTUPLOADIMAGE);

                    var newRepresentativeProcessor = new RepresentativesPaymentProcessor();
                    newRepresentativeProcessor.UserId = Member.CurrentId;
                    newRepresentativeProcessor.Name = RepresentativePaymentMethod.ProcessorName;
                    newRepresentativeProcessor.LogoPath = RepresentativePaymentMethod.LogoImagePath;                    
                    newRepresentativeProcessor.WithdrawalInfo = RepresentativePaymentMethod.WithdrawalInfo;
                    newRepresentativeProcessor.DepositInfo = RepresentativePaymentMethod.DepositInfo;
                    newRepresentativeProcessor.Save();
                }

                Representant.Save();

                SuccMessagePanel.Visible = true;
                SuccMessage.Text = L1.OP_SUCCESS;
                ErrorMessagePanel.Visible = false;

                ContactPanelPlaceHolder.Visible = false;
                TitlePlaceHolder.Visible = true;
                CountriesPlaceHolder.Visible = true;

                ClearFields();
            }
            catch (MsgException ex)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
                ContactPanelPlaceHolder.Visible = true;
                TitlePlaceHolder.Visible = false;
                CountriesPlaceHolder.Visible = false;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }
    }

    private void ClearFields()
    {
        NameTextBox.Text = string.Empty;
        EmailTextBox.Text = string.Empty;
        WhyTextBox.Text = string.Empty;
        CityTextBox.Text = string.Empty;
        CountryTextBox.Text = string.Empty;

        if (CountryDropDownList.Items.Count > 0)
            CountryDropDownList.SelectedIndex = 0;

        SkypeTextBox.Text = string.Empty;
        FacebookTextBox.Text = string.Empty;
        PhoneTextBox.Text = string.Empty;
        LanguagesTextBox.Text = string.Empty;
        UsernameTextBox.Text = string.Empty;
    }

    protected void PhoneNumberCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = PhoneNumbers.PhoneNumberUtil.IsViablePhoneNumber(PhoneTextBox.Text);
    }

    protected void EditPaymentMethodsLinkButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("representatives.aspx?tab=0");
    }

    private RepresentativesPaymentProcessor _editedRepresentativesPaymentProcessor;
    public RepresentativesPaymentProcessor EditedRepresentativesPaymentProcessor
    {
        get
        {
            if (_editedRepresentativesPaymentProcessor == null)
            {
                if (ViewState["EditedRepresentativesPaymentProcessor"] == null)
                    _editedRepresentativesPaymentProcessor = null;
                else
                    _editedRepresentativesPaymentProcessor = (RepresentativesPaymentProcessor)ViewState["EditedRepresentativesPaymentProcessor"];
            }                
            return _editedRepresentativesPaymentProcessor;
        }
        set
        {
            ViewState["EditedRepresentativesPaymentProcessor"] = value;
        }
    }

    protected void MenuAddNewPaymentMethodButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("representatives.aspx?tab=0");
    }

    protected void MenuEditPaymentMethodsButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("representatives.aspx?tab=1");
    }

    protected void RepPaymentProcessorsGridView_SqlDataSource_Init(object sender, EventArgs e)
    {
        RepPaymentProcessorsGridView_SqlDataSource.SelectCommand = string.Format("Select * FROM RepresentativesPaymentProcessors WHERE UserId = {0}", Member.CurrentId);
    }

    protected void RepPaymentProcessorsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Generate LOGO
            var LogoPath = e.Row.Cells[2].Text;
            e.Row.Cells[2].Text = "<img src=\"" + UrlUtils.ConvertTildePathToImgPath(LogoPath) + "\" />";

            e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);
            e.Row.Cells[4].Text = HttpUtility.HtmlDecode(e.Row.Cells[4].Text);
        }
    }

    protected void RepPaymentProcessorsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string[] commands = new string[2] { "remove", "edit" };

        if (commands.Contains(e.CommandName))
        {
            var index = e.GetSelectedRowIndex() % RepPaymentProcessorsGridView.PageSize;
            var row = RepPaymentProcessorsGridView.Rows[index];
            var paymentMethodId = Convert.ToInt32(row.Cells[0].Text);
            var paymentMethod = new RepresentativesPaymentProcessor(paymentMethodId);

            if (e.CommandName == "remove")
                paymentMethod.Delete();
            else if (e.CommandName == "edit")
            {
                EditedRepresentativesPaymentProcessor = paymentMethod;
                Response.Redirect(string.Format("representatives.aspx?tab=0&eid={0}", paymentMethodId));
            }

            titleLiteral.Text = U6010.EDITPAYMENTMETHODS;
            ForRepSubTitlePlaceHolder.Visible = false;
            RepPaymentProcessorsGridView.DataBind();
        }
    }

    protected void SavePaymentMethodButton_Click(object sender, EventArgs e)
    {
        SuccMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;

        try
        {
            if (string.IsNullOrEmpty(RepresentativePaymentMethodForCurrentRep.ProcessorName))
                throw new MsgException(U6010.PROVIDEPROCESSORNAME);

            if (string.IsNullOrEmpty(RepresentativePaymentMethodForCurrentRep.DepositInfo) && string.IsNullOrEmpty(RepresentativePaymentMethodForCurrentRep.WithdrawalInfo))
                throw new MsgException(U6010.PAYMENTMETHODEMPTYERROR);

            if (string.IsNullOrEmpty(RepresentativePaymentMethodForCurrentRep.LogoImagePath))
                throw new MsgException(U6000.PLEASEUPLOADIMAGE);

            if (EditedRepresentativesPaymentProcessor == null)
                EditedRepresentativesPaymentProcessor = new RepresentativesPaymentProcessor();

            EditedRepresentativesPaymentProcessor.UserId = Member.CurrentId;
            EditedRepresentativesPaymentProcessor.Name = RepresentativePaymentMethodForCurrentRep.ProcessorName;
            EditedRepresentativesPaymentProcessor.LogoPath = RepresentativePaymentMethodForCurrentRep.LogoImagePath;
            EditedRepresentativesPaymentProcessor.DepositInfo = RepresentativePaymentMethodForCurrentRep.DepositInfo;
            EditedRepresentativesPaymentProcessor.WithdrawalInfo = RepresentativePaymentMethodForCurrentRep.WithdrawalInfo;
            EditedRepresentativesPaymentProcessor.Save();

            RepresentativePaymentMethodForCurrentRep.ClearFields();

            SuccMessagePanel.Visible = true;
            if(SavePaymentMethodButton.Text == L1.SAVE)
                SuccMessage.Text = U6010.PAYMENTMETHODSAVED;
            else
                SuccMessage.Text = U6010.PAYMENTMETHODADDED;
        }
        catch(MsgException ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
    }
}