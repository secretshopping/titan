<%@ WebHandler Language="C#" Class="GetAds" %>
using System;
using System.Web;
using Titan.Publisher.InTextAds;
using Prem.PTC;
using Titan.Publisher;

public class GetAds : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {

        string publishersWebsiteId = context.Request.QueryString[GlobalPostback.Parameters.PublishersWebsiteId];
        if (!string.IsNullOrEmpty(publishersWebsiteId) && context.Request.UrlReferrer != null)
        {
            var publishersWebsite = PublishersWebsite.GetActiveWebsite(context.Request.UrlReferrer.Host, Convert.ToInt32(publishersWebsiteId));
            if (publishersWebsite == null)
                return;

            context.Response.ContentType = "application/javascript";
            context.Response.Write(GetJavaScript(publishersWebsiteId));
        }
    }
    public bool IsReusable
    {
        get { return false; }
    }

    private string GetJavaScript(string publishersWebsiteId)
    {
        string script = @"window.jQuery||document.write('<script src=\'https://code.jquery.com/jquery-1.12.4.min.js\' defer><\/script>');document.write('<script src=\'{1}Scripts/default/assets/plugins/bootstrap-popover/bootstrap-popover.min.js\' defer><\/script>');document.write('<script src=\'https://cdnjs.cloudflare.com/ajax/libs/mark.js/8.9.0/mark.min.js\' defer><\/script>');inTextAds={0};adUrl='{1}handlers/intext/showad.ashx?{2}&id=';document.write('<script src=\'{1}Scripts/assets/js/client.js\' defer><\/script>');";
        return string.Format(script, new InTextAdvertJsonCreator().GetAdsAsJson(), AppSettings.Site.Url, GlobalPostback.Parameters.PublishersWebsiteId+"="+publishersWebsiteId);
    }
}