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
using Prem.PTC.Texts;
using Prem.PTC.Offers;

public partial class Controls_ExternalCpaOffer : System.Web.UI.UserControl, IExternalCpaOfferControl
{
    public CPAOffer CpaOffer { get; set; }
    public string Text { get; set; }
    public string Sender { get; set; }
    public string OfferInfoTableCssClass { get; set; }

    public event CpaEventHandler SubmitButtonClicked;

    public override void DataBind()
    {
        base.DataBind();

        SubmitButton.Text = L1.SUBMIT;

        //Visibility
        LoginIDPlaceHolder.Visible = CpaOffer.LoginBoxRequired;
        EmailIDPlaceHolder.Visible = CpaOffer.EmailBoxRequired;
        FieldsGroupPlaceHolder.Visible = CpaOffer.LoginBoxRequired && CpaOffer.EmailBoxRequired;
        //if (Request.Browser.IsMobileDevice)
        OfferInfoTableCssClass = FieldsGroupPlaceHolder.Visible ? "col-md-6" : "col-md-12";
    }

    public string OfferImageHTML
    {
        get
        {
            if (String.IsNullOrEmpty(CpaOffer.ImageURL))
                return AppSettings.Site.Url + "Images/Misc/placeholder.png";

            return CpaOffer.ImageURL.Replace("\\", "/").Replace("~/", AppSettings.Site.Url);
        }
    }

    public string Amount
    {
        get
        {
            Money amount = CpaOffer.GetAmount(Member.CurrentInCache.Membership.CPAProfitPercent);

            if (CpaOffer.GetFinalCreditAs() == CreditAs.Points)
                return PointsConverter.ToPoints(amount) + " " + AppSettings.PointsName;

            return amount.ToString();
        }
    }


    protected void SubmitButton_Click(object sender, EventArgs e)
    {
        if (SubmitButtonClicked != null)
        {
            SubmitButtonClicked(this,
                new ExternalCpaOfferEventArgs(CpaOffer,
                                              CpaOffer.LoginBoxRequired ? LoginIDTextBox.Text : null,
                                              CpaOffer.EmailBoxRequired ? EmailIDTextBox.Text : null));
        }
    }
}
