using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan.Publisher;

public partial class externalcpaoffer : System.Web.UI.Page
{
    int publishersWebsiteId { get; set; }
    public string externalUserName { get; set; }
    bool isAuthorized { get; set; }
    string subId2 { get; set; }
    string subId3 { get; set; }
    public string countryCode { get; set; }
    ExternalOfferWallsManager externalOfferWallsManager { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        int _publishersWebsiteId;

        bool IsPreview = Request.UrlReferrer != null && Request.Url.Host == new Uri(AppSettings.Site.Url).Host;

        isAuthorized = Request.UrlReferrer != null 
            && (Request.UrlReferrer.Host != Request.Url.Host 
               || IsPreview)
            && Request.QueryString[GlobalPostback.Parameters.SubId] != null 
            && int.TryParse(Request.QueryString[GlobalPostback.Parameters.PublishersWebsiteId], out _publishersWebsiteId);

        if (!isAuthorized)
        {
            HandleUnauthorizedRequest();
            return;
        }

        PemissionErrorPlaceHolder.Visible = false;
        OffersPlaceHolder.Visible = true;

        publishersWebsiteId = Convert.ToInt32(Request.QueryString[GlobalPostback.Parameters.PublishersWebsiteId]);
        externalUserName = Request.QueryString[GlobalPostback.Parameters.SubId];
        subId2 = Request.QueryString[GlobalPostback.Parameters.SubId2];
        subId3 = Request.QueryString[GlobalPostback.Parameters.SubId3];
        countryCode = new CountryInformation(IP.Current).CountryCode;

        externalOfferWallsManager = new ExternalOfferWallsManager(Request.UrlReferrer.Host, publishersWebsiteId, countryCode, IsPreview);

        if(externalOfferWallsManager.PublishersWebsite == null)
        {
            ErrorLogger.Log(string.Format("Requested Offers on website that has not been accepted ({0})", Request.UrlReferrer.Host), LogType.Publisher);
            HandleUnauthorizedRequest();
            return;
        } 
        LoadOffers();
    }

    void HandleUnauthorizedRequest()
    {
        PemissionErrorPlaceHolder.Visible = true;
        OffersPlaceHolder.Visible = false;
    }

    void LoadOffers()
    {
        var cpaOffers = externalOfferWallsManager.CpaOffers;

        OffersPlaceHolder.Controls.Clear();
        foreach (var offer in cpaOffers)
        {
            UserControl offerControl = (UserControl)Page.LoadControl("~/Controls/Advertisements/ExternalCpaOffer.ascx");
            PropertyInfo myProp = offerControl.GetType().GetProperty("CpaOffer");
            myProp.SetValue(offerControl, offer, null);

            offerControl.DataBind();
            OffersPlaceHolder.Controls.Add(offerControl);

            ((IExternalCpaOfferControl)offerControl).SubmitButtonClicked += CpaOfferControl_SubmitButtonClicked;
        }
    }

    private void CpaOfferControl_SubmitButtonClicked(object sender, ExternalCpaOfferEventArgs e)
    {
        ExternalCpaOfferSubmission.Create(externalOfferWallsManager.PublishersWebsite, e.CPAOffer.Id, externalUserName, e.LoginId, e.EmailId, subId2, subId3, IP.Current, countryCode, Money.Zero);
    }
}