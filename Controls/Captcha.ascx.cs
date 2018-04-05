using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;
using System.Text;
using Prem.PTC.Advertising;
using System.Resources;
using hbehr.recaptcha;
using Prem.PTC.Utils;
using VisualCaptcha;

public partial class Controls_Captcha : System.Web.UI.UserControl
{
    public string ValidationGroup { get; set; } //Required for Titan Captcha
    public string ValidatorDisplay { get; set; }

    public string SolveMediaURL
    {
        get
        {
            if (HttpContext.Current.Request.IsSecureConnection)
                return "https://api-secure.solvemedia.com";

            return "http://api.solvemedia.com";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            switch (ProperCaptchaType)
            {
                case CaptchaType.GoogleCaptcha:
                    ReCaptcha.Configure(AppSettings.Captcha.ReCAPTCHASiteKey, AppSettings.Captcha.ReCAPTCHASecretKey);
                    CaptchaLiteral.Text = ReCaptcha.GetCaptcha(ReCaptchaLanguage.EnglishUs).ToHtmlString();
                    IsGoogleCaptchaOk = 0;
                    break;

                case CaptchaType.Titan:
                    TitanCaptchaPlaceHolder.Visible = true;
                    CaptchaCheckedCustomValidator.ValidationGroup = ValidationGroup;
                    CaptchaCheckedCustomValidator.Display = System.Web.UI.WebControls.ValidatorDisplay.None;
                    CaptchaCheckedCustomValidator.ErrorMessage = U6000.REQCAPTCHA;
                    break;

                case CaptchaType.SolveMedia:
                    SolveMediaPlaceHolder.Visible = true;
                    IsSolveMediaCaptchaOk = 0;
                    break;

                case CaptchaType.Coinhive:
                    CoinhivePlaceHolder.Visible = true;
                    IsCoinhiveCaptchaOk = 0;
                    break;
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }


    public bool IsValid
    {
        get
        {
            switch (ProperCaptchaType)
            {
                case CaptchaType.GoogleCaptcha:
                    if (IsGoogleCaptchaOk == 0)
                    {
                        string userResponse = (string)HttpContext.Current.Request.Form["g-recaptcha-response"];
                        bool validCaptcha = ReCaptcha.ValidateCaptcha(userResponse);
                        IsGoogleCaptchaOk = validCaptcha ? 1 : 2;
                    }
                    return IsGoogleCaptchaOk == 1;

                case CaptchaType.Titan:
                    return CaptchaHelper.Verify(HttpContext.Current.Request.Params["captcha-value"]);

                case CaptchaType.SolveMedia:
                    if (IsSolveMediaCaptchaOk == 0)
                    {
                        IsSolveMediaCaptchaOk = SolveMedia.Verify(Request.Form["adcopy_response"], Request.Form["adcopy_challenge"]) ? 1 : 2;
                    }
                    return IsSolveMediaCaptchaOk == 1;

                case CaptchaType.Coinhive:
                    if (IsSolveMediaCaptchaOk == 0)
                    {
                        IsSolveMediaCaptchaOk = Coinhive.Verify(HttpContext.Current.Request.Params["coinhive-token"]) ? 1 : 2;
                    }
                    return true;
            }


            return false; //for compilator
        }
    }

    public string TitanCaptchaJavascript
    {
        get
        {
            return "";
            // return ((Literal)TitanCaptchaJavascriptPlaceHolder.Controls[0]).Text;
        }
    }

    private CaptchaType ProperCaptchaType
    {
        get
        {
            if (AppSettings.Captcha.AllowMembersToChooseCaptcha && Member.IsLogged)
            {
                //Members can have their own type
                return Member.CurrentInCache.SelectedCaptchaType;
            }
            return AppSettings.Captcha.Type;
        }
    }

    /// <summary>
    /// 0 = not typed, 1 = OK, 2 = bad
    /// </summary>
    private int IsGoogleCaptchaOk
    {
        get
        {
            if (Session["Captcha1Ok"] == null)
                return 0;
            return (int)Session["Captcha1Ok"];
        }
        set
        {
            Session["Captcha1Ok"] = value;
        }
    }

    /// <summary>
    /// 0 = not typed, 1 = OK, 2 = bad
    /// </summary>
    private int IsSolveMediaCaptchaOk
    {
        get
        {
            if (Session["Captcha2Ok"] == null)
                return 0;
            return (int)Session["Captcha2Ok"];
        }
        set
        {
            Session["Captcha2Ok"] = value;
        }
    }

    /// <summary>
    /// 0 = not typed, 1 = OK, 2 = bad
    /// </summary>
    private int IsCoinhiveCaptchaOk
    {
        get
        {
            if (Session["Captcha3Ok"] == null)
                return 0;
            return (int)Session["Captcha3Ok"];
        }
        set
        {
            Session["Captcha3Ok"] = value;
        }
    }
}
