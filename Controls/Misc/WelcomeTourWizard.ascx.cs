using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;
using System.Text;
using Prem.PTC.Advertising;
using System.Globalization;
using Titan.WelcomeTour;

public partial class Controls_WelcomeTourWizard : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Member.IsLogged && AppSettings.TitanFeatures.QuickStartGuideEnabled && !Member.CurrentInCache.IsQuickGuideViewed)
        {
            if(HttpContext.Current.Request.Cookies.Get("QuickStartGuideHideInDemo") == null)
                QuickStartGuidePlaceHolder.Visible = true;
        }
            

        if (!IsPostBack && Request["QuickStartGuide"] == "hide")
        {
            if (!AppSettings.IsDemo)
                HideForeverQuickStartGuideForCurrentUser();
            else
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("QuickStartGuideHideInDemo"));
        }
    }

    public String GetWelcomeTourJsonString()
    {
        return WelcomeTourStepManager.WelcomeTourJsonString();
    }

    private void HideForeverQuickStartGuideForCurrentUser()
    {
        Member.CurrentInCache.IsQuickGuideViewed = true;
        Member.CurrentInCache.Save();
    }
}
