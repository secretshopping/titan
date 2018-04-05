using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class Browser
{
    public static BrowserType Current
    {
        get
        {
            System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;

            string type = browser.Type.ToUpper();

            if (type.Contains("FIREFOX"))
                return BrowserType.Firefox;
            if (type.Contains("IE") || type.Contains("EXPLOR"))
                return BrowserType.IE;
            if (type.Contains("CHROME"))
                return BrowserType.Chrome;
            return BrowserType.Unknown;
        }
    }


    public static string GetBodyExtensionHTML(string buttontext, string url = "")
    {
        var browser = Browser.Current;
        
        switch (browser)
        {
            case BrowserType.Chrome:
                return "<a href='#'><div class=\"chromebutton\" onclick=\"chrome.webstore.install()\" id=\"install-button\">" + buttontext.Replace("%e%", browser.ToString()) 
                    + "</div></a><script>if (chrome.app.isInstalled) {  document.getElementById('install-button').style.display = 'none';}</script>";

            case BrowserType.Firefox:
                return "<a href='#'><div class=\"firefoxbutton\" onclick=\"return install(event);\">" + buttontext.Replace("%e%", browser.ToString())
                    + "</div></a>";

            case BrowserType.IE:
                return "<a href='" + url + "'><div class=\"iebutton\">" + buttontext.Replace("%e%", browser.ToString())
                    + "</div>";

            default:
                return "";
        }
    }

}