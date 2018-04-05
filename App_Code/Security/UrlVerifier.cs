using Prem.PTC;
using Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

public class UrlVerifier
{
    /// <summary>
    /// Checks the specified URL.
    /// </summary>
    /// <param name="Url">The URL.</param>
    public static void Check(string Url)
    {
        bool ok = CheckUrl(Url);
        if (!ok)
            throw new MsgException(Resources.U4200.URLCHECKBAD);
    }

    private static bool CheckUrl(string Url)
    {
        try
        {
            WebRequestUtils.SetUpSecurityProtocols();
            var request = WebRequest.Create(Url) as HttpWebRequest;

            using (var client = new MyWebClient())
            {
                client.DownloadString(Url);

                if (request == null)
                    return false;

                if (!AppSettings.Site.URLCheckerAllowRedirect && !UrlCreator.ExtractDomainUrl(Url).ToLower().Contains(UrlCreator.ExtractDomainUrl(client.ResponseUri.Host.ToLower())))                
                        throw new MsgException(U4200.URLCHECKBAD + ": Attempted Redirection");

                //var sourcePage = WebUtility.HtmlDecode(client.DownloadString(Url)).Trim();
                //sourcePage = sourcePage.Replace(" ", string.Empty);

                //if (sourcePage.Contains("top.location!=self.location"))
                //    throw new MsgException(Resources.U4200.URLCHECKBAD);

                //Some pages send wrong formatted x-frame-options in headers like 'deny' or 'DENY,DENY' so have to use ToLower() and Contains
                var xFrameHeader = client.ResponseHeaders["X-Frame-Options"] != null ? client.ResponseHeaders["X-Frame-Options"].ToLower() : null;

                if (xFrameHeader != null && (xFrameHeader.Contains("deny") || xFrameHeader.Contains("sameorigin")))
                    throw new MsgException(U6011.WEBSITEDONTALLOWIFRAME);
            }
            return true;
        }
        catch (UriFormatException ex)
        {
            //Invalid Url
            throw new MsgException(Resources.U4200.URLCHECKBAD + ". " + ex.Message);
        }
        catch (WebException ex)
        {
            //Unable to access url
            throw new MsgException(Resources.U4200.URLCHECKBAD + ". " + ex.Message);
        }
    }
    public static bool IsIframeValid(string url)
    {
        if (url.StartsWith("<iframe") && url.EndsWith("></iframe>"))
            return true;
        return false;
    }
}

class MyWebClient : WebClient
{
    Uri _responseUri;

    public MyWebClient() : base()
    {
        WebRequestUtils.SetUpSecurityProtocols();
    }

    public Uri ResponseUri
    {
        get { return _responseUri; }
    }

    protected override WebResponse GetWebResponse(WebRequest request)
    {
        WebResponse response = base.GetWebResponse(request);
        _responseUri = response.ResponseUri;
        return response;
    }
}