<%@ WebHandler Language="C#" Class="ShowAd" %>
using System;
using System.Web;
using Titan.Publisher.InTextAds;
using Titan.Publisher.Security;
using Prem.PTC;
using Prem.PTC.Members;
using Titan;
using Titan.Publisher;

public class ShowAd : IHttpHandler
{
    bool isAuthorized;
    PublishersWebsite publishersWebsite;
    InTextAdvert advert;
    HttpContext context;

    public void ProcessRequest(HttpContext context)
    {
        string publishersWebsiteId = context.Request.QueryString[GlobalPostback.Parameters.PublishersWebsiteId];

        if (!string.IsNullOrEmpty(publishersWebsiteId))
        {
            this.context = context;

            publishersWebsite = PublishersWebsite.GetActiveWebsite(context.Request.UrlReferrer.Host, Convert.ToInt32(publishersWebsiteId));

            isAuthorized = context.Request.QueryString["id"] != null && publishersWebsite != null;

            if (isAuthorized)
            {
                try
                {
                    var inTextAdvertId = Convert.ToInt32(Encryption.Decrypt(context.Request.QueryString["id"]));

                    advert = new InTextAdvert(inTextAdvertId);

                    var tracker = new ActionTrackerCreator<InTextActionTracker>(publishersWebsite.Id, context.Request.UserHostAddress, inTextAdvertId).GetOrCreate();

                    tracker.HandleAction(HandleSuccessfulClick);

                    context.Response.Redirect(advert.Url);
                }
                catch (System.Threading.ThreadAbortException)
                {
                    //DO NOTHING
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                    ErrorLogger.Log(ex.Message, LogType.Publisher);
                    context.Response.Redirect(AppSettings.Site.Url);
                }
            }
            else
                context.Response.Redirect(AppSettings.Site.Url);
        }
    }

    private void HandleSuccessfulClick()
    {
        var publisher = new Member(publishersWebsite.UserId);

        var crediter = new InTextAdCrediter(advert.PricePaid / advert.ClicksBought, publishersWebsite.UserId);
        var moneyLeftForPools = crediter.Credit();

        advert.AddClick();
    }
    public bool IsReusable
    {
        get { return false; }
    }
}