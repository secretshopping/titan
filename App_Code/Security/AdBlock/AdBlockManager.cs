using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Prem.PTC;
using Prem.PTC.Members;

public class AdBlockManager
{
    // Page to be redirected to, if AdBlock is enabled
    private static readonly string AdBlockRedirectPage = "~/sites/adblock.aspx";

    // Page to be redirected to (from AdBlockRedirectPage), if AdBlock has been turned off
    private static readonly string AdBlockDisabledRedirectPage = "~/user/default.aspx";

    public static bool IsAdBlockedFeatureEnabled
    {
        get
        {
            bool result = false;

            //Earn
            result |= AppSettings.TitanFeatures.EarnAdsEnabled;
            result |= AppSettings.TitanFeatures.EarnOfferwallsEnabled;
            result |= AppSettings.TitanFeatures.EarnSearchEnabled;
            result |= AppSettings.TitanFeatures.EarnTrafficExchangeEnabled;
            result |= AppSettings.TitanFeatures.EarnTrafficGridEnabled;

            //Publish
            result |= AppSettings.TitanFeatures.PublishOfferWallsEnabled;
            result |= AppSettings.TitanFeatures.PublishPTCOfferWallsEnabled;

            //Revenue sharing
            result |= AppSettings.TitanFeatures.AdvertAdPacksEnabled;
            result |= AppSettings.TitanFeatures.EarnAdPacksEnabled;

            return result;
        }
    }

    public static void CheckAccessRights()
    {
        CheckDenyForLoggedIn();
        CheckDenyForAll();
    }

    public static void CheckDenyForAll()
    {
        if(AppSettings.Site.AdBlockPolicy == AdBlockPolicy.DenyAccessForAll)
            BlockAccesstoThisPage(); 
    }

    public static void CheckDenyForLoggedIn()
    {
        if (AppSettings.Site.AdBlockPolicy == AdBlockPolicy.DenyAccessForLoggedIn && Member.IsLogged)
            BlockAccesstoThisPage();
    }

    /// <summary>
    /// Run in Page_Load
    /// </summary>
    public static void BlockAccesstoThisPage()
    {
        if (TitanFeatures.IsJ5Walter)
            return;

        Page currentPage = (HttpContext.Current.Handler as Page);

        //Do not redirect from landing page
        if (HttpContext.Current.Request.Url.AbsoluteUri == new Uri(currentPage.Request.Url, currentPage.ResolveUrl(AdBlockRedirectPage)).AbsoluteUri)
            return;

        currentPage.ClientScript.RegisterStartupScript(currentPage.GetType(), "AdBlockBlocker",
            @"

            // Function called if AdBlock is detected
            function adBlockDetected() {
                window.location.href = '" + currentPage.ResolveUrl(AdBlockRedirectPage) + @"';
            }

            function adBlockNotDetected() {
            }
         
            " + HelperJSCode, true);
    }

    /// <summary>
    /// Run in Page_Load on AdBlockRedirectPage page (default ~/adblock.aspx)
    /// </summary>
    public static void RunOnAdBlockRedirectPage()
    {
        Page currentPage = (HttpContext.Current.Handler as Page);

        currentPage.ClientScript.RegisterStartupScript(currentPage.GetType(), "AdBlockUnblocker",
            @"

            // Function called if AdBlock is disabled
            function adBlockNotDetected() {
                window.location.href = '" + currentPage.ResolveUrl(AdBlockDisabledRedirectPage) + @"';
            }

            function adBlockDetected() {
                console.log('adblock enabled');
            }

            " + HelperJSCode, true);
    }

    private static string HelperJSCode
    {
        get
        {
            return @"            
            // Recommended audit because AdBlock lock the file 'blockadblock.js' 
            // If the file is not called, the variable does not exist 'blockAdBlock'
            // This means that AdBlock is present

            if(typeof blockAdBlock === 'undefined') {
                adBlockDetected();
            } else {
                blockAdBlock.onDetected(adBlockDetected);
                blockAdBlock.onNotDetected(adBlockNotDetected);
                // and|or
                blockAdBlock.on(true, adBlockDetected);
                blockAdBlock.on(false, adBlockNotDetected);
                // and|or
                blockAdBlock.on(true, adBlockDetected).onNotDetected(adBlockNotDetected);
            }";
        }
    }
}