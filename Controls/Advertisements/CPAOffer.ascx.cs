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

public partial class Controls_CPAOffer : System.Web.UI.UserControl, ICPAOfferObjectControl
{
    public CPAOffer Object { get; set; }
    public string Text { get; set; }
    public bool ShowReturnButton { get; set; }
    public string Sender { get; set; }
    public string OfferInfoTableClass { get; set; }

    public override void DataBind()
    {
        base.DataBind();

        if (AppSettings.CPAGPT.ReadOnlyModeEnabled)
        {
            IgnoreButton.Visible = false;
            SubmitButton.Visible = false;
            GoToOfferButton.Visible = false;
            RatingAndCreditingTimePlaceHolder.Visible = false;
        }

        if (AppSettings.CPAGPT.AutoApprovalEnabled)
            RatingAndCreditingTimePlaceHolder.Visible = false;

        if (TitanFeatures.IsJ5WalterFreebiesFromHome)
        {
            amountTr.Visible = false;
            amountDiv.Visible = false;
        }

        IgnoreButton.Text = L1.IGNORE;
        ReportMessage.Text = L1.WHATISPROBLEM;
        SubmitButton.Text = L1.SUBMIT;
        GoToOfferButton.Text = U6012.OPEN;
        ReportButton.Text = L1.REPORT;
        MakeReportButton.Text = L1.REPORT;
        CancelReportButton.Text = U4000.CANCEL;
        ReturnButton.Text = L1.MOVEBACK;

        GoToOfferButton.Attributes.Add("href", Object.GetTargetURL());
        GoToOfferButton.Attributes.Add("target", "_blank");

        IgnoreButton.OnClientClick = "MakeIgnore('" + Object.Id + "');";
        ReturnButton.OnClientClick = "MakeReturn('" + Object.Id + "', '" + Sender + "');";
        SubmitButton.OnClientClick = "MakeSubmit('" + Object.Id + "');";
        ReportButton.OnClientClick = "ShowReportTable('" + Object.Id + "'); return false;";
        CancelReportButton.OnClientClick = "HideReportTable('" + Object.Id + "'); return false;";
        MakeReportButton.OnClientClick = "MakeReport('" + Object.Id + "');";

        SubmitButton.CommandArgument = Object.Id.ToString();
        ReportButton.CommandArgument = Object.Id.ToString();
        MakeReportButton.CommandArgument = Object.Id.ToString();
        ReturnButton.CommandArgument = Object.Id.ToString();
        CancelReportButton.CommandArgument = Object.Id.ToString();
        IgnoreButton.CommandArgument = Object.Id.ToString();

        LoginID.ID += Object.Id;
        EmailID.ID += Object.Id;
        ReportMessage.ID += Object.Id;

        //Visibility
        LoginIDPlaceHolder.Visible = Object.LoginBoxRequired;
        EmailIDPlaceHolder.Visible = Object.EmailBoxRequired;
        FieldsGroupPlaceHolder.Visible = Object.LoginBoxRequired || Object.EmailBoxRequired;    

        OfferInfoTableClass = FieldsGroupPlaceHolder.Visible ? string.Format("col-md-6") : string.Format("col-md-12");

        if (ShowReturnButton)
            ReturnButton.Visible = true;

        if (String.IsNullOrEmpty(Text))
            ButtonsPlaceHolder.Visible = true;

        if (TitanFeatures.IsTami9191)
        {
            DateAddTr.Visible = false;
            CategoryTr.Visible = false;
            OfferRatingTr.Visible = false;
        }
    }

    public string OfferImageHTML
    {
        get
        {
            if (String.IsNullOrEmpty(Object.ImageURL))
                return "Images/Misc/placeholder.png";

            return Object.ImageURL.Replace("\\", "/").Replace("~/", "");
        }
    }

    public string Amount
    {
        get
        {
            Money amount = Object.GetAmount(Member.CurrentInCache.Membership.CPAProfitPercent);

            if (Object.GetFinalCreditAs() == CreditAs.Points)
                return PointsConverter.ToPoints(amount) + " " + AppSettings.PointsName;

            return amount.ToString();
        }
    }

}
