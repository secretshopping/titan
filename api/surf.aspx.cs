using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Prem.PTC.Referrals;
using Resources;
using Titan;
using System.Net;
using System.Reflection;
using Prem.PTC.Utils;
using Newtonsoft.Json;
using System.Web.Services;
using Titan.Publish.PTCOfferWalls;

public partial class Api_Surf : System.Web.UI.Page
{
    protected string JsonData { get; set; }
    protected bool SuccessfulCaptcha { get; set; }
    #region Session Properties
    int publishersWebsiteId
    {
        get
        {
            if (Session["Surf_PublishersWebsiteId"] == null)
                Session["Surf_PublishersWebsiteId"] = -1;
            return Convert.ToInt32(Session["Surf_PublishersWebsiteId"]);
        }
        set
        {
            Session["Surf_PublishersWebsiteId"] = value;
        }
    }
    int ptcOfferWallId
    {
        get
        {
            if (Session["Surf_PtcOfferWallId"] == null)
                Session["Surf_PtcOfferWallId"] = -1;
            return Convert.ToInt32(Session["Surf_PtcOfferWallId"]);
        }
        set
        {
            Session["Surf_PtcOfferWallId"] = value;
        }
    }

    protected string ptcOfferWallTitle
    {
        get
        {
            return Session["Surf_PtcOfferWallTitle"].ToString();
        }
        set
        {
            Session["Surf_PtcOfferWallTitle"] = value;
        }
    }

    string externalUsername
    {
        get
        {
            return Session["Surf_ExternalUsername"].ToString();
        }
        set
        {
            Session["Surf_ExternalUsername"] = value;
        }
    }

    string subId2
    {
        get
        {
            return Session["Surf_SubId2"].ToString();
        }
        set
        {
            Session["Surf_SubId2"] = value;
        }
    }

    string subId3
    {
        get
        {
            return Session["Surf_SubId3"].ToString();
        }
        set
        {
            Session["Surf_SubId3"] = value;
        }
    }

    string countryCode
    {
        get
        {
            return Session["Surf_CountryCode"].ToString();
        }
        set
        {
            Session["Surf_CountryCode"] = value;
        }
    }
    Gender gender
    {
        get
        {
            return (Gender)Enum.Parse(typeof(Gender), Session["Surf_Gender"].ToString());
        }
        set
        {
            Session["Surf_Gender"] = value;
        }
    }

    int? age
    {
        get
        {
            if (Session["Surf_Age"] == null)
                return null;
            return Convert.ToInt32(Session["Surf_Age"]);
        }
        set
        {
            Session["Surf_Age"] = value;
        }
    }


    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        Form.Action = Request.RawUrl;
        if (!IsPostBack)
            Initialize();
        else
            JsonData = "1";
    }

    private void Initialize()
    {
        externalUsername = Request.Form["__SubId"];
        ptcOfferWallTitle = Request.Form["__OfferWallTitle"];
        subId2 = Request.Form["__SubId2"];
        subId3 = Request.Form["__SubId3"];
        countryCode = Request.Form["__CountryCode"];

        Gender tempGender;

        if (!Enum.TryParse(Request.Form["__Gender"], true, out tempGender))
            gender = Gender.Null;
        else
            gender = tempGender;

        age = Request.Form["__Age"].ToNullableInt();

        int tempPtcOfferWallId;
        int temppublishersWebsiteId;

        if (!int.TryParse(Request.Form["__OfferWallId"], out tempPtcOfferWallId) || !int.TryParse(Request.Form["__PublishersWebsiteId"], out temppublishersWebsiteId)
            || string.IsNullOrEmpty(externalUsername) || string.IsNullOrEmpty(ptcOfferWallTitle))
            HandleError();

        ptcOfferWallId = Convert.ToInt32(Request.Form["__OfferWallId"]);
        publishersWebsiteId = Convert.ToInt32(Request.Form["__PublishersWebsiteId"]);

        LangAdder.Add(CreditAfterCaptchaButton, U6003.DONE);
        LangAdder.Add(CustomValidator1, L1.ER_BADCAPTCHA);

        PTCOfferWall offerWall = null;
        try
        {
            offerWall = new PTCOfferWall(ptcOfferWallId);
        }
        catch
        {
            HandleError();
        }
        var urls = PTCOfferWallManager.GetUserUrls(ptcOfferWallId).Select(u => u.Url).ToList();

        if (urls.Count == 0)
            HandleError();

        LoadAds(urls, offerWall);
    }

    protected void CreditAfterCaptcha()
    {
        try
        {
            var submission = new PTCOfferWallSubmisson(ptcOfferWallId, ptcOfferWallTitle, publishersWebsiteId, externalUsername, subId2, subId3, IP.Current, countryCode, gender, age);
            submission.Credit();
            SuccessfulCaptcha = true;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            ErrorLogger.Log("api/Surf, CreditAfterCaptcha: " + ex.Message, LogType.Publisher);
        }
        finally
        {
            ClearSession();
        }
    }

    private void ClearSession()
    {
        Session["Surf_PublishersWebsiteId"] = null;
        Session["Surf_PtcOfferWallId"] = null;
        Session["Surf_PtcOfferWallTitle"] = null;
        Session["Surf_ExternalUsername"] = null;
        Session["Surf_SubId2"] = null;
        Session["Surf_SubId3"] = null;
        Session["Surf_CountryCode"] = null;
        Session["Surf_Gender"] = null;
        Session["Surf_Age"] = null;
    }

    protected void CreditAfterCaptcha_Click(object sender, EventArgs e)
    {
        CaptchaPanel.Style["display"] = "block";
        RegisterUserValidationSummary.Visible = true;

        if (TitanCaptcha.IsValid)
        {
            CreditAfterCaptcha();
            CaptchaPanel.Style["display"] = "none";

        }
    }

    void LoadAds(List<string> urls, PTCOfferWall offerWall)
    {
        JsonData = JsonConvert.SerializeObject(new { offerWall.Title, offerWall.Description, offerWall.DisplayTime, offerWall.AutosurfEnabled, urls });
    }

    void HandleError()
    {
        //To Do: show some error page
        Response.Redirect(AppSettings.Site.Url);
    }

    public static string BaseUrl
    {
        get
        {
            HttpContext context = HttpContext.Current;
            string baseUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + context.Request.ApplicationPath.TrimEnd('/') + '/';
            return baseUrl;
        }
    }

    protected void Validate_Captcha(object source, ServerValidateEventArgs args)
    {
        args.IsValid = TitanCaptcha.IsValid;
    }
}
