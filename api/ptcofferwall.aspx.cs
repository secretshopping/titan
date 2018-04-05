using Prem.PTC;
using Prem.PTC.Utils;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan.Publish.PTCOfferWalls;
using Titan.Publisher;

public partial class ptcofferwall : System.Web.UI.Page
{
    protected string ExternalUserName { get; set; }
    protected int PublishersWebsiteId { get; set; }
    bool isAuthorized { get; set; }
    protected string SubId2 { get; set; }
    protected string SubId3 { get; set; }
    protected string Age { get; set; }
    protected string Gender { get; set; }
    protected string CountryCode { get; set; }

    
    PTCOfferWallManager ptcOfferWallManager { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AddLang();


            int publishersWebsiteId;
            isAuthorized = Request.UrlReferrer != null && Request.UrlReferrer.Host != Request.Url.Host && Request.QueryString[GlobalPostback.Parameters.SubId] != null
                && int.TryParse(Request.QueryString[GlobalPostback.Parameters.PublishersWebsiteId], out publishersWebsiteId);
            //isAuthorized = true;
            if (!isAuthorized)
            {
                HandleUnauthorizedRequest();
                return;
            }

            PublishersWebsiteId = Convert.ToInt32(Request.QueryString[GlobalPostback.Parameters.PublishersWebsiteId]);
            ExternalUserName = Request.QueryString[GlobalPostback.Parameters.SubId];
            SubId2 = Request.QueryString[GlobalPostback.Parameters.SubId2];
            SubId3 = Request.QueryString[GlobalPostback.Parameters.SubId3];
            Age = Request.QueryString[GlobalPostback.Parameters.Age] ?? "Unknown";
            CountryCode = new CountryInformation(IP.Current).CountryCode;

            var host = Request.UrlReferrer.Host;
            //host = "usetitan.com";
            ptcOfferWallManager = new PTCOfferWallManager(host,
                                                           CountryCode,
                                                            ExternalUserName,
                                                            PublishersWebsiteId,
                                                            Request.Browser.IsMobileDevice,
                                                            Age,
                                                            Request.QueryString[GlobalPostback.Parameters.Gender]);

            if (ptcOfferWallManager.PublishersWebsite == null)
            {
                ErrorLogger.Log(string.Format("Requested Ptc Offerwall on website that has not been accepted ({0})", Request.UrlReferrer.Host), LogType.Publisher);
                HandleUnauthorizedRequest();
                return;
            }

            Session["PtcOfferWallManager"] = ptcOfferWallManager;

            Gender = ptcOfferWallManager.Gender.ToString();
            LoadOfferWalls();
        }
    }

    private void AddLang()
    {
        ReloadOfferWallsButton.Text = "<span class='fa fa-refresh'></span>";
    }

    void HandleUnauthorizedRequest()
    {
        // TO DO: show some error page
        this.Visible = false;
    }

    void LoadOfferWalls()
    {
        var offerWalls = ptcOfferWallManager.GetOfferWalls();
        if (offerWalls.Count == 0)
        {
            HandleUnauthorizedRequest();
            return;
        }

        OffersPlaceHolder.Controls.Clear();
        foreach (var offer in offerWalls)
        {
            UserControl offerControl = (UserControl)Page.LoadControl("~/Controls/Advertisements/PtcOfferWall.ascx");
            PropertyInfo myProp = offerControl.GetType().GetProperty("OfferWall");
            myProp.SetValue(offerControl, offer, null);

            offerControl.DataBind();
            OffersPlaceHolder.Controls.Add(offerControl);
        }
    }

    protected void PtcOfferWallsUpdatePanel_Load(object sender, EventArgs e)
    {
        if (IsPostBack && Session["PtcOfferWallManager"] as PTCOfferWallManager != null)
        {
            ptcOfferWallManager = (PTCOfferWallManager)Session["PtcOfferWallManager"];
            LoadOfferWalls();
        }
    }
}
