using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Resources;

public partial class user_earn_coinhiveclaim : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnCaptchaClaim);

        CheckCaptcha();

        if (!IsPostBack)
        {
            SetupMessagges();

            LangAdder.Add(MainTab, "Coinhive " + U4200.CLAIM.ToLower());
        }
    }

    private void SetupMessagges()
    {
        SuccessMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;

        string claimPrize = AppSettings.CaptchaClaim.CaptchaClaimPrize.ToString();
        claimPrize = Money.CutEndingZeros(claimPrize);

        if(AppSettings.CaptchaClaim.CaptchaClaimPrizeType == BalanceType.PointsBalance)
        {
            claimPrize += " " + AppSettings.PointsName;
        }
        else
        {
            claimPrize = Money.AddCurrencySign(claimPrize);
        }

        CoinhiveClaimDescription.Text = String.Format(U6013.CAPTCHACLAINDESCRIPTION, claimPrize);
        SuccessMessageLabel.Text = String.Format(U6013.CAPTCHACLAIMSUCCESS, claimPrize);
        ErrorMessageLabel.Text = U6013.CAPTCHACLAIMFAILURE;
    }

    private void CheckCaptcha()
    {
        CoinhiveClaim.IsValid += delegate (bool value)
        {
            SuccessMessagePanel.Visible = value;
            ErrorMessagePanel.Visible = !value;
        };
    }
}