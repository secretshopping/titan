using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Prem.PTC.Referrals;
using Prem.PTC.Payments;
using Prem.PTC.Advertising;
using Resources;
using System.Drawing;
using System.Text;

public partial class Testimonials : System.Web.UI.Page
{
    string validURL;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.TestimonialsEnabled || !Member.IsLogged);

        if (!Page.IsPostBack)
        {
            this.DataBind();
        }

        LangAdder.Add(PasswordRequired, L1.TEXT + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(RequiredFieldValidator1, U5008.SIGNATURE + " " + U3900.FIELDISREQUIRED, true);
    }

    public override void DataBind()
    {
        base.DataBind();
        AddTexts();
    }

    private void AddTexts()
    {
        CreateTestimonialButton.Text = L1.SEND;
    }
  
    private void ClearAll()
    {
        BodyTextBox.Text = string.Empty;
        SignatureTextBox.Text = string.Empty;
    }

    protected void CreateTestimonialButton_Click(object sender, EventArgs e)
    {
        try
        {
            ErrorMessagePanel.Visible = SuccMessagePanel.Visible = false;

            if (!Member.IsLogged)
                AccessManager.RedirectIfDisabled(false);

            string body = InputChecker.HtmlEncode(BodyTextBox.Text, BodyTextBox.MaxLength, L1.TEXT);
            string signature = InputChecker.HtmlEncode(SignatureTextBox.Text, SignatureTextBox.MaxLength, U5008.SIGNATURE);

            Testimonial.Add(Member.CurrentId, body, signature);
            ClearAll();
            SuccMessagePanel.Visible = true;
            SuccMessage.Text = U5008.THANKSFORFEEDBACK;
        }
        catch (Exception ex)
        {
            if (ex is MsgException)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
            else
                ErrorLogger.Log(ex);
        }
    }
}
