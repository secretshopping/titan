using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.API
{
    public class ApiApplicationSettings
    {
        public List<ApiPage> pages { get; set; }

        public bool facebookLoginEnabled { get; set; }
        public string facebookApplicationID { get; set; }

        public string currencyCode { get; set; }
        public string currencySign { get; set; }
        public int currencyDecimalPlaces { get; set; }

        public ApiApplicationSettings()
        {
            //Pages
            List<MobileAppPage> allPages = MobileAppPagesHelper.GetList();
            pages = new List<ApiPage>();

            foreach (var page in allPages)
                pages.Add(new ApiPage(page));

            //Facebook Login
            facebookLoginEnabled = AppSettings.Authentication.IsFacebookEnabled;
            facebookApplicationID = AppSettings.Facebook.ApplicationId;

            //Currency
            currencyCode = AppSettings.Site.CurrencyCode;
            currencySign = AppSettings.Site.CurrencySign;
            currencyDecimalPlaces = CoreSettings.GetMaxDecimalPlaces();
        }
    }
}