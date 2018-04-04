using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using Resources;
using Prem.PTC.Members;
using Prem.PTC;
using Prem.PTC.Utils;
using Titan;

public partial class Controls_Login : System.Web.UI.UserControl
{
    private bool ReactivationEnabled = true;
    private Member _user;
    private Member User
    {
        get
        {
            if (_user == null)
                _user = new Member(Username.Text);
            return _user;
        }
        set
        {
            _user = value;
        }
    }   

    protected void Page_Load(object sender, EventArgs e)
    {
        if (AppSettings.Authentication.LoginUsingEmail)        
            Username.Attributes.Add("PlaceHolder", "Email");        
        else        
            Username.Attributes.Add("PlaceHolder", L1.USERNAME);        

        if (AppSettings.Authentication.ResetPasswordAndPinTogether)
            ForgotMyPasswordHyperLink.Text = L1.IFORGOTPWD + "/PIN";
        else
            ForgotMyPasswordHyperLink.Text = L1.IFORGOTPWD;

        //Facebook OAuth
        if (Request.QueryString["fb"] != null && Request["accessToken"] != null)
        {
            var accessToken = Request["accessToken"];
            Session["AccessToken"] = accessToken;
            try
            {
                FacebookMember User = new FacebookMember(accessToken);
                TitanAuthService.LoginOrRegister(User);
            }
            catch (MsgException ex)
            {
                FailureP.Visible = true;
                FailureText.Text = ex.Message;
                FormsAuthentication.SignOut();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw ex;
            }
        }

        //Demo autofill
        if (AppSettings.IsDemo)
        {
            Username.Text = "demo";
            Password.Text = "demopassword";
            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "document.getElementById('" + Password.ClientID + "').value = 'demopassword';", true);
        }

        //General autofill
        if (Request.QueryString["username"] != null && Request.QueryString["password"] != null)
        {
            string username = HttpUtility.UrlDecode(Request.QueryString["username"]);
            string password = HttpUtility.UrlDecode(Request.QueryString["password"]);

            Username.Text = username;
            Password.Text = password;

            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "document.getElementById('" + Password.ClientID + "').value = '" + password + "';", true);
        }

        //Add label & hint translations 
        HintAdder.Add(Password2, L1.LEAVEBLANKIFNOTPWD2);
        LangAdder.Add(LoginButton, U4000.LOGINTEXT);
        LangAdder.Add(CustomValidator1, L1.ER_BADCAPTCHA);
        LoginUserValidationSummary.HeaderText = L1.ER_ALLFIELDSREQUIRED;

        //Check wheather we should request Captcha (two bad logins trials)
        if (MemberAuthenticationService.GetBadLoginTrials(Context) > 1)
        {
            CaptchaPanel1.Visible = true;
            LoginUserValidationSummary.HeaderText = L1.ER_ALLFIELDSREQUIRED2;
        }

        if (Request.QueryString["afterregister"] != null && Convert.ToInt32(Request.QueryString["afterregister"]) == 1)
        {
            ShowResendActivationControls(true);
            ResendEmailButton.Visible = false;            
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

    protected void LoginButton_Click(object sender, EventArgs e)
    {       
        if (Page.IsValid)
        {
            try
            {                
                TitanAuthService.Login(Username.Text, Password.Text, Password2.Text);               
            }
            catch (SpecialException ex)
            {
                //Account inctive
                if (ReactivationEnabled)
                {
                    ReactivateButton.Visible = true;
                    LoginButton.Visible = false;
                }
                FailureP.Visible = true;
                FailureText.Text = ex.Message;
                FormsAuthentication.SignOut();
            }
            catch (MsgException ex)
            {
                FailureP.Visible = true;
                FailureText.Text = ex.Message;
                FormsAuthentication.SignOut();

                if (ex.Message == L1.ACCNOTACTIVATED)
                {
                    FailureText.Text += " " + U6006.CLICKBUTTONBELOW;
                    ShowResendActivationControls();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw ex;
            }
        }
    }

    protected void ReactivateButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            try
            {
                TitanAuthService.Reactivate(Username.Text, Password.Text, Password2.Text);
            }
            catch (MsgException ex)
            {
                FailureP.Visible = true;
                FailureText.Text = ex.Message;                
                FormsAuthentication.SignOut();                
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw ex;
            }
        }
    }

    protected void ResendEmailButton_Click(object sender, EventArgs e)
    {
        try
        {
            Mailer.SendActivationLink(User.Name, User.Email);
            ResendEmailButton.Visible = false;
            FailureP.Visible = false;
            SuccesP.Visible = true;
            SuccesText.Text = L1.ST_REGISTER2;
        }
        catch (MsgException ex)
        {
            FailureP.Visible = true;
            FailureText.Text = ex.Message;
            FormsAuthentication.SignOut();
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            throw ex;
        }        
    }

    public void ShowResendActivationControls(bool showInfo = false)
    {
        FacebookLogin1.Visible = false;
        LoginButton.Visible = false;
        ResendEmailButton.Visible = true;
        ResendEmailButton.Text = U6006.RESENDACTIVATIONLINK;

        if (showInfo)
        {
            FailureP.Visible = true;
            FailureText.Text = L1.ACCNOTACTIVATED;
        }
    }
}