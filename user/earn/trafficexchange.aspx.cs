using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Prem.PTC;
using Prem.PTC.Advertising;
using System.Reflection;

public partial class About : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnersRoleEnabled && 
            AppSettings.TitanFeatures.EarnTrafficExchangeEnabled
            && Member.CurrentInCache.IsEarner);
        Form.Action = Request.RawUrl;
        Member user = Member.Current;
        var numberOfAvailableAds = TrafficExchangeManager.GetAdsAvailableForUser(user).Count;
        if (numberOfAvailableAds > 0)
        {
            AdsAvailablePlaceholder.Visible = true;
            AdsUnavailablePlaceholder.Visible = false;
        }
        else
        {
            AdsAvailablePlaceholder.Visible = false;
            AdsUnavailablePlaceholder.Visible = true;
        }
    }

   
}
