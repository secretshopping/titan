using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.API
{
    public class ApiPage
    {
        public int pageId { get; set; }
        public string pageName { get; set; }
        public bool enabled { get; set; }

        public ApiPage(MobileAppPage appPage)
        {
            pageId = (int)appPage;
            pageName = appPage.ToString();
            enabled = MobileAppPagesHelper.IsEnabled(appPage);
        }
    }
}