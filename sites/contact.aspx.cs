using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;

public partial class sites_contact : TitanPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        ShowOrHideTicketsBox();
        InsertNamePlaceHolder.Visible = AppSettings.SupportTickets.InsertNameWhenCreatingTicketsEnabled;
        InsertPhoneNumberPlaceHolder.Visible = AppSettings.SupportTickets.InsertPhoneWhenCreatingTicketsEnabled;

        Member User;

        ShowOrHideTicketDepartmentsBox();
        if (Member.IsLogged)
        {
            FormLiteral.Text = L1.SENDSUPPORTTICKET;
            FromUsernameLabel.Visible = true;
            Option2Panel.Visible = true;
            User = Member.CurrentInCache;
            AvatarImage.ImageUrl = User.AvatarUrl;
            UserNameLiteral.Text = User.FormattedName;

            //Show support tickets
            SupportTicketsMenu.Visible = true;
            LangAdder.Add(Button1, L1.SENDSUPPORTTICKET);
            LangAdder.Add(Button2, U3501.VIEWTICKETS);

            if (!IsPostBack)
            {
                FullNameTextBox.Text = User.FirstName + " " + User.SecondName;
                CountryCodeTextBox.Text = User.PhoneCountryCode;
                PhoneNumberTextBox.Text = User.PhoneNumber;

                if (!CheckPosibilityToOpenNewTicket(User))
                {
                    Button1.Visible = false;
                    SendTicketPlaceHolder.Visible = false;
                    ErrorMessagePanel.Visible = true;
                    ErrorMessage.Text = U6008.CANTOPENNEWTICKET;
                }
            }
        }
        else
        {
            FormLiteral.Text = L1.CONTACTFORM;
            Option1SecondPanel.Visible = true;
            Option1Panel.Visible = true;
        }

        //Text
        LabelEmail.Text = AppSettings.Email.Forward;
        EmailLabelWhenTicketsDisabled.Text = AppSettings.Email.Forward;

        //Lang
        FromUsernameLabel.Text = L1.FROM + ":";
        LangAdder.Add(EmailRequired, L1.REG_REQ_EMAIL);
        LangAdder.Add(CorrectEmailRequired, L1.ER_BADEMAILFORMAT);
        LangAdder.Add(TextRequired, L1.REQ_TEXT);
        LangAdder.Add(CustomValidator1, L1.ER_BADCAPTCHA);
        LangAdder.Add(SendMessageButton, L1.SEND);
    }

    protected void ShowOrHideTicketsBox()
    {
        if (AreTicketsDisabled())
        {
            TicketPlaceHolder.Visible = false;
            EmailPlaceHolder.Visible = true;
        }
        else
        {
            TicketPlaceHolder.Visible = true;
            EmailPlaceHolder.Visible = false;
        }
    }

    protected void ShowOrHideTicketDepartmentsBox()
    {
        if (AppSettings.SupportTickets.TicketDepartmentsEnabled)
        {
            TicketDepartmentsPlaceHolder.Visible = true;
            TicketDepartmentsButtonList.Items.Clear();
            TicketDepartmentsButtonList.Items.AddRange(SupportTicketDepartment.AvailableDepartments);
            if (TicketDepartmentsButtonList.SelectedIndex == -1)
                TicketDepartmentsButtonList.SelectedIndex = 0;
        }
        else
            TicketDepartmentsPlaceHolder.Visible = false;
    }

    protected bool CheckPosibilityToOpenNewTicket(Member user)
    {
        var query = string.Format(@"SELECT COUNT(*) FROM SupportTickets WHERE FromUsername = '{0}' AND IsSolved = 0", user.Name);
        int count = (int)TableHelper.SelectScalar(query);

        if (count >= AppSettings.SupportTickets.MaxSimultaneousOpenUserTickets)
            return false;

        return true;
    }

    protected bool AreTicketsDisabled()
    {
        return AppSettings.SupportTickets.TicketsDisabled;
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("~/sites/contact.aspx");
        else
            Response.Redirect("~/sites/tickets.aspx");
    }

    protected void SendMessageButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        if (Page.IsValid)
        {
            AppSettings.Email.Reload();
            
            try
            {
                var InText = InputChecker.HtmlEncode(MessageText.Text, MessageText.MaxLength, U5004.MESSAGE);
                if (Member.IsLogged)
                {
                    string Title = (InText.Length > 41) ? InText.Substring(0, 40) : InText;
                    string BrowserPlatformInfo = Request.Browser.Browser + Request.Browser.Version + " " + Request.Browser.Platform;
                    string name = AppSettings.SupportTickets.InsertNameWhenCreatingTicketsEnabled ? FullNameTextBox.Text : String.Empty;
                    string phoneNumber = AppSettings.SupportTickets.InsertPhoneWhenCreatingTicketsEnabled ? CountryCodeTextBox.Text + PhoneNumberTextBox.Text : String.Empty;
                    BrowserPlatformInfo = (BrowserPlatformInfo.Length > 49) ? BrowserPlatformInfo.Substring(0, 49) : BrowserPlatformInfo;

                    name = InputChecker.HtmlEncode(name, name.Length, L1.NAME);
                    phoneNumber = InputChecker.HtmlEncode(phoneNumber, 100, U4200.PHONE);

                    int departmentId = 1;

                    if (AppSettings.SupportTickets.TicketDepartmentsEnabled)
                        departmentId = Convert.ToInt32(TicketDepartmentsButtonList.SelectedValue);

                    var newTicket = new SupportTicket(Member.Current, Title, Mailer.ReplaceNewLines(InText), departmentId, BrowserPlatformInfo, name, phoneNumber);
                    newTicket.IsRead = true;
                    newTicket.Save();

                    Response.Redirect("/sites/tickets.aspx");
                    SuccMessage.Text = U3501.SUPPSENT;
                }
                else
                {
                    Mailer.SendContactMessage(Email.Text, MessageText.Text);
                    SuccMessage.Text = U3501.MESSAGESENT;
                }

                SuccMessagePanel.Visible = true;
                //SuccMessage.Text = L1.OP_SUCCESS;

                //Clear the fields
                ErrorMessagePanel.Visible = false;
                MessageText.Text = "";
                Email.Text = "";
            }
            catch (MsgException ex)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }
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
}