using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Utils;
using Resources;
using Prem.PTC;
using Prem.PTC.Members;
using System.Net.Mail;
using System.IO;

public partial class About : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Member.IsLogged)
            Response.Redirect("~/user/default.aspx");

        //Set proper language to page controls
        HintAdder.Add(PIN, L1.ENTERPIN);
        LangAdder.Add(UserNameRequired, L1.REG_REQ_USERNAME);
        LangAdder.Add(EmailRequired, L1.REG_REQ_EMAIL);
        LangAdder.Add(RegularExpressionValidator1, L1.ER_INVALIDUSERNAME);
        LangAdder.Add(CorrectEmailRequired, L1.ER_BADEMAILFORMAT);
        LangAdder.Add(CustomValidator1, L1.ER_BADCAPTCHA);

        LangAdder.Add(ResetButton, L1.SEND);
        LangAdder.Add(Button1, L1.SEND);
        LangAdder.Add(RegularExpressionValidator3, L1.ER_BADPIN);
        LangAdder.Add(RequiredFieldValidator2, L1.REG_REQ_PIN);


        //Set placeholders
        Username.Attributes.Add("placeholder", L1.USERNAME);
        Email.Attributes.Add("placeholder", "Email");
        PIN.Attributes.Add("placeholder", L1.CURRENTPIN);
        Password.Attributes.Add("placeholder", L1.PASSWORD);
        ConfirmPassword.Attributes.Add("placeholder", L1.CONFIRMPASSWORD);
        PINTextBox.Attributes.Add("placeholder", "PIN");
        ConfirmPINTextBox.Attributes.Add("placeholder", L1.CONFIRM + " PIN");

        //Add translations                             
        LangAdder.Add(RegularExpressionValidator1, L1.ER_INVALIDUSERNAME, true);
        LangAdder.Add(UserNameRequired, L1.REG_REQ_USERNAME, true);
        LangAdder.Add(EmailRequired, L1.REG_REQ_EMAIL, true);
        LangAdder.Add(CorrectEmailRequired, L1.ER_BADEMAILFORMAT, true);

        LangAdder.Add(RegularExpressionValidator3, L1.ER_BADPIN, true);
        LangAdder.Add(RequiredFieldValidator2, L1.REG_REQ_PIN, true);
        LangAdder.Add(CustomValidator1, L1.ER_BADCAPTCHA, true);

        pinTr.Visible = AppSettings.Registration.IsPINEnabled;

        if (AppSettings.Authentication.ResetPasswordIndirectLinkEnabled)
        {
            pinTr.Visible = false;
            RegularExpressionValidator3.Visible = false;
            RegularExpressionValidator3.Enabled = false;
            RequiredFieldValidator2.Visible = false;
            RequiredFieldValidator2.Enabled = false;
            usernameTr.Visible = false;
            RegularExpressionValidator1.Visible = false;
            RegularExpressionValidator1.Enabled = false;
            UserNameRequired.Visible = false;
            UserNameRequired.Enabled = false;
        }

        if (AppSettings.Authentication.ResetPasswordAndPinTogether &&
            AppSettings.Registration.IsPINEnabled)
        {
            HeaderLabel1.Text = L1.RESETPWD + " " + L1.AND + " " + "PIN";
            HeaderLabel2.Text = L1.RESETPWD + " " + L1.AND + " " + "PIN";
            ResetPINPlaceHolder.Visible = true;
        }
        else
        {
            HeaderLabel1.Text = L1.RESETPWD;
            HeaderLabel2.Text = L1.RESETPWD;
            ResetPINPlaceHolder.Visible = false;

        }
            


        if (Request.QueryString["from"] != null &&
            Request.QueryString["s"] != null)
        {
            ResetPanel1.Visible = false;
            ResetPanel2.Visible = true;
        }

    }

    protected void ResetButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            try
            {
                AppSettings.DemoCheck();

                string InUsername = Username.Text.Trim();
                string InEmail = Email.Text.Trim();
                string InPIN = PIN.Text.Trim();

                if (AppSettings.Authentication.ResetPasswordIndirectLinkEnabled)
                {
                    var membersByEmail = TableHelper.SelectRows<Member>(TableHelper.MakeDictionary("Email", InEmail));
                    if (membersByEmail.Count == 0)
                        throw new MsgException(U4000.EMAILNOTINDATABASE);

                    Member memberByEmail = membersByEmail[0];

                    string secret = ResetPasswordRequest.Add(memberByEmail.Id, memberByEmail.Email);
                    Mailer.SendResetPasswordLink(memberByEmail.Name, memberByEmail.Email, secret, memberByEmail.Id);

                    //Inform about success 
                    SuccMessagePanel.Visible = true;
                    SuccMessage.Text = U4000.PASSWORDLINKSENT;
                }
                else
                {

                    //1. Check if Username exists
                    if (!Member.Exists(InUsername))
                        throw new MsgException(L1.ER_USER_NOTFOUND);

                    Member member = new Member(InUsername);

                    //2. Check if Email+PIN matches
                    if (!(member.Email == InEmail) || !(member.PIN.ToString() == InPIN))
                        throw new MsgException(L1.ER_EMAILORPINBAD);

                    //3. Reset passwords
                    if (member.HasSecondaryPassword())
                        member.SecondaryPassword = null;

                    string newPassword = MemberAuthenticationService.ComputeRandomPassword();
                    member.PrimaryPassword = MemberAuthenticationService.ComputeHash(newPassword);
                    member.Save();

                    //4. Send email
                    Mailer.SendResetPasswordInformation(member.Name, member.Email, newPassword);

                    //5. Inform about success 
                    SuccMessagePanel.Visible = true;
                    SuccMessage.Text = L1.RESETPWDSUCC;
                    Response.Redirect("~/status.aspx?type=ok&id=reset");
                }
            }
            catch (MsgException ex)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
            catch (SmtpFailedRecipientException ex)
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

    protected void ResetButton2_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            try
            {
                AppSettings.DemoCheck();

                if (Request.QueryString["from"] != null &&
                Request.QueryString["s"] != null && AppSettings.Authentication.ResetPasswordIndirectLinkEnabled)
                {
                    int userId = Convert.ToInt32(Request.QueryString["from"]);
                    string code = HttpUtility.UrlDecode(Request.QueryString["s"]);

                    string InPassword = Password.Text.Trim();

                    var request = ResetPasswordRequest.Get(userId, code);
                    if (request != null)
                    {
                        if (request.ExpiresOn < DateTime.Now)
                            throw new MsgException(U4000.REQUESTEXPIRED);

                        Member target = new Member(request.UserId);


                        //3. Reset passwords
                        if (target.HasSecondaryPassword())
                            target.SecondaryPassword = null;

                        target.PrimaryPassword = MemberAuthenticationService.ComputeHash(InPassword);

                        if (AppSettings.Authentication.ResetPasswordAndPinTogether)
                            target.PIN = Convert.ToInt32(PINTextBox.Text);

                        target.Save();

                        //5. Inform about success 
                        SuccMessagePanel.Visible = true;
                        SuccMessage.Text = L1.RESETPWDSUCC;
                        Response.Redirect("~/status.aspx?type=ok&id=reset2");

                        //PASSWORDCHANGESUCCESS
                    }
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

    protected void Validate_Captcha1(object source, ServerValidateEventArgs args)
    {
        if (TitanCaptcha1.IsValid)
        {
            args.IsValid = true;
        }
        else
        {
            args.IsValid = false;
        }
    }

    protected void Validate_Captcha2(object source, ServerValidateEventArgs args)
    {
        if (TitanCaptcha2.IsValid)
        {
            args.IsValid = true;
        }
        else
        {
            args.IsValid = false;
        }
    }

}
